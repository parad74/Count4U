using System;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.Audit;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask.Script
{
    public class MaskScriptOpenViewModel : ScriptOpenBaseViewModel
    {
       // private readonly IServiceLocator _serviceLocator;

        public MaskScriptOpenViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
			: base(contextCbiRepository, eventAggregator, serviceLocator)
        {
           // this._serviceLocator = serviceLocator;
        }

        protected override void RunScript()
        {
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
            IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
            log.Clear();

			try
			{
				object domainObject = base.GetCurrentDomainObject();
				string importMaskTable = "";
				if (domainObject is Customer)
				{						 
					importMaskTable = @"[CustomerMask]";
					string sql = InsertSqlFromScriptToTable(importMaskTable);
					alterADOProvider.ImportToMainDB(sql, importMaskTable, this._isClear);
				}
				else if (domainObject is Branch)
				{						
					importMaskTable = @"[BranchMask]";
					string sql = InsertSqlFromScriptToTable(importMaskTable);
					alterADOProvider.ImportToMainDB(sql, importMaskTable, this._isClear);
				}
				else if (domainObject is Inventor)
				{
					importMaskTable = @"[InventorMask]";
					string sql = InsertSqlFromScriptToTable(importMaskTable);
					alterADOProvider.ImportToAuditDB(sql, importMaskTable, this._isClear);
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