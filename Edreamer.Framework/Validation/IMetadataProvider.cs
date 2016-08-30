using System;

namespace Edreamer.Framework.Validation
{
    public interface IMetadataProvider
    {
        ObjectMetadata GetMetadata(Type type);
    }
}
