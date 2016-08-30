using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.DataAnnotations;
using Edreamer.Framework.Security;

namespace Edreamer.Framework.Domain
{
    public class User
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }

        [Required, ShortString]
        public string Username { get; set; }
        [Required, ShortString, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordSalt { get; set; }
        [Required, ShortString]
        public string HashAlgorithm { get; set; }
        public PasswordFormat PasswordFormat { get; set; }

        public bool Approved { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Disabled { get; set; }

        public string UserData { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
