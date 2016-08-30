using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.MetaData
{
    public class MetaDataManager: IMetaDataManager
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IMetaTagBuilder _metaTagBuilder;

        public MetaDataManager(IWorkContextAccessor workContextAccessor, IMetaTagBuilder metaTagBuilder)
        {
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            _workContextAccessor = workContextAccessor;
            _metaTagBuilder = metaTagBuilder;
        }

        public IEnumerable<IncludedMetaContext> GetAllIncludedMetaData()
        {
            return CurrentIncludedMetaData().Select(m => new IncludedMetaContext(m, _metaTagBuilder)).ToList();
        }

        public void IncludeMetaData(MetaEntry meta)
        {
            Throw.IfArgumentNull(meta, "meta");
            CurrentIncludedMetaData().Add(meta);
        }

        private ISet<MetaEntry> CurrentIncludedMetaData()
        {
            return _workContextAccessor.Context.GetState<ISet<MetaEntry>>("IncludedMetaData");
        }
    }
}