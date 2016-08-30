using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class FileExtensionsAttributeAdapter : MvcDataAnnotationsValidatorAdapter<FileExtensionsAttribute>
    {
        public FileExtensionsAttributeAdapter(ObjectMetadata metadata, FileExtensionsAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = GetAttributeErrorMessage(metadata.GetDisplayName());
            var rule = new ModelClientValidationRule
            {
                ValidationType = "accept",
                ErrorMessage = errorMessage
            };
            rule.ValidationParameters["exts"] = Attribute.Extensions;
            yield return rule;
        }
    }
}
