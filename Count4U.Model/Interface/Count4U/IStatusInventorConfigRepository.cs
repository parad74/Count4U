using System;
using Count4U.Model.SelectionParams; 

namespace Count4U.Model.Count4U
{
	public interface IStatusInventorConfigRepository
	{
		StatusInventorConfigs GetStatuses();
		StatusInventorConfigs GetStatuses(SelectParams selectParams);
	}
}
