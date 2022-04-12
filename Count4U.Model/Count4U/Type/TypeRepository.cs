using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class TypeRepository : ITypeRepository
	{
        private Types _list;

        #region ITypeRepository Members

		public Types GetTypes(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new Types {
                    new Type() { ID = 1, Name = "Type1", Description = "Type1" },
                    new Type() { ID = 2, Name = "Type2", Description = "Type2" },
                };
            }
            return this._list;
        }

        #endregion
    }
}
