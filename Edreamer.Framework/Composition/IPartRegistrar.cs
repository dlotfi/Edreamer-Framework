using System.ComponentModel.Composition.Registration;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Defines the required contract for implementing a part registrar.
    /// </summary>
    public interface IPartRegistrar
    {
        #region Methods
        /// <summary>
        /// Defines conventions that should be used by composition container.
        /// </summary>
        /// <param name="rb">The registration builder object used to define conventions.</param>
        void DefineConventions(RegistrationBuilder rb);
        #endregion
    }
}
