using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildMaestro.Shared.BuildAgent.Git;
using BuildMaestro.Shared.Models;
using System.IO;
using BuildMaestro.BuildAgent.Core;

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
            var buildConfigurationWorkspace = Path.Combine(this.BuildAgent.ApplicationConfiguration.Workspace, buildConfiguration.Id.ToString());
            var gitFolder = Path.Combine(this.BuildAgent.ApplicationConfiguration.Workspace, ".git");

            // Create workspace
            this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspace);
            if (!Directory.Exists(buildConfigurationWorkspace))
            {
                Directory.CreateDirectory(buildConfigurationWorkspace);
            }
            this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.CreatingWorkspaceSuccess);


            // Clone or Pull
            try
            {
                this.CheckForGitRepository(buildConfigurationWorkspace, buildConfiguration);
            }
            catch(Exception exception)
            {
                File.WriteAllText(Path.Combine(buildConfigurationWorkspace, "log.txt"), exception.Message);
            }

            return new ÚpdateWorkspaceResult { Success = true };
        }

        private void CheckForGitRepository(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var gitStatusResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, this.BuildAgent.ApplicationConfiguration.GitExecutable, "status");

            if (!string.IsNullOrEmpty(gitStatusResult.Output))
            {
                if (gitStatusResult.Output.ToLower().Contains("not a git repository"))
                {
                    // Git clone
                    try
                    {
                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitCloning);

                        var cloneResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, 
                                                                                this.BuildAgent.ApplicationConfiguration.GitExecutable, 
                                                                                string.Concat("clone ", buildConfiguration.GitRepository, " ."));

                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitCloningSuccess);
                    }
                    catch (Exception ex)
                    {
                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitCloningFailure);
                    }
                }
                else
                {
                    // Git pull
                    try
                    {
                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitPull);

                        var cloneResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, 
                                                                                this.BuildAgent.ApplicationConfiguration.GitExecutable, 
                                                                                "pull");

                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitPullSuccess);
                    }
                    catch (Exception ex)
                    {
                        this.BuildAgent.OnGitStatusEvent(buildConfiguration.Id, Events.GitServiceEventCode.GitPullFailure);
                    }
                }
            }
        }
    }
}
