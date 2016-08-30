using System.Linq;
using System.Text;
using System.Web;

namespace Edreamer.Framework.UI.Resources
{
    public static class ResourcesExtensions
    {
        public static IHtmlString GetAllRequiredStyles(this IResourceManager resourceManager, bool absoluteUrl = false)
        {
            var html = new StringBuilder();
            foreach (var resource in resourceManager.GetAllRequiredResources().Where(r => r.Resource.Type == "stylesheet"))
                html.Append(resource.GetHtml(absoluteUrl));
            return new HtmlString(html.ToString());
        }

        public static IHtmlString GetAllRequiredScriptsAtHead(this IResourceManager resourceManager, bool absoluteUrl = false)
        {
            var html = new StringBuilder();
            foreach (var resource in resourceManager.GetAllRequiredResources()
                .Where(r => r.Resource.Type == "script" && r.Settings.Location == ResourceLocation.Head))
                html.Append(resource.GetHtml(absoluteUrl));
            return new HtmlString(html.ToString());
        }

        public static IHtmlString GetAllRequiredScriptsAtFoot(this IResourceManager resourceManager, bool absoluteUrl = false)
        {
            var html = new StringBuilder();
            foreach (var resource in resourceManager.GetAllRequiredResources()
                .Where(r => r.Resource.Type == "script" && r.Settings.Location != ResourceLocation.Head))
                html.Append(resource.GetHtml(absoluteUrl));
            return new HtmlString(html.ToString());
        }
    }
}