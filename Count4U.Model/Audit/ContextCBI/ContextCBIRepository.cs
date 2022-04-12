using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model;
using Count4U.Model.Audit;
//using Count4U.Model.Audit.Mapping;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
//using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using NLog;
using Count4U.Model.Count4U;
//using Count4U.Model.Count4Mobile;
//using System.Data.Entity;
using Count4U.Model.Common;
using System.Data.Entity;
using Count4U.Localization;

namespace Count4U.Model.Audit
{
    public class ContextCBIRepository : IContextCBIRepository
    {
        public readonly CBIContext _currentContextCBI;
        public readonly IServiceLocator _serviceLocator;
        public readonly ICustomerRepository _customerRepository;
        public readonly IBranchRepository _branchRepository;
        public readonly IInventorRepository _inventorRepository;
        public readonly IStatusInventorRepository _statusInventorRepository;
        public readonly List<IAuditConfigRepository> _auditConfigsRepository;
        public readonly IAuditConfigRepository _createInventorCBIConfigRepository;
        public readonly IAuditConfigRepository _historyCBIConfigRepository;
        public readonly IAuditConfigRepository _mainCBIConfigRepository;
        //public readonly IInventorConfigRepository _inventorConfigRepository;
        //public readonly IImportProviderDomainRepository _importProviderDomainRepository;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ContextCBIRepository(
            IServiceLocator serviceLocator,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository
            //IInventorConfigRepository inventorConfigRepository,
            // IImportProviderDomainRepository importProviderDomainRepository
            //IStatusInventorRepository statusInventorRepository
            //IAuditConfigRepository auditConfigRepository
            )
        {
            if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
            if (customerRepository == null) throw new ArgumentNullException("customerRepository");
            if (branchRepository == null) throw new ArgumentNullException("branchRepository");
            if (inventorRepository == null) throw new ArgumentNullException("inventorRepository");
            //if (inventorConfigRepository == null) throw new ArgumentNullException("inventorConfigRepository");
            //if (statusInventorRepository == null) throw new ArgumentNullException("statusInventorRepository");

            this._serviceLocator = serviceLocator;
            this._customerRepository = customerRepository;
            this._branchRepository = branchRepository;
            this._inventorRepository = inventorRepository;
            //this._inventorConfigRepository = inventorConfigRepository;
            //this._importProviderDomainRepository = importProviderDomainRepository;
            //this._statusInventorRepository = statusInventorRepository;
            this._auditConfigsRepository = new List<IAuditConfigRepository>(
                serviceLocator.GetAllInstances<IAuditConfigRepository>());
            if (this._auditConfigsRepository.Count < 1) throw new ArgumentNullException("auditConfigsRepository");

            this._createInventorCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.CreateInventor];		//0
            this._historyCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.History];		 //1
            this._mainCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.Main];		 //2

            this._createInventorCBIConfigRepository.SetCurrentConfig(CBIContext.CreateInventor, this.NewCBIConfig("AuditConfigCreateInventorCode"));
            //AuditConfig curAuditCofig  = this._historyCBIConfigRepository.GetProcessConfig(CBIContext.History);
            //if (curAuditCofig == null)
            //{				//TODO:
            //this._historyCBIConfigRepository.SetCurrentConfig(CBIContext.History, this.NewCBIConfig("AuditConfigHistoryCode"));
            //}
            this._mainCBIConfigRepository.SetCurrentConfig(CBIContext.Main, this.NewCBIConfig("AuditConfigViewMainCode"));

            this._currentContextCBI = CBIContext.CreateInventor;

 			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
         }
        #region IContextCBIRepository Members


        #region CBI

        #region private
        private Customers Customers
        {
            get
            {
                return this._customerRepository.GetCustomers(CBIContext.Main);
            }
            set
            {
                this._customerRepository.SetCustomers(value);			 // TODO: проверить необходимость
            }
        }

        private Branches Branches
        {
            get
            {
                return this._branchRepository.GetBranches();
            }
            set
            {
                this._branchRepository.SetBranches(value);
            }
        }

        private Inventors Inventors
        {
            get
            {
                return this._inventorRepository.GetInventors();
            }
            set
            {
                this._inventorRepository.SetInventors(value);
            }
        }

        private Customers GetCustomers(SelectParams selectParams)
        {
            Customers customers = this._customerRepository.GetCustomers(selectParams);
            return customers;
        }

        public Branches GetBranches(SelectParams selectParams)
        {
            Branches branches = this._branchRepository.GetBranches(selectParams);
            return branches;
        }

        public Inventors GetInventors(SelectParams selectParams)
        {
            Inventors inventors = this._inventorRepository.GetInventors(selectParams);
            return inventors;
        }
        #endregion

        public Customer GetCurrentCustomer(AuditConfig auditConfig)
        {
            return this._customerRepository.GetCurrent(auditConfig);
        }

        public Branch GetCurrentBranch(AuditConfig auditConfig)
        {
            return this._branchRepository.GetCurrent(auditConfig);
        }

        public Inventor GetCurrentInventor(AuditConfig auditConfig)
        {
            return this._inventorRepository.GetCurrent(auditConfig);
        }


        public string GetCodeCurrentCustomer(AuditConfig auditConfig)
        {
            return this._customerRepository.GetCurrentCode(auditConfig);
        }

        public string GetCodeCurrentBranch(AuditConfig auditConfig)
        {
            return this._branchRepository.GetCurrentCode(auditConfig);
        }

        public string GetCodeCurrentInventor(AuditConfig auditConfig)
        {
            return this._inventorRepository.GetCurrentCode(auditConfig);
        }

        /// <summary>
        /// Получить клиента  по коду
        /// </summary>
        /// <returns></returns>
        public Customer GetCustomerByCode(string code)
        {
            Customer ret = this._customerRepository.GetCustomerByCode(code);
            return ret;
        }

        /// <summary>
        /// Получить магазин по коду
        /// </summary>
        /// <returns></returns>
        public Branch GetBranchByCode(string code)
        {
            Branch ret = this._branchRepository.GetBranchByCode(code);
            return ret;
        }

        /// <summary>
        /// Получить инвентаризацию по коду
        /// </summary>
        /// <returns></returns>
        public Inventor GetInventorByCode(string code)
        {
            Inventor ret = this._inventorRepository.GetInventorByCode(code);
            return ret;
        }


        /// <summary>
        /// ТО DO проверить программую отработку  установки текущего 
        /// </summary>
        /// <param name="Code"></param>
        public void SetCurrentCustomer(Customer сustomer, AuditConfig auditConfig)
        {
            if (this._customerRepository.GetCurrent(auditConfig) != сustomer)
            {
                this._customerRepository.SetCurrent(сustomer, auditConfig);
                this._branchRepository.SetCurrent(null, auditConfig);
                this._inventorRepository.SetCurrent(null, auditConfig);

                //this.Customers.CurrentItem = сustomer;
                //this.Branches.CurrentItem = null;
                //this.Inventors.CurrentItem = null;
            }
        }

        public void SetCurrentBranch(Branch branch, AuditConfig auditConfig)
        {
            //if (this.Branches.CurrentItem != branch)
            //{
            //    this.Branches.CurrentItem = branch;
            //    this.Inventors.CurrentItem = null;
            //}
            if (this._branchRepository.GetCurrent(auditConfig) != branch)
            {
                this._branchRepository.SetCurrent(branch, auditConfig);
                this._inventorRepository.SetCurrent(null, auditConfig);
            }
        }

        public void SetCurrentInventor(Inventor inventor, AuditConfig auditConfig)
        {
            //if (this.Inventors.CurrentItem != inventor)
            //{
            //    this.Inventors.CurrentItem = inventor;
            //}
            if (this._inventorRepository.GetCurrent(auditConfig) != inventor)
            {
                this._inventorRepository.SetCurrent(inventor, auditConfig);
            }
        }

        public void CreateContextCustomer(Customer customer, AuditConfig auditConfig, bool toDoCurrent)
        {
			if (customer == null)
			{
				_logger.Trace("CreateContextCustomer", "customer is null");
				return;
			}

            this._customerRepository.Insert(customer);
            this.GetImportFolderPath(customer);
            if (toDoCurrent == true)
            {
                this.SetCurrentCustomer(customer, auditConfig);
            }
        }

        public void CreateContextBranch(Branch branch, AuditConfig auditConfig, bool toDoCurrent, bool inheritDataFromCustomer = false)
        {
			if (branch == null)
			{
				_logger.Trace("CreateContextBranch", "branch is null");
				return;
			}

            try
            {
                branch.CustomerCode = this.GetCodeCurrentCustomer(auditConfig);
            }
            catch (Exception exp)
			{
				_logger.Trace("CreateContextBranch", "GetCodeCurrentCustomer(auditConfig) return Exception :" + exp.Message);
			}

            if (inheritDataFromCustomer == true)
            {
                Customer customer = this.GetCustomerByCode(branch.CustomerCode);
				string inheritFromDBPath = this.GetDBPath(customer);

				DateTime lastUpdatedCatalog = GetLastUpdatedCatalog(customer);
				branch.LastUpdatedCatalog = lastUpdatedCatalog;

				this._branchRepository.Insert(branch, true, inheritFromDBPath);
            }
            else
            {
				branch.LastUpdatedCatalog = DateTime.Now;
                this._branchRepository.Insert(branch);
            }
            this.GetImportFolderPath(branch);
            if (toDoCurrent == true)
            {
                this.SetCurrentBranch(branch, auditConfig);
            }
        }

        public void CreateContextInventor(Inventor inventor, AuditConfig auditConfig, bool toDoCurrent, object domainObjectCopyDb = null)
        {
			if (inventor == null)
			{
				_logger.Trace("CreateContextInventor", "inventor is null");
				return;
			}

            try
            {
                inventor.CustomerCode = this.GetCodeCurrentCustomer(auditConfig);
                inventor.BranchCode = this.GetCodeCurrentBranch(auditConfig);
                //inventor.Code = auditConfig.InventorCode;
                //inventor.InventorDate = auditConfig.InventorDate;
                //inventor.CreateDate = auditConfig.CreateDate;
                //inventor.Description = auditConfig.Description;
                //inventor.DBPath = auditConfig.DBPath;
            }
			catch (Exception exp)
			{
				_logger.ErrorException("CreateContextInventor",  exp);
			}


            //Insert inventor to AuditDB

            string inheritFromDBPath = String.Empty;
            if (domainObjectCopyDb != null)
            {
                inheritFromDBPath = this.GetDBPath(domainObjectCopyDb);
				DateTime lastUpdatedCatalog = GetLastUpdatedCatalog(domainObjectCopyDb);
				inventor.LastUpdatedCatalog = lastUpdatedCatalog;
            }

            this._inventorRepository.Insert(inventor, inheritFromDBPath);	

            this.GetImportFolderPath(inventor);
            //Create new Count4UDB for Inventor
            //string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
            //string pathDB = this._inventorRepository.CopyEmptyDB(auditConfig.DBPath, folder);

            //Insert auditConfig to AuditDB
            //auditConfig.DBPath = pathDB;
			this._createInventorCBIConfigRepository.SetCurrentConfig(CBIContext.CreateInventor, auditConfig);
            this._createInventorCBIConfigRepository.Insert(auditConfig, CBIContext.CreateInventor);

            if (inventor.DBPath != auditConfig.DBPath)
            {
                string inventorCode = inventor.Code;
                inventor = this._inventorRepository.GetInventorByCode(inventorCode);
                inventor.DBPath = auditConfig.DBPath;
                this._inventorRepository.Update(inventor);
            }

            //TODO: Добавить запись в созданную БД Count4U в таблицу InventorConfig данные AuditConfig 
            //try
            //{
            //    this._inventorConfigRepository.SetCurrentConfig(auditConfig, inventor.DBPath);
            //}
            //catch { }

            if (toDoCurrent == true)
            {
                this.SetCurrentInventor(inventor, auditConfig);
            }
        }

        public Customers GetContextCustomers(AuditConfig auditConfig, CBIContext contextCBI)
        {
            
			return this._customerRepository.GetCustomers(contextCBI);
        }

        public Branches GetContextBranches(AuditConfig auditConfig, CBIContext contextCBI)
        {
            string currentCustomerCode = this.GetCodeCurrentCustomer(auditConfig);
            if (string.IsNullOrWhiteSpace(currentCustomerCode) == true) return null;

            Branches branches =
                this._branchRepository.GetBranchesByCustomerCode(currentCustomerCode, contextCBI);
            return branches;
        }

        public Inventors GetContextInventors(AuditConfig auditConfig)
        {
            string currentBrancheCode = this.GetCodeCurrentBranch(auditConfig);
            if (currentBrancheCode == null) return null;
            Inventors inventors =
                this._inventorRepository.GetInventorsByBranchCode(currentBrancheCode);
            return inventors;
        }

        public Customers GetContextCustomers(SelectParams selectParams, AuditConfig auditConfig)
        {
            Customers customers = this._customerRepository.GetCustomers(selectParams);
            return customers;
        }

        public Branches GetContextBranches(SelectParams selectParams, AuditConfig auditConfig)
        {
            string currentCustomerCode = this.GetCodeCurrentCustomer(auditConfig);
            if (string.IsNullOrWhiteSpace(currentCustomerCode) == true) return null;

            selectParams.FilterParams.Add("CustomerCode", new FilterParam()
            {
                Operator = FilterOperator.Contains,
                Value = currentCustomerCode
            });

            Branches branches = this._branchRepository.GetBranches(selectParams);

            //Customer currentCustomer = this.GetCurrentCustomer();
            //if (currentCustomer == null) return null;
            //Branches branches =
            //    this._branchRepository.GetBranchesByCustomerCode(currentCustomer.Code);
            return branches;
        }

        public Inventors GetContextInventors(SelectParams selectParams, AuditConfig auditConfig)
        {
            string currentBrancheCode = this.GetCodeCurrentBranch(auditConfig);
            if (string.IsNullOrWhiteSpace(currentBrancheCode) == true) return null;

            selectParams.FilterParams.Add("BranchCode", new FilterParam()
            {
                Operator = FilterOperator.Contains,
                Value = currentBrancheCode
            });

            Inventors inventors = this._inventorRepository.GetInventors(selectParams);

            //Inventors inventors =
            //    this._inventorRepository.GetInventorsByBranchCode(currentBranche.Code);
            return inventors;
        }

		//public Customer Clone(Customer сustomer)
		//{
		//    return this._customerRepository.Clone(сustomer);
		//}

		//public Branch Clone(Branch branch)
		//{
		//    return this._branchRepository.Clone(branch);
		//}

		//public Inventor Clone(Inventor inventor)
		//{
		//    return this._inventorRepository.Clone(inventor);
		//}

        public void Delete(Customer сustomer, bool full = true)
        {
            this._inventorRepository.DeleteAllByCustomerCode(сustomer.Code, full);
            this._branchRepository.DeleteAllByCustomerCode(сustomer.Code, full);
            this._customerRepository.Delete(сustomer);
            this._historyCBIConfigRepository.DeleteAllByCustomerCode(сustomer.Code, CBIContext.History);
        }

        public void Delete(Branch branch, bool full = true)
        {
            this._inventorRepository.DeleteAllByBranchCode(branch.Code, full);
            this._branchRepository.Delete(branch);
            this._historyCBIConfigRepository.DeleteAllByBranchCode(branch.Code, CBIContext.History);
        }

        public void Delete(Inventor inventor)
        {	 
  			if (inventor == null) return;
			string inventorCode = inventor.Code;
            this._inventorRepository.Delete( inventorCode);
			this._historyCBIConfigRepository.DeleteByInventorCode(inventorCode, CBIContext.History);
        }

        public void Update(Customer сustomer)
        {
			if (сustomer == null) return;
            this._customerRepository.Update(сustomer);
            this.GetImportFolderPath(сustomer);
        }

        public void Update(Branch branch)
        {
			if (branch == null) return;
            this._branchRepository.Update(branch);
            this.GetImportFolderPath(branch);
        }

        public void Update(Inventor inventor)
        {
			if (inventor == null) return;
            this._inventorRepository.Update(inventor);
            this.GetImportFolderPath(inventor);
        }

        public bool IsExistsCustomerCode(string code)
        {
            Customer customer = this._customerRepository.GetCustomerByCode(code);
            if (customer != null) return true;
            else return false;
        }

        public bool IsExistsBranchCode(string code)
        {
            Branch branch = this._branchRepository.GetBranchByCode(code);
            if (branch != null) return true;
            else return false;
        }

        public bool IsExistsInventorCode(string code)
        {
            Inventor inventor = this._inventorRepository.GetInventorByCode(code);
            if (inventor != null) return true;
            else return false;
        }

        #endregion

        #region Status
        public StatusAuditConfigs GetConfigStatus()
        {
            throw new NotImplementedException();
        }

        public void SetAuditStatus(StatusInventor statusAudit)
        {
            //if (statusInventor == null) throw new ArgumentNullException("statusInventor");

            //Inventor currentInventor = this.GetCurrentInventor();
            //if (currentInventor == null) throw new ArgumentNullException("currentInventor");
            //else
            //{
            //    this._inventorRepository.SetStatus(currentInventor, statusInventor.Code, statusInventor.Name);
            //}
            throw new NotImplementedException();
        }

        public StatusInventor GetAuditStatus()
        {
            //Inventor currentInventor = GetCurrentInventor();
            //if (currentInventor == null)
            //{
            //    return this._statusAuditRepository.CreateNoneStatusInventor();
            //    throw new ArgumentNullException("currentInventor");
            //}
            //else
            //{
            //    return this._statusAuditRepository.GetStatusByCode(currentInventor.StatusCode);
            //}
            throw new NotImplementedException();
        }
        #endregion


        #region editCBIConfig
        public AuditConfigs GetCBIConfigs(CBIContext contextCBI)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetAuditConfigs(CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetAuditConfigs(CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigs(CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigs(CBIContext.Main);
            }
        }


        //public void ClearListContextCBI()
        //{
        //    this.GetAuditConfigs().Clear();
        //}

        //// TODO:
        //public bool InitializeCurrentContextCBIByCode(string code)
        //{
        //    throw new NotImplementedException();
        //}


        public AuditConfig GetCurrentCBIConfig(CBIContext contextCBI, bool fromDB = false)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetCurrentConfig(CBIContext.CreateInventor, fromDB);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetCurrentConfig(CBIContext.Main, fromDB);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetCurrentConfig(CBIContext.History, fromDB);
                default:
                    return this._mainCBIConfigRepository.GetCurrentConfig(CBIContext.Main, fromDB);
            }

        }

        public void SetProcessCBIConfig(CBIContext contextCBI, AuditConfig auditConfig)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    {
                        this._createInventorCBIConfigRepository.SetProcessConfig(CBIContext.CreateInventor, auditConfig);
                        break;
                    }
                case CBIContext.Main:
                    {
                        this._mainCBIConfigRepository.SetProcessConfig(CBIContext.Main, auditConfig);
                        break;
                    }
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.SetProcessConfig(CBIContext.History, auditConfig);
                        break;
                    }

                default:
                    {
                        this._mainCBIConfigRepository.SetProcessConfig(CBIContext.Main, auditConfig);
                        break;
                    }
            }

			
        }

        public AuditConfig GetProcessCBIConfig(CBIContext contextCBI)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetProcessConfig(CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetProcessConfig(CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetProcessConfig(CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetProcessConfig(CBIContext.Main);
            }

        }

        public void SetCurrentCBIConfig(CBIContext contextCBI, AuditConfig auditConfig)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    {
                        this._createInventorCBIConfigRepository.SetCurrentConfig(CBIContext.CreateInventor, auditConfig);
                        break;
                    }
                case CBIContext.Main:
                    {
                        this._mainCBIConfigRepository.SetCurrentConfig(CBIContext.Main, auditConfig);
                        break;
                    }
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.SetCurrentConfig(CBIContext.History, auditConfig);
                        break;
                    }

                default:
                    {
                        this._mainCBIConfigRepository.SetCurrentConfig(CBIContext.Main, auditConfig);
                        break;
                    }
            }
        }

        public void SaveCurrentCBIConfig(CBIContext contextCBI, AuditConfig auditConfig)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    {
                        this._createInventorCBIConfigRepository.SetCurrentConfig(CBIContext.CreateInventor, auditConfig);
                        break;
                    }
                case CBIContext.Main:
                    {
                        this._mainCBIConfigRepository.SetCurrentConfig(CBIContext.Main, auditConfig);
                        break;
                    }
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.SetCurrentConfig(CBIContext.History, auditConfig);
                        break;
                    }

                default:
                    {
                        this._mainCBIConfigRepository.SetCurrentConfig(CBIContext.Main, auditConfig);
                        break;
                    }
            }
        }

        public AuditConfigs GetCBIConfigsLast(CBIContext contextCBI, int count)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    //return this._createInventorCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.CreateInventor);
                    return null;
                case CBIContext.Main:
                    //return this._mainCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.Main);
                    return null;
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.History);
            }
        }

        public Customers GetHistoryCustomerLast()
        {
            Customers customers = this._historyCBIConfigRepository.GetHistoryCustomerLast();
            return customers;
        }

        public Branches GetHistoryBranchForCustomerLast(string customerCode, int count)
        {
            return this._historyCBIConfigRepository.GetHistoryBranchForCustomerLast(customerCode, count);
        }

        public AuditConfigs GetCBIConfigsByCustomerCodeLast(CBIContext contextCBI, int count,
            string customerCode)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return null;
                //return this._createInventorCBIConfigRepository.GetAuditConfigsByCustomerCodeLast
                //	(count, customerCode, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return null;
                //return this._mainCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.Main);
                case CBIContext.History:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByCustomerCodeLast
                        (count, customerCode, CBIContext.History);
                default:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByCustomerCodeLast
                        (count, customerCode, CBIContext.History);
            }
        }

        public AuditConfigs GetCBIConfigsByBranchCodeLast(CBIContext contextCBI, int count,
        string branchCode)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return null;
                //return this._createInventorCBIConfigRepository.GetAuditConfigsByCustomerCodeLast
                //	(count, customerCode, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return null;
                //return this._mainCBIConfigRepository.GetAuditConfigsLast(count, CBIContext.Main);
                case CBIContext.History:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByBranchCodeLast
                        (count, branchCode, CBIContext.History);
                default:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByBranchCodeLast
                        (count, branchCode, CBIContext.History);
            }
        }

        public AuditConfig GetCBIConfigByCode(CBIContext contextCBI, string code)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetAuditConfigByCode(code, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetAuditConfigByCode(code, CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigByCode(code, CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigByCode(code, CBIContext.Main);
            }

        }

        public AuditConfigs GetCBIConfigsByCustomerCode(CBIContext contextCBI, string customerCode)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByCustomerCode(customerCode, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetAuditConfigsByCustomerCode(customerCode, CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigsByCustomerCode(customerCode, CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigsByCustomerCode(customerCode, CBIContext.Main);
            }
        }

        public AuditConfigs GetCBIConfigsByBranchCode(CBIContext contextCBI, string branchCode)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetAuditConfigsByBranchCode(branchCode, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetAuditConfigsByBranchCode(branchCode, CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigsByBranchCode(branchCode, CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigsByBranchCode(branchCode, CBIContext.Main);
            }
        }

        public AuditConfig GetCBIConfigByInventorCode(CBIContext contextCBI, string inventorCode)
        {
            switch (contextCBI)
            {
                case CBIContext.CreateInventor:
                    return this._createInventorCBIConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.CreateInventor);
                case CBIContext.Main:
                    return this._mainCBIConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.Main);
                case CBIContext.History:
                    return this._historyCBIConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.History);
                default:
                    return this._mainCBIConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.Main);
            }
        }

        public AuditConfig NewCBIConfig(string newCode)
        {
            return new AuditConfig { Code = newCode };
        }

		//public AuditConfig Clone(AuditConfig auditConfig)
		//{
		//    AuditConfig audConfig = new AuditConfig();
		//    return audConfig;
		//}

        public void Delete(AuditConfig auditConfig, CBIContext contextCBI)
        {
            switch (contextCBI)
            {
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.Delete(auditConfig, CBIContext.History);
                        break;
                    }
                case CBIContext.CreateInventor:
                case CBIContext.Main:
                default:
                    return;
            }

        }

        public void Insert(AuditConfig auditConfig, CBIContext contextCBI)		  //TODO:
        {
            switch (contextCBI)
            {
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.Insert(auditConfig, CBIContext.History);
                        break;
                    }
                case CBIContext.CreateInventor:
                    {
                        this._historyCBIConfigRepository.Insert(auditConfig, CBIContext.CreateInventor);
                        break;
                    }
                case CBIContext.Main:
                default:
                    return;
            }
        }

        public void Update(AuditConfig auditConfig, CBIContext contextCBI)
        {

			if (auditConfig == null) return;
            switch (contextCBI)
            {
                case CBIContext.History:
                    {
                        this._historyCBIConfigRepository.Update(auditConfig, CBIContext.History);
                        break;
                    }
                case CBIContext.CreateInventor:
                case CBIContext.Main:
                default:
                    return;
            }
        }
        #endregion



        #endregion


        #region IContextCBIRepository Members

        //build relaive path to c/b/i .sdf file . Not part of IContextCBIRepository interface
		public string BuildRelativeDbPath(object domainObject)
        {
            if (domainObject == null) return String.Empty;

            Customer customer = domainObject as Customer;
            if (customer != null)
                return PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.DBPath;

            Branch branch = domainObject as Branch;
            if (branch != null)
                return PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.DBPath;

            Inventor inventor = domainObject as Inventor;
            if (inventor != null)
                return PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;

            return String.Empty;
        }

		public string BuildLongCodesPath(object domainObject)
		{
			if (domainObject == null) return String.Empty;

			Customer customer = domainObject as Customer;
			if (customer != null)
			{
				string customerCode = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.Code;
				return customerCode;
			}

			Branch branch = domainObject as Branch;
			if (branch != null)
			{
				string customerCode = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + branch.CustomerCode;
				string branchCode = PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.Code;
				return customerCode + @"\" + branchCode;
			}

			Inventor inventor = domainObject as Inventor;
			if (inventor != null)
			{
				string customerCode = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + inventor.CustomerCode;
				string branchCode = PropertiesSettings.FolderBranch.Trim('\\') + @"\" + inventor.BranchCode;
				string inventorCode = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
				return customerCode + @"\" + branchCode + @"\" + inventorCode;
			}

			return String.Empty;
		}

        //build relaive path to c/b/i .sdf file . Not part of IContextCBIRepository interface
        public string BuildFullDbPath(object domainObject)
        {
            if (domainObject == null) return String.Empty;

            string relativePath = BuildRelativeDbPath(domainObject);
            String fullPath = String.Empty;

            Customer customer = domainObject as Customer;
            if (customer != null)
                fullPath = this._customerRepository.Connection.BuildCount4UDBFilePath(relativePath);

            Branch branch = domainObject as Branch;
            if (branch != null)
                fullPath = this._branchRepository.Connection.BuildCount4UDBFilePath(relativePath);

            Inventor inventor = domainObject as Inventor;
            if (inventor != null)
                fullPath = this._inventorRepository.Connection.BuildCount4UDBFilePath(relativePath);

            return fullPath;
        }

		//build relaive path to c/b/i AnaliticDB.sdf file . Not part of IContextCBIRepository interface
		public string BuildFullAnaliticDbPath(object domainObject)
		{
			if (domainObject == null) return String.Empty;

			string relativePath = BuildRelativeDbPath(domainObject);
			String fullPath = String.Empty;

			Customer customer = domainObject as Customer;
			if (customer != null)
				fullPath = this._customerRepository.Connection.BuildAnalyticDBFilePath(relativePath);

			Branch branch = domainObject as Branch;
			if (branch != null)
				fullPath = this._branchRepository.Connection.BuildAnalyticDBFilePath(relativePath);

			Inventor inventor = domainObject as Inventor;
			if (inventor != null)
				fullPath = this._inventorRepository.Connection.BuildAnalyticDBFilePath(relativePath);

			return fullPath;
		}

        //return path to /c/b/i .sdf with additional logging
        public string GetDBPath(object domainObject)
        {
            if (domainObject == null) return "";
            string relativePath = BuildRelativeDbPath(domainObject);
            string count4UFilePath = BuildFullDbPath(domainObject);

            if (File.Exists(count4UFilePath) == false)
            {
                Customer customer = domainObject as Customer;
                if (customer != null)
                {
                    _logger.Info("ContextCBIRepository.GetDBPath >> File DB for Customer [ " + count4UFilePath + " ] Is Not Exists");
                    relativePath = "";
                }

                Branch branch = domainObject as Branch;
                if (branch != null)
                {
                    _logger.Info("ContextCBIRepository.GetDBPath >> File DB for Branch [ " + count4UFilePath + " ] Is Not Exists");
                    relativePath = "";
                }

                Inventor inventor = domainObject as Inventor;
                if (inventor != null)
                {
                    _logger.Info("ContextCBIRepository.GetDBPath >> File DB for Inventor [ " + count4UFilePath + " ] Is Not Exists");
					string inventorRelative = this.BuildRelativeDbPath(inventor);
					string path1 = this._inventorRepository.Connection.CopyEmptyCount4UAndAnaliticDB(inventorRelative);
					if (string.IsNullOrWhiteSpace(path1) == true)
					{
						relativePath = "";
						_logger.Info("ContextCBIRepository.GetDBPath >> File DB for Inventor [ " + count4UFilePath + " ] Can't create");
					}
					else 
					{
						this._inventorRepository.RefillInventorConfigs(inventor);
					}
                }
            }

            return relativePath;
        }

        //check that .sdf file for c/b/i exists
        public bool CheckDbPath(object domainObject)
        {
            string count4UFilePath = BuildFullDbPath(domainObject);
			string analiticDBFilePath = BuildFullAnaliticDbPath(domainObject);

			if (String.IsNullOrWhiteSpace(count4UFilePath) || !File.Exists(count4UFilePath) || !File.Exists(analiticDBFilePath))
                return false;

            return true;
        }

		public bool CheckCount4UDbPath(object domainObject)
		{
			string count4UFilePath = BuildFullDbPath(domainObject);
	
			if (String.IsNullOrWhiteSpace(count4UFilePath) || !File.Exists(count4UFilePath) )
				return false;

			return true;
		}

		public bool CheckAnalitic4UDbPath(object domainObject)
		{
			string analiticDBFilePath = BuildFullAnaliticDbPath(domainObject);

			if (String.IsNullOrWhiteSpace(analiticDBFilePath) || !File.Exists(analiticDBFilePath))
				return false;

			return true;
		}

        //create .sdf path for Customer or Branch
        public void AddMissedCount4UDbFile(object domainObject, bool isInherit = false)
        {
			string newCount4UDbFilePath = "";
            string count4UsdfFilePath = BuildFullDbPath(domainObject);

            _logger.Info(String.Format("Request for create .sdf file: [{0}]", count4UsdfFilePath));

            if (String.IsNullOrWhiteSpace(count4UsdfFilePath) == true)
                return;

            try
            {
                FileInfo fi = new FileInfo(count4UsdfFilePath);
                string sdfDir = fi.DirectoryName;
                if (String.IsNullOrWhiteSpace(sdfDir) == false) //create directory if not exist
                {
                    if (Directory.Exists(sdfDir) == false)
                    {
                        _logger.Info(String.Format("Directory was missed: {0}", sdfDir));
                        Directory.CreateDirectory(sdfDir);
                    }
                }

                Customer customer = domainObject as Customer;
                if (customer != null)
                {
                    _logger.Info(String.Format("Copying empty db for customer:  [{0}]", customer.Code));
					string customerRelative = this.BuildRelativeDbPath(customer);
					string customerCount4UDFull = this._customerRepository.Connection.BuildCount4UDBFilePath(customerRelative);
					if (File.Exists(customerCount4UDFull) == false) 
					{
						this._customerRepository.Connection.CopyEmptyCount4UAndAnaliticDB(customerRelative);
					}
					else
					{
						this._branchRepository.Connection.CopyEmptyAnaliticDB(customerRelative);
					}
	            }

                Branch branch = domainObject as Branch;
                if (branch != null)
                {
                    if (isInherit == true) //inherit .sdf file from customer
                    {
                        Customer customerFromBranch = this.GetCustomerByCode(branch.CustomerCode);
						if (customerFromBranch != null)
						{
							_logger.Info(String.Format("Copying customer [ {0} ] db for branch: [{1}]", customerFromBranch.Code, branch.Code));

							string cusomterRelative = this.GetDBPath(customerFromBranch);
							if (string.IsNullOrWhiteSpace(cusomterRelative) == false)		//есть папка c Customer
							{
								string customerCount4UDFull = this._customerRepository.Connection.BuildCount4UDBFilePath(cusomterRelative);
								if (File.Exists(customerCount4UDFull) == true) // есть файл  Castomer Count4U.sdf можем копировать его в Branch
								{
									string branchRelativeFolder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";
									newCount4UDbFilePath = this._branchRepository.Connection.CopyCount4UDB(branch.DBPath, branchRelativeFolder, customerCount4UDFull);
									//Analitic не наследуем
									string branchRelative =  this.BuildRelativeDbPath(branch);
									this._branchRepository.Connection.CopyEmptyAnaliticDB(branchRelative);
								}
								else   // нет файла  Customer Count4U.sdf нечего копировать , создаем пустые
								{
									_logger.Info(String.Format("Copying empty db for branch: [ {0} ]", branch.Code));
									string branchRelative = this.BuildRelativeDbPath(branch);
									newCount4UDbFilePath = this._branchRepository.Connection.CopyEmptyCount4UAndAnaliticDB(branchRelative);
								}
								
							}
							else
							{
								_logger.Info(String.Format("Copying empty db for branch: [ {0} ]", branch.Code));
								string branchRelative = this.BuildRelativeDbPath(branch);
								newCount4UDbFilePath = this._branchRepository.Connection.CopyEmptyCount4UAndAnaliticDB(branchRelative);
							}

						}
                    }
                    else
                    {
                        _logger.Info(String.Format("Copying empty db for branch: [ {0} ]", branch.Code));
						string branchRelative = this.BuildRelativeDbPath(branch);
						newCount4UDbFilePath = this._branchRepository.Connection.CopyEmptyCount4UAndAnaliticDB(branchRelative);
                    }
                }

				Inventor inventor = domainObject as Inventor;
				if (inventor != null)
				{
					_logger.Info(String.Format("Copying empty AnaliticDB for inventor:  [{0}]", inventor.Code));
					string inventorRelative = this.BuildRelativeDbPath(inventor);
					string customerAnalyticDBFull = this._customerRepository.Connection.BuildAnalyticDBFilePath(inventorRelative);
					if (File.Exists(customerAnalyticDBFull) == false)
					{
						this._branchRepository.Connection.CopyEmptyAnaliticDB(inventorRelative);
					}
				}

				if (string.IsNullOrWhiteSpace(newCount4UDbFilePath) == true)
				{
					//_logger.Info("ContextCBIRepository.GetDBPath >> File DB [" + count4UsdfFilePath + "] Can't create");
				}
				else
				{
					this.RefillInventorConfigs(domainObject);
				}
            }
            catch (Exception exc)
            {
                _logger.Error(String.Format("Can't create: [{0}]", count4UsdfFilePath));
				_logger.ErrorException("AddMissedCount4UDbFile", exc);
            }
        }

		public DateTime GetLastUpdatedCatalog(object domainObject)
		{
			DateTime lastUpdatedCatalog = DateTime.Now;
			if (domainObject == null) return lastUpdatedCatalog;

			Customer customer = domainObject as Customer;
			if (customer != null) return customer.LastUpdatedCatalog;

			Branch branch = domainObject as Branch;
			if (branch != null) return branch.LastUpdatedCatalog;

			Inventor inventor = domainObject as Inventor;
			if (inventor != null) return inventor.LastUpdatedCatalog;

			return lastUpdatedCatalog;

		}

		public void RefillInventorConfigs(object domainObject)
		{
			    Customer customer = domainObject as Customer;
                if (customer != null)
                {
					this._customerRepository.RefillInventorConfigs (customer);
                }

                Branch branch = domainObject as Branch;
				if (branch != null)
				{
					this._branchRepository.RefillInventorConfigs(branch);
				}

				Inventor inventor = domainObject as Inventor;
				if (inventor != null)
				{
					this._inventorRepository.RefillInventorConfigs(inventor);
				}
		}

		public void RefillAllCBIInventorConfigs()
		{
			this._customerRepository.RefillInventorConfigsAllCustomersInMainDB();
			this._branchRepository.RefillInventorConfigsAllBranchesInMainDB();
			this._inventorRepository.RefillInventorConfigsAllInventorsInAuditDB();
		}

        public string CreateNewDBPath(object domainObject, string code)
        {
            if (domainObject == null) return "";
            string path = "";
            if (domainObject is Customer)
            {
                path = code;
            }
            if (domainObject is Branch)
            {
                path = code;
            }
            Inventor inventor = domainObject as Inventor;
            if (inventor != null)
            {
                DateTime date = inventor.InventorDate;
                path = String.Format(@"{0}\{1}\{2}\{3}", date.Year, date.Month, date.Day, code);
            }
            return path;
        }

		
        public string GetImportFolderPath(object domainObject)
        {
            if (domainObject == null) return "";
            string folderPath = this._customerRepository.Connection.ImportFolderPath().Trim('\\');
            string folderImport = "";
            if (domainObject is Customer)
            {
                folderImport = folderPath + @"\" +
                    PropertiesSettings.FolderCustomer.Trim('\\') + @"\" +
                     ((Customer)domainObject).Code;
            }
            if (domainObject is Branch)
            {
                folderImport = folderPath + @"\" +
                    PropertiesSettings.FolderBranch.Trim('\\') + @"\" +
                    ((Branch)domainObject).Code;
            }
            if (domainObject is Inventor)
            {
                folderImport = folderPath + @"\" +
                    PropertiesSettings.FolderInventor.Trim('\\') + @"\" +
                     ((Inventor)domainObject).Code;
            }

			if (Directory.Exists(folderImport) == false)
			{
				try
				{
					Directory.CreateDirectory(folderImport);
				}
				catch (Exception ex)
				{
					_logger.ErrorException("GetImportFolderPath", ex);
					return "";
				}
			}
            return folderImport;

        }

	
		public string GetConfigFolderPath(object currentDomainObject)
		{
			if (currentDomainObject != null)
			{

				string dataInPath = this.GetImportFolderPath(currentDomainObject);

				string dataInConfigPath = dataInPath + @"\Config";

				if (string.IsNullOrWhiteSpace(dataInPath) == false)
				{
					if (Directory.Exists(dataInConfigPath) == false)
					{
						Directory.CreateDirectory(dataInConfigPath);
					}
				}
				return dataInConfigPath;
			}
			return String.Empty;
		}

		public string GetImportLogDomainObjectFolderPath(object currentDomainObject, string folder)
		{
			string logPath = "";
			if (currentDomainObject != null)
			{
				logPath = this.GetImportFolderPath(currentDomainObject);

				if (Directory.Exists(logPath) == false)
				{
					try { Directory.CreateDirectory(logPath); }
					catch { }
				}

				logPath = logPath.Trim('\\') + folder;
				if (Directory.Exists(logPath) == false)
				{
					try { Directory.CreateDirectory(logPath); }
					catch { }
				}
			}

			return logPath;
		}

        public string GetExportToPDAFolderPath(object domainObject, bool withCurrentDomainObject = true)
        {
            string uploadFolder = "";
            if (domainObject == null) return "";
            uploadFolder = this._customerRepository.Connection.ExportToPDAFolderPath().Trim('\\');
            if (withCurrentDomainObject == false)
            {
                return uploadFolder;
            }

            if (domainObject is Customer)
            {
                uploadFolder = uploadFolder + @"\" + ((Customer)domainObject).Code;
            }
            else if (domainObject is Branch)
            {
                uploadFolder = uploadFolder + @"\" + ((Branch)domainObject).Code;
            }
            else if (domainObject is Inventor)
            {
                uploadFolder = uploadFolder + @"\" + ((Inventor)domainObject).Code;
            }

            return uploadFolder;
        }


        public string GetFolderLogoPath(object domainObject)
        {
            if (domainObject == null) return "";
            string folderPath = this._customerRepository.Connection.FolderLogoPath().Trim('\\');
            string folderLogo = "";
            if (domainObject is Customer)
            {
                folderLogo = folderPath + @"\" +
                    PropertiesSettings.FolderCustomer.Trim('\\') + @"\" +
                     ((Customer)domainObject).Code;
            }
            if (domainObject is Branch)
            {
                folderLogo = folderPath + @"\" +
                    PropertiesSettings.FolderBranch.Trim('\\') + @"\" +
                    ((Branch)domainObject).Code;
            }
            if (domainObject is Inventor)
            {
                folderLogo = folderPath + @"\" +
                    PropertiesSettings.FolderInventor.Trim('\\') + @"\" +
                     ((Inventor)domainObject).Code;
            }

            if (Directory.Exists(folderLogo) == false)
            {
                try
                {
                    Directory.CreateDirectory(folderLogo);
                }
                catch
				{
					return "";
				}
            }
            return folderLogo;

        }

		public List<string> GetCount4UDBPathFolderList()
		{
			List<string> count4UDBPathList = new List<string>();
			string folderPath = this._customerRepository.Connection.BuildCount4UDBFolderPath("").Trim('\\');	   //../../.App_Data

			string folderCustomer = folderPath + @"\" + PropertiesSettings.FolderCustomer.Trim('\\');	//../../.App_Data/Customer
			string folderBranch = folderPath + @"\" + PropertiesSettings.FolderBranch.Trim('\\');		   //../../.App_Data/Branch
			string folderInventor = folderPath + @"\" + PropertiesSettings.FolderInventor.Trim('\\');	  //../../.App_Data/Inventor

			count4UDBPathList = AddCount4UDBPathToList(count4UDBPathList, folderCustomer);	  //+ все директории внутри  ../../.App_Data/Customer, где найдена "Count4UDB.sdf"
			count4UDBPathList = AddCount4UDBPathToList(count4UDBPathList, folderBranch);
			count4UDBPathList = AddCount4UDBPathToList(count4UDBPathList, folderInventor);
			return count4UDBPathList;
		}

		private static List<string> AddCount4UDBPathToList(List<string> count4UDBPath, string folder)
		{
			if (Directory.Exists(folder) == true)
			{
				folder = folder + @"\";
				string[] files = Directory.GetFiles(folder, "Count4UDB.sdf", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					count4UDBPath.Add(Path.GetFullPath(files[i]));
				}
			}
			return count4UDBPath;
		}


		public List<string> GetCount4UDBPathDBList()
		{
			List<string> count4UDBPath = new List<string>();
			
			count4UDBPath = this.FindCount4UDBPathDB(count4UDBPath, PropertiesSettings.FolderCustomer.Trim('\\'));
			count4UDBPath = this.FindCount4UDBPathDB(count4UDBPath, PropertiesSettings.FolderBranch.Trim('\\'));
			count4UDBPath = this.FindCount4UDBPathDB(count4UDBPath, PropertiesSettings.FolderInventor.Trim('\\'));
			return count4UDBPath;
		}

		public List<string> FindCount4UDBPathDB(List<string> count4UDBPath, string subFolder)
		{
			string folderPath = this._customerRepository.Connection.BuildCount4UDBFolderPath("").Trim('\\');		// D:\Count4U\trunk\Count4U\Count4U\bin\Debug\..\..\..\Count4U.Model\App_Data
			subFolder = @"\" + subFolder + @"\";
			string folder = folderPath + subFolder;	   //D:\Count4U\trunk\Count4U\Count4U\bin\Debug\..\..\..\Count4U.Model\App_Data\Customer\
			if (Directory.Exists(folder) == true)
			{
				string[] files = Directory.GetFiles(folder, "Count4UDB.sdf", SearchOption.AllDirectories);	 // D:\Count4U\trunk\Count4U\Count4U\bin\Debug\..\..\..\Count4U.Model\App_Data\Customer\a0ab472d-28e6-4c94-8087-eb3b2bd75fee\Count4UDB.sdf
				for (int i = 0; i < files.Length; i++)
				{
					int index = files[i].IndexOf(subFolder);
					string dbPath = files[i].Substring(index);
					index = dbPath.IndexOf("Count4UDB.sdf");
					dbPath = dbPath.Remove(index);
					count4UDBPath.Add(dbPath); // \Customer\a0ab472d-28e6-4c94-8087-eb3b2bd75fee\
				}
			}
			return count4UDBPath;
		}

		public string FindCount4UDBPathDB(string subFolder)
		{
			string folderPath = this._customerRepository.Connection.BuildCount4UDBFolderPath("").Trim('\\');		  //App_Data
			subFolder = @"\" + subFolder + @"\";
			string folder = folderPath + subFolder;
			if (Directory.Exists(folder) == true)
			{
				string[] files = Directory.GetFiles(folder, "Count4UDB.sdf", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					if (files[i].Contains("removed") == true) continue;
					int index = files[i].IndexOf(subFolder);
					string dbPath = files[i].Substring(index);
					index = dbPath.IndexOf("Count4UDB.sdf");
					dbPath = dbPath.Remove(index);
					return dbPath;
				}
			}
			return "";
		}

		public List<string> FindCount4UDBPathDB(List<string> count4UDBPath, string subFolder, List<string> files)
		{
			//string folderPath = this._customerRepository.Connection.BuildCount4UDBFolderPath("").Trim('\\');		  //App_Data
			subFolder = @"\" + subFolder + @"\";
			//string folder = folderPath + subFolder;
			//if (Directory.Exists(folder) == true)
			//{
				//string[] files = Directory.GetFiles(folder, "Count4UDB.sdf", SearchOption.AllDirectories);
				for (int i = 0; i < files.Count; i++)
				{
					int index = files[i].IndexOf(subFolder);
					string dbPath = files[i].Substring(index);
					index = dbPath.IndexOf("Count4UDB.sdf");
					dbPath = dbPath.Remove(index);
					count4UDBPath.Add(dbPath);
				}
			//}
			return count4UDBPath;
		}

		public List<string> GetCount4UDBPathList_FromMainDBAndAuditDB()
        {
            List<string> dbPathes = new List<string>();
            List<object> cbiObject = new List<object>();

            AuditConfigs auditConfigs = this.GetCBIConfigs(CBIContext.History);
            foreach (AuditConfig auditConfig in auditConfigs)
            {
                Inventor inventor = this.GetInventorByCode(auditConfig.InventorCode);
                cbiObject.Add(inventor);
            }

            Customers customers = this._customerRepository.GetCustomers(CBIContext.Main);
            foreach (Customer customer in customers)
            {
                this.GetCustomerByCode(customer.Code);
                cbiObject.Add(customer);
            }

            Branches branchs = this._branchRepository.GetBranches(CBIContext.Main);
            foreach (Branch branch in branchs)
            {
                this.GetBranchByCode(branch.Code);
                cbiObject.Add(branch);
            }

            cbiObject = cbiObject.Distinct().ToList();

            List<string> dbPathList = new List<string>();

            cbiObject.ForEach(x => dbPathList.Add(this.GetDBPath(x)));
            return dbPathList;
        }

        #endregion

     
    }
}

