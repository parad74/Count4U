using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Audit
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Аудитам во временном хранилище
    /// </summary>
	public interface IAuditConfigRepository 
	{	
		/// <summary>
		/// Получить объект текущей инвентаризации
		/// </summary>
		/// <returns></returns>
		AuditConfig GetCurrentConfig(CBIContext contextCBI, bool fromDB=false);

		/// <summary>
		/// Установить объект текущей инвентаризации
		/// </summary>
		/// <returns></returns>
		void SetCurrentConfig(CBIContext contextCBI, AuditConfig auditConfig);

		/// <summary>
		/// Получить рабочую инвентаризацию
		/// </summary>
		/// <returns></returns>
		AuditConfig GetProcessConfig(CBIContext contextCBI);

		bool IsExistProcessConfig();

		/// <summary>
		/// Установить  рабочую инвентаризацию
		/// </summary>
		/// <returns></returns>
		void SetProcessConfig(CBIContext contextCBI, AuditConfig auditConfig);

	 
		/// <summary>
		/// Список всех аудитов
		/// </summary>
		/// <returns></returns>
		AuditConfigs GetAuditConfigs(CBIContext contextCBI);

		/// <summary>
		/// Список последних аудитов по времени проведения
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		AuditConfigs GetAuditConfigsLast(int count, CBIContext contextCBI);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		Customers GetHistoryCustomerLast();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="customerCode"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		Branches GetHistoryBranchForCustomerLast(string customerCode, int count);

		/// <summary>
		/// Список последних аудитов для Customer по времени проведения
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		 AuditConfigs GetAuditConfigsByCustomerCodeLast(int count, string customerCode,
			CBIContext contextCBI);

		 /// <summary>
		 /// Список последних аудитов для Branch по времени проведения
		 /// </summary>
		 /// <param name="count"></param>
		 /// <returns></returns>
		 AuditConfigs GetAuditConfigsByBranchCodeLast(int count, string branchCode,
			CBIContext contextCBI);

		/// <summary>
		/// Аудит по коду  зарегестрированному в общем репозитории 
		/// </summary>
		/// <param name="Code"></param>
		/// <returns></returns>
		AuditConfig GetAuditConfigByInventorCode(string inventorCode, CBIContext contextCBI);

		/// <summary>
		/// Аудиты клиента
		/// </summary>
		/// <param name="CustomerCode"></param>
		/// <returns></returns>
		AuditConfigs GetAuditConfigsByCustomerCode(string customerCode, CBIContext contextCBI);

		/// <summary>
		/// Аудиты магазина
		/// </summary>
		/// <param name="CustomerCode"></param>
		/// <returns></returns>
		AuditConfigs GetAuditConfigsByBranchCode(string branchCode, CBIContext contextCBI);

		Inventors GetInventors() ;
		List<string> GetInventorCodeList();
		/// <summary>
		/// Создать конфигурацию аудита на основе существующей
		/// </summary>
		/// <param name="inventorConfig"></param>
		/// <returns></returns>
		AuditConfig Clone(AuditConfig inventorConfig);

		void DeleteByInventorCode(string inventorCode, CBIContext contextCBI);
		void DeleteAllByBranchCode(string branchCode, CBIContext contextCBI);
		void DeleteAllByCustomerCode(string customerCode, CBIContext contextCBI);


		/// <summary>
		/// Удалить
		/// </summary>
		/// <param name="inventorConfig"></param>
		void Delete(AuditConfig inventorConfig, CBIContext contextCBI);

		/// <summary>
		/// Вставить
		/// </summary>
		/// <param name="inventorConfig"></param>
		void Insert(AuditConfig inventorConfig, CBIContext contextCBI);
		void Insert(AuditConfigs auditConfigs);
		void InsertDomainAuditConfigFromInventorConfig(AuditConfigs auditConfigs);

		AuditConfigs UpdateBranchNameByCode(string branchCode, string name, CBIContext contextCBI);
		AuditConfigs UpdateCustomerNameByCode(string customerCode, string name, CBIContext contextCBI);
		

		/// <summary>
		/// Обновить
		/// </summary>
		/// <param name="inventorConfig"></param>
		void Update(AuditConfig inventorConfig, CBIContext contextCBI);

		/// <summary>
		/// Получить список допустимых статусов инвентаризации
		/// </summary>
		/// <returns></returns>
		StatusAuditConfigs GetConfigStatus(CBIContext contextCBI);

		/// <summary>
		/// Установить статус текущей инвентаризации
		/// </summary>
		/// <param name="statusInventor"></param>
		void SetAuditStatus(AuditConfig auditConfig, StatusInventor statusInventor, CBIContext contextCBI);


		/// <summary>
		/// Получить статус текущей инвентаризации
		/// </summary>
		StatusInventor GetAuditStatus(CBIContext contextCBI);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <param name="contextCBI"></param>
		/// <returns></returns>
		AuditConfig GetAuditConfigByCode(string code, CBIContext contextCBI);
	}
}
