using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Module
{
    public abstract class ModuleManagerBase: IModuleManager
    {
        public abstract void Initialize(IEnumerable<Module> modules);
        public abstract void CompleteInitialization();
        public abstract IEnumerable<Module> Modules { get; }
        public abstract bool TryGetModule(string assembly, string @namespace, out Module module);

        public Module GetModule(Type type)
        {
            Throw.IfArgumentNull(type, "type");
            return GetModule(type.Assembly.FullName, type.Namespace);
        }

        public bool TryGetModule(Type type, out Module module)
        {
            Throw.IfArgumentNull(type, "type");
            return TryGetModule(type.Assembly.FullName, type.Namespace, out module);
        }

        public Module GetModule(string assembly, string @namespace)
        {
            Module module;
            Throw.IfNot(TryGetModule(assembly, @namespace, out module))
                .A<ResolvingModuleException>("No module has been registered for assembly '{0}' and namespace '{1}'."
                .FormatWith(assembly, @namespace));
            return module;
        }

        public bool ModuleExists(string moduleName)
        {
            Throw.IfArgumentNullOrEmpty(moduleName, "moduleName");
            return Modules.Any(m => m.Name.EqualsIgnoreCase(moduleName));
        }
    }
}
