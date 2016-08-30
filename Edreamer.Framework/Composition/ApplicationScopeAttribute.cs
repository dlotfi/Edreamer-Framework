using System;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Specifies application scope for a part.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApplicationScopeAttribute : Attribute
    {
    }
}
