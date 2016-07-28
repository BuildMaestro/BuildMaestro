using BuildMaestro.Data;
using BuildMaestro.Shared.Models;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildMaestro.Data.Models;

namespace BuildMaestro.Service
{
    public class BuildConfigurationService
    {
        public List<BuildConfigurationModel> GetBuildConfigurations()
        {
            List<BuildConfigurationModel> result = new List<BuildConfigurationModel>();

            using (var context = new BuildMaestroContext())
            {
                var buildConfigurations = context.BuildConfigurations.Include(x => x.RepositoryConfigurations);

                foreach(var buildConfiguration in buildConfigurations)
                {
                    var latestCommit = buildConfiguration.RepositoryConfigurations.SelectMany(s => s.GitCommits).OrderByDescending(commit => commit.DateTime).FirstOrDefault();
                    var applicationSettingService = new ApplicationSettingService();
                    var dateTimeFormat = applicationSettingService.GetSetting(ApplicationSettingKey.DateTimeFormat);

                    result.Add(new BuildConfigurationModel
                    {
                        Id = buildConfiguration.Id,
                        Name = buildConfiguration.Name,
                        RepositoryConfigurations = buildConfiguration.RepositoryConfigurations.Select((repositoryConfiguration) => new RepositoryConfigurationModel
                        {
                            Id = repositoryConfiguration.Id,
                            Active = repositoryConfiguration.Active,
                            AutoUpdate = repositoryConfiguration.AutoUpdate,
                            AutoUpdateTag = repositoryConfiguration.AutoUpdateTag,
                            Branch = repositoryConfiguration.Branch,
                            RelativeSolutionFile = repositoryConfiguration.RelativeSolutionFileLocation,
                            RepositoryUrl = repositoryConfiguration.RepositoryUrl
                        }).ToList()
                    });
                }
            }

            return result;
        }

        //public bool SetGitCommit(RepositoryConfiguration repositoryConfiguration, GitCommit gitCommit)
        //{
        //    using (var context = new BuildMaestroContext())
        //    {
        //        if (repositoryConfiguration != null)
        //        {
        //            repositoryConfiguration.GitCommits.Add(gitCommit);

        //            context.SaveChanges();
        //        }
        //    }

        //    return true;
        //}
    }
}
