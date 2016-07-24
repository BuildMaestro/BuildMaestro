using BuildMaestro.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.Service
{
    public class ApplicationSettingService
    {
        public string GetSetting(Data.Models.ApplicationSettingKey key)
        {
            var settingValue = string.Empty;

            using (var context = new BuildMaestroContext())
            {
                var setting = context.AplicationSettings.FirstOrDefault(x => x.Key == key);

                if (setting != null)
                {
                    settingValue = setting.Value;
                }
            }

            return settingValue;
        }
    }
}
