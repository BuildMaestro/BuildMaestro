using BuildMaestro.BuildAgent.Events;
using BuildMaestro.BuildAgent.Services;
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

        public BuildAgentServiceState State { get; private set; }

        private Task Task { get; set; }

        private CancellationTokenSource TokenSource { get; set; }

        readonly Random _random = new Random();

        public delegate void StatusEventHandler(object sender, BuildAgentStatusEventArgs e);
        public event StatusEventHandler StatusEvent;

        public ApplicationConfigurationModel ApplicationConfiguration { get; set; }

        public List<BuildConfigurationModel> BuildConfigurations { get; set; }


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

                this.WorkerUpdateWorkspaces();

                lastRun = DateTime.UtcNow;
            }

        }

        private void WorkerUpdateWorkspaces()
        {
            if (this.ApplicationConfiguration != null && this.BuildConfigurations != null)
            {
                using (var gitService = new GitService(this))
                {
                    foreach (var buildConfiguration in this.BuildConfigurations)
                    {
                        gitService.ÚpdateWorkspace(buildConfiguration);
                    }
                }
            }
        }

        public void OnGitStatusEvent(int buildConfigurationId, GitServiceEventCode code)
        {
            if (this.StatusEvent != null)
            {
                this.StatusEvent(this, new BuildAgentStatusEventArgs()
                {
                    Type = BuildAgentStatusEventType.GitService,
                    BuildConfigurationId = buildConfigurationId,
                    EventCode = (int)code
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
