// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// CompositionDependencyResolver
    /// </summary>
    public class CompositionDependencyResolver : IDependencyResolver, IServiceProvider
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionDependencyResolver"/> class.
        /// </summary>
        /// <param name="compositionContainerAccessor">The composition container accessor.</param>
        public CompositionDependencyResolver(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            _compositionContainerAccessor = compositionContainerAccessor;
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>The requested service or object.</returns>
        public object GetService(Type serviceType)
        {
            Throw.IfArgumentNull(serviceType, "serviceType");
            var export = _compositionContainerAccessor.Container.GetExports(serviceType, null, null)
                            .Select(e => e.Value)
                            .FirstOrDefault();
            return export;
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            Throw.IfArgumentNull(serviceType, "serviceType");
            var exports = _compositionContainerAccessor.Container.GetExports(serviceType, null, null)
                            .Select(e => e.Value);
            return exports;
        }
    }
}