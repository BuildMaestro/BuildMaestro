using BuildMaestro.BuildAgent.Models;
using BuildMaestro.Data.Models;
using BuildMaestro.Service;
using BuildMaestro.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Services
{
    internal class BuildService
    {
        private ApplicationSettingService ApplicationSetting { get; set; }
        private BuildAgentService BuildAgent { get; set; }
        private ShellCommandService ShellCommand { get; set; }

        public BuildService(BuildAgentService buildAgent)
        {
            this.ApplicationSetting = new ApplicationSettingService();
            this.BuildAgent = buildAgent;
            this.ShellCommand = new ShellCommandService();
        }

        public async Task<BuildRunResult> Run(BuildConfigurationModel buildConfiguration)
        {
            var buildRunResult = await Task<BuildRunResult>.Run(() =>
            {
                var result = new BuildRunResult() { Success = false };
                var restoreResult = this.NugetRestore("", buildConfiguration);

                if (restoreResult.Success)
                {
                    var buildResult = this.Build("", buildConfiguration);

                    if (buildResult.Success)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Exception = new Exception("Build failed");
                        result.Success = false;
                    }
                }
                else
                {
                    result.Exception = new Exception("Nuget restore failed");
                    result.Success = false;
                }

                return result;
            });

            return buildRunResult;
        }

        public BuildResult Build(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var result = new BuildResult { Success = false };

            try
            {
                //this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.BuildStarted);
                //var msbuildResult = this.ShellCommand.ExecuteShellCommand(buildConfigurationWorkspace, 
                //                                                          this.ApplicationSetting.GetSetting(ApplicationSettingKey.MsBuildExecutable), 
                //                                                          string.Concat(buildConfiguration.RelativeSolutionFileLocation, " ", "/t:Rebuild /p:outdir=\"", buildConfigurationWorkspace, "\\..\\Output\" /p:Configuration=Release /p:Platform=\"Any CPU\""));

                //if (msbuildResult.Output.ToLower().Contains("build failed."))
                //{
                //    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.BuildFailure);
                //}
                //else
                //{
                //    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.BuildSuccess);
                //}
            }
            catch (Exception ex)
            {
                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.BuildFailure);
            }

            return result;
        }

        public NugetRestoreResult NugetRestore(string buildConfigurationWorkspace, BuildConfigurationModel buildConfiguration)
        {
            var result = new NugetRestoreResult { Success = false };

            try
            {
                //var solutionPath = buildConfiguration.RelativeSolutionFileLocation.Contains(@"\") ?
                //                        buildConfiguration.RelativeSolutionFileLocation.Substring(0, buildConfiguration.RelativeSolutionFileLocation.LastIndexOf(@"\") + 1) :
                //                        string.Empty;
                //var workspaceSolutionPath = Path.Combine(buildConfigurationWorkspace, solutionPath);

                //this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.NugetRestoreStarted);

                //var restoreResult = this.ShellCommand.ExecuteShellCommand(workspaceSolutionPath,
                //                                                          this.ApplicationSetting.GetSetting(ApplicationSettingKey.NugetExecutable),
                //                                                          "restore");

                //if (restoreResult.Output.ToLower().Contains("restore failed."))
                //{
                //    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.NugetRestoreFailure);

                //    result = new NugetRestoreResult { Success = false, Exception = new Exception(string.Concat("Restore failed: ", restoreResult.Output)) };
                //}
                //else
                //{
                //    this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.NugetRestoreSuccess);
                //}
            }
            catch (Exception ex)
            {
                this.BuildAgent.OnStatusChangeEvent(buildConfiguration.Id, Events.EventCode.NugetRestoreFailure);
                result = new NugetRestoreResult { Success = false, Exception = ex };
            }

            return result;
        }
    }
}
