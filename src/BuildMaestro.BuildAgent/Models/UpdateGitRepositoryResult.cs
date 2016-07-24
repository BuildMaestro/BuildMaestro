using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Models
{
    public class UpdateGitRepositoryResult
    {
        public Exception Exception { get; set; }
        public UpdateGitRepositoryResultSuccesType Success { get; set; }
    }

    public enum UpdateGitRepositoryResultSuccesType
    {
        Failure,
        Success,
        SuccessMerged,
        SuccessNotMerged
    }
}
