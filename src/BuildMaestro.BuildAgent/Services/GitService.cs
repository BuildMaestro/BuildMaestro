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
using BuildMaestro.BuildAgent.Models;
using System.Text;

namespace BuildMaestro.BuildAgent.Services
{
    public class GitService : Disposable
    {
        const string GIT_DATE_FORMAT = "format:%Y-%m-%dT%H:%M:%S";

        private ApplicationSettingService ApplicationSettingService { get; set; }
        private BuildAgentService BuildAgent { get; set; }
        private BuildConfigurationService BuildConfigurationService { get; set; }
        private GitCommitService GitCommitService  { get; set; }
        private ShellCommandService ShellCommand { get; set; }

        public GitService(BuildAgentService buildAgent)
        {
            this.ApplicationSettingService = new ApplicationSettingService();
            this.BuildAgent = buildAgent;
            this.BuildConfigurationService = new BuildConfigurationService();
            this.GitCommitService = new GitCommitService();
            this.ShellCommand = new ShellCommandService();
        }

        public InitializeWorkspaceResult InitializeWorkspace(BuildConfigurationModel buildConfiguration)
        {
            var result = new InitializeWorkspaceResult { Success = true };
            var workspace = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.Workspace);

            if (!string.IsNullOrEmpty(workspace))
            {
                var buildConfigurationWorkspace = Path.Combine(workspace, buildConfiguration.Id.ToString(), "Repository");

                // Check and/or create build configuration workspace
                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspace);
                try
                {
                    if (!Directory.Exists(buildConfigurationWorkspace))
                    {
                        Directory.CreateDirectory(buildConfigurationWorkspace);

                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspaceSuccess);
                    }
                    else
                    {
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspaceAlreadyExists);
                    }

                    result.WorkspaceDirectory = buildConfigurationWorkspace;
                }
                catch (Exception ex)
                {
                    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspaceFailure);

                    result = new InitializeWorkspaceResult { Success = false, Exception = ex };
                }
            }

            return result;
        }

        public InitializeGitRepositoryResult InitializeGitRepository(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var gitExecutable = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            var result = new InitializeGitRepositoryResult { Success = true };

            if (string.IsNullOrWhiteSpace(gitExecutable) || !File.Exists(gitExecutable))
            {
                result = new InitializeGitRepositoryResult { Success = false, Exception = new Exception("Git executable not found.") };
            }
            else
            {
                var gitStatusResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, gitExecutable, "status");

                if (!string.IsNullOrEmpty(gitStatusResult.Output) && gitStatusResult.Output.ToLower().Contains("not a git repository"))
                {
                    try
                    {
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloning);

                        var cloneResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace,
                                                                                gitExecutable,
                                                                                string.Concat("clone ", buildConfiguration.GitRepository, " ."));

                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloningSuccess);
                    }
                    catch (Exception ex)
                    {
                        result = new InitializeGitRepositoryResult { Success = false, Exception = ex };

                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitCloningFailure);
                    }
                }
            }

            return result;
        }

        public UpdateGitRepositoryResult UpdateGitRepository(string workspaceDirectory, BuildConfigurationModel buildConfiguration)
        {
            var gitExecutable = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            var result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Success };

            if (string.IsNullOrWhiteSpace(gitExecutable) || !File.Exists(gitExecutable))
            {
                result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Failure, Exception = new Exception("Git executable not found.") };
            }
            else
            {
                try
                {
                    // Fetch new commits
                    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitFetching);

                    var fetchResult = this.GitFetch(workspaceDirectory, gitExecutable);

                    if (fetchResult.Success)
                    {
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitFetchingSuccess);

                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMerging);

                        // If incoming commits
                        if (fetchResult.GitCommits != null && fetchResult.GitCommits.Any())
                        {
                            // If auto deploy
                            if (buildConfiguration.AutoDeploy)
                            {
                                if (string.IsNullOrWhiteSpace(buildConfiguration.AutoDeployTags))
                                {
                                    try
                                    {
                                        var mergeResult = this.ShellCommand.ExecuteShellCommand(workspaceDirectory, gitExecutable, "merge");

                                        if (mergeResult.Output.Contains("updating") && mergeResult.Output.Contains("fast-forward"))
                                        {
                                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessMerged);
                                            result.Success = UpdateGitRepositoryResultSuccesType.SuccessMerged;
                                        }
                                        else
                                        {
                                            // Investigate possible output scenarios
                                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessNotMerged);
                                            result.Success = UpdateGitRepositoryResultSuccesType.SuccessNotMerged;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingFailure);
                                        result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Failure, Exception = new Exception("Git merge error") };
                                    }
                                }
                                else
                                {
                                    var tags = buildConfiguration.AutoDeployTags.Split(',').ToList();

                                    tags = tags.Select(x => x.Trim()).ToList(); // Trim tags

                                    var latestAutoDeployCommit = fetchResult.GitCommits.OrderByDescending(o => o.DateTime).FirstOrDefault(x => tags.Any(tag => x.Message.Contains(tag)));

                                    if (latestAutoDeployCommit != null)
                                    {
                                        // Auto deploy, so merge changes
                                        try
                                        {
                                            var mergeResult = this.ShellCommand.ExecuteShellCommand(workspaceDirectory, gitExecutable, string.Concat("merge ", latestAutoDeployCommit.Hash));

                                            if (mergeResult.Output.ToLower().Contains("updating") && mergeResult.Output.ToLower().Contains("fast-forward"))
                                            {
                                                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessMerged);
                                                result.Success = UpdateGitRepositoryResultSuccesType.SuccessMerged;
                                            }
                                            else
                                            {
                                                // Investigate possible output scenarios
                                                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessNotMerged);
                                                result.Success = UpdateGitRepositoryResultSuccesType.SuccessNotMerged;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingFailure);
                                            result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Failure, Exception = new Exception("Git merge error") };
                                        }

                                    }
                                    else
                                    {
                                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessNotMerged);
                                        result.Success = UpdateGitRepositoryResultSuccesType.SuccessNotMerged;
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitMergingSucessNotMerged);
                            result.Success = UpdateGitRepositoryResultSuccesType.SuccessNotMerged;
                        }
                    }
                    else
                    {
                        this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitFetchingFailure);
                        result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Failure, Exception = new Exception("Git fetch error") };
                    }
                }
                catch (Exception ex)
                {
                    result = new UpdateGitRepositoryResult { Success = UpdateGitRepositoryResultSuccesType.Failure, Exception = ex };
                }
            }

            return result;
        }

        public GitServiceEventCode UpdateLatestCommit(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var gitExecutable = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            GitServiceEventCode resultEventCode = GitServiceEventCode.Unknown;

            resultEventCode = GitServiceEventCode.GitUpdatingLastCommit;
            this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommit);

            if (!string.IsNullOrEmpty(gitExecutable))
            {
                var gitLogResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, gitExecutable, string.Concat("log -n 1 --date=", GIT_DATE_FORMAT));
                var lines = gitLogResult.Output.Split('\n');
                var commit = this.GetGitCommitsFromLines(lines).Single();

                if (commit != null && (buildConfiguration.LatestGitCommit == null || buildConfiguration.LatestGitCommit.Hash != commit.Hash))
                {
                    this.BuildConfigurationService.SetGitCommit(buildConfiguration.Id, commit);

                    var gitCommitModel = this.GitCommitService.GetGitCommitModel(commit);

                    resultEventCode = GitServiceEventCode.GitUpdatingLastCommitChanged;
                    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommitChanged, gitCommitModel);
                }
                else
                {
                    resultEventCode = GitServiceEventCode.GitUpdatingLastCommitNoChange;
                    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, GitServiceEventCode.GitUpdatingLastCommitNoChange);
                }
            }

            return resultEventCode;
        }

        private GitFetchResult GitFetch(string workspaceDirectory, string gitExecutable)
        {
            var result = new GitFetchResult { Success = true };

            try
            {
                var fetchResult = this.ShellCommand.ExecuteShellCommand(workspaceDirectory, gitExecutable, "fetch");
                var logIncomingCommitsResult = this.ShellCommand.ExecuteShellCommand(workspaceDirectory, gitExecutable, string.Concat("log master..origin/master --date=", GIT_DATE_FORMAT));
                var incomingCommits = GetIncommingCommitsFromFetchLogOutput(logIncomingCommitsResult.Output);

                result = new GitFetchResult { Success = true, GitCommits = incomingCommits };
            }
            catch (Exception ex)
            {
                result = new GitFetchResult { Success = false, Exception = ex };
            }

            return result;
        }

        private List<GitCommit> GetIncommingCommitsFromFetchLogOutput(string output)
        {
            var result = new List<GitCommit>();

            if (string.IsNullOrWhiteSpace(output))
            {
                return result;
            }

            var outputLines = output.Split('\n');
            if (outputLines.Length < 5)
            {
                return result;
            }

            return this.GetGitCommitsFromLines(outputLines);
        }

        private List<GitCommit> GetGitCommitsFromLines(string[] lines)
        {
            var gitCommits = new List<GitCommit>();
            var lineIndex = 0;
            GitCommit newCommit = null;
            StringBuilder newCommitMessage = new StringBuilder();

            // Populate gitCommits from output lines
            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex];

                if (line.ToLower().StartsWith("commit "))
                {
                    if (newCommit == null)
                    {
                        newCommit = new GitCommit();
                        newCommit.Hash = line.Substring(7);
                        newCommitMessage.Clear();
                    }
                    else
                    {
                        newCommit.Message = newCommitMessage.ToString().Trim();
                        gitCommits.Add(newCommit);

                        newCommit = new GitCommit();
                        newCommit.Hash = line.Substring(7);
                        newCommitMessage.Clear();
                    }
                }
                else if (line.ToLower().StartsWith("author:"))
                {
                    newCommit.Author = line.Split(':')[1].Trim();
                }
                else if (line.ToLower().StartsWith("date:"))
                {
                    var commitDateTimeString = line.Substring(line.IndexOf(":") + 1).Trim();
                    var splittedDateTime = commitDateTimeString.Split('T');
                    var commitDateSplitted = splittedDateTime[0].Split('-');
                    var commitTimeSplitted = splittedDateTime[1].Split(':');

                    newCommit.DateTime = new DateTime(Convert.ToInt16(commitDateSplitted[0]),
                                                      Convert.ToInt16(commitDateSplitted[1]),
                                                      Convert.ToInt16(commitDateSplitted[2]),
                                                      Convert.ToInt16(commitTimeSplitted[0]),
                                                      Convert.ToInt16(commitTimeSplitted[1]),
                                                      Convert.ToInt16(commitTimeSplitted[2]));
                }
                else
                {
                    newCommitMessage.AppendLine(line);
                }

                lineIndex++;
            }

            if (newCommit != null)
            {
                newCommit.Message = newCommitMessage.ToString().Trim();
                gitCommits.Add(newCommit);
            }

            return gitCommits;
        }

    }
}
