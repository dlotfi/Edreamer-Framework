using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Mvc.WebParts
{
    public abstract class ActionWebPart : IWebPart
    {
        [Import]
        public IModuleManager ModuleManager { get; set; }

        public HtmlHelper Html { get; set; }
        public string Controller { get; private set; }
        public string Action { get; private set; }

        protected ActionWebPart(string controller, string action)
        {
            Throw.IfArgumentNull(controller, "controller");
            Throw.IfArgumentNullOrEmpty(action, "action");
            Controller = controller;
            Action = action;
        }

        protected RouteValueDictionary GetRouteValues()
        {
            var area = ModuleManager.GetModule(GetType()).Name;
            return new RouteValueDictionary(new { area, controller = Controller, action = Action });
        }

        public virtual IHtmlString Get()
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            return Html.Action(Action, GetRouteValues());
        }

        public virtual void Render()
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            Html.RenderAction(Action, GetRouteValues());
        }
    }


    public abstract class ActionWebPart<TModel> : IWebPart<TModel>
    {
        [Import]
        public IModuleManager ModuleManager { get; set; }

        public HtmlHelper Html { get; set; }
        public string Controller { get; private set; }
        public string Action { get; private set; }

        protected ActionWebPart(string controller, string action)
        {
            Throw.IfArgumentNull(controller, "controller");
            Throw.IfArgumentNullOrEmpty(action, "action");
            Controller = controller;
            Action = action;
        }

        protected RouteValueDictionary GetRouteValues(TModel model)
        {
            var area = ModuleManager.GetModule(GetType()).Name;
            var routeValues = new RouteValueDictionary(new { area, controller = Controller, action = Action });
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(model))
            {
                routeValues.Add(property.Name, property.GetValue(model));
            }
            return routeValues;
        }


        public virtual IHtmlString Get(TModel model)
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            return Html.Action(Action, GetRouteValues(model));
        }

        public virtual void Render(TModel model)
        {
            Throw.IfNull(Html).A<WebPartResolveException>("HtmlHelper is not provided.");
            Html.RenderAction(Action, GetRouteValues(model));
        }
    }


    public abstract class ActionWebPartWithOptionalParameter<TModel> : ActionWebPart<TModel>, IWebPart
    {
        protected TModel DefaultModel { get; set; }

        protected ActionWebPartWithOptionalParameter(string controller, string action)
            : base(controller, action)
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
