using System;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// CompositionViewPageActivator
    /// </summary>
    public class CompositionViewPageActivator : IViewPageActivator
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="compositionContainerAccessor">The composition container accessor.</param>
        public CompositionViewPageActivator(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        /// <summary>
        /// Creates a view page.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="type">The type of the view page.</param>
        /// <returns>The created view page.</returns>
        public object Create(ControllerContext controllerContext, Type type)
        {
            var view = Activator.CreateInstance(type);
            _compositionContainerAccessor.Container.SatisfyImportsOnce(view);
            return view;
        }
    }
}
