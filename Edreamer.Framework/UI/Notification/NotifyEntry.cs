namespace Edreamer.Framework.UI.Notification
{
    public class NotifyEntry
    {
        public NotifyType Type { get; set; }
        public string Message { get; set; }

        #region Equality
        public override bool Equals(object obj)
        {
            return Equals(obj as NotifyEntry);
        }

        public bool Equals(NotifyEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Message, Message);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }
        #endregion
    }
}