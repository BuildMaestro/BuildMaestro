using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Models
{
    public class SyncGitRepositoryResult
    {
        public Exception Exception { get; set; }

        public bool Success { get; set; }
    }
}
