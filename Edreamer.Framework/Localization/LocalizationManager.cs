// Based on Orchard CMS

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;

namespace Edreamer.Framework.Localization
{
    public class LocalizationManager: ILocalizationManager
    {
        private readonly IModuleManager _moduleManager;
        private const string DefaultLocalizationFilePathFormat = "~/App_Data/Default ({0}).po";
        private const string FrameworkLocalizationFilePathFormat = "~/App_Data/Edreamer.Framework ({0}).po";
        private const string ModulesLocalizationFilePathFormat = "~/App_Data/{0} ({1}).po";
        

        public LocalizationManager(IModuleManager moduleManager)
        {
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            _moduleManager = moduleManager;
        }

        public ICache Cache { get; set; }

        // This will translate a string into a string in the target cultureName.
        // The scope portion is optional, it amounts to the location of the file containing 
        // the string in case it lives in a view, or the namespace name if the string lives in a binary.
        // If the culture doesn't have a translation for the string, it will fallback to the 
        // parent culture as defined in the .net culture hierarchy. e.g. fr-FR will fallback to fr.
        // In case it's not found anywhere, the text is returned as is.
        public string GetLocalizedString(string scope, string text, string cultureName)
        {
            Throw.IfArgumentNullOrEmpty(text, "text");
            Throw.IfArgumentNullOrEmpty(cultureName, "cultureName");

            var translations = GetTranslations(cultureName);

            var scopedKey = (scope + "|" + text).ToLowerInvariant();
            if (translations.ContainsKey(scopedKey))
                return translations[scopedKey];

            var genericKey = ("|" + text).ToLowerInvariant();
            if (translations.ContainsKey(genericKey))
                return translations[genericKey];

            return GetParentTranslation(scope, text, cultureName);
        }

        private string GetParentTranslation(string scope, string text, string cultureName) 
        {
            var scopedKey = (scope + "|" + text).ToLowerInvariant();
            var genericKey = ("|" + text).ToLowerInvariant();
            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
                var parentCultureInfo = cultureInfo.Parent;
                if (parentCultureInfo.IsNeutralCulture)
                {
                    var translations = GetTranslations(parentCultureInfo.Name);
                    if (translations.ContainsKey(scopedKey))
                        return translations[scopedKey];
                    if (translations.ContainsKey(genericKey))
                        return translations[genericKey];
                    return text;
                }
            }
            catch (CultureNotFoundException) 
            { }

            return text;
        }


        // Loads the translations in memory and caches it.
        private IDictionary<string, string> GetTranslations(string culture)
        {
            return Cache.Get(culture, ctx => {
                //ctx.Monitor(_signals.When("culturesChanged"));
                return LoadTranslationsForCulture(ctx.Key);
            });
        }

        // Merging occurs from multiple locations in the following order (reverse priority):
        // - DefaultLocalizationFilePathFormat = "~/App_Data/Default ({0}).po"
        // - FrameworkLocalizationFilePathFormat ="~/App_Data/Edreamer.Framework ({0}).po"
        // - ModulesLocalizationFilePathFormat = "/App_Data/{0} ({1}).po";
        // The dictionary entries from po files that live in higher priority locations will
        // override the ones from lower priority locations during loading of dictionaries.
        private IDictionary<string, string> LoadTranslationsForCulture(string culture)
        {
            IDictionary<string, string> translations = new Dictionary<string, string>();

            var defaultFile = new FileInfo(PathHelpers.GetPhysicalPath(DefaultLocalizationFilePathFormat.FormatWith(culture)));
            if (defaultFile.Exists)
            {
                using (var stream = defaultFile.OpenRead())
                {
                    ParseLocalizationStream(stream, translations, true);
                }
            }


            var frameworkFile = new FileInfo(PathHelpers.GetPhysicalPath(FrameworkLocalizationFilePathFormat.FormatWith(culture)));
            if (frameworkFile.Exists)
            {
                using (var stream = frameworkFile.OpenRead())
                {
                    ParseLocalizationStream(stream, translations, true);
                }
            }


            foreach (var module in _moduleManager.Modules)
            {
                var moduleFile = new FileInfo(PathHelpers.GetPhysicalPath(ModulesLocalizationFilePathFormat.FormatWith(module.Name, culture)));
                if (moduleFile.Exists)
                {
                    using (var stream = moduleFile.OpenRead())
                    {
                        ParseLocalizationStream(stream, translations, true);
                    }
                }
            }

            return translations;
        }

        private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char>
        {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

        private static string Unescape(string str) {
            StringBuilder sb = null;
            bool escaped = false;
            for (var i = 0; i < str.Length; i++) {
                var c = str[i];
                if (escaped) {
                    if (sb == null) {
                        sb = new StringBuilder(str.Length);
                        if (i > 1) {
                            sb.Append(str.Substring(0, i - 1));
                        }
                    }
                    char unescaped;
                    if (_escapeTranslations.TryGetValue(c, out unescaped)) {
                        sb.Append(unescaped);
                    }
                    else {
                        // General rule: \x ==> x
                        sb.Append(c);
                    }
                    escaped = false;
                }
                else {
                    if (c == '\\') {
                        escaped = true;
                    }
                    else if (sb != null) {
                        sb.Append(c);
                    }
                }
            }
            return sb == null ? str : sb.ToString();
        }

        private static void ParseLocalizationStream(Stream stream, IDictionary<string, string> translations, bool merge)
        {
            var reader = new StringReader(new StreamReader(stream).ReadToEnd());
            string poLine, id, scope;
            id = scope = String.Empty;
            while ((poLine = reader.ReadLine()) != null) {
                if (poLine.StartsWith("#:")) {
                    scope = ParseScope(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgid")) {
                    id = ParseId(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgstr")) {
                    string translation = ParseTranslation(poLine);
                    // ignore incomplete localizations (empty msgid or msgstr)
                    if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(translation)) {
                        string scopedKey = (scope + "|" + id).ToLowerInvariant();
                        if (!translations.ContainsKey(scopedKey)) {
                            translations.Add(scopedKey, translation);
                        }
                        else {
                            if (merge) {
                                translations[scopedKey] = translation;
                            }
                        }
                    }
                    id = scope = String.Empty;
                }

            }
        }

        private static string ParseTranslation(string poLine) {
            return Unescape(poLine.Substring(6).Trim().Trim('"'));
        }

        private static string ParseId(string poLine) {
            return Unescape(poLine.Substring(5).Trim().Trim('"'));
        }

        private static string ParseScope(string poLine) {
            return Unescape(poLine.Substring(2).Trim().Trim('"'));
        }
    }
}

