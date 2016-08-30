using Edreamer.Framework.Composition;
using Edreamer.Framework.Injection.Injectors;

namespace Edreamer.Framework.Injection
{
    public static class Injector
    {
        public static ICompositionContainerAccessor CompositionContainerAccessor { get; set; }

        public static IInjectionBuilder<TTarget> Into<TTarget>(TTarget target)
        {
            return new InjectionBuilder<TTarget>(target, CompositionContainerAccessor);
        }

        public static TTarget PlaneInject<TTarget>(TTarget target, params object[] sources)
        {
            return Into(target)
                .ForAny(i => i.Inject<PlaneValueInjector>(null, sources))
                .GetValue();
        }

        public static TTarget CloneInject<TTarget>(TTarget target, params object[] sources)
        {
            return Into(target)
                .ForAny(i => i.Inject<NestedCloneInjector>(null, sources))
                .GetValue();
        }

        public static TTarget Flat<TTarget>(TTarget target, params object[] sources)
        {
            return Into(target)
                .ForAny(i => i.Inject<FlatteningValueInjector>(null, sources))
                .GetValue();
        }

        public static TTarget Unflat<TTarget>(TTarget target, params object[] sources)
        {
            return Into(target)
                .ForAny(i => i.Inject<UnflatteningValueInjector>(null, sources))
                .GetValue();
        }
    }


}
