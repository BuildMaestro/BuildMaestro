using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.Models
{
    public class BuildConfigurationModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string BuildType { get; set; }
        public GitCommitModel LatestGitCommit { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public List<RepositoryConfigurationModel> RepositoryConfigurations { get; set; }
        public string Target { get; set; }
        public List<string> TargetFolders { get; set; }
        public List<string> TargetUrls { get; set; }
    }
}
