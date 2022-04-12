using System;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface.Audit
{
    /// <summary>
	/// Интерфейс репозитория для доступа к StatusAuditConfig объектам
    /// </summary>
	public interface IStatusAuditConfigRepository
	{
       /// <summary>
		/// Получить все статусы AuditConfig - аудита
       /// </summary>
       /// <returns></returns>
		StatusAuditConfigs GetStatuses();
	}
}
