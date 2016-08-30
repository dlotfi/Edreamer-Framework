using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class RangeAttributeAdapter : MvcDataAnnotationsValidatorAdapter<RangeAttribute>
    {
        public RangeAttributeAdapter(ObjectMetadata metadata, RangeAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            yield return new ModelClientValidationRangeRule(errorMessage, Attribute.Minimum, Attribute.Maximum);
        }
    }
}
