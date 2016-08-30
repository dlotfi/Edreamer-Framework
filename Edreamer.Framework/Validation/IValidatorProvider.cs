using System.Collections.Generic;

namespace Edreamer.Framework.Validation
{
    public interface IValidatorProvider
    {
        IEnumerable<IValidator> GetValidators(ObjectMetadata metadata);
    }
}
