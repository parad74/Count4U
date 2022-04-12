using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class Catalogs : ObservableCollection<Catalog>
	{
		public static Catalogs FromEnumerable(IEnumerable<Catalog> list)
		{
			var collection = new Catalogs();
			return Fill(collection, list);
		}

		private static Catalogs Fill(Catalogs collection, IEnumerable<Catalog> list)
		{
			foreach (Catalog item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public Catalog CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

