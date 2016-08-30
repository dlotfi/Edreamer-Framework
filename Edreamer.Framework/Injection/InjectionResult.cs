using System.Collections.Generic;

namespace Edreamer.Framework.Injection
{
    public class InjectionResult
    {
        public IEnumerable<string> InjectedProperties { get; set; }
        public object Target { get; set; }
    }
}
