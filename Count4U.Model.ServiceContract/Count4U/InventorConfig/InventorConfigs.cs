using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
	public class InventorConfigs : ObservableCollection<InventorConfig>
	{

		public static InventorConfigs FromEnumerable(System.Collections.Generic.IEnumerable<InventorConfig> List)
		{
			InventorConfigs inventors = new InventorConfigs();
			foreach (InventorConfig item in List)
			{
				inventors.Add(item);
			}
			return inventors;
		}

		public static Dictionary<string, InventorConfig> FromEnumerableToDictionary(System.Collections.Generic.IEnumerable<InventorConfig> List)
		{
			Dictionary<string, InventorConfig> inventorConfigDictionary = new Dictionary<string, InventorConfig>();
			foreach (InventorConfig item in List)
			{
				inventorConfigDictionary[item.Code] = item;
			}
			return inventorConfigDictionary;
		}

		public InventorConfig CurrentItem { get; set; }

		public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}
