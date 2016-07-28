using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    public enum RunServiceStatusType
    {
        Idle,
        Running,
        Fail,
        Success,
        FinishedSucessfully,
        FinishedWithError
    }
}
