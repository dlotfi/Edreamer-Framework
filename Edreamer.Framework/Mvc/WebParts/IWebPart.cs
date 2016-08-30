using System.Web;
using System.Web.Mvc;

namespace Edreamer.Framework.Mvc.WebParts
{
    public interface IAnyWebPart
    {
        HtmlHelper Html { get; set; }
    }

    public interface IWebPart : IAnyWebPart
    {
        IHtmlString Get();
        void Render();
    }

    public interface IWebPart<in TModel> : IAnyWebPart
    {
        IHtmlString Get(TModel model);
        void Render(TModel model);
    }
}
