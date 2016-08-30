using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Edreamer.Framework.Helpers
{
    public static class CollectionHelpers
    {
        public static IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> value)
        {
            return value ?? Enumerable.Empty<T>();
        }

        public static IDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            return value ?? new Dictionary<TKey, TValue>();
        }

        public static bool IsNullOrEmpty(IEnumerable value)
        {
            return value == null || !value.Cast<object>().Any();
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> CombineKeysAndValues<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            Throw.IfArgumentNull(keys, "keys");
            Throw.IfArgumentNull(values, "values");
            Throw.If(keys.Count() != values.Count()).AnArgumentException("Keys and values should have the same size.");

            var keysList = new List<TKey>(keys);
            var valuesList = new List<TValue>(values);
            var result = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keysList.Count; i++)
            {
                result.Add(keysList[i], valuesList[i]);
            }
            return result;
        }

        public static IDictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }
            var mergedDictionary = new Dictionary<TKey, TValue>(first);
            foreach (var pair in second)
            {
                mergedDictionary[pair.Key] = pair.Value;
            }
            return mergedDictionary;
        }
    }
}
