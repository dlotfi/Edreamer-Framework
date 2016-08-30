using System;

namespace Edreamer.Framework.Mvc.Security
{
    /// <summary>
    /// Specifies this action is used for authenticating users and can be called by everyone.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthActionAttribute : Attribute
    {
    }
}
