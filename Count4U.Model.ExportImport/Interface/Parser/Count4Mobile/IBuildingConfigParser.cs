using System;
using System.Collections.Generic;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface.Count4Mobile
{
	public interface IBuildingConfigParser
	{
		Dictionary<string, BuildingConfig> GetBuildingConfigs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
		Dictionary<string, string> buildingConfigFromDBDictionary,
			DomainObjectTypeEnum domainObjectType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, BuildingConfig> BuildingConfigDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
