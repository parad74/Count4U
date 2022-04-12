using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportSupplierRepository
	{
		void InsertSuppliers(string fromPathFile, string pathDB, SupplierParserEnum supplierParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		void ClearSuppliers(string pathDB);
	}
}
