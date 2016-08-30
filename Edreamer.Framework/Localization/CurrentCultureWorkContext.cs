using System;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Localization
{
    public class CurrentCultureWorkContext : IWorkContextStateProvider
    {
        private readonly ISettingsService _settingsService;

        public CurrentCultureWorkContext(ISettingsService settingsService)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            _settingsService = settingsService;
        }

        public Func<IWorkContext, object> Get(string name)
        {
            if (name.EqualsIgnoreCase("CurrentCulture"))
            {
                return ctx => _settingsService.GetDefaultCulture();
            }
            return null;
        }
    }

    public static class CurrentCultureWorkContextExtensions
    {
        public static string CurrentCulture(this IWorkContext workContext)
        {
            Throw.IfArgumentNull(workContext, "workContext");
            return workContext.GetState<string>("CurrentCulture");
        }
    }
}
