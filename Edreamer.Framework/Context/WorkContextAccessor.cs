using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Context
{
    public class WorkContextAccessor : IWorkContextAccessor
    {

        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        public WorkContextAccessor(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        public IWorkContext Context
        {
            get { return _compositionContainerAccessor.Container.GetExportedValue<IWorkContext>(); }
        }
    }
}
