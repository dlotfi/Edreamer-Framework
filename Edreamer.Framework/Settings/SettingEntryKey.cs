using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Settings
{
    public class SettingEntryKey
    {
        public string Category { get; set; }
        public string Name { get; set; }

        #region Equality
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SettingEntryKey)) return false;
            return Equals((SettingEntryKey)obj);
        }

        public bool Equals(SettingEntryKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Category.EqualsIgnoreCase(Category) && other.Name.EqualsIgnoreCase(Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Category != null ? Category.ToLower().GetHashCode() : 0) * 397) ^
                       (Name != null ? Name.ToLower().GetHashCode() : 0);
            }
        }

        public static bool operator ==(SettingEntryKey left, SettingEntryKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SettingEntryKey left, SettingEntryKey right)
        {
            return !Equals(left, right);
        } 
        #endregion
    }
}
