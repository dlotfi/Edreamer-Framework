// Based on the original work of Maarten Balliauw, published as part of MefContrib but using MEF 2 features

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using MefContrib.Hosting.Interception;
using MefContrib.Hosting.Interception.Configuration;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// CompositionContainerAccessor
    /// </summary>
    public class CompositionContainerAccessor : ICompositionContainerAccessor
    {
        private readonly IRequestContext _requestContext;
        private readonly CompositionContainer _applicationScopeContainer;
        private readonly ComposablePartCatalog _requestScopeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerAccessor"/> class.
        /// </summary>
        /// <param name="scanningPaths">Paths to scan for parts.</param>
        /// <param name="requestContext">Context used to store container. If not specified <see cref="CurrentRequestContext"/> is used.</param>
        public CompositionContainerAccessor(IEnumerable<string> scanningPaths, IRequestContext requestContext = null)
        {
            _requestContext = requestContext ?? new CurrentRequestContext();

            var physicalScanningPaths = new HashSet<string>();
            physicalScanningPaths.AddRange(CollectionHelpers.EmptyIfNull(scanningPaths).Select(PathHelpers.GetPhysicalPath));

            var conventions = GetRegisterationConventions( physicalScanningPaths);
            var catalogs = new List<ComposablePartCatalog>();
            catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly(), conventions));
            catalogs.AddRange(physicalScanningPaths.Select(p => new DirectoryCatalog(p, conventions)));
            var interceptor = new CompositionInterceptor();
            var cfg = new InterceptionConfiguration()
                .AddHandler(new PrioritizedExportHandler(() => Container.GetExportedValue<IModuleManager>()))
                .AddInterceptionCriteria(new PredicateInterceptionCriteria(interceptor, cpd => !cpd.ContainsPartMetadataWithKey(CompositionConstants.IgnoreInterceptMetadataName) || !(bool)cpd.Metadata[CompositionConstants.IgnoreInterceptMetadataName]));
            var extendedCatalog = new InterceptingCatalog(new AggregateDistinctCatalog(catalogs), cfg);

            // Filter the global part catalog to a set of parts that has been marked as application scoped.
            var applicationScopeCatalog = extendedCatalog.Filter(cpd => cpd.ContainsPartMetadata(CompositionConstants.ApplicationScopeMetadataName, true));
            _applicationScopeContainer = new CompositionContainer(applicationScopeCatalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            // Add container to its application scope container so that it can be resolved.
            var batch = new CompositionBatch();
            batch.AddExportedValue<ICompositionContainerAccessor>(this);
            _applicationScopeContainer.Compose(batch);

            // Filter the global part catalog to a set of parts that has not been marked as application scoped.
            _requestScopeCatalog = applicationScopeCatalog.Complement;

            interceptor.Interceptors.Clear();
            interceptor.Interceptors.AddRange(Container.GetExportedValues<ICompositionInterceptor>());
        }

        /// <summary> 
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public ICompositionContainer Container
        {
            get
            {
                lock (this)
                {
                    if (!_requestContext.Items.Contains(CompositionConstants.ContainerKey))
                    {
                        ICompositionContainer container = new CompositionContainerWrapper(
                            new CompositionContainer(_requestScopeCatalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe, _applicationScopeContainer));
                        _requestContext.Items.Add(CompositionConstants.ContainerKey, container);
                    }
                }
                return (ICompositionContainer)_requestContext.Items[CompositionConstants.ContainerKey];
            }
        }

        /// <remarks>
        ///  The precedence of specified scope, creation policy and priority is as follows (in descending order):
        ///  1 - Those specified by PartCreationPolicy, PartPriority and ApplicationScope attributes
        ///      applied to parts themselves (no checking for conflicts)
        ///  2 - Those specified in part registrars (no checking for conflicts)
        ///  3 - Those specified by InterfaceExport attributes (checks for conflicts)
        /// </remarks>
        protected virtual ReflectionContext GetRegisterationConventions(IEnumerable<string> scanningPaths)
        {
            var conventions = new RegistrationBuilder();

            // 3 - InterfaceExport attributes
            conventions.ForTypesMatching(ImplementsAnyInterfaceExports)
                .ExportInterfaces(HasInterfaceExportAttribute);

            conventions.ForTypesMatching(ImplementsInterfaceExportsWithApplicationScope)
                .AddMetadata(CompositionConstants.ApplicationScopeMetadataName, true);

            conventions.ForTypesMatching(ImplementsAnyInterfaceExportsWithSharedCreationPolicy)
                .SetCreationPolicy(CreationPolicy.Shared);

            conventions.ForTypesMatching(ImplementsAnyInterfaceExportsWithNonSharedCreationPolicy)
                .SetCreationPolicy(CreationPolicy.NonShared);

            // 2 - Part registrars
            // Configure a composition container to find all part registrars
            var rb = new RegistrationBuilder();
            rb.ForTypesDerivedFrom<IPartRegistrar>()
                .Export<IPartRegistrar>();
            var partRegistrationCatalog = new AggregateDistinctCatalog();
            partRegistrationCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly(), rb));
            partRegistrationCatalog.Catalogs.AddRange(scanningPaths.Select(p => new DirectoryCatalog(p, rb)));
            using (var partRegistrationContainer = new CompositionContainer(partRegistrationCatalog))
            {
                // Find all part registrars 
                var partRegistrars = partRegistrationContainer.GetExportedValues<IPartRegistrar>();
                partRegistrars.ForEach(p => p.DefineConventions(conventions));
            }

            // 1 - Attributes
            conventions.ForTypesMatching(HasApplicationScopeAttribute)
                .AddMetadata(CompositionConstants.ApplicationScopeMetadataName, true);

            conventions.ForTypesMatching(HasPartPriorityAttribute)
                .AddMetadata(CompositionConstants.PriorityMetadataName, GetPartPriority);


            //Other conventions

            // ToDo-Low [12081115]: It's better not to add part metadata to system types due to peformance considerations.
            // There's not a good method to distinguish system types form user defined types but this method can be used:
            // http://stackoverflow.com/questions/3174921/how-do-i-determine-if-system-type-is-a-custome-type-or-a-framework-type
            conventions.ForTypesMatching(t => true)
                .AddMetadata(CompositionConstants.TypeMetadataName, t => t);

            conventions.ForTypesDerivedFrom<ICompositionInterceptor>()
                .Export<ICompositionInterceptor>()
                .AddMetadata(CompositionConstants.IgnoreInterceptMetadataName, true);

            return conventions;
        }


        #region Private Methods
        private static bool ImplementsAnyInterfaceExports(Type type)
        {
            return type.IsClass &&
                   type.GetInterfaces().Any(i => i.GetCustomAttributes(typeof(InterfaceExportAttribute), true).Any());
        }

        private static bool HasInterfaceExportAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(InterfaceExportAttribute), true).Any();
        }

        private static bool ImplementsInterfaceExportsWithApplicationScope(Type type)
        {
            return type.IsClass && GetInterfaceExportsScope(type) == Scope.Application;
        }

        private static bool ImplementsAnyInterfaceExportsWithSharedCreationPolicy(Type type)
        {
            return type.IsClass && GetInterfaceExportsCreationPolicy(type) == CreationPolicy.Shared;
        }

        private static bool ImplementsAnyInterfaceExportsWithNonSharedCreationPolicy(Type type)
        {
            return type.IsClass && GetInterfaceExportsCreationPolicy(type) == CreationPolicy.NonShared;
        }

        private static Scope? GetInterfaceExportsScope(Type type)
        {
            var interfaceExportAttributes = type.GetInterfaces().SelectMany(i => i.GetCustomAttributes(typeof(InterfaceExportAttribute), true).OfType<InterfaceExportAttribute>()).ToList();
            if (!interfaceExportAttributes.Any()) return null;
            var scope = interfaceExportAttributes.First().Scope;
            Throw.IfNot(interfaceExportAttributes.All(a => a.Scope == scope))
                .A<InvalidOperationException>("There's a conflict between scopes of implemented interfaces.");
            return scope;
        }

        private static CreationPolicy GetInterfaceExportsCreationPolicy(Type type)
        {
            var interfaceExportAttributes = type.GetInterfaces().SelectMany(i => i.GetCustomAttributes(typeof(InterfaceExportAttribute), true).OfType<InterfaceExportAttribute>());
            var creationPolicy = CreationPolicy.Any;
            foreach (var attribute in interfaceExportAttributes)
            {
                Throw.If(creationPolicy != CreationPolicy.Any && attribute.CreationPolicy != CreationPolicy.Any && attribute.CreationPolicy != creationPolicy)
                    .A<InvalidOperationException>("There's a conflict between creation policies of implemented interfaces.");
                creationPolicy = attribute.CreationPolicy;
            }
            return creationPolicy;
        }

        private static bool HasApplicationScopeAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(ApplicationScopeAttribute), false).Any();
        }

        private static bool HasPartPriorityAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(PartPriorityAttribute), false).Any();
        }

        private static object GetPartPriority(Type type)
        {
            return type.GetCustomAttributes(typeof(PartPriorityAttribute), false).OfType<PartPriorityAttribute>().Single().Priority;
        }
        #endregion
    }
}
