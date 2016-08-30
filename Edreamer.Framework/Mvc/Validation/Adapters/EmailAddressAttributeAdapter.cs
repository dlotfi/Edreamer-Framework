using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class EmailAddressAttributeAdapter : MvcDataAnnotationsValidatorAdapter<EmailAddressAttribute>
    {
        public EmailAddressAttributeAdapter(ObjectMetadata metadata, EmailAddressAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            yield return new ModelClientValidationRule
            {
                ValidationType = "email",
                ErrorMessage = errorMessage
            };
        }
    }
}
