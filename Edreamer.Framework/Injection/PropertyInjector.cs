using System.Collections.Generic;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection
{
    public class PropertyInjector : IPropertyInjector
    {
        private readonly object _target;
        private readonly IEnumerable<string> _properties;
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;


        public PropertyInjector(object target, IEnumerable<string> properties, ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(target, "target");
            Throw.IfArgumentNullOrEmpty(properties, "properties");
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _target = target;
            _properties = properties;
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public InjectionResult Inject<TInjector>(object injectorParameter, params object[] sources) where TInjector : IValueInjector
        {
            Throw.IfArgumentNullOrEmpty(sources, "sources");
            var injector = GetService<TInjector>();
            return injector.Inject(injectorParameter, _target, _properties, sources);
        }
       
        public InjectionResult Inject<TInjector>(object injectorParameter) where TInjector : INoSourceValueInjector
        {
            var injector = GetService<TInjector>();
            return injector.Inject(injectorParameter, _target, _properties);
        }

        public InjectionResult DoNotInject()
        {
            return new InjectionResult { InjectedProperties = _properties, Target = _target };
        }

        private T GetService<T>()
        {
            return _compositionContainerAccessor.Container.GetExportedValue<T>();
        }
    }
}
