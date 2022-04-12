using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IImportBuildingConfigADORepository
	{
		void InsertBuildingConfig(string fromPathFile, string pathDB,
			BuildingConfigParserEnum buildingConfigParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		void ClearBuildingConfig(string pathDB);
	}
}
