using System;
using System.Web;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using Edreamer.Framework.Settings;

namespace Edreamer.Framework.UI.Resources
{
    public class ContentLocator: IContentLocator
    {
        private readonly IModuleManager _moduleManager;
        private string _modulesPath;

        public ContentLocator(ISettingsService settingsService, IModuleManager moduleManager)
        {
            Throw.IfArgumentNull(settingsService, "settingsService");
            Throw.IfArgumentNull(moduleManager, "moduleManager");
            _modulesPath = settingsService.GetModulesPath();
            _moduleManager = moduleManager;
        }

        public string GetContentUrl(string moduleName, string contentPath)
        {
            Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
            Throw.IfArgumentNullOrEmpty(contentPath, "contentPath");
            var basePath = _modulesPath + moduleName;
            contentPath = contentPath.Replace("~", basePath);
            if (!Uri.IsWellFormedUriString(contentPath, UriKind.Absolute))
                contentPath = VirtualPathUtility.ToAbsolute(contentPath);
            return contentPath;
        }

        public string GetContentUrl(Type type, string contentPath)
        {
            Throw.IfArgumentNull(type, "type");
            Module.Module module;
            Throw.IfNot(_moduleManager.TryGetModule(type, out module))
                .AnArgumentException("The type {0} is not defined in any modules.".FormatWith(type.Name), "type");
            return GetContentUrl(module.Name, contentPath);
        }
    }
}