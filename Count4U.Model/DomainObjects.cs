using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Count4U.Model
{
    public class DomainObjects<T> : ObservableCollection<T>
    {
        public static DomainObjects<T> FromEnumerable(IEnumerable<T> list)
        {
            var collection = new DomainObjects<T>();
            return Fill(collection, list);
        }

        public static DomainObjects<T> FromEnumerable(IEnumerable<T> list, long totalCount)
        {
            var collection = new DomainObjects<T>(totalCount);
            return Fill(collection, list);
        }

        private static DomainObjects<T> Fill(DomainObjects<T> collection, IEnumerable<T> list)
        {
            foreach (T item in list)
                collection.Add(item);
            return collection;
        }

        public DomainObjects()
        {

        }

        public DomainObjects(long totalCount)
        {
            TotalCount = totalCount;
        }

        public T CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; private set; }
    }

	
}
