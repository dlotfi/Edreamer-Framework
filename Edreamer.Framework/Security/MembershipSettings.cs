// Based on Orchard CMS

namespace Edreamer.Framework.Security
{
    public class MembershipSettings
    {
        public bool EnablePasswordRetrieval { get; set; }
        public bool EnablePasswordReset { get; set; }
        public int MaxInvalidPasswordAttempts { get; set; }
        public int PasswordAttemptWindow { get; set; }
        public bool RequiresUniqueEmail { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public string PasswordHashAlgorithm { get; set; }
        public int MinRequiredPasswordLength { get; set; }
        public int MinRequiredNonAlphanumericCharacters { get; set; }
        public string PasswordStrengthRegularExpression { get; set; }
    }
}