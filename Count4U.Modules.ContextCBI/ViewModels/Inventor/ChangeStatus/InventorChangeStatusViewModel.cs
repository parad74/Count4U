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
using Count4U.Common.Web;
using Count4U.Common.Constants;
using Count4U.Common.UserSettings;
using Count4U.Model.Transfer;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorChangeStatusViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IStatusInventorRepository _statusInventorRepository;
        private readonly IDBSettings _dbSettings;
		private readonly IZip _zip;
        private readonly IIniFileParser _iniFileParser;
        private readonly IReportRepository _reportRepository;
        private readonly IGenerateReportRepository _generateReportRepository;
		private readonly IReportIniRepository _reportIniRepository;
		private readonly IServiceLocator _serviceLocator;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
		protected readonly IUserSettingsManager _userSettingsManager;

        private StatusInventors _statuses;
        private StatusInventor _currentStatus;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _openConfigCommand;
        private readonly DelegateCommand _reloadConfigCommand;
		protected readonly DelegateCommand _openFtpCommand;

        private readonly SendDataOffice _sendDataOffice;
	//	private readonly ToFtpViewModel _toFTPViewModel;
		private readonly FtpFolderProFile _ftpFolderFile;
		protected IConnectionDB _connection;
		


        private DateTime _inventorDate;

        private bool _includeSdf;
		private bool _uploadFilesToFtp;
        private bool _includePack;
        private bool _includeInventoryFiles;

        private string _zipPath;
		private string _ftpPath;
		private string _ftpFolder;
		private string _ftpFileName;

        private bool _isBusy;
        private string _busyContent;
		private bool _isSentOneTime;
	

		//private double _progress;
		//private string _progressState;
		//private string _progressFile;

		private readonly DelegateCommand _busyCancelCommand;
		private CancellationTokenSource _cancellationTokenSource;

        private readonly DelegateCommand _sendDataCommand;
        private readonly DelegateCommand _printCommand;

        private readonly ObservableCollection<ReportPrintItemViewModel> _reports;
        private string _reportIniFile;

        private string _adapterName;
        private bool _isRunExportErp;

		private Customer _customer;
		private bool _isEditable;

		private string _host { get; set; }
		private bool _canOpentFtp { get; set; }
		

        public InventorChangeStatusViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IStatusInventorRepository statusInventorRepository,
            SendDataOffice sendDataOffice,
            IDBSettings dbSettings,
			IZip zip,
            IIniFileParser iniFileParser,
            IReportRepository reportRepository,
            IGenerateReportRepository generateReportRepository,
			IReportIniRepository reportIniRepository,
			IServiceLocator serviceLocator,
			IUserSettingsManager userSettingsManager,
			//ToFtpViewModel toFtpViewModel,
			FtpFolderProFile ftpFolderFile ,
			IConnectionDB connection,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository)
            : base(contextCBIRepository)
        {
			this._userSettingsManager = userSettingsManager;
            this._generateReportRepository = generateReportRepository;
			this._reportIniRepository = reportIniRepository;
			this._reportRepository = reportRepository;
            this._iniFileParser = iniFileParser;
            this._dbSettings = dbSettings;
			 this._zip = zip;
            this._sendDataOffice = sendDataOffice;
            this._statusInventorRepository = statusInventorRepository;
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._ftpFolderFile = ftpFolderFile;
			this._connection = connection;  
		//	this._toFTPViewModel = toFtpViewModel;
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);
            this._printCommand = new DelegateCommand(PrintCommandExecuted);
			this._busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted);
            this._includeSdf = false;
			this._uploadFilesToFtp = false;
            this._includePack = false;
            this._includeInventoryFiles = true;
			bool _enableSsl = false;
			this._host = _userSettingsManager.HostFtpGet(out _enableSsl);
            this._reports = new ObservableCollection<ReportPrintItemViewModel>();
            this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
            this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);
			this._openFtpCommand = new DelegateCommand(this.OpenFtpCommandExecuted, this.OpenFtpCommandCanExecute);
			this._canOpentFtp = false;
			this._isEditable = true;
			this._isSentOneTime = false;
			
        }

		//protected ToFtpViewModel ToFTPViewModel
		//{
		//	get { return _toFTPViewModel; }
		//}

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

        public string TextStatus
        {
            get { return string.Format(Localization.Resources.ViewModel_InventorStatusChange_Status, CurrentStatus == null ? String.Empty : CurrentStatus.Code); }
        }

		private string _uploadFilesToFtpStatus;
		public string UploadFilesToFtpStatus
        {
		get { return _uploadFilesToFtpStatus; }
			set
			{
				_uploadFilesToFtpStatus = value;
				RaisePropertyChanged(() => UploadFilesToFtpStatus);
			}
        }

		private string _uploadFilesToFtpErrorStatus;
		public string UploadFilesToFtpErrorStatus
		{
			get { return _uploadFilesToFtpErrorStatus; }
			set
			{
				_uploadFilesToFtpErrorStatus = value;
				RaisePropertyChanged(() => UploadFilesToFtpErrorStatus);
			}
		}

		private bool _visibilityUploadFilesToFtpStatus;
		public bool VisibilityUploadFilesToFtpStatus
		{
			get { return _visibilityUploadFilesToFtpStatus; }
			set
			{
				_visibilityUploadFilesToFtpStatus = value;
				RaisePropertyChanged(() => VisibilityUploadFilesToFtpStatus);
			}
		}

		private bool _visibilityUploadFilesToFtpErrorStatus;
		public bool VisibilityUploadFilesToFtpErrorStatus
		{
			get { return _visibilityUploadFilesToFtpErrorStatus; }
			set
			{
				_visibilityUploadFilesToFtpErrorStatus = value;
				RaisePropertyChanged(() => VisibilityUploadFilesToFtpErrorStatus);
			}
		}

		

		private void RfreshFtpStatus(string status = "")
		{
			if (string.IsNullOrWhiteSpace(status) == true)
			{
				this.UploadFilesToFtpStatus = "";
				this.VisibilityUploadFilesToFtpStatus = false;
			}
			else
			{
				this.UploadFilesToFtpStatus = status;
				this.VisibilityUploadFilesToFtpStatus = true;
			}
			this.UploadFilesToFtpErrorStatus = "";
			this.VisibilityUploadFilesToFtpErrorStatus = false;

			RaisePropertyChanged(() => UploadFilesToFtpStatus);
			RaisePropertyChanged(() => UploadFilesToFtpErrorStatus);
			RaisePropertyChanged(() => VisibilityUploadFilesToFtpStatus);
			RaisePropertyChanged(() => VisibilityUploadFilesToFtpErrorStatus);
		}
		private void RfreshFtpErrorStatus(string errorStatus = "")
		{

			this.UploadFilesToFtpStatus = "";
			this.VisibilityUploadFilesToFtpStatus = false;
		
			if (string.IsNullOrWhiteSpace(errorStatus) == true)
			{
				this.UploadFilesToFtpErrorStatus = "";
				this.VisibilityUploadFilesToFtpErrorStatus = false;
			}
			else
			{
				this.UploadFilesToFtpErrorStatus = errorStatus;
				this.VisibilityUploadFilesToFtpErrorStatus = true;
			}
			RaisePropertyChanged(() => UploadFilesToFtpStatus);
			RaisePropertyChanged(() => UploadFilesToFtpErrorStatus);
			RaisePropertyChanged(() => VisibilityUploadFilesToFtpStatus);
			RaisePropertyChanged(() => VisibilityUploadFilesToFtpErrorStatus);
		}

        public StatusInventors Statuses
        {
            get { return _statuses; }
        }

		public DelegateCommand OpenFtpCommand
		{
			get { return _openFtpCommand; }
		}


		private void OpenFtpCommandExecuted()
		{
			if (String.IsNullOrEmpty(this.FtpPath))
				return;

			string root = this._connection.RootCount4UFolderFtp(); //Count4U 
			Utils.OpenFtpFolderInExplorer(_host, root + @"\" + this._ftpFolder);
		}

		private bool OpenFtpCommandCanExecute()
		{
			if (String.IsNullOrEmpty(this.FtpPath))
				return false;
			return this._canOpentFtp;
		}

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public StatusInventor CurrentStatus
        {
            get { return this._currentStatus; }
            set
            {
                this._currentStatus = value;
                this.RaisePropertyChanged(() => this.CurrentStatus);
                this.RaisePropertyChanged(() => this.TextStatus);
                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime InventorDate
        {
            get { return _inventorDate; }
            set
            {
                _inventorDate = value;
                RaisePropertyChanged(() => InventorDate);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
			//	Progress = 0;
            }
        }



		public bool IsSentOneTime
        {
			get { return _isSentOneTime; }
            set
            {
				_isSentOneTime = value;
				RaisePropertyChanged(() => IsSentOneTime);
				this._sendDataCommand.RaiseCanExecuteChanged();
			//	Progress = 0;
            }
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

		public DelegateCommand BusyCancelCommand
		{
			get { return this._busyCancelCommand; }
		}

        public DelegateCommand SendDataCommand
        {
            get { return _sendDataCommand; }
        }

        public DelegateCommand PrintCommand
        {
            get { return _printCommand; }
        }

        public bool IncludeSdf
        {
            get { return _includeSdf; }
            set
            {
                _includeSdf = value;
                RaisePropertyChanged(() => IncludeSdf);
            }
        }


		public bool UploadFilesToFtp
        {
			get { return _uploadFilesToFtp; }
            set
            {
				_uploadFilesToFtp = value;
				this._userSettingsManager.SendToFtpOfficeSet(value);
				RaisePropertyChanged(() => UploadFilesToFtp);
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

                _sendDataCommand.RaiseCanExecuteChanged();
            }
        }

		public string FtpPath
		{
			get { return _ftpPath; }
			set
			{
				_ftpPath = value;
				RaisePropertyChanged(() => FtpPath);

				_sendDataCommand.RaiseCanExecuteChanged();
			}
		}

        public string CurrentCustomerString
        {
            get
            {
                Customer customer = base.CurrentCustomer;
                return String.Format("{0} [{1}]", customer.Name, customer.Code);
            }
        }

        public string CurrentBranchString
        {
            get
            {
                Branch branch = base.CurrentBranch;
                return String.Format("{0} [{1}]", branch.Name, branch.BranchCodeERP);
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

            this._statuses = this._statusInventorRepository.GetStatuses();

            Inventor inventor = base.CurrentInventor;
			this._canOpentFtp = false;
			this._openFtpCommand.RaiseCanExecuteChanged();
            this._currentStatus = Statuses.FirstOrDefault(r => r.Code == inventor.StatusCode);
            this._inventorDate = inventor.InventorDate;
			this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(inventor.Code);
			this._uploadFilesToFtp = this._userSettingsManager.SendToFtpOfficeGet();
			this.RfreshFtpStatus();
			this.InitFromState(base.State, this._reportIniFile);
			  
			this.BuildAdapter(base.State.CurrentInventor.ExportERPAdapterCode, base.State.CurrentCustomer.Code);

		  //	this._toFTPViewModel.InitCBIState(base.State);
			
        }

		//для инициализации из вне 
		public void InitFromState(CBIState state, string reportIniFile)
		{
			this.SetDefaultZipPath(state.CurrentCustomer, state.CurrentBranch, state.CurrentBranch.BranchCodeERP);
			this.BuildReports(reportIniFile, state.CurrentCustomer.Code);
		}

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return _currentStatus != null;
        }

        private void OkCommandExecuted()
        {
            using (new CursorWait())
            {
                AuditConfig config = base.GetAuditConfigByCurrentContext();
                //                config.StatusInventorCode = CurrentStatus.Code;
                //                config.CreateDate = this.CreateDate;
                //                config.InventorDate = this.InventorDate;
                //                base.ContextCBIRepository.Update(config, CBIContext.History);

                Inventor inventor = base.ContextCBIRepository.GetInventorByCode(config.InventorCode);
                inventor.InventorDate = this.InventorDate;
                inventor.StatusCode = _currentStatus.Code;
                base.ContextCBIRepository.Update(inventor);

                this._eventAggregator.GetEvent<InventorEditedEvent>().Publish(inventor);
                this._eventAggregator.GetEvent<InventorStatusChangedEvent>().Publish(inventor);

                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
            }
        }

        private void PrintCommandExecuted()
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


        private bool SendDataCommandCanExecute()
        {
			if (this.IsSentOneTime == true) return false;
			if (this.IsBusy == true) return false;
				
            return IsOkZipPath();
        }

	

		public void SendDataToOfficeWitoutGUICommandExecuted(string resultFileZipPath, CBIState state)
		{
			if (this.IsBusy == false)
			{
				this.IsBusy = true;

				Task.Factory.StartNew(() =>
				{
					this.BuildZipWithoutGUI(Thread.CurrentThread.CurrentUICulture,UpdateStatus, resultFileZipPath, state);

				}).LogTaskFactoryExceptions("SendDataToOfficeWitoutGUICommandExecuted");


				Utils.RunOnUI(() =>
				{
					IsBusy = false;

					if (File.Exists(resultFileZipPath))
					{
						Utils.OpenFileInExplorer(resultFileZipPath);
					}

				
				});
			}
		}

		//private void UpdateProgress(double progress, string processedTotal, string fileName)
		//{
		//	Utils.RunOnUI(() =>
		//	{
		//		//Progress = progress;
		//		//ProgressFile = fileName;
		//		//ProgressState = processedTotal;
		//	});
		//}

		//public double Progress
		//{
		//	get { return this._progress; }
		//	set
		//	{
		//		this._progress = value;
		//		RaisePropertyChanged(() => Progress);
		//	}
		//}

		//public string ProgressFile
		//{
		//	get { return this._progressFile; }
		//	set
		//	{
		//		this._progressFile = value;
		//		RaisePropertyChanged(() => ProgressFile);
		//	}
		//}

		//public string ProgressState
		//{
		//	get { return this._progressState; }
		//	set
		//	{
		//		this._progressState = value;
		//		RaisePropertyChanged(() => ProgressState);
		//	}

		//}

		private void BusyCancelCommandExecuted()
		{
			this._cancellationTokenSource.Cancel();

			IsBusy = false;
		}

		private void SendDataCommandExecuted()
		{
			if (this.IsBusy == false)
			{
				this.IsSentOneTime = true;

				_cancellationTokenSource = new CancellationTokenSource();
				this.IsBusy = true;
			//	BusyText = String.Empty;	 //todo

				//Task.Factory.StartNew(BuildZip, Thread.CurrentThread.CurrentUICulture).LogTaskFactoryExceptions("SendDataCommandExecuted");
				Task.Factory.StartNew(() =>
				{
					if (_cancellationTokenSource.IsCancellationRequested)
					{
						this.IsBusy = false;
						return;
					}

					string zipFilePath = BuildZip(Thread.CurrentThread.CurrentUICulture);
					
					//if (_cancellationTokenSource.IsCancellationRequested)
					//{
					//	this.IsBusy = false;
					//	return;
					//}
					//else if (string.IsNullOrWhiteSpace(zipFilePath) == true)
					//{
					//	this.IsBusy = false;
					//	return;
					//}
					//else
					//{
					if (UploadFilesToFtp == true)
					{
						UpdateStatus(Localization.Resources.View_InventorStatusChange_UploadingToFtpWait);
					}

						Utils.RunOnUI(() =>
						{
							if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested == false)
							{
						
								if (string.IsNullOrWhiteSpace(zipFilePath) == false)
								{
									if (File.Exists(zipFilePath))
									{
										Utils.OpenFileInExplorer(zipFilePath);
									}



									//ftp
									if (UploadFilesToFtp == true)
									{
										string messageCreateFolder = "";
										
										try
										{
											//FTP
											//mINV\Customer\<CustomerCode>
											//rootFonderOnFtp = "mINV" 
											string rootCount4UFolderFtp = this._connection.RootCount4UFolderFtp(); //Count4U 
											//string rootFolderFtp = this._connection.RootFolderFtp(); //mINV 

											this._ftpFolderFile.CreatePathOnFtp(rootCount4UFolderFtp, this._ftpFolder, ref messageCreateFolder);

											string localfolder = Path.GetDirectoryName(this._zipPath);
											string fromRoot = rootCount4UFolderFtp + @"\" + this._ftpFolder;
											string fileName = Path.GetFileName(this._zipPath);
											this._ftpFolderFile.CopyFileToFtp(localfolder, fromRoot, fileName);
											this._canOpentFtp = true;
											this._openFtpCommand.RaiseCanExecuteChanged();
										}
										catch (Exception exc)
										{
											this._canOpentFtp = false;
											this.RfreshFtpErrorStatus(Localization.Resources.View_InventorStatusChange_ErrorWithFtp);
											_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
										}

										if (this._canOpentFtp == true)
										{
											this.RfreshFtpStatus(Localization.Resources.View_InventorStatusChange_IploadedSuccessfullyFtp);

											//string root = this._connection.RootCount4UFolderFtp(); //Count4U 
											//Utils.OpenFtpFolderInExplorer(_host, root + @"\" + this._ftpFolder);

										}
									}
								}
							}
						});
						IsBusy = false;
					//}
				}).LogTaskFactoryExceptions("SendDataCommandExecuted");
			}
		}

		public string BuildZip(object state)
		{
			string resultZipFile = String.Empty;
			this._canOpentFtp = false;
			this._openFtpCommand.RaiseCanExecuteChanged();
			CultureInfo culture = state as CultureInfo;
			if (culture != null)
			{
				Thread.CurrentThread.CurrentUICulture = culture;
			}

			List<ReportInfo> reportInfo = new List<ReportInfo>();
			foreach (ReportPrintItemViewModel viewModel in this._reports)
			{
				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return resultZipFile;
				}
				ReportInfo item = new ReportInfo(this._reportRepository);
				item.ReportCode = viewModel.ReportCode;
				item.Format = viewModel.FileFormat;
				if (item.Format == ReportFileFormat.Excel2007) item.Format = ReportFileFormat.EXCELOPENXML;
				item.IncludeInZip = viewModel.Include;
				item.Print = viewModel.Print;
				item.RefillAlways = viewModel.RefillAlways;
				reportInfo.Add(item);
			}

			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return resultZipFile;
			}

			//_sendDataOffice.BuildZip(
				resultZipFile = this.BuildZipLocal(
				cbiState: base.State,
				updateStatus: UpdateStatus,
				reportInfoList: reportInfo,
				includeInventorSdf: _includeSdf,
				includePack: _includePack,
				includeEndOfInventoryFiles: _includeInventoryFiles,
				isRunExportErp: _isRunExportErp,
				resultPathZipPath: _zipPath);

			//if (_cancellationTokenSource.IsCancellationRequested)
			//{
			//	return resultZipFile;
			//}

			//resultZipFile = _zipPath;

			//if (UploadFilesToFtp == true)
			//{
			//	string messageCreateFolder = "";

			//	try
			//	{
			//		//FTP
			//		//mINV\Customer\<CustomerCode>
			//		//rootFonderOnFtp = "mINV" 
			//		string rootCount4UFolderFtp = this._connection.RootCount4UFolderFtp(); //Count4U 
			//		//string rootFolderFtp = this._connection.RootFolderFtp(); //mINV 

			//		this._ftpFolderFile.CreatePathOnFtp(rootCount4UFolderFtp, this._ftpFolder, ref messageCreateFolder);

			//		string localfolder = Path.GetDirectoryName(this._zipPath);
			//		string fromRoot = rootCount4UFolderFtp + @"\" + this._ftpFolder;
			//		string fileName = Path.GetFileName(this._zipPath);
			//		this._ftpFolderFile.CopyFileToFtp(localfolder, fromRoot, fileName);
			//		this._canOpentFtp = true;
			//		this._openFtpCommand.RaiseCanExecuteChanged();
			//	}
			//	catch (Exception exc)
			//	{
			//		_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
			//	}
			//}

		

			//Utils.RunOnUI(() =>
			//	{
			//		IsBusy = false;

			//		if (File.Exists(_zipPath))
			//		{
			//			Utils.OpenFileInExplorer(_zipPath);
			//		}
			//		if (this._canOpentFtp == true)
			//		{
			//			string root = this._connection.RootCount4UFolderFtp(); //Count4U 
			//			Utils.OpenFtpFolderInExplorer(_host, root + @"\" + this._ftpFolder);
			//		}
			//});

			return resultZipFile;
					
		}

		public string BuildZipLocal(
		   CBIState cbiState,
		   Action<string> updateStatus,
		   List<ReportInfo> reportInfoList,
		   bool includeInventorSdf,
		   bool includePack,
		   bool includeEndOfInventoryFiles,
		   bool isRunExportErp,
		   string resultPathZipPath)
		{
			//if (this._isBusy == true) return "";
			//cancellationTokenSource = new CancellationTokenSource();

			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return "";
			}

			this._isBusy = true;
			const string zipRoot = "/";
			reportInfoList = reportInfoList.Where(x => x != null).ToList();

			foreach (ReportInfo reportInfo in reportInfoList)
			{
				reportInfo.BuildReportArgs(cbiState);
			}

			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return "";
			}

			try
			{
				List<ZipRelativePath> filesToZip = new List<ZipRelativePath>();

				//print reports
				_sendDataOffice.BuildAndPrintReports(cbiState, updateStatus, reportInfoList);

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				//build reports files
				List<string> reportFiles = _sendDataOffice.BuildAndSaveReports(cbiState, updateStatus, reportInfoList, "BuildZip");
				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}


				if (reportFiles != null)
				{
					foreach (string reportPath in reportFiles)
					{
						filesToZip.Add(new ZipRelativePath() { Path = reportPath, RelativePath = zipRoot });
					}
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				//include .sdf
				if (includeInventorSdf)
				{
					updateStatus(Localization.Resources.View_SendDataOffice_BuildingInventorSdf);

					string inventorSdfFullPath = _sendDataOffice.BuildInventorSdfPath(cbiState);
					if (File.Exists(inventorSdfFullPath))
					{
						filesToZip.Add(new ZipRelativePath() { Path = inventorSdfFullPath, RelativePath = zipRoot });
					}
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				//include pack
				if (includePack)
				{
					updateStatus(Localization.Resources.View_SendDataOffice_BuildingPackZipFile);

					string pack = _sendDataOffice.BuildPackFile(cbiState);

					if (_cancellationTokenSource.IsCancellationRequested)
					{
						return "";
					}

					filesToZip.Add(new ZipRelativePath() { Path = pack, RelativePath = zipRoot });
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				if (includeEndOfInventoryFiles)
				{
					updateStatus(Localization.Resources.View_SendDataOffice_BuildingEndInventor);

					if (isRunExportErp)
					{
						_sendDataOffice.RunExport(cbiState);	    // TODO       
					}

					if (_cancellationTokenSource.IsCancellationRequested)
					{
						return "";
					}

					List<string> exportFiles = _sendDataOffice.BuildExportFilesPath(cbiState);
					if (exportFiles != null)
					{
						foreach (string exportFile in exportFiles)
						{
							if (_cancellationTokenSource.IsCancellationRequested)
							{
								return "";
							}
							filesToZip.Add(new ZipRelativePath() { Path = exportFile, RelativePath = zipRoot });
						}
					}
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				//building zip
				updateStatus(Localization.Resources.View_SendDataOffice_BuildingZip);
				if (File.Exists(resultPathZipPath))
				{
					_logger.Info(String.Format("Removed previous zip: {0}", resultPathZipPath));
					File.Delete(resultPathZipPath);
				}

				if (UploadFilesToFtp == true)
				{
					updateStatus(Localization.Resources.View_Upload_Zip_Files_To_Ftp);			 //?
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				_zip.CreateZip(filesToZip, resultPathZipPath);

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return "";
				}

				
			}
			catch (Exception exc)
			{
				//this._isBusy = false;
				_logger.ErrorException("BuildZip", exc);
				return "";
			}
			
			//this._isBusy = false;
			
			return resultPathZipPath;
		}


		public void BuildZipWithoutGUI(
				object cultureInfo,
				 Action<string> updateStatus,
				string resultFileZipPath, 
				CBIState state )
		{
			bool includeInventorDBSdf = false;
			bool includePackOf = false;
			bool includeEndOfInventorFiles = true;
			bool isRunExportErpAdapter = false; //всегда ExportERP до того как делаем правильно

			CultureInfo culture = cultureInfo as CultureInfo;
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
				item.RefillAlways = true;//viewModel.RefillAlways;
				reportInfo.Add(item);
			}

		
			_sendDataOffice.BuildZip(
				cbiState: state,
				updateStatus: UpdateStatus,
				reportInfoList: reportInfo,
				includeInventorSdf: includeInventorDBSdf,
				includePack: includePackOf,
				includeEndOfInventoryFiles: includeEndOfInventorFiles,
				isRunExportErp: false,		   //isRunExportErpAdapter
				resultPathZipPath: resultFileZipPath);

				Utils.RunOnUI(() =>
						{
							//this.IsBusy = false;

							if (File.Exists(resultFileZipPath))
							{
								Utils.OpenFileInExplorer(resultFileZipPath);
							}
						});
		

		}

        private void UpdateStatus(string status)
        {
            Utils.RunOnUI(() => BusyContent = status);
        }

		private void SetDefaultZipPath(Customer customer, Branch branch, string branchCodeERP)
        {
			if (customer == null) return;
			if (branch == null) return;
			string customerCode = customer.Code;
            string sendDataFolderPath = UtilsPath.ZipOfficeFolder(_dbSettings);
			if (string.IsNullOrWhiteSpace(customer.SendToOfficePath) == false) sendDataFolderPath = customer.SendToOfficePath;
			DateTime dt = DateTime.Now;
			string date = dt.ToString("yyyy-MM-dd_HH-mm");
            string sendDataZipFileName = String.Format("{0}_{1}_{2}.zip",
													   customerCode,
													   branchCodeERP,
                                                       date);

            string sendDataZipFilePath = Path.Combine(sendDataFolderPath, sendDataZipFileName);

            this._zipPath = sendDataZipFilePath;
			//do path = <ftp>/Count4U/SendToOffice/CustomCode_CustomerName/BranchCode/<Date>/FileName.zip

			//string dateFtp = dt.ToString("yyyy") + @"\" + dt.ToString("MM") + @"\" + dt.ToString("dd");
			string customerName = customer.Name;
			string branchName = branch.Name;
			string branchCode = branch.Code;
			 this._ftpFolder = @"SendToOffice" + @"\" + customerCode  + @"\" +branchCode;

	
			this._ftpFileName = sendDataZipFileName;
			this._ftpPath = _ftpFolder + @"\" + sendDataZipFileName;
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
				switch (propertyName)
				{
					case "FtpPath":

						//if (!IsOkZipPath())
						//	return Localization.Resources.ViewModel_InventorChangeStatus_InvalidPath;

						break;
				}

                return String.Empty;
            }
        }

		private void BuildReports( string reportIniFile, string customerCode)
        {
            this._reports.Clear();

			//if (File.Exists(this._reportIniFile) == false)
			//{																														
			//	this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(inventorCode);
			//}
			//if (File.Exists(this._reportIniFile) == false) return;

			const string Context = ReportIniProperty.ContextSendToOffice;
			const string ForCustomer = ReportIniProperty.ForCustomer;
			const string RefillAlways = ReportIniProperty.RefillAlways;
            const string FileFormatKey = ReportIniProperty.FileType;
            const string IncludeInZipKey = ReportIniProperty.IncludeInZip;
            const string PrintKey = ReportIniProperty.Print;

			foreach (IniFileData iniFileData in this._iniFileParser.Get(reportIniFile))
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
						if (forCustomerCode == customerCode ) { addReport = true; }
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
			//this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentInventor.Code);

			if (File.Exists(this._reportIniFile) == false)
			{
				this._reportIniFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentInventor.Code);
			}
			if (File.Exists(this._reportIniFile) == false) return;

			this.BuildReports(this._reportIniFile, base.CurrentCustomer.Code);
        }

		private void BuildAdapter(string exportERPAdapterCode, string customerCode)
        {
            try
            {
				string name = exportERPAdapterCode; //

                if (String.IsNullOrEmpty(name))
                {
                    AdapterName = Localization.Resources.ViewModel_InventorStatusChange_msgAdapterNotSet;
                    _isRunExportErp = false;
                }
                else
                {
					string param = CustomerParamIsExcludeNotExistingInCatalog(AdapterName, customerCode);
					AdapterName = String.Format("[{0}]", name + param);
                    _isRunExportErp = true;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildAdapter", exc);
            }
        }

		private string CustomerParamIsExcludeNotExistingInCatalog(string adapterName, string customerCode)
		{
			if (string.IsNullOrWhiteSpace(adapterName) == true) return ": With NotExistingInCatalog";
			if (string.IsNullOrWhiteSpace(customerCode) == true) return ": With NotExistingInCatalog";
			bool isExcludeNotExistingInCatalog = false;
			string currentCustomerCode = customerCode;
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


	//=================
	public static class ExportErpParamParse
	{
		public static int GetIntValue(this Dictionary<string, CustomerConfig> config, int parm,
			CustomerConfigIniEnum adapterParm)
		{
			int? parseValue = ParseValue(config, adapterParm);
			if (parseValue != null)
			{
				return parseValue.Value;
			}
			else
			{
				return parm;

			}
		}

		public static string GetStringValue(this Dictionary<string, CustomerConfig> config, string parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				return parseValue;
			}
			else
			{
				return parm;
			}
		}


		public static bool GetBoolValue(this Dictionary<string, CustomerConfig> config, bool parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				bool ret = parseValue == "1";
				return ret;
			}
			else
			{
				return parm;
			}
		}

		private static int? ParseValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value;
			int n;
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
				if (Int32.TryParse(value, out n))
					return n;
			}

			return null;
		}

		private static string ParseStringValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value = "";
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				try
				{
					value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
					return value;
				}
				catch { return value; }
			}

			return value;
		}
	}







}