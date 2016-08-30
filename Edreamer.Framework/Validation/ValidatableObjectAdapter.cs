using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;

namespace Edreamer.Framework.Validation
{
    public class ValidatableObjectAdapter : IValidator
    {
        public ValidatableObjectAdapter(ObjectMetadata metadata, Localizer localizer)
        {
            Throw.IfArgumentNull(metadata, "metadata");
            Throw.IfArgumentNull(localizer, "localizer");
            Metadata = metadata;
            Localizer = localizer;
        }

        public ObjectMetadata Metadata { get; private set; }

        protected Localizer Localizer { get; private set; }

        [Import]
        public ICompositionContainerAccessor CompositionContainerAccessor { get; set; }

        public IEnumerable<ValidationResult> Validate(object instance, object container)
        {
            var validatable = instance as IValidatableObject;
            Throw.IfNull(validatable)
                .A<ArgumentException>("The instance should be compatible with {0}, but was actually {1}."
                .FormatWith(typeof(IValidatableObject).FullName, instance.GetType().FullName));

            const string placeHolder = "*DISPLAYNAME_PLACEHOLDER*";
            // NOTE (from Mvc3 source): Container is never used here, because IValidatableObject doesn't give you
            // any way to get access to your container.
            // var context = new ValidationContext(container ?? instance, null, null);
            var context = new ValidationContext(instance, null, null);
            context.MemberName = Metadata.Name;
            context.DisplayName = placeHolder;

            // Allow dependency injection in validatable objects
            CompositionContainerAccessor.Container.SatisfyImportsOnce(validatable);

            var results = validatable.Validate(context);
            foreach (var result in results)
            {
                if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
                {
                    if (CollectionHelpers.IsNullOrEmpty(result.MemberNames))
                    {
                        yield return new ValidationResult(instance,
                                                          null,
                                                          result.ErrorMessage.UnformatWith(placeHolder)); // Validatable objects should localize their error messages
                    }
                    else
                    {
                        foreach (var memberName in result.MemberNames)
                        {
                            yield return new ValidationResult(instance,
                                                              memberName,
                                                              result.ErrorMessage.UnformatWith(placeHolder)); // Validatable objects should localize their error messages
                        }
                    }
                }
            }
        }
    }
}
