using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Security.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly IFrameworkDataContext _dataContext;

        protected ISet<PermissionInfo> Permissions
        {
            get
            {
                return Cache.Get("Permissions", ctx => new HashSet<PermissionInfo>(new KeyEqualityComparer<PermissionInfo>(pi => pi.Permission)));
            }
        }

        public ICache Cache { get; set; }

        public PermissionService(IFrameworkDataContext dataContext)
        {
            Throw.IfArgumentNull(dataContext, "dataContext");
            _dataContext = dataContext;
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return Permissions.Select(pi => pi.Permission);
        }

        public void RegisterPermission(Permission permission, string moduleName)
        {
            // ToDo-Low [01241214]: Validate permission
            Throw.IfArgumentNull(permission, "permission");
            Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
            if (!Permissions.Add(new PermissionInfo(permission, moduleName)))
            {
                var duplicatePermissionInfo = Permissions
                        .SingleOrDefault(pi => pi.Permission == permission && !pi.ModuleName.EqualsIgnoreCase(moduleName));
                Throw.If(duplicatePermissionInfo != null)
                    .A<PermissionException>("Another permission named {0} already registered for module {1}"
                    .FormatWith(permission.Name, duplicatePermissionInfo == null ? "" : duplicatePermissionInfo.ModuleName));
                //Bug [01271734]: Checking for null because of the bug in exception throwing mechanism
            }
        }

        public string GetPermissionModuleName(string name)
        {
            var permissionInfo = Permissions.SingleOrDefault(pi => pi.Permission.Name.EqualsIgnoreCase(name));
            Throw.IfNull(permissionInfo)
                .A<PermissionException>("No permission named {0} exists.".FormatWith(name));
            return permissionInfo.ModuleName;
        }

        public Permission GetPermission(string name)
        {
            var permissionInfo = Permissions.SingleOrDefault(pi => pi.Permission.Name.EqualsIgnoreCase(name));
            Throw.IfNull(permissionInfo)
                .A<PermissionException>("No permission named {0} exists.".FormatWith(name));
            return permissionInfo.Permission;
        }

        public void DeleteModulePermissions(string moduleName)
        {
            moduleName = moduleName.ToLower();
            var rolePermissionsToDelete = _dataContext.RolesPermissions.Where(rp => rp.ModuleName.ToLower() == moduleName);
            rolePermissionsToDelete.ForEach(rp => _dataContext.RolesPermissions.Remove(rp));
            _dataContext.SaveChanges();
        }

        protected class PermissionInfo
        {
            public PermissionInfo(Permission permission, string moduleName)
            {
                Throw.IfArgumentNull(permission, "permission");
                Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
                Permission = permission;
                ModuleName = moduleName;
            }

            public Permission Permission { get; private set; }
            public string ModuleName { get; private set; }
        }
    }
}
