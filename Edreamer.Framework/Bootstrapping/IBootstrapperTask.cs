namespace Edreamer.Framework.Bootstrapping
{
    /// <summary>
    /// Defines the required contract for implementing a bootstrapper task.
    /// </summary>
    public interface IBootstrapperTask
    {
        #region Methods
        /// <summary>
        /// Runs the task.
        /// </summary>
        void Run();
        #endregion
    }
}