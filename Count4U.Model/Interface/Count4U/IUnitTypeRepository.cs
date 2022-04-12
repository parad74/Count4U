using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к UnitType объектам (единицы измерения)
    /// </summary>
    public interface IUnitTypeRepository
	{
        /// <summary>
        /// Получить весь список
        /// </summary>
        /// <returns></returns>
		UnitTypes GetUnitTypes(string pathDB);
	}
}
