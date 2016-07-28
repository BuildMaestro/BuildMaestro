using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Services
{
    internal class ShellCommandServiceResult
    {
        public ShellCommandServiceResultType Type { get; set; }

        public string Output { get; set; }
    }

    internal enum ShellCommandServiceResultType
    {
        Error,
        Success
    }
}
