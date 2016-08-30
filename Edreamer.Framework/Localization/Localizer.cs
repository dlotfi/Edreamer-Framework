// Based on Orchard CMS

using System;
using System.Globalization;
using System.Linq;

namespace Edreamer.Framework.Localization
{
    public delegate string Localizer(string text, params object[] args);

    public static class LocalizerExtensions
    {
        public static string Plural(this Localizer T, string textSingular, string textPlural, int count, params object[] args)
        {
            return T(count == 1 ? textSingular : textPlural, new object[] { count }.Concat(args).ToArray());
        }

        public static Localizer IfCantThen(this Localizer firstLocalizer, Localizer secondLocalizer)
        {
            return (text, args) =>
            {
                var localizedText = firstLocalizer(text, LocalizerFormatting.Skip);
                return localizedText != text
                    ? firstLocalizer(text, args)
                    : secondLocalizer(text, args);
            };
        }
    }

    public class NullLocalizer
    {
        public static Localizer Instance { get { return Localizer; } }

        private static string Localizer(string text, params object[] args)
        {
            return (args == null || args.Length == 0 || args.Any(x => x is LocalizerFormatting)) ? text : String.Format(CultureInfo.InvariantCulture, text, args);
        }
    }
}