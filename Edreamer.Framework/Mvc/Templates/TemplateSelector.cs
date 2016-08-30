using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Collections;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using Edreamer.Framework.Mvc.ViewEngine;

namespace Edreamer.Framework.Mvc.Templates
{
    public class TemplateSelector : CachedSelectorBase<ITemplateRegistrar, Template>, ITemplateSelector
    {
        private readonly IModuleManager _moduleManager;
        private readonly IViewLocationProvider _viewLocationProvider;

        protected IDictionary<string, string> BaseTemplateContexts { get; private set; }

        public TemplateSelector(IEnumerable<ITemplateRegistrar> registrars, IViewLocationProvider viewLocationProvider, IModuleManager moduleManager)
            :base(registrars)
        {
            Throw.IfArgumentNull(registrars, "registrars");
            Throw.IfArgumentNull(viewLocationProvider, "viewLocationProvider");
            Throw.IfArgumentNull(moduleManager, "moduleManager");

            _moduleManager = moduleManager;
            _viewLocationProvider = viewLocationProvider;
            BaseTemplateContexts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected override IEnumerable<Template> Register(ITemplateRegistrar registrar)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.If(String.IsNullOrWhiteSpace(registrar.TemplateContext))
                .A<TemplateRegistrationException>("Template registrar '{0}' has not specified template context.".FormatWith(registrar.GetType().FullName));
            if (BaseTemplateContexts.ContainsKey(registrar.TemplateContext))
            {
                Throw.IfNot(BaseTemplateContexts[registrar.TemplateContext].EqualsIgnoreCase(registrar.BaseTemplateContext))
                    .A<TemplateRegistrationException>("The template context '{0}' has been previously registered with base context '{1}'."
                    .FormatWith(registrar.TemplateContext, BaseTemplateContexts[registrar.TemplateContext]));
            }
            else
            {
                BaseTemplateContexts.Add(registrar.TemplateContext, registrar.BaseTemplateContext);
            }
            var templates = new List<Template>();
            registrar.RegisterTemplates(templates);
            return templates;
        }

        protected override string GetKey(ITemplateRegistrar registrar, Template item)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.IfArgumentNull(item, "item");
            return registrar.TemplateContext + ":" + item.Name;
        }

        protected override object GetValue(ITemplateRegistrar registrar, Template item)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.IfArgumentNull(item, "item");
            return CollectionHelpers.EmptyIfNull(GetLocations(registrar, item)).Where(PathHelpers.FileExists).FirstOrDefault();
        }

        protected override void FinalizeCaching()
        {
            // ToDo-Low [12121120]: Using DependencyList to check for invalid base template contexts and cyclic dependencies seems somewhat inappropriate.
            var list = new DependencyList<KeyValuePair<string, string>, string>(x => x.Key, x => (x.Value == null ? null : new[] { x.Value }));
            list.AddRange(BaseTemplateContexts);
            try
            {
                list.Sort();
            }
            catch (Exception ex)
            {
                Throw.Now.A<TemplateRegistrationException>("There is a problem in chain of dependencies between template" +
                                                           " contexts and their bases. See the inner exception for details.", ex);
            }
        }

        public string GetTemplatePath(string templateName, string templateContext)
        {
            Throw.IfArgumentNullOrEmpty(templateName, "templateName");
            object value = null;
            while (value == null && !String.IsNullOrEmpty(templateContext))
            {
                var key = templateContext + ":" + templateName;
                if (!Items.TryGetValue(key, out value))
                {
                    key = Items.Keys.SingleOrDefault(k => IsKeyPartiallyMatch(k, key));
                    value = key != null ? Items[key] : null;
                }
                templateContext = BaseTemplateContexts[templateContext];
            }
            return value != null ? value.ToString() : null;
        }

        protected IEnumerable<string> GetLocations(ITemplateRegistrar registrar, Template item)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.IfArgumentNull(item, "item");
            var moduleName = _moduleManager.GetModule(registrar.GetType()).Name;
            return _viewLocationProvider.TemplateLocationFormats
                .Select(l => l.FormatWith(item.Path, null, moduleName));
        }

        private static bool IsKeyPartiallyMatch(string thisKey, string targetKey)
        {
            Throw.IfArgumentNullOrEmpty(thisKey, "thisKey");
            Throw.IfArgumentNullOrEmpty(targetKey, "targetKey");

            if (thisKey.EndsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                thisKey = thisKey.TrimEnd("*");
                return targetKey.StartsWith(thisKey, StringComparison.OrdinalIgnoreCase);    
            }
            return false;
        }
    }
}
