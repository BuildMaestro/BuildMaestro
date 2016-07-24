using System;
using BuildMaestro.BuildAgent;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using BuildMaestro.Web.Hubs;
using BuildMaestro.BuildAgent.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            Service.StatusChangeEvent += this.OnEvent;
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

        private void OnEvent(object sender, BuildAgentStatusChangeEventArgs e)
        {
            if (e.BuildConfigurationId != 0)
            {
                var data = JsonConvert.SerializeObject(e.Data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                Startup.BuildAgentHub.Clients.All.statusChange(e.BuildConfigurationId, e.EventCode, data);
            }
        }
    }
}
