using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using ErikEJ.SqlCe;

namespace Count4U.Model.Interface
{
	public interface IImportStatusInventProductBlukRepository
	{
		void InsertStatusInventProducts(string fromPathFile, string pathDB,
			StatusInventProductSimpleParserEnum inventProductSimpleParserEnum,
			Encoding encoding, string[] separators, 
			int countExcludeFirstString,  
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			List<string[]> ColumnMappings = null);
		void ClearStatusInventProducts(string pathDB);

	}
}
