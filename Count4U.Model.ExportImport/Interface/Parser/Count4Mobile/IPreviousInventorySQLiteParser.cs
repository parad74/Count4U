using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface IPreviousInventorySQLiteParser
	{
		IEnumerable<PreviousInventory> GetPreviousInventory(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PreviousInventory> previousInventoryFromDBDictionary,
			//Dictionary<string, string> catalogFromDBDictionary,
			//Dictionary<string, string> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, PreviousInventory> PreviousInventoryDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
