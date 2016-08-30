using System.Collections.Generic;

namespace Edreamer.Framework.Security.Permissions
{
    public interface IPermissionService
    {
        IEnumerable<Permission> GetPermissions();
        void RegisterPermission(Permission permission, string moduleName);
        string GetPermissionModuleName(string name);
        Permission GetPermission(string name);
        void DeleteModulePermissions(string moduleName);
    }
}
