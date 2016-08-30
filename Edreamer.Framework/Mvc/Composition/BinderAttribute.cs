using System;

namespace Edreamer.Framework.Mvc.Composition
{
    /// <summary>
    /// BinderAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BinderAttribute: Attribute, IBinderMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinderAttribute"/> class.
        /// </summary>
        /// <param name="modelTypes">Types of the model.</param>
        public BinderAttribute(params Type[] modelTypes)
        {
            ModelTypes = modelTypes;
        }

        /// <summary>
        /// Gets or sets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] ModelTypes { get; set; }
    }
}
