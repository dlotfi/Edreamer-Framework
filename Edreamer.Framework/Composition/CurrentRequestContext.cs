// Based on the original work of Maarten Balliauw, published as part of MefContrib

using System.Collections;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Web;

namespace Edreamer.Framework.Composition
{
    /// <summary>
    /// CurrentRequestContext
    /// </summary>
    public class CurrentRequestContext : IRequestContext
    {
        private static ConcurrentDictionary<object, object> _items;

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IDictionary Items
        {
            get
            {
                if (HttpContext.Current != null) 
                {
                    return HttpContext.Current.Items;
                }
                if (WcfOperationContext.Current != null)
                {
                    return WcfOperationContext.Current.Items;
                }
                return _items ?? (_items = new ConcurrentDictionary<object, object>());
            }
        }
    }

    /// <summary>
    /// WcfOperationContext extension
    /// </summary>
    internal class WcfOperationContext : IExtension<OperationContext>
    {
        private readonly IDictionary _items;

        private WcfOperationContext()
        {
            _items = new Hashtable();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IDictionary Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the current WcfOperationContext.
        /// </summary>
        /// <value>The current WcfOperationContext.</value>
        public static WcfOperationContext Current
        {
            get
            {
                if (OperationContext.Current == null) return null;
                var context = OperationContext.Current.Extensions.Find<WcfOperationContext>();
                if (context == null)
                {
                    context = new WcfOperationContext();
                    OperationContext.Current.Extensions.Add(context);
                }
                return context;
            }
        }

        /// <summary>
        /// Attaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Attach(OperationContext owner) { }

        /// <summary>
        /// Detaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Detach(OperationContext owner) { }
    }
}
