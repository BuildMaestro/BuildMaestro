using BuildMaestro.Web.Hubs;
using BuildMaestro.Web.Models;
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
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var buildConfigurationsAsString = readJsonData("build-configurations.json");
            var model = JsonConvert.DeserializeObject<List<BuildConfigurationModel>>(buildConfigurationsAsString, jsonSettings);


            return Json(model);
        }

        [HttpPost]
        public IActionResult Test(string value)
        {
            var result = true;

            BuildAgentHub.Clients.All.newCpuValue(33);

            return Json(result);
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
