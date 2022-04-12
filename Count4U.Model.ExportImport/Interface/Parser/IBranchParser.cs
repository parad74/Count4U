using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;
using Count4U.Model.Main;

namespace Count4U.Model.Interface.Count4U
{
	public interface IBranchParser
	{
		Dictionary<string, Branch> GetBranchs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Branch> branchFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);

		Dictionary<string, Branch> BranchDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
