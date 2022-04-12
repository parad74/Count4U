using System;

namespace Count4U.Model.Interface.Audit
{
    /// <summary>
    /// Интерфейс предназначен для описания Аудитов, которые еще не переведены в БДб 
    /// как синхронизированные с общим репозиторием Аудитов (временное хранилище).
    /// Синхронизироваться с общим репозиторием Аудитов будет по полям <>Code. 
    /// Поля <>Code могут быть как фактическими из общего репозитория Аудитов
    /// так и фиктивные, которые при синхронизации проходят обработку и замену на фактические
    /// </summary>
	public interface IAuditConfig
    {
		/// <summary>
		/// ID
		/// </summary>
		long ID { get; set; }

		/// <summary>
		/// Ключ конфига 
		/// </summary>
		string Code { get; set; }

		/// <summary>
		/// StatusInventorConfigCode	 - текущий статус аудита
		/// </summary>
		string StatusInventorCode { get; set; }

		/// <summary>
		/// Код объекта Branch
		/// </summary>
		string BranchCode { get; set; }

		/// <summary>
		/// Имя объекта Branch
		/// </summary>
		string BranchName { get; set; }

		/// <summary>
		/// Код объекта Customer
		/// </summary>
		string CustomerCode { get; set; }

		/// <summary>
		/// Имя объекта Customer
		/// </summary>
		string CustomerName { get; set; }

		/// <summary>
		/// Код объекта Inventor
		/// </summary>
		string InventorCode { get; set; }

		/// <summary>
		/// Имя объета Inventor
		/// </summary>
		string InventorName { get; set; }

		/// <summary>
		/// Описание 
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Статус аудита TODO: {New, Process, Dirty, Save, Complet, Delete}
		/// </summary>
		string StatusInventor{ get; set; }

		/// <summary>
		/// Дата создания
		/// </summary>
		DateTime CreateDate { get; set; }

		/// <summary>
		/// Дата инветаризации
		/// </summary>
		DateTime InventorDate { get; set; }

		/// <summary>
		/// Не синхронизированно с центральной БД
		/// </summary>
		bool IsDirty { get; set; }

		/// <summary>
		///  Несинхронизированный с центральной БД код объекта Inventor
		/// </summary>
		string DirtyInventorCode { get; set; }

		/// <summary>
		/// Несинхронизированный с центральной БД код объекта Branch
		/// </summary>
		string DirtyBranchCode { get; set; }

		string StatusAuditConfig { get; set; }
						
		string DBPath { get; set; }

    }
}
