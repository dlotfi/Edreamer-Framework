using Edreamer.Framework.Data;

namespace Edreamer.Framework.Domain
{
    public interface IFrameworkDataContext : IDataContext
    {
        IRepository<Module> Modules { get; }
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<RolePermission> RolesPermissions { get; }
        IRepository<Media> Media { get; }
        IRepository<Setting> Settings { get; }
    }
}
