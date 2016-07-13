using BuildMaestro.Shared.Models;
using BuildMaestro.Web.Hubs;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace BuildMaestro.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly IHubContext BuildAgentHub;

        public DataController(IConnectionManager connectionManager)
        {
            BuildAgentHub = connectionManager.GetHubContext<BuildAgentHub>();
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult BuildConfigurations()
        {
            var dataService = new Services.DataService();

            return Json(dataService.GetBuildConfigurations());
        }

        [HttpPost]
        public IActionResult StartBuildAgent()
        {
            var buildAgent = Core.BuildAgent.Instance;
            var dataService = new Services.DataService();

            if (buildAgent.Service.State != BuildAgent.BuildAgentServiceState.Started)
            {
                buildAgent.Service.ApplicationConfiguration = dataService.GetApplicationConfiguration();
                buildAgent.Service.BuildConfigurations = dataService.GetBuildConfigurations();
                buildAgent.Service.Start();

                return Json(true);
            }

            return Json(false);
        }

        [HttpPost]
        public IActionResult StopBuildAgent()
        {
            var buildAgent = Core.BuildAgent.Instance;
            var dataService = new Services.DataService();

            if (buildAgent.Service.State != BuildAgent.BuildAgentServiceState.Stopped)
            {
                buildAgent.Service.Stop();

                return Json(true);
            }

            return Json(false);
        }

        private string readJsonData(string filename)
        {
            var result = string.Empty;
            var currentDir = Directory.GetCurrentDirectory();
            var fullFilename = Path.Combine(currentDir, "..", "Data", filename);
            using (TextReader file = System.IO.File.OpenText(fullFilename))
            {
                result = file.ReadToEnd();
            }
             
            return result; 
        }
    }
}
