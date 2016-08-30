using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class RegularExpressionAttributeAdapter : MvcDataAnnotationsValidatorAdapter<RegularExpressionAttribute>
    {
        public RegularExpressionAttributeAdapter(ObjectMetadata metadata, RegularExpressionAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            yield return new ModelClientValidationRegexRule(errorMessage, Attribute.Pattern);
        }
    }
}
