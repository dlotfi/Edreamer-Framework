using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class ModelValidatorAdapter : ModelValidator
    {
        protected IValidator Validator { get; private set; }

        public ModelValidatorAdapter(ModelMetadata metadata, ControllerContext controllerContext, IValidator validator)
            : base(metadata, controllerContext)
        {
            Throw.IfArgumentNull(validator, "validator");
            Validator = validator;
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            foreach (var validationResult in Validator.Validate(Metadata.Model, container))
            {
                yield return new ModelValidationResult
                                 {
                                     // ToDo-Low [07011815]: Check meaning of MemberName property in ValidationResult and change its
                                     // name or value if needed. It seems that ModelValidationResult.MemberName points to child members.
                                     //MemberName = validationResult.MemberName,
                                     Message = validationResult.GetFormattedMessage(Metadata.GetDisplayName())
                                 };
            }
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var results = base.GetClientValidationRules();

            var clientValidatable = Validator as IClientValidatable;
            if (clientValidatable != null)
            {
                results = results.Concat(clientValidatable.GetClientValidationRules(Metadata, ControllerContext));
            }

            return results;
        }
    }
}
