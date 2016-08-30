using Edreamer.Framework.Bootstrapping;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Injection
{
    public class InjectionBootstrapper : IBootstrapperTask
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        public InjectionBootstrapper(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public void Run()
        {
            Injector.CompositionContainerAccessor = _compositionContainerAccessor;
        }
    }
}
