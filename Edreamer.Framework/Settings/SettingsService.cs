using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IEnumerable<ISettingsProvider> _settingsProviders;


        public SettingsService(IEnumerable<ISettingsProvider> settingsProviders)
        {
            Throw.IfArgumentNullOrEmpty(settingsProviders, "settingsProviders");
            _settingsProviders = settingsProviders;
        }

        public virtual bool SettingExists(SettingEntryKey key)
        {
            return _settingsProviders.Any(p => p.SettingExists(key));
        }

        public virtual bool TryGetSetting<T>(SettingEntryKey key, out T value)
        {
            Throw.IfArgumentNull(key, "key");
            var provider = _settingsProviders.FirstOrDefault(p => p.SettingExists(key));
            value = default(T);
            return provider != null && provider.TryGetSetting(key, out value);
        }

        public virtual bool TrySetSetting<T>(SettingEntryKey key, T value)
        {
            Throw.IfArgumentNull(key, "key");
            var provider = (IWritableSettingsProvider)_settingsProviders.FirstOrDefault(p => p.SettingExists(key) && p is IWritableSettingsProvider);
            return provider != null && provider.TrySetSetting(key, value);
        }

        public T GetSetting<T>(SettingEntryKey key)
        {
            Throw.IfArgumentNull(key, "key");
            T value;
            Throw.IfNot(TryGetSetting(key, out value))
                .AnArgumentException("No provider has a setting entry named '{0}' in category '{1}'.".FormatWith(key.Name, key.Category), "key");
            return value;
        }

        public void SetSetting<T>(SettingEntryKey key, T value)
        {
            Throw.IfArgumentNull(key, "key");
            Throw.IfNot(TrySetSetting(key, value))
                .AnArgumentException("No writable provider has a setting entry named '{0}' in category '{1}'.".FormatWith(key.Name, key.Category), "key");
        }
    }
}
