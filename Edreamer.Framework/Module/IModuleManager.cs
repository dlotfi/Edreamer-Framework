using System;
using System.Collections.Generic;

namespace Edreamer.Framework.Module
{
    public interface IModuleManager
    {
        void Initialize(IEnumerable<Module> modules);
        void CompleteInitialization();

        IEnumerable<Module> Modules { get; }

        Module GetModule(Type type);
        Module GetModule(string assembly, string @namespace);

        bool TryGetModule(Type type, out Module module);
        bool TryGetModule(string assembly, string @namespace, out Module module);

        bool ModuleExists(string moduleName);
    }
}
