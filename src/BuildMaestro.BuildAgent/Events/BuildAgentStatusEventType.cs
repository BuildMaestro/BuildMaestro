﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    public enum BuildAgentStatusEventType
    {
        Unkown = 0,
        BuildAgent = 1,
        GitService = 2
    }
}
