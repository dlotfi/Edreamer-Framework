using System;
using System.Linq;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Settings.Providers
{
    public class RepositorySettingsProvider: IWritableSettingsProvider
    {
        private readonly IFrameworkDataContext _dataContext;
        
        public RepositorySettingsProvider(IFrameworkDataContext dataContext)
        {
            Throw.IfArgumentNull(dataContext, "dataContext");
            _dataContext = dataContext;
        }

        public bool TryGetSetting<T>(SettingEntryKey key, out T value)
        {
            value = default(T);
            key.Category = key.Category.ToLower();
            key.Name = key.Name.ToLower();
            var settingEntry = _dataContext.Settings
                .SingleOrDefault(s => s.Category.ToLower() == key.Category && s.Name.ToLower() == key.Name);
            if (settingEntry == null) return false;
            var deserializedValue = SerializationHelpers.Deserialize(settingEntry.Value);
            if (deserializedValue == null)
            {
                Throw.IfNot(typeof(T).AllowsNullValue())
                     .A<InvalidOperationException>("The setting entry is null but output type is not allowed null.");
            }
            else
            {
                Throw.IfNot(deserializedValue is T)
                     .A<InvalidOperationException>("The setting entry is of type {0} but output type is {1}.".FormatWith(deserializedValue.GetType(), typeof(T)));
            }
            value = (T)deserializedValue;
            return true;
        }

        public bool TrySetSetting<T>(SettingEntryKey key, T value)
        {
            key.Category = key.Category.ToLower();
            key.Name = key.Name.ToLower();
            var settingEntry = _dataContext.Settings.WithTracking()
                .SingleOrDefault(s => s.Category.ToLower() == key.Category && s.Name.ToLower() == key.Name);
            if (settingEntry == null) return false;
            settingEntry.Value = SerializationHelpers.Serialize(value);
            _dataContext.SaveChanges();
            return true;
        }

        public void DefineSetting(SettingEntryKey key, object initialValue, string moduleName)
        {
            Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
            var setting = new Setting
                              {
                                  Category = key.Category,
                                  Name = key.Name,
                                  ModuleName = moduleName,
                                  Value = SerializationHelpers.Serialize(initialValue)
                              };
            _dataContext.Settings.Add(setting);
            _dataContext.SaveChanges();
        }

        public void DeleteSettings(string moduleName)
        {
            moduleName = moduleName.ToLower();
            var settingEntriesToDelete = _dataContext.Settings.Where(s => s.ModuleName.ToLower() == moduleName);
            settingEntriesToDelete.ForEach(entry => _dataContext.Settings.Remove(entry));
            _dataContext.SaveChanges();
        }

        public bool SettingExists(SettingEntryKey key)
        {
            key.Category = key.Category.ToLower();
            key.Name = key.Name.ToLower();
            return _dataContext.Settings
                .Any(s => s.Category.ToLower() == key.Category && s.Name.ToLower() == key.Name);
        }

        public string ProviderName
        {
            get { return "RepositorySettingsProvider"; }
        }
    }
}
