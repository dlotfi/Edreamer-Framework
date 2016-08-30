// Based on Orchard CMS

namespace Edreamer.Framework.Security.Encryption
{
    public class EncryptionSettings
    {
        public string EncryptionAlgorithm { get; set; }
        public string EncryptionKey { get; set; }
        public string HashAlgorithm { get; set; }
        public string HashKey { get; set; }
    }
}
