using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Settings
{
    public class SettingsUpdater : IModuleEventHandler
    {
        private readonly IEnumerable<IWritableSettingsProvider> _settingsProviders;
        private readonly IEnumerable<Lazy<ISettingRegistrar, IModuleMetadata>> _settingRegistrars;

        public ILogger Logger { get; set; }

        public SettingsUpdater(IEnumerable<IWritableSettingsProvider> settingsProviders,
            IEnumerable<Lazy<ISettingRegistrar, IModuleMetadata>> settingRegistrars)
        {
            _settingsProviders = CollectionHelpers.EmptyIfNull(settingsProviders);
            _settingRegistrars = CollectionHelpers.EmptyIfNull(settingRegistrars);
        }

        public void Installed(string moduleName)
        {
            var settingRegistrars = _settingRegistrars
                .Where(pr => pr.Metadata.ModuleName.EqualsIgnoreCase(moduleName))
                .Select(pr => pr.Value);

            var allSettingEntries = new List<SettingEntryInfo>();
            foreach (var settingRegistrar in settingRegistrars)
            {
                var settingsEntries = new List<SettingEntryInfo>();
                settingRegistrar.RegisterSettings(settingsEntries);
                allSettingEntries.AddRange(settingsEntries);
            }

            if (allSettingEntries.Any())
            {
                Logger.Debug("Configuring settings for installed module {0}", moduleName);
            }

            foreach (var item in allSettingEntries
                .GroupBy(e => e.ProviderName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase))
            {
                var provider = _settingsProviders.SingleOrDefault(p => p.ProviderName.EqualsIgnoreCase(item.Key));
                Throw.IfNull(provider)
                    .A<SettingRegistrationException>("No writable provider with the name '{0}' exists.".FormatWith(item.Key));
                item.Value.ForEach(entry => provider.DefineSetting(entry.Key, entry.Value, moduleName));
            }
        }

        public void Uninstalled(string moduleName)
        {
            Logger.Debug("Deleting settings for uninstalled module {0}", moduleName);
            _settingsProviders.ForEach(p => p.DeleteSettings(moduleName));
        }
    }
}