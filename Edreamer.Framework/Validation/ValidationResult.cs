using System;
using System.Globalization;

namespace Edreamer.Framework.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
            : this(null, null, null)
        {
        }

        public ValidationResult(object value, string memberName, string localizedMessage)
        {
            Value = value;
            MemberName = memberName ?? "";
            LocalizedMessage = localizedMessage ?? "";
        }

        public object Value { get; set; }
        public string MemberName { get; set; }
        public string LocalizedMessage { get; set; }

        public string GetFormattedMessage(string name)
        {
            return String.IsNullOrEmpty(LocalizedMessage)
                ? ""
                : String.Format(CultureInfo.CurrentUICulture, LocalizedMessage, name);
        }
    }
}
