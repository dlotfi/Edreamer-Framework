using System.Collections.Generic;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Security
{
    public class SecuritySettingsRegistrar : ISettingRegistrar
    {
        public void RegisterSettings(ICollection<SettingEntryInfo> settingEntries)
        {
            settingEntries.Add(new SettingEntryInfo
            {
                ProviderName = "RepositorySettingsProvider",
                Key = new SettingEntryKey { Category = "Security", Name = "SuperUser" },
                Value = "admin"
            });
        }
    }
}
