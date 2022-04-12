using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class StatusDocHeaderRepository : IStatusDocHeaderRepository
	{
        private StatusDocHeaders _list;

        #region IStatusDocHeaderRepository Members

		public StatusDocHeaders GetStatuses(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new StatusDocHeaders {
                    new StatusDocHeader() { ID = 1, Name = "StatusDocHeader1", Description = "StatusDocHeader1" },
                    new StatusDocHeader() { ID = 2, Name = "StatusDocHeader2", Description = "StatusDocHeader2" },
                };
            }
            return this._list;
        }

        #endregion
    }
}
