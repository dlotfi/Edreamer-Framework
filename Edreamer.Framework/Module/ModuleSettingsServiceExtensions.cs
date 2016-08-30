using System.Web;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Module
{
    public static class ModuleSettingsServiceExtensions
    {
        public static string GetModulesPath(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            var modulesPath = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Module", Name = "ModulesPath" });
            return VirtualPathUtility.AppendTrailingSlash(PathHelpers.GetVirtualPath(modulesPath));
        }
    }
}
