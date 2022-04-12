using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Products : ObservableCollection<Product>
    {
        public static Products FromEnumerable(IEnumerable<Product> list)
        {
            var collection = new Products();
            return Fill(collection, list);
        }

        private static Products Fill(Products collection, IEnumerable<Product> list)
        {
            foreach (Product item in list)
                collection.Add(item);
            return collection;
        }

        public Product CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
