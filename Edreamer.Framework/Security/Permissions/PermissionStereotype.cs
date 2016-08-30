// Based on Orchard CMS

using System.Collections.Generic;

namespace Edreamer.Framework.Security.Permissions
{
    public class PermissionStereotype
    {
        public string RoleName { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
