using System;
using BuildMaestro.BuildAgent;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using BuildMaestro.Web.Hubs;

namespace BuildMaestro.Web.Core
{
    public sealed class BuildAgent
    {
        private static volatile BuildAgent instance;
        private static object syncRoot = new Object();
        
        public BuildAgentService Service { get; set; }

        private BuildAgent()
        {
            Service = new BuildAgentService();
            Service.CpuValuChanged += this.OnEvent;
        }

        public static BuildAgent Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BuildAgent();
                        }
                    }
                }

                return instance;
            }
        }

        private void OnEvent(object sender, EventArgs e)
        {
            BuildAgentEventArgs eventsArgs = e as BuildAgentEventArgs;

            Startup.BuildAgentHub.Clients.All.newCpuValue(eventsArgs.Value);
        }
    }
}
