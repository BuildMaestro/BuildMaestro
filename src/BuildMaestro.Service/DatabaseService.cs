using BuildMaestro.Data.Models;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Service
{
    public class DatabaseService
    {
        public void Initialize()
        {
            // Migrate database to latest version.
            using (var context = new Data.BuildMaestroContext())
            {
                context.Database.Migrate();
            }

            // Seed database if empty
            using (var context = new Data.BuildMaestroContext())
            {
                // Add Application settings
                if (!context.AplicationSettings.Any())
                {
                    context.AplicationSettings.Add(new ApplicationSetting
                    {
                        Key = ApplicationSettingKey.GitExecutable,
                        Value = @"C:\Program Files\Git\cmd\git.exe"
                    });
                    context.AplicationSettings.Add(new ApplicationSetting
                    {
                        Key = ApplicationSettingKey.Workspace,
                        Value = @"C:\BuildMaestro\Workspace"
                    });
                    context.AplicationSettings.Add(new ApplicationSetting
                    {
                        Key = ApplicationSettingKey.DateTimeFormat,
                        Value = @"dd-MM-yyyy HH:mm:ss"
                    });
                }

                // Add default build configurations
                if (!context.BuildConfigurations.Any())
                {
                    context.BuildConfigurations.Add(new BuildConfiguration
                    {
                        Active = true,
                        Name = "Example mvc application (TEST1)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitRepository = "https://github.com/BuildMaestro/ExampleMvcApplication.git",
                        RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln"
                    });

                    context.BuildConfigurations.Add(new BuildConfiguration
                    {
                        Active = true,
                        Name = "Example mvc application (TEST2)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitRepository = "https://github.com/BuildMaestro/ExampleMvcApplication.git",
                        RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln"

                    });

                    context.BuildConfigurations.Add(new BuildConfiguration
                    {
                        Active = true,
                        Name = "Example mvc application (ACC)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitRepository = "https://github.com/BuildMaestro/ExampleMvcApplication.git",
                        RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln"

                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
