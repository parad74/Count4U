using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	public interface ICurrentInventoryAdvancedReader
	{
		IEnumerable<CurrentInventoryAdvanced> GetCurrentInventoryAdvanceds(string pathDB,
		bool refill = true,
		bool refillCatalogDictionary = false,
		SelectParams selectParms = null,
		Dictionary<object, object> parmsIn = null,
		List<ImportDomainEnum> importType = null);
	}
}
