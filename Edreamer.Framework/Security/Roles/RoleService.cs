using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Injection;
using Edreamer.Framework.Logging;
using Edreamer.Framework.Module;
using Edreamer.Framework.Security.Permissions;

namespace Edreamer.Framework.Security.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IFrameworkDataContext _dataContext;
        private readonly IPermissionService _permissionService;
        private readonly IModuleManager _moduleManager;

        public RoleService(IFrameworkDataContext dataContext, IPermissionService permissionService, IModuleManager moduleManager)
        {

            Throw.IfArgumentNull(dataContext, "dataContext");
            Throw.IfArgumentNull(permissionService, "permissionService");
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            _dataContext = dataContext;
            _permissionService = permissionService;
            _moduleManager = moduleManager;
        }

        public ILogger Logger { get; set; }

        public IEnumerable<Role> GetRoles()
        {
            return _dataContext.Roles.ToList()
                .Select(r => Injector.PlaneInject(new Role(), r));
        }

        public Role AddRole(Role role)
        {
            // ToDo-Low [01231613]: Validate role
            Throw.IfArgumentNull(role, "role");
            Logger.Information("CreateRole {0}", role.Name);
            var roleEntity = Injector.PlaneInject(new Domain.Role(), role);
            _dataContext.Roles.Add(roleEntity);
            _dataContext.SaveChanges();
            return Injector.PlaneInject(new Role(), roleEntity);
        }

        public void UpdateRole(Role role)
        {
            // ToDo-Low [01231624]: Validate role
            Throw.IfArgumentNull(role, "role");
            var roleEntity = Injector.PlaneInject(new Domain.Role(), role);
            _dataContext.Roles.Update(roleEntity);
            _dataContext.SaveChanges();
        }

        public void DeleteRole(int id)
        {
            var role = _dataContext.Roles.Find(id);
            Throw.IfNull(role).AnArgumentException("No role with id {0} found.".FormatWith(id), "id");
            Throw.If(Role.IsSystemRole(role.Name)).A<InvalidOperationException>("Cannot delete a system role.");
            _dataContext.Roles.Remove(role);
            _dataContext.SaveChanges();
        }

        public Role GetRole(int id)
        {
            var roleEntity = _dataContext.Roles.Find(id);
            return Injector.PlaneInject(new Role(), roleEntity);
        }

        public Role GetRole(string name)
        {
            Throw.IfArgumentNullOrEmpty(name, "name");
            var lowerName = name.ToLower();
            var roleEntity = _dataContext.Roles.SingleOrDefault(r => r.Name.ToLower() == lowerName);
            return Injector.PlaneInject(new Role(), roleEntity);
        }

        public void SetPermissionsOfRole(int id, IEnumerable<Permission> permissions)
        {
            var permissionsToSet = CollectionHelpers.EmptyIfNull(permissions).Select(p => p.Name);
            var currentPermission = _dataContext.RolesPermissions
                .Where(rp => rp.RoleId == id)
                .Select(rp => rp.PermissionName)
                .ToList();

            foreach (var permission in currentPermission.Except(permissionsToSet, StringComparer.OrdinalIgnoreCase))
            {
                _dataContext.RolesPermissions.Remove(null, id, permission);
            }

            foreach (var permission in permissionsToSet.Except(currentPermission, StringComparer.OrdinalIgnoreCase))
            {
                _dataContext.RolesPermissions.Add(new RolePermission
                                                      {
                                                          RoleId = id,
                                                          PermissionName = permission,
                                                          ModuleName = _permissionService.GetPermissionModuleName(permission)
                                                      });
            }
            _dataContext.SaveChanges();
        }

        public IEnumerable<Permission> GetPermissionsOfRole(int id)
        {
            var permissions = _dataContext.RolesPermissions.Where(rp => rp.RoleId == id).ToList();
            return permissions
                .Where(p => _moduleManager.ModuleExists(p.ModuleName))
                .Select(p => _permissionService.GetPermission(p.PermissionName));
        }

        public IEnumerable<Permission> GetPermissionsOfRole(string name)
        {
            Throw.IfArgumentNullOrEmpty(name, "name");
            var lowerName = name.ToLower();
            var permissions = _dataContext.RolesPermissions.Where(rp => rp.Role.Name.ToLower() == lowerName).ToList();
            return permissions
                .Where(p => _moduleManager.ModuleExists(p.ModuleName))
                .Select(p => _permissionService.GetPermission(p.PermissionName));
        }

        public bool VerifyRoleNameUnicity(string roleName, int? id = null)
        {
            Throw.IfArgumentNullOrEmpty(roleName, "roleName");
            roleName = roleName.ToLower();
            return id == null
                       ? !_dataContext.Roles.Any(r => r.Name.ToLower() == roleName)
                       : !_dataContext.Roles.Any(r => r.Id != id && r.Name.ToLower() == roleName);
        }
    }
}
