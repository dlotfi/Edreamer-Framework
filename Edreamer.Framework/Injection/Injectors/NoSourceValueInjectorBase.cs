using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class NoSourceValueInjectorBase : INoSourceValueInjector
    {
        protected object Parameter { get; private set; }

        public InjectionResult Inject(object injectorParameter, object target, IEnumerable<string> properties)
        {
            Throw.IfArgumentNull(target, "target");
            Throw.IfArgumentNull(properties, "properties");
            CheckParameter(injectorParameter);
            Parameter = injectorParameter;

            if (!properties.Any())
                return new InjectionResult { InjectedProperties = new List<string>(), Target = target };

            return Inject(target, properties);
        }

        protected abstract InjectionResult Inject(object target, IEnumerable<string> properties);

        protected virtual void CheckParameter(object injectorParameter)
        {
        }
    }
}
