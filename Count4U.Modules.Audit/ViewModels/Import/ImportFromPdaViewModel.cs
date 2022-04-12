using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Report.ViewModels.ExportPda;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.Model;
using NLog;
using Count4U.GenerationReport;
using Count4U.Common.Extensions;
using Count4U.Common.Constants;
using System.Xml.Linq;
using System.IO;
using Count4U.Modules.ContextCBI.Xml.Config;
using Count4U.Localization;
using Count4U.Model.Audit;

namespace Count4U.Modules.Audit.ViewModels.Import
{
    public class ImportFromPdaViewModel : ImportWithModulesBaseViewModel, IConfirmNavigationRequest
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        //private readonly DelegateCommand _navigateToGridCommand;

        private readonly IUserSettingsManager _userSettingsManager;

        private readonly IImportAdapterRepository _importAdapterRepository;
        private ExportPdaExtraSettingsViewModel _extraSettingsViewModel;
        private readonly List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;
        private readonly IIturRepository _iturRepository;
        private readonly DelegateCommand _setAsDefaultCommand;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;
        private readonly UICommandRepository _uiCommandRepository;
        private readonly IServiceLocator _serviceLocator;			
		private readonly ModalWindowLauncher _modalWindowLauncher;

        private bool _isNavigateBack;
//		private bool _isCopyFromSource;
        private bool _isContinueGrabFiles;
        private bool _isContinueGrabFilesEnabled;
		private bool _extraSettingsVisibility = true;
		private string _adapterName;
		private readonly DelegateCommand _getFromFtpCommand;

		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _saveConfigCommand;
		private readonly DelegateCommand _saveConfigToCustomerCommand;
		private readonly DelegateCommand _importByConfigCommand;

		bool _isFromGUIEnabled;
		bool _isFromConfigEnabled;
		bool _fromGUI;
		bool _fromConfig;

		private bool _fromAdapter;
		private bool _fromCustomer;
		private bool _fromBranch;
		private bool _fromInventor;

		private string _configXML;
		private XDocument _configXDocument;

		private string _dataInConfigPath = String.Empty;

	    public ImportFromPdaViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUnityContainer container,
            IImportAdapterRepository importAdapterRepository,
            INavigationRepository navigationRepository,
            UICommandRepository commandRepository,
            IIturRepository iturRepository,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
            UICommandRepository uiCommandRepository,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
			ModalWindowLauncher modalWindowLauncher	,
			DBSettings dbSettings
            )
			: base(contextCBIRepository, eventAggregator, regionManager, container, navigationRepository, commandRepository, dbSettings)
        {
			this._modalWindowLauncher = modalWindowLauncher;
            _userSettingsManager = userSettingsManager;
            _serviceLocator = serviceLocator;
            _uiCommandRepository = uiCommandRepository;
            _inventorRepository = inventorRepository;
            _branchRepository = branchRepository;
            _customerRepository = customerRepository;
            _iturRepository = iturRepository;
            this._importAdapterRepository = importAdapterRepository;
            if (contextCBIRepository == null) throw new ArgumentNullException("contextCBIRepository");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            if (regionManager == null) throw new ArgumentNullException("regionManager");
            if (importAdapterRepository == null) throw new ArgumentNullException("importAdapterRepository");

            base._progressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;

            base.ImportCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted, ImportCommandCanExecute);
			this._getFromFtpCommand = _commandRepository.Build(enUICommand.GetFromFtp, GetFromFtpCommandExecuted, GetFromFtpCommandCanExecute);

            this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();

            this._setAsDefaultCommand = new DelegateCommand(SetAsDefaultCommandExecuted, SetAsDefaultCommandCanExecute);

			base.NavigateToGridCommand = _uiCommandRepository.Build(enUICommand.FromImportToGrid, this.NavigateToGridCommandExecuted, base.NavigateToGridCommandCanExecute);
			//base.ImportCommand.RaiseCanExecuteChanged();

			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			this._saveConfigToCustomerCommand = new DelegateCommand(SaveConfigToCustomerCommandExecuted, SaveConfigToCustomerCommandCanExecute);
			this._importByConfigCommand = new DelegateCommand(ImportByConfigCommandExecuted, ImportByConfigCommandCanExecute);

			this._isFromGUIEnabled = true;
			this._isFromConfigEnabled = false;

			this._fromGUI = true;
			this._fromConfig = false;

			this._fromAdapter = false;
			this._fromCustomer = true;
			this._fromBranch = false;
			this._fromInventor = false;
        }

		public DelegateCommand SetAsDefaultCommand
		{
			get { return _setAsDefaultCommand; }
		}


		public bool IsNavigateBack
		{
			get { return _isNavigateBack; }
			set
			{
				_isNavigateBack = value;
				RaisePropertyChanged(() => IsNavigateBack);

				_userSettingsManager.NavigateBackImportPdaFormSet(_isNavigateBack);
			}
		}

		public DelegateCommand GetFromFtpCommand
		{
			get { return this._getFromFtpCommand; }
		}

		private bool GetFromFtpCommandCanExecute()
		{
			if (this._adapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
				//|| this._adapterName == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaYesXlsxAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		protected void GetFromFtpCommandExecuted()
		{
			var settings = new Dictionary<string, string>();
			Utils.AddContextToDictionary(settings, base.Context);
			Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
			settings.Add(NavigationSettings.AdapterName, this._adapterName);

			//settings.Add(NavigationSettings.CheckBoundrate, IsCheckBaudratePDA.ToString());

			//using (new CursorWait())
			//{
			object result = _modalWindowLauncher.StartModalWindow(
					   Common.ViewNames.FromFtpView,
					   WindowTitles.FromFtp,
					   800, 600,
					   ResizeMode.CanResize, settings,
					   null,
					   650, 450);
			//}
		}

		//public bool IsCopyFromSource
		//{
		//	get { return _isCopyFromSource; }
		//	set
		//	{
		//		_isCopyFromSource = value;
		//		RaisePropertyChanged(() => IsCopyFromSource);

		//		_userSettingsManager.CopyFromSourceSet(_isCopyFromSource);
		//	}
		//}

		public string AdapterName
		{
			get { return _adapterName; }
		}

		public bool IsExtraSettingsVisibility
		{
			get { return _extraSettingsVisibility; }
			set
			{
				_extraSettingsVisibility = value;

				RaisePropertyChanged(() => IsExtraSettingsVisibility);
				if (_extraSettingsVisibility == true)
				{

				}
			}
		}


		public bool IsContinueGrabFiles
		{
			get { return _isContinueGrabFiles; }
			set
			{
				_isContinueGrabFiles = value;
				RaisePropertyChanged(() => IsContinueGrabFiles);
			}
		}

		public bool IsContinueGrabFilesEnabled
		{
			get { return _isContinueGrabFilesEnabled; }
			set
			{
				_isContinueGrabFilesEnabled = value;
				RaisePropertyChanged(() => IsContinueGrabFilesEnabled);
			}
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
			string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
			string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

			base.Adapters = new ObservableCollection<IImportModuleInfo>(
				Utils.GetImportAdapters(base.Container, this._importAdapterRepository, ImportDomainEnum.ImportInventProduct,
										currentCustomerCode, currentBranchCode, currentInventorCode));


			string currentAdapter = GetSelectedAdapterNameByCBI();
			if (String.IsNullOrWhiteSpace(currentAdapter) == true)
			{
				this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.IsDefault);
			}
			else
			{
				this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.Name == currentAdapter);
			}

			this._extraSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaExtraSettingsViewModel>(Common.RegionNames.ExportPdaExtraSettings, this._regionManager);
			if (this._extraSettingsViewModel != null)
			{
				switch (base.CBIDbContext)
				{
					case Common.NavigationSettings.CBIDbContextCustomer:
						this._extraSettingsViewModel.SetCustomer(base.CurrentCustomer);
						break;
					case Common.NavigationSettings.CBIDbContextBranch:
						this._extraSettingsViewModel.SetBranch(base.CurrentBranch, enBranchAdapterInherit.InheritNothing);
						break;
					case Common.NavigationSettings.CBIDbContextInventor:
						this._extraSettingsViewModel.SetInventor(base.CurrentInventor, enInventorAdapterInherit.InheritNothing);
						break;
				}
			}


			this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName(base.State);
			RaisePropertyChanged(() => this.ContextCustomerAdapterName);

			_isFromConfigEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;

			//_isNavigateBack = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.NavigateBackCheckboxIsChecked);
			_isNavigateBack = _userSettingsManager.NavigateBackImportPdaFormGet();
			//			_isCopyFromSource = _userSettingsManager.CopyFromSourceGet();

			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoStartImportPda))
			{
				if (base.ImportCommand.CanExecute())
				{
					base.ImportCommand.Execute();
				}
			}

			//CompareSelectedAdapterInventorAndCustomer();
			
		}

		public override void CompareSelectedAdapterInventorAndCustomer()
		{
			if (this.SelectedAdapter != null)
			{
				string selectedAdapterName = this.SelectedAdapter.Name;
				_errorAdapterName = "";

				string adapterName = this.GetContextCustomerAdaperName(base.State);
				if (selectedAdapterName.ToLower() != adapterName.ToLower())
				{
					_errorAdapterName = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; //"Selected Adapter not equals Adapter for Customer";
				}
			}
			this.RaisePropertyChanged(() => this.ErrorAdapterName);
		}

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);


		}


	


		private bool ImportCommandCanExecute()
		{
			if (SelectedAdapter == null) return false;
			if (this._fromConfig == true)
			{
				//string selectedAdapterName = SelectedAdapter.Name;
				//string contextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//if (selectedAdapterName.ToLower() != contextCustomerAdapterName.ToLower()) return false;
				return false;
			}

			bool canImportByDynViewModel = base.DynViewModel == null ? true : base.DynViewModel.CanImport();
			return !base.IsBusy && canImportByDynViewModel;
		}

		private bool NavigateToGridCommandCanExecute()
		{
			bool canImportByDynViewModel = base.DynViewModel == null ? true : base.DynViewModel.CanImport();
			return !base.IsBusy && canImportByDynViewModel;
		}



		#region  fromConfig
		// ============= Config	================

		public bool IsFromGUIEnabled
		{
			get { return this._isFromGUIEnabled; }
			set
			{
				this.IsFromGUIEnabled = value;
				RaisePropertyChanged(() => IsFromGUIEnabled);
			}
		}

		public bool IsFromConfigEnabled
		{
			get { return this._isFromConfigEnabled; }
			set
			{
				this._isFromConfigEnabled = value;
				RaisePropertyChanged(() => IsFromConfigEnabled);
			}
		}

		public bool FromGUI
		{
			get { return this._fromGUI; }
			set
			{
				this._fromGUI = value;
				this.RaisePropertyChanged(() => this.FromGUI);

				if (value == true)
				{
					this._fromConfig = false;
				}


				this._setAsDefaultCommand.RaiseCanExecuteChanged();

				base.ImportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
			}
		}



		public bool FromConfig
		{
			get { return this._fromConfig; }
			set
			{
				this._fromConfig = value;

				this.RaisePropertyChanged(() => this.FromConfig);

				if (this._fromConfig == true)
				{
					this._fromGUI = false;
					this.LoadXMLConfig();
				}

				base.ImportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);

			}
		}


		public string ConfigXML
		{
			get { return _configXML; }
			set
			{
				_configXML = value;
				RaisePropertyChanged(() => ConfigXML);
			}
		}

		public XDocument ConfigXDocument
		{
			get { return _configXDocument; }
			set
			{
				_configXDocument = value;
				if (value != null)
				{
				_configXML = _configXDocument.ToString();
				}
				else
				{
					_configXML = String.Empty;
				}
				RaisePropertyChanged(() => ConfigXDocument);
				RaisePropertyChanged(() => ConfigXML);
			}
		}

		public string DataInConfigPath
		{
			get { return this._dataInConfigPath; }
			set
			{
				this._dataInConfigPath = value;
				this.RaisePropertyChanged(() => this.DataInConfigPath);
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
			}
		}

		public DelegateCommand ImportByConfigCommand
		{
			get { return _importByConfigCommand; }
		}

		string _contextCustomerAdapterName;
		public string ContextCustomerAdapterName
		{
			get { return this._contextCustomerAdapterName; }
			set
			{
				this._contextCustomerAdapterName = value;
				this.RaisePropertyChanged(() => this.ContextCustomerAdapterName);
			}
		}


		string _errorAdapterName;
		public string ErrorAdapterName
		{
			get { return this._errorAdapterName; }
			set
			{
				this._errorAdapterName = value;
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
			}
		}

		private void ImportByConfigCommandExecuted()
		{
			// this.DataInConfigPath похоже что не используется, НО строится 
			// и используется у каждого адаптера путь по умолчанию 
			// если путь меняется, то надо все перетестировать
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			using (new CursorWait())
			{
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				//this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
				ImportDomainEnum mode = base.Mode;
				ImportModuleBaseViewModel viewModel = base.DynViewModel;
				//ContextAdapterName  из customer 
				ImportFromConfigMode(viewModel, ContextCustomerAdapterName, mode, fromConfigXDoc, base.State, null);
			}
		}

		private bool ImportByConfigCommandCanExecute()
		{
			if (IsBusy == true) return false;
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;

			if (this.SelectedAdapter == null) return false;
			string selectedAdapterName = this.SelectedAdapter.Name;
			_errorAdapterName = "";

			string adapterName = this.GetContextCustomerAdaperName(base.State);
			if (selectedAdapterName.ToLower() != adapterName.ToLower())
			{
				_errorAdapterName = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; //"Selected Adapter not equals Adapter for Customer";
				return false;
			}

			if (adapterName == WarningAdapter.AdapterInCustomerNotSet) return false;
			string adapterConfigFileName = @"\" + adapterName + ".config";

			if (File.Exists(this.DataInConfigPath + adapterConfigFileName) == true) return true;

			return false;
		}

	
		public void ImportByConfig(ImportModuleBaseViewModel viewModel, 
			CBIState state,
			string adapterName, 
			ImportDomainEnum mode, 
			ConfigXDocFromEnum fromConfigXDoc,
			string comment,
			ImportModuleBaseViewModel complexViewModel) //, string dataInConfigPath)
		{
			//base.SetSelectedAdapterWithoutGUI(comment/*complexAdapter, complexViewModel*/);
			ImportFromConfigMode(viewModel, adapterName, mode, fromConfigXDoc, state, null);
		}

		public void ClearByConfig(ImportModuleBaseViewModel viewModel,
			CBIState state,
			string adapterName,
			ImportDomainEnum mode,
			ConfigXDocFromEnum fromConfigXDoc) //, string dataInConfigPath)
		{
			ClearFromConfigMode(viewModel, adapterName, mode, fromConfigXDoc, state, true);
		}


	
		
		private ConfigXDocFromEnum GetFromConfigXDoc()
		{
			ConfigXDocFromEnum fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
			if (this.FromCustomer == true)
			{
				fromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
			}
			else if (this.FromBranch == true)
			{
				fromConfigXDoc = ConfigXDocFromEnum.FromBranchInData;
			}
			else if (this.FromInventor == true)
			{
				fromConfigXDoc = ConfigXDocFromEnum.FromInventorInData;
			}
			return fromConfigXDoc;
		}

		public DelegateCommand OpenConfigCommand
		{
			get { return _openConfigCommand; }
		}

		/// <summary>
		/// OpenConfigCommand
		/// </summary>
		private void OpenConfigCommandExecuted()
		{
			if (!Directory.Exists(_dataInConfigPath)) return;

			Utils.OpenFolderInExplorer(_dataInConfigPath);
		}

		private bool OpenConfigCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;

			return Directory.Exists(this.DataInConfigPath);
		}


		public DelegateCommand SaveConfigCommand
		{
			get { return _saveConfigCommand; }
		}

		public DelegateCommand SaveConfigToCustomerCommand
		{
			get { return _saveConfigToCustomerCommand; }
		}

		public string SaveConfigContent
		{
			get {
				if (this.SelectedAdapter == null) return String.Empty;
				return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for " + this.SelectedAdapter.Title;
			}
		}

		public string SaveConfigForCustomer
		{
			get { return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for Customer"; }
		}

		//not use now
		private void SaveConfigCommandExecuted()
		{
			//if (base.SelectedAdapter == null) return;
			//string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
			//string adapterName = base.SelectedAdapter.Name;
			//string adapterConfigFileName = @"\" + adapterName + ".config";
			//string fileConfig = adapterDefaultParamFolderPath + adapterConfigFileName;

			//if (Directory.Exists(adapterDefaultParamFolderPath) == false)
			//{
			//	Directory.CreateDirectory(adapterDefaultParamFolderPath);
			//}

			//if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
			//{
			//	try
			//	{
			//		File.Delete(fileConfig);
			//	}
			//	catch { }
			//}

			//this.ConfigXDocument = XDocument.Parse(_configXML);
			//this.ConfigXDocument = ViewModelConfigRepository.VerifyToAdapter(this.ConfigXDocument, this.SelectedAdapter);

			//this.ConfigXDocument.Save(fileConfig);

		}

		private bool SaveConfigCommandCanExecute()
		{
			if (this.SelectedAdapter == null) return false;
			return true;
		}

	   //not use now?
		private void SaveConfigToCustomerCommandExecuted()
		{
			if (this.CurrentCustomer == null) return;
			if (base.SelectedAdapter == null) return;
			string adapterName = base.SelectedAdapter.Name;

			Customer currentDomainObject = this.CurrentCustomer;
			string customerConfigPath = "";
			if (base.DynViewModel != null)
			{
				customerConfigPath = this._contextCBIRepository.GetConfigFolderPath(currentDomainObject);
			}
			else
			{
				return;
			}

			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfig = customerConfigPath + adapterConfigFileName;

			if (Directory.Exists(customerConfigPath) == false)
			{
				Directory.CreateDirectory(customerConfigPath);
			}

			if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
			{
				try
				{
					File.Delete(fileConfig);
				}
				catch { }
			}

			this.ConfigXDocument = XDocument.Parse(_configXML);
			this.ConfigXDocument = ViewModelConfigRepository.VerifyToAdapter(this.ConfigXDocument, this.SelectedAdapter);

			this.ConfigXDocument.Save(fileConfig);

		}

		private bool SaveConfigToCustomerCommandCanExecute()
		{
			//if (this.SelectedAdapter == null) return false;
			//return true;
			return false;
		}

		//not use now?
		private void ReloadConfigCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			if (Directory.Exists(this.DataInConfigPath) == false) return;

			if (base.SelectedAdapter == null) return;
			string adapterName = base.SelectedAdapter.Name;
			string adapterConfigFileName = @"\" + adapterName + ".config";

			string fileConfig = this.DataInConfigPath + adapterConfigFileName;
			if (File.Exists(fileConfig) == false) return;


			this.ConfigXDocument = XDocument.Load(fileConfig); //, LoadOptions.PreserveWhitespace

		}

		//not use now?
		private bool ReloadConfigCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;
			if (Directory.Exists(this.DataInConfigPath) == false) return false;

			if (base.SelectedAdapter == null) return false;
			string adapterName = base.SelectedAdapter.Name;
			string adapterConfigFileName = @"\" + adapterName + ".config";

			string fileConfig = this.DataInConfigPath + adapterConfigFileName;
			if (File.Exists(fileConfig) == false) return false;

			return true;

		}

		public bool FromCustomer
		{
			get { return this._fromCustomer; }
			set
			{
				this._fromCustomer = value;

				this.RaisePropertyChanged(() => this.FromCustomer);

				if (value == true)
				{
					this.FromBranch = false;
					this.FromInventor = false;
					this.FromAdapter = false;
					this.LoadXMLConfig();

				}
			}
		}


		// Not use now
		public bool FromAdapter
		{
			get { return this._fromAdapter; }
			set
			{
				this._fromAdapter = value;

				this.RaisePropertyChanged(() => this.FromAdapter);

				if (value == true)
				{
					this.FromBranch = false;
					this.FromInventor = false;
					this.FromCustomer = false;
					this.LoadXMLConfig();
				}
			}
		}

		public bool FromBranch
		{
			get { return this._fromBranch; }
			set
			{
				this._fromBranch = value;

				this.RaisePropertyChanged(() => this.FromBranch);

				if (value == true)
				{
					this.FromCustomer = false;
					this.FromInventor = false;
					this.FromAdapter = false;
					this.LoadXMLConfig();
				}
			}
		}

		public bool FromInventor
		{
			get { return this._fromInventor; }
			set
			{
				this._fromInventor = value;

				this.RaisePropertyChanged(() => this.FromInventor);

				if (value == true)
				{
					this.FromCustomer = false;
					this.FromBranch = false;
					this.FromAdapter = false;
					this.LoadXMLConfig();
				}
			}
		}


		public void LoadXMLConfig()
		{
			if (this._fromConfig == false) return;
			string fileConfig = "";
			this.DataInConfigPath = "";
			// Not use now
			//if (this.FromAdapter == true)
			//{
			//	string originalAdapterDefaultParamFolderPath = _dbSettings.GetOriginalAdapterDefaultParamFolderPath().TrimEnd(@"\".ToCharArray());
	
			//	string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());

			//	if (base.SelectedAdapter != null)
			//	{
			//		string adapterName = base.SelectedAdapter.Name;
			//		string adapterConfigFileName = @"\" + adapterName + ".config";
			//		this.DataInConfigPath = adapterDefaultParamFolderPath;
			//		fileConfig = this.DataInConfigPath + adapterConfigFileName;
			//	}
			//}
			//else				 //Object InData
			//{
				string adapterName = "";
				object currentDomainObject = null;
				if (this.FromCustomer == true)
				{
					currentDomainObject = base.CurrentCustomer;
					adapterName = base.CurrentCustomer.ImportPDAProviderCode;
				}
				else if (this.FromBranch == true)
				{
					currentDomainObject = base.CurrentBranch;
					adapterName = base.CurrentBranch.ImportPDAProviderCode;
				}
				else if (this.FromInventor == true)
				{
					currentDomainObject = base.CurrentInventor;
					adapterName = base.CurrentInventor.ImportPDAProviderCode;
				}
				if (base.DynViewModel != null)
				{
					this.DataInConfigPath = this._contextCBIRepository.GetConfigFolderPath(currentDomainObject);
				}

				string adapterConfigFileName = @"\" + adapterName + ".config";
				fileConfig = this.DataInConfigPath + adapterConfigFileName;
			//}

			this.ConfigXDocument = new XDocument();
			if (File.Exists(fileConfig) == false)	   //если нет сохраненного файла config.xml
			{
				//this.ConfigXDocument = XDocument.Parse(data.ConfigXML);
				//this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
				//this.ConfigXDocument.Save(fileConfig);
				this.ConfigXDocument = null;
			}
			else
			{
				try
				{
					this.ConfigXDocument = XDocument.Load(fileConfig);
				}
				catch (Exception exp)
				{
					this.ConfigXML = "Error Load Xml form file : " + fileConfig + " :  " + exp.Message;
				}
			}

			OpenConfigCommand.RaiseCanExecuteChanged();
		}



		//public string GetConfigFolderPath(object currentDomainObject)
		//{
		//		if (currentDomainObject != null)
		//	{

		//		string dataInPath = base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

		//		string dataInConfigPath = dataInPath + @"\Config";

		//		if (string.IsNullOrWhiteSpace(dataInPath) == false)
		//		{
		//			if (Directory.Exists(dataInConfigPath) == false)
		//			{
		//				Directory.CreateDirectory(dataInConfigPath);
		//			}
		//		}


		//		return dataInConfigPath;
		//	}
		//	return String.Empty;
		//	}


		private void ImportCommandExecuted()
		{
			ImportWithInitMode(this._fromGUI, this._fromConfig);
		}

		private void ImportWithInitMode(
			bool iniFromGUI = true,  
			bool orInitFromConfig = false)
		{
			if (iniFromGUI == true)
			{
				if (base.DynViewModel != null)
				{
					if (base.DynViewModel.CanImport())
					{
						ImportFromPdaCommandInfo info = new ImportFromPdaCommandInfo();
						info.IsWriteLogToFile = base.IsWriteLogToFile;
						info.Callback = () => Utils.RunOnUI(() =>
							{
								base.Progress = String.Empty;
								base.ProgressStep = String.Empty;
								this.CollectNewImportDocumentList();

								if (_isNavigateBack)
								{
									if (UtilsNavigate.CanGoBack(base._regionManager))
										UtilsNavigate.GoBack(base._regionManager);
								}
								else
								{
									ShowLog();
								}
							});
						base.Cts = new CancellationTokenSource();
						info.CancellationToken = base.Cts.Token;
						info.IsContinueGrabFiles = _isContinueGrabFiles;

						if (this._extraSettingsViewModel != null)
						{
							GenerationReport.Report report = this._extraSettingsViewModel.SelectedReport;

							info.IsAutoPrint = this._extraSettingsViewModel.IsAutoPrint;
							info.Report = report;
						}

						info.AdapterName = "";
						info.ConfigXDocument = null;
						info.ConfigXDocumentPath = "";
						info.FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;

						base.DynViewModel.RunImportBase(info);
					}
				}
			}
			else if (orInitFromConfig == true)
			{
				ImportDomainEnum mode = base.Mode;
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				ContextCustomerAdapterName = this.GetContextCustomerAdaperName(base.State);
				ImportFromConfigMode(base.DynViewModel, ContextCustomerAdapterName, mode, fromConfigXDoc, base.State, null);
			}
		}

		public void ImportFromConfigMode(
			ImportModuleBaseViewModel viewModel, 
			string adapterName,
			ImportDomainEnum mode, 
			ConfigXDocFromEnum fromConfigXDoc, 
			CBIState state,
			ImportFromPdaCommandInfo infoParam,
			bool isWriteLogToFile = true)
		{
			if (viewModel != null)
			{
			ImportFromPdaCommandInfo info = new ImportFromPdaCommandInfo();
				info.IsWriteLogToFile = base.IsWriteLogToFile;
				info.Callback = () => Utils.RunOnUI(() =>
				{
					base.Progress = String.Empty;
					base.ProgressStep = String.Empty;
					this.CollectNewImportDocumentList();
					ShowLog();
				});
				base.Cts = new CancellationTokenSource();
				info.CancellationToken = base.Cts.Token;
				info.IsContinueGrabFiles = _isContinueGrabFiles;
				info.IsWriteLogToFile = isWriteLogToFile;
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Mode = mode;
				if (infoParam != null)
				{
					//f. e. info.AdapterName = infoParam.AdapterName;
				}

				viewModel.RunImportWithoutGUIBase(info, state);
			}
		}


		public void ClearFromConfigMode(
			ImportModuleBaseViewModel viewModel, 
			string adapterName,
			ImportDomainEnum mode, 
			ConfigXDocFromEnum fromConfigXDoc, 
			CBIState state, 
			bool isWriteLogToFile = true)
		{
			if (viewModel != null)
			{
				ImportFromPdaCommandInfo info = new ImportFromPdaCommandInfo();
				info.IsWriteLogToFile = base.IsWriteLogToFile;
				info.Callback = () => Utils.RunOnUI(() =>
				{
					base.Progress = String.Empty;
					base.ProgressStep = String.Empty;
					this.CollectNewImportDocumentList();
					ShowLog();
				});
				base.Cts = new CancellationTokenSource();
				info.CancellationToken = base.Cts.Token;
				info.IsContinueGrabFiles = _isContinueGrabFiles;
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Mode = mode;
				info.IsWriteLogToFile = isWriteLogToFile;

				viewModel.RunImportClearWithoutGUIBase(info, state);
			}
		}

		#endregion  fromConfig
		// ============= Config
		
        protected override void OnSelectedAdapterChanged()
        {
            base.OnSelectedAdapterChanged();

            this._setAsDefaultCommand.RaiseCanExecuteChanged();

            if (base.DynViewModel != null)
            {
                base.DynViewModel.UpdateBusyText = UpdateBusyText;
                base.DynViewModel.SetIsCancelOk = SetIsCancelOk;
                base.DynViewModel.InputFileFolderChanged = isDirectory =>
                    {                     
                        IsContinueGrabFilesEnabled = isDirectory;
                        if (!isDirectory)
                        {
                            IsContinueGrabFiles = false;
                        }
                    };

                TemplateAdapterFileFolderViewModel fileFolder = base.DynViewModel as TemplateAdapterFileFolderViewModel;
                if (fileFolder != null)
                {
                    if (fileFolder.IsDirectory == false)
                    {
                        IsContinueGrabFiles = false;    
                    }                    
                    IsContinueGrabFilesEnabled = fileFolder.IsDirectory;
                }

                TemplateAdapterOneFileViewModel oneFile = base.DynViewModel as TemplateAdapterOneFileViewModel;
                if (oneFile != null)
                {
                    IsContinueGrabFiles = false;
                    IsContinueGrabFilesEnabled = false;
                }

                TemplateAdapterTwoFilesViewModel twoFiles = base.DynViewModel as TemplateAdapterTwoFilesViewModel;
                if (twoFiles != null)
                {
                    IsContinueGrabFiles = false;
                    IsContinueGrabFilesEnabled = false;
                }

				_adapterName = base.DynViewModel.AdapterName;

				if (
					base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaDB3Adapter
					//|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMISAdapter
					//|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMISAndDefaultAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaYesXlsxAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaAddCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMergeCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaCompareCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaContinueAfterCompareCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaAddSumCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaCloneCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMinusByMakatCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMerkavaUpdateDbAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaUpdateBarcodeDbAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMultiCsvAdapter
					|| base.DynViewModel.AdapterName == ImportAdapterName.ImportPdaMISSqliteAdapter
					)
				{
					IsExtraSettingsVisibility = false;
				}
				else
				{
					IsExtraSettingsVisibility = true;
				}
            }
			this._getFromFtpCommand.RaiseCanExecuteChanged();
        }

        private void UpdateBusyText(string text)
        {
            Utils.RunOnUI(() =>
                {
                    if (String.IsNullOrWhiteSpace(text))
                    {
                        ProgressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;
                    }
                    else
                    {
                        ProgressText = text;
                    }
                });
        }

        private void SetIsCancelOk(bool ok)
        {
            Utils.RunOnUI(() =>
                {
                    base._isCancelOk = ok;
                    BusyCancelCommand.RaiseCanExecuteChanged();
                });
        }

        private void CollectNewImportDocumentList()
        {
            if (base.DynViewModel != null)
            {
                IImportPdaAdapter importPdaAdapter = base.DynViewModel as IImportPdaAdapter;
                if (importPdaAdapter != null)
                {
                    foreach (string code in importPdaAdapter.NewDocumentCodeList)
                    {
                        if (this._newDocumentCodeList.Contains(code) == false)
                        {
                            this._newDocumentCodeList.Add(code);
                        }
                    }
					
					foreach (string code in importPdaAdapter.NewSessionCodeList)
                    {
						if (this._newSessionCodeList.Contains(code) == false)
                        {
							this._newSessionCodeList.Add(code);
                        }
                    }
					
                }

				
            }
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
			base._isBusy = false;                                    // rem !!YES
			base.IsBusy = true;                                       // rem !!YES
			this.UpdateBusyText(Localization.Resources.View_IturListDetails_busyContent);     // rem !!YES
			this.SetIsCancelOk(false);                                                                                        // rem !!YES
			_logger.Info("Task RefillApproveStatusBit ::  start");
			Task taskA = Task.Factory.StartNew(RefillApproveStatusBit, continuationCallback);     // rem !!YES
																								  //Task taskA = Task.Factory.StartNew(RefillApproveStatusBit);	 //!!YES
			taskA.ContinueWith(t => { _logger.Info("Task RefillApproveStatusBit ::  come to end -> OnlyOnRanToCompletion"); }, TaskContinuationOptions.OnlyOnRanToCompletion);
			taskA.ContinueWith(t => { _logger.Info("Task RefillApproveStatusBit ::  Canceled"); }, TaskContinuationOptions.OnlyOnCanceled);
			taskA.LogTaskFactoryExceptions("ConfirmNavigationRequest");

			//if (continuationCallback != null)		  //!!YES
			//	continuationCallback(true);

		}

		private void RefillApproveStatusBit(object state)   // rem object state !!YES
		{
            try
            {
				
				//this._iturRepository.RefillApproveStatusBit(this._newDocumentCodeList, this._newSessionCodeList,  base.GetDbPath);		было
				this._iturRepository.RefillApproveStatusBitByStep(this._newDocumentCodeList, this._newSessionCodeList, base.GetDbPath);
				//this._iturRepository.RefillApproveStatusBit(base.GetDbPath);


				ImportPdaPrintQueue queue = _serviceLocator.GetInstance<ImportPdaPrintQueue>();
                if (queue.IsPrinting)
                {
                    Utils.RunOnUI(() =>
                        {
                            UpdateBusyText(Localization.Resources.ViewModel_ImportFromPda_Printing);
                        });

                    while (queue.IsPrinting)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("RefillApproveStatusBit", exc);
            }
            finally
            {
                Utils.RunOnUI(() =>
                    {
                        this.UpdateBusyText(String.Empty);
                        base.IsBusy = false;
                        this.SetIsCancelOk(true);

						Action<bool> continuationCallback = state as Action<bool>;         // rem !!YES
						if (continuationCallback != null)                                                  // rem !!YES
							continuationCallback(true);                                                   // rem !!YES
					});
            }
        }

        private bool SetAsDefaultCommandCanExecute()
        {
			if (base.Adapters == null) return false;
			return base.Adapters.Any() && base.SelectedAdapter != null
                   && base.SelectedAdapter.Name != this.GetSelectedAdapterNameByCBI();
        }

        private void SetAsDefaultCommandExecuted()
        {
            using (new CursorWait())
            {
                try
                {
					string adapterName = "";
					if (base.SelectedAdapter != null) adapterName = base.SelectedAdapter.Name;
                    switch (base.CBIDbContext)
                    {
                        case NavigationSettings.CBIDbContextCustomer:
							base.CurrentCustomer.ImportPDAProviderCode = adapterName;
                            this._customerRepository.Update(base.CurrentCustomer);
                            break;
                        case NavigationSettings.CBIDbContextBranch:
							base.CurrentBranch.ImportPDAProviderCode = adapterName;
                            this._branchRepository.Update(base.CurrentBranch);
                            break;
                        case NavigationSettings.CBIDbContextInventor:
							base.CurrentInventor.ImportPDAProviderCode = adapterName;
                            this._inventorRepository.Update(base.CurrentInventor);
                            break;

                    }

                    this._setAsDefaultCommand.RaiseCanExecuteChanged();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("SetAsDefaultCommandExecuted", exc);
                }
            }
        }

        private string GetSelectedAdapterNameByCBI()
        {
            switch (base.CBIDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportPDAProviderCode;
                case Common.NavigationSettings.CBIDbContextBranch:
                    return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportPDAProviderCode;
                case Common.NavigationSettings.CBIDbContextInventor:
                    return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportPDAProviderCode;
            }
            return String.Empty;
        }

		public string GetContextCustomerAdaperName(CBIState state)
		{
			string contextAdapterName = "";
			//if (this.SelectedAdapter != null)
			//{
			//	contextAdapterName = SelectedAdapter.Name;
			//}
			//if (state == null) return contextAdapterName;
			//if (this.FromCustomer == true)
			//{
				contextAdapterName = state.CurrentCustomer == null ? String.Empty : state.CurrentCustomer.ImportPDAProviderCode;
			//}
			//else if (this.FromBranch == true)
			//{
			//	contextAdapterName = state.CurrentBranch == null ? String.Empty : state.CurrentBranch.ImportPDAProviderCode;
			//}
			//else if (this.FromInventor == true)
			//{
			//	contextAdapterName = state.CurrentInventor == null ? String.Empty : state.CurrentInventor.ImportPDAProviderCode;
			//}
				if (string.IsNullOrWhiteSpace(contextAdapterName) == true)
					contextAdapterName = WarningAdapter.AdapterInCustomerNotSet; 
			return contextAdapterName;
		}

		private void NavigateToGridCommandExecuted()
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());


			UtilsNavigate.InventProductListOpen(this._regionManager, query);
		}
    }
}