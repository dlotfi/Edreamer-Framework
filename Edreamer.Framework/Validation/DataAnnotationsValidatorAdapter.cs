using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;

namespace Edreamer.Framework.Validation
{
    public class DataAnnotationsValidatorAdapter : IValidator
    {
        public DataAnnotationsValidatorAdapter(ObjectMetadata metadata, ValidationAttribute attribute, Localizer localizer)
        {
            Throw.IfArgumentNull(metadata, "metadata");
            Throw.IfArgumentNull(attribute, "attribute");
            Throw.IfArgumentNull(localizer, "localizer");
            Metadata = metadata;
            Attribute = attribute;
            Localizer = localizer;
        }

        public ObjectMetadata Metadata { get; private set; }

        protected ValidationAttribute Attribute { get; private set; }

        protected Localizer Localizer { get; private set; }

        public IEnumerable<ValidationResult> Validate(object instance, object container)
        {
            const string placeHolder = "*DISPLAYNAME_PLACEHOLDER*";
            var context = new ValidationContext(container ?? instance, null, null);
            context.MemberName = Metadata.Name;
            context.DisplayName = placeHolder;

            var errorMessageString = Attribute.GetErrorMessageString();
            if (errorMessageString != null)
                Attribute.ErrorMessage = Localizer(errorMessageString, LocalizerFormatting.Skip);
            
            var result = Attribute.GetValidationResult(instance, context);
            if (result != null && result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                yield return new ValidationResult(instance, 
                                                  Metadata.Name,
                                                  result.ErrorMessage.UnformatWith(placeHolder));
            }
        }

    }
}
