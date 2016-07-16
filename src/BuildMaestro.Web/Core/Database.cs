using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Web.Core
{
    public class Database
    {
        public static void Initialize(IServiceProvider serviceProviders)
        {
            // Migrate database to latest version.
            using (var context = new Data.BuildMaestroContext())
            {
                context.Database.Migrate();
            }

            // Seed database if empty
            using (var context = new Data.BuildMaestroContext())
            {
                if (!context.BuildConfigurations.Any())
                {
                    context.BuildConfigurations.Add(new Models.BuildConfiguration
                    {
                        Name = "Example application (TEST1)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitCommitDate = null,
                        GitCommitHash = "",
                        GitRepository = "https://github.com/mvdcorput/buildmaestro-testproject.git",
                    });

                    context.BuildConfigurations.Add(new Models.BuildConfiguration
                    {
                        Name = "Example application (TEST2)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitCommitDate = null,
                        GitCommitHash = "",
                        GitRepository = "https://github.com/mvdcorput/buildmaestro-testproject.git",
                    });

                    context.BuildConfigurations.Add(new Models.BuildConfiguration
                    {
                        Name = "Example application (ACC)",
                        AutoDeploy = true,
                        AutoDeployTags = "",
                        GitBranch = "master",
                        GitCommitDate = null,
                        GitCommitHash = "",
                        GitRepository = "https://github.com/mvdcorput/buildmaestro-testproject.git",
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
