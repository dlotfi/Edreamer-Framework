using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class StringLengthAttributeAdapter : MvcDataAnnotationsValidatorAdapter<StringLengthAttribute>
    {
        public StringLengthAttributeAdapter(ObjectMetadata metadata, StringLengthAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            yield return new ModelClientValidationStringLengthRule(errorMessage, Attribute.MinimumLength, Attribute.MaximumLength);
        }
    }
}
