using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Localization
{
    public static class LocalizationSettingsServiceExtensions
    {
        public static string GetDefaultCulture(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return settingsService.GetSetting<string>(new SettingEntryKey { Category = "Localization", Name = "DefaultCulture" });
        }
    }
}
