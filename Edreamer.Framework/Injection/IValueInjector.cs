using System.Collections.Generic;

namespace Edreamer.Framework.Injection
{
    public interface IValueInjector 
    {
        InjectionResult Inject(object injectorParameter, object target, IEnumerable<string> properties, params object[] sources);
    }

    public interface INoSourceValueInjector
    {
        InjectionResult Inject(object injectorParameters, object target, IEnumerable<string> properties);
    }
}
