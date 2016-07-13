using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.Models
{
    public class BuildConfigurationModel
    {
        public int Id { get; set; }

        public bool AutoDeploy { get; set; }
        public string BuildType { get; set; }
        public string DeployTag { get; set; }
        public string GitRepository { get; set; }
        public string GitBranch { get; set; }
        public string GitCommit { get; set; }
        public DateTime? LastDeployDate { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Target { get; set; }
        public List<string> TargetFolders { get; set; }
        public List<string> TargetUrls { get; set; }
    }
}
