using System;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Audit
{
    public class StatusAuditConfigRepository : IStatusAuditConfigRepository
	{
		private StatusAuditConfigs _statusList;

        #region IStatusInventorConfigRepository Members

		public StatusAuditConfigs GetStatuses()
		{
			if (this._statusList == null)
			{
				this._statusList = new StatusAuditConfigs {
                    new StatusAuditConfig() { Code = StatusAuditConfigEnum.InProcess.ToString(), Name =  StatusAuditConfigEnum.InProcess.ToString() },
                    new StatusAuditConfig() { Code = StatusAuditConfigEnum.NotCurrent.ToString(), Name =  StatusAuditConfigEnum.NotCurrent.ToString()},
                };
			}
			return this._statusList;
		}

		public StatusAuditConfigs GetStatuses(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}


        #endregion
    }
}
