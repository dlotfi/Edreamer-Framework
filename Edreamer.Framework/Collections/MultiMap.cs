// Based on original work of Frans Bouma - http://stackoverflow.com/questions/380595/multimap-in-net

using System.Collections.Generic;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Collections
{
    /// <summary>
    /// Extension to the normal Dictionary. This class can store more than one value for every key. It keeps a List for every Key value.
    /// Calling Add with the same Key and multiple values will store each value under the same Key in the Dictionary. Obtaining the values
    /// for a Key will return the List with the Values of the Key. 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class MultiMap<TKey, TValue> : Dictionary<TKey, ICollection<TValue>>
    {
        /// <summary>
        /// Adds the specified value under the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            Throw.IfArgumentNull(key, "key");

            ICollection<TValue> container;
            if (!TryGetValue(key, out container))
            {
                container = new List<TValue>();
                Add(key, container);
            }
            container.Add(value);
        }


        /// <summary>
        /// Determines whether this dictionary contains the specified value for the specified key 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            Throw.IfArgumentNull(key, "key");
            bool toReturn = false;
            ICollection<TValue> values;
            if (TryGetValue(key, out values))
            {
                toReturn = values.Contains(value);
            }
            return toReturn;
        }


        /// <summary>
        /// Removes the specified value for the specified key. It will leave the key in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Remove(TKey key, TValue value)
        {
            Throw.IfArgumentNull(key, "key");

            ICollection<TValue> container;
            if (TryGetValue(key, out container))
            {
                container.Remove(value);
                if (container.Count <= 0)
                {
                    Remove(key);
                }
            }
        }


        /// <summary>
        /// Merges the specified multivaluedictionary into this instance.
        /// </summary>
        /// <param name="toMergeWith">To merge with.</param>
        public void Merge(MultiMap<TKey, TValue> toMergeWith)
        {
            if (toMergeWith == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, ICollection<TValue>> pair in toMergeWith)
            {
                foreach (TValue value in pair.Value)
                {
                    Add(pair.Key, value);
                }
            }
        }


        /// <summary>
        /// Gets the values for the key specified. This method is useful if you want to avoid an exception for key value retrieval and you can't use TryGetValue
        /// (e.g. in lambdas)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="returnEmptyCollection">if set to true and the key isn't found, an empty collection is returned, otherwise, if the key isn't found, null is returned</param>
        /// <returns>
        /// This method will return null (or an empty collection if returnEmptyCollection is true) if the key wasn't found, or
        /// the values if key was found.
        /// </returns>
        public ICollection<TValue> GetValues(TKey key, bool returnEmptyCollection)
        {
            ICollection<TValue> toReturn;
            if (!TryGetValue(key, out toReturn) && returnEmptyCollection)
            {
                toReturn = new List<TValue>();
            }
            return toReturn;
        }
    }
}

