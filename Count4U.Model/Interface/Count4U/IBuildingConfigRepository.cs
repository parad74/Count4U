using System;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    public interface IBuildingConfigRepository
	{
		BuildingConfigs GetBuildingConfigs(string pathDB);
		Dictionary<string, BuildingConfig> GetBuildingConfigDictionary(string pathDB, bool refill = true);
		
	}
}
