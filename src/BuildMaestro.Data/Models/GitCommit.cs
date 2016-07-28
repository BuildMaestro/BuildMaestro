using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Data.Models
{
    public class GitCommit
    {
        [Key]
        public int Id { get; set; }

        public string Author { get; set; }

        public string Branch { get; set; }

        public virtual RepositoryConfiguration RepositoryConfiguration{ get; set; }

        public int RepositoryConfigurationId { get; set; }

        public DateTime DateTime { get; set; }

        public string Hash { get; set; }

        public string Message { get; set; }

        public string Repository { get; set; }
    }
}
