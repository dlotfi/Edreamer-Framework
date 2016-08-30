using System.Collections.Generic;

namespace Edreamer.Framework.Settings
{
    public interface ISettingRegistrar
    {
        void RegisterSettings(ICollection<SettingEntryInfo> settingEntries);
    }
}
