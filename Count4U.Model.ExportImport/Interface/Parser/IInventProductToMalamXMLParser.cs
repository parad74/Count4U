using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IInventProductToObjectParser
	{
		Dictionary<string, DocumentHeader> DocumentHeaderDictionary { get; }
		object GetMyObject(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, 
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null
		);
		List<BitAndRecord> ErrorBitList { get; }
	}
}
