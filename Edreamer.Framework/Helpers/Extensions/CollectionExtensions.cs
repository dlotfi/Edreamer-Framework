using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Edreamer.Framework.Helpers
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="source">The collection to be added to.</param>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ICollection{T}"/>.</param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(collection, "collection");

            foreach (var item in collection.ToList())
            {
                source.Add(item);
            }
        }

        /// <summary>
        /// Removes the elements of the specified collection from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="source">The collection to be removed from.</param>
        /// <param name="collection">The collection whose elements should be removed from the <see cref="ICollection{T}"/>.</param>
        public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(collection, "collection");

            foreach (var item in collection.ToList())
            {
                source.Remove(item);
            }
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to perform action on it.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="ICollection{T}"/>.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(action, "action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Assigns the specified value to the top item in the <see cref="Stack{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="Stack{T}"/> to change its top item.</param>
        /// <param name="value">The value to assign to the top item in the <see cref="Stack{T}"/>.</param>
        public static void SetTopItem<T>(this Stack<T> source, T value)
        {
            Throw.IfArgumentNull(source, "source");
            lock (source)
            {
                source.Pop();
                source.Push(value);
            }
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlyCollection{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create a <see cref="ReadOnlyCollection{T}"/> from.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            Throw.IfArgumentNull(source, "source");
            return new ReadOnlyCollection<T>(source.ToList());
        }

        /// <summary>
        /// Gets the top priority item of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the priority key.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to find its top priority item.</param>
        /// <param name="prioritySelector">A function to extract a key from an element.</param>
        /// <returns>the top priority item.</returns>
        public static T GetTopPriorityItem<T, TKey>(this IEnumerable<T> source, Func<T, TKey> prioritySelector)
        {
            return GetTopPriorityItem(source, prioritySelector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Gets the top priority item of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the priority key.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to find its top priority item.</param>
        /// <param name="prioritySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        /// <returns>the top priority item.</returns>
        public static T GetTopPriorityItem<T, TKey>(this IEnumerable<T> source, Func<T, TKey> prioritySelector, IComparer<TKey> comparer)
        {
            var topItems = GetTopPriorityItems(source, prioritySelector, comparer);
            Throw.If(topItems.Count() > 1)
                .AnArgumentException("The source contains more than one top priority item.", "source");
            return topItems.First();
        }

        /// <summary>
        /// Gets the top priority items of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the priority key.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to find its top priority items.</param>
        /// <param name="prioritySelector">A function to extract a key from an element.</param>

        /// <returns>an <see cref="IEnumerable{T}"/> of top priority items.</returns>
        public static IEnumerable<T> GetTopPriorityItems<T, TKey>(this IEnumerable<T> source, Func<T, TKey> prioritySelector)
        {
            return GetTopPriorityItems(source, prioritySelector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Gets the top priority items of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the priority key.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to find its top priority items.</param>
        /// <param name="prioritySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        /// <returns>an <see cref="IEnumerable{T}"/> of top priority items.</returns>
        public static IEnumerable<T> GetTopPriorityItems<T, TKey>(this IEnumerable<T> source, Func<T, TKey> prioritySelector, IComparer<TKey> comparer)
        {
            Throw.IfArgumentNullOrEmpty(source, "source");
            Throw.IfArgumentNull(prioritySelector, "prioritySelector");
            Throw.IfArgumentNull(comparer, "comparer");
            var orderedSource = source.OrderByDescending(prioritySelector, comparer);
            var topPriority = orderedSource.Select(prioritySelector).First();
            return orderedSource.Where(x => comparer.Compare(prioritySelector(x), topPriority) == 0);
        }

        public static bool HasDuplicateElements<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(comparer, "comparer");
            var set = new HashSet<T>(comparer);
            return source.All(item => !set.Add(item));
        }

        public static bool HasDuplicateElements(this IEnumerable source)
        {
            Throw.IfArgumentNull(source, "source");
            return HasDuplicateElements(source.Cast<object>(), EqualityComparer<object>.Default);
        }

        public static bool HasDuplicateElements<T>(this IEnumerable<T> source, Func<T, object> keyExtractor)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(keyExtractor, "keyExtractor");
            return HasDuplicateElements(source, new KeyEqualityComparer<T>(keyExtractor));
        }
    }
}
