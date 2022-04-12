using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportPreviousInventorysSQLiteADORepository
	{
		void InsertPreviousInventorys(string fromPathFile, string pathDB,
			PreviousInventorySQLiteParserEnum previousInventoryParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		void ClearPreviousInventory(string pathDB);
		void VacuumPreviousInventory(string pathDB3);
	}
}
