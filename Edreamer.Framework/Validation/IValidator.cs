using System.Collections.Generic;

namespace Edreamer.Framework.Validation
{
    public interface IValidator
    {
        IEnumerable<ValidationResult> Validate(object instance, object container);
    }
}
