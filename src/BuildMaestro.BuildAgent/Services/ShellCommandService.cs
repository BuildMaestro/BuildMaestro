using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Services
{
    internal class ShellCommandService
    {
        public ShellCommandServiceResult ExecuteShellCommand(string directory, string command, string args)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = directory
            };

            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                // Success
                return new ShellCommandServiceResult { Type = ShellCommandServiceResultType.Success, Output = output };
            }

            var err = proc.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(err))
            {
                // Failure
                return new ShellCommandServiceResult { Type = ShellCommandServiceResultType.Error, Output = err };
            }
            proc.WaitForExit();

            // Failure
            return new ShellCommandServiceResult { Type = ShellCommandServiceResultType.Error, Output = string.Empty };
        }
    }
}
