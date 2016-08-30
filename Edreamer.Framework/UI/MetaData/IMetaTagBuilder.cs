using System.Web;

namespace Edreamer.Framework.UI.MetaData
{
    public interface IMetaTagBuilder
    {
        IHtmlString BuildMetaTag(MetaEntry meta);
    }
}