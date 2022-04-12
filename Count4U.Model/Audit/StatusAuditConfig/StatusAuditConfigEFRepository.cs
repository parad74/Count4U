using System;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit.Mapping;

namespace Count4U.Model.Audit
{
    public class StatusAuditConfigEFRepository : IStatusAuditConfigRepository
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

		#region private

		//private App_Data.StatusAuditConfig GetEntityByCode(App_Data.AuditDB dc, string code)
		//{
		//    var entity = dc.StatusAuditConfig.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
		//    return entity;
		//}

		#endregion
    }
}
