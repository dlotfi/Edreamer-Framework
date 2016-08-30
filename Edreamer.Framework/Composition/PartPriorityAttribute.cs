using System;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// Specifies the priority for a part.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PartPriorityAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Initialises a new instance of <see cref="PartPriorityAttribute"/>.
        /// </summary>
        /// <param name="priority">A value specifying the priority.</param>
        public PartPriorityAttribute(int priority)
        {
            Priority = priority;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the priority value.
        /// </summary>
        public int Priority { get; private set; }
        #endregion

        #region Consts

        /// <summary>
        /// Default priority of a part.
        /// </summary>
        public const int Default = 0;

        /// <summary>
        /// Minimum possible priority.
        /// </summary>
        public const int Minimum = int.MinValue;

        /// <summary>
        /// Maximum possible priority.
        /// </summary>
        public const int Maximum = int.MaxValue;

        #endregion
    }
}
