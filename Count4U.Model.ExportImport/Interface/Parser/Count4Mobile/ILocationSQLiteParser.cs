using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ILocationSQLiteParser
	{
		Dictionary<string, LocationMobile> GetLocationMobiles(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, string> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, LocationMobile> LocationDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
