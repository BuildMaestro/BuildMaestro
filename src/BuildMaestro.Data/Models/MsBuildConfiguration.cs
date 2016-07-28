using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Data.Models
{
    public class MsBuildConfiguration
    {
        [Key]
        public int Id { get; set; }

        public int BuildConfigurationId { get; set; }

        public bool Active { get; set; }

        public virtual RepositoryConfiguration RepositoryConfiguration { get; set; }

        public virtual BuildConfiguration BuildConfiguration { get; set; }
    }
}
