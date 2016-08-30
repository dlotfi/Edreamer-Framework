// Based on Orchard CMS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Module;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourceManager: IResourceManager
    {
        private readonly IModuleManager _moduleManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IResourceTagBuilder _resourceTagBuilder;
        private readonly IEnumerable<IResourceRegistrar> _registrars;
        private readonly string _modulesPath;

        protected IEnumerable<ResourceDefinition> RegisteredResources
        {
            get { return Cache.Get("Items", ctx => InitializeCache(_registrars)); }
        }

        public ICache Cache { get; set; }

        public ResourceManager(IEnumerable<IResourceRegistrar> registrars, IModuleManager moduleManager,
            IWorkContextAccessor workContextAccessor, ISettingsService settingsService,
            IResourceTagBuilder resourceTagBuilder)
        {
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(resourceTagBuilder, "resourceTagBuilder");
            _moduleManager = moduleManager;
            _workContextAccessor = workContextAccessor;
            _resourceTagBuilder = resourceTagBuilder;
            _modulesPath = settingsService.GetModulesPath();
            _registrars = CollectionHelpers.EmptyIfNull(registrars);
        }

        public IEnumerable<RequiredResourceContext> GetAllRequiredResources()
        {
            var allResources = new OrderedDictionary();
            foreach (var settings in CurrentRequiredResources())
            {
                var requiredResource = GetRequiredResource(settings);
                ExpandDependencies(requiredResource.Resource, settings, allResources);
            }
            return (from DictionaryEntry entry in allResources
                    select new RequiredResourceContext((ResourceDefinition)entry.Key, (RequireSettings)entry.Value, _resourceTagBuilder)
                   ).ToList();
        }

        public RequiredResourceContext GetRequiredResource(RequireSettings settings)
        {
            Throw.IfArgumentNull(settings, "settings");
            var resource = FindResource(settings);
            Throw.IfNull(resource)
                .AnArgumentException("No resource of type {0} and name {1} can be found."
                .FormatWith(settings.Type, settings.Name), "settings");

            return new RequiredResourceContext(resource, settings, _resourceTagBuilder);
        }

        public RequireSettings Require(string resourceType, string resourceName)
        {
            Throw.IfArgumentNullOrEmpty(resourceType, "resourceType");
            Throw.IfArgumentNullOrEmpty(resourceName, "resourceName");
            var workContext = _workContextAccessor.Context;
            var currentRequiredResources = CurrentRequiredResources();
            var settings = currentRequiredResources
                .SingleOrDefault(s => s.Type.EqualsIgnoreCase(resourceType) && s.Name.EqualsIgnoreCase(resourceName));
            if (settings == null)
            {
                bool useCdn, useDebugMode;
                if (!workContext.TryGetState("UseCdnForResources", out useCdn))
                    useCdn = false;
                if (!workContext.TryGetState("UseDebugModeForResources", out useDebugMode))
                    useDebugMode = workContext.CurrentHttpContext().IsDebuggingEnabled;
                settings = new RequireSettings(resourceType, resourceName, RequirementSatisfier)
                    .UseCulture(workContext.CurrentCulture())
                    .UseDebugMode(useDebugMode)
                    .UseCdn(useCdn);
                currentRequiredResources.Add(settings);
            }
            return settings;
        }

        protected virtual RequiredResourceContext RequirementSatisfier(RequireSettings requireSettings)
        {
            Throw.IfArgumentNull(requireSettings, "requireSettings");
            var currentRequiredResources = CurrentRequiredResources();
            var settings = currentRequiredResources
                .SingleOrDefault(s => s.Type.EqualsIgnoreCase(requireSettings.Type) && s.Name.EqualsIgnoreCase(requireSettings.Name));
            if (settings != null)
            {
                currentRequiredResources.Remove(settings);
            }
            return GetRequiredResource(requireSettings);
        }

        public RequireSettings Include(ResourceDefinition resource)
        {
            CurrentIncludedResources().Add(resource);
            return Require(resource.Type, resource.Name);
        }

        protected virtual void ExpandDependencies(ResourceDefinition resource, RequireSettings settings, OrderedDictionary allResources)
        {
            if (resource == null) return;
            // Settings is given so they can cascade down into dependencies.
            // forge the effective require settings for this resource
            // (1) If a require exists for the resource, combine with it. Last settings in, gets preference for its specified values.
            // (2) If no require already exists, form a new settings object based on the given one but with its own type/name.
            settings = allResources.Contains(resource)
                ? ((RequireSettings)allResources[resource]).Combine(settings)
                : new RequireSettings(resource.Type, resource.Name).Combine(settings);
            if (resource.Dependencies != null)
            {
                var dependencies = from d in resource.Dependencies
                                   select FindResource(new RequireSettings(resource.Type, d));
                foreach (var dependency in dependencies)
                {
                    if (dependency == null)
                    {
                        continue;
                    }
                    ExpandDependencies(dependency, settings, allResources);
                }
            }
            allResources[resource] = settings;
        }

        protected virtual ResourceDefinition FindResource(RequireSettings settings)
        {
            // find the resource with the given type and name. If multiple,
            // return the resource with the greatest version number.
            var includedResources = CurrentIncludedResources();
            return (from r in RegisteredResources.Union(includedResources)
                    where settings.Type.EqualsIgnoreCase(r.Type) &&
                          settings.Name.EqualsIgnoreCase(r.Name)
                    let version = r.Version != null ? new Version(r.Version) : null
                    orderby version descending
                    select r).FirstOrDefault();
        }

        protected virtual IEnumerable<ResourceDefinition> InitializeCache(IEnumerable<IResourceRegistrar> registrars)
        {
            var resources = new HashSet<ResourceDefinition>();
            foreach (var registrar in registrars)
            {
                var moduleName = _moduleManager.GetModule(registrar.GetType()).Name;

                var context = new ResourceRegistrarContext(moduleName, _modulesPath);
                registrar.RegisterResources(context);
                var duplicateResource = resources.Intersect(context.Resources).FirstOrDefault();
                Throw.If(duplicateResource != null)
                        .A<ResourceDuplicateException>("A resource with the type '{0}', name '{1}' and version '{2}' has already been registerd."
                        .FormatWith(duplicateResource == null ? "" : duplicateResource.Type,
                                    duplicateResource == null ? "" : duplicateResource.Name,
                                    duplicateResource == null ? "" : duplicateResource.Version));
                //Bug [01271734]: Checking for null because of the bug in exception throwing mechanism
                resources.AddRange(context.Resources);
            }
            return resources;
        }

        private ISet<RequireSettings> CurrentRequiredResources()
        {
            return _workContextAccessor.Context.GetState<ISet<RequireSettings>>("RequiredResources");
        }

        private ISet<ResourceDefinition> CurrentIncludedResources()
        {
            return _workContextAccessor.Context.GetState<ISet<ResourceDefinition>>("IncludedResources");
        }

    }
}
