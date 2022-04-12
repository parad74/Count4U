using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	 [Serializable]
	public abstract class BaseProduct
	{
		protected abstract BaseProduct Create();
 
	}
}
