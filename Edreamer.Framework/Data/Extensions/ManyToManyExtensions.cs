using System;
using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Data.Extensions
{
    public static class ManyToManyExtensions
    {
        public static void SetCollection<T>(this ICollection<T> navigationProperty, IEnumerable<T> items, Func<T,T> addedItemModifier, Func<T,T> removedItemModifier)
        {
            SetCollection(navigationProperty, items, addedItemModifier, removedItemModifier, EqualityComparer<T>.Default);
        }

        public static void SetCollection<T>(this ICollection<T> navigationProperty, IEnumerable<T> items, Func<T, T> addedItemModifier, Func<T, T> removedItemModifier, Func<T, object> keyExtractor)
        {
            Throw.IfArgumentNull(keyExtractor, "keyExtractor");
            SetCollection(navigationProperty, items, addedItemModifier, removedItemModifier, new KeyEqualityComparer<T>(keyExtractor));
        }

        public static void SetCollection<T>(this ICollection<T> navigationProperty, IEnumerable<T> items, Func<T, T> addedItemModifier, Func<T, T> removedItemModifier, IEqualityComparer<T> comparer)
        {
            Throw.IfArgumentNull(navigationProperty, "navigationProperty");
            items = CollectionHelpers.EmptyIfNull(items);
            var itemsToAdd = items.Except(navigationProperty, comparer);
            var itemsToRemove = navigationProperty.Except(items, comparer);
            if (addedItemModifier != null)
            {
                itemsToAdd = itemsToAdd.Select(addedItemModifier);
            }
            navigationProperty.AddRange(itemsToAdd);
            if (removedItemModifier != null)
            {
                itemsToRemove = itemsToRemove.Select(removedItemModifier);
            }
            navigationProperty.RemoveRange(itemsToRemove);
        }
    }
}
