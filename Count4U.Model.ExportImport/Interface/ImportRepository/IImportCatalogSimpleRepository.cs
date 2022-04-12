using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportCatalogADORepository
	{
		void InsertProducts(string fromPathFile, string pathDB, 
			ProductSimpleParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null	
			);

		void ClearProducts(string pathDB);
		void ClearProductsMakatOnly(string pathDB);
		void ClearSupplier(string pathDB);
	}
}
