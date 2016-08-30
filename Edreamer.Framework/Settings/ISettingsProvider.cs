namespace Edreamer.Framework.Settings
{
    public interface ISettingsProvider
    {
        bool TryGetSetting<T>(SettingEntryKey key, out T value);
        bool SettingExists(SettingEntryKey key);
        string ProviderName { get; }
    }

    public interface IWritableSettingsProvider: ISettingsProvider
    {
        bool TrySetSetting<T>(SettingEntryKey key, T value);
        void DefineSetting(SettingEntryKey key, object initialValue, string moduleName);
        void DeleteSettings(string moduleName);
    }
}
