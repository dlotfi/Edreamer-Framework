using System;
using System.Collections.Generic;

namespace Edreamer.Framework.Module
{
    /// <summary>
    /// Defines an exception to be thrown when a problem occurs during a module registration.
    /// </summary>
    public class ModuleRegistrationException : Exception
    {
        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="ModuleRegistrationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModuleRegistrationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ModuleRegistrationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ModuleRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ModuleRegistrationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="module">The registered module that causes error.</param>
        /// <param name="confictingModules">The modules conflicting with the registered module.</param>
        public ModuleRegistrationException(string message, Module module, IEnumerable<Module> confictingModules)
            : base(message)
        {
            Module = module;
            ConflictingModules = confictingModules;
        }

        public Module Module { get; private set; }
        public IEnumerable<Module> ConflictingModules { get; private set; }

        #endregion
    }
}
