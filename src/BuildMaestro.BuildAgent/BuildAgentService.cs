using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent
{
    public class BuildAgentService
    {
        public BuildAgentServiceState State { get; private set; }

        readonly Random _random = new Random();

        /// <summary>
        /// Spinner task
        /// </summary>
        private Task Task { get; set; }

        /// <summary>
        /// Spinner task token source (for stop/cancel)
        /// </summary>
        private CancellationTokenSource TokenSource { get; set; }

        public event EventHandler CpuValuChanged;

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
                    var lastRun = DateTime.UtcNow;

                    while (!token.IsCancellationRequested)
                    {
                        
                        while (DateTime.UtcNow.Subtract(lastRun).TotalSeconds <= 1)
                        {
                            System.Threading.Tasks.Task.Delay(100).Wait();
                        }

                       this.OnCpuValueChanged();
                    }
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

        private void OnCpuValueChanged()
        {
            if (this.CpuValuChanged != null)
            {
                CpuValuChanged(this, new BuildAgentEventArgs(_random.Next(0, 100)));
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
