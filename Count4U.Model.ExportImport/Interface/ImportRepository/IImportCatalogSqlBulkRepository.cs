using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportCatalogSqlBulkRepository
	{
		void CopyCatalog(string fromPathDB, string toPathDB,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);

		void ClearProduct(string pathDB);
		void ClearSupplier(string pathDB);
	}
}
