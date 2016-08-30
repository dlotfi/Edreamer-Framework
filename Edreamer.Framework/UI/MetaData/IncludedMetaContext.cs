using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.MetaData
{
    public class IncludedMetaContext
    {
        private readonly IMetaTagBuilder _metaTagBuilder;

        public IncludedMetaContext(MetaEntry meta, IMetaTagBuilder metaTagBuilder)
        {
            Throw.IfArgumentNull(meta, "meta");
            Throw.IfArgumentNull(metaTagBuilder, "metaTagBuilder");
            Meta = meta;
            _metaTagBuilder = metaTagBuilder;
        }

        public MetaEntry Meta { get; private set; }
        
        public IHtmlString GetHtml()
        {
            return _metaTagBuilder.BuildMetaTag(Meta);
        }
    }
}
