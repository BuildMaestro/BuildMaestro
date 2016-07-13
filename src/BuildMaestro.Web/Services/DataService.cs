using BuildMaestro.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Web.Services
{
    public class DataService
    {
        public ApplicationConfigurationModel GetApplicationConfiguration()
        {
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var buildConfigurationsAsString = readJsonData("application-configuration.json");
            var model = JsonConvert.DeserializeObject<ApplicationConfigurationModel>(buildConfigurationsAsString, jsonSettings);

            return model;
        }

        public List<BuildConfigurationModel> GetBuildConfigurations()
        {
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var buildConfigurationsAsString = readJsonData("build-configurations.json");
            var model = JsonConvert.DeserializeObject<List<BuildConfigurationModel>>(buildConfigurationsAsString, jsonSettings);

            return model;
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
