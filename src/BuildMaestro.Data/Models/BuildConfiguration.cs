using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Data.Models
{
    public class BuildConfiguration
    {
        public BuildConfiguration()
        {
            GitCommits = new List<GitCommit>();
        }

        [Key]
        public int Id { get; set; }

        public bool Active { get; set; }

        public bool AutoDeploy { get; set; }

        public string AutoDeployTags { get; set; }

        public bool CleanWorkspace { get; set; }

        public bool Enabled { get; set; }

        public string GitRepository { get; set; }

        public string GitBranch { get; set; }
        
        public string Name { get; set; }

        public string RelativeSolutionFileLocation { get; set; }

        public virtual ICollection<GitCommit> GitCommits { get; set; }

    }
}
