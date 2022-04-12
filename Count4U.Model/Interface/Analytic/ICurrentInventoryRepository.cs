using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ICurrentInventoryRepository
	{
		CurrentInventorys GetCurrentInventorys(string pathDB);
		CurrentInventorys GetCurrentInventorys(SelectParams selectParams, string pathDB);
		void Insert(CurrentInventorys currentInventorys, string pathDB);
		void Update(CurrentInventory currentInventory, string pathDB);
		void DeleteAll(string pathDB);
    }
}
