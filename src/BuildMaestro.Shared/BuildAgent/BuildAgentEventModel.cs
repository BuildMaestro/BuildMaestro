using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.BuildAgent
{
    public class BuildAgentEventModel
    {
        public Guid Id { get; set; }

        public BuildAgentEventType Type { get; set; }
    }

    public enum BuildAgentEventType
    {
        Unknown = 0,
        BuildAgentStarted = 1,
        BuildAgentStopped = 2
    }
}
