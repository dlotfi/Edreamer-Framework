using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc.Validation
{
    public class ModelValidatorProviderAdapter: ModelValidatorProvider
    {
        private readonly IEnumerable<IValidatorProvider> _validatorProviders;

        public ModelValidatorProviderAdapter(IEnumerable<IValidatorProvider> validatorProviders)
        {
            _validatorProviders = CollectionHelpers.EmptyIfNull(validatorProviders);
        }

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var objectMetadata = ConvertModelMetadataToObjectMetadata(metadata);
            foreach (var validatorProvider in _validatorProviders)
            {
                foreach (var validator in validatorProvider.GetValidators(objectMetadata))
                {
                    yield return new ModelValidatorAdapter(metadata, context, validator);
                }
            }
        }

        private static ObjectMetadata ConvertModelMetadataToObjectMetadata(ModelMetadata modelMetadata)
        {
            return new ObjectMetadata(modelMetadata.ModelType, modelMetadata.ContainerType,
                                      modelMetadata.PropertyName,
                                      // Note that we can use the name of the property as qualified name because Mvc3
                                      // performs validation just for two levels: model itself and properties of the model.
                                      // See CompositeModelValidator class in Mvc3 source code.
                                      modelMetadata.PropertyName,
                                      container => GetPropertiesMetadata(modelMetadata));
        }

        private static IEnumerable<ObjectMetadata> GetPropertiesMetadata(ModelMetadata modelMetadata)
        {
            Throw.IfArgumentNull(modelMetadata, "modelMetadata");
            return modelMetadata.Properties.Select(ConvertModelMetadataToObjectMetadata);
        }
    }
}
