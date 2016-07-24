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
                var buildConfigurations = context.BuildConfigurations.Include(x => x.GitCommits);

                foreach(var config in buildConfigurations)
                {
                    var latestCommit = config.GitCommits.OrderByDescending(commit => commit.DateTime).FirstOrDefault();
                    var applicationSettingService = new ApplicationSettingService();
                    var dateTimeFormat = applicationSettingService.GetSetting(ApplicationSettingKey.DateTimeFormat);

                    result.Add(new BuildConfigurationModel
                    {
                        AutoDeploy = config.AutoDeploy,
                        AutoDeployTags = config.AutoDeployTags,
                        LatestGitCommit = latestCommit == null ? null : new GitCommitModel
                        {
                            Author = latestCommit.Author,
                            DateTime = latestCommit.DateTime.ToString(dateTimeFormat),
                            Hash = latestCommit.Hash,
                            Message = latestCommit.Message
                        },
                        GitRepository = config.GitRepository,
                        Id = config.Id,
                        Name = config.Name
                    });
                }
            }

            return result;
        }

        public bool SetGitCommit(int buildConfigurationId, GitCommit gitCommit)
        {
            using (var context = new BuildMaestroContext())
            {
                var buildConfiguration = context.BuildConfigurations.SingleOrDefault(x => x.Id == buildConfigurationId);

                if (buildConfiguration != null)
                {
                    buildConfiguration.GitCommits.Add(gitCommit);

                    context.SaveChanges();
                }
            }

            return true;
        }
    }
}
