using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Count4U
{
    public class StatusInventorConfigRepository : IStatusInventorConfigRepository
	{
		private StatusInventorConfigs _list;

        #region IStatusInventorConfigRepository Members

		public StatusInventorConfigs GetStatuses()
		{
			if (this._list == null)
			{
				this._list = new StatusInventorConfigs {
                    new StatusInventorConfig() { ID = 1, Name = "StatusInventorConfig1", Description = "StatusInventorConfig1" },
                    new StatusInventorConfig() { ID = 2, Name = "StatusInventorConfig2", Description = "StatusInventorConfig2" },
                    new StatusInventorConfig() { ID = 3, Name = "StatusInventorConfig3", Description = "StatusInventorConfig3" },
                };
			}
			return this._list;
		}

		public StatusInventorConfigs GetStatuses(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}


        #endregion
    }
}
