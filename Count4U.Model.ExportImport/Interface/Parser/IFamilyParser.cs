using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IFamilyParser
	{
		Dictionary<string, Family> GetFamilys(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, Family> FamilyFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, Family> FamilyDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
