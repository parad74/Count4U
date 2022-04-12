using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Events;
using NLog;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Common.UserSettings;
using System.Collections.ObjectModel;
using Count4U.Modules.ContextCBI.ViewModels;
using System.IO;
using Count4U.Common.Services.Ini;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Threading;
using Count4U.Model.SelectionParams;
using System.Globalization;
using Count4U.Common.Extensions;
using Count4U.Modules.Audit.Events;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturDeleteViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
		private readonly IReportRepository _reportRepository;
		private readonly IGenerateReportRepository _generateReportRepository;
		private readonly IReportIniRepository _reportIniRepository;
		private readonly IServiceLocator _serviceLocator;

		//private readonly DelegateCommand _okCommand;
		private readonly DelegateCommand _clearCommand;
        private readonly DelegateCommand _cancelCommand;
		//private readonly DelegateCommand _openConfigCommand;
		//private readonly DelegateCommand _reloadConfigCommand;
		//private readonly DelegateCommand _sendDataCommand;

		private readonly ObservableCollection<ReportPrintItemViewModel> _reports;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;

        private string _textValue;
		private string _countItems;
        private string _totalIturs;
		private string _reportIniFile;

        private string _numberPrefix;

		private readonly IDBSettings _dbSettings;
		private readonly IIniFileParser _iniFileParser;
		private readonly SendDataOffice _sendDataOffice;

		private string _path;
		private bool _isBusy;
		private string _busyContent;

		//private string _saveReportFolderPath;

		public IturDeleteViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
			IIturRepository iturRepository,
			SendDataOffice sendDataOffice,
			IDBSettings dbSettings,
			IIniFileParser iniFileParser,
			IUserSettingsManager userSettingsManager,
			IReportRepository reportRepository,
			 IGenerateReportRepository generateReportRepository,
			IReportIniRepository reportIniRepository,
			IServiceLocator serviceLocator,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository)
            : base(contextCbiRepository)
        {
			this._iniFileParser = iniFileParser;
			this._dbSettings = dbSettings;
			this._reportRepository = reportRepository;
			this._generateReportRepository = generateReportRepository;
			this._reportIniRepository = reportIniRepository;
			this._iturRepository = iturRepository;
			this._eventAggregator = eventAggregator;
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._sendDataOffice = sendDataOffice;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            //this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._clearCommand = new DelegateCommand(ClearCommandExecuted, ClearCommandCanExecuted);
			//this._reports = new ObservableCollection<ReportPrintItemViewModel>();
			//this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			//this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);
			//this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);

        }

        public string TextValue
        {
            get { return _textValue; }
            set
            {
                this._textValue = value;
                RaisePropertyChanged(() => TextValue);

               // List<int> res = CommaDashStringParser.Parse(this._textValue);
                //TotalIturs = res == null ? "0" : res.Count.ToString();

				List<string> res = GetIturCodeList();
				CountIturs = res == null ? "0" : res.Count.ToString();
				TotalIturs =  res.JoinRecord(",");

                _clearCommand.RaiseCanExecuteChanged();
            }
        }

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				RaisePropertyChanged(() => IsBusy);
			}
		}

	

        public string TotalIturs
        {
            get { return this._totalIturs; }
            set
            {
                this._totalIturs = value;
                RaisePropertyChanged(() => TotalIturs);
            }
        }

		public string CountIturs
		{
			get { return this._countItems; }
			set
			{
				this._countItems = value;
				RaisePropertyChanged(() => CountIturs);
			}
		}


	

        public string NumberPrefix
        {
            get { return _numberPrefix; }
            set
            {
                this._numberPrefix = value;
                RaisePropertyChanged(()=>NumberPrefix);
				List<string> res = GetIturCodeList();
				CountIturs = res == null ? "0" : res.Count.ToString();
				TotalIturs = res.JoinRecord(",");

                _clearCommand.RaiseCanExecuteChanged();
            }
        }



		public DelegateCommand ClearCommand
        {
            get { return _clearCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public object ResultData { get; set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }       

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "TextValue":
                        {
                            if (this.IsTextValid() == false)
                            {
                                return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
                            }
                        }
                        break;

                    case "NumberPrefix":
                        {
                            if (String.IsNullOrWhiteSpace(_numberPrefix))
                                return String.Empty;

                            int bit = this._numberPrefix.PrefixValidate();
                            if (bit != 0)
                            {
                                return IturValidate.Bit2PrefixErrorMessage(bit);
                            }
                        }
                        //if (IsPrefixValid() == false)
                        //{
                        //    return "Prefix is not empty numeric string 4 characters maximum";
                        //}

                        break;
                }

                return string.Empty;
            }
        }

        public string Error { get; private set; }       

        private bool IsTextValid()
        {
            return CommaDashStringParser.IsValid(this._textValue);
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

		private List<string> GetIturCodeList()
		{
			List<string> iturCodes = new List<string>();
			if (string.IsNullOrWhiteSpace(this._textValue) == true) return iturCodes;
			try
			{
				List<int> viewCodes = CommaDashStringParser.Parse(this._textValue.Trim()).ToList();
				List<string> dbCodes = this._iturRepository.GetIturCodes(base.GetDbPath).ToList();

				foreach (int viewCode in viewCodes)
				{
					string sufix = UtilsItur.SuffixFromNumber(viewCode);
					string prefix = UtilsItur.PrefixFromString(_numberPrefix);
					string code = UtilsItur.CodeFromPrefixAndSuffix(prefix, sufix);

					if (dbCodes.Contains(code) == true)
					{
						iturCodes.Add(code);
					}
				}
			}
			catch {  }
			return iturCodes;
		}

		//TO DO
        private void ClearCommandExecuted()
        {
			if (!ClearCommand.CanExecute())
                return;

            try
            {
				using (new CursorWait())
				{
					//List<int> viewCodes = CommaDashStringParser.Parse(this._textValue.Trim()).ToList();
					//List<string> dbCodes = _iturRepository.GetIturCodes(base.GetDbPath).ToList();

					//List<string> iturCodes = new List<string>();

					//foreach (int viewCode in viewCodes)
					//{
					//	string sufix = UtilsItur.SuffixFromNumber(viewCode);
					//	string prefix = UtilsItur.PrefixFromString(_numberPrefix);
					//	string code = UtilsItur.CodeFromPrefixAndSuffix(prefix, sufix);

					//	if (dbCodes.Contains(code))
					//	{
					//		iturCodes.Add(code);
					//	}
					//}
					List<string> res = GetIturCodeList();
					foreach (string iturCode in res)
					{
						Itur itur = this._iturRepository.GetIturByCode(iturCode,  base.GetDbPath);
						if (itur != null)
						{
							this._iturRepository.ClearIturHierarchical(itur, base.GetDbPath);
						}
					}
					this._eventAggregator.GetEvent<IturDeletedEvent>().Publish(true);
					this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("DeleteCommandExecuted", exc);
            }
        }

        private bool ClearCommandCanExecuted()
        {
			if (string.IsNullOrWhiteSpace(CountIturs) == true) return false;
			if (CountIturs == "0") return false;
			return true;
        }

    }
}