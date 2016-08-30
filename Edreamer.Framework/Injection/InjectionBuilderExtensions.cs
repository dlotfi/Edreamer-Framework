using System;
using System.Collections.Generic;
using Edreamer.Framework.Injection.Injectors;

namespace Edreamer.Framework.Injection
{
    public static class InjectionBuilderExtensions
    {
        public static InjectionResult PlaneInject(this IPropertyInjector injector, params object[] sources)
        {
            return injector.Inject<PlaneValueInjector>(null, sources);
        }

        public static InjectionResult CloneInject(this IPropertyInjector injector, IEnumerable<Type> excludedTypes, params object[] sources)
        {
            return injector.Inject<NestedCloneInjector>(excludedTypes, sources);
        }

        public static InjectionResult Flat(this IPropertyInjector injector, params object[] sources)
        {
            return injector.Inject<FlatteningValueInjector>(null, sources);
        }

        public static InjectionResult Unflat(this IPropertyInjector injector, params object[] sources)
        {
            return injector.Inject<UnflatteningValueInjector>(null, sources);
        }
    }
}
