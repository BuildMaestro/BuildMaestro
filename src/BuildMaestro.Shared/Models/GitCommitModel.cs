using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.Models
{
    public class GitCommitModel
    {
        public string Author { get; set; }

        public string DateTime { get; set; }

        public string Hash { get; set; }

        public string Message { get; set; }
    }
}
