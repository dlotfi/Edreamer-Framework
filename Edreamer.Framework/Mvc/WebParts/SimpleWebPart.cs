using System.ComponentModel.Composition;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Mvc.WebParts
{
    public abstract class SimpleWebPart : IWebPart
    {
        [Import]
        public IPartialStore PartialStore { get; set; }

        [Import]
        public IModuleManager ModuleManager { get; set; }

        public HtmlHelper Html { get; set; }
        public string ViewPath { get; private set; }

        protected string GetViewFullPath()
        {
            var area = ModuleManager.GetModule(GetType()).Name;
            return PartialStore.GetPartialViewPath(area, ViewPath);
        }

        protected SimpleWebPart(string viewPath)
        {
            Throw.IfArgumentNullOrEmpty(viewPath, "viewPath");
            ViewPath = viewPath;
        }

        public virtual IHtmlString Get()
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            var partialViewPath = GetViewFullPath();
            Throw.IfNull(partialViewPath)
                .A<WebPartResolveException>("Cannot find proper view for simple webpart {0}.".FormatWith(GetType().Name));
            return Html.Partial(partialViewPath);
        }

        public virtual void Render()
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            var partialViewPath = GetViewFullPath();
            Throw.IfNull(partialViewPath)
                .A<WebPartResolveException>("Cannot find proper view for simple webpart {0}.".FormatWith(GetType().Name));
            Html.RenderPartial(partialViewPath);
        }
    }

    public abstract class SimpleWebPart<TModel> : IWebPart<TModel>
    {
        [Import]
        public IPartialStore PartialStore { get; set; }

        [Import]
        public IModuleManager ModuleManager { get; set; }

        public HtmlHelper Html { get; set; }
        public string ViewPath { get; private set; }

        protected string GetViewFullPath()
        {
            var area = ModuleManager.GetModule(GetType()).Name;
            return PartialStore.GetPartialViewPath(area, ViewPath);
        }

        protected SimpleWebPart(string viewPath)
        {
            Throw.IfArgumentNullOrEmpty(viewPath, "viewPath");
            ViewPath = viewPath;
        }

        public IHtmlString Get(TModel model)
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            var partialViewPath = GetViewFullPath();
            Throw.IfNull(partialViewPath)
                .A<WebPartResolveException>("Cannot find proper view for simple webpart {0}.".FormatWith(GetType().Name));
            // Note: There's a problem in Mvc which causes passing parent's model to partial if you pass null as model to 
            // Partial and RenderPartial methods.
            // See http://stackoverflow.com/questions/650393/asp-net-mvc-renderpartial-with-null-model-gets-passed-the-wrong-type
            return !Equals(model, default(TModel)) ? Html.Partial(partialViewPath, new ViewDataDictionary(model)) :  Html.Partial(partialViewPath);
        }

        public void Render(TModel model)
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            var partialViewPath = GetViewFullPath();
            Throw.IfNull(partialViewPath)
                .A<WebPartResolveException>("Cannot find proper view for simple webpart {0}.".FormatWith(GetType().Name));
            Html.RenderPartial(partialViewPath, new ViewDataDictionary(model));
        }
    }

    public abstract class SimpleWebPartWithOptionalParameter<TModel> : SimpleWebPart<TModel>, IWebPart
    {
        protected TModel DefaultModel { get; set; }

        protected SimpleWebPartWithOptionalParameter(string viewPath)
            : base(viewPath)
        {
            DefaultModel = default(TModel);
        }

        public IHtmlString Get()
        {
            return Get(DefaultModel);
        }

        public void Render()
        {
            Render(DefaultModel);
        }
    }
}
