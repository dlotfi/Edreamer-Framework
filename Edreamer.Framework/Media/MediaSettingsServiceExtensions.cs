using System.Collections.Generic;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Media
{
    public static class MediaSettingsServiceExtensions
    {
        public static string GetStoragePath(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return settingsService.GetSetting<string>(new SettingEntryKey { Category = "Media", Name = "StoragePath" });
        }

        public static IEnumerable<string> GetAcceptableMediaTypes(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return settingsService.GetSetting<IEnumerable<string>>(new SettingEntryKey { Category = "Media", Name = "AcceptableMediaTypes" });
        }

        public static IEnumerable<string> GetUnsafeExtensions(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return settingsService.GetSetting<IEnumerable<string>>(new SettingEntryKey { Category = "Media", Name = "UnsafeExtensions" });
        }
    }
}
