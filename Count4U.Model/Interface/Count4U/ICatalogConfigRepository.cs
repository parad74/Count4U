using System;
using Count4U.Model.Count4U;
using Count4U.Model.Audit;
using Count4U.Model.Main;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к InventorConfig объектам
    /// </summary>
	public interface ICatalogConfigRepository
	{
		void ClearCatalogConfigs(string pathDB);
		CatalogConfig GetCurrentCatalogConfig(string pathDB);
		void Insert(AuditConfig auditConfig, string pathDB);
		void Insert(CatalogConfig catalogConfig, string pathDB);
		void SetCurrentCatalogConfig(CatalogConfig catalogConfig, string pathDB);
		void SetCurrentCatalogConfig(AuditConfig auditConfig, string pathDB);
		void Update(CatalogConfig catalogConfig, string pathDB);

	}
}
