using System;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;
using System.Text;
using Count4U.Model;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
	public interface IProductSimpleParser
	{
		//IEnumerable<ProductSimple> GetProducts(string fromPathFile,
		IEnumerable<Product> GetProducts(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, ProductMakat> productMakatDBDictionary,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, ProductSimple> ProductDictionary { get; }
		Dictionary<string, Supplier> SupplierDictionary { get; }
		Dictionary<string, ProductMakat> ProductParentMakatDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
		//List<ProductSimple> ProductSimpleList { get;  }
	}
}
