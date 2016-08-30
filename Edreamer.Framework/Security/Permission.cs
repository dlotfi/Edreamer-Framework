// Based on Orchard CMS

using System.Collections.Generic;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Security.Permissions;

namespace Edreamer.Framework.Security
{
    public class Permission
    {
        public Permission()
        {
            _impliedBy = new HashSet<Permission>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        private ISet<Permission> _impliedBy;
        public IEnumerable<Permission> ImpliedBy
        {
            get
            {
                var fullImpliedByPermissions = new HashSet<Permission>();
                fullImpliedByPermissions.Add(this);
                foreach (var impliedByPermission in _impliedBy)
                {
                    fullImpliedByPermissions.AddRange(impliedByPermission.ImpliedBy);
                }
                fullImpliedByPermissions.Add(StandardPermissions.SiteOwner);
                return fullImpliedByPermissions;
            }
            set
            {
                _impliedBy = new HashSet<Permission>(CollectionHelpers.EmptyIfNull(value));
            }
        }

        public static Permission Named(string name)
        {
            return new Permission { Name = name };
        }

        #region Equality
        public override bool Equals(object obj)
        {
            return Equals(obj as Permission);
        }

        public bool Equals(Permission other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Name.EqualsIgnoreCase(Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.ToLower().GetHashCode() : 0);
        }

        public static bool operator ==(Permission left, Permission right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Permission left, Permission right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
