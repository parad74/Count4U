using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IImportCatalogSQLiteADORepository
	{											 				
		void InsertCatalogs(string fromPathFile,   //GetDbPath		Count4U
			string toPathDB3,									//db3Path		  sqlite
			CatalogSQLiteParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null	
			);
		Catalog GetCatalogMobileByItemCode(string itemCode, string pathDB);
		Dictionary<string, Catalog> GetCatalogMobileDictionary(Encoding encoding, string pathDB);
		void ClearCatalogs(string pathDB3);
		void DropCatalogIndexTable(string pathDB3);
		void VacuumCatalogs(string pathDB3);

		void CreateCatalogIndexTable(string pathDB3);
	
	}
}
