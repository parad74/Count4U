using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Common.Events;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Count4U.Common.Services.UICommandService;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism;
using System.Windows.Input;
using System.Windows.Threading;
using Count4U.Modules.Audit.ViewModels.Export;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Common.Extensions;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Common.Web;

namespace Count4U.ComplexDefaultAdapter
{																												 
	public class TemplateComplexAdapterViewModel : ImportModuleBaseViewModel, IImportPdaAdapter	 //TemplateAdapterFileFolderViewModel было
    {
		protected readonly IIturRepository _iturRepository;
		protected readonly IReportIniRepository _reportIniRepository;
		protected readonly IDBSettings _dbSettings;
		protected readonly IUnityContainer _unityContainer;
		protected readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
		protected readonly List<string> _newSessionCodeList;
		protected bool _isAutoPrint;	   //??
		protected bool _isContinueGrabFiles;	  //??
		protected string CurrentAdapterName { get; set; }
		//protected string _fileName { get; set; }

		//=====================
		protected readonly IImportAdapterRepository _importAdapterRepository;

		private readonly FromFtpViewModel _fromFTPViewModel;
  		private readonly ToFtpViewModel _toFTPViewModel;


		protected IImportModuleInfo _selectedCatalog;
		protected ObservableCollection<IImportModuleInfo> _itemsCatalogs;
		protected bool _configFileImportCatalogExists;


		protected IImportModuleInfo _selectedUpdateCatalog;
		protected ObservableCollection<IImportModuleInfo> _itemsUpdateCatalog;
		protected bool _configFileImportUpdateCatalogExists;

		protected IImportModuleInfo _selectedComplex;
		protected ObservableCollection<IImportModuleInfo> _itemsComplex;

		protected ResultModuleInfo _selectedSendToOffice;

		protected IImportModuleInfo _selectedItur;
		protected ObservableCollection<IImportModuleInfo> _itemsIturs;
		protected bool _configFileImportIturExists;

		protected IImportModuleInfo _selectedLocation;
		protected ObservableCollection<IImportModuleInfo> _itemsLocations;
		protected bool _configFileImportLocationExists;

		protected IImportModuleInfo _selectedSection;
		protected ObservableCollection<IImportModuleInfo> _itemsSections;
		protected bool _configFileImportSectionExists;

		protected IImportModuleInfo _selectedFamily;
		protected ObservableCollection<IImportModuleInfo> _itemsFamilys;
		protected bool _configFileImportFamilyExists;

		protected IImportModuleInfo _selectedSupplier;
		protected ObservableCollection<IImportModuleInfo> _itemsSuppliers;
		protected bool _configFileImportSupplierExists;

		protected IImportModuleInfo _selectedImportFromPDA;
		protected ObservableCollection<IImportModuleInfo> _itemsPDA;
		protected bool _configFileImportPDAExists;

		//============= Export PDA ===============
		protected ObservableCollection<IExportPdaModuleInfo> _itemsExportPda;
		protected IExportPdaModuleInfo _selectedExportPda;
		protected bool _configFileExportPDAExists;

		//============= Export Erp ===============
		protected ObservableCollection<IExportErpModuleInfo> _itemsExportErp;
		protected IExportErpModuleInfo _selectedExportErp;
		protected bool _configFileExportErpExists;

		protected string	_selectedInfrastructuraImportTitle = "";
		protected string _selectedInfrastructuraImportTooltip = "";
		


		protected UICommandRepository _commandRepository;
		protected UICommandRepository<IImportModuleInfo> _commandRepositoryImportModuleInfoObject;
		protected UICommandRepository<IExportPdaModuleInfo> _commandRepositoryExportPdaModuleInfoObject;
		protected UICommandRepository<IExportErpModuleInfo> _commandRepositoryExportErpModuleInfoObject;
		protected UICommandRepository<ResultModuleInfo> _commandRepositoryResultModuleInfoObject;


		protected DelegateCommand<IImportModuleInfo> _runImportByConfigCommand;
		protected DelegateCommand<IImportModuleInfo> _runImportClearByConfigCommand;
		protected DelegateCommand<IImportModuleInfo> _showImportLogCommand;
		protected DelegateCommand<IImportModuleInfo> _showImportConfigCommand;
		protected DelegateCommand<IImportModuleInfo> _navigateToGridImportCommand;
		protected DelegateCommand _openImportFixedPathCommand;
		protected DelegateCommand _openImportInDataPathCommand;
		protected DelegateCommand _openExportErpFixedPathCommand;
		protected DelegateCommand _openSendToOfficeFixedPathCommand;
		


		protected DelegateCommand<IExportPdaModuleInfo> _runExportPdaByConfigCommand;
		protected DelegateCommand<IExportPdaModuleInfo> _runExportPdaClearByConfigCommand;
		protected DelegateCommand<IExportPdaModuleInfo> _showExportPdaLogCommand;
		protected DelegateCommand<IExportPdaModuleInfo> _showExportPdaConfigCommand;

		protected DelegateCommand<IExportErpModuleInfo> _runExportErpByConfigCommand;
		protected DelegateCommand<IExportErpModuleInfo> _runExportErpClearByConfigCommand;
		protected DelegateCommand<IExportErpModuleInfo> _showExportErpLogCommand;
		protected DelegateCommand<IExportErpModuleInfo> _showExportErpConfigCommand;

		protected DelegateCommand<ResultModuleInfo> _runSendToOfficeCommand;
		protected DelegateCommand<ResultModuleInfo> _showSendToOfficeIniCommand;

		protected DelegateCommand _openImportLogPathCommand;
		protected DelegateCommand _openImportConfigPathCommand;
		protected DelegateCommand _openExportPdaLogPathCommand;
		protected DelegateCommand _openExportPdaConfigPathCommand;
		protected DelegateCommand _openExportErpLogPathCommand;
		protected DelegateCommand _openExportErpConfigPathCommand;

		protected readonly DelegateCommand _sendToFtpCommand;
		protected readonly DelegateCommand _getFromFtpCommand;

		protected DelegateCommand _openZipPathCommand;
		protected DelegateCommand _openIniSendToOfficePathCommand;

		protected DelegateCommand _copyFilesImportFromCustomerToInventorCommand;

		protected string _zipPath;
		protected string _iniSendToOfficePath;
		protected string _importLogPath;
		protected string _importFixedPath;
		protected string _exportErpFixedPath;
		protected string _sendToOfficeFixedPath;
		protected string _importInDataPath;
		protected string _exportPdaLogPath;
		protected string _exportErpLogPath;
		protected string _logText;
		protected string _importConfigPath;
		protected string _exportPdaConfigPath;
		protected string _exportErpConfigPath;

		public TemplateComplexAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
			UICommandRepository<IImportModuleInfo> commandRepositoryImportModuleInfoObject,
			UICommandRepository<IExportPdaModuleInfo> commandRepositoryExportPdaModuleInfoObject,
			UICommandRepository<IExportErpModuleInfo> commandRepositoryExportErpModuleInfoObject,
			UICommandRepository<ResultModuleInfo> commandRepositoryResultModuleInfoObject,
			 UICommandRepository commandRepository,
            IUnityContainer unityContainer,
			IImportAdapterRepository importAdapterRepository,
			IDBSettings dbSettings,
			IReportIniRepository reportIniRepository,
			FromFtpViewModel fromFtpViewModel,
			ToFtpViewModel toFtpViewModel) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager,
			logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
          {
			this._commandRepository = commandRepository;
			this._commandRepositoryImportModuleInfoObject = commandRepositoryImportModuleInfoObject;
			this._commandRepositoryExportPdaModuleInfoObject = commandRepositoryExportPdaModuleInfoObject;
			this._commandRepositoryExportErpModuleInfoObject = commandRepositoryExportErpModuleInfoObject;
			this._commandRepositoryResultModuleInfoObject = commandRepositoryResultModuleInfoObject;
            this._unityContainer = unityContainer;
			this._reportIniRepository = reportIniRepository;
			this._dbSettings = dbSettings;
			this._toFTPViewModel = toFtpViewModel;
			this._fromFTPViewModel	= fromFtpViewModel;
			
            this._iturRepository = iturRepository;
			this._importAdapterRepository = importAdapterRepository;
			this._newSessionCodeList = new List<string>();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

			this._openImportInDataPathCommand = new DelegateCommand(this.OpenImportInDataPathCommandExecute, this.OpenImportInDataPathCommandCanExecute);
			this._sendToFtpCommand = _commandRepository.Build(enUICommand.ToFtp, this.SendToFtpCommandExecuted, this.SendToFtpCommandCanExecute);
			this._getFromFtpCommand = _commandRepository.Build(enUICommand.FromFtp, this.GetFromFtpCommandExecuted, this.GetFromFtpCommandCanExecute);
																																 
			//this._runImportByConfigCommand = _commandRepositoryImportModuleInfoObject.Build(enUICommand.RunByConfig, RunImportByConfigCommandExecuted);
			//this._runImportClearByConfigCommand = _commandRepositoryImportModuleInfoObject.Build(enUICommand.ClearByConfig, RunImportClearByConfigCommandExecuted);
			//this._showImportLogCommand = _commandRepositoryImportModuleInfoObject.Build(enUICommand.ShowLogByConfig, RunImportShowLogByConfigCommandExecuted);
			//this._showImportConfigCommand = commandRepositoryImportModuleInfoObject.Build(enUICommand.ShowConfig, ShowImportConfigCommandExecuted);
			//this._openImportLogPathCommand = new DelegateCommand(OpenImportLogPathCommandExecute, OpenImportLogPathCommandCanExecute);
			//this._openImportConfigPathCommand = new DelegateCommand(OpenImportConfigPathCommandExecute, OpenImportConfigPathCommandCanExecute);
			//this._navigateToGridImportCommand = _commandRepositoryImportModuleInfoObject.Build(enUICommand.NavigateToGrid, NavigateToGridImportCommandExecuted);

			//this._runExportPdaByConfigCommand = _commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.RunByConfig, RunExportPdaByConfigCommandExecuted);
			//this._runExportPdaClearByConfigCommand = _commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ClearByConfig, RunExportPdaClearByConfigCommandExecuted);
			//this._showExportPdaLogCommand = _commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ShowLogByConfig, ShowExportPdaLogCommandExecuted);
			//this._showExportPdaConfigCommand = _commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ShowConfig, ShowExportPdaConfigCommandExecuted);
			//this._openExportPdaLogPathCommand = new DelegateCommand(OpenExportPdaLogPathCommandExecute, OpenExportPdaLogPathCommandCanExecute);
			//this._openExportPdaConfigPathCommand = new DelegateCommand(OpenExportPdaConfigPathCommandExecute, OpenExportPdaConfigPathCommandCanExecute);

			//this._runExportErpByConfigCommand = _commandRepositoryExportErpModuleInfoObject.Build(enUICommand.RunByConfig, RunExportErpByConfigCommandExecuted);
			//this._runExportErpClearByConfigCommand = _commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ClearByConfig, RunExportErpClearByConfigCommandExecuted);
			//this._showExportErpLogCommand = _commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ShowLogByConfig, ShowExportErpLogCommandExecuted);
			//this._showExportErpConfigCommand = _commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ShowConfig, ShowExportErpConfigCommandExecuted);
			//this._openExportErpLogPathCommand = new DelegateCommand(OpenExportErpLogPathCommandExecute, OpenExportErpLogPathCommandCanExecute);
			//this._openExportErpConfigPathCommand = new DelegateCommand(OpenExportErpConfigPathCommandExecute, OpenExportErpConfigPathCommandCanExecute);
					

			//this._runSendToOfficeCommand = _commandRepositoryResultModuleInfoObject.Build(enUICommand.RunByConfig, RunSendToOfficeCommandExecuted);
			//this._showSendToOfficeIniCommand = _commandRepositoryResultModuleInfoObject.Build(enUICommand.ShowIni, ShowSendToOfficeIniCommandExecuted);
			//this._openZipPathCommand = new DelegateCommand(OpenZipPathCommandExecute, OpenZipPathCommandCanExecute);
			//this._openIniSendToOfficePathCommand = new DelegateCommand(OpenIniSendToOfficePathCommandExecute, OpenIniSendToOfficePathCommandCanExecute);

			this._itemsCatalogs = new ObservableCollection<IImportModuleInfo>();
			this._itemsUpdateCatalog = new ObservableCollection<IImportModuleInfo>();
			this._itemsIturs = new ObservableCollection<IImportModuleInfo>();
			this._itemsLocations = new ObservableCollection<IImportModuleInfo>();
			this._itemsSections = new ObservableCollection<IImportModuleInfo>();
			this._itemsFamilys = new ObservableCollection<IImportModuleInfo>();
			this._itemsSuppliers = new ObservableCollection<IImportModuleInfo>();
			this._itemsPDA  = new ObservableCollection<IImportModuleInfo>();
			this._itemsExportPda = new ObservableCollection<IExportPdaModuleInfo>();
			this._itemsExportErp = new ObservableCollection<IExportErpModuleInfo>();

			this.IsBusyGetFromFtp = false;
	    }

		protected FromFtpViewModel FromFTPViewModel
		{
			get { return _fromFTPViewModel; }
		}

		protected ToFtpViewModel ToFTPViewModel
		{
			get { return _toFTPViewModel; }
		}

		public DelegateCommand SendToFtpCommand
		{
			get { return this._sendToFtpCommand; }
		}


		public DelegateCommand GetFromFtpCommand
		{
			get { return this._getFromFtpCommand; }
		}

		//_sendToFtpCommand.RaiseCanExecuteChanged();
		private bool SendToFtpCommandCanExecute()
		{
			if (this.SelectedExportPda == null) return false;
			string adapterName = this.SelectedExportPda.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			if (adapterName == ExportPdaAdapterName.ExportHT630Adapter) return false;
			if (adapterName == ExportPdaAdapterName.ExportPdaMISAdapter) return false;
			if (this._toFTPViewModel.Items.Any(r => r.IsChecked) == false)		 return false;

			return true;
		}

		private bool _isBusyGetFromFtp;
		public bool IsBusyGetFromFtp
		{
			get { return _isBusyGetFromFtp; }
			set
			{
				_isBusyGetFromFtp = value;
				this._getFromFtpCommand.RaiseCanExecuteChanged();
			}
		}
		private bool GetFromFtpCommandCanExecute()
		{
			if (this.SelectedImportFromPDA == null) return false;
			if (this.IsBusyGetFromFtp == true) return false;
			//if (this._fromFTPViewModel.IsBusy == true) return false;
			string adapterName = this.SelectedImportFromPDA.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			if (adapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
				|| adapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
			{
				//if (this._fromFTPViewModel.Items.Any(r => r.IsChecked) == false) return false;
				//else 
					return true;
			}
			return false;
		}

		private void SendToFtpCommandExecuted()
		{
			if (base.State == null) return;
		
			this._toFTPViewModel.SendToPDAOnFtp(base.State.CurrentInventor);
			this._sendToFtpCommand.RaiseCanExecuteChanged();
			//using (new CursorWait())
			//{
			////this._toFTPViewModel.Build(true);
			//}
		}

		private void GetFromFtpCommandExecuted()
		{
			if (base.State == null) return;
			if (this.IsBusyGetFromFtp == true) return;
			base.LogImport.Clear();
			Utils.RunOnUI(() =>
			{
				this.LogText = "Wait ...";
				this.IsBusyGetFromFtp = true;
			});
			
			Thread.Sleep(300);

			using (new CursorWait())
			{
				this._fromFTPViewModel.Build(true);
			}

			this._fromFTPViewModel.GetFromFtp(base.State.CurrentInventor);
			//this._fromFTPViewModel.IsBusy = false;
		
			//FileLogInfo fileLogInfo = new FileLogInfo();
			//fileLogInfo.File = this.ImportLogPath + @"\getFtpLog.txt";
			//base.IsSaveFileLog = true;
			//base.SaveFileLog(fileLogInfo);
			string logtext1 = base.LogImport.PrintLog();
			Utils.RunOnUI(() =>
			{
				this.LogText = logtext1;
			
			});

#if DEBUG
	
#else              
		  		using (new CursorWait())
			{
				this._fromFTPViewModel.Build(true);
				if (this._fromFTPViewModel.Items.Count > 1) { this.IsBusyGetFromFtp = false; }
			}
#endif

		}

		public DelegateCommand OpenImportLogPathCommand
		{
			get { return _openImportLogPathCommand; }
		}


		public DelegateCommand OpenExportPdaLogPathCommand
		{
			get { return _openExportPdaLogPathCommand; }
		}


		public DelegateCommand OpenExportErpLogPathCommand
		{
			get { return _openExportErpLogPathCommand; }
		}

		public DelegateCommand OpenImportConfigPathCommand
		{
			get { return _openImportConfigPathCommand; }
		}


		public DelegateCommand OpenExportPdaConfigPathCommand
		{
			get { return _openExportPdaConfigPathCommand; }
		}

		public DelegateCommand OpenExportErpConfigPathCommand
		{
			get { return _openExportErpConfigPathCommand; }
		}


		public DelegateCommand CopyFilesImportFromCustomerToInventorCommand
		{
			get { return _copyFilesImportFromCustomerToInventorCommand; }
		}


		public DelegateCommand OpenZipPathCommand
		{
			get { return _openZipPathCommand; }
		}

		public DelegateCommand OpenIniSendToOfficePathCommand
		{
			get { return _openIniSendToOfficePathCommand; }
		}

		public virtual bool OpenExportPdaLogPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ExportPdaLogPath) == true) return false;

			return Directory.Exists(this.ExportPdaLogPath);
		}

		public virtual bool OpenExportPdaConfigPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ExportPdaConfigPath) == true) return false;

			return Directory.Exists(this.ExportPdaConfigPath);
		}

		public virtual bool OpenExportErpLogPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ExportErpLogPath) == true) return false;

			return Directory.Exists(this.ExportErpLogPath);
		}

		public virtual bool OpenExportErpConfigPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ExportErpConfigPath) == true) return false;

			return Directory.Exists(this.ExportErpConfigPath);
		}

		public virtual bool OpenImportLogPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ImportLogPath) == true) return false;

			return Directory.Exists(this.ImportLogPath);
		}


		public virtual bool OpenImportFixedPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ImportFixedPath) == true) return false;

			if (Directory.Exists(this.ImportFixedPath) == false)
			{
			   Directory.CreateDirectory(this.ImportFixedPath);
			}

			return Directory.Exists(this.ImportFixedPath);
		}

		public virtual bool OpenExportErpFixedPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ExportErpFixedPath) == true) return false;

			if (Directory.Exists(this.ExportErpFixedPath) == false)
			{
				Directory.CreateDirectory(this.ExportErpFixedPath);
			}

			return Directory.Exists(this.ExportErpFixedPath);
		}

		
		
		public virtual bool OpenImportInDataPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ImportInDataPath) == true) return false;

			if (Directory.Exists(this.ImportInDataPath) == false)
			{
				Directory.CreateDirectory(this.ImportInDataPath);
			}

			return Directory.Exists(this.ImportInDataPath);
		}

		public virtual bool OpenImportConfigPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ImportConfigPath) == true) return false;

			return Directory.Exists(this.ImportConfigPath);
		}

		public virtual bool OpenZipPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.ZipPath) == true) return false;

			bool canOpen = Directory.Exists(this.ZipPath);
			return canOpen;
		}

		public virtual void OpenZipPathCommandExecute()
		{
			if (!Directory.Exists(this.ZipPath)) return;

			Utils.OpenFolderInExplorer(this.ZipPath);
		}

		// ============= ExportErp =================
		public virtual void OpenExportErpLogPathCommandExecute()
		{
			if (!Directory.Exists(this.ExportErpLogPath)) return;

			Utils.OpenFolderInExplorer(this.ExportErpLogPath);
		}

		public virtual void OpenExportErpConfigPathCommandExecute()
		{
			if (!Directory.Exists(this.ExportErpConfigPath)) return;

			Utils.OpenFolderInExplorer(this.ExportErpConfigPath);
		}

		//===========
		public virtual void CopyFilesImportFromCustomerToInventorCommandExecute()
		{
			this.CopyImportFolderFormCustomerToInventor();
		}

		public void CopyImportFolderFormCustomerToInventor()
		{
			if (base.State != null)
			{
				if (base.State.CurrentInventor != null)
				{
					if (base.State.CurrentCustomer != null)
					{
						string importFolderCustomer = base.ContextCBIRepository.GetImportFolderPath(base.State.CurrentCustomer);
						string importFolderInventor = base.ContextCBIRepository.GetImportFolderPath(base.State.CurrentInventor);

						if (!Directory.Exists(importFolderInventor))
						{
							Directory.CreateDirectory(importFolderInventor);
						}

						foreach (string file in Directory.EnumerateFiles(importFolderCustomer))
						{
							FileInfo fi = new FileInfo(file);

							string targetFile = Path.Combine(importFolderInventor, fi.Name);
							File.Copy(fi.FullName, targetFile,true);
						}
					}
				}
			}
		}

		public void CopyImportFolderFormFixedToInventor()
		{
			if (base.State != null)
			{
				if (base.State.CurrentInventor != null)
				{
					string importFolderInventor = base.ContextCBIRepository.GetImportFolderPath(base.State.CurrentInventor);

					if (!Directory.Exists(importFolderInventor))
					{
						Directory.CreateDirectory(importFolderInventor);
					}

					string importFixedFolder = Path.GetFullPath(this.ImportFixedPath);
					if (Directory.Exists(importFixedFolder) == false) return;

					foreach (string file in Directory.EnumerateFiles(importFixedFolder))
					{
						FileInfo fi = new FileInfo(file);

						string targetFile = Path.Combine(importFolderInventor, fi.Name);
						File.Copy(fi.FullName, targetFile, true);
					}
				}
			}
		}


		public void CopyLogFolderFormFixedToInventor(string adapterName)
		{
			if (base.State != null)
			{
				if (base.State.CurrentInventor != null)
				{
					string importFolderInventor = base.ContextCBIRepository.GetImportFolderPath(base.State.CurrentInventor);

					if (!Directory.Exists(importFolderInventor))
					{
						Directory.CreateDirectory(importFolderInventor);
					}

					string importFixedFolder = Path.GetFullPath(this.ImportFixedPath);
					if (Directory.Exists(importFixedFolder) == false) return;
							

					string importLogFixedFolder = Path.Combine(importFixedFolder, "Log", adapterName);
					if (Directory.Exists(importLogFixedFolder) == false) return;

					string importLogInventorFolder = Path.Combine(importFolderInventor, "Log", adapterName);
					{
						Directory.CreateDirectory(importLogInventorFolder);
					}

					foreach (string file in Directory.EnumerateFiles(importLogFixedFolder))
					{
						FileInfo fi = new FileInfo(file);

						string targetFile = Path.Combine(importLogInventorFolder, fi.Name);
						File.Copy(fi.FullName, targetFile, true);
					}

				}
			}
		}




		// ============= ExportPda =================
		public virtual void OpenExportPdaLogPathCommandExecute()
		{
			if (!Directory.Exists(this.ExportPdaLogPath)) return;

			Utils.OpenFolderInExplorer(this.ExportPdaLogPath);
		}

		public virtual void OpenExportPdaConfigPathCommandExecute()
		{
			if (!Directory.Exists(this.ExportPdaConfigPath)) return;

			Utils.OpenFolderInExplorer(this.ExportPdaConfigPath);
		}

		// ============	Import ===========================
		public virtual void OpenImportLogPathCommandExecute()
		{
			if (!Directory.Exists(this.ImportLogPath)) return;

			Utils.OpenFolderInExplorer(this.ImportLogPath);
		}

		public virtual void OpenImportConfigPathCommandExecute()
		{
			if (!Directory.Exists(this.ImportConfigPath)) return;

			Utils.OpenFolderInExplorer(this.ImportConfigPath);
		}

		// ============= Send to office ini file =================
		public virtual void OpenIniSendToOfficePathCommandExecute()
		{
			if (!Directory.Exists(this.IniSendToOfficePath)) return;

			Utils.OpenFolderInExplorer(this.IniSendToOfficePath);
		}

		public virtual bool OpenIniSendToOfficePathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.IniSendToOfficePath) == true) return false;

			return Directory.Exists(this.IniSendToOfficePath);
		}

			// ============= Send to office FixedPath =================
		public virtual void OpenSendToOfficeFixedPathCommandExecute()
		{
			if (!Directory.Exists(this.SendToOfficeFixedPath)) return;

			Utils.OpenFolderInExplorer(this.SendToOfficeFixedPath);
		}

		public virtual bool OpenSendToOfficeFixedPathCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.SendToOfficeFixedPath) == true) return false;

			return Directory.Exists(this.SendToOfficeFixedPath);
		}
		//================

		public virtual void ShowImportConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			using (new CursorWait("ShowImportConfigCommand"))
			{
				this.RunImportShowConfigFile(importModuleInfo);
			}
//			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = importModuleInfo });
		}


		public virtual void OpenImportFixedPathCommandExecute()
		{
			if (!Directory.Exists(this.ImportFixedPath)) return;

			Utils.OpenFolderInExplorer(this.ImportFixedPath);
		}

		// =========== ExportErp
		public virtual void OpenExportErpFixedPathCommandExecute()
		{
			if (!Directory.Exists(this.ExportErpFixedPath)) return;

			Utils.OpenFolderInExplorer(this.ExportErpFixedPath);
		}

		public void CopyExportErpFolderFormInventorToFixed()
		{
			if (string.IsNullOrWhiteSpace(this.ExportErpFixedPath) == true) return;		 //COPYTO

			if (base.State != null)
			{
				if (base.State.CurrentInventor != null)
				{

					//COPYFROM
					string exportErpFolderInventor = UtilsPath.ExportErpFolder(this._dbSettings, "Inventor", base.State.CurrentInventor.Code);

					string exportErpFixedFolder = Path.GetFullPath(exportErpFolderInventor);
					if (Directory.Exists(exportErpFixedFolder) == false) return;

					//COPYTO
					if (!Directory.Exists(this.ExportErpFixedPath))
					{
						Directory.CreateDirectory(this.ExportErpFixedPath);
					}

					foreach (string file in Directory.EnumerateFiles(exportErpFixedFolder))
					{
						FileInfo fi = new FileInfo(file);

						string targetFile = Path.Combine(ExportErpFixedPath, fi.Name);
						File.Copy(fi.FullName, targetFile, true);
					}
				}
			}
		}

		// ================== Copy ZIP send to office
		public string CopySendToOfficeZipToFixed(string sendToOfficeZipFile)
		{
			string targetFile = "";
			if (string.IsNullOrWhiteSpace(this.SendToOfficeFixedPath) == true) return "";	
			if (string.IsNullOrWhiteSpace(sendToOfficeZipFile) == true) return "";		 //COPYTO
			 if (File.Exists(sendToOfficeZipFile) == false) return "";
			 string fileName = Path.GetFileName(sendToOfficeZipFile);
			 if (string.IsNullOrWhiteSpace(fileName) == true) return "";	

			if (base.State != null)
			{
				if (base.State.CurrentCustomer != null)
				{

					//COPYFROM file
					// sendToOfficeZipFile

					//COPYTO  folder
					string toFolder = "";
				//	toFolder = base.State.CurrentCustomer.ReportPath;
					toFolder = this.SendToOfficeFixedPath;
					if (!Directory.Exists(toFolder))
					{
						Directory.CreateDirectory(toFolder);
					}

					targetFile = Path.Combine(toFolder, fileName);
					File.Copy(sendToOfficeZipFile, targetFile, true);
					return targetFile;
				}
			}
			return targetFile;
		}
		//-----------------
		
		public virtual void OpenImportInDataPathCommandExecute()
		{
			if (!Directory.Exists(this.ImportInDataPath)) return;

			Utils.OpenFolderInExplorer(this.ImportInDataPath);
		}

		public DelegateCommand<IImportModuleInfo> RunImportByConfigCommand
		{
			get { return this._runImportByConfigCommand; }
		}


		public DelegateCommand<IImportModuleInfo> ShowImportConfigCommand
		{
			get { return this._showImportConfigCommand; }
		}

		public DelegateCommand<IExportPdaModuleInfo> ShowExportPdaConfigCommand
		{
			get { return this._showExportPdaConfigCommand; }
		}

		public DelegateCommand<IExportErpModuleInfo> ShowExportErpConfigCommand
		{
			get { return this._showExportErpConfigCommand; }
		}

		public bool IsConfigFileExportPdaExists(IExportPdaModuleInfo exportModuleInfo)
		{
			if (exportModuleInfo == null) return false;
			string adapterName = exportModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			ExportPdaWithModulesViewModel exportPdaWithModulesViewModel = null;
			try
			{
				exportPdaWithModulesViewModel = base.ServiceLocator.GetInstance<ExportPdaWithModulesViewModel>(adapterName);
			}
			catch { }
			if (exportPdaWithModulesViewModel == null) return false;

			// ?? Пока договорились что конфиг всегда берется из Customer
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string getConfigFolderPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string configPath = getConfigFolderPath + adapterConfigFileName;
			if (File.Exists(configPath) == true)
			{
				return true;
			}
			return false;
		}

		public bool IsConfigFileExportErpExists(IExportErpModuleInfo exportModuleInfo)
		{
			if (exportModuleInfo == null) return false;
			string adapterName = exportModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			ExportErpWithModulesViewModel exportErpWithModulesViewModel = null;
			try
			{
				exportErpWithModulesViewModel = base.ServiceLocator.GetInstance<ExportErpWithModulesViewModel>(adapterName);
			}
			catch { }
			if (exportErpWithModulesViewModel == null) return false;

			// ?? Пока договорились что конфиг всегда берется из Customer
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string getConfigFolderPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string configPath = getConfigFolderPath + adapterConfigFileName;
			//string configPath = exportErpWithModulesViewModel.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;
			if (File.Exists(configPath) == true)
			{
				return true;
			}
			return false;
		}

		public bool IsConfigFileImportExists(IImportModuleInfo importModuleInfo)
		{
			if (importModuleInfo == null) return false;
			string adapterName = importModuleInfo.Name;
			if (adapterName == Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter) return false;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return false;
			ImportModuleBaseViewModel importModuleBaseViewModel = null;
			try{
			importModuleBaseViewModel = base.ServiceLocator.GetInstance<ImportModuleBaseViewModel>(adapterName);
			} catch{}
			if (importModuleBaseViewModel == null) return false;

			// ?? Пока договорились что конфиг всегда берется из Customer
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;
			if (File.Exists(configPath) == true)
			{
				return true;
			}
			return false;
		}

		public bool ConfigFileImportCatalogExists
		{
			get
			{
				//return ConfigFileImportExists(this.SelectedCatalog);
				return this._configFileImportCatalogExists;
			}
			set { this._configFileImportCatalogExists = value; }
		}


		public bool ConfigFileImportUpdateCatalogExists
		{
			get
			{

				return this._configFileImportUpdateCatalogExists;
			}
			set { this._configFileImportUpdateCatalogExists = value; }
		}

		public bool ConfigFileImportIturExists
		{
			get
			{
				return this._configFileImportIturExists;
				// ConfigFileImportExists(this.SelectedItur);
			}
			set { this._configFileImportIturExists = value; }
		}
		public bool ConfigFileImportLocationExists
		{
			get { return _configFileImportLocationExists; }
			set { _configFileImportLocationExists = value; }
		}

		public bool ConfigFileImportSectionExists
		{
			get { return _configFileImportSectionExists; }
			set { _configFileImportSectionExists = value; }
		}

		public bool ConfigFileImportFamilyExists
		{
			get { return _configFileImportFamilyExists; }
			set { _configFileImportFamilyExists = value; }
		}

		public bool ConfigFileImportSupplierExists
		{
			get { return _configFileImportSupplierExists; }
			set { _configFileImportSupplierExists = value; }
		}

		public bool ConfigFileImportPDAExists
		{
			get { return _configFileImportPDAExists; }
			set { _configFileImportPDAExists = value; }
		}

		public bool ConfigFileExportPDAExists
		{
			get { return _configFileExportPDAExists; }
			set { _configFileExportPDAExists = value; }
		}

		public bool ConfigFileExportErpExists
		{
			get { return _configFileExportErpExists; }
			set { _configFileExportErpExists = value; }
		}
		

		public DelegateCommand<IImportModuleInfo> ShowImportLogCommand
		{
			get { return this._showImportLogCommand; }
		}

		public DelegateCommand OpenImportFixedPathCommand
		{
			get { return this._openImportFixedPathCommand; }
		}


		public DelegateCommand OpenExportErpFixedPathCommand
		{
			get { return this._openExportErpFixedPathCommand; }
		}


		public DelegateCommand OpenSendToOfficeFixedPathCommand
		{
			get { return this._openSendToOfficeFixedPathCommand; }
		}

		public DelegateCommand OpenImportInDataPathCommand
		{
			get { return this._openImportInDataPathCommand; }
		}

		public DelegateCommand<IImportModuleInfo> RunImportClearByConfigCommand
		{
			get { return this._runImportClearByConfigCommand; }
		}


		public DelegateCommand<IExportPdaModuleInfo> RunExportPdaByConfigCommand
		{
			get { return this._runExportPdaByConfigCommand; }
		}

		
		public DelegateCommand<IExportErpModuleInfo> RunExportErpByConfigCommand
		{
			get { return this._runExportErpByConfigCommand; }
		}

		public DelegateCommand<IExportPdaModuleInfo> RunExportPdaClearByConfigCommand
		{
			get { return this._runExportPdaClearByConfigCommand; }
		}

		public DelegateCommand<IExportErpModuleInfo> RunExportErpClearByConfigCommand
		{
			get { return this._runExportErpClearByConfigCommand; }
		}

		public DelegateCommand<IExportPdaModuleInfo> ShowExportPdaLogCommand
		{
			get { return this._showExportPdaLogCommand; }
		}

		public DelegateCommand<IExportErpModuleInfo> ShowExportErpLogCommand
		{
			get { return this._showExportErpLogCommand; }
		}
		

		public DelegateCommand<ResultModuleInfo> RunSendToOfficeCommand
		{
			get { return this._runSendToOfficeCommand; }
		}

		public DelegateCommand<ResultModuleInfo> ShowSendToOfficeIniCommand
		{
			get { return this._showSendToOfficeIniCommand; }
		}
		

		public DelegateCommand<IImportModuleInfo> NavigateToGridImportCommand
		{
			get { return this._navigateToGridImportCommand; }
		}

	
		
		public List<string> GetTitleListOfImportAdaptersWithConfig()
		{
			List<string> list = new List<string>();
			if (ConfigFileImportCatalogExists == true)
			{
				if (SelectedCatalog != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedCatalog.Title) == false)
					{
						list.Add(SelectedCatalog.Title);
					}
				}
			}

			if (ConfigFileImportLocationExists == true)
			{
				if (SelectedLocation != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedLocation.Title) == false)
					{
						list.Add(SelectedLocation.Title);
					}
				}
			}

			if (ConfigFileImportIturExists == true)
			{
				if (SelectedItur != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedItur.Title) == false)
					{
						list.Add(SelectedItur.Title);
					}
				}
			}


			if (ConfigFileImportSectionExists == true)
			{
				if (SelectedSection != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSection.Title) == false)
					{
						list.Add(SelectedSection.Title);
					}
				}
			}

			if (ConfigFileImportFamilyExists == true)
			{
				if (SelectedFamily != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedFamily.Title) == false)
					{
						list.Add(SelectedFamily.Title);
					}
				}
			}


			if (ConfigFileImportSupplierExists == true)
			{
				if (SelectedSupplier != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSupplier.Title) == false)
					{
						list.Add(SelectedSupplier.Title);
					}
				}
			}
			return list;

		}


	
		public List<string> GetTooltipListOfImportAdaptersWithConfig()
		{
			List<string> list = new List<string>();
			if (ConfigFileImportCatalogExists == true)
			{
				if (SelectedCatalog != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedCatalog.Title) == false)
					{
						list.Add(SelectedCatalog.Title + " [ImportCatalog]");
					}
				}
			}

			if (ConfigFileImportLocationExists == true)
			{
				if (SelectedLocation != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedLocation.Title) == false)
					{
						list.Add(SelectedLocation.Title + " [ImportLocation]");
					}
				}
			}

			if (ConfigFileImportIturExists == true)
			{
				if (SelectedItur != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedItur.Title) == false)
					{
						list.Add(SelectedItur.Title + " [ImportItur]");
					}
				}
			}


			if (ConfigFileImportSectionExists == true)
			{
				if (SelectedSection != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSection.Title) == false)
					{
						list.Add(SelectedSection.Title + " [ImportSection]");
					}
				}
			}

			if (ConfigFileImportFamilyExists == true)
			{
				if (SelectedFamily != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedFamily.Title) == false)
					{
						list.Add(SelectedFamily.Title + " [ImportFamily]");
					}
				}
			}


			if (ConfigFileImportSupplierExists == true)
			{
				if (SelectedSupplier != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSupplier.Title) == false)
					{
						list.Add(SelectedSupplier.Title + " [ImportSupplier]");
					}
				}
			}
			return list;
		}

		public List<string> GetCodeListOfImportAdaptersWithConfig()
		{
			List<string> list = new List<string>();
			if (ConfigFileImportCatalogExists == true)
			{
				if (SelectedCatalog != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedCatalog.Name) == false)
					{
						list.Add(SelectedCatalog.Name);
					}
				}
			}

			if (ConfigFileImportLocationExists == true)
			{
				if (SelectedLocation != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedLocation.Name) == false)
					{
						list.Add(SelectedLocation.Name);
					}
				}
			}

			if (ConfigFileImportIturExists == true)
			{
				if (SelectedItur != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedItur.Name) == false)
					{
						list.Add(SelectedItur.Name);
					}
				}
			}


			if (ConfigFileImportSectionExists == true)
			{
				if (SelectedSection != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSection.Name) == false)
					{
						list.Add(SelectedSection.Name);
					}
				}
			}

			if (ConfigFileImportFamilyExists == true)
			{
				if (SelectedFamily != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedFamily.Name) == false)
					{
						list.Add(SelectedFamily.Name);
					}
				}
			}


			if (ConfigFileImportSupplierExists == true)
			{
				if (SelectedSupplier != null)
				{
					if (string.IsNullOrWhiteSpace(SelectedSupplier.Name) == false)
					{
						list.Add(SelectedSupplier.Name);
					}
				}
			}
			return list;
		}


		public List<IImportModuleInfo> GetAdapterInfoListOfImportAdaptersWithConfig()
		{
			List<IImportModuleInfo> list = new List<IImportModuleInfo>();
			if (ConfigFileImportCatalogExists == true)
			{
				if (SelectedCatalog != null)
				{
					list.Add(SelectedCatalog);
 				}
			}

			if (ConfigFileImportLocationExists == true)
			{
				if (SelectedLocation != null)
				{
					list.Add(SelectedLocation);
				}
			}

			if (ConfigFileImportIturExists == true)
			{
				if (SelectedItur != null)
				{
 					list.Add(SelectedItur);
				}
			}


			if (ConfigFileImportSectionExists == true)
			{
				if (SelectedSection != null)
				{
					list.Add(SelectedSection);
				}
			}

			if (ConfigFileImportFamilyExists == true)
			{
				if (SelectedFamily != null)
				{
					list.Add(SelectedFamily);
				}
			}


			if (ConfigFileImportSupplierExists == true)
			{
				if (SelectedSupplier != null)
				{
					list.Add(SelectedSupplier);
				}
			}
			return list;
		}

	
		//========== CLEAR Import============================
		public virtual void RunImportClearByConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			ClearLogText();
			//LoadWaitCursor();
			RunImportClearByConfig(importModuleInfo);
			//LoadDefaultCursor();
		}

		public virtual void RunImportClearByConfig(IImportModuleInfo importModuleInfo)
		{
			using (new CursorWait("RunImportClearByConfig"))
			{
				this.ImportLogPath = "";
				this.ImportConfigPath = "";
				if (importModuleInfo == null)
				{
					this.LogText = "ImportModuleInfo if null";

					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", importModuleInfo.ImportDomainEnum.ToString(), importModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}
				this.ImportLogPath = GetImpotLogPath(base.CurrentInventor, importModuleInfo);

				ImportModuleBaseViewModel importModuleBaseViewModel = null;
				string adapterName = importModuleInfo.Name;
				importModuleBaseViewModel = base.ServiceLocator.GetInstance<ImportModuleBaseViewModel>(adapterName);
				if (importModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ImportViewModel with Name [" + adapterName + "]";
					return;
				}
				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				string configFilePath = configPath + adapterConfigFileName;
				this.ImportConfigPath = configPath;
				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				//ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
				// ?? Пока договорились что конфиг всегда берется из Customer проверяю гипотезу
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;

				if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportInventProduct)
				{
					Count4U.Modules.Audit.ViewModels.Import.ImportFromPdaViewModel importFromPdaViewModel =
						base.ServiceLocator.GetInstance<Count4U.Modules.Audit.ViewModels.Import.ImportFromPdaViewModel>();

						importFromPdaViewModel.ClearByConfig(importModuleBaseViewModel,
							 base.State,
							 adapterName,
							importModuleInfo.ImportDomainEnum,
							fromConfigXDoc);
				}

				else if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportCatalog
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportItur
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportLocation
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportFamily
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportSection
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportSupplier
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.UpdateCatalog)
				{
					Count4U.Modules.Audit.ViewModels.Import.ImportWithModulesViewModel importWithModulesViewModel =
						base.ServiceLocator.GetInstance<Count4U.Modules.Audit.ViewModels.Import.ImportWithModulesViewModel>();

					importWithModulesViewModel.ClearImportByConfig(importModuleBaseViewModel,
							 base.State,
							 adapterName,
							 importModuleInfo.ImportDomainEnum,
							fromConfigXDoc);

				}
			} //  using (new CursorWait())

			string logtext = base.LogImport.PrintLog();
			Utils.RunOnUI(() =>
			{
				this.LogText = logtext;
			});


			//	base.UpdateLogFromILog();
			//FileLogInfo fileLogInfo = new FileLogInfo();
			//fileLogInfo.Directory = this.LogPath;
			//base.IsSaveFileLog = true;
			//base.SaveFileLog(fileLogInfo);
			//Thread.Sleep(500);

			//if (importModuleBaseViewModel != null)
			//{
			//	this.LogText = base.LogImport.PrintLog();
			//	importModuleBaseViewModel.UpdateLog = r => Utils.RunOnUI(() =>
			//	{
			//		this.LogText = r;
			//	});
			//}



			//importModuleBaseViewModel.RunImportWithoutGUIBase(importModuleInfo);

			//this._importFromPdaViewModel.ImportByConfig(importModuleInfo.UserControlType as ImportModuleBaseViewModel, 
			//	importModuleInfo.Name, 
			//	ImportDomainEnum.ImportInventProduct, 
			//	ConfigXDocFromEnum.FromCustomerInData);
			//			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = importModuleInfo });
		}

		//========== IMPORT ============================	   
		public virtual void RunImportByConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			ClearLogText();
			RunImportByConfig(importModuleInfo);
		}

		public virtual void RunImportByConfig(IImportModuleInfo importModuleInfo)
		{
			this.LogText = "";
			using (new CursorWait("RunImportByConfig"))
			{
				this.ImportLogPath = "";
				this.ImportConfigPath = "";
				if (importModuleInfo == null)
				{
					this.LogText = "ImportModuleInfo if null";
					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", importModuleInfo.ImportDomainEnum.ToString(), importModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}

				this.ImportLogPath = this.GetImpotLogPath(base.CurrentInventor, importModuleInfo);
				//..ициализируем из config ффйла катосмера
				//this.ImportFixedPath = this.GetImpotFixedPath(this.ImportConfigPath, this.CurrentAdapterName);
				
				ImportModuleBaseViewModel importModuleBaseViewModel = null;

				string adapterName = importModuleInfo.Name;
				importModuleBaseViewModel = base.ServiceLocator.GetInstance<ImportModuleBaseViewModel>(adapterName);
				if (importModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ImportViewModel with Name [" + adapterName + "]";
					return;
				}

				importModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
				importModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;

				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				this.ImportConfigPath = configPath;
				string configFilePath = configPath + adapterConfigFileName;
				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				//bool ret = importModuleBaseViewModel.CanImport();

				//ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
				// ?? Пока договорились что конфиг всегда берется из Customer проверяю гипотезу
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;

				if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportInventProduct)
				{
					Count4U.Modules.Audit.ViewModels.Import.ImportFromPdaViewModel importFromPdaViewModel =
						base.ServiceLocator.GetInstance<Count4U.Modules.Audit.ViewModels.Import.ImportFromPdaViewModel>();

					string complexAdapterName = "";
					if (SelectedComplex != null) complexAdapterName = SelectedComplex.Name;

					importFromPdaViewModel.ImportByConfig(importModuleBaseViewModel,
							 base.State,
							 adapterName,
							 importModuleInfo.ImportDomainEnum,
							fromConfigXDoc,
							complexAdapterName, this);

					CopyLogFolderFormFixedToInventor(adapterName);
				}

				else if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportCatalog
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportItur
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportLocation
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportFamily
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportSection
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportSupplier
							|| importModuleInfo.ImportDomainEnum == ImportDomainEnum.UpdateCatalog)
				{

					Count4U.Modules.Audit.ViewModels.Import.ImportWithModulesViewModel importWithModulesViewModel =
						base.ServiceLocator.GetInstance<Count4U.Modules.Audit.ViewModels.Import.ImportWithModulesViewModel>();


					
				
						if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
						{
							try
							{
								XDocument configXDoc = new XDocument();
								configXDoc = XDocument.Load(configPath);
								XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
								string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
							}
							catch { }
						}

					string complexAdapterName = "";
					if (SelectedComplex != null) complexAdapterName = SelectedComplex.Name;
					importWithModulesViewModel.ImportByConfig(importModuleBaseViewModel,
							 base.State,
							 adapterName,
							 importModuleInfo.ImportDomainEnum,
							fromConfigXDoc,
							complexAdapterName, this);

					
					CopyLogFolderFormFixedToInventor(adapterName);
					//Thread.Sleep(5000);
				}
			}	//using (new CursorWait())


			//string fixedLogPath = GetImpotLogFixedPath(this.ImportFixedPath, importModuleInfo);
		
			this.RefreshLogText( this.ImportLogPath);
		}


	   ////Предполагает что есть адаптер импорта каталога
	   // public virtual bool RunImportCatalogByConfigCommandCanExecute(IImportModuleInfo importModuleInfo)
	   // {
	   //		 if (this.SelectedCatalog != null) return true;
	   //	 else return false;
	   // }
		// ============================= LOG	  Import
		public virtual void RunImportShowLogByConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			string importLogPath = RunImportShowLogByConfig(importModuleInfo);
			if (string.IsNullOrWhiteSpace(importLogPath) == false)
			{
				RefreshLogText(importLogPath);
			}
			else
			{
				this.LogText = "";
			}
		}

		public virtual string RunImportShowLogByConfig(IImportModuleInfo importModuleInfo)
		{
			this.ImportLogPath = "";
			this.LogText = "";
			if (importModuleInfo == null) return "";
			if (base.CurrentInventor == null) return "";
			this.ImportLogPath = GetImpotLogPath(base.CurrentInventor, importModuleInfo);
			return this.ImportLogPath;
		}

		// ============================= Config  Import
		public virtual void RunImportShowConfigFile(IImportModuleInfo importModuleInfo)
		{
			this.ImportConfigPath = "";
			this.LogText = "";
			if (importModuleInfo == null)
			{
				this.LogText = "ImportModuleInfo is null";
				return;
			}

			if (base.CurrentInventor == null)
			{
				this.LogText = "CurrentInventor is null";
				return;
			}

			string adapterName = importModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true)
			{
				this.LogText = "AdapterName Is NullOrWhiteSpace ";
				return;
			}

			ImportModuleBaseViewModel importModuleBaseViewModel = null;
			try
			{
				importModuleBaseViewModel = base.ServiceLocator.GetInstance<ImportModuleBaseViewModel>(adapterName);
			}
			catch { }
			if (importModuleBaseViewModel == null) return;

			string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string adapterConfigFileName = @"\" + adapterName + ".config";
			this.ImportConfigPath = configPath;
			string configFilePath = configPath + adapterConfigFileName;

			this.LogText = System.IO.File.ReadAllText(configFilePath);
		}
	
		// ==================== Export PDA
		public virtual void RunExportPdaByConfigCommandExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.ClearLogText();
			this.RunExportPdaByConfig(exportPdaModuleInfo);
			string adapterName = exportPdaModuleInfo.Name;
			if (adapterName != ExportPdaAdapterName.ExportHT630Adapter 
			 && adapterName != ExportPdaAdapterName.ExportPdaMISAdapter)
			{
				try
				{
					this._toFTPViewModel.InitProperty(base.CurrentInventor, true, base.State);
				}
				catch (Exception ex)
				{
					base.LogImport.Add(MessageTypeEnum.Error, String.Format(" toFTPViewModel.InitProperty  ", ex.Message));
				}
			}
			this._sendToFtpCommand.RaiseCanExecuteChanged();
		}

		//Предполагает что есть адаптер импорта каталога
		//public virtual bool RunExportPdaByConfigCommandCanExecute(IExportPdaModuleInfo exportPdaModuleInfo)
		//{
		//	if (this.SelectedExportPda != null) return true;
		//	else return false;
		//}

		public void RunExportPdaByConfig(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.LogText = "";
			base.LogImport.Clear();
			using (new CursorWait("RunExportPdaByConfig"))
			{
				this.ExportPdaLogPath = "";
				this.ExportPdaConfigPath = "";
				if (exportPdaModuleInfo == null)
				{
					this.LogText = "ExportPdaModuleInfo if null";
					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", exportPdaModuleInfo.ImportDomainEnum.ToString(), exportPdaModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}

				this.ExportPdaLogPath = GetExportPdaLogPath(base.CurrentInventor, exportPdaModuleInfo);
				// если что есть функция  IExportPdaModule Get_ExportPdaModuleBaseViewModel_AsIExportPdaModule(string adapterName)
				ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel = null;

				string adapterName = exportPdaModuleInfo.Name;
				try
				{
					exportPdaModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportPdaModuleBaseViewModel>(adapterName);
				}
				catch { }

				if (exportPdaModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ExportPda ViewModel with Name [" + adapterName + "]";
					return;
				}

				exportPdaModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
				exportPdaModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;
				// ========
				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = base._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				this.ExportPdaConfigPath = configPath;
				string configFilePath = configPath + adapterConfigFileName;
				// =========

				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				//bool ret = importModuleBaseViewModel.CanImport();

				//ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
				// ?? Пока договорились что конфиг всегда берется из Customer проверяю гипотезу
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
		   
				if (exportPdaModuleInfo.ImportDomainEnum == ImportDomainEnum.ExportCatalog)
				{
					// ExportPdaModuleBaseViewModel	  is  IExportPdaModule
					ExportPdaWithModulesViewModel exportPdaWithModulesViewModel = base.ServiceLocator.GetInstance<ExportPdaWithModulesViewModel>();
					IExportPdaModule viewModel = exportPdaModuleBaseViewModel as IExportPdaModule;
					if (exportPdaWithModulesViewModel != null)
					{
						string complexAdapterName = "";
						if (SelectedComplex != null) complexAdapterName = SelectedComplex.Name;
						exportPdaWithModulesViewModel.ExportPdaByConfig(viewModel,
								 base.State,
								 adapterName,
								fromConfigXDoc,
								complexAdapterName, this);
					}
				}
			
			}	//using (new CursorWait())

			this.RefreshLogText(this.ExportPdaLogPath);

		}


		//========== CLEAR Pda ============================
		public virtual void RunExportPdaClearByConfigCommandExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.ClearLogText();

			this.RunExportPdaClearByConfig(exportPdaModuleInfo);
			string adapterName = exportPdaModuleInfo.Name;
			if (adapterName != ExportPdaAdapterName.ExportHT630Adapter
			 && adapterName != ExportPdaAdapterName.ExportPdaMISAdapter)
			{
				try
				{
					this._toFTPViewModel.InitProperty(base.CurrentInventor, true, base.State);
				}
				catch (Exception ex)
				{
					base.LogImport.Add(MessageTypeEnum.Error, String.Format(" toFTPViewModel.InitProperty  ", ex.Message));
				}
			}
			this._sendToFtpCommand.RaiseCanExecuteChanged();
		}

		public void RunExportPdaClearByConfig(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			base.LogImport.Clear();
			using (new CursorWait("RunExportPdaClearByConfig"))
			{
				this.ExportPdaLogPath = "";
				this.ExportPdaConfigPath = "";
				if (exportPdaModuleInfo == null)
				{
					this.LogText = "ExportPdaModuleInfo if null";

					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", exportPdaModuleInfo.ImportDomainEnum.ToString(), exportPdaModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}
				this.ExportPdaLogPath = GetExportPdaLogPath(base.CurrentInventor, exportPdaModuleInfo);
				ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel = null;

				string adapterName = exportPdaModuleInfo.Name;
				try
				{
					exportPdaModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportPdaModuleBaseViewModel>(adapterName);
				}
				catch { }

				if (exportPdaModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ExportPda ViewModel with Name [" + adapterName + "]";
					return;
				}

				exportPdaModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
				exportPdaModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;

				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = base._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				this.ExportPdaConfigPath = configPath;
				string configFilePath = configPath + adapterConfigFileName;
				// =========

				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				// ?? Пока договорились что конфиг всегда берется из Customer
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;

				if (exportPdaModuleInfo.ImportDomainEnum == ImportDomainEnum.ExportCatalog)
				{
					// ExportPdaModuleBaseViewModel	  is  IExportPdaModule
					ExportPdaWithModulesViewModel exportPdaWithModulesViewModel = base.ServiceLocator.GetInstance<ExportPdaWithModulesViewModel>();
					IExportPdaModule viewModel = exportPdaModuleBaseViewModel as IExportPdaModule;
					if (exportPdaWithModulesViewModel != null)
					{
						exportPdaWithModulesViewModel.ClearExportPdaByConfig(viewModel,
								 base.State,
								 adapterName,
								fromConfigXDoc);
					}
				}
			 } //  using (new CursorWait())

			//this.RefreshLogText(this.ExportPdaLogPath);
			string logtext = base.LogImport.PrintLog();
			Utils.RunOnUI(() =>
			{
				this.LogText = logtext;
			});

		}
		// ============================= LOG  Pda
		public virtual void ShowExportPdaLogCommandExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			using (new CursorWait("ShowExportPdaLogCommand"))
			{
				string exportPdaLogPath = RunExportPdaShowLogByConfig(exportPdaModuleInfo);
				if (string.IsNullOrWhiteSpace(exportPdaLogPath) == false)
				{
					this.RefreshLogText(exportPdaLogPath);
				}
				else
				{
					this.LogText = "";
				}
			}

			
		}

		public string RunExportPdaShowLogByConfig(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.ExportPdaLogPath = "";
			this.LogText = "";
			if (exportPdaModuleInfo == null) return "";
			if (base.CurrentInventor == null) return "";
			this.ExportPdaLogPath = GetExportPdaLogPath(base.CurrentInventor, exportPdaModuleInfo);
			return this.ExportPdaLogPath;
		}


		// =============== Show Config	Pda
		public virtual void ShowExportPdaConfigCommandExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.RunExportPdaShowConfigFile(exportPdaModuleInfo);
		}

		public virtual void RunExportPdaShowConfigFile(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			this.ExportPdaConfigPath = "";
			this.LogText = "";
			if (exportPdaModuleInfo == null)
			{
				this.LogText = "ExportPdaModuleInfo is null";
				return;
			}

			if (base.CurrentInventor == null)
			{
				this.LogText = "CurrentInventor is null";
				return;
			}

			string adapterName = exportPdaModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true)
			{
				this.LogText = "AdapterName Is NullOrWhiteSpace ";
				return;
			} 
			
			ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel = null;
			try
			{
				exportPdaModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportPdaModuleBaseViewModel>(adapterName);
			}
			catch { }
			if (exportPdaModuleBaseViewModel == null) return;

			string configPath = base._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string adapterConfigFileName = @"\" + adapterName + ".config";
			this.ExportPdaConfigPath = configPath;
			string configFilePath = configPath + adapterConfigFileName;

			this.LogText = System.IO.File.ReadAllText(configFilePath);
		}

		// ==================== Export Erp
		
		//public void SaveExportErpConfigForCustomer(Customer customer, IExportErpModuleInfo exportErpModuleInfo) //base.CurrentCustomer this.SelectedExportErp
		//{
		//	if (exportErpModuleInfo == null) return;	  //from customer
		//	ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;
		//	string adapterName = exportErpModuleInfo.Name;
		//	try
		//	{
		//		exportErpModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportErpModuleBaseViewModel>(adapterName);
		//	}
		//	catch { }

		//	if (exportErpModuleBaseViewModel == null)
		//	{
		//		return;
		//	}

		//	//IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
		//	ExportErpWithModulesViewModel exportErpWithModulesViewModel = base.ServiceLocator.GetInstance<ExportErpWithModulesViewModel>();
		//	exportErpWithModulesViewModel.SaveConfigByDefaultForCustomer(customer, exportErpModuleInfo, exportErpModuleBaseViewModel);
		//}

		public virtual void RunExportErpByConfigCommandExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			using (new CursorWait("RunExportErpByConfig"))
			{
				ClearLogText();
				RunExportErpByConfig(exportErpModuleInfo);
				CopyExportErpFolderFormInventorToFixed();
			}
		}

		public virtual void RunExportErpByConfig(IExportErpModuleInfo exportErpModuleInfo)
		{
			this.LogText = "";
			base.LogImport.Clear();
			//using (new CursorWait("RunExportErpByConfig"))
			//{
				this.ExportErpLogPath = "";
				this.ExportErpConfigPath = "";
				if (exportErpModuleInfo == null)
				{
					this.LogText = "ExportErpModuleInfo if null";
					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", " ExportErpAdapter ", exportErpModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}

				this.ExportErpLogPath = GetExportErpLogPath("Inventor", base.CurrentInventor.Code, exportErpModuleInfo);
				ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;

				string adapterName = exportErpModuleInfo.Name;
				try
				{
					exportErpModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportErpModuleBaseViewModel>(adapterName);
				}
				catch { }

				if (exportErpModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ExportErp ViewModel with Name [" + adapterName + "]";
					return;
				}

				exportErpModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
				exportErpModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;
				// ========
				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				this.ExportErpConfigPath = configPath;
				string configFilePath = configPath + adapterConfigFileName;
				// =========

				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				//bool ret = importModuleBaseViewModel.CanImport();

				//ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
				// ?? Пока договорились что конфиг всегда берется из Customer проверяю гипотезу
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;


				ExportErpWithModulesViewModel exportErpWithModulesViewModel = base.ServiceLocator.GetInstance<ExportErpWithModulesViewModel>();
				IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
				if (exportErpWithModulesViewModel != null)
				{
					string complexAdapterName = "";
					if (SelectedComplex != null) complexAdapterName = SelectedComplex.Name;
					
					exportErpWithModulesViewModel.ExportErpByConfig(viewModel,
								 base.State,
								 adapterName,
								fromConfigXDoc,
								complexAdapterName, this);
				}

			//}	//using (new CursorWait())

			this.RefreshLogText(this.ExportErpLogPath);

		}


		//========== CLEAR Erp ============================
		public virtual void RunExportErpClearByConfigCommandExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			using (new CursorWait("RunExportErpClearByConfig"))
			{
				ClearLogText();
				RunExportErpClearByConfig(exportErpModuleInfo);
			}
		}

		public void RunExportErpClearByConfig(IExportErpModuleInfo exportErpModuleInfo)
		{
			base.LogImport.Clear();
			//using (new CursorWait("RunExportErpClearByConfig"))
			//{
				this.ExportErpLogPath = "";
				this.ExportErpConfigPath = "";
				if (exportErpModuleInfo == null)
				{
					this.LogText = "ExportErpModuleInfo if null";

					return;
				}
				base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Adapter : {0}  {1}", "ExportErp ", exportErpModuleInfo.Name));

				if (base.CurrentInventor == null)
				{
					this.LogText = "CurrentInventor if null";
					return;
				}
				this.ExportErpLogPath = GetExportErpLogPath("Inventor", base.CurrentInventor.Code, exportErpModuleInfo);

				ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;

				string adapterName = exportErpModuleInfo.Name;
				try
				{
					exportErpModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportErpModuleBaseViewModel>(adapterName);
				}
				catch { }

				if (exportErpModuleBaseViewModel == null)
				{
					this.LogText = "Can't resolve ExportErp ViewModel with Name [" + adapterName + "]";
					return;
				}

				exportErpModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
				exportErpModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;

				// ?? Пока договорились что конфиг всегда берется из Customer
				string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string adapterConfigFileName = @"\" + adapterName + ".config";
				this.ExportErpConfigPath = configPath;
				string configFilePath = configPath + adapterConfigFileName;
				// =========

				if (File.Exists(configFilePath) == false)
				{
					this.LogText = "Config file not Exists";
					return;
				}

				// ?? Пока договорились что конфиг всегда берется из Customer
				ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;

				ExportErpWithModulesViewModel exportErpWithModulesViewModel = base.ServiceLocator.GetInstance<ExportErpWithModulesViewModel>();
				IExportErpModule viewModel = exportErpModuleBaseViewModel as IExportErpModule;
				if (exportErpWithModulesViewModel != null)
				{
					exportErpWithModulesViewModel.ClearExportErpByConfig(viewModel,
							 base.State,
							 adapterName,
							fromConfigXDoc);
				}

			//} //  using (new CursorWait())

			//this.RefreshLogText(this.ExportErpLogPath);
			string logtext = base.LogImport.PrintLog();
			Utils.RunOnUI(() =>
			{
				this.LogText = logtext;
			});
		}

		// ============================= LOG  Erp
		public virtual void ShowExportErpLogCommandExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			string exportErpLogPath = RunExportErpShowLogByConfig(exportErpModuleInfo);

			if (string.IsNullOrWhiteSpace(exportErpLogPath) == false)
			{
				RefreshLogText(exportErpLogPath);
			}
			else
			{
				this.LogText = "";
			}
		}
									   
		public virtual string RunExportErpShowLogByConfig(IExportErpModuleInfo exportErpModuleInfo)
		{
			this.ExportErpLogPath = "";
			this.LogText = "";
			if (exportErpModuleInfo == null) return "";

			if (base.CurrentInventor == null) return "";

			this.ExportErpLogPath = GetExportErpLogPath("Inventor", base.CurrentInventor.Code, exportErpModuleInfo);
			return this.ExportErpLogPath;
			//string s = System.IO.File.ReadAllText(@"fileName.txt", Encoding.Default).Replace("\n", " ");
			
		}


		// =============== Show Config Erp
		public virtual void ShowExportErpConfigCommandExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			this.RunExportErpShowConfigFile(exportErpModuleInfo);
		}

		public virtual void RunExportErpShowConfigFile(IExportErpModuleInfo exportErpModuleInfo)
		{
			this.ExportErpConfigPath = "";
			this.LogText = "";
			if (exportErpModuleInfo == null)
			{
				this.LogText = "ExportErpModuleInfo is null";
				return;
			}

			if (base.CurrentInventor == null)
			{
				this.LogText = "CurrentInventor is null";
				return;
			}

			string adapterName = exportErpModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == true)
			{
				this.LogText = "AdapterName Is NullOrWhiteSpace ";
				return;
			}

			ExportErpModuleBaseViewModel exportErpModuleBaseViewModel = null;
			try
			{
				exportErpModuleBaseViewModel = base.ServiceLocator.GetInstance<ExportErpModuleBaseViewModel>(adapterName);
			}
			catch { }
			if (exportErpModuleBaseViewModel == null) return;

			// ?? Пока договорились что конфиг всегда берется из Customer
			string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			string adapterConfigFileName = @"\" + adapterName + ".config";
			this.ExportErpConfigPath = configPath;
			string configFilePath = configPath + adapterConfigFileName;

			this.LogText = System.IO.File.ReadAllText(configFilePath);
		}

		//TODO	   Сдалеть проверку файла и копированию по умолчанию
		public virtual bool RunSendToOfficeCommandCanExecute(ResultModuleInfo importModuleInfo)
		{
			return true;
		}

		public virtual void RunSendToOfficeCommandExecuted(ResultModuleInfo importModuleInfo)
		{
			InventorChangeStatusViewModel inventorChangeStatusViewModel = null;
			try
			{
				inventorChangeStatusViewModel = base.ServiceLocator.GetInstance<InventorChangeStatusViewModel>(Common.ViewModelNames.InventorChangeStatusViewModel);
			}
			catch { }
			if (inventorChangeStatusViewModel == null) return;
			using (new CursorWait("RunSendToOfficeCommandExecuted"))
			{
				RunExportErpClearByConfig(this.SelectedExportErp);
				RunExportErpByConfig(this.SelectedExportErp);

				string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				string configPathWithFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
				inventorChangeStatusViewModel.InitFromState(base.State, configPathWithFile);
				this._selectedSendToOffice.ZipPath = inventorChangeStatusViewModel.ZipPath;

				//TODO TEST
				inventorChangeStatusViewModel.SendDataToOfficeWitoutGUICommandExecuted(this._selectedSendToOffice.ZipPath, base.State);

				
				//if (inventorChangeStatusViewModel.IsBusy == false)
				//{

					//inventorChangeStatusViewModel.IsBusy = true;

					//Task.Factory.StartNew(inventorChangeStatusViewModel.BuildZip, Thread.CurrentThread.CurrentUICulture).LogTaskFactoryExceptions("SendDataCommandExecuted");
					//inventorChangeStatusViewModel.BuildZipWithoutGUI(
					//	this._selectedSendToOffice.ZipPath, //this.ZipPath,
					//	Thread.CurrentThread.CurrentUICulture,
					//	base.State
					//	//,
					//	//includeInventorDBSdf: false,
					//	//includePackOf: false,
					//	//includeEndOfInventorFiles: true,
					//	//isRunExportErpAdapter: false  //!!! false так как запускаем как надо пере тем как
					//	);
				//}

			}
			this.ZipPath = Path.GetDirectoryName(inventorChangeStatusViewModel.ZipPath);
			this._openZipPathCommand.RaiseCanExecuteChanged();

		}

		public virtual void ShowSendToOfficeIniCommandExecuted(ResultModuleInfo resultModuleInfo)
		{
			ShowSendToOfficeIni(resultModuleInfo);
		}

		public virtual void ShowSendToOfficeIni(ResultModuleInfo resultModuleInfo)
		{
			string fileConfig = "";
			object currentDomainObject = null;
			currentDomainObject = base.CurrentCustomer;

			//this.DataInConfigPath = resultModuleInfo.ConfigPath;
			//this.DataInConfigPathWithFile = resultModuleInfo.ConfigPathWithFile;

			if (File.Exists(resultModuleInfo.ConfigPathWithFile) == true)
			{
				try
				{
					using (StreamReader sr = new StreamReader(resultModuleInfo.ConfigPathWithFile))
					{
						this.LogText = sr.ReadToEnd();
					}
				}
				catch (Exception exp)
				{
					this.LogText = "Error Load Xml form file : " + fileConfig + " :  " + exp.Message;
				}
			}
			else
			{
				this.LogText = "";
			}

		}

		public virtual void NavigateToGridImportCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

			switch (importModuleInfo.ImportDomainEnum)
			{
				case ImportDomainEnum.ImportItur:
					UtilsNavigate.IturimAddEditDeleteOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportLocation:
					UtilsNavigate.LocationAddEditDeleteOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportCatalog:
					UtilsNavigate.CatalogOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportSection:
					UtilsNavigate.SectionAddEditDeleteOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportSupplier:
					UtilsNavigate.SupplierAddEditDeleteOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportFamily:
					UtilsNavigate.FamilyAddEditDeleteOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.UpdateCatalog:
					UtilsNavigate.CatalogOpen(this._regionManager, query);
					break;
				case ImportDomainEnum.ImportInventProduct:
					UtilsNavigate.InventProductListOpen(base._regionManager, query);
					break;
			}
		}

	

	

		public ResultModuleInfo SelectedSendToOffice
		{
			get { return this._selectedSendToOffice; }
			set
			{
				this._selectedSendToOffice = value;
				RaisePropertyChanged(() => SelectedSendToOffice);
			}
		}

		
		public IImportModuleInfo SelectedComplex
		{
			get { return this._selectedComplex; }
		}

		public string SelectedCatalogName { get; set; }
		public string SelectedCatalogTitle { get; set; } 
		public IImportModuleInfo SelectedCatalog
		{
			get { return this._selectedCatalog; }
			set
			{
				this._selectedCatalog = value;
				SelectedCatalogName = String.Empty;
				if (this._selectedCatalog != null) SelectedCatalogName = _selectedCatalog.Name;
				RaisePropertyChanged(() => SelectedCatalogName);

				SelectedCatalogTitle = String.Empty;
				if (this._selectedCatalog != null) SelectedCatalogTitle = _selectedCatalog.Title;
				RaisePropertyChanged(() => SelectedCatalogTitle);
				RaisePropertyChanged(() => SelectedCatalog);
				RaisePropertyChanged(() => ImportFixedPath);
				RaisePropertyChanged(() => ExportErpFixedPath);
				
												  
				//if (!_isEventFromCode)
				//{
				//	if (_selectedCatalogs != null)
				//		this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });
				//}

				//SetSupplierAdapterOnCatalogAdapterChange();
			}
		}


		public string SelectedUpdateCatalogName { get; set; }
		public string SelectedUpdateCatalogTitle { get; set; }
		public IImportModuleInfo SelectedUpdateCatalog
		{
			get { return this._selectedUpdateCatalog; }
			set
			{
				this._selectedUpdateCatalog = value;
				SelectedUpdateCatalogName = String.Empty;
				if (this._selectedUpdateCatalog != null) SelectedUpdateCatalogName = _selectedUpdateCatalog.Name;
				RaisePropertyChanged(() => SelectedUpdateCatalogName);

				SelectedUpdateCatalogTitle = String.Empty;
				if (this._selectedUpdateCatalog != null) SelectedUpdateCatalogTitle = _selectedUpdateCatalog.Title;
				RaisePropertyChanged(() => SelectedUpdateCatalogTitle);
				RaisePropertyChanged(() => SelectedUpdateCatalog);

				//if (!_isEventFromCode)
				//{
				//	if (_selectedCatalogs != null)
				//		this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });
				//}

				//SetSupplierAdapterOnCatalogAdapterChange();
			}
		}


		public string SelectedIturName { get; set; }
		public string SelectedIturTitle { get; set; } 
		public IImportModuleInfo SelectedItur
		{
			get { return this._selectedItur; }
			set
			{
				this._selectedItur = value;
				SelectedIturName = String.Empty;
				if (this._selectedItur != null) SelectedIturName = _selectedItur.Name;
				RaisePropertyChanged(() => SelectedIturName);

				SelectedIturTitle = String.Empty;
				if (this._selectedItur != null) SelectedIturTitle = _selectedItur.Title;
				RaisePropertyChanged(() => SelectedIturTitle);

				RaisePropertyChanged(() => SelectedItur);



				//if (_selectedIturs != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}


		public string SelectedLocationName { get; set; }
		public string SelectedLocationTitle { get; set; } 
		public IImportModuleInfo SelectedLocation
		{
			get { return this._selectedLocation; }
			set
			{
				this._selectedLocation = value;

				SelectedLocationName = String.Empty;
				if (this._selectedLocation != null) SelectedLocationName = _selectedLocation.Name;
				RaisePropertyChanged(() => SelectedLocationName);

				SelectedLocationTitle = String.Empty;
				if (this._selectedLocation != null) SelectedLocationTitle = _selectedLocation.Title;
				RaisePropertyChanged(() => SelectedLocationTitle);

				RaisePropertyChanged(() => SelectedLocation);

				//if (_selectedLocations != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}


		public string SelectedSectionName { get; set; }
		public string SelectedSectionTitle { get; set; } 
		public IImportModuleInfo SelectedSection
		{
			get { return _selectedSection; }
			set
			{
				_selectedSection = value;
				SelectedSectionName = String.Empty;
				if (this._selectedSection != null) SelectedSectionName = _selectedSection.Name;
				RaisePropertyChanged(() => SelectedSectionName);

				SelectedSectionTitle = String.Empty;
				if (this._selectedSection != null) SelectedSectionTitle = _selectedSection.Title;
				RaisePropertyChanged(() => SelectedSectionTitle);

				RaisePropertyChanged(() => SelectedSection);

				//if (_selectedSections != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}

		public string SelectedFamilyName { get; set; }
		public string SelectedFamilyTitle { get; set; } 
		public IImportModuleInfo SelectedFamily
		{
			get { return _selectedFamily; }
			set
			{
				_selectedFamily = value;
				SelectedFamilyName = String.Empty;
				if (this._selectedFamily != null) SelectedFamilyName = _selectedFamily.Name;
				RaisePropertyChanged(() => SelectedFamilyName);

				SelectedFamilyTitle = String.Empty;
				if (this._selectedFamily != null) SelectedFamilyTitle = _selectedFamily.Title;
				RaisePropertyChanged(() => SelectedFamilyTitle);


				RaisePropertyChanged(() => SelectedFamily);

				//if (_selectedFamily != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}

		public string SelectedSupplierName { get; set; }
		public string SelectedSupplierTitle { get; set; } 
		public IImportModuleInfo SelectedSupplier
		{
			get { return _selectedSupplier; }
			set
			{
				_selectedSupplier = value;
				SelectedSupplierName = String.Empty;
				if (this._selectedSupplier != null) SelectedSupplierName = _selectedSupplier.Name;
				RaisePropertyChanged(() => SelectedSupplierName);

				SelectedSupplierTitle = String.Empty;
				if (this._selectedSupplier != null) SelectedSupplierTitle = _selectedSupplier.Title;
				RaisePropertyChanged(() => SelectedSupplierTitle);

				RaisePropertyChanged(() => SelectedSupplier);

				//if (_selectedSuppliers != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}

		public string SelectedImportFromPDAName { get; set; }
		public string SelectedImportFromPDATitle { get; set; } 
		public IImportModuleInfo SelectedImportFromPDA
		{
			get { return _selectedImportFromPDA; }
			set
			{
				_selectedImportFromPDA = value;
				SelectedImportFromPDAName = String.Empty;
				if (this._selectedImportFromPDA != null) SelectedImportFromPDAName = _selectedImportFromPDA.Name;
				RaisePropertyChanged(() => SelectedImportFromPDAName);

				SelectedImportFromPDATitle = String.Empty;
				if (this._selectedImportFromPDA != null) SelectedImportFromPDATitle = _selectedImportFromPDA.Title;
				RaisePropertyChanged(() => SelectedImportFromPDATitle);

				RaisePropertyChanged(() => SelectedImportFromPDA);

				//if (_selectedPDA != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}

		public string SelectedExportPDAName { get; set; }
		public string SelectedExportPDATitle { get; set; }
		public IExportPdaModuleInfo SelectedExportPda
		{
			get { return _selectedExportPda; }
			set
			{
				_selectedExportPda = value;
				SelectedExportPDAName = String.Empty;
				if (this._selectedExportPda != null) SelectedExportPDAName = _selectedExportPda.Name;
				RaisePropertyChanged(() => SelectedExportPDAName);

				SelectedExportPDATitle = String.Empty;
				if (this._selectedExportPda != null) SelectedExportPDATitle = _selectedExportPda.Title;
				RaisePropertyChanged(() => SelectedExportPDATitle);

				RaisePropertyChanged(() => SelectedExportPda);

				//if (_selectedPDA != null)
				//	this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });

			}
		}


		public string SelectedExportErpName { get; set; }
		public string SelectedExportErpTitle { get; set; }
		public IExportErpModuleInfo SelectedExportErp
		{		 
			get { return _selectedExportErp; }
			set
			{
				_selectedExportErp = value;
				SelectedExportErpName = String.Empty;
				if (this._selectedExportErp != null) SelectedExportErpName = _selectedExportErp.Name;
				RaisePropertyChanged(() => SelectedExportErpName);

				SelectedExportErpTitle = String.Empty;
				if (this._selectedExportErp != null) SelectedExportErpTitle = _selectedExportErp.Title;
				RaisePropertyChanged(() => SelectedExportErpTitle);

				RaisePropertyChanged(() => SelectedExportErp);
			}
		}

		public string SelectedInfrastructuraImportTitle
		{
			get
			{
				return this._selectedInfrastructuraImportTitle;
			}
			set
			{
				this._selectedInfrastructuraImportTitle = value.Trim();
				RaisePropertyChanged(() => SelectedInfrastructuraImportTitle);
			}
		}


		public string SelectedInfrastructuraImportTooltip
		{
			get
			{
				return this._selectedInfrastructuraImportTooltip;
			}
			set
			{
				this._selectedInfrastructuraImportTooltip = value;
				RaisePropertyChanged(() => SelectedInfrastructuraImportTooltip);
			}
		}

		public virtual void FillAdaptersNameFromCustomer(Customer customer)
		{
		
			// ============== Import Adapters =========================
			// ============== Customer ================================
			if (customer != null)
			{
				string catalogCode = customer.ImportCatalogProviderCode;
				this._configFileImportCatalogExists = false;
				if (!String.IsNullOrEmpty(catalogCode) && _itemsCatalogs.Any(r => r.Name == catalogCode))
					SelectedCatalog = _itemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);
				else
					SelectedCatalog = _itemsCatalogs.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportCatalogExists = IsConfigFileImportExists(this.SelectedCatalog);

				this._configFileImportUpdateCatalogExists = false;
				if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
					SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
				else
					SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.IsDefault);
				ConfigFileImportUpdateCatalogExists = IsConfigFileImportExists(this.SelectedUpdateCatalog);

				string iturCode = customer.ImportIturProviderCode;
				if (!String.IsNullOrEmpty(iturCode) && _itemsIturs.Any(r => r.Name == iturCode))
					SelectedItur = _itemsIturs.FirstOrDefault(r => r.Name == iturCode);
				else
					SelectedItur = _itemsIturs.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportIturExists = IsConfigFileImportExists(this.SelectedItur);

				string locationCode = customer.ImportLocationProviderCode;
				if (!String.IsNullOrEmpty(locationCode) && _itemsLocations.Any(r => r.Name == locationCode))
					SelectedLocation = _itemsLocations.FirstOrDefault(r => r.Name == locationCode);
				else
					SelectedLocation = _itemsLocations.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportLocationExists = IsConfigFileImportExists(this.SelectedLocation);

				string sectionCode = customer.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && _itemsSections.Any(r => r.Name == sectionCode))
					SelectedSection = _itemsSections.FirstOrDefault(r => r.Name == sectionCode);
				else
					SelectedSection = _itemsSections.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportSectionExists = IsConfigFileImportExists(this.SelectedSection);

				string familyCode = customer.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && _itemsFamilys.Any(r => r.Name == familyCode))
					SelectedFamily = _itemsFamilys.FirstOrDefault(r => r.Name == familyCode);
				else
					SelectedFamily = _itemsFamilys.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportFamilyExists = IsConfigFileImportExists(this.SelectedFamily);

				string supplierCode = customer.ImportSupplierAdapterCode;
				if (!String.IsNullOrEmpty(supplierCode) && _itemsSuppliers.Any(r => r.Name == supplierCode))
					SelectedSupplier = _itemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);
				else
					SelectedSupplier = _itemsSuppliers.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportSupplierExists = IsConfigFileImportExists(this.SelectedSupplier);

				string importPdaCode = customer.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(importPdaCode) && _itemsPDA.Any(r => r.Name == importPdaCode))
					SelectedImportFromPDA = _itemsPDA.FirstOrDefault(r => r.Name == importPdaCode);
				else
					SelectedImportFromPDA = _itemsPDA.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportPDAExists = IsConfigFileImportExists(this.SelectedImportFromPDA);

				// ==============  Export PDA
				string exportPdaCode = customer.ExportCatalogAdapterCode;
				IExportPdaModuleInfo defaultExportPda = _itemsExportPda.FirstOrDefault(r => r.IsDefault);
				if (String.IsNullOrWhiteSpace(exportPdaCode) == false)
				{
					if (this._itemsExportPda.Any(r => r.Name == exportPdaCode))
						defaultExportPda = _itemsExportPda.FirstOrDefault(r => r.Name == exportPdaCode);
				}
				this.SelectedExportPda = defaultExportPda;

				ConfigFileExportPDAExists = IsConfigFileExportPdaExists(this.SelectedExportPda);

				// ==============  Export Erp
				string exportErpCode = customer.ExportERPAdapterCode;
				IExportErpModuleInfo defaultExportErp = _itemsExportErp.FirstOrDefault(r => r.IsDefault);
				if (String.IsNullOrWhiteSpace(exportErpCode) == false)
				{
					if (this._itemsExportErp.Any(r => r.Name == exportErpCode))
						defaultExportErp = _itemsExportErp.FirstOrDefault(r => r.Name == exportErpCode);
				}
				this.SelectedExportErp = defaultExportErp;
				ConfigFileExportErpExists = IsConfigFileExportErpExists(this.SelectedExportErp);

				// ==============  Zip =====================
				ResultModuleInfo resultModuleInfo = new ResultModuleInfo();
				resultModuleInfo.ZipPath = ZipPath;
				 string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				 resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
				//resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
				this.IniSendToOfficePath = System.IO.Path.GetDirectoryName(resultModuleInfo.ConfigPathWithFile) ;
				resultModuleInfo.ConfigPath = "";
				if (string.IsNullOrWhiteSpace(resultModuleInfo.ConfigPathWithFile) == false)
				{
					resultModuleInfo.ConfigPath = System.IO.Path.GetDirectoryName(resultModuleInfo.ConfigPathWithFile);
				}

				this.SelectedSendToOffice = resultModuleInfo;


				this.SelectedInfrastructuraImportTitle = this.GetTitleImportAdapters();
				SelectedInfrastructuraImportTooltip = this.GetTooltipImportAdapters();
			}



		}


		public string GetTitleImportAdapters()
		{
			List<string> list = GetTitleListOfImportAdaptersWithConfig();
			string title = list.JoinRecord(" >> ");
			title = title.TrimEnd(" >> ".ToCharArray());
			return title.Trim();
		}

		public string GetTooltipImportAdapters()
		{
			List<string> list = GetTooltipListOfImportAdaptersWithConfig();
			string title = list.JoinRecord(" >> ");
			title = title.TrimEnd(" >> ".ToCharArray());
			return title;
		}
		
		


		public string ZipPath
		{
			get { return _zipPath; }
			set
			{
				_zipPath = value;
				RaisePropertyChanged(() => ZipPath);
				this._openZipPathCommand.RaiseCanExecuteChanged();
			}
		}

		public string IniSendToOfficePath
		{
			get { return _iniSendToOfficePath; }
			set
			{
				_iniSendToOfficePath = value;
				RaisePropertyChanged(() => IniSendToOfficePath);
				this._openIniSendToOfficePathCommand.RaiseCanExecuteChanged();
			}
		}

		//public virtual void SetDefaultZipPath()
		//{
		//	string sendDataFolderPath = UtilsPath.ZipOfficeFolder(_dbSettings);

		//	string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
		//	string sendDataZipFileName = String.Format("{0}_{1}_{2}.zip",
		//											   base.CurrentCustomer.Code,
		//											   base.CurrentBranch.BranchCodeERP,
		//											   date);

		//	string sendDataZipFilePath = System.IO.Path.Combine(sendDataFolderPath, sendDataZipFileName);

		//	this.ZipPath = sendDataZipFilePath;
		//}

		public string ImportLogPath
		{
			get { return _importLogPath; }
			set
			{
				_importLogPath = value;
				RaisePropertyChanged(() => ImportLogPath);
				this._openImportLogPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string ImportFixedPath
		{
			get { return _importFixedPath; }
			set
			{
				_importFixedPath = value;
				RaisePropertyChanged(() => ImportFixedPath);
				this._openImportFixedPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string ExportErpFixedPath
		{
			get { return _exportErpFixedPath; }
			set
			{
				_exportErpFixedPath = value;
				RaisePropertyChanged(() => ExportErpFixedPath);
				this._openExportErpFixedPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string SendToOfficeFixedPath
		{
			get { return _sendToOfficeFixedPath; }
			set
			{
				_sendToOfficeFixedPath = value;
				RaisePropertyChanged(() => SendToOfficeFixedPath);
				this._openSendToOfficeFixedPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string ImportInDataPath
		{
			get { return _importInDataPath; }
			set
			{
				_importInDataPath = value;
				RaisePropertyChanged(() => ImportInDataPath);
				this._openImportInDataPathCommand.RaiseCanExecuteChanged();
			}
		}

		public string ImportConfigPath
		{
			get { return _importConfigPath; }
			set
			{
				_importConfigPath = value;
				RaisePropertyChanged(() => ImportConfigPath);
				this._openImportConfigPathCommand.RaiseCanExecuteChanged();
			}
		}

		public string ExportPdaLogPath
		{
			get { return _exportPdaLogPath; }
			set
			{
				_exportPdaLogPath = value;
				RaisePropertyChanged(() => ExportPdaLogPath);
				this._openExportPdaLogPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string ExportPdaConfigPath
		{
			get { return _exportPdaConfigPath; }
			set
			{
				_exportPdaConfigPath = value;
				RaisePropertyChanged(() => ExportPdaConfigPath);
				this._openExportPdaConfigPathCommand.RaiseCanExecuteChanged();
			}
		}

		public string ExportErpLogPath
		{
			get { return _exportErpLogPath; }
			set
			{
				_exportErpLogPath = value;
				RaisePropertyChanged(() => ExportErpLogPath);
				this._openExportErpLogPathCommand.RaiseCanExecuteChanged();
			}
		}


		public string ExportErpConfigPath
		{
			get { return _exportErpConfigPath; }
			set
			{
				_exportErpConfigPath = value;
				RaisePropertyChanged(() => ExportErpConfigPath);
				this._openExportErpConfigPathCommand.RaiseCanExecuteChanged();
			}
		}

		public string LogText
		{
			get { return _logText; }
			set
			{
				_logText = value;
				RaisePropertyChanged(() => LogText);

			}
		}


	
		// BASE adapter function
		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			//if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			//{
			//	string configPath = this.GetXDocumentConfigPath(ref info);		//в нескольких местах - надо решить
			//	XDocument configXDoc = new XDocument();
			//	if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
			//	{
			//		try
			//		{
			//			configXDoc = XDocument.Load(configPath);
			//			XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

			//			string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
			//			base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						
			//		}
			//		catch (Exception exp)
			//		{
			//			base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
			//		}
			//	}
			//	else
			//	{
			//		base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
			//	}
			//}
		}

	


        protected override void RunImport(ImportCommandInfo info)
        {
            this.Import();
        }

        protected override void RunClear()
        {
            this.Clear();
        }

		public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
		{
			get { return _yesNoRequest; }
		}


		//public List<string> NewDocumentCodeList
		//{
		//	get
		//	{
		//		//ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
		//		//List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);
		//		//return newDocumentCodeList;
		//		IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
		//		System.Collections.Generic.List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
		//		return docCodes; 
		//	}
		//}

		
		public List<string> NewSessionCodeList
		{
			get { return this._newSessionCodeList; }
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);
			this._newSessionCodeList.Clear();

			this.FillAdaptersList();

			this.FillLogPath();

			this.FillInDataPath();

			this.FillConfigPath();

			this.FillAdaptersNameFromCustomer(base.CurrentCustomer);
		
			// если что надо переинициализировать в конкретном адаптере
			// подумаем 3 дня
			this.InitAdapterCatalogFromConfig(this.SelectedCatalog);
			//ResultModuleInfo SelectedSendToOffice
			//IExportErpModuleInfo SelectedExportErp
			this.InitAdapterFixedPathFromCustomer(base.CurrentCustomer);

			string adapterName = this.SelectedExportPda.Name;
				if (adapterName != ExportPdaAdapterName.ExportHT630Adapter
				 && adapterName != ExportPdaAdapterName.ExportPdaMISAdapter)
				{
					try
					{
						this._toFTPViewModel.InitProperty(base.CurrentInventor, true, base.State);
					}
					catch(Exception ex) 
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format(" toFTPViewModel.InitProperty  ", ex.Message));
					}
				}

			this._sendToFtpCommand.RaiseCanExecuteChanged();

			adapterName = this.SelectedImportFromPDA.Name;
			if (adapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
				|| adapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
				|| adapterName == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
				|| adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
			{
				try
				{
					this._fromFTPViewModel.InitProperty(base.CurrentInventor, true, base.State, adapterName);
				}
				catch (Exception ex)
				{
					base.LogImport.Add(MessageTypeEnum.Error, String.Format(" fromFTPViewModel.InitProperty  ", ex.Message));
				}
			}

			this.IsBusyGetFromFtp = false;
			this._getFromFtpCommand.RaiseCanExecuteChanged();
		}

		public void InitAdapterCatalogFromConfig(IImportModuleInfo selectedCatalog)
		{
			if (selectedCatalog == null) return;
			ImportCommandInfo info = new ImportCommandInfo();
			info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
			info.AdapterName = selectedCatalog.Name;
			InitCatalogFromConfig(info, this.State);
		}

		public void InitAdapterFixedPathFromCustomer(Customer currentCustomer)
		{
			this._exportErpFixedPath = "";
			this._sendToOfficeFixedPath = "";
			if (currentCustomer == null) return ;
			//===================== Static Path1	/Customer.MaskCode
			string fixedImport = "";
			if (string.IsNullOrWhiteSpace(base.State.CurrentCustomer.ImportCatalogPath) == false)		//Static Path2
			{
				fixedImport = base.State.CurrentCustomer.ImportCatalogPath;
				try
				{
					if (Directory.Exists(fixedImport) == false)
					{
						Directory.CreateDirectory(fixedImport);
					}
				}
				catch { }
			}
			this.ImportFixedPath = fixedImport;
			//===================== Static Path2	/Customer.MaskCode
			string fixedExportErp = "";
			if (string.IsNullOrWhiteSpace(base.State.CurrentCustomer.ExportErpPath) == false)		//Static Path2
			{
				fixedExportErp = base.State.CurrentCustomer.ExportErpPath;
				try{
						if (Directory.Exists(fixedExportErp) == false)
						{
							Directory.CreateDirectory(fixedExportErp);
						}
				}catch{}
			}
			this.ExportErpFixedPath = fixedExportErp;

			//===================== Static Path2	/Customer.ReportPath
			string fixedZip = "";
			if (string.IsNullOrWhiteSpace(base.State.CurrentCustomer.SendToOfficePath) == false)		   //Static Path3
			{
				fixedZip = base.State.CurrentCustomer.SendToOfficePath;
				try{
						if (Directory.Exists(fixedZip) == false)
						{
							Directory.CreateDirectory(fixedZip);
						}
				}catch{}
			}
			else
			{
				fixedZip = UtilsPath.ZipOfficeFolder(_dbSettings);
			}
			this.SendToOfficeFixedPath = fixedZip;
		}

		protected void FillLogPath()
		{
			try
			{
			this.ImportLogPath = base.ContextCBIRepository.GetImportLogDomainObjectFolderPath(base.CurrentInventor, @"\Log");
			this.ExportPdaLogPath = base.ContextCBIRepository.GetExportToPDAFolderPath(base.CurrentInventor, true);
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error FillLogPath  ", ex.Message));
			}
		}

		protected void FillInDataPath()
		{
		try
			{
			this.ImportInDataPath = base.ContextCBIRepository.GetImportLogDomainObjectFolderPath(base.CurrentInventor, @"\InData");
			}
				catch (Exception ex)
				{
					base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error FillInDataPath  ", ex.Message));
				}
		}

																 // IImportModuleInfo importModuleInfo
		protected void InitCatalogFromConfig(ImportCommandInfo info, CBIState state) //?? не вызывается сверхую Вроде и не должна
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
						//  <FROMPATH domainobject="inventor" adapteruse="import" howuse="relative" from="indata" value="" isdefault="1" />
						//<FROMPATH domainobject="inventor" adapteruse="import" howuse="asis" from="absolute" value="C:\ErpPath" isdefault="1" />
						//string fixedImportPath = XDocumentConfigRepository.GetFixedImportPath(configXDoc);
						//this.ImportFixedPath = fixedImportPath;
						//this._openImportFixedPathCommand.RaiseCanExecuteChanged();
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

		//ExportErpCommandInfo info
		protected void InitExportErpFromConfig(ImportCommandInfo info, CBIState state) //?? не вызывается сверхую Вроде и не должна
		{
			if (state == null) return;
			if (info == null) return;
			base.State = state;

			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
						//  <FROMPATH domainobject="inventor" adapteruse="import" howuse="relative" from="indata" value="" isdefault="1" />
						//<FROMPATH domainobject="inventor" adapteruse="import" howuse="asis" from="absolute" value="C:\ErpPath" isdefault="1" />
						// NO FROM CONFIG - from Customer string fixedExportErp = XDocumentConfigRepository.GetFixedExportErpPath(this, configXDoc);
						// FROM CUSTOMER - после экспорта как южуал , потом копируется в папку из кастомера
						//string fixedExportErp = "";
						//if (string.IsNullOrWhiteSpace(base.State.CurrentCustomer.Address) == false)
						//{
						//	fixedExportErp = base.State.CurrentCustomer.Address;
						//}
						//this.ExportErpFixedPath = fixedExportErp;
						//this._openExportErpFixedPathCommand.RaiseCanExecuteChanged();
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

		protected void FillConfigPath()
		{
			string configPath = "";
			ImportModuleBaseViewModel importModuleBaseViewModel = null;
			importModuleBaseViewModel = base.ServiceLocator.GetInstance<ImportModuleBaseViewModel>(Common.Constants.ImportAdapterName.ImportIturDefaultAdapter);
			if (importModuleBaseViewModel != null)
			{
				configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
			}
			if (configPath == null) configPath = "";
			this.ImportConfigPath = configPath;
			this.ExportPdaConfigPath = configPath;
			this.ExportErpConfigPath = configPath;
			//this.ImportFixedPath = "";
			//..ициализируем из config файла катосмера
			//this.ImportFixedPath = this.GetImpotFixedPath(configPath, this.CurrentAdapterName);
		}

		protected void FillAdaptersList()
		{
			string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
			string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
			string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;


			try
			{
				this._itemsComplex = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ComplexOperation,
					currentCustomerCode, currentBranchCode, currentInventorCode));

				this._selectedComplex = this._itemsComplex.FirstOrDefault(r => r.Name == base.CurrentCustomer.ComplexAdapterCode);
				
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format("OnNavigatedTo : Error init Complex  ", ex.Message));
			}

			try
			{
				this._itemsCatalogs = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportCatalog,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format("OnNavigatedTo : Error init _itemsCatalogs  ", ex.Message));
			}

			try
			{
				this._itemsUpdateCatalog = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.UpdateCatalog,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(": Error init _itemsUpdateCatalog  ", ex.Message));
			}

			try
			{
				this._itemsIturs = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportItur,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsIturs  ", ex.Message));
			}

			try
			{
				this._itemsLocations = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportLocation,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsLocations  ", ex.Message));
			}

			try
			{
				this._itemsSections = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportSection,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsSections  ", ex.Message));
			}

			try
			{
				this._itemsFamilys = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportFamily,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsFamilys  ", ex.Message));
			}

			try
			{

				this._itemsSuppliers = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportSupplier,
					currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsSuppliers  ", ex.Message));
			}

			try
			{
				this._itemsPDA = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._unityContainer, this._importAdapterRepository, ImportDomainEnum.ImportInventProduct,
				  currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsPDA  ", ex.Message));
			}

			try
			{
				this._itemsExportPda =
				new ObservableCollection<IExportPdaModuleInfo>(Utils.GetExportPdaAdapters(this._unityContainer, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsExportPda  ", ex.Message));
			}

			try
			{
				this._itemsExportErp =
						new ObservableCollection<IExportErpModuleInfo>(Utils.GetExportErpAdapters(this._unityContainer, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode));
			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format(" : Error init _itemsExportErp  ", ex.Message));
			}
		}

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

		//public void Refresh()
		//{
		//	IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
		//	List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
		//}

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        protected override void ProcessImportInfo(ImportCommandInfo info)
        {
            ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
            if (pdaInfo != null)
            {
				//this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
                this._isAutoPrint = pdaInfo.IsAutoPrint;
                this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
            }
        }

        #region Implementation of IImportAdapter

		public List<string> NewDocumentCodeList
		{
			get
			{
				//ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				//List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);
				//return newDocumentCodeList;
				IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				System.Collections.Generic.List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
				return docCodes;
			}
		}

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
	         base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			//base.ProcessLisner = 0;

			//InitInventProductAdvancedField();

            base.IsInvertLetters = false;
            base.IsInvertWords = false;
			this.StepTotal = 1;
			this.Session = 0;
        }

		private void InitInventProductAdvancedField()
		{

		}

        public override void InitFromIni()
        {

        }


		public override void Import()
		{
		}

        public override void Clear()
        {
		
        }

        #endregion

        public void RunPrintReport(string documentCode)
        {
		
        }
		
		#region   baseComplexClass
		//..ициализируем из config файла катосмера
		//public string GetImpotFixedPath(string configPath, string adapterName)
		//{
		//	string defoultFixedPath =@"C:\ErpPath" ;
		//	return defoultFixedPath;
		//}

		public string GetImpotLogPath(object currentDomainObject, IImportModuleInfo importModuleInfo)
		{
			if (base.CurrentInventor == null) return String.Empty;
			string logPath = base.ContextCBIRepository.GetImportLogDomainObjectFolderPath(base.CurrentInventor, @"\Log");

			if (importModuleInfo == null) return logPath;

			string adapterName = importModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == false)
			{
				if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportInventProduct)
				{
					logPath = base.ContextCBIRepository.GetImportLogDomainObjectFolderPath(base.CurrentInventor, @"\inData\Log\" + adapterName);
				}
				else
				{
					logPath = base.ContextCBIRepository.GetImportLogDomainObjectFolderPath(base.CurrentInventor, @"\Log\" + adapterName);
				}
			}
			return logPath;
		}

		public string GetImpotLogFixedPath(string fixedPath, IImportModuleInfo importModuleInfo)
		{
			if (string.IsNullOrWhiteSpace(fixedPath) == true) return "" ;

			string logPath = fixedPath;
			string adapterName = importModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == false)
			{
				if (importModuleInfo.ImportDomainEnum == ImportDomainEnum.ImportInventProduct)
				{
					logPath = Path.Combine(fixedPath, @"inData\Log", adapterName);
				}
				else
				{
					logPath = Path.Combine(fixedPath, @"Log", adapterName);
				}
			}
			return logPath;
		}

		public string GetExportPdaLogPath(object currentDomainObject, IExportPdaModuleInfo exportPdaModuleInfo)
		{
			if (base.CurrentInventor == null) return String.Empty;
			string logPath = base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);

			if (exportPdaModuleInfo == null) return logPath.TrimEnd('\\') + @"\Log";

			string adapterName = exportPdaModuleInfo.Name;
			if (string.IsNullOrWhiteSpace(adapterName) == false)
			{
				logPath = logPath.TrimEnd('\\') + @"\Log\" + adapterName;
			}
			return logPath;
		}

		public string GetExportErpLogPath(string objectType, string objectCode, IExportErpModuleInfo exportErpModuleInfo)
		{
			if (base.CurrentInventor == null) return String.Empty;
			if (string.IsNullOrWhiteSpace(objectType) == true) return String.Empty;
			if (string.IsNullOrWhiteSpace(objectCode) == true) return String.Empty;
			string logPath = UtilsPath.ExportErpFolder(this._dbSettings, objectType, objectCode);

			//if (exportErpModuleInfo == null) return logPath.TrimEnd('\\') + @"\Log";

			//string adapterName = exportErpModuleInfo.Name;
			//if (string.IsNullOrWhiteSpace(adapterName) == false)
			//{
			//	logPath = logPath.TrimEnd('\\') + @"\Log\" + adapterName;
			//}
			return logPath;
		}

		public void RefreshLogText(/*string importFixedPath,*/string logFolderPath)
		{

			if (System.IO.Directory.Exists(logFolderPath) == true)
			{
				//this.LogText = System.IO.File.ReadAllText(this.LogPath);
				List<string> getFiles = Directory.GetFiles(logFolderPath).ToList();
				foreach (string filePath in getFiles)
				{
					FileInfo fi = new FileInfo(filePath);
					if (fi.Name.Contains(".full.log.txt") == true)
					{
						continue;
					}
					if (fi.Name.Contains(".log.txt") == true)
					{
						this.LogText = System.IO.File.ReadAllText(filePath);
						break;
					}
				}
			}

			//if (string.IsNullOrWhiteSpace(importFixedPath) == true) return;
			//if (System.IO.Directory.Exists(importFixedPath) == false) return;
			
		}

		

		public void ClearLogText()
		{
			LoadWaitCursor();
			Utils.RunOnUI(() =>
			{
				this.LogText = "import";
			});
			LoadDefaultCursor();
		}

		public void LoadWaitCursor()
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				Mouse.OverrideCursor = Cursors.Wait;
			}));
		}
		public void LoadDefaultCursor()
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				Mouse.OverrideCursor = null;
			}));
		}
	
		#endregion

		public override bool CanImport()
		{
			return true;
		}

		protected override void EncondingUpdated()
		{
			
		}
	}
}