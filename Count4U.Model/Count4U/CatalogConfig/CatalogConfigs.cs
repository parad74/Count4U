using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
	public class CatalogConfigs : ObservableCollection<CatalogConfig>
	{

		public static CatalogConfigs FromEnumerable(System.Collections.Generic.IEnumerable<CatalogConfig> List)
		{
			CatalogConfigs catalogs = new CatalogConfigs();
			foreach (CatalogConfig item in List)
			{
				catalogs.Add(item);
			}
			return catalogs;
		}

		public CatalogConfig CurrentItem { get; set; }

		public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}
