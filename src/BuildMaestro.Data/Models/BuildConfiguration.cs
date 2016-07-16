using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Models
{
    public class BuildConfiguration
    {
        public int Id { get; set; }

        public bool AutoDeploy { get; set; }

        public string AutoDeployTags { get; set; }

        public bool Enabled { get; set; }

        public string GitRepository { get; set; }

        public string GitBranch { get; set; }

        public string GitCommitHash { get; set; }

        public DateTime? GitCommitDate { get; set; }

        public string Name { get; set; }
    }
}
