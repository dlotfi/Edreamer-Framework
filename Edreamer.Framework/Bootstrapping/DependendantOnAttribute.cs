using System;

namespace Edreamer.Framework.Bootstrapping
{
    /// <summary>
    /// Specifies the target class dependencies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class DependendantOnAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="DependendantOnAttribute"/>.
        /// </summary>
        /// <param name="dependencies">Any named dependencies the attributed class is explicitly dependent on.</param>
        public DependendantOnAttribute(params string[] dependencies)
        {
            Dependencies = dependencies;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        public string[] Dependencies { get; private set; }
        #endregion
    }
}