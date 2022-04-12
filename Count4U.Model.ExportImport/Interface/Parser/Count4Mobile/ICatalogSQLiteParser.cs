using System;
using System.Collections.Generic;
//using Count4U.Model.Count4U;
using System.Text;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface ICatalogSQLiteParser
	{
		//IEnumerable<ProductSimple> GetProducts(string fromPathFile,
		IEnumerable<Catalog> GetCatalogs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> productMakatDBDictionary,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, Catalog> CatalogDictionary { get; }
		//		Dictionary<string, Supplier> SupplierDictionary { get; }
		//		Dictionary<string, Catalog> ProductParentMakatDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
		//List<ProductSimple> ProductSimpleList { get;  }
	}
}
