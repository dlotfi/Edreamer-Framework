using System;
using System.Collections.Generic;

namespace Edreamer.Framework.DataAnnotations
{
    public abstract class CompositeDataAnnotationsAttribute : Attribute
    {
        public abstract IEnumerable<Attribute> GetAttributes();
    }
}
