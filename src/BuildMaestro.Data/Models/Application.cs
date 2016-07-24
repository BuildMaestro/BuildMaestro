using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Data.Models
{
    public class ApplicationSetting
    {
        [Key]
        public ApplicationSettingKey Key { get; set; }

        public string Value { get; set; }
    }

    public enum ApplicationSettingKey
    {
        Workspace = 0,
        GitExecutable = 1,
        DateTimeFormat = 2
    }
}
