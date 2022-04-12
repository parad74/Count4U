using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Audit
{
     /// <summary>
    /// Интерфейс репозитория для доступа к Inventor объектам
    /// </summary>
    public interface IInventorRepository
	{
		string BuildRelativeDbPath(Inventor inventor);


		Inventors GetInventors();

		/// <summary>
		/// Получить список интвентаризаций по параметрами выборки
		/// </summary>
		/// <param name="selectParams"></param>
		/// <returns></returns>
		Inventors GetInventors(SelectParams selectParams);

	      /// <summary>
        /// Присвоить текущему списку  - список inventors
        /// </summary>
       void SetInventors(Inventors inventors);

		/// <summary>
		/// Получить Inventor по коду
		/// </summary>
		/// <param name="InventorID"></param>
		/// <returns></returns>
        Inventor GetInventorByCode(string сode);
		int GetInventorCountByBranchCode(string branchCode);

           
		/// <summary>
		///  Получить все Inventor проведенные в магазине
		///  </summary>
		/// <param name="documentHeaderName"></param>
		/// <returns></returns>
        Inventors GetInventorsByBranchCode(string branchCode);

        /// <summary>
        ///  Получить все Inventor проведенные у клиента
        ///  </summary>
        /// <param name="documentHeaderName"></param>
        /// <returns></returns>
        Inventors GetInventorsByCustomerCode(string customerCode);

		/// <summary>
		///   Получить все Inventor с заданным статусом
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
        Inventors GetInventorsByStatus(string status);

		/// <summary>
		/// Установить текущую инвентаризацию
		/// </summary>
		/// <param name="currentInventor"></param>
		void SetCurrent(Inventor currentInventor, AuditConfig auditConfig);

		/// <summary>
		/// Получить код  инвентаризации
		/// </summary>
		/// <returns></returns>
		string GetCurrentCode(AuditConfig auditConfig);

		/// <summary>
		/// Получить текущую инвентаризацию
		/// </summary>
		/// <returns></returns>
		Inventor GetCurrent(AuditConfig auditConfig);

		List<string> GetCodeList();
		List<string> GetInventorCodeListByCustomerCode(string customerCode);
		List<string> GetInventorCodeListByBranchCode(string branchCode);

		/// <summary>
		///  Получить все Inventor с заданным статусом
		/// </summary>
		/// <param name="statusID"></param>
		/// <returns></returns>
   //     Inventors GetInventorsByStatusID(long statusID);

		/// <summary>
		/// Установить с
		/// </summary>
		/// <param name="inventor"></param>
		/// <param name="statusCode"></param>
		/// <param name="statusName"></param>
		void SetStatus(Inventor inventor, string statusCode, string statusName);


		/// <summary>
		/// Клонировать Inventor по публичной информации 
		/// </summary>
		/// <param name="inventor"></param>
		/// <returns></returns>
		//Inventor Clone(Inventor inventor);

		void DeleteAllByBranchCode(string branchCode, bool full = true);
		void DeleteAllByCustomerCode(string customerCode, bool full = true);
		//string RemoveDB(string  dbPath, bool full = false);

		/// <summary>
		/// Удалить
		/// </summary>
		/// <param name="inventor"></param>
		void Delete(Inventor inventor, bool full = true);

		void Delete(string code, bool full = true);

         /// <summary>
         /// Вставить  inventor
         /// </summary>
         /// <param name="inventor"></param>
		void Insert(Inventor inventor, string inheritFromDBPath = null);
		void Insert(Inventors inventors);
		void InsertDomainInventorFromInventorConfig(Inventors inventors);

		//string CopyEmptyDB(string dbPath, string folder);
		//string CopyEmptyDB(string toFolderPath);
		IConnectionDB Connection { get; set; }
		

		/// <summary>
		/// Обновить inventor
		/// </summary>
		/// <param name="inventor"></param>
        void Update(Inventor inventor);
		void UpdateDomainInventorByInventorConfig(Inventors inventors);
		void RefillInventorConfigs(Inventor inventor);
		void RefillInventorConfigsAllInventorsInAuditDB();

		//string CopyEmptyDB(string dbPath);
	}
}
