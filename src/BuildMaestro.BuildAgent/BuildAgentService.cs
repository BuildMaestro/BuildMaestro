using BuildMaestro.BuildAgent.Events;
using BuildMaestro.BuildAgent.Models;
using BuildMaestro.BuildAgent.Services;
using BuildMaestro.Service;
using BuildMaestro.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent
{
    public class BuildAgentService
    {
        const int WORKER_LOOP_INTERVAL = 5; // seconds
        const int WORKER_WAIT_DELAY = 100; // miliseconds

        public delegate void StatusChangeEventHandler(object sender, BuildAgentStatusChangeEventArgs e);
        public event StatusChangeEventHandler StatusChangeEvent;

        public BuildAgentServiceState State { get; private set; }

        private Task Task { get; set; }

        private CancellationTokenSource TokenSource { get; set; }

        readonly Random _random = new Random();

        public BuildAgentService()
        {
            this.TokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            var token = this.TokenSource.Token;

            if (this.Task == null)
            {
                this.Task = Task.Run(() =>
                {
                    this.Worker(token);
                }, token);
            }
        }

        public void Stop()
        {
            this.TokenSource.Cancel();
            this.Task.Wait();
            this.Task = null;
            this.State = BuildAgentServiceState.Stopped;
        }

        private void Worker(CancellationToken token)
        {
            var lastRun = DateTime.UtcNow;

            while (!token.IsCancellationRequested)
            {
                while (DateTime.UtcNow.Subtract(lastRun).TotalSeconds <= WORKER_LOOP_INTERVAL)
                {
                    System.Threading.Tasks.Task.Delay(WORKER_WAIT_DELAY).Wait();
                }

                this.WorkerRunBuildConfigurations();

                lastRun = DateTime.UtcNow;
            }

        }

        private async void WorkerRunBuildConfigurations()
        {
            var buildConfigurationService = new BuildConfigurationService();
            var buildConfigurations = buildConfigurationService.GetBuildConfigurations();
            var gitService = new GitService(this);
            var buildService = new BuildService(this);

            foreach (var buildConfiguration in buildConfigurations)
            {
                var gitTask = await gitService.Run(buildConfiguration);

                if (gitTask.Success)
                {
                    var buildTask = await buildService.Run(buildConfiguration);
                }

                var s = 2;
            }
        }


        //private void WorkerRunBuildConfigurations()
        //{
        //    var buildConfigurationService = new BuildConfigurationService();
        //    var buildConfigurations = buildConfigurationService.GetBuildConfigurations();
        //    var gitService = new GitService(this);
        //    var buildService = new BuildService(this);

        //    foreach (var buildConfiguration in buildConfigurations)
        //    {
        //        // Run build confiration
        //        var initializeWorkspaceResult = gitService.InitializeWorkspace(buildConfiguration);
        //        if (!initializeWorkspaceResult.Success)
        //        {
        //            continue;
        //        }

        //        var initializeGitRepositoryResult = gitService.InitializeGitRepository(initializeWorkspaceResult.WorkspaceDirectory, buildConfiguration);
        //        if (!initializeGitRepositoryResult.Success)
        //        {
        //            continue;
        //        }

        //        var updateGitRepositoryResult = gitService.UpdateGitRepository(initializeWorkspaceResult.WorkspaceDirectory, buildConfiguration);
        //        if (updateGitRepositoryResult.Success != Models.UpdateGitRepositoryResultSuccesType.SuccessMerged)
        //        {
        //            continue;
        //        }

        //        var updateLatestCommitResultGitEventCode = gitService.UpdateLatestCommit(initializeWorkspaceResult.WorkspaceDirectory, buildConfiguration);

        //        if (updateLatestCommitResultGitEventCode != EventCode.GitUpdatingLastCommitChanged)
        //        {
        //            continue;
        //        }
        //        var restoreResult = buildService.NugetRestore(initializeWorkspaceResult.WorkspaceDirectory, buildConfiguration);

        //        var buildResult = buildService.Build(initializeWorkspaceResult.WorkspaceDirectory, buildConfiguration);

        //    }
        //}

        public void OnStatusChangeEvent(int buildConfigurationId, EventCode code)
        {
            this.OnStatusChangeEvent(buildConfigurationId, code, null);
        }

        public void OnStatusChangeEvent(int buildConfigurationId, EventCode code, dynamic data)
        {
            if (this.StatusChangeEvent != null)
            {
                this.StatusChangeEvent(this, new BuildAgentStatusChangeEventArgs()
                {
                    BuildConfigurationId = buildConfigurationId,
                    Data = data,
                    DateTime = DateTime.UtcNow,
                    EventCode = (int)code,
                    Type = BuildAgentStatusChangeEventType.GitService
                });
            }
        }
    }

    public class BuildAgentEventArgs : EventArgs
    {
        public BuildAgentEventArgs(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
