using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface IPropertyStrListSQLiteParser
	{
		Dictionary<string, PropertyStr1> GetPropertyStrList(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> propertyStrFromDBDictionary,
			DomainObjectTypeEnum domainObjectType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		//Dictionary<string, PropertyStr1> PropertyStrDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
