using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ITemplateInventoryRepository
	{
		TemplateInventorys GetTemplateInventorys(SelectParams selectParams, string pathDB);
		TemplateInventorys GetTemplateInventorys(string pathDB);
		//TemplateInventory GetPortByCode(string portCode);
		//List<string> GetCodeList();
		//void Delete(TemplateInventory port, bool full = true);
		void DeleteAll(string pathDB);
		void Insert(TemplateInventorys templateInventorys, string pathDB);
		void Update(TemplateInventory templateInventory, string pathDB);
    }
}
