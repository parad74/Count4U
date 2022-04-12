using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class InputTypeRepository : IInputTypeRepository
	{
        private InputTypes _list;

        #region IInputTypeRepository Members

		public InputTypes GetInputTypes(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new InputTypes {
                    new InputType() { ID = 1, Name = "InputType1", Description = "InputType1" },
                    new InputType() { ID = 2, Name = "InputType2", Description = "InputType2" },
                };
            }
            return this._list;
        }

        #endregion
    }
}
