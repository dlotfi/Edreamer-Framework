using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection.Injectors
{
    public abstract class ValueInjectorBase: IValueInjector
    {
        protected object Parameter { get; private set; }

        public InjectionResult Inject(object injectorParameter, object target, IEnumerable<string> properties, params object[] sources)
        {
            Throw.IfArgumentNull(target, "target");
            Throw.IfArgumentNull(properties, "properties");
            Throw.IfArgumentNullOrEmpty(sources, "sources");
            CheckParameter(injectorParameter);
            Parameter = injectorParameter;

            if (!properties.Any())
                return new InjectionResult { InjectedProperties = new List<string>(), Target = target };

            var injectedTarget = target;
            var injectedProperties = new List<string>();
            foreach (var source in sources)
            {
                var result = Inject(injectedTarget, properties, source);
                injectedProperties.AddRange(result.InjectedProperties);
                injectedTarget = target;
                properties = properties.Except(injectedProperties, StringComparer.OrdinalIgnoreCase);
            }
            
            return new InjectionResult { InjectedProperties = injectedProperties, Target = injectedTarget };
        }

        protected abstract InjectionResult Inject(object target, IEnumerable<string> properties, object source);

        protected virtual void CheckParameter(object injectorParameter)
        {
        }
    }
}
