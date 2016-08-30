using System.Web.Mvc;
using Edreamer.Framework.Mvc.WebParts;

namespace Edreamer.Framework.Mvc.Extensions
{
    public static class WebPartExtensions
    {
        public static T WebPart<T>(this HtmlHelper htmlHelper)
            where T: IAnyWebPart
        {
            var webPart = htmlHelper.GetContainer().GetExportedValue<T>();
            webPart.Html = htmlHelper;
            return webPart;
        }

        public static T WebPart<T>(this Controller controller)
            where T : IAnyWebPart
        {
            return controller.HtmlHelper().WebPart<T>();
        }
    }
}
