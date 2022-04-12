using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface ISupplierParser
	{
		Dictionary<string, Supplier> GetSuppliers(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, Supplier> SupplierFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, Supplier> SupplierDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
