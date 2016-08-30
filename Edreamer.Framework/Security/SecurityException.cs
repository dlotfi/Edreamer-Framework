// Based on Orchard CMS

using System;
using System.Runtime.Serialization;

namespace Edreamer.Framework.Security
{
    [Serializable]
    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message) { }

        public SecurityException(string message, Exception innerException) : base(message, innerException) { }

        public SecurityException(string message, SecurityExceptionInfo info)
            : this(message, null, info)
        {
        }

        public SecurityException(string message, string localMessage)
            : this(message, localMessage, null)
        {
        }

        public SecurityException(string message, string localMessage, SecurityExceptionInfo info)
            : base(message)
        {
            LocalMessage = localMessage;
            Info = info ?? new SecurityExceptionInfo();
        }

        protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public SecurityExceptionInfo Info { get; set; }
        public string LocalMessage { get; set; }
    }

    public class SecurityExceptionInfo
    {
        public string PermissionName { get; set; }
        public User User { get; set; }
        public object Content { get; set; }
    }
}
