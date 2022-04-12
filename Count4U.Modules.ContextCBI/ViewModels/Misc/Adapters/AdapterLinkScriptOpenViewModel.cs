using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using System.Text;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters
{
    public class AdapterLinkScriptOpenViewModel : ScriptOpenBaseViewModel
    {
		//private readonly IServiceLocator _serviceLocator;

        private bool _toSetupDB;

        public AdapterLinkScriptOpenViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
			: base(contextCbiRepository, eventAggregator, serviceLocator)
        {
			//this._serviceLocator = serviceLocator;
        }

        public bool ToSetupDB
        {
            get { return _toSetupDB; }
            set
            {
                _toSetupDB = value;
                RaisePropertyChanged(() => ToSetupDB);
            }
        }

        protected override void RunScript()
		{
			this.Log = "";
			ILog log = this._serviceLocator.GetInstance<ILog>();
			IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			log.Clear();
			try
			{
				string importAdapterTable = @"[ImportAdapter]";
				string sql = InsertSqlFromScriptToTable(importAdapterTable);
				//alterADOProvider.ImportToMainDB(sql, importAdapterTable, this._isClear);
				alterADOProvider.ImportMainAdapterLink(sql, this._isClear, this.ToSetupDB);
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
		}	
    }
}