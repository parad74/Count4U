using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportInventProductRepository
	{
		//Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = false);
		void InsertInventProducts(string fromPathFile, string pathDB,
			InventProductSimpleParserEnum inventProductSimpleParserEnum,
			Encoding encoding, string[] separators, 
			int countExcludeFirstString,  
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		void ClearInventProducts(string pathDB);
		void ClearDocumentHeaders(string pathDB);
		void ClearSession(string pathDB);
		void ClearItur(string pathDB);
		void FillLogFromErrorBitList(List<BitAndRecord> errorBitList);
	}
}
