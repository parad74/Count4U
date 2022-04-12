using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class ProductSimples : ObservableCollection<ProductSimple>
    {
		public static ProductSimples FromEnumerable(IEnumerable<ProductSimple> list)
        {
			var collection = new ProductSimples();
            return Fill(collection, list);
        }

		private static ProductSimples Fill(ProductSimples collection, IEnumerable<ProductSimple> list)
        {
			foreach (ProductSimple item in list)
                collection.Add(item);
            return collection;
        }

		public ProductSimple CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
