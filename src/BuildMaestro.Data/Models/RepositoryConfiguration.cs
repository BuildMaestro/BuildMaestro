using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Data.Models
{
    public class RepositoryConfiguration
    {
        public RepositoryConfiguration()
        {
            GitCommits = new List<GitCommit>();
        }

        [Key]
        public int Id { get; set; }

        public int BuildConfigurationId { get; set; }

        public bool Active { get; set; }

        public bool AutoUpdate { get; set; }

        public string AutoUpdateTag { get; set; }

        public string Branch { get; set; }

        public virtual BuildConfiguration BuildConfiguration { get; set; }

        public virtual ICollection<GitCommit> GitCommits { get; set; }

        public string RelativeSolutionFileLocation { get; set; }

        public string RepositoryUrl { get; set; }
    }
}
