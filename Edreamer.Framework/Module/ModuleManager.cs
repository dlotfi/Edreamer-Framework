using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Caching;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Module
{
    public class ModuleManager : ModuleManagerBase
    {
        private readonly object _lock = new object();
        private readonly IEnumerable<Lazy<IModuleEventHandler>> _moduleEventHandlers;
        private readonly Lazy<IFrameworkDataContext> _dataContext;
        private readonly Lazy<ISettingsService> _settingsService;
        private bool _initialized;

        protected ISet<Module> ActiveModules
        {
            get { return Cache.Get("Modules", ctx => new HashSet<Module>()); }
        }

        protected ISet<string> InstalledModuleNames
        {
            get { return Cache.Get("InstalledModuleNames", ctx => new HashSet<string>()); }
        }

        protected ISet<string> UninstalledModuleNames
        {
            get { return Cache.Get("UninstalledModuleNames", ctx => new HashSet<string>()); }
        }

        public ICache Cache { get; set; }

        // Lazily resolving dependencies to prevent cyclic dependencies (because IModuleManager is used in PrioritizedExportHandler)
        public ModuleManager(IEnumerable<Lazy<IModuleEventHandler>> moduleEventHandlers, Lazy<IFrameworkDataContext> dataContext, Lazy<ISettingsService> settingsService)
        {
            Throw.IfArgumentNull(dataContext, "dataContext");
            Throw.IfArgumentNull(settingsService, "settingsService");
            _moduleEventHandlers = CollectionHelpers.EmptyIfNull(moduleEventHandlers);
            _dataContext = dataContext;
            _settingsService = settingsService;
        }

        public override void Initialize(IEnumerable<Module> modules)
        {
            modules = CollectionHelpers.EmptyIfNull(modules);

            lock (_lock)
            {
                Throw.If(_initialized).A<InvalidOperationException>("Module manager has already been initialized.");

                foreach (var module in modules)
                {
                    // Check for missing information
                    Throw.If(String.IsNullOrEmpty(module.Name) || String.IsNullOrEmpty(module.Assembly) ||
                        CollectionHelpers.IsNullOrEmpty(module.Namespaces) || module.Namespaces.Any(String.IsNullOrEmpty))
                        .A<ModuleRegistrationException>("Module contains invalid information.", module, null);

                    // Check for duplicate module name
                    var nameConflictingModules = ActiveModules.Where(m => m.Name.EqualsIgnoreCase(module.Name)).ToList();
                    Throw.If(nameConflictingModules.Any())
                        .A<ModuleRegistrationException>(
                            "Another module with the name '{0}' has already been registered.".FormatWith(module.Name),
                            module, nameConflictingModules);

                    // Check for namespace conflict between modules in a same assembly
                    var namespaceConflictingModules = ActiveModules.Where(m =>
                        m.Assembly.EqualsIgnoreCase(module.Assembly) &&
                        HasAnyNamespacesInCommon(m.Namespaces, module.Namespaces)).ToList();
                    Throw.If(namespaceConflictingModules.Any())
                        .A<ModuleRegistrationException>(
                            "Another module in the same assembly and with some namespaces in common has already been registered.",
                            module, namespaceConflictingModules);

                    ActiveModules.Add(module);
                }

                // Save modules to the data store and fires module events.
                // ToDo-OnDemand [01020930]: There's no way to reregister modules. (One work around is defining a Reregister property in Module class.)
                var installedModulesNames = _dataContext.Value.Modules.Select(m => m.Name).ToList();
                var newModulesNames = ActiveModules
                    .Where(m => !installedModulesNames.Any(mn => mn.EqualsIgnoreCase(m.Name)))
                    .Select(m => m.Name)
                    .ToList();
                var removedModulesNames = installedModulesNames
                    .Where(mn => !ActiveModules.Any(m => m.Name.EqualsIgnoreCase(mn)))
                    .ToList();

                InstalledModuleNames.AddRange(newModulesNames);
                UninstalledModuleNames.AddRange(removedModulesNames);
                _initialized = true;
            }
        }

        public override void CompleteInitialization()
        {
            // Using TransactionScope helps to wrap all transaction aware data context save changes and all other
            // transaction aware operations in a single transaction. Be aware of MS-DTC transactions. 

            var uninstallDeletedModules = _settingsService.Value.GetSetting<bool>(new SettingEntryKey { Category = "Module", Name = "UninstallDeletedModules" });

            // ToDo [01252236]: I commented it because it triggers MS-DTC. Find a solution for it. Think of a solution to wrap all events in a transaction
            //using (var scope = new TransactionScope())
            //{
            if (uninstallDeletedModules)
            {
                foreach (var moduleName in UninstalledModuleNames)
                {
                    _moduleEventHandlers.ForEach(meh => meh.Value.Uninstalled(moduleName));
                    _dataContext.Value.Modules.Remove(null, moduleName);
                }
            }
            foreach (var moduleName in InstalledModuleNames)
            {
                _moduleEventHandlers.ForEach(meh => meh.Value.Installed(moduleName));
                _dataContext.Value.Modules.Add(new Domain.Module { Name = moduleName });
            }
            _dataContext.Value.SaveChanges();
            //    scope.Complete();
            //}
        }

        public override IEnumerable<Module> Modules
        {
            get { return ActiveModules; }
        }

        public override bool TryGetModule(string assembly, string @namespace, out Module module)
        {
            Throw.IfArgumentNullOrEmpty(assembly, "assembly");
            Throw.IfArgumentNullOrEmpty(@namespace, "namespace");

            module = ActiveModules.FirstOrDefault(m =>
                      m.Assembly.EqualsIgnoreCase(assembly) &&
                      m.Namespaces.Any(ns => IsNamespaceMatch(ns, @namespace)));
            return (module != null);
        }

        /// <summary>
        /// Checks if targetNamespace matches requestedNamespace.
        /// </summary>
        /// <param name="requestedNamespace">The requested namespace.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <returns>True if they match otherwise false.</returns>
        // Based on ASP.Net Mvc3 source code (ControllerTypeCache.cs)
        private static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            // degenerate cases
            if (requestedNamespace == null)
            {
                return false;
            }
            if (requestedNamespace.Length == 0)
            {
                return true;
            }

            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
                // looking for exact namespace match
                return String.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
            }
            // looking for exact or sub-namespace match
            requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
            if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (requestedNamespace.Length == targetNamespace.Length)
            {
                // exact match
                return true;
            }
            if (targetNamespace[requestedNamespace.Length] == '.')
            {
                // good prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar.Baz"
                return true;
            }
            // bad prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar2"
            return false;
        }

        private static bool HasAnyNamespacesInCommon(IEnumerable<string> namespaces1, IEnumerable<string> namespaces2)
        {
            return (from ns1 in namespaces1
                    from ns2 in namespaces2
                    where IsNamespaceMatch(ns1, ns2) || IsNamespaceMatch(ns2, ns1)
                    select 0).Any();
        }
    }
}
