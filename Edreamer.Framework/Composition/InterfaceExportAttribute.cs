using System;
using System.ComponentModel.Composition;

namespace Edreamer.Framework.Composition
{
    public enum Scope
    {
        Request,
        Application
    }

    /// <summary>
    /// Marks all implementaion of an interface for export and specifies the default scope and creation policy of them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class InterfaceExportAttribute : Attribute
    {
        #region Constructor
        public InterfaceExportAttribute()
            : this(Scope.Request, CreationPolicy.Any)
        {
        }

        public InterfaceExportAttribute(Scope scope)
            : this(scope, CreationPolicy.Any)
        {
        }

        public InterfaceExportAttribute(CreationPolicy creationPolicy)
            : this(Scope.Request, creationPolicy)
        {
        }

        public InterfaceExportAttribute(Scope scope, CreationPolicy creationPolicy)
        {
            Scope = scope;
            CreationPolicy = creationPolicy;
        }
        #endregion

        #region Properties
        
        public Scope Scope { get; private set; }
        public CreationPolicy CreationPolicy { get; private set; }

        #endregion
    }

    /// <summary>
    /// Marks all implementaion of an interface for export and specifies application scope with shared creation policy for them.
    /// </summary>
    public class InterfaceExportSingletonAttribute : InterfaceExportAttribute
    {
        public InterfaceExportSingletonAttribute()
            : base(Scope.Application, CreationPolicy.Shared)
        {
        }
    }
}
