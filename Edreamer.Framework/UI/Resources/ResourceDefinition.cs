// Based on Orchard CMS

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourceDefinition
    {
        private readonly Dictionary<ResolveSettings, string> _urlResolveCache = new Dictionary<ResolveSettings, string>();

        public ResourceDefinition(string basePath, string type, string name)
        {
            Throw.IfArgumentNullOrEmpty(basePath, "basePath");
            Throw.IfArgumentNullOrEmpty(type, "type");
            Throw.IfArgumentNullOrEmpty(name, "name");
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            BasePath = VirtualPathUtility.AppendTrailingSlash(basePath);
            Type = type;
            Name = name;
        }

        public string BasePath { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }
        public string UrlDebug { get; private set; }
        public string UrlCdn { get; private set; }
        public string UrlCdnDebug { get; private set; }
        public string[] Cultures { get; private set; }
        public bool CdnSupportsSsl { get; private set; }
        public IEnumerable<string> Dependencies { get; private set; }
        public IDictionary<string, string> Attributes { get; private set; }

        public ResourceDefinition AddAttribute(string name, string value, bool replaceExisting = false)
        {
            if (!Attributes.ContainsKey(name))
            {
                Attributes.Add(name, value);
            }
            else if (replaceExisting)
            {
                Attributes[name] = value;
            }
            return this;
        }

        public ResourceDefinition SetUrl(string url)
        {
            Url = url;
            return this;
        }

        public ResourceDefinition SetUrlDebug(string urlDebug)
        {
            UrlDebug = urlDebug;
            return this;
        }

        public ResourceDefinition SetCdn(string cdnUrl)
        {
            UrlCdn = cdnUrl;
            return this;
        }

        public ResourceDefinition SetCdnDebug(string cdnUrlDebug)
        {
            UrlCdnDebug = cdnUrlDebug;
            return this;
        }

        public ResourceDefinition SetVersion(string version)
        {
            Version = version;
            return this;
        }

        public ResourceDefinition SetCultures(params string[] cultures)
        {
            Cultures = cultures;
            return this;
        }

        public ResourceDefinition SetDependencies(params string[] dependencies)
        {
            Dependencies = dependencies;
            return this;
        }

        public string ResolveUrl(RequireSettings requireSettings)
        {
            string url;
            var settings = new ResolveSettings
                           {
                               DebugMode = requireSettings.DebugMode,
                               CdnMode = requireSettings.CdnMode,
                               Culture = requireSettings.Culture
                           };
            if (_urlResolveCache.TryGetValue(settings, out url))
            {
                return url;
            }
            // Url priority:
            if (settings.DebugMode)
            {
                url = settings.CdnMode
                    ? Coalesce(UrlCdnDebug, UrlDebug, UrlCdn, Url)
                    : Coalesce(UrlDebug, Url, UrlCdnDebug, UrlCdn);
            }
            else
            {
                url = settings.CdnMode
                    ? Coalesce(UrlCdn, Url, UrlCdnDebug, UrlDebug)
                    : Coalesce(Url, UrlDebug, UrlCdn, UrlCdnDebug);
            }
            if (String.IsNullOrEmpty(url))
            {
                return null;
            }
            if (!String.IsNullOrEmpty(settings.Culture))
            {
                var nearestCulture = FindNearestCulture(settings.Culture);
                if (!String.IsNullOrEmpty(nearestCulture))
                {
                    url = Path.ChangeExtension(url, nearestCulture + "." + Path.GetExtension(url));
                }
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && !VirtualPathUtility.IsAbsolute(url) && !VirtualPathUtility.IsAppRelative(url) && !String.IsNullOrEmpty(BasePath))
            {
                // relative urls are relative to the base path of the module that defined the manifest
                url = VirtualPathUtility.Combine(BasePath, url);
            }
            _urlResolveCache[settings] = url;
            return url;
        }

        public string FindNearestCulture(string culture)
        {
            // go for an exact match
            if (Cultures == null)
            {
                return null;
            }
            var selectedIndex = Array.IndexOf(Cultures, culture);
            if (selectedIndex != -1)
            {
                return Cultures[selectedIndex];
            }
            // try parent culture if any
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            if (cultureInfo.Parent.Name != culture)
            {
                var selectedCulture = FindNearestCulture(cultureInfo.Parent.Name);
                if (selectedCulture != null)
                {
                    return selectedCulture;
                }
            }
            return null;
        }

        #region Equality
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ResourceDefinition)) return false;
            return Equals((ResourceDefinition) obj);
        }

        public bool Equals(ResourceDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Name.EqualsIgnoreCase(Name) &&
                   other.Type.EqualsIgnoreCase(Type) &&
                   other.Version.EqualsIgnoreCase(Version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Name.GetHashCode();
                result = (result * 397) ^ Type.GetHashCode();
                result = (result * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                return result;
            }
        }
        #endregion


        private static string Coalesce(params string[] strings)
        {
            return strings.FirstOrDefault(str => !String.IsNullOrEmpty(str));
        }

        // Settings that matter for resolving url
        private struct ResolveSettings
        {
            public string Culture { get; set; }
            public bool DebugMode { get; set; }
            public bool CdnMode { get; set; }
        }
       
    }
}
