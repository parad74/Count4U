using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface ISectionParser
	{
		Dictionary<string, Section> GetSections(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, Section> SectionFromDBDictionary,
		Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, Section> SectionDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
