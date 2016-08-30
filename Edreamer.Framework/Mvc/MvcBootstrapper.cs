using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Edreamer.Framework.Bootstrapping;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Context;
using Edreamer.Framework.Mvc.Composition;
using Edreamer.Framework.Mvc.Layouts;
using Edreamer.Framework.Mvc.Routes;
using Edreamer.Framework.Mvc.Templates;
using Edreamer.Framework.Mvc.Validation;
using Edreamer.Framework.Mvc.ViewEngine;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Validation;

namespace Edreamer.Framework.Mvc
{
    [DependendantOn("Injection")]
    public class MvcBootstrapper : IBootstrapperTask
    {
        private readonly ICompositionContainerAccessor _compositionContainerAccessor;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILayoutSelector _layoutSelector;
        private readonly ITemplateSelector _templateSelector;
        private readonly IRoutesPublisher _routesPublisher;
        private readonly IViewLocationProvider _viewLocationProvider;
        private readonly IEnumerable<IRouteRegistrar> _routeRegistrars;
        private readonly IEnumerable<IValidatorProvider> _validatorProviders;

        public MvcBootstrapper(ICompositionContainerAccessor compositionContainerAccessor,
            IWorkContextAccessor workContextAccessor,
            ILayoutSelector layoutSelector, ITemplateSelector templateSelector,
            IRoutesPublisher routePublisher, IViewLocationProvider viewLocationProvider,
            IEnumerable<IRouteRegistrar> routeRegistrars,
            IEnumerable<IValidatorProvider> validatorProviders)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            Throw.IfArgumentNull(layoutSelector, "layoutSelector");
            Throw.IfArgumentNull(templateSelector, "templateSelector");
            Throw.IfArgumentNull(routePublisher, "routePublisher");
            Throw.IfArgumentNull(viewLocationProvider, "viewLocationProvider");

            _compositionContainerAccessor = compositionContainerAccessor;
            _workContextAccessor = workContextAccessor;
            _layoutSelector = layoutSelector;
            _templateSelector = templateSelector;
            _routesPublisher = routePublisher;
            _viewLocationProvider = viewLocationProvider;
            _routeRegistrars = CollectionHelpers.EmptyIfNull(routeRegistrars);
            _validatorProviders = CollectionHelpers.EmptyIfNull(validatorProviders);
        }

        public void Run()
        {
            // Tell MVC to use MEF as its dependency resolver.
            var dependencyResolver = new CompositionDependencyResolver(_compositionContainerAccessor);
            DependencyResolver.SetResolver(dependencyResolver);

            // Tell MVC to resolve dependencies in controllers
            ControllerBuilder.Current.SetControllerFactory(
                new CompositionControllerFactory(new CompositionControllerActivator(_compositionContainerAccessor), _compositionContainerAccessor));

            // Tell MVC to resolve dependencies in filters
            FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
            FilterProviders.Providers.Add(new CompositionFilterAttributeFilterProvider(_compositionContainerAccessor));

            // Tell MVC to resolve model binders through MEF. A model binder should be decorated
            // with [ModelBinderExport].
            ModelBinderProviders.BinderProviders.Add(new CompositionModelBinderProvider(_compositionContainerAccessor));

            // Set the view engine that supports imported modules, automatic layout selection and templates from other modules.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ModuleViewEngine(_viewLocationProvider, _layoutSelector, _templateSelector, _workContextAccessor));

            // Register all routes defined in modules
            _routesPublisher.Publish(RouteTable.Routes, _routeRegistrars);
            
            RegisterClientValidatorAdapters();
            // Tell MVC to use ModelValidatorProviderAdapter which uses framework's validation mechanism.
            // Note that Framework's validation mechanism allows dependency injection in validation attributes.
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<ClientDataTypeModelValidatorProvider>().Single());
            ModelValidatorProviders.Providers.Add(new ModelValidatorProviderAdapter(_validatorProviders));
        }

        private static void RegisterClientValidatorAdapters()
        {
            DataAnnotationsValidatorProvider.RegisterDefaultAdapterFactory(
                (metadata, attribute, localizer) => new MvcDataAnnotationsValidatorAdapter(metadata, attribute, localizer));

            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(RangeAttribute),
                (metadata, attribute, localizer) => new Validation.RangeAttributeAdapter(metadata, (RangeAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(RegularExpressionAttribute),
                (metadata, attribute, localizer) => new Validation.RegularExpressionAttributeAdapter(metadata, (RegularExpressionAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(RequiredAttribute),
                (metadata, attribute, localizer) => new Validation.RequiredAttributeAdapter(metadata, (RequiredAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(StringLengthAttribute),
                (metadata, attribute, localizer) => new Validation.StringLengthAttributeAdapter(metadata, (StringLengthAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(System.ComponentModel.DataAnnotations.CompareAttribute),
                (metadata, attribute, localizer) => new Validation.CompareAttributeAdapter(metadata, (System.ComponentModel.DataAnnotations.CompareAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(EmailAddressAttribute),
                (metadata, attribute, localizer) => new Validation.EmailAddressAttributeAdapter(metadata, (EmailAddressAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(CreditCardAttribute),
                (metadata, attribute, localizer) => new Validation.CreditCardAttributeAdapter(metadata, (CreditCardAttribute)attribute, localizer));
            DataAnnotationsValidatorProvider.RegisterAdapterFactory(typeof(FileExtensionsAttribute),
                (metadata, attribute, localizer) => new Validation.FileExtensionsAttributeAdapter(metadata, (FileExtensionsAttribute)attribute, localizer));
        }
    }
}
