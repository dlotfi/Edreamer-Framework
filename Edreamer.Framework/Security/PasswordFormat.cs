namespace Edreamer.Framework.Security
{
    public enum PasswordFormat
    {
        /// <summary>
        /// Passwords are not encrypted.
        /// </summary>
        Clear = 0,
        
        /// <summary>
        /// Passwords are encrypted one-way using the SHA1 hashing algorithm.
        /// </summary>
        Hashed = 1,

        /// <summary>
        /// Passwords are encrypted using the encryption settings determined by the machineKey
        /// Element (ASP.NET Settings Schema) element configuration.
        /// </summary>
        Encrypted = 2,
    }
}
