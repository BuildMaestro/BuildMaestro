using BuildMaestro.Data.Models;
using BuildMaestro.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Service
{
    public class GitCommitService
    {
        public GitCommitModel GetGitCommitModel(GitCommit commit)
        {
            var applicationSettingService = new ApplicationSettingService();
            var dateFormat = applicationSettingService.GetSetting(ApplicationSettingKey.DateTimeFormat);

            return new GitCommitModel
            {
                Author = commit.Author,
                DateTime = commit.DateTime.ToString(dateFormat),
                Hash = commit.Hash,
                Message = commit.Message
            };
        }
    }
}
