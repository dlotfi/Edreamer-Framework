// Based on Orchard CMS

using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Module;
using Edreamer.Framework.Security.Permissions;
using Edreamer.Framework.Security.Roles;

namespace Edreamer.Framework.Security
{
    public class RoleUpdater : IModuleEventHandler
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IEnumerable<Lazy<IPermissionRegistrar, IModuleMetadata>> _permissionRegistrars;

        public ILogger Logger { get; set; }

        public RoleUpdater(IRoleService roleService, IPermissionService permissionService,
            IEnumerable<Lazy<IPermissionRegistrar, IModuleMetadata>> permissionRegistrars)
        {
            Throw.IfArgumentNull(roleService, "roleService");
            Throw.IfArgumentNull(permissionService, "permissionService");
            _permissionRegistrars = CollectionHelpers.EmptyIfNull(permissionRegistrars);
            _roleService = roleService;
            _permissionService = permissionService;
        }

        public void Installed(string moduleName)
        {
            var permissionRegistrars = _permissionRegistrars
                .Where(pr => pr.Metadata.ModuleName.EqualsIgnoreCase(moduleName))
                .Select(pr => pr.Value);

            if (permissionRegistrars.Any())
            {
                Logger.Debug("Configuring permissions and default roles for installed module {0}", moduleName);
            }

            foreach (var permissionRegistrar in permissionRegistrars)
            {
                var permissionStereotypes = new List<PermissionStereotype>();
                permissionRegistrar.RegisterDefaultStereotypes(permissionStereotypes);
                foreach (var stereotype in permissionStereotypes)
                {
                    var role = _roleService.GetRole(stereotype.RoleName);
                    if (role == null)
                    {
                        Logger.Error("Cannot find role {0} for permission stereotype in module {1}", stereotype.RoleName, moduleName);
                        continue;
                    }
                    var currentPermissions = _roleService.GetPermissionsOfRole(role.Id).ToList();
                    var newPermissions = currentPermissions.Union(stereotype.Permissions).Distinct();
                    if (!currentPermissions.SequenceEqual(newPermissions))
                    {
                        _roleService.SetPermissionsOfRole(role.Id, newPermissions);
                    }
                }
            }
        }

        public void Uninstalled(string moduleName)
        {
            Logger.Debug("Deleting permissions for uninstalled module {0}", moduleName);
            _permissionService.DeleteModulePermissions(moduleName);
        }
    }
}
