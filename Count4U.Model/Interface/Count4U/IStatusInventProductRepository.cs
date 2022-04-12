using System;
using Count4U.Model.Count4U;
using System.Collections;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
	/// Интерфейс репозитория для доступа к StatusInventProduct объектам
    /// </summary>
    public interface IStatusInventProductRepository
	{
        /// <summary>
		/// Получить все статусы InventProduct
        /// </summary>
        /// <returns></returns>
		StatusInventProducts GetStatuses(string pathDB);
		Dictionary<string, StatusInventProduct> GetStatusInventProductSumBitDictionary(string pathDB);

		//BitArray GetStatusBitArray(string pathDB);

		//List<BitArray> GetStatusBitArrayList(string pathDB);
	}
}
