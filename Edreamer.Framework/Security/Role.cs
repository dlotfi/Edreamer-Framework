using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Security
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public bool SystemRole
        {
            get { return IsSystemRole(Name); }
        }

        public static bool IsSystemRole(string roleName)
        {
            return roleName != null &&
                   (roleName.EqualsIgnoreCase("Administrator") ||
                    roleName.EqualsIgnoreCase("Anonymous") ||
                    roleName.EqualsIgnoreCase("Authenticated"));
        }
    }
}
