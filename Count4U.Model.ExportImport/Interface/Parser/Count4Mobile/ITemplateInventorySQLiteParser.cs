using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ITemplateInventorySQLiteParser
	{
		IEnumerable<TemplateInventory> GetTemplateInventorys(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> templateInventoryFromDBDictionary,
 			Dictionary<ImportProviderParmEnum, object> parms = null);

		Dictionary<string, TemplateInventory> TemplateInventoryDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
