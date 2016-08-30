using System.Collections.Generic;

namespace Edreamer.Framework.Security.Roles
{
    public interface IRoleService
    {
        IEnumerable<Role> GetRoles();
        Role AddRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(int id);

        Role GetRole(int id);
        Role GetRole(string name);

        void SetPermissionsOfRole(int id, IEnumerable<Permission> permissions);
        IEnumerable<Permission> GetPermissionsOfRole(int id);
        IEnumerable<Permission> GetPermissionsOfRole(string name);
        bool VerifyRoleNameUnicity(string roleName, int? id = null);
    }
}