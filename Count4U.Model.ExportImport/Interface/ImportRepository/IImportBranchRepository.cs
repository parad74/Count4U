using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.Threading;
using Count4U.Model.Main;

namespace Count4U.Model.Interface
{
	public interface IImportBranchRepository
	{
		void InsertBranchs(string fromPathFile, string pathDB, BranchParserEnum branchParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		//void FromDictionaryToDB(string pathDB, Dictionary<string, Branch> branchToDBDictionary, 
		//    CancellationToken cancellationToken, Action<long> countAction);
		void ClearBranchs(string pathDB);
	}
}
