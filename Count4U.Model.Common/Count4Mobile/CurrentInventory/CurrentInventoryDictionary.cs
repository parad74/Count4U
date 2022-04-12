using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Common;

namespace Count4U.Model.Count4Mobile
{
	public static class CurrentInventoryDictionary 
	{
		public static Dictionary<Pair<string, string, string>, CurrentInventory> QuantitySerialCurrentInventorys(this CurrentInventorys collection)
		{
			var dictionary = new Dictionary<Pair<string, string, string>, CurrentInventory>();
			foreach (CurrentInventory currentIventory in collection)
			{																									  //futureLocationCode, futureItemCode, serialNumber
				var futureKey = new Pair<string, string, string>(currentIventory.LocationCode, currentIventory.ItemCode, currentIventory.SerialNumberLocal);
				
			}
			//foreach (tblCurrentInventory currentIventory in tempCurrentInventories.Where(currentIventory => currentIventory.tblCatalog.ItemType == QuantityKey))
			//{
			//	var futureKey = new Pair<string, string, string>(currentIventory.tblLocation.LocationCode, currentIventory.tblCatalog.ItemCode, currentIventory.PropertyStr8);
			//	if (!dictionary3.ContainsKey(futureKey))
			//	{
			//		dictionary3.Add(futureKey, currentIventory);
			//	}
			//	else
			//	{
			//		SystemLog.Instance.WriteToLog(LogMessageType.Error,
			//			string.Format(
			//				"Error - duplicated Quantity current inventory <LocationCode,ItemCode, PropertyStr8> (<{0},{1},{2}>) on table CurrentInventory- please remove item from db",
			//				futureKey.First, futureKey.Second, futureKey.Third), GetType(),
			//			MethodBase.GetCurrentMethod().Name);
			//	}
			//}
			return dictionary;
		}


		//public static CurrentInventorys FromEnumerable(IEnumerable<CurrentInventory> list)
		//{
		//	var collection = new CurrentInventorys();
		//	return Fill(collection, list);
		//}

		//private static CurrentInventorys Fill(CurrentInventorys collection, IEnumerable<CurrentInventory> list)
		//{
		//	foreach (CurrentInventory item in list)
		//	{
		//		collection.Add(item);
		//	}
		//	return collection;
		//}

	
	}
}

