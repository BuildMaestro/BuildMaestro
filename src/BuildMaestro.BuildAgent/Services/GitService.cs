using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildMaestro.Shared.BuildAgent.Git;
using BuildMaestro.Shared.Models;
using System.IO;
using BuildMaestro.BuildAgent.Core;
using BuildMaestro.BuildAgent.Events;
using BuildMaestro.Service;
using BuildMaestro.Data.Models;

namespace BuildMaestro.BuildAgent.Services
{
    public class GitService : Disposable
    {
        private BuildAgentService BuildAgent { get; set; }
        private ShellCommandService ShellCommand { get; set; }

        public GitService(BuildAgentService buildAgent)
        {
            this.BuildAgent = buildAgent;
            this.ShellCommand = new ShellCommandService();
        }

        public ÚpdateWorkspaceResult ÚpdateWorkspace(BuildConfigurationModel buildConfiguration)
        {
            var applicationSettingService = new ApplicationSettingService();
            var workspace = applicationSettingService.GetSetting(ApplicationSettingKey.Workspace);

            if (!string.IsNullOrEmpty(workspace))
            {
                var buildConfigurationWorkspace = Path.Combine(workspace, buildConfiguration.Id.ToString(), "Repository");

                // Create workspace
                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspace);
                if (!Directory.Exists(buildConfigurationWorkspace))
                {
                    Directory.CreateDirectory(buildConfigurationWorkspace);
                }
                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspaceSuccess);


                var GitResultCode = GitServiceEventCode.Unknown;

                // Clone or Pull
                try
                {
                    GitResultCode = this.CheckForGitRepository(buildConfigurationWorkspace, buildConfiguration);
                }
                catch (Exception exception)
                {
                    File.WriteAllText(Path.Combine(buildConfigurationWorkspace, "log.txt"), exception.Message);
                }

                if (GitResultCode == GitServiceEventCode.GitCloningSuccess || GitResultCode == GitServiceEventCode.GitPullSuccess)
                {
                    // Check latest commit
                    try
                    {
                        GitResultCode = this.UpdateLatestCommit(buildConfigurationWorkspace, buildConfiguration);
                    }
                    catch (Exception exception)
                    {
                        File.WriteAllText(Path.Combine(buildConfigurationWorkspace, "log.txt"), exception.Message);
                    }
                }
            }

            return new ÚpdateWorkspaceResult { Success = true };
        }

        private GitServiceEventCode CheckForGitRepository(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var applicationSettingService = new ApplicationSettingService();
            var gitExecutable = applicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);

            GitServiceEventCode resultEventCode = GitServiceEventCode.Unknown;

            if (!string.IsNullOrEmpty(gitExecutable))
            {
                var gitStatusResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, gitExecutable, "status");

                if (!string.IsNullOrEmpty(gitStatusResult.Output))
                {
                    if (gitStatusResult.Output.ToLower().Contains("not a git repository"))
                    {
                        // Git clone
                        try
                        {
                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloning);

                            var cloneResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace,
                                                                                    gitExecutable,
                                                                                    string.Concat("clone ", buildConfiguration.GitRepository, " ."));

                            resultEventCode = GitServiceEventCode.GitCloningSuccess;

                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloningSuccess);
                        }
                        catch (Exception ex)
                        {
                            resultEventCode = GitServiceEventCode.GitCloningFailure;

                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloningFailure);
                        }
                    }
                    else
                    {
                        // Git pull
                        try
                        {
                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitPull);

                            var pullResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace,
                                                                                    gitExecutable,
                                                                                    "pull");

                            resultEventCode = GitServiceEventCode.GitPullSuccess;

                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitPullSuccess);
                        }
                        catch (Exception ex)
                        {
                            resultEventCode = GitServiceEventCode.GitPullFailure;

                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitPullFailure);
                        }
                    }
                }
            }

            return resultEventCode;
        }

        public GitServiceEventCode UpdateLatestCommit(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var applicationSettingService = new ApplicationSettingService();
            var gitExecutable = applicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            GitServiceEventCode resultEventCode = GitServiceEventCode.Unknown;

            resultEventCode = GitServiceEventCode.GitUpdatingLastCommit;
            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommit);

            if (!string.IsNullOrEmpty(gitExecutable))
            {
                var gitStatusResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, gitExecutable, "log -n 1 --date=format:%Y-%m-%dT%H:%M:%S");

                if (!string.IsNullOrWhiteSpace(gitStatusResult.Output))
                {
                    var buildConfigurationService = new BuildConfigurationService();
                    var gitCommit = this.GetGitCommitFromLogOutput(buildConfiguration, gitStatusResult.Output);

                    if (gitCommit != null && (buildConfiguration.LatestGitCommit == null || buildConfiguration.LatestGitCommit.Hash != gitCommit.Hash))
                    {
                        buildConfigurationService.SetGitCommit(buildConfiguration.Id, gitCommit);

                        var gitCommitService = new GitCommitService();
                        var gitCommitModel = gitCommitService.GetGitCommitModel(gitCommit);

                        resultEventCode = GitServiceEventCode.GitUpdatingLastCommitChanged;
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommitChanged, gitCommitModel);
                    }
                    else
                    {
                        resultEventCode = GitServiceEventCode.GitUpdatingLastCommitNoChange;
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommitNoChange);
                    }
                }
            }

            return resultEventCode;
        }

        private GitCommit GetGitCommitFromLogOutput(BuildConfigurationModel buildConfiguration, string output)
        {
            var commitAuthor = string.Empty;
            DateTime? commitDate = null;
            var commitHash = string.Empty;
            var lines = output.Split('\n');

            // Check readability of output;
            if (lines.Length < 5)
            {
                return null;
            }
            
            // Check for commit hash
            var hashLine = lines[0];
            if (hashLine.Length != 47 || hashLine.Split(' ').Length != 2)
            {
                return null;
            }
            else
            {
                commitHash = hashLine.Split(' ')[1];
            }

            // Check for commit author
            var authorLine = lines[1];
            var authorString = "author:";
            if (!authorLine.ToLower().StartsWith(authorString) || authorLine.Split(':').Length != 2)
            {
                return null;
            }
            else
            {
                commitAuthor = authorLine.Split(':')[1].Trim();
            }

            // Check for commit date
            var dateLine = lines[2];
            var dateString = "date:";
            if (!dateLine.ToLower().StartsWith(dateString) || dateLine.Length <= dateString.Length)
            {
                return null;
            }
            else
            {
                var commitDateTimeString = dateLine.Substring(dateString.Length + 1).Trim();
                var splittedDateTime = commitDateTimeString.Split('T');
                var commitDateSplitted = splittedDateTime[0].Split('-');
                var commitTimeSplitted = splittedDateTime[1].Split(':');

                commitDate = new DateTime(Convert.ToInt16(commitDateSplitted[0]),
                                          Convert.ToInt16(commitDateSplitted[1]),
                                          Convert.ToInt16(commitDateSplitted[2]),
                                          Convert.ToInt16(commitTimeSplitted[0]),
                                          Convert.ToInt16(commitTimeSplitted[1]),
                                          Convert.ToInt16(commitTimeSplitted[2]));
            }

            return new GitCommit
            {
                Author = commitAuthor,
                BuildConfigurationId = buildConfiguration.Id,
                DateTime = commitDate.Value,
                Hash = commitHash,
                Branch = buildConfiguration.GitBranch,
                Repository = buildConfiguration.GitRepository,
            };
        }
    }
}
