using System;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface.Audit
{
    /// <summary>
	/// Интерфейс репозитория для доступа к StatusInventor объектам
    /// </summary>
	public interface IStatusInventorRepository
	{
        /// <summary>
        /// Получить все статусы Inventor
        /// </summary>
        /// <returns></returns>
		StatusInventors GetStatuses();

		/// <summary>
		/// Получить статус по имени
		/// </summary>
		/// <returns></returns>
		StatusInventor GetStatusByName(string name);

		/// <summary>
		/// Получить статус по коду
		/// </summary>
		/// <returns></returns>
		StatusInventor GetStatusByCode(string statusCode);

		/// <summary>
		/// Создать неопределенный статус - None
		/// </summary>
		/// <returns></returns>
		StatusInventor CreateNoneStatusInventor();

	}
}
