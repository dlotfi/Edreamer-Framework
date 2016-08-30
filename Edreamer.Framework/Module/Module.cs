using System.Collections.Generic;

namespace Edreamer.Framework.Module
{
    public abstract class Module
    {
        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the namespaces of the module.
        /// </summary>
        public virtual IEnumerable<string> Namespaces
        {
            get { return new[] { GetType().Namespace + ".*" }; }
        }

        /// <summary>
        /// Gets the assembly qualified name of the module.
        /// </summary>
        public virtual string Assembly
        {
            get { return GetType().Assembly.FullName; }
        }

        //#region Equality
        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as Module);
        //}

        //public bool Equals(Module other)
        //{
        //    if (ReferenceEquals(null, other)) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return other.Name.EqualsIgnoreCase(Name);
        //}

        //public override int GetHashCode()
        //{
        //    return (Name != null ? Name.ToLower().GetHashCode() : 0);
        //}

        //public static bool operator ==(Module left, Module right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(Module left, Module right)
        //{
        //    return !Equals(left, right);
        //}
        //#endregion
    }
}
