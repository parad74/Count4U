using System;

namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Магазин
    /// </summary>
    public interface IBranch : IRequisite
    {
           /// <summary>
        /// Код клиента
        /// </summary>
		string CustomerCode { get; set; }
     }
}
