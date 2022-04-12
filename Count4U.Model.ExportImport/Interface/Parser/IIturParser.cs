using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IIturParser
	{
		Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, Itur> IturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);

		IEnumerable<Itur> GetItursEnumerable(string fromPathFile,
		  Encoding encoding, string[] separators,
		  int countExcludeFirstString,
		  Dictionary<string, Itur> IturFromDBDictionary,
		  Dictionary<ImportProviderParmEnum, object> parms = null);

		Dictionary<string, Itur> IturDictionary { get; }
		List<Location> LocationToDBList { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
