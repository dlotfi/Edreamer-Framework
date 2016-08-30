// Based on Matthew Abbott's article "MVC3 and MEF" - http://www.fidelitydesign.net/?p=259

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Collections
{
    /// <summary>
    /// Represents a list whose items can be dependent on each other, and enumerating the list will sort the items
    /// as per their dependency requirements.
    /// </summary>
    /// <typeparam name="TModel">The model in the list.</typeparam>
    /// <typeparam name="TKey">The identifier type.</typeparam>
    public class DependencyList<TModel, TKey> : IList<TModel>
    {
        #region Fields
        private readonly Func<TModel, IEnumerable<TKey>> _dependancyFunc;
        private readonly Func<TModel, TKey> _identifierFunc;
        private readonly List<DependencyListNode<TModel, TKey>> _nodes;

        private bool _modified;
        private List<TModel> _lastSort;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="DependencyList{TModel,TKey}"/>.
        /// </summary>
        /// <param name="identifierFunc">A delegate used to get the identifier for an item.</param>
        /// <param name="dependancyFunc">A delegate used to get dependancies for an item.</param>
        public DependencyList(Func<TModel, TKey> identifierFunc, Func<TModel, IEnumerable<TKey>> dependancyFunc)
        {
            Throw.IfArgumentNull(identifierFunc, "identifierFunc");
            Throw.IfArgumentNull(dependancyFunc, "dependancyFunc");

            _identifierFunc = identifierFunc;
            _dependancyFunc = dependancyFunc;

            _nodes = new List<DependencyListNode<TModel, TKey>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a count of the number of items in the list.
        /// </summary>
        public int Count { get { return _nodes.Count; } }

        /// <summary>
        /// Gets whether the list is read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        public void Add(TModel item)
        {
            var identifier = _identifierFunc(item);
            var node = new DependencyListNode<TModel, TKey>(item, identifier);

            if (item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += (s, e) => { _modified = true; };

            _nodes.Add(node);
            _modified = true;
        }

        /// <summary>
        /// Adds any dependancies required by the specified node.
        /// </summary>
        /// <param name="node">The node to add dependancies to.</param>
        private void AddDependancies(DependencyListNode<TModel, TKey> node)
        {
            var dependancies = _dependancyFunc(node.Item);
            node.Dependencies.Clear();

            if (dependancies == null)
                return;

            foreach (var dependancy in dependancies)
            {
                var dependantNode = _nodes
                    .Where(n => n.Identifier.Equals(dependancy))
                    .FirstOrDefault();

                if (dependantNode == null)
                    Throw.Now.A<InvalidOperationException>("A dependant item with key '{0}' is missing.".FormatWith(dependancy));

                node.Dependencies.Add(dependantNode);
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
            _modified = true;
        }

        /// <summary>
        /// Determines if the list contains the specified item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>True if the list contains the item, otherwise false.</returns>
        public bool Contains(TModel item)
        {
            return _nodes.Any(n => n.Item.Equals(item));
        }

        /// <summary>
        /// Copies the items to the specified array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="index">The index at which to start copying.</param>
        public void CopyTo(TModel[] array, int index)
        {
            var items = Sort();

            items.CopyTo(array, index);
        }

        /// <summary>
        /// Gets an enumerator for enumerating over items in the list.
        /// </summary>
        /// <returns>An enumerator for enumerating over items in the list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for enumerating over items in the list.
        /// </summary>
        /// <returns>An enumerator for enumerating over items in the list.</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            var list = Sort();
            return list.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of the specified item in the list.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of the item in the list.</returns>
        public int IndexOf(TModel item)
        {
            var list = Sort();
            return list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item into the collection.  This operation is not supported.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, TModel item)
        {
            Throw.Now.A<NotSupportedException>("The operation 'Insert' is not supported.");
        }

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed, otherwise false.</returns>
        public bool Remove(TModel item)
        {
            var node = _nodes.Where(n => n.Item.Equals(item)).FirstOrDefault();
            if (node == null)
                return false;

            _modified = true;
            return _nodes.Remove(node);
        }

        /// <summary>
        /// Removes the item at the specified index. This operation is not supported.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            Throw.Now.A<NotSupportedException>("The operation 'RemoveAt' is not supported.");
        }

        /// <summary>
        /// Returns the sorted collection of items.
        /// </summary>
        /// <returns>The sorted collection of items.</returns>
        internal List<TModel> Sort()
        {
            if (_modified || _lastSort == null)
                _lastSort = SortInternal();

            return _lastSort;
        }

        /// <summary>
        /// Returns the sorted collection of items.
        /// </summary>
        /// <returns>The sorted collection of items.</returns>
        private List<TModel> SortInternal()
        {
            var sort = new List<DependencyListNode<TModel, TKey>>();

            _nodes.ForEach(n =>
            {
                n.Visited = false;
                AddDependancies(n);
            });
            _nodes.ForEach(n => Visit(n, sort, n));

            _modified = false;

            return sort
                .Select(n => n.Item)
                .ToList();
        }

        /// <summary>
        /// Gets or sets the item at the specified index. Set operations are not supported.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The instance at the specified index.</returns>
        public TModel this[int index]
        {
            get { return Sort()[index]; }
            set
            {
                Throw.Now.A<NotSupportedException>("The operation 'this.Set' is not supported.");
            }
        }

        /// <summary>
        /// Performs a visit on a node.
        /// </summary>
        /// <param name="node">The current node being visited.</param>
        /// <param name="list">The dependancy sorted list of items.</param>
        /// <param name="root">The root node.</param>
        /// <returns>True if the node has not previously been visited and is not the root node.</returns>
        private static bool Visit(
            DependencyListNode<TModel, TKey> node,
            List<DependencyListNode<TModel, TKey>> list,
            DependencyListNode<TModel, TKey> root)
        {
            if (node.Visited)
                return (node.Root != root);

            node.Visited = true;
            node.Root = root;

            foreach (var dependancy in node.Dependencies)
            {
                Throw.If(!Visit(dependancy, list, root) && node != root)
                     .A<InvalidOperationException>("A cyclic dependancy on item with key '{0}' has occured.".FormatWith(dependancy.Identifier));
            }

            list.Add(node);

            return true;
        }
        #endregion
    }

    /// <summary>
    /// Represents a node within a dependency list.
    /// </summary>
    /// <typeparam name="TModel">The model in the list.</typeparam>
    /// <typeparam name="TKey">The identifier type.</typeparam>
    internal class DependencyListNode<TModel, TKey>
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="DependencyListNode{TModel,TKey}"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="identifier">The identifier of the item.</param>
        public DependencyListNode(TModel item, TKey identifier)
        {
            Item = item;
            Identifier = identifier;

            Dependencies = new List<DependencyListNode<TModel, TKey>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of dependancies.
        /// </summary>
        public List<DependencyListNode<TModel, TKey>> Dependencies { get; private set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public TKey Identifier { get; set; }

        /// <summary>
        /// Gets or sets the model instance.
        /// </summary>
        public TModel Item { get; set; }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        public DependencyListNode<TModel, TKey> Root { get; set; }

        /// <summary>
        /// Gets or sets whether this item has been visited.
        /// </summary>
        public bool Visited { get; set; }
        #endregion
    }
}