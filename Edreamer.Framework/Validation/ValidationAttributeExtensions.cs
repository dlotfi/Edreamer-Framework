using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Validation
{
    public static class ValidationAttributeExtensions
    {
        public static string GetErrorMessageString(this ValidationAttribute attribute)
        {
            Throw.IfArgumentNull(attribute, "attribute");

            var errorMessageStringProperty = attribute.GetType().GetPropertyInfo("ErrorMessageString", typeof (string), PropertyAccessRequired.Get);
            if (errorMessageStringProperty == null)
                return null;

            return errorMessageStringProperty.GetValue(attribute, null) as string;
        }
    }
}
