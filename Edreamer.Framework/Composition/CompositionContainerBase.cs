using System;
using System.Collections.Generic;
using System.Linq;

namespace Edreamer.Framework.Composition
{
    public abstract class CompositionContainerBase : ICompositionContainer
    {
        #region Abstract Methods
        public abstract IEnumerable<Lazy<object, object>> GetExports(Type type, Type metadataViewType, string contractName);

        public abstract IEnumerable<Lazy<T>> GetExports<T>(string contractName);
        public abstract IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName);

        public abstract Lazy<T> GetExport<T>(string contractName);
        public abstract Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName);

        public abstract void SatisfyImportsOnce(object part);
        #endregion

        #region GetExport
        public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
        {
            return GetExports<T, TMetadataView>(null);
        }

        public IEnumerable<Lazy<T>> GetExports<T>()
        {
            return GetExports<T>(null);
        }

        public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
        {
            return GetExport<T, TMetadataView>(null);
        }

        public Lazy<T> GetExport<T>()
        {
            return GetExport<T>(null);
        }
        #endregion

        #region GetExportedValue
        public IEnumerable<T> GetExportedValues<T>(string contractName)
        {
            return GetExports<T>().Select(e => e.Value);
        }

        public IEnumerable<T> GetExportedValues<T>()
        {
            return GetExportedValues<T>(null);
        }

        public T GetExportedValue<T>(string contractName)
        {
            return GetExport<T>().Value;
        }

        public T GetExportedValue<T>()
        {
            return GetExportedValue<T>(null);
        }
        #endregion

        #region IDisposable Implementation
        ~CompositionContainerBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // so that Dispose(false) isn't called later
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

    }
}
