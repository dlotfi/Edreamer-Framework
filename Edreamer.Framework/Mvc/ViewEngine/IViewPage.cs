using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Context;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Security.Authorization;
using Edreamer.Framework.UI.MetaData;
using Edreamer.Framework.UI.Notification;
using Edreamer.Framework.UI.Resources;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    public interface IViewPage: IViewDataContainer
    {
        Localizer T { get; }
        string Layout { get; set; }
        string Template { get; set; }
        IEnumerable<NotifyEntry> Notifications { get; }
        IWorkContext WorkContext { get; }
        ICompositionContainer Container { get; }
        IAuthorizer Authorizer { get; }
        IResourceManager ResourceManager { get; }
        IMetaDataManager MetaDataManager { get; }
        string Content(string contentPath);
        RequireSettings RequireResource(string resourceType, string resourceName);
        RequireSettings IncludeResource(string resourceType, string resourceUrl, Func<ResourceDefinition, ResourceDefinition> resourceCreator = null);
        MetaEntry IncludeMetaData(string content = null, string name = null, string httpEquiv = null, string charset = null);
    }
}
