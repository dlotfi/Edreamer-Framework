using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.WebPages;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Mvc.Extensions;
using Edreamer.Framework.Mvc.Layouts;
using Edreamer.Framework.Security.Authorization;
using Edreamer.Framework.UI.MetaData;
using Edreamer.Framework.UI.Notification;
using Edreamer.Framework.UI.Resources;
using JetBrains.Annotations;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>, IViewPage
    {
        private string _templateContext;
        private Localizer _localizer;
        private IEnumerable<NotifyEntry> _notifications;

        // Using RequestContext.GetContainer() extension method to get composition container and resolve required services
        // in layout pages. Because layout pages are not created by Mvc (using ViewPageActvator).
        
        private ILayoutSelector _layoutSelector;
        [Import]
        public ILayoutSelector LayoutSelector
        {
            get { return _layoutSelector ?? (_layoutSelector = Container.GetExportedValue<ILayoutSelector>()); }
            set { _layoutSelector = value; }
        }

        private ILocalizerProvider _localizerProvider;
        [Import]
        public ILocalizerProvider LocalizerProvider
        {
            get { return _localizerProvider ?? (_localizerProvider = Container.GetExportedValue<ILocalizerProvider>()); }
            set { _localizerProvider = value; }
        }

        private IViewLocationProvider _viewLocationProvider;
        [Import]
        public IViewLocationProvider ViewLocationProvider
        {
            get { return _viewLocationProvider ?? (_viewLocationProvider = Container.GetExportedValue<IViewLocationProvider>()); }
            set { _viewLocationProvider = value; }
        }

        private IResourceManager _resourceManager;
        [Import]
        public IResourceManager ResourceManager
        {
            get { return _resourceManager ?? (_resourceManager = Container.GetExportedValue<IResourceManager>()); }
            set { _resourceManager = value; }
        }

        private IMetaDataManager _metaDataManager;
        [Import]
        public IMetaDataManager MetaDataManager
        {
            get { return _metaDataManager ?? (_metaDataManager = Container.GetExportedValue<IMetaDataManager>()); }
            set { _metaDataManager = value; }
        }

        private IAuthorizer _authorizer;
        [Import]
        public IAuthorizer Authorizer
        {
            get { return _authorizer ?? (_authorizer = Container.GetExportedValue<IAuthorizer>()); }
            set { _authorizer = value; }
        }

        public Localizer T
        {
            get { return _localizer ?? (_localizer = LocalizerProvider.GetLocalizer(VirtualPath)); }
        }

        public override string Layout
        {
            get
            {
                return base.Layout;
            }
            set
            {
                if (value != null && value.EnclosedBy("<", ">"))
                {
                    var layoutName = value.Trim('<', '>');
                    base.Layout = LayoutSelector.GetLayoutPath(layoutName);
                }
                else
                {
                    base.Layout = value;
                }
            }
        }

        public string Template
        {
            get
            {
                return _templateContext;
            }
            set
            {
                _templateContext = value;
                WorkContext.CurrentTemplateContexts().SetTopItem(_templateContext);
            }
        }

        public IEnumerable<NotifyEntry> Notifications
        {
            get { return _notifications ?? (_notifications = CollectionHelpers.EmptyIfNull(TempData["_NotificationMessages"] as IEnumerable<NotifyEntry>)); }
        }

        private IWorkContext _workContext;
        public IWorkContext WorkContext
        {
            get { return _workContext ?? (_workContext = Container.GetExportedValue<IWorkContextAccessor>().Context); }
            set { _workContext = value; }
        }

        private ICompositionContainer _container;
        public ICompositionContainer Container
        {
            get { return _container ?? (_container = ViewContext.GetContainer()); }
            set { _container = value; }
        }

        public string Content([PathReference]string contentPath)
        {
            Throw.IfArgumentNullOrEmpty(contentPath, "contentPath");
            var basePath = ViewLocationProvider.GetBasePathFromVirtualPath(VirtualPath);
            if (contentPath.StartsWith("~"))
            {
                contentPath = contentPath.Replace("~", basePath);
                return Url.Content(contentPath);
            }
            return Url.Content(contentPath);
        }

        public RequireSettings RequireResource(string resourceType, string resourceName)
        {
            return ResourceManager.Require(resourceType, resourceName);
        }

        public RequireSettings IncludeResource(string resourceType, string resourceUrl, Func<ResourceDefinition, ResourceDefinition> resourceCreator = null)
        {
            var basePath = ViewLocationProvider.GetBasePathFromVirtualPath(VirtualPath);
            var resourceName = basePath + resourceUrl;
            var resource = new ResourceDefinition(basePath, resourceType, resourceName);
            resource = resource.SetUrl(resourceUrl);
            if (resourceCreator != null)
                resource = resourceCreator(resource);
            return ResourceManager.Include(resource);
        }

        public RequireSettings RequireScript(string name)
        {
            return RequireResource("script", name);
        }

        public RequireSettings RequireStyle(string name)
        {
            return RequireResource("stylesheet", name);
        }

        public RequireSettings IncludeScript(string url, string urlDebug = null)
        {
            return IncludeResource("script", url, r => r.SetUrlDebug(urlDebug));
        }

        public RequireSettings IncludeStyle(string url, string urlDebug = null)
        {
            return IncludeResource("stylesheet", url, r => r.SetUrlDebug(urlDebug));
        }

        public MetaEntry IncludeMetaData(string content = null, string name = null, string httpEquiv = null, string charset = null)
        {
            var meta = new MetaEntry { Content = content, Name = name, HttpEquiv = httpEquiv, Charset = charset };
            MetaDataManager.IncludeMetaData(meta);
            return meta;
        }

        protected override void InitializePage()
        {
            base.InitializePage();
            WorkContext.CurrentTemplateContexts().Push(null);
        }

        public override void ExecutePageHierarchy()
        {
            base.ExecutePageHierarchy();
            WorkContext.CurrentTemplateContexts().Pop();
        }

        public new void DefineSection(string name, SectionWriter action)
        {
            (this as WebPageBase).DefineSection(name, 
                () => {
                          WorkContext.CurrentTemplateContexts().Push(_templateContext);
                          action();
                          WorkContext.CurrentTemplateContexts().Pop();
                      });
        }


        // Based on Marcin Doboz's work - http://blogs.msdn.com/b/marcinon/archive/2010/12/15/razor-nested-layouts-and-redefined-sections.aspx
        // Note that Func<object, HelperResult> is the type of templated Razor delegates - http://haacked.com/archive/2011/02/27/templated-razor-delegates.aspx/

        private static readonly object _o = new object();

        /// <summary>
        /// Renders a section content if specified in view, otherwise use defaultContent.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="defaultContent">Content to be used if section content is not specified in view.</param>
        public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            if (IsSectionDefined(sectionName))
            {
                return RenderSection(sectionName);
            }
            return defaultContent(_o);
        }

        /// <summary>
        /// Redefines a section in a child layout to make it available to parent layout.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        public HelperResult RedefineSection(string sectionName)
        {
            return RedefineSection(sectionName, null);
        }

        /// <summary>
        /// Redefines a section in a child layout to make it available to parent layout.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="defaultContent">Content to be used if section content is not specified in view.</param>
        public HelperResult RedefineSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            if (IsSectionDefined(sectionName))
            {
                DefineSection(sectionName, () => Write(RenderSection(sectionName)));
            }
            else if (defaultContent != null)
            {
                DefineSection(sectionName, () => Write(defaultContent(_o)));
            }
            return new HelperResult(_ => { });
        }

        /// <summary>
        /// Postpones rendering the content till the end of rendering the page.
        /// </summary>
        /// <param name="content">Content to be rendered.</param>
        public HelperResult DeferRender(Func<object, HelperResult> content)
        {
            var guid = Guid.NewGuid();
            var placeHolderString = "PLACEHOLDER-" + guid + "-PLACEHOLDER";
            WorkContext.CurrentDeferredRenders().Add(placeHolderString, () => content(_o));
            return new HelperResult(w => w.Write(placeHolderString));
        }
    }
}
