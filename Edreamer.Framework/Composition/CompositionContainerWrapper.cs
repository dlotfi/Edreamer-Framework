using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Composition
{
    public class CompositionContainerWrapper : CompositionContainerBase
    {
        private readonly CompositionContainer _container;
        private bool _disposed;
        private IEnumerable<ICompositionInterceptor> _interceptors;

        public CompositionContainerWrapper(CompositionContainer container)
        {
            Throw.IfArgumentNull(container, "container");
            _container = container;
            _interceptors = null;
        }

        public override IEnumerable<Lazy<object, object>> GetExports(Type type, Type metadataViewType, string contractName)
        {
            return _container.GetExports(type, metadataViewType, contractName);
        }

        public override IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName)
        {
            return _container.GetExports<T, TMetadataView>(contractName);
        }

        public override IEnumerable<Lazy<T>> GetExports<T>(string contractName)
        {
            return _container.GetExports<T>(contractName);
        }

        public override Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
        {
            return _container.GetExport<T, TMetadataView>(contractName);
        }

        public override Lazy<T> GetExport<T>(string contractName)
        {
            return _container.GetExport<T>(contractName);
        }

        public override void SatisfyImportsOnce(object part)
        {
            _container.SatisfyImportsOnce(part);
            Intercept(part);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _container.Dispose();
            }
            _disposed = true;
        }

        private void Intercept(object part)
        {
            if (_interceptors == null)
            {
                _interceptors = GetExportedValues<ICompositionInterceptor>();
            }
            foreach (var interceptor in _interceptors)
            {
                interceptor.Intercept(part);
            }
        }
    }
}
