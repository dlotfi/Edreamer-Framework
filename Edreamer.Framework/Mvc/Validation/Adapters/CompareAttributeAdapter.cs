using System.Collections.Generic;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class CompareAttributeAdapter : MvcDataAnnotationsValidatorAdapter<System.ComponentModel.DataAnnotations.CompareAttribute>
    {
        public CompareAttributeAdapter(ObjectMetadata metadata, System.ComponentModel.DataAnnotations.CompareAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            yield return new ModelClientValidationEqualToRule(errorMessage, "*." + Attribute.OtherProperty);
        }
    }
}
