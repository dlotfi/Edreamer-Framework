namespace Edreamer.Framework.Domain
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public string PermissionName { get; set; }
        public string ModuleName { get; set; }
    }
}
