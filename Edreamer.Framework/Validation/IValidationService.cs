using System.Collections.Generic;

namespace Edreamer.Framework.Validation
{
    public interface IValidationService
    {
        IEnumerable<ValidationResult> Validate(object instance, bool recursive = true);
    }
}
