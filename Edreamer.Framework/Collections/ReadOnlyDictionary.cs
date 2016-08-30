﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Collections
{
    /// <summary>
    /// Provides the base class for a generic read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of values in the dictionary.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// An instance of the <b>ReadOnlyDictionary</b> generic class is
    /// always read-only. A dictionary that is read-only is simply a
    /// dictionary with a wrapper that prevents modifying the
    /// dictionary; therefore, if changes are made to the underlying
    /// dictionary, the read-only dictionary reflects those changes. 
    /// See <see cref="Dictionary{TKey,TValue}"/> for a modifiable version of 
    /// this class.
    /// </para>
    /// <para>
    /// <b>Notes to Implementers</b> This base class is provided to 
    /// make it easier for implementers to create a generic read-only
    /// custom dictionary. Implementers are encouraged to extend this
    /// base class instead of creating their own. 
    /// </para>
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    [DebuggerTypeProxy(typeof(ReadOnlyDictionaryDebugView<,>))]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        IDictionary
    {
        private readonly IDictionary<TKey, TValue> _source;
        private readonly bool _allowAssigning;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}" />
        ///  class that contains elements copied from the specified 
        /// <see cref="IDictionary{TKey, TValue}" /> and uses the default equality
        /// comparer for the key type.
        /// </summary>
        /// <param name="dictionaryToWrap">The <see cref="IDictionary{TKey, TValue}" />
        /// that will be wrapped.</param>
        /// <param name="allowAssigning">Determines if changing values of items is possible.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when the dictionary is null.
        /// </exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionaryToWrap, bool allowAssigning = false)
        {
            Throw.IfArgumentNull(dictionaryToWrap, "dictionaryToWrap");

            _source = dictionaryToWrap;
            _allowAssigning = allowAssigning;
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"></see>.
        /// </summary>
        /// <value>The number of key/value pairs.</value>
        /// <returns>The number of key/value pairs contained in the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"></see>.</returns>
        public int Count
        {
            get { return _source.Count; }
        }

        /// <summary>Gets a collection containing the keys in the
        /// <see cref="ReadOnlyDictionary{TKey,TValue}"></see>.</summary>
        /// <value>A <see cref="Dictionary{TKey,TValue}.KeyCollection"/> 
        /// containing the keys.</value>
        /// <returns>A
        /// <see cref="Dictionary{TKey,TValue}.KeyCollection"/>
        /// containing the keys in the
        /// <see cref="Dictionary{TKey,TValue}"></see>.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get { return _source.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the values of the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>The collection of values.</value>
        public ICollection<TValue> Values
        {
            get { return _source.Values; }
        }

        /// <summary>Gets a value indicating whether the dictionary is read-only.
        /// This value will always be true.</summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>Gets a value indicating whether the 
        /// dictionary has a fixed size. This property will
        /// always return true.</summary>
        bool IDictionary.IsFixedSize
        {
            get { return true; }
        }

        /// <summary>Gets a value indicating whether the 
        /// dictionary is read-only.This property will
        /// always return true.</summary>
        bool IDictionary.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>Gets an <see cref="ICollection"/> object 
        /// containing the keys of the dictionary object.</summary>
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_source).Keys; }
        }

        /// <summary>
        /// Gets an ICollection object containing the values in the DictionaryBase object.
        /// </summary>
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_source).Values; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the dictionary
        /// is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_source).IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to dictionary.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return ((ICollection)_source).SyncRoot; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// The value associated with the specified key. If the specified key
        /// is not found, a get or set operation throws a 
        /// <see cref="T:System.Collections.Generic.KeyNotFoundException" />.
        /// </returns>
        /// <param name="key">The key of the value to get or set.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when the key is null.
        /// </exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and key does not exist in the collection.
        /// </exception>
        public TValue this[TKey key]
        {
            get { return _source[key]; }
            set
            {
                if (_allowAssigning)
                    if (ContainsKey(key))
                        _source[key] = value;
                    else
                        new KeyNotFoundException("The given key was not present in the dictionary");
                else
                    ThrowNotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key. 
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key. </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when a value is set unless assigning has been allowed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the key is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_source)[key]; }
            set 
            {
                if (_allowAssigning)
                    ((IDictionary)_source)[key] = value;
                else
                    ThrowNotSupportedException();
            }
        }

        /// <summary>This method is not supported by the 
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.</param>
        /// <param name="value">
        /// The object to use as the value of the element to add.</param>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            ThrowNotSupportedException();
        }

        /// <summary>Determines whether the <see cref="ReadOnlyDictionary{TKey, TValue}" />
        /// contains the specified key.</summary>
        /// <returns>
        /// True if the <see cref="ReadOnlyDictionary{TKey, TValue}" /> contains
        /// an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"></see>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown when the key is null.
        /// </exception>
        public bool ContainsKey(TKey key)
        {
            return _source.ContainsKey(key);
        }

        /// <summary>
        /// This method is not supported by the <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// True if the element is successfully removed; otherwise, false.
        /// </returns>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            ThrowNotSupportedException();
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value
        /// associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>
        /// <b>true</b> if the <see cref="ReadOnlyDictionary{TKey, TValue}" /> contains
        /// an element with the specified key; otherwise, <b>false</b>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _source.TryGetValue(key, out value);
        }

        /// <summary>This method is not supported by the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <param name="item">
        /// The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(
            KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
        }

        /// <summary>This method is not supported by the 
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</summary>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a
        /// specific value.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        /// <returns>
        /// <b>true</b> if item is found in the <b>ICollection</b>; 
        /// otherwise, <b>false</b>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(
            KeyValuePair<TKey, TValue> item)
        {
            return _source.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a
        /// particular Array index. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the
        /// destination of the elements copied from ICollection.
        /// The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
            KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        /// <summary>This method is not supported by the
        /// <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <param name="item">
        /// The object to remove from the ICollection.
        /// </param>
        /// <returns>Will never return a value.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<TKey, TValue>>
            IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_source).GetEnumerator();
        }

        /// <summary>
        /// This method is not supported by the <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The System.Object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The System.Object to use as the value of the element to add.
        /// </param>
        void IDictionary.Add(object key, object value)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// This method is not supported by the <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        void IDictionary.Clear()
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the IDictionary object.
        /// </param>
        /// <returns>
        /// <b>true</b> if the IDictionary contains an element with the key;
        /// otherwise, <b>false</b>.
        /// </returns>
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_source).Contains(key);
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> for the
        /// <see cref="IDictionary"/>. 
        /// </summary>
        /// <returns>
        /// An IDictionaryEnumerator for the IDictionary.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_source).GetEnumerator();
        }

        /// <summary>
        /// This method is not supported by the <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// Gets an <see cref="ICollection"/> object containing the keys of the 
        /// <see cref="IDictionary"/> object.
        /// </param>
        void IDictionary.Remove(object key)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// For a description of this member, see <see cref="ICollection.CopyTo"/>. 
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from 
        /// ICollection. The Array must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in Array at which copying begins.
        /// </param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_source).CopyTo(array, index);
        }

        private static void ThrowNotSupportedException()
        {
            throw new NotSupportedException("This Dictionary is read-only");
        }
    }

    internal sealed class ReadOnlyDictionaryDebugView<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dict;

        public ReadOnlyDictionaryDebugView(ReadOnlyDictionary<TKey, TValue> dictionary)
        {
            Throw.IfArgumentNull(dictionary, "dictionary");
            _dict = dictionary;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                var array = new KeyValuePair<TKey, TValue>[_dict.Count];
                _dict.CopyTo(array, 0);
                return array;
            }
        }
    }

}