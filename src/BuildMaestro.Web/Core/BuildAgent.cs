using System;
using BuildMaestro.BuildAgent;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using BuildMaestro.Web.Hubs;
using BuildMaestro.BuildAgent.Events;

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

            Service.StatusEvent += this.OnEvent;
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

        public void Initialize()
        {

        }

        private void OnEvent(object sender, BuildAgentStatusEventArgs  e)
        {
            if (e.BuildConfigurationId != 0)
            {
                Startup.BuildAgentHub.Clients.All.bcStatusChange(e.BuildConfigurationId, e.EventCode);
            }
        }
    }
}
