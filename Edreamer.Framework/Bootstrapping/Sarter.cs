using System;
using System.Linq;
using Edreamer.Framework.Collections;
using System.Collections.Generic;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Module;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Bootstrapping
{
    /// <summary>
    /// This class is used to start the application.
    /// </summary>
    public static class Sarter
    {
        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="compositionContainerAccessor">Composition container accessor used to access to the composition container.</param>
        public static void Run(ICompositionContainerAccessor compositionContainerAccessor)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");

            // Register all modules
            var moduleManager = compositionContainerAccessor.Container.GetExportedValue<IModuleManager>();
            var modules = compositionContainerAccessor.Container.GetExportedValues<Module.Module>().ToList();
            moduleManager.Initialize(modules);

            // Executes all bootstrapper tasks in order of their dependencies
            ExecuteBootstrapperTasks(compositionContainerAccessor.Container);

            // Complete modules registration
            moduleManager.CompleteInitialization();
        }

        #region Bootstrapper Tasks Execution

        private static void ExecuteBootstrapperTasks(ICompositionContainer container)
        {
            // Find all bootstrapper tasks 
            var tasks = container.GetExports<IBootstrapperTask, IBootstrapperTaskMetadata>().ToList();

            // Execute framework bootstrapper tasks in order of their dependencies
            ExecuteTasks(tasks.Where(t => t.Metadata.IsPartOfFramework));

            // Execute non framework bootstrapper tasks in order of their dependencies
            ExecuteTasks(tasks.Where(t => !t.Metadata.IsPartOfFramework));
        }

        private static void ExecuteTasks(IEnumerable<Lazy<IBootstrapperTask, IBootstrapperTaskMetadata>> tasks)
        {
            var list = new DependencyList<Lazy<IBootstrapperTask, IBootstrapperTaskMetadata>, string>(
                l => l.Metadata.Type.Name.TrimEnd(StringComparison.OrdinalIgnoreCase, "Bootstrapper"),
                l =>
                {
                    var attr = l.Metadata.Type
                                   .GetCustomAttributes(typeof(DependendantOnAttribute), true)
                                   .SingleOrDefault() as DependendantOnAttribute;
                    return attr == null ? null : attr.Dependencies;
                });

            list.AddRange(tasks);
            list.Select(x => x.Value).ForEach(t => t.Run());
        }

        #endregion
    }
}