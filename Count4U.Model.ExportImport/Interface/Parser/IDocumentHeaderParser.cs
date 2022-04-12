using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IDocumentHeaderParser
	{
		Dictionary<string, DocumentHeader> GetDocumentHeaders(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, DocumentHeader> documentHeaderFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, DocumentHeader> DocumentHeaderDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
