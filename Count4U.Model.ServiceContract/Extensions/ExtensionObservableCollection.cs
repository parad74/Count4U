using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Count4U.Model.Extensions
{
    public static class ExtensionObservableCollection
    {
        public static ObservableCollection<T> RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            var itemsToRemove = collection.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                collection.Remove(itemToRemove);
            }

            return collection;
        }
    }
}
