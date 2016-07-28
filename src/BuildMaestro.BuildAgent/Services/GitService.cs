using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildMaestro.Shared.Models;
using System.IO;
using BuildMaestro.BuildAgent.Core;
using BuildMaestro.Service;
using BuildMaestro.Data.Models;
using BuildMaestro.BuildAgent.Models;
using System.Text;

namespace BuildMaestro.BuildAgent.Services
{
    internal class GitService : Disposable
    {
        const string GIT_DATE_FORMAT = "format:%Y-%m-%dT%H:%M:%S";

        private ApplicationSettingService ApplicationSettingService { get; set; }
        private BuildAgentService BuildAgent { get; set; }
        private BuildConfigurationService BuildConfigurationService { get; set; }
        private GitCommitService GitCommitService { get; set; }
        private ShellCommandService ShellCommand { get; set; }

        public GitService(BuildAgentService buildAgent)
        {
            this.ApplicationSettingService = new ApplicationSettingService();
            this.BuildAgent = buildAgent;
            this.BuildConfigurationService = new BuildConfigurationService();
            this.GitCommitService = new GitCommitService();
            this.ShellCommand = new ShellCommandService();
        }

        public async Task<GitRunResult> Run(BuildConfigurationModel buildConfiguration)
        {
            var result = new GitRunResult { Success = false };

            var gitTasks = buildConfiguration.RepositoryConfigurations.Select((repositoryConfiguration) => UpdateGitRepository(buildConfiguration, repositoryConfiguration));
            var gitresults = await Task.WhenAll(gitTasks);

            if (gitresults.Any(gitResult => result.Success = false))
            {
                result.Success = false;

                return result;
            }

            result.Success = false;

            return result;
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

        private string GetWorkspaceRepositoriesRootPath(BuildConfigurationModel buildConfiguration)
        {
            var workspace = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.Workspace);
            var buildConfigurationWorkspace = Path.Combine(workspace, buildConfiguration.Id.ToString());
            var workspaceRepositoriesRootPath = Path.Combine(buildConfigurationWorkspace, "Repositories");

            return workspaceRepositoriesRootPath;
        }

        private string GetWorkspaceOutputRootPath(BuildConfigurationModel buildConfiguration)
        {
            var workspace = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.Workspace);
            var buildConfigurationWorkspace = Path.Combine(workspace, buildConfiguration.Id.ToString());
            var workspaceRepositoriesRootPath = Path.Combine(buildConfigurationWorkspace, "Output");

            return workspaceRepositoriesRootPath;
        }

        private InitializeGitRepositoryResult InitializeGitRepository(BuildConfigurationModel buildConfiguration, RepositoryConfigurationModel repositoryConfiguration)
        {
            var gitExecutable = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            var result = new InitializeGitRepositoryResult { Success = false };
            var repositoriesRoot = this.GetWorkspaceRepositoriesRootPath(buildConfiguration);
            var repositoryFolder = Path.Combine(repositoriesRoot, repositoryConfiguration.RelativeSolutionFilePath);

            if (string.IsNullOrWhiteSpace(gitExecutable) || !File.Exists(gitExecutable))
            {
                return new InitializeGitRepositoryResult { Success = false, Exception = new Exception("Git executable not found.") };
            }

            if (!Directory.Exists(repositoryFolder))
            {
                Directory.CreateDirectory(repositoryFolder);
            }

            var gitStatusResult = this.ShellCommand.ExecuteShellCommand(repositoryFolder, gitExecutable, "status");

            if (!string.IsNullOrEmpty(gitStatusResult.Output) && gitStatusResult.Output.ToLower().Contains("not a git repository"))
            {
                try
                {
                    var cloneResult = this.ShellCommand.ExecuteShellCommand(repositoryFolder,
                                                                            gitExecutable,
                                                                            string.Concat("clone ", repositoryConfiguration.RepositoryUrl, " ."));

                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result = new InitializeGitRepositoryResult { Success = false, Exception = ex };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(gitStatusResult.Output) && gitStatusResult.Output.ToLower().Contains("branch is up-to-date"))
                {
                    result.Success = true;
                }
            }

            return result;
        }

        private InitializeWorkspaceResult InitializeWorkspace(BuildConfigurationModel buildConfiguration)
        {
            var result = new InitializeWorkspaceResult { Success = false };
            var repositoriesRoot = this.GetWorkspaceRepositoriesRootPath(buildConfiguration);

            try
            {
                if (!Directory.Exists(repositoriesRoot))
                {
                    Directory.CreateDirectory(repositoriesRoot);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result = new InitializeWorkspaceResult { Success = false, Exception = ex };
            }

            return result;
        }

        private SyncGitRepositoryResult SyncGitRepository(BuildConfigurationModel buildConfiguration, RepositoryConfigurationModel repositoryConfiguration)
        {
            var gitExecutable = this.ApplicationSettingService.GetSetting(ApplicationSettingKey.GitExecutable);
            var repositoriesRoot = this.GetWorkspaceRepositoriesRootPath(buildConfiguration);
            var repositoryFolder = Path.Combine(repositoriesRoot, repositoryConfiguration.RelativeSolutionFilePath);
            var result = new SyncGitRepositoryResult { Success = false };

            if (string.IsNullOrWhiteSpace(gitExecutable) || !File.Exists(gitExecutable))
            {
                return new SyncGitRepositoryResult { Success = false, Exception = new Exception("Git executable not found.") };
            }

            var fetchResult = this.GitFetch(repositoryFolder, gitExecutable);

            if (fetchResult.Success)
            {
                // If incoming commits
                if (fetchResult.GitCommits != null && fetchResult.GitCommits.Any())
                {
                    // If auto deploy
                    if (repositoryConfiguration.AutoUpdate)
                    {
                        var mergeResult = this.SyncGitRepositoryMerge(fetchResult.GitCommits, repositoryConfiguration.AutoUpdateTag, repositoryFolder, gitExecutable);

                        if (mergeResult.Success)
                        {
                            result.Success = true;
                        }
                        else
                        {
                            result.Exception = new Exception("Git merge error");
                            result.Success = false;
                        }
                    }
                    else
                    {
                        result.Success = true;
                    }
                }
                else
                {
                    result.Success = true;
                }
            }
            else
            {
                result.Exception = new Exception("Git fetch error");
                result.Success = false;
            }

            return result;
        }

        private SyncGitRepositoryResult SyncGitRepositoryMerge(List<GitCommit> newCommits, string tag, string repositoryFolder, string gitExecutable)
        {
            var result = new SyncGitRepositoryResult { Success = true };
            var tags = tag.Split(',').ToList().Select(x => x.Trim()).ToList();
            var taggedCommit = newCommits.OrderByDescending(o => o.DateTime).FirstOrDefault(x => tags.Any(t => x.Message.Contains(t)));

            try
            {
                var mergeResult = taggedCommit != null ? this.ShellCommand.ExecuteShellCommand(repositoryFolder, gitExecutable, string.Concat("merge ", taggedCommit.Hash)) :
                                                         this.ShellCommand.ExecuteShellCommand(repositoryFolder, gitExecutable, string.Concat("merge"));

                if (mergeResult.Output.ToLower().Contains("updating") && mergeResult.Output.ToLower().Contains("fast-forward"))
                {
                    result.Success = true;
                }
                else
                {
                    // Investigate possible output scenarios
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Success = false;
            }

            return result;
        }

        private async Task<UpdateGitRepositoryResult> UpdateGitRepository(BuildConfigurationModel buildConfiguration, RepositoryConfigurationModel repositoryConfiguration)
        {
            return await Task<UpdateGitRepositoryResult>.Run(() =>
            {
                var result = new UpdateGitRepositoryResult { Success = false };

                var initializeWorkspace = this.InitializeWorkspace(buildConfiguration);
                if (initializeWorkspace.Success == false)
                {
                    result.Exception = new Exception("Workspace initialization failed.");
                    result.Success = false; 
                }

                var initializeGitRepository = this.InitializeGitRepository(buildConfiguration, repositoryConfiguration);
                if (initializeGitRepository.Success == false)
                {
                    result.Exception = new Exception("Repository initialization failed.");
                    result.Success = false;
                }

                var syncGitRepository = this.SyncGitRepository(buildConfiguration, repositoryConfiguration);
                if (syncGitRepository.Success == false)
                {
                    result.Exception = new Exception("Repository sync failed.");
                    result.Success = false;
                }

                result.Success = true;

                return result;
            });
        }
    }
}
