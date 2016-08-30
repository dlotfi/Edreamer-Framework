using System;
using System.Collections.Generic;

namespace Edreamer.Framework.Composition
{
    public interface ICompositionContainer: IDisposable
    {
        IEnumerable<Lazy<object, object>> GetExports(Type type, Type metadataViewType, string contractName);
        
        IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>();
        IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName);

        IEnumerable<Lazy<T>> GetExports<T>();
        IEnumerable<Lazy<T>> GetExports<T>(string contractName);

        Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName);
        Lazy<T, TMetadataView> GetExport<T, TMetadataView>();
        
        Lazy<T> GetExport<T>(string contractName);
        Lazy<T> GetExport<T>();

        IEnumerable<T> GetExportedValues<T>(string contractName);
        IEnumerable<T> GetExportedValues<T>();

        T GetExportedValue<T>(string contractName);
        T GetExportedValue<T>();

        //T GetExportedValueOrDefault<T>(string contractName);
        //T GetExportedValueOrDefault<T>();

        void SatisfyImportsOnce(object part);
    }
}
