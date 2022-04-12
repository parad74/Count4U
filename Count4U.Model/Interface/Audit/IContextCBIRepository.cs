using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Audit
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Аудитам во временном хранилище
    /// </summary>
    public interface IContextCBIRepository
    {
          AuditConfig NewCBIConfig(string newCode);

        /// <summary>
        /// Получить список магазинов по параметрам запроса
        /// </summary>
        /// <returns></returns>
        Branches GetBranches(SelectParams selectParams);

        /// <summary>
        /// Получить список инвентаризаций по параметрам запроса
        /// </summary>
        /// <returns></returns>
        Inventors GetInventors(SelectParams selectParams);

        /// <summary>
        /// Получить объект текущей инвентаризации
        /// </summary>
        /// <returns></returns>
        //AuditConfig GetCurrentCBIConfig(CBIContext contextCBI);
        AuditConfig GetCurrentCBIConfig(CBIContext contextCBII, bool fromDB = false);


        /// <summary>
        /// Получить рабочую инвентаризации
        /// </summary>
        /// <returns></returns>
        AuditConfig GetProcessCBIConfig(CBIContext contextCBI);


        /// <summary>
        /// Получить клиента текущей инвентаризации
        /// </summary>
        /// <returns></returns>
        Customer GetCurrentCustomer(AuditConfig auditConfig);

        /// <summary>
        /// Получить магазин инвентаризации
        /// </summary>
        /// <returns></returns>
        Branch GetCurrentBranch(AuditConfig auditConfig);

        /// <summary>
        /// Получить инвентаризацию
        /// </summary>
        /// <returns></returns>
        Inventor GetCurrentInventor(AuditConfig auditConfig);

        /// <summary>
        /// Получить код клиента инвентаризации
        /// </summary>
        /// <returns></returns>
        string GetCodeCurrentCustomer(AuditConfig auditConfig);

        /// <summary>
        /// Получить код магазина нвентаризации
        /// </summary>
        /// <returns></returns>
        string GetCodeCurrentBranch(AuditConfig auditConfig);

        /// <summary>
        /// Получить текущую инвентаризацию
        /// </summary>
        /// <returns></returns>
        string GetCodeCurrentInventor(AuditConfig auditConfig);

        /// <summary>
        /// Получить клиента  по коду
        /// </summary>
        /// <returns></returns>
        Customer GetCustomerByCode(string code);

        /// <summary>
        /// Получить магазин по коду
        /// </summary>
        /// <returns></returns>
        Branch GetBranchByCode(string code);

        /// <summary>
        /// Получить инвентаризацию по коду
        /// </summary>
        /// <returns></returns>
        Inventor GetInventorByCode(string code);

        /// <summary>
        /// Существует ли в БД клиент с таким кодом
        /// </summary>
        /// <returns></returns>
        bool IsExistsCustomerCode(string code);

        /// <summary>
        /// Существует ли в БД клиент с таким кодом
        /// </summary>
        /// <returns></returns>
        bool IsExistsBranchCode(string code);

        /// <summary>
        /// Существует ли в БД клиент с таким кодом
        /// </summary>
        /// <returns></returns>
        bool IsExistsInventorCode(string code);

        /// <summary>
        /// Установить клиента с заданным кодом, как текущего
        /// </summary>
        /// <returns></returns>
        void SetCurrentCustomer(Customer сustomer, AuditConfig auditConfig);

        /// <summary>
        /// Установить магазин с заданным кодом, как текущий
        /// </summary>
        /// <returns></returns>
        void SetCurrentBranch(Branch branch, AuditConfig auditConfig);

        /// <summary>
        /// Установить аудит с заданным кодом, как текущий 
        /// </summary>
        /// <returns></returns>
        void SetCurrentInventor(Inventor inventor, AuditConfig auditConfig);

        /// <summary>
        /// Получить клиентов текущей инвентаризации,
        /// список зависящий от контекста 
        /// </summary>
        /// <returns></returns>
        Customers GetContextCustomers(AuditConfig auditConfig, CBIContext contextCBI);

        /// <summary>
        /// Получить магазины текущей инвентаризации
        /// список зависящий от контекста (от текущего Customer)
        /// </summary>
        /// <returns></returns>
        Branches GetContextBranches(AuditConfig auditConfig, CBIContext contextCBI);

        /// <summary>
        /// Получить текущие инвентаризации
        /// писок зависящий от контекста (от текущего Customer, и текущего магазина)
        /// </summary>
        /// <returns></returns>
        Inventors GetContextInventors(AuditConfig auditConfig);

        /// <summary>
        /// Получить клиентов текущей инвентаризации,
        /// список зависящий от контекста
        /// с дополнительными параметрами отбора
        /// </summary>
        /// <returns></returns>
        Customers GetContextCustomers(SelectParams selectParams, AuditConfig auditConfig);

        /// <summary>
        /// Получить магазины текущей инвентаризации
        /// список зависящий от контекста (от текущего Customer)
        /// с дополнительными параметрами отбора
        /// </summary>
        /// <returns></returns>
        Branches GetContextBranches(SelectParams selectParams, AuditConfig auditConfig);

        /// <summary>
        /// Получить текущие инвентаризации
        /// писок зависящий от контекста (от текущего Customer, и текущего магазина)
        /// с дополнительными параметрами отбора
        /// </summary>
        /// <returns></returns>
        Inventors GetContextInventors(SelectParams selectParams, AuditConfig auditConfig);

        /// <summary>
        /// Создать клиента в текущем контексте
        /// </summary>
        /// <param name="toDoCurrent">сделать текущим</param>
        /// <returns></returns>
        void CreateContextCustomer(Customer customer, AuditConfig auditConfig, bool toDoCurrent);

        /// <summary>
        /// Создать магазин в текущем контексте (для текущего клиента)
        /// </summary>
        /// <param name="toDoCurrent">сделать текущим</param>
        /// <returns></returns>
        void CreateContextBranch(Branch branch, AuditConfig auditConfig, bool toDoCurrent, bool inheritDataFromCustomer = false);

        /// <summary>
        /// Создать инвентаризацию для текущего клиента и текущего магазина
        /// </summary>
        /// <param name="toDoCurrent">сделать текущим</param>
        /// <returns></returns>
        void CreateContextInventor(Inventor inventor, AuditConfig auditConfig, bool toDoCurrent, object domainObjectCopyDb = null);


        /// <summary>
        /// Клонировать объект
        /// </summary>
        /// <returns></returns>
		//Customer Clone(Customer сustomer);

        /// <summary>
        /// Удалить
        /// </summary>
        void Delete(Customer сustomer, bool full = true);

        /// <summary>
        /// Удалить
        /// </summary>
        void Delete(Branch branch, bool full = true);

        /// <summary>
        /// Удалить
        /// </summary>
        void Delete(AuditConfig auditConfig, CBIContext contextCBI);

        /// <summary>
        /// Удалить
        /// </summary>
        void Delete(Inventor inventor);

        /// <summary>
        /// Обновить
        /// </summary>
        void Update(Customer сustomer);

        /// <summary>
        /// Обновить
        /// </summary>
        void Update(Branch branch);

        /// <summary>
        /// Обновить
        /// </summary>
        void Update(Inventor inventor);

        /// <summary>
        /// Обновить
        /// </summary>
        void Update(AuditConfig auditConfig, CBIContext contextCBI);

        string CreateNewDBPath(object domainObject, string code);
        string GetDBPath(object domainObject);
		DateTime GetLastUpdatedCatalog(object domainObject);
        string BuildFullDbPath(object domainObject);
		string BuildFullAnaliticDbPath(object domainObject);
		string BuildRelativeDbPath(object domainObject);
		string BuildLongCodesPath(object domainObject);
        bool CheckDbPath(object domainObject);
		bool CheckCount4UDbPath(object domainObject) ;
		bool CheckAnalitic4UDbPath(object domainObject);
        void AddMissedCount4UDbFile(object domainObject, bool isInherit = false);
        string GetImportFolderPath(object domainObject);
		string GetConfigFolderPath(object domainObject);  // решение что все конфиги лежат в ImportFolder - нужно для pack\unpack
		string GetImportLogDomainObjectFolderPath(object currentDomainObject, string folder);
        string GetExportToPDAFolderPath(object domainObject, bool withCurrentDomainObject = true);
        string GetFolderLogoPath(object domainObject);
		List<string> GetCount4UDBPathList_FromMainDBAndAuditDB();
		List<string> GetCount4UDBPathFolderList();
		void RefillInventorConfigs(object domainObject);
		void RefillAllCBIInventorConfigs();

        AuditConfigs GetCBIConfigs(CBIContext contextCBI);
        void SetCurrentCBIConfig(CBIContext contextCBI, AuditConfig auditConfig);
        void SaveCurrentCBIConfig(CBIContext contextCBI, AuditConfig auditConfig);
        void SetProcessCBIConfig(CBIContext contextCBI, AuditConfig auditConfig);

        AuditConfigs GetCBIConfigsLast(CBIContext contextCBI, int count);
        Customers GetHistoryCustomerLast();
        Branches GetHistoryBranchForCustomerLast(string customerCode, int count);
        AuditConfigs GetCBIConfigsByCustomerCodeLast(CBIContext contextCBI, int count,
        string customerCode);
        AuditConfigs GetCBIConfigsByBranchCodeLast(CBIContext contextCBI, int count,
        string branchCode);

        AuditConfig GetCBIConfigByCode(CBIContext contextCBI, string code);
        AuditConfigs GetCBIConfigsByCustomerCode(CBIContext contextCBI, string customerCode);
        AuditConfigs GetCBIConfigsByBranchCode(CBIContext contextCBI, string branchCode);
        AuditConfig GetCBIConfigByInventorCode(CBIContext contextCBI, string inventorCode);

        StatusAuditConfigs GetConfigStatus();
        void Insert(AuditConfig auditConfig, CBIContext contextCBI);
        void SetAuditStatus(StatusInventor statusInventor);
		List<string> GetCount4UDBPathDBList();
		
		string FindCount4UDBPathDB(string subFolder);
		List<string> FindCount4UDBPathDB(List<string> count4UDBPath, string subFolder);
		List<string> FindCount4UDBPathDB(List<string> count4UDBPath, string subFolder, List<string> files);

    }
}
