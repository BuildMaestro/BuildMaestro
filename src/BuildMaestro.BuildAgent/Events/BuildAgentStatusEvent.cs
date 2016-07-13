using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    public class BuildAgentStatusEventArgs : EventArgs
    {
        public Guid Id { get; set; }

        public BuildAgentStatusEventType Type { get; set; }

        public int BuildConfigurationId { get; set; }

        public int EventCode { get; set; }

        public DateTime DateTime { get; set; }
    }
}
