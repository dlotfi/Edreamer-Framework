using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using Edreamer.Framework.Mvc.ViewEngine;

namespace Edreamer.Framework.Mvc.Layouts
{
    public class LayoutSelector : CachedSelectorBase<ILayoutRegistrar, Layout>, ILayoutSelector
    {
        private readonly IModuleManager _moduleManager;
        private readonly IViewLocationProvider _viewLocationProvider;

        public LayoutSelector(IEnumerable<ILayoutRegistrar> registrars, IViewLocationProvider viewLocationProvider, IModuleManager moduleManager)
            : base(registrars)
        {
            Throw.IfArgumentNull(registrars, "registrars");
            Throw.IfArgumentNull(viewLocationProvider, "viewLocationProvider");
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            _moduleManager = moduleManager;
            _viewLocationProvider = viewLocationProvider;
        }

        protected override IEnumerable<Layout> Register(ILayoutRegistrar registrar)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            var layouts = new List<Layout>();
            registrar.RegisterLayouts(layouts);
            return layouts;
        }

        protected override string GetKey(ILayoutRegistrar registrar, Layout item)
        {
            Throw.IfArgumentNull(item, "item");
            return item.Name;
        }

        protected override object GetValue(ILayoutRegistrar registrar, Layout item)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.IfArgumentNull(item, "item");
            return CollectionHelpers.EmptyIfNull(GetLocations(registrar, item)).Where(PathHelpers.FileExists).FirstOrDefault();
        }

        public string GetLayoutPath(string layoutName)
        {
            Throw.IfArgumentNullOrEmpty(layoutName, "layoutName");
            object value;
            return Items.TryGetValue(layoutName, out value) ? value.ToString() : null;
        }

        protected IEnumerable<string> GetLocations(ILayoutRegistrar registrar, Layout item)
        {
            Throw.IfArgumentNull(registrar, "registrar");
            Throw.IfArgumentNull(item, "item");
            var moduleName = _moduleManager.GetModule(registrar.GetType()).Name;
            return _viewLocationProvider.LayoutLocationFormats
                .Select(l => l.FormatWith(item.Path, null, moduleName));
        }
    }
}
