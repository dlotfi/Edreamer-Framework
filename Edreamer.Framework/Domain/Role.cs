using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.DataAnnotations;

namespace Edreamer.Framework.Domain
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        [Required, ShortString]
        public string Name { get; set; }
        [Required, ShortString]
        public string DisplayName { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}
