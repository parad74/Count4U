using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using System.Collections.Generic;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Script
{
    public class CBIScriptOpenViewModel: ScriptOpenBaseViewModel
    {
		//private readonly IServiceLocator _serviceLocator;
        private enCBIScriptMode _mode;

        public CBIScriptOpenViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
			: base(contextCbiRepository, eventAggregator, serviceLocator)
        {
            //this._serviceLocator = serviceLocator;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._mode = UtilsConvert.ConvertToEnum<enCBIScriptMode>(navigationContext);
        }

		public void  CopyDB()
        {
	
		 }

        protected override void RunScript()
        {
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
            IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
            log.Clear();
		   	try
			{
				string importTable = "";
				string sql = "";
				if (this._mode == enCBIScriptMode.Customer)
				{
					importTable = @"[Customer]";
					ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
					List<string> listCode = customerRepository.GetCodeList();
					sql = InsertSqlFromScriptToTable(importTable, listCode);
					alterADOProvider.ImportToMainDB(sql, importTable, this._isClear);
				}
				else if (this._mode == enCBIScriptMode.Branch)
				{
					importTable = @"[Branch]";
					IBranchRepository branchRepository = this._serviceLocator.GetInstance<IBranchRepository>();
					List<string> listCode = branchRepository.GetCodeList();
					sql = InsertSqlFromScriptToTable(importTable, listCode);
					alterADOProvider.ImportToMainDB(sql, importTable, this._isClear);
					branchRepository.DeleteBranchWithDoubleCode("RepairBranch");
 				}
				else if (this._mode == enCBIScriptMode.Inventor)
				{
					importTable = @"[Inventor]";
					IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
					List<string> listCode = inventorRepository.GetCodeList();
					sql = InsertSqlFromScriptToTable(importTable, listCode);
					alterADOProvider.ImportToAuditDB(sql, importTable, this._isClear);
				}
				else return;

					this.Log = log.PrintLog();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunScript", exc);
			}
			finally
			{
				Utils.RunOnUI(() => IsBusy = false);

				this._isScriptWasLaunched = true;
			}

            this.Log = log.PrintLog();
       }        
    }
}