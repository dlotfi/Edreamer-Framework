using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using Edreamer.Framework.Context;

namespace Edreamer.Framework.Mvc.Routes
{
    public abstract class NamedRoute : INamedRoute
    {
        [Import]
        public IWorkContextAccessor WorkContextAccessor { get; set; }

        public string Name
        {
            get { return GetType().FullName; }
        }

        public virtual string Get()
        {
            var urlHelper = new UrlHelper(WorkContextAccessor.Context.CurrentHttpContext().Request.RequestContext);
            return urlHelper.RouteUrl(Name);
        }
    }

    public abstract class NamedRoute<TModel> : INamedRoute<TModel>
    {
        [Import]
        public IWorkContextAccessor WorkContextAccessor { get; set; }

        public string Name
        {
            get { return GetType().FullName; }
        }

        public virtual string Get(TModel model)
        {
            var requestContext = WorkContextAccessor.Context.CurrentHttpContext().Request.RequestContext;

            // Cleaning current route data to force using only the provided values
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(TModel)))
            {
                requestContext.RouteData.Values.Remove(property.Name);
            }
            
            var urlHelper = new UrlHelper(requestContext);
            return urlHelper.RouteUrl(Name, model);
        }
    }

    public abstract class NamedRouteWithOptionalParameter<TModel> : NamedRoute<TModel>, INamedRoute
    {
        protected TModel DefaultModel { get; set; }

        protected NamedRouteWithOptionalParameter()
        {
            DefaultModel = default(TModel);
        }

        protected NamedRouteWithOptionalParameter(TModel defaultModel)
        {
            DefaultModel = defaultModel;
        }

        public virtual string Get()
        {
            return Get(DefaultModel);
        }
    }
}
