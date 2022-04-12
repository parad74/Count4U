using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IShelfParser
	{
		//int CountExcludeFirstString { get; set; }
		Dictionary<string, Itur> IturDictionary { get; }
		//string FromPathFile { get; set; }
		IEnumerable<Shelf> GetShelfs(
			string fromPathFile, Encoding encoding, string[] separators,
			int countExcludeFirstString, 
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			string iturCodeIn = "", string shelfCodeIn = "");
		Dictionary<string, Supplier> SupplierDictionary { get; }
	}
}
