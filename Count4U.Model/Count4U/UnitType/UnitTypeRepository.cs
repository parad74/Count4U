using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class UnitTypeRepository : IUnitTypeRepository
	{
        private UnitTypes _list;

        #region IUnitTypeRepository Members

		public UnitTypes GetUnitTypes(string pathDB)
		{
            if (this._list == null)
            {
                this._list = new UnitTypes {
                    new UnitType() { ID = 1, Name = "UnitType1", Description = "UnitType1" },
                    new UnitType() { ID = 2, Name = "UnitType2", Description = "UnitType2" },
                };
            }
            return this._list;
		}

		#endregion
	}
}
