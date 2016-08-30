using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly IEnumerable<IValidatorProvider> _validatorProviders;
        private readonly IMetadataProvider _metadataProvider;

        
        public ValidationService(IEnumerable<IValidatorProvider> validatorProviders, IMetadataProvider metadataProvider)
        {
            Throw.IfArgumentNullOrEmpty(validatorProviders, "validatorProviders");
            Throw.IfArgumentNull(metadataProvider, "metadataProvider");
            _validatorProviders = validatorProviders;
            _metadataProvider = metadataProvider;            
        }

        public virtual IEnumerable<ValidationResult> Validate(object instance, bool recursive = true)
        {
            Throw.IfArgumentNull(instance, "instance");
            var metadata = _metadataProvider.GetMetadata(instance.GetType());
            return Validate(instance, metadata, recursive);
        }

        private IEnumerable<ValidationResult> Validate(object instance, ObjectMetadata metadata, bool recursive)
        {
            var propertiesValid = true;

            foreach (var propertyMetadata in metadata.Properties)
            {
                var propertyDescriptor = metadata.Type.GetTypeDescriptor().GetProperties().Find(propertyMetadata.Name, true);
                var propertyValue = propertyDescriptor.GetValue(instance);
                foreach (var propertyValidator in _validatorProviders.SelectMany(vp => vp.GetValidators(propertyMetadata)).ToList())
                {
                    foreach (var propertyResult in recursive ? Validate(propertyValue, propertyMetadata, true)
                                                             : propertyValidator.Validate(propertyValue, instance))
                    {
                        propertiesValid = false;
                        propertyResult.MemberName = propertyMetadata.QualifiedName;
                        yield return propertyResult;
                    }
                }
            }

            if (propertiesValid)
            {
                foreach (var typeValidator in _validatorProviders.SelectMany(vp => vp.GetValidators(metadata)).ToList())
                {
                    foreach (var typeResult in typeValidator.Validate(instance, null))
                    {
                        yield return typeResult;
                    }
                }
            }
        }
    }
}
