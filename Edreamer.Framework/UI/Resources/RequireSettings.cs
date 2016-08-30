// Based on Orchard CMS

using System;
using System.Collections.Generic;
using System.Web;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.UI.Resources
{
    public class RequireSettings
    {
        private readonly Func<RequireSettings, RequiredResourceContext> _requirementSatisfier;
        private bool? _debugMode;
        private bool? _cdnMode;

        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Culture { get; private set; }
        public bool DebugMode { get { return _debugMode ?? false; } }
        public bool CdnMode { get { return _cdnMode ?? false; } }
        public ResourceLocation Location { get; private set; }
        public string Condition { get; private set; }
        public IDictionary<string, string> Attributes { get; private set; }

        public IHtmlString GetHtml(bool absoluteUrl = false)
        {
            Throw.IfNull(_requirementSatisfier).A<InvalidOperationException>("No requirement satisfier has been defined to get resource html.");
            return _requirementSatisfier(this).GetHtml(absoluteUrl);
        }

        //public string GetPath()
        //{
        //    Throw.IfNull(_requirementSatisfier).A<InvalidOperationException>("No requirement satisfier has been defined to get resource html.");
        //    return VirtualPathUtility.ToAbsolute(_requirementSatisfier(this).Resource.ResolveUrl(this));
        //}

        public RequireSettings(string type, string name)
            : this(type, name, null)
        {
        }

        public RequireSettings(string type, string name, Func<RequireSettings, RequiredResourceContext> requirementSatisfier)
        {
            Throw.IfArgumentNullOrEmpty(name, "name");
            Throw.IfArgumentNullOrEmpty(type, "type");
            Name = name;
            Type = type;
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _requirementSatisfier = requirementSatisfier;
        }

        public RequireSettings UseCulture(string cultureName)
        {
            if (!String.IsNullOrEmpty(cultureName))
            {
                Culture = cultureName;
            }
            return this;
        }

        public RequireSettings UseDebugMode(bool debugMode)
        {
            return UseDebugModeInternal(debugMode);
        }

        private RequireSettings UseDebugModeInternal(bool? debugMode)
        {
            if (debugMode != null)
            {
                _debugMode = debugMode;
            }
            return this;
        }

        public RequireSettings UseCdn(bool cdn)
        {
            return UseCdnInternal(cdn);
        }

        private RequireSettings UseCdnInternal(bool? cdn)
        {
            if (cdn != null)
            {
                _cdnMode = cdn;
            }
            return this;
        }

        public RequireSettings UseCondition(string condition)
        {
            if (!String.IsNullOrEmpty(condition))
            {
                Condition = condition;
            }
            return this;
        }

        public RequireSettings SetAttribute(string name, string value)
        {
            Attributes[name] = value;
            return this;
        }

        /// <summary>
        /// The resource will be displayed in the head of the page
        /// </summary>
        public RequireSettings AtHead()
        {
            return AtLocationInternal(ResourceLocation.Head);
        }

        /// <summary>
        /// The resource will be displayed at the foot of the page
        /// </summary>
        public RequireSettings AtFoot()
        {
            return AtLocationInternal(ResourceLocation.Foot);
        }

        private RequireSettings AtLocationInternal(ResourceLocation location)
        {
            // if head is specified it takes precedence since it's safer than foot
            Location = (ResourceLocation)Math.Max((int)Location, (int)location);
            return this;
        }

        public RequireSettings Combine(RequireSettings other)
        {
            var settings = new RequireSettings(Type, Name)
                .AtLocationInternal(Location).AtLocationInternal(other.Location)
                .UseCdnInternal(_cdnMode).UseCdnInternal(other._cdnMode)
                .UseDebugModeInternal(_debugMode).UseDebugModeInternal(other._debugMode)
                .UseCulture(Culture).UseCulture(other.Culture)
                .UseCondition(Condition).UseCondition(other.Condition);
            settings.Attributes = CollectionHelpers.MergeDictionaries(Attributes, other.Attributes);
            return settings;
        }

    }
}
