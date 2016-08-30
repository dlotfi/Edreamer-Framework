using Edreamer.Framework.Helpers;
using Edreamer.Framework.Security.Encryption;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Security
{
    public static class SecuritySettingsServiceExtensions
    {
        public static string GetSuperUser(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return settingsService.GetSetting<string>(new SettingEntryKey { Category = "Security", Name = "SuperUser" });
        }

        public static void SetSuperUser(this ISettingsService settingsService, string superUser)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(superUser, "superUser");
            settingsService.SetSetting(new SettingEntryKey { Category = "Security", Name = "SuperUser" }, superUser);
        }

        public static MembershipSettings GetMembershipSettings(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return new MembershipSettings
                {
                    EnablePasswordRetrieval = settingsService.GetSetting<bool>(new SettingEntryKey { Category = "Membership", Name = "EnablePasswordRetrieval" }),
                    EnablePasswordReset = settingsService.GetSetting<bool>(new SettingEntryKey { Category = "Membership", Name = "EnablePasswordReset" }),
                    RequiresUniqueEmail = settingsService.GetSetting<bool>(new SettingEntryKey { Category = "Membership", Name = "RequiresUniqueEmail" }),
                    MaxInvalidPasswordAttempts = settingsService.GetSetting<int>(new SettingEntryKey { Category = "Membership", Name = "MaxInvalidPasswordAttempts" }),
                    PasswordAttemptWindow = settingsService.GetSetting<int>(new SettingEntryKey { Category = "Membership", Name = "PasswordAttemptWindow" }),
                    PasswordFormat = settingsService.GetSetting<PasswordFormat>(new SettingEntryKey { Category = "Membership", Name = "PasswordFormat" }),
                    PasswordHashAlgorithm = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Membership", Name = "PasswordHashAlgorithm" }),
                    MinRequiredPasswordLength = settingsService.GetSetting<int>(new SettingEntryKey { Category = "Membership", Name = "MinRequiredPasswordLength" }),
                    MinRequiredNonAlphanumericCharacters = settingsService.GetSetting<int>(new SettingEntryKey { Category = "Membership", Name = "MinRequiredNonAlphanumericCharacters" }),
                    PasswordStrengthRegularExpression = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Membership", Name = "PasswordStrengthRegularExpression" }),
                };
        }

        public static EncryptionSettings GetEncryptionSettings(this ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            return new EncryptionSettings
                {
                    EncryptionAlgorithm = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Encryption", Name = "EncryptionAlgorithm" }),
                    EncryptionKey = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Encryption", Name = "EncryptionKey" }),
                    HashAlgorithm = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Encryption", Name = "HashAlgorithm" }),
                    HashKey = settingsService.GetSetting<string>(new SettingEntryKey { Category = "Encryption", Name = "HashKey" })
                };
        }
    }
}
