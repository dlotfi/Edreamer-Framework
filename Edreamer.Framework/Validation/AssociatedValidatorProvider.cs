// Based on ASP.Net Mvc3 source code (AssociatedValidatorProvider.cs)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Edreamer.Framework.Helpers;


namespace Edreamer.Framework.Validation
{

    public abstract class AssociatedValidatorProvider : IValidatorProvider
    {
        protected virtual ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return type.GetTypeDescriptor();
        }

        public IEnumerable<IValidator> GetValidators(ObjectMetadata metadata)
        {
            Throw.IfArgumentNull(metadata, "metadata");

            if (metadata.ContainerType != null)
            {
                return GetValidatorsForProperty(metadata);
            }

            return GetValidatorsForType(metadata);
        }

        protected abstract IEnumerable<IValidator> GetValidators(ObjectMetadata metadata, IEnumerable<Attribute> attributes);

        private IEnumerable<IValidator> GetValidatorsForProperty(ObjectMetadata metadata)
        {
            var typeDescriptor = GetTypeDescriptor(metadata.ContainerType);
            var property = typeDescriptor.GetProperties().Find(metadata.Name, true);
            Throw.IfNull(property)
                .AnArgumentException("The property {0}.{1} could not be found."
                .FormatWith(metadata.ContainerType.FullName, metadata.Name), "metadata");
            return GetValidators(metadata, property.Attributes.OfType<Attribute>());
        }

        private IEnumerable<IValidator> GetValidatorsForType(ObjectMetadata metadata)
        {
            return GetValidators(metadata, GetTypeDescriptor(metadata.Type).GetAttributes().Cast<Attribute>());
        }
    }
}
