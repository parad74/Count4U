using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Count4U.Model
{
    /// <summary>
    /// Коллекция сущностей предметной области.
    /// </summary>
    /// <typeparam name="T">Тип сущности предметной области.</typeparam>
    public class CollectionOf<T> : ObservableCollection<T>
    {
        public static CollectionOf<T> FromEnumerable(IEnumerable<T> list)
        {
            var collection = new CollectionOf<T>();
            return Fill(collection, list);
        }

        protected static CollectionOf<T> Fill(CollectionOf<T> collection, IEnumerable<T> list)
        {
            foreach (T item in list)
                collection.Add(item);
            return collection;
        }

        public T CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }
    }
}
