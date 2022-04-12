using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к StatusDocHeader объектам
    /// </summary>
    public interface IStatusDocHeaderRepository
	{
        /// <summary>
        /// Получить все статусы DocHeader - документа
        /// </summary>
        /// <returns></returns>
		StatusDocHeaders GetStatuses(string pathDB);
	}
}
