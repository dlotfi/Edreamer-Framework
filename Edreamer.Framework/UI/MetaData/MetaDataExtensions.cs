using System.Text;
using System.Web;

namespace Edreamer.Framework.UI.MetaData
{
    public static class MetaDataExtensions
    {
        public static IHtmlString GetAllMetaData(this IMetaDataManager metaDataManager)
        {
            var html = new StringBuilder();
            foreach (var metaData in metaDataManager.GetAllIncludedMetaData())
                html.Append(metaData.GetHtml());
            return new HtmlString(html.ToString());
        }
    }
}