using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IInventProductSimpleParser
	{
		Dictionary<string, DocumentHeader> DocumentHeaderDictionary { get; }
		IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
			Dictionary<string, ProductMakat> productMakatDictionary,
			//Dictionary<string, ProductMakat> productBarcodeDictionary,
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null
		);
		//Dictionary<string, Session> SessionDictionary { get; }
		Dictionary<string, Itur> IturDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
