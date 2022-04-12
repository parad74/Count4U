using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Interface
{
	public interface IImportCatalogProvider	
	{
		void InsertProducts(string fromPathFile, string pathDB, 
			Encoding encoding, string[] separators, int countExcludeFirstString);
		void ClearProducts(string pathDB);
		void ClearSupplier(string pathDB);
	}
}
