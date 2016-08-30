// Based on Orchard CMS

using System.Web.Mvc;

namespace Edreamer.Framework.Mvc.Filters
{
    public abstract class FilterProviderBase : IExtraFilterProvider
    {
        void IExtraFilterProvider.AddFilters(FilterInfo filterInfo)
        {
            AddFilters(filterInfo);
        }

        protected virtual void AddFilters(FilterInfo filterInfo)
        {
            if (this is IAuthorizationFilter)
                filterInfo.AuthorizationFilters.Add(this as IAuthorizationFilter);
            if (this is IActionFilter)
                filterInfo.ActionFilters.Add(this as IActionFilter);
            if (this is IResultFilter)
                filterInfo.ResultFilters.Add(this as IResultFilter);
            if (this is IExceptionFilter)
                filterInfo.ExceptionFilters.Add(this as IExceptionFilter);
        }

    }
}
