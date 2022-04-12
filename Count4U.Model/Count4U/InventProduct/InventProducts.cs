using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.MappingEF;

namespace Count4U.Model.Count4U
{
    public class InventProducts : ObservableCollection<InventProduct>
    {
        public static InventProducts FromEnumerable(IEnumerable<InventProduct> list)
        {
            var collection = new InventProducts();
            return Fill(collection, list);
        }

        private static InventProducts Fill(InventProducts collection, IEnumerable<InventProduct> list)
        {
            foreach (InventProduct item in list)
                collection.Add(item);
            return collection;
        }

		public static InventProducts FromEntityList(List<App_Data.InventProduct> list)
		{
			var collection = new InventProducts();
			foreach (var item in list) collection.Add(item.ToDomainObject());
			return collection;
		}

        public InventProduct CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }

		public long TotalItur { get; internal set; }

		public double SumQuantityEdit { get; set; }

		public InventProducts()
		{
			this.TotalCount = 0;
			this.TotalItur = 0;
			this.SumQuantityEdit = 0;
		}

    }
}
