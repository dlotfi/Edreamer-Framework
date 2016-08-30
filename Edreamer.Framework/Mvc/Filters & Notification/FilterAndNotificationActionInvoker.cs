// Based on Orchard CMS

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.UI.Notification;

namespace Edreamer.Framework.Mvc.Filters
{
    [PartPriority(PartPriorityAttribute.Default + 10)] // In order to override ControllerActionInvoker 
    public class FilterAndNotificationActionInvoker : ControllerActionInvoker
    {
        private readonly INotifier _notifier;
        private readonly IEnumerable<IExtraFilterProvider> _extrafilterProviders;
        private const string TempDataMessages = "_NotificationMessages";

        public FilterAndNotificationActionInvoker(IEnumerable<IExtraFilterProvider> extraFilterProviders, INotifier notifier)
        {
            _notifier = notifier;
            _extrafilterProviders = CollectionHelpers.EmptyIfNull(extraFilterProviders);
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            foreach (var provider in _extrafilterProviders)
            {
                provider.AddFilters(filters);
            }
            return filters;
        }

        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            var result = base.InvokeAction(controllerContext, actionName);
            if (_notifier.List().Any())
            {
                SaveNotifications(controllerContext, _notifier.List());
            }
            return result;
        }

        private static void SaveNotifications(ControllerContext context, IEnumerable<NotifyEntry> notifications)
        {
            var tempData = context.Controller.TempData;
            lock (tempData)
            {
                var notificationsList = new HashSet<NotifyEntry>(CollectionHelpers.EmptyIfNull(tempData[TempDataMessages] as IEnumerable<NotifyEntry>));
                notificationsList.AddRange(CollectionHelpers.EmptyIfNull(notifications));
                tempData[TempDataMessages] = notificationsList;
            }
        }
    }
}