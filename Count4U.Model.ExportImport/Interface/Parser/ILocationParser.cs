using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface ILocationParser
	{
		Dictionary<string, Location> GetLocations(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, Location> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, Location> LocationDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
