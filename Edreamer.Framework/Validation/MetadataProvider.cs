using System;
using System.Collections.Generic;
using System.ComponentModel;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Validation
{
    public class MetadataProvider : IMetadataProvider
    {
        public ObjectMetadata GetMetadata(Type type)
        {
            Throw.IfArgumentNull(type, "type");
            return GetMetadata(type, null, null);
        }

        private static ObjectMetadata GetMetadata(Type type, ObjectMetadata container, string name)
        {
            var containerType = container == null ? null : container.Type;
            var qualifiedName = container == null ? name : container.QualifiedName + "." + name;
            return new ObjectMetadata(type, containerType, name, qualifiedName, GetPropertiesMetadata);
        }

        private static IEnumerable<ObjectMetadata> GetPropertiesMetadata(ObjectMetadata container)
        {
            Throw.IfArgumentNull(container, "container");
            if (container.Type.IsComplex())
            {
                foreach (PropertyDescriptor property in container.Type.GetTypeDescriptor().GetProperties())
                {
                    yield return GetMetadata(property.PropertyType, container, property.Name);
                }
            }
        }
    }
}
