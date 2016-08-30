using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class ClientDataTypeValidatorProvider : IValidatorProvider
    {
        private readonly ILocalizerProvider _localizerProvider;

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new Type[] {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        });

        public ClientDataTypeValidatorProvider(ILocalizerProvider localizerProvider)
        {
            Throw.IfArgumentNull(localizerProvider, "localizerProvider");
            _localizerProvider = localizerProvider;
        }

        public IEnumerable<IValidator> GetValidators(ObjectMetadata metadata)
        {
            Throw.IfArgumentNull(metadata, "metadata");
            if (IsNumericType(metadata.Type))
            {
                yield return new NumericValidator(_localizerProvider.GetLocalizer(typeof(NumericValidator).FullName));
            }
        }

        private static bool IsNumericType(Type type) {
            var underlyingType = Nullable.GetUnderlyingType(type); // strip off the Nullable<>
            return NumericTypes.Contains(underlyingType ?? type);
        }
    }

    internal class NumericValidator : IValidator, IClientValidatable
    {
        private readonly Localizer _localizer;

        public NumericValidator(Localizer localizer)
        {
            Throw.IfArgumentNull(localizer, "localizer");
            _localizer = localizer;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "number",
                ErrorMessage = _localizer("The field {0} must be a number.", metadata.GetDisplayName())
            };
        }

        public IEnumerable<ValidationResult> Validate(object instance, object container)
        {
            // this is not a server-side validator
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
