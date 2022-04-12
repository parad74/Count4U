using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.Threading;

namespace Count4U.Model.Interface
{
	public interface IImportPropertyStrRepository
	{
		void InsertPropertyStrs(string fromPathFile, string pathDB,
			PropertyStrParserEnum propertyStrParserEnum,
			DomainObjectTypeEnum  domainObjectType,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		void FromDictionaryToDB(string pathDB, Dictionary<string, PropertyStr> propertyStrToDBDictionary, 
			CancellationToken cancellationToken, Action<long> countAction);
		void ClearPropertyStrs(string pathDB);
		void ClearPropertyStrs(DomainObjectTypeEnum domainObject, string pathDB);
	}
}
