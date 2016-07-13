using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.Models
{
    public class ApplicationConfigurationModel
    {
        public string GitExecutable { get; set; }
        public string Workspace { get; set; }
    }
}
