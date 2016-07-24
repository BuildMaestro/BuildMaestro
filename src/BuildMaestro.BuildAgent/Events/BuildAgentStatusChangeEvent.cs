using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    public class BuildAgentStatusChangeEventArgs : EventArgs
    {
        public Guid Id { get; set; }
        
        public int BuildConfigurationId { get; set; }

        public object Data { get; set; }

        public DateTime DateTime { get; set; }

        public int EventCode { get; set; }

        public BuildAgentStatusChangeEventType Type { get; set; }

    }
}
