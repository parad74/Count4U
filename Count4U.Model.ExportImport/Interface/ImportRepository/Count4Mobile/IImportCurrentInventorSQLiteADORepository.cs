using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportCurrentInventorSQLiteADORepository
	{
		void InsertCurrentInventors(string fromPathFile,   //GetDbPath		Count4U
			string toPathDB3,									//db3Path		  sqlite
			CurrentInventorSQLiteParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null	
			);

		void ClearCurrentInventors(string pathDB);
		void VacuumCurrentInventory(string pathDB3);
	
	}
}
