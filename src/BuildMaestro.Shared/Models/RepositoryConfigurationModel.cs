using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Shared.Models
{
    public class RepositoryConfigurationModel
    {
        public int Id { get; set; }

        public bool Active { get; set; }

        public bool AutoUpdate { get; set; }

        public string AutoUpdateTag { get; set; }

        public string Branch { get; set; }

        public int BuildConfigurationId { get; set; }

        public List<GitCommitModel> GitCommits { get; set; }

        public string RelativeSolutionFile { get; set; }

        public string RelativeSolutionFilePath
        {
            get
            {
                const string folderSeperator = @"\";
                var result = string.Empty;

                if (this.RelativeSolutionFile.Contains(folderSeperator))
                {
                    result = this.RelativeSolutionFile.Substring(0, this.RelativeSolutionFile.LastIndexOf(folderSeperator) + 1);
                }

                return result;
            }
        }

        public string RepositoryUrl { get; set; }

        public string RepositoryFolder { get; set; }
    }
}
