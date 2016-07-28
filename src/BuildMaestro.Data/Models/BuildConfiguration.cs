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
            MsBuildConfigurations = new List<MsBuildConfiguration>();
            RepositoryConfigurations = new List<RepositoryConfiguration>();
        }

        [Key]
        public int Id { get; set; }

        public bool Active { get; set; }

        public bool Enabled { get; set; }

        public virtual ICollection<MsBuildConfiguration> MsBuildConfigurations { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RepositoryConfiguration> RepositoryConfigurations { get; set; }
    }
}
