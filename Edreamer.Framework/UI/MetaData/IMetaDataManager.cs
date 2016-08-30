using System.Collections.Generic;

namespace Edreamer.Framework.UI.MetaData
{
    public interface IMetaDataManager
    {
        IEnumerable<IncludedMetaContext> GetAllIncludedMetaData();

        void IncludeMetaData(MetaEntry meta);
    }
}
