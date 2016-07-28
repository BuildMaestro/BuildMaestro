using BuildMaestro.Data.Models;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
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
                    context.AplicationSettings.Add(new ApplicationSetting
                    {
                        Key = ApplicationSettingKey.MsBuildExecutable,
                        Value = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
                    });
                    context.AplicationSettings.Add(new ApplicationSetting
                    {
                        Key = ApplicationSettingKey.NugetExecutable,
                        Value = @"C:\BuildMaestro\\nuget.exe"
                    });
                }

                // Add default build configurations
                if (!context.BuildConfigurations.Any())
                {
                    var buildConfigTest1 = new BuildConfiguration
                    {
                        Active = true,
                        Enabled = true,
                        Name = "Example mvc application (TEST1)",
                        RepositoryConfigurations = new List<RepositoryConfiguration>
                        {
                            new RepositoryConfiguration
                            {
                                Active = true,
                                AutoUpdate = true,
                                AutoUpdateTag = "#deploy",
                                Branch = "master",
                                RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln",
                                RepositoryUrl = "https://github.com/BuildMaestro/ExampleMvcApplication.git"
                            }
                        }
                    };
                    var buildConfigTest1MsBuildConfigurations = new List<MsBuildConfiguration>
                    {
                        new MsBuildConfiguration { BuildConfiguration = buildConfigTest1, RepositoryConfiguration = buildConfigTest1.RepositoryConfigurations.Single() }
                    };

                    buildConfigTest1.MsBuildConfigurations = buildConfigTest1MsBuildConfigurations;
                    context.BuildConfigurations.Add(buildConfigTest1);

                    var buildConfigTest2 = new BuildConfiguration
                    {
                        Active = true,
                        Enabled = true,
                        Name = "Example mvc application (TEST2)",
                        RepositoryConfigurations = new List<RepositoryConfiguration>
                        {
                            new RepositoryConfiguration
                            {
                                Active = true,
                                AutoUpdate = true,
                                AutoUpdateTag = "#deploy",
                                Branch = "master",
                                RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln",
                                RepositoryUrl = "https://github.com/BuildMaestro/ExampleMvcApplication.git"
                            }
                        }
                    };
                    var buildConfigTest2MsBuildConfigurations = new List<MsBuildConfiguration>
                    {
                        new MsBuildConfiguration { BuildConfiguration = buildConfigTest2, RepositoryConfiguration = buildConfigTest2.RepositoryConfigurations.Single() }
                    };
                    buildConfigTest2.MsBuildConfigurations = buildConfigTest2MsBuildConfigurations;
                    context.BuildConfigurations.Add(buildConfigTest2);

                    var buildConfigAcc = new BuildConfiguration
                    {
                        Active = true,
                        Enabled = true,
                        Name = "Example mvc application (ACC)",
                        RepositoryConfigurations = new List<RepositoryConfiguration>
                        {
                            new RepositoryConfiguration
                            {
                                Active = true,
                                AutoUpdate = false,
                                AutoUpdateTag = "",
                                Branch = "master",
                                RelativeSolutionFileLocation = @"ExampleMvcApplication\ExampleMvcApplication.sln",
                                RepositoryUrl = "https://github.com/BuildMaestro/ExampleMvcApplication.git"
                            },
                            new RepositoryConfiguration
                            {
                                Active = true,
                                AutoUpdate = false,
                                AutoUpdateTag = "",
                                Branch = "master",
                                RelativeSolutionFileLocation = @"",
                                RepositoryUrl = "https://github.com/BuildMaestro/BuildMaestro.git"
                            }
                        }
                    };
                    var buildConfigAccMsBuildConfigurations = new List<MsBuildConfiguration>
                    {
                        new MsBuildConfiguration { BuildConfiguration = buildConfigAcc, RepositoryConfiguration = buildConfigAcc.RepositoryConfigurations.First() }
                    };
                    buildConfigAcc.MsBuildConfigurations = buildConfigAccMsBuildConfigurations;
                    context.BuildConfigurations.Add(buildConfigAcc);

                    context.SaveChanges();
                }
            }
        }
    }
}
