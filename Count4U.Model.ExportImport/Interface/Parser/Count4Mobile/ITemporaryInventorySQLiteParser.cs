using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ITemporaryInventorySQLiteParser
	{
		IEnumerable<TemporaryInventory> GetTemporaryInventorys(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> temporaryInventoryFromDBDictionary,
 			Dictionary<ImportProviderParmEnum, object> parms = null);

		Dictionary<string, TemporaryInventory> TemporaryInventoryDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
