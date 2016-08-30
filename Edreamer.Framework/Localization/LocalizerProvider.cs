using System;
using System.Globalization;
using System.Linq;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Localization
{
    public class LocalizerProvider : ILocalizerProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizationManager _localizationManager;

        public LocalizerProvider(IWorkContextAccessor workContextAccessor, ILocalizationManager localizationManager)
        {
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            Throw.IfArgumentNull(localizationManager, "localizationManager");
            _workContextAccessor = workContextAccessor;
            _localizationManager = localizationManager;
        }

        public Localizer GetLocalizer(string scope)
        {
            return GetLocalizer(scope, _workContextAccessor.Context.CurrentCulture());
        }

        public Localizer GetLocalizer(string scope, string currentCulture)
        {
            return (text, args) =>
                       {
                           var localizedString = _localizationManager.GetLocalizedString(scope, text, currentCulture);
                           if (args == null || args.Any(x => x is LocalizerFormatting)) return localizedString;
                           return (localizedString == null) ? null : String.Format(CultureInfo.CurrentUICulture, localizedString, args);
                       };
        }
    }
}
