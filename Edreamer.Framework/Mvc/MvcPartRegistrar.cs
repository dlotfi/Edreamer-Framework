using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Web.Mvc;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Mvc.Composition;
using Edreamer.Framework.Mvc.Filters;
using Edreamer.Framework.Mvc.Layouts;
using Edreamer.Framework.Mvc.Routes;
using Edreamer.Framework.Mvc.Templates;
using Edreamer.Framework.Mvc.ViewEngine;
using Edreamer.Framework.Mvc.WebParts;

namespace Edreamer.Framework.Mvc
{
    public class MvcPartRegistrar : IPartRegistrar
    {
        public void DefineConventions(RegistrationBuilder rb)
        {
            rb.ForTypesDerivedFrom<IController>()
                .Export<IController>(c => c.AddMetadata("ControllerType", t => t))
                .Export()
                .SetCreationPolicy(CreationPolicy.NonShared);

            rb.ForTypesDerivedFrom<IActionInvoker>()
                .Export<IActionInvoker>();

            rb.ForTypesDerivedFrom<IExtraFilterProvider>()
                .Export<IExtraFilterProvider>();

            rb.ForTypesMatching(t => t.IsClass && typeof(IModelBinder).IsAssignableFrom(t) && t.GetCustomAttributes(typeof(BinderAttribute), false).Any())
                .Export<IModelBinder>(c => c.AddMetadata("ModelTypes", t => t.GetCustomAttributes(typeof(BinderAttribute), false).OfType<BinderAttribute>().First().ModelTypes));

            rb.ForTypesDerivedFrom<IViewLocationProvider>()
                .Export<IViewLocationProvider>();

            rb.ForTypesDerivedFrom<IRouteRegistrar>()
                .Export<IRouteRegistrar>();

            rb.ForTypesDerivedFrom<IRoutesPublisher>()
                .Export<IRoutesPublisher>();

            rb.ForTypesDerivedFrom<IAnyNamedRoute>()
                .ExportInterfaces(t => typeof(IAnyNamedRoute).IsAssignableFrom(t))
                .Export(); // 'Export' let us define route name classes without requiring to define an interface

            rb.ForTypesDerivedFrom<VoidNamedRouteBase>()
                .SetPriority(PartPriorityAttribute.Minimum);

            rb.ForTypesDerivedFrom<ILayoutRegistrar>()
                .Export<ILayoutRegistrar>();

            rb.ForTypesDerivedFrom<ILayoutSelector>()
                .Export<ILayoutSelector>();

            rb.ForTypesDerivedFrom<ITemplateRegistrar>()
                .Export<ITemplateRegistrar>();

            rb.ForTypesDerivedFrom<ITemplateSelector>()
                .Export<ITemplateSelector>();

            rb.ForTypesDerivedFrom<IPartialStore>()
                .Export<IPartialStore>();

            rb.ForTypesDerivedFrom<IAnyWebPart>()
                .ExportInterfaces(t => typeof(IAnyWebPart).IsAssignableFrom(t));

            rb.ForTypesDerivedFrom<IViewPageActivator>()
                .Export<IViewPageActivator>();
        }
    }
}
