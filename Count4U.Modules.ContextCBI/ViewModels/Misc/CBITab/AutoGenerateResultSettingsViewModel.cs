using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using NLog;
using Count4U.Common.Extensions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Modules.ContextCBI.Events.Misc;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class AutoGenerateResultSettingsViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IStatusInventorRepository _statusInventorRepository;
        private readonly IDBSettings _dbSettings;
        private readonly IIniFileParser _iniFileParser;
        private readonly IReportRepository _reportRepository;
        private readonly IGenerateReportRepository _generateReportRepository;
		private readonly IReportIniRepository _reportIniRepository;
		private readonly IServiceLocator _serviceLocator;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
		private CancellationTokenSource _cancellationTokenSource;

        private StatusInventors _statuses;
        private StatusInventor _currentStatus;
		//private readonly DelegateCommand _okCommand;
		//private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _openConfigCommand;
        private readonly DelegateCommand _reloadConfigCommand;

        private readonly SendDataOffice _sendDataOffice;

        private DateTime _inventorDate;

        private bool _includeSdf;
        private bool _includePack;
        private bool _includeInventoryFiles;

        private string _zipPath;

        private bool _isBusy;
        private string _busyContent;

		//private readonly DelegateCommand _sendDataCommand;
		//private readonly DelegateCommand _printCommand;

        private readonly ObservableCollection<ReportPrintItemViewModel> _reports;
        private string _reportIniFile;

        private string _adapterName;
        private bool _isRunExportErp;

		private Customer _customer;
		private bool _isEditable;

		private readonly UICommandRepository _commandRepository;
		private readonly DelegateCommand _showConfigCommand;

		private bool _isShowConfig;

		public AutoGenerateResultSettingsViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IStatusInventorRepository statusInventorRepository,
            SendDataOffice sendDataOffice,
            IDBSettings dbSettings,
            IIniFileParser iniFileParser,
            IReportRepository reportRepository,
            IGenerateReportRepository generateReportRepository,
			IReportIniRepository reportIniRepository,
			IServiceLocator serviceLocator,
			UICommandRepository commandRepository,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository)
            : base(contextCBIRepository)
        {
            this._generateReportRepository = generateReportRepository;
			this._reportIniRepository = reportIniRepository;
			this._reportRepository = reportRepository;
            this._iniFileParser = iniFileParser;
            this._dbSettings = dbSettings;
            this._sendDataOffice = sendDataOffice;
            this._statusInventorRepository = statusInventorRepository;
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._commandRepository = commandRepository;
			//this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
			//this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
			//this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);
			//this._printCommand = new DelegateCommand(PrintCommandExecuted);
            this._includeSdf = false;
            this._includePack = false;
            this._includeInventoryFiles = true;
            this._reports = new ObservableCollection<ReportPrintItemViewModel>();
            this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
            this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);

			// Config Form path
			this._isEditable = true;
			this._showConfigCommand = _commandRepository.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted);
			this._isShowConfig = true;
        }


		public void SetIsEditable(bool isEditable)
		{
			this.IsEditable = isEditable;
		}

		public bool IsEditable
		{
			get { return _isEditable; }
			set
			{
				_isEditable = value;
				RaisePropertyChanged(() => IsEditable);
			}
		}

		public void SetCustomer(Customer customer)
		{
			this._customer = customer;
		}

		public DelegateCommand ShowConfigCommand
		{
			get { return this._showConfigCommand; }
		}

		private void ShowConfigCommandExecuted()
		{
			//this.SelectedExportPda = exportPdaModuleInfo;
			//if (exportPdaModuleInfo != null)
			//{
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() {  });
			//}
		}

		public void GotNewFofusConfig()
		{
			if (IsShowConfig == true)
			{
				ResultModuleInfo resultModuleInfo = new ResultModuleInfo();
				resultModuleInfo.ZipPath = ZipPath;
				string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
				//resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
				resultModuleInfo.ConfigPath = "";
				if (string.IsNullOrWhiteSpace(resultModuleInfo.ConfigPathWithFile) == false)
				{
					resultModuleInfo.ConfigPath = Path.GetDirectoryName(resultModuleInfo.ConfigPathWithFile);
				}

				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ResultModule = resultModuleInfo });
			}
		}

		public void LostFocusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ResultModule = null });
			}
		}

		public bool IsShowConfig
		{
			get { return this._isShowConfig; }
			set
			{
				this._isShowConfig = value;
				RaisePropertyChanged(() => IsShowConfig);
			}
		}

		public void SetIsShowConfig(bool isShowConfig)
		{
			this.IsShowConfig = isShowConfig;
		}

		//============================================================== 	Send to Office


      

        public bool IncludeSdf
        {
            get { return _includeSdf; }
            set
            {
                _includeSdf = value;
                RaisePropertyChanged(() => IncludeSdf);
            }
        }

        public bool IncludePack
        {
            get { return _includePack; }
            set
            {
                _includePack = value;
                RaisePropertyChanged(() => IncludePack);
            }
        }

        public bool IncludeInventoryFiles
        {
            get { return _includeInventoryFiles; }
            set
            {
                _includeInventoryFiles = value;
                RaisePropertyChanged(() => IncludeInventoryFiles);
            }
        }

        public string ZipPath
        {
            get { return _zipPath; }
            set
            {
                _zipPath = value;
                RaisePropertyChanged(() => ZipPath);

                //_sendDataCommand.RaiseCanExecuteChanged();
            }
        }

    

        public string Error { get; private set; }

        public ObservableCollection<ReportPrintItemViewModel> Reports
        {
            get { return this._reports; }
        }

        public DelegateCommand OpenConfigCommand
        {
            get { return _openConfigCommand; }
        }

        public DelegateCommand ReloadConfigCommand
        {
            get { return _reloadConfigCommand; }
        }

        public string AdapterName
        {
            get { return _adapterName; }
            set
            {
                _adapterName = value;
                RaisePropertyChanged(() => AdapterName);
            }
        }

       

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

             this.SetDefaultZipPath();
           
			// this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
			 string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			 this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
			this.BuildReports();
			this.BuildAdapter();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

   
        private bool IsOkZipPath()
        {
            if (String.IsNullOrWhiteSpace(_zipPath))
                return false;

            try
            {
                FileInfo fi = new FileInfo(_zipPath);
                if (!fi.Directory.Exists)
                    return false;

                if (String.IsNullOrWhiteSpace(fi.Name))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

     
        private void BuildZip(object state)
        {
			_cancellationTokenSource = new CancellationTokenSource();
            CultureInfo culture = state as CultureInfo;
            if (culture != null)
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            List<ReportInfo> reportInfo = new List<ReportInfo>();
            foreach (ReportPrintItemViewModel viewModel in this._reports)
            {
				ReportInfo item = new ReportInfo(this._reportRepository);
                item.ReportCode = viewModel.ReportCode;
                item.Format = viewModel.FileFormat;
				if (item.Format == ReportFileFormat.Excel2007) item.Format = ReportFileFormat.EXCELOPENXML;
                item.IncludeInZip = viewModel.Include;
                item.Print = viewModel.Print;
				item.RefillAlways = viewModel.RefillAlways;
                reportInfo.Add(item);
            }

            _sendDataOffice.BuildZip(
                cbiState: base.State, 
                updateStatus: UpdateStatus, 
                reportInfoList: reportInfo, 
                includeInventorSdf: _includeSdf, 
                includePack: _includePack,
                includeEndOfInventoryFiles: _includeInventoryFiles, 
                isRunExportErp: _isRunExportErp,
                resultPathZipPath: _zipPath);

			//Utils.RunOnUI(() =>
			//	{
			//		IsBusy = false;

			//		if (File.Exists(_zipPath))
			//		{
			//			Utils.OpenFileInExplorer(_zipPath);
			//		}
			//	});
        }

		private void UpdateStatus(string status)
		{
			Utils.RunOnUI(() => BusyContent = status);
		}

		public string BusyContent
		{
			get { return _busyContent; }
			set
			{
				_busyContent = value;
				RaisePropertyChanged(() => BusyContent);
			}
		}


   
        private void SetDefaultZipPath()
        {
            string sendDataFolderPath = UtilsPath.ZipOfficeFolder(_dbSettings);

            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string sendDataZipFileName = String.Format("{0}_{1}_{2}.zip",
                                                       base.CurrentCustomer.Code,
                                                       base.CurrentBranch.BranchCodeERP,
                                                       date);

            string sendDataZipFilePath = Path.Combine(sendDataFolderPath, sendDataZipFileName);

            this._zipPath = sendDataZipFilePath;
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "ZipPath":

                        if (!IsOkZipPath())
                            return Localization.Resources.ViewModel_InventorChangeStatus_InvalidPath;

                        break;
                }

                return String.Empty;
            }
        }

        private void BuildReports()
        {
            this._reports.Clear();

			if (File.Exists(this._reportIniFile) == false)
			{
			//	this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
			//	this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentInventor.Code);
				string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
			}
			if (File.Exists(this._reportIniFile) == false) return;

			const string Context = ReportIniProperty.ContextSendToOffice;
			const string ForCustomer = ReportIniProperty.ForCustomer;
			const string RefillAlways = ReportIniProperty.RefillAlways;
            const string FileFormatKey = ReportIniProperty.FileType;
            const string IncludeInZipKey = ReportIniProperty.IncludeInZip;
            const string PrintKey = ReportIniProperty.Print;

			foreach (IniFileData iniFileData in this._iniFileParser.Get(this._reportIniFile))
			{
				string reportCode = iniFileData.SectionName;

				bool isContextSendToOffice = false;
				if (iniFileData.Data.ContainsKey(Context))
				{
					isContextSendToOffice = iniFileData.Data[Context] == "1";
				}
				if (isContextSendToOffice == false) continue;

				if (String.IsNullOrWhiteSpace(reportCode) == true) continue;

				string fileTypeList = String.Empty;
				if (iniFileData.Data.ContainsKey(FileFormatKey))
				{
					fileTypeList = iniFileData.Data[FileFormatKey];
				}

				if (String.IsNullOrWhiteSpace(fileTypeList))
					continue;


				bool isInclude = false;
				if (iniFileData.Data.ContainsKey(IncludeInZipKey))
				{
					isInclude = iniFileData.Data[IncludeInZipKey] == "1";
				}


				bool isPrint = false;
				if (iniFileData.Data.ContainsKey(PrintKey))
				{
					isPrint = iniFileData.Data[PrintKey] == "1";
				}

				string forCustomer = String.Empty;
				if (iniFileData.Data.ContainsKey(ForCustomer))
				{
					forCustomer = iniFileData.Data[ForCustomer];
				}

				bool refillAlways = false;
				if (iniFileData.Data.ContainsKey(RefillAlways))
				{
					refillAlways = iniFileData.Data[RefillAlways] == "1";
				}
				

				bool addReport = false;
				if (string.IsNullOrWhiteSpace(forCustomer) == true)		  //нет фильтра по кастомеру
				{
					addReport = true;
				}
				else
				{
					addReport = false;
					foreach (string forCustomerCode in forCustomer.Split(',')) // список customerCode 
					{
						if (forCustomerCode == base.CurrentCustomer.Code) { addReport = true; }
					}
				}

				if (addReport == true)
				{
					foreach (string fileType in fileTypeList.Split(','))
					{
						string reportCodeBracket = String.Format("[{0}]", reportCode);
						Reports reports = _reportRepository.GetReportByCodeReport(reportCodeBracket);
						Count4U.GenerationReport.Report report = null;
						if (reports != null)
						{
							report = reports.FirstOrDefault();
						}

						if (report == null)
						{
							_logger.Warn("BuildReports: Report is missing{0}", reportCode);
							continue;
						}

						ReportPrintItemViewModel viewModel = new ReportPrintItemViewModel();
						viewModel.FileFormat = FromStringToReportFileFormat(fileType);
						viewModel.Include = isInclude;
						viewModel.Print = isPrint;
						viewModel.ReportCode = reportCodeBracket;
						viewModel.ReportName = this._generateReportRepository.GetLocalizedReportName(report);
						viewModel.Landscape = report.Landscape;
						viewModel.RefillAlways = refillAlways;
						this._reports.Add(viewModel);
					}
				}
			}
        }

        private string FromStringToReportFileFormat(string input)
        {
            input = input.Trim().ToLower();

            switch (input)
            {
                case "pdf":
                    return ReportFileFormat.Pdf;
                case "excel":
                    return ReportFileFormat.Excel;
                case "word":
                    return ReportFileFormat.Word;
				case "excel2007":
					return ReportFileFormat.Excel2007;
            }

            return input;
        }

		//private void CopyReportTemplateIniFile()
		//{
		//	try
		//	{
		//		string source = UtilsPath.ExportReportTemplateIniFile(this._dbSettings);

		//		if (File.Exists(source) == false) return;

		//		string inventorCode = base.CurrentInventor.Code;
		//		if (String.IsNullOrWhiteSpace(inventorCode)) return;

		//		this._reportIniFile = Common.Helpers.UtilsPath.ExportReportIniFile(this._dbSettings, inventorCode);

		//		if (File.Exists(this._reportIniFile) == true) return;

		//		File.Copy(source, this._reportIniFile);
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.Info("CopyTemplateIniFile", exc);
		//	}
		//}

        private bool OpenConfigCommandCanExecute()
        {
            return File.Exists(this._reportIniFile);
        }

        private void OpenConfigCommandExecuted()
        {
            Utils.OpenFileInExplorer(this._reportIniFile);
        }

        private bool ReloadConfigCommandCanExecute()
        {
            return File.Exists(this._reportIniFile);
        }

        private void ReloadConfigCommandExecuted()
        {
			//this.CopyReportTemplateIniFile();
			string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
			//this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
            this.BuildReports();
        }

        private void BuildAdapter()
        {
            try
            {
                string name = base.CurrentInventor.ExportERPAdapterCode;

                if (String.IsNullOrEmpty(name))
                {
                    AdapterName = Localization.Resources.ViewModel_InventorStatusChange_msgAdapterNotSet;
                    _isRunExportErp = false;
                }
                else
                {
					string param = CustomerParamIsExcludeNotExistingInCatalog(AdapterName);
					AdapterName = String.Format("[{0}]", name + param);
                    _isRunExportErp = true;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildAdapter", exc);
            }
        }

		private string CustomerParamIsExcludeNotExistingInCatalog(string adapterName)
		{
			bool isExcludeNotExistingInCatalog = false;
			string currentCustomerCode = base.CurrentCustomer.Code;
			string keyCode = currentCustomerCode + "|" + adapterName;
			ICustomerConfigRepository customerConfigRepository = this._serviceLocator.GetInstance<ICustomerConfigRepository>();
			Dictionary<string, CustomerConfig> configDictionary = customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			if (configDictionary != null)
			{
				isExcludeNotExistingInCatalog = configDictionary.GetBoolValue(isExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
			}
			if (isExcludeNotExistingInCatalog == true) return ": Without NotExistingInCatalog";
			else return ": With NotExistingInCatalog";
			
		}


		public void ClearIturAnalysis(object param)
		{
			string dbPath = base.GetDbPath;
			if (String.IsNullOrWhiteSpace(dbPath) == false)
			{
				//this._iturAnalyzesSourceRepository.ClearIturAnalyzes(dbPath);
				this._iturAnalyzesSourceRepository.AlterTableIturAnalyzes(dbPath);
			}
		}
	}


	
	}







