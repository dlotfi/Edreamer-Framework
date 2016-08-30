using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    public class ViewLocationProvider : IViewLocationProvider
    {
        private readonly ICollection<string> _viewLocationFormats;
        private readonly ICollection<string> _layoutLocationFormats;
        private readonly ICollection<string> _templateLocationFormats;
        private readonly string _modulesPath;

        public ViewLocationProvider(ISettingsService settingsService)
        {
            _viewLocationFormats = new List<string>();
            _layoutLocationFormats = new List<string>();
            _templateLocationFormats = new List<string>();

            _modulesPath = settingsService.GetModulesPath();

            _viewLocationFormats.Add(_modulesPath + "{2}/Views/{1}/{0}.cshtml");
            _viewLocationFormats.Add(_modulesPath + "{2}/Views/{1}/{0}.vbhtml");
            _viewLocationFormats.Add(_modulesPath + "{2}/Views/Shared/{0}.cshtml");
            _viewLocationFormats.Add(_modulesPath + "{2}/Views/Shared/{0}.vbhtml");

            _layoutLocationFormats.Add(_modulesPath + "{2}/Layouts/{0}.cshtml");
            _layoutLocationFormats.Add(_modulesPath + "{2}/Layouts/{0}.vbhtml");

            _templateLocationFormats.Add(_modulesPath + "{2}/Templates/{0}.cshtml");
            _templateLocationFormats.Add(_modulesPath + "{2}/Templates/{0}.vbhtml");
        }

        public IEnumerable<string> AreaViewLocationFormats
        {
            get { return _viewLocationFormats; }
        }

        public IEnumerable<string> AreaMasterLocationFormats
        {
            get { return _viewLocationFormats; }
        }

        public IEnumerable<string> AreaPartialViewLocationFormats
        {
            get { return _viewLocationFormats; }
        }

        public IEnumerable<string> LayoutLocationFormats
        {
            get { return _layoutLocationFormats; }
        }

        public IEnumerable<string> TemplateLocationFormats
        {
            get { return _templateLocationFormats; }
        }

        public string GetBasePathFromVirtualPath(string viewPath)
        {
            Throw.IfArgumentNull(viewPath, "viewPath");
            Throw.IfNot(viewPath.StartsWith(_modulesPath))
                .AnArgumentException("This view is not defined in modules", "viewPath");
            var moduleRelativePath = viewPath.TrimStart(_modulesPath);
            var moduleName = moduleRelativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).First();
            return _modulesPath + moduleName;
        }
    }
}
