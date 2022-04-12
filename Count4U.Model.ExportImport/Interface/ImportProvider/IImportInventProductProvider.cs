using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface
{
	public interface IImportInventProductProvider //: IImportLog
	{
		//Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB, bool refill = false);
		void InsertInventProducts(string fromPathFile, string pathDB, 
			Encoding encoding, string[] separators, 
			int countExcludeFirstString,  
			 List<ImportDomainEnum> importType);
		void InsertDocumentHeaders(string pathDB, string fromPathFile,
			Encoding encoding, string[] separators, 
			int countExcludeFirstString);
		void ClearInventProducts(string pathDB);
		void ClearDocumentHeaders(string pathDB);
		void ClearSession(string pathDB);
	}
}
