using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using NLog;
using Count4U.Model.Interface.Count4U;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using Count4U.Common.Extensions;
using Count4U.Model.Count4U;
using Count4U.Model.ServiceContract;
using Count4U.Model.Interface.Main;

namespace Count4U.Common.ViewModel.Script
{
    public abstract class ScriptOpenBaseViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IEventAggregator _eventAggregator;
		protected readonly IServiceLocator _serviceLocator;

        protected readonly DelegateCommand _runCommand;
        protected readonly DelegateCommand _cancelCommand;
        protected readonly DelegateCommand _browseCommand;

        protected readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;

        protected string _path;
        protected bool _isClear;
        protected bool _isClearTag;
        protected bool _isBusy;
        protected string _log;

        protected bool _isScriptWasLaunched;

        protected ScriptOpenBaseViewModel(
          IContextCBIRepository contextCbiRepository,
          IEventAggregator eventAggregator,
			IServiceLocator serviceLocator)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
            this._runCommand = new DelegateCommand(RunCommandExecuted, RunCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._browseCommand = new DelegateCommand(BrowseCommandExecuted);

            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();

            this._isScriptWasLaunched = false;
            this._isClearTag = false;
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);
            }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged(() => Log);
            }
        }

        public DelegateCommand RunCommand
        {
            get { return _runCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public DelegateCommand BrowseCommand
        {
            get { return _browseCommand; }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);

                this._runCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsClear
        {
            get { return _isClear; }
            set { _isClear = value; }
        }

        public bool IsClearTag
        {
            get { return _isClearTag; }
            set { _isClearTag = value; }
        }

      
        

        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return _fileChooseDilogRequest; }
        }

        #region Implementation of IChildWindowViewModel
        public object ResultData { get; set; }
        #endregion

        protected abstract void RunScript();

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();
            notification.Filter = "All files (*.*)|*.*";

            this._fileChooseDilogRequest.Raise(notification, r =>
            {
                if (r.IsOK)
                    this.Path = r.FileName;
            });
        }

        private void CancelCommandExecuted()
        {
            this.ResultData = new ScriptOpenData() { IsOk = this._isScriptWasLaunched };
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool RunCommandCanExecute()
        {
            if (String.IsNullOrWhiteSpace(this._path))
                return false;

            return File.Exists(this._path);
        }

        private void RunCommandExecuted()
        {
            if (!_runCommand.CanExecute()) return;

            this.Log = String.Empty;
            this.IsBusy = true;

            if (IsClearTag == true)
            {
                ICustomerReportRepository customerReportRepository = this._serviceLocator.GetInstance<ICustomerReportRepository>();
                customerReportRepository.DeleteAllNotTag();
            }

            Task.Factory.StartNew(Run).LogTaskFactoryExceptions("RunCommandExecuted");
          
        }

        private void Run()
        {
            try
            {
                RunScript();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Run", exc);
            }
            finally
            {
                Utils.RunOnUI(() => IsBusy = false);
                this._isScriptWasLaunched = true;
            }
        }

        #region Implementation of IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Path":
                        if (String.IsNullOrEmpty(this._path))
                            return null;

                        if (!File.Exists(this._path))
                            return Localization.Resources.Validation_File_Not_Exist;
                        break;
                }
                return String.Empty;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        #endregion


		public string InsertSqlFromScriptToTable(string importAdapterTable, List<string> exludeCode = null)
		{
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());
			string sql = "";
			foreach (String record in fileParser.GetRecords(this._path,
			Encoding.GetEncoding("windows-1255"), 0))
			{
				if (ExcludeRecordeCodeExistsInDB(record, exludeCode) == true) continue;
				if (record.ToUpper().Contains("DROP") == false
					&& record.ToUpper().Contains("DELETE") == false
					//&& record.ToUpper().Contains("UPDATE") == false
					&& record.ToUpper().Contains("ALTER") == false
					&& record.ToUpper().Contains("CREATE") == false
					&& record.ToUpper().Contains("SELECT") == false)
				{
					if (record.ToUpper().Contains("INSERT") == true)
					{
						if (record.Contains(importAdapterTable) == true)
						{
							sql = sql + record + Environment.NewLine;
						}

					}
				}
			}  //foreach record
			return sql;
		}

		private bool ExcludeRecordeCodeExistsInDB( string record, List<string> exludeCode)
		{
			bool ret = false;
			if (exludeCode != null)
			{
				foreach (string code in exludeCode)
				{
					if (record.ToUpper().Contains(code.ToUpper()))
					{
						return true;
					}
				}
			}
			return ret;
		}

    }
}