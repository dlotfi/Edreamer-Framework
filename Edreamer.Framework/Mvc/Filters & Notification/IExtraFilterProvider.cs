// Based on Orchard CMS

using System.Web.Mvc;

namespace Edreamer.Framework.Mvc.Filters
{
    public interface IExtraFilterProvider
    {
        void AddFilters(FilterInfo filterInfo);
    }
}
