using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class MvcDataAnnotationsValidatorAdapter : DataAnnotationsValidatorAdapter, IClientValidatable
    {
        public MvcDataAnnotationsValidatorAdapter(ObjectMetadata metadata, ValidationAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        protected string GetAttributeErrorMessage(string name)
        {
            var errorMessageString = Attribute.GetErrorMessageString();
            if (errorMessageString != null)
                Attribute.ErrorMessage = Localizer(errorMessageString, LocalizerFormatting.Skip);
            
            return Attribute.FormatErrorMessage(name);
        }

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidatable = Attribute as IClientValidatable;
            return clientValidatable != null
                ? clientValidatable.GetClientValidationRules(metadata, context)
                : Enumerable.Empty<ModelClientValidationRule>();
        }
    }

    public class MvcDataAnnotationsValidatorAdapter<TAttribute> : MvcDataAnnotationsValidatorAdapter
        where TAttribute : ValidationAttribute
    {
        public MvcDataAnnotationsValidatorAdapter(ObjectMetadata metadata, TAttribute attribute, Localizer localizer)
            : base(metadata, attribute, localizer)
        {
        }

        protected new TAttribute Attribute
        {
            get { return (TAttribute)base.Attribute; }
        }
    }
}
