using System.Collections.Generic;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Security.Permissions
{
    public class PermissionPublisher: IPermissionPublisher
    {
        private readonly IModuleManager _moduleManager;
        private readonly IPermissionService _permissionService;

        public PermissionPublisher(IModuleManager moduleManager, IPermissionService permissionService)
        {
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            Throw.IfArgumentNull(permissionService, "permissionService");
            _moduleManager = moduleManager;
            _permissionService = permissionService;
        }

        public void Publish(IEnumerable<IPermissionRegistrar> permissionRegistrars)
        {
            Throw.IfArgumentNull(permissionRegistrars, "permissionRegistrars");

            foreach (var registrar in permissionRegistrars)
            {
                var permissions = new List<Permission>();
                registrar.RegisterPermissions(permissions);
                var moduleName = _moduleManager.GetModule(registrar.GetType()).Name;
                permissions.ForEach(permission => _permissionService.RegisterPermission(permission, moduleName));
            }
        }
    }
}
