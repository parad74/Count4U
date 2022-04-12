using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using ErikEJ.SqlCe;

namespace Count4U.Model.Interface
{
	public interface IImportShelfBlukRepository
	{
		//Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = false);
		void InsertShelves(string fromPathFile, string pathDB,
			ImportShelfParserEnum inventShelfParserEnum,
			Encoding encoding, string[] separators, 
			int countExcludeFirstString,  
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			List<string[]> columnMappings = null);
		void ClearShelves(string pathDB);
	}
}
