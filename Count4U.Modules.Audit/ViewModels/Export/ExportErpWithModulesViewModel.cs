using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Windows.Controls;
using NLog;
using System.Xml.Linq;
using Count4U.Modules.ContextCBI.Xml.Config;
using Count4U.Localization;
using Count4U.Model.Audit;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel;
using Count4U.Common.Enums;

namespace Count4U.Modules.Audit.ViewModels.Export
{
    public class ExportErpWithModulesViewModel : ExportWithModulesBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();	

        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IUnityContainer _container;
        private readonly IDBSettings _dbSettings;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly LocationListViewModelBuilder _locationListViewModelBuilder;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;
        private readonly UICommandRepository _uiCommandRepository;
        private readonly IEventAggregator _eventAggregator;
		private readonly ICustomerConfigRepository _customerConfigRepository;
		

        private readonly DelegateCommand _setAsDefaultCommand;
        private readonly DelegateCommand _navigateToGridCommand;
        

        private readonly ObservableCollection<IExportErpModuleInfo> _adapters;
        private IExportErpModuleInfo _selectedAdapter;

     //   private ExportErpModuleBaseViewModel _dynViewModel;
        private IExportErpModule _dynViewModel;
        private readonly IUserSettingsManager _userSettingsManager;

        private bool _isFull;		//Full
		 private bool _isExcludeNotExistingInCatalog; //Exclude Items with Makat not existing in Catalog
		
        private string _itursText;
        private readonly ObservableCollection<LocationItemViewModel> _locationItems;
        private bool _isFilterByItur;
        private bool _isFilterByLocation;

        private string _itursPrefix;
        private bool _autoClose;
		private bool _isCheckedLocations;

		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _saveConfigCommand;
		private readonly DelegateCommand _saveConfigToCustomerCommand;
		private readonly DelegateCommand _exportByConfigCommand;

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

        public ExportErpWithModulesViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            IUnityContainer unityContainer,
            IDBSettings dbSettings,
            IImportAdapterRepository importAdapterRepository,
            IUserSettingsManager userSettingsManager,
            UICommandRepository commandRepository,
            LocationListViewModelBuilder locationListViewModelBuilder,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
            UICommandRepository uiCommandRepository	,
			ICustomerConfigRepository customerConfigRepository
)
            : base(contextCbiRepository, eventAggregator, commandRepository)
        {
            this._eventAggregator = eventAggregator;
            this._uiCommandRepository = uiCommandRepository;
            this._inventorRepository = inventorRepository;
            this._branchRepository = branchRepository;
            this._customerRepository = customerRepository;
            this._locationListViewModelBuilder = locationListViewModelBuilder;
            this._userSettingsManager = userSettingsManager;
            this._importAdapterRepository = importAdapterRepository;
            this._dbSettings = dbSettings;
            this._container = unityContainer;
            this._navigationRepository = navigationRepository;
            this._regionManager = regionManager;
			this._customerConfigRepository = customerConfigRepository;

            this._adapters = new ObservableCollection<IExportErpModuleInfo>();
            this._locationItems = new ObservableCollection<LocationItemViewModel>();

            this._isFull = true;
			this._isExcludeNotExistingInCatalog = false;

            this._setAsDefaultCommand = new DelegateCommand(SetAsDefaultCommandExecuted, SetAsDefaultCommandCanExecute);

            this._navigateToGridCommand = _uiCommandRepository.Build(enUICommand.FromImportToGrid, NavigateToGridCommandExecuted);

			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			this._saveConfigToCustomerCommand = new DelegateCommand(SaveConfigToCustomerCommandExecuted, SaveConfigToCustomerCommandCanExecute);

		//	this._saveConfigToCustomerCommand = new DelegateCommand(SaveConfigToCustomerCommandExecuted, SaveConfigToCustomerCommandCanExecute);
			
			this._exportByConfigCommand = new DelegateCommand(ExportByConfigCommandExecuted, ExportByConfigCommandCanExecute);


			this.IsCheckedLocations = false;

			this._isFromGUIEnabled = true;
			this._isFromConfigEnabled = false;
			this._fromGUI = true;
			this._fromConfig = false;

			this._fromGUI = true;
			this._fromConfig = false;

			this._fromAdapter = false;
			this._fromCustomer = true;
			this._fromBranch = false;
			this._fromInventor = false;
        }

		/// <summary>
		///
		/// </summary>


		protected IExportErpModule DynViewModel
		{
			get { return _dynViewModel; }
			set { _dynViewModel = value; }
		}

		#region  fromConfig
		// ============= Config
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

				base.ExportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
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


				base.ExportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
				//base.NavigateToGridCommand.RaiseCanExecuteChanged();
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
				ExportByConfigCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
			}
		}

		public DelegateCommand ExportByConfigCommand
		{
			get { return _exportByConfigCommand; }
		}

		//договорились что смотрим только на Customer
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

		private void ExportByConfigCommandExecuted()
		{
			// this.DataInConfigPath похоже что не используется, НО строится 
			// и используется у каждого адаптера путь по умолчанию 
			// если путь меняется, то надо все перетестировать
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			using (new CursorWait("RunExportPdaByConfig"))
			{
				//this.SetSelectedAdapterWithoutGUI();	  !!ToDO
				IExportErpModule viewModel = this._dynViewModel;
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				//this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//ContextAdapterName  из customer 
				ExportErpFromConfigMode(viewModel, ContextCustomerAdapterName, fromConfigXDoc, base.State, null);
			}
			
	}

		private bool ExportByConfigCommandCanExecute()
		{
			if (IsBusy == true) return false;
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;

			if (this.SelectedAdapter == null) return false;
			string selectedAdapterName = this.SelectedAdapter.Name;
			_errorAdapterName = "";

			string adapterName = this.GetContextCustomerAdaperName();
			if (selectedAdapterName.ToLower() != adapterName.ToLower())
			{
				_errorAdapterName = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; //"Selected Adapter not equals Adapter for Customer";
				return false;
			}
	  
			string adapterConfigFileName = @"\" + adapterName + ".config";
			if (File.Exists(this.DataInConfigPath + adapterConfigFileName) == true) return true;

			return false;
		}

		//Для вызова из-вне. 
		public void ExportErpByConfig(
			IExportErpModule viewModel,
			CBIState state,
			string adapterName, 
			ConfigXDocFromEnum fromConfigXDoc,
			string comment,  //? это откуда запускаем, предположительно для process
			ImportModuleBaseViewModel complexViewModel)
		{
			//this.SetSelectedAdapterWithoutGUI(comment, /*complexViewModel,*/ state);
			ExportErpFromConfigMode(viewModel, adapterName, fromConfigXDoc, state, null);
		}

		public void ExportErpFromConfigMode(
			IExportErpModule viewModel, 
			string adapterName, 
			ConfigXDocFromEnum fromConfigXDoc, 
			CBIState state,
			ExportErpCommandInfo infoParam)
		{

			if (viewModel != null)
			{
				ExportErpCommandInfo info = new ExportErpCommandInfo();
				info.Callback = ShowLog;
				info.IsSaveFileLog = _isWriteLogToFile;
  				this._cts = new CancellationTokenSource();
				info.CancellationToken = this._cts.Token;
				//FillExportErpInfo(info);
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.IsFull = true;
				if (infoParam != null)
				{
					info.IturCodeList = infoParam.IturCodeList;
					info.LocationCodeList = infoParam.LocationCodeList;
				}
	
				viewModel.RunExportErpWithoutGUIBase(info, state);
			}
		}

		public void ClearExportErpByConfig(
			IExportErpModule viewModel,
			CBIState state,
			string adapterName,
			ConfigXDocFromEnum fromConfigXDoc)
		{
			this.ClearFromConfigMode(viewModel, adapterName, fromConfigXDoc, state);
		}

		public void ClearFromConfigMode(IExportErpModule viewModel,
			string adapterName,
			ConfigXDocFromEnum fromConfigXDoc,
			CBIState state)
		{
			string warningMessage = String.Empty;

			if (viewModel != null)
			{
				ExportErpCommandInfo info = new ExportErpCommandInfo();
				info.Callback = ShowLog;
				info.IsSaveFileLog = _isWriteLogToFile;
				this._cts = new CancellationTokenSource();
				info.CancellationToken = this._cts.Token;
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";

				viewModel.RunExportErpClearWithoutGUIBase(info, state);
			}
		}

	

		private string GetContextCustomerAdaperName()
		{
			string contextAdapterName = "";
			//if (this.SelectedAdapter != null)
			//{
			//	contextAdapterName = SelectedAdapter.Name;
			//}
			//if (this.FromCustomer == true)
			//{
				contextAdapterName = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
			//}
			//else if (this.FromBranch == true)
			//{
			//	contextAdapterName = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportERPAdapterCode;
			//}
			//else if (this.FromInventor == true)
			//{
			//	contextAdapterName = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportERPAdapterCode;
			//}

			if (string.IsNullOrWhiteSpace(contextAdapterName) == true)
				contextAdapterName = WarningAdapter.AdapterInCustomerNotSet; 
			return contextAdapterName;
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
			
	   //not use
		public string SaveConfigContent
		{
			get
			{
				if (SelectedAdapter == null) return String.Empty;
				return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for " + SelectedAdapter.Title; 
			}
		}

		public string SaveConfigForCustomer
		{
			get { return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for Customer"; }
		}

		
		//Not Use now
		private void SaveConfigCommandExecuted()
		{
			//if (this.SelectedAdapter == null) return;
			//string adapterDefaultConfigFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
			//string adapterName = this.SelectedAdapter.Name;
			//string adapterConfigFileName = @"\" + adapterName + ".config";
			//string fileConfig = adapterDefaultConfigFolderPath + adapterConfigFileName;

			//if (Directory.Exists(adapterDefaultConfigFolderPath) == false)
			//{
			//	Directory.CreateDirectory(adapterDefaultConfigFolderPath);
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

		//not use now
		private void SaveConfigToCustomerCommandExecuted()
		{
			if (this.CurrentCustomer == null) return;
			if (this.SelectedAdapter == null) return;
			string adapterName = this.SelectedAdapter.Name;

			Customer currentDomainObject = this.CurrentCustomer;
			string customerConfigPath = this._contextCBIRepository.GetConfigFolderPath(currentDomainObject);
			//string customerConfigPath = this.GetConfigFolderPath(currentDomainObject);

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
			//if (this.CurrentCustomer == null) return false;
			//return true;
			return false;
		}


		 //?? Not use
		private void ReloadConfigCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			if (Directory.Exists(this.DataInConfigPath) == false) return;

			if (this.SelectedAdapter == null) return;
			string adapterName = this.SelectedAdapter.Name;
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

			if (this.SelectedAdapter == null) return false;
			string adapterName = this.SelectedAdapter.Name;
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


		//not use now
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

			//	if (this.SelectedAdapter != null)
			//	{
			//		string adapterName = this.SelectedAdapter.Name;
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
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextCustomer.ToString());
				}
				else if (this.FromBranch == true)
				{
					currentDomainObject = base.CurrentBranch;
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextBranch.ToString());
				}
				else if (this.FromInventor == true)
				{
					currentDomainObject = base.CurrentInventor;
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextInventor.ToString());
				}
				if (this.DynViewModel != null)
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
		//	if (currentDomainObject != null)
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
		//}

		//public string GetConfigFolderPath(object domainObject)
		//{
		//	string subFolder = @"\Config";
		//	string path = "";
		//	if (domainObject == null) return path;
		//	string code = "";
		//	string currentObjectType = "";
		//	Customer customer = domainObject as Customer;
		//	if (customer != null)
		//	{
		//		code = customer.Code;
		//		currentObjectType = "Customer";
		//	}

		//	Branch branch = domainObject as Branch;
		//	if (branch != null)
		//	{
		//		code = branch.Code;
		//		currentObjectType = "Branch";
		//	}

		//	Inventor inventor = domainObject as Inventor;
		//	if (inventor != null)
		//	{
		//		code = inventor.Code;
		//		currentObjectType = "Inventor";
		//	}

		//	if (string.IsNullOrWhiteSpace(code) == true)
		//		return String.Empty;

		//	path =  UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);

		//	string pathWithSubFolder = path.TrimEnd('\\') + @"\" + subFolder.Trim('\\');
		//	if (Directory.Exists(pathWithSubFolder) == false)
		//	{
		//		try
		//		{
		//			Directory.CreateDirectory(pathWithSubFolder);
		//		}
		//		catch (Exception exp)
		//		{
		//			pathWithSubFolder = "";
		//		}
		//	}
		//	return pathWithSubFolder;
		//}

		//public string GetXDocumentConfigPath(ref ExportErpCommandInfo info)
		//{
		//	string adapterName = info.AdapterName;
		//	string adapterConfigFileName = @"\" + adapterName + ".config";
	
		//	string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
		//	string fileConfigPath = String.Empty;
		//	if (string.IsNullOrWhiteSpace(adapterName) == false)		   //by default будет браться с адаптера
		//	{
		//		fileConfigPath = adapterDefaultParamFolderPath + adapterConfigFileName; // @"\" + adapterName + @"\Config.xml";
		//	}

		//	switch (info.FromConfigXDoc)
		//	{
		//		case ConfigXDocFromEnum.InitWithoutConfig:
		//			info.ConfigXDocumentPath = String.Empty;
		//			fileConfigPath = String.Empty;
		//			break;
		//		case ConfigXDocFromEnum.FromConfigXDocument:
		//		case ConfigXDocFromEnum.FromDefaultAdapter:
		//		case ConfigXDocFromEnum.FromRootPath:
		//		case ConfigXDocFromEnum.FromRootFolderAndCustomer:
		//		case ConfigXDocFromEnum.FromRootFolderAndBranch:
		//		case ConfigXDocFromEnum.FromRootFolderAndInventor:
		//		case ConfigXDocFromEnum.FromFtpCustomer:
		//		case ConfigXDocFromEnum.FromFtpBranch:
		//		case ConfigXDocFromEnum.FromFtpInventor:
		//			break;

		//		case ConfigXDocFromEnum.FromCustomerInData:
		//			fileConfigPath = this.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromBranchInData:
		//			fileConfigPath = this.GetConfigFolderPath(base.CurrentBranch) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromInventorInData:
		//			fileConfigPath = this.GetConfigFolderPath(base.CurrentInventor) + adapterConfigFileName;							 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromDomainObjectInData:
		//			object currentDomainObject = base.GetCurrentDomainObject();
		//			fileConfigPath = this.GetConfigFolderPath(currentDomainObject) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromFullPath:
		//			fileConfigPath = info.ConfigXDocumentPath;
		//			break;


		//	}
		//return fileConfigPath;
		//}
		///// ============= End Config
		#endregion  fromConfig

		public bool IsCheckedLocations
		{
			get { return _isCheckedLocations; }
			set
			{
				_isCheckedLocations = value;
				this.LocationItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedLocations);

			}
		}

        public ObservableCollection<IExportErpModuleInfo> Adapters
        {
            get { return _adapters; }
        }

        public IExportErpModuleInfo SelectedAdapter
        {
            get { return _selectedAdapter; }
            set
            {
                _selectedAdapter = value;

                RaisePropertyChanged(() => SelectedAdapter);

                if (this._selectedAdapter != null)
                {
                    UriQuery query = Utils.UriQueryFromNavigationContext(base.NavigationContext);

                    this._container.RegisterType(typeof(object), this._selectedAdapter.UserControlType, Common.ViewNames.ExportErpByModuleView);
                    this._regionManager.RequestNavigate(Common.RegionNames.ExportErpByModule, new Uri(Common.ViewNames.ExportErpByModuleView + query, UriKind.Relative));


                    IRegion region = this._regionManager.Regions[Common.RegionNames.ExportErpByModule];
                    UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
                    if (userControl != null)
                    {
                        this._dynViewModel = userControl.DataContext as IExportErpModule;
                        if (this._dynViewModel != null)
                        {
                            this._dynViewModel.RaiseCanExport = () => this._exportCommand.RaiseCanExecuteChanged();
                            this._dynViewModel.UpdateLog = r => Utils.RunOnUI(() =>
                                                                                  {
                                                                                      _log = r;
                                                                                      _logCommand.RaiseCanExecuteChanged();
																					  _configCommand.RaiseCanExecuteChanged();
                                                                                  });
                            this._dynViewModel.SetIsBusy = r => Utils.RunOnUI(() => this.IsBusy = r);
                        }
                    }

					this._isExcludeNotExistingInCatalog = false;
					string keyCode = base.CurrentCustomer.Code + "|" + _selectedAdapter.Name;
					Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
					if (configDictionary != null)
					{
						this._isExcludeNotExistingInCatalog = configDictionary.GetBoolValue(this._isExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
						
					}
					RaisePropertyChanged(() => IsEcludeNotExistingInCatalog);
                }

                _exportCommand.RaiseCanExecuteChanged();
                _setAsDefaultCommand.RaiseCanExecuteChanged();

				CompareSelectedAdapterInventorAndCustomer();
            }
        }


		public void SetSelectedAdapterWithoutGUI( )
		{
			if (this._dynViewModel != null)
			{
				this._dynViewModel.RaiseCanExport = () => this._exportCommand.RaiseCanExecuteChanged();
				this._dynViewModel.UpdateLog = r => Utils.RunOnUI(() =>
				{
					_log = r;
					_logCommand.RaiseCanExecuteChanged();
					_configCommand.RaiseCanExecuteChanged();
				});
				this._dynViewModel.SetIsBusy = r => Utils.RunOnUI(() => this.IsBusy = r);
			}

			this._isExcludeNotExistingInCatalog = false;
			//if (state != null)
			//{
			//	if (state.CurrentCustomer != null)
			//	{
			//		string keyCode = state.CurrentCustomer.Code + "|" + comment;

			//		Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			//		if (configDictionary != null)
			//		{
			//			this._isExcludeNotExistingInCatalog = configDictionary.GetBoolValue(this._isExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
			//		}
			//	}
			//}
			

			//OnSelectedAdapterChanged();
		}

        public bool IsFull
        {
            get { return _isFull; }
            set
            {
                _isFull = value;
                RaisePropertyChanged(() => IsFull);
            }
        }


		public bool IsEcludeNotExistingInCatalog
        {
			get { return this._isExcludeNotExistingInCatalog; }
            set
            {
				this._isExcludeNotExistingInCatalog = value;
				RaisePropertyChanged(() => IsEcludeNotExistingInCatalog);
            }
        }
		

        public string ItursText
        {
            get { return _itursText; }
            set
            {
                _itursText = value;
                RaisePropertyChanged(() => ItursText);

                _exportCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<LocationItemViewModel> LocationItems
        {
            get { return _locationItems; }
        }

	
        public bool IsFilterByItur
        {
            get { return _isFilterByItur; }
            set
            {
                _isFilterByItur = value;
                RaisePropertyChanged(() => IsFilterByItur);

                _exportCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsFilterByLocation
        {
            get { return _isFilterByLocation; }
            set
            {
                _isFilterByLocation = value;
                RaisePropertyChanged(() => IsFilterByLocation);
            }
        }

        public string ItursPrefix
        {
            get { return _itursPrefix; }
            set
            {
                _itursPrefix = value;
                RaisePropertyChanged(() => ItursPrefix);

                _exportCommand.RaiseCanExecuteChanged();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._autoClose = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoCloseExportErpWindow);

            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;
			this._isFromConfigEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;

			List<IExportErpModuleInfo> adapters = Utils.GetExportErpAdapters(this._container, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode);

            foreach (var adapter in adapters)
            {
                _adapters.Add(adapter);
            }

            IExportErpModuleInfo selected = _adapters.FirstOrDefault(r => r.IsDefault);

            string defaultAdapterCodeByCBI = GetDefaultAdapterName();
            if (!String.IsNullOrWhiteSpace(defaultAdapterCodeByCBI))
            {
                if (this._adapters.Any(r => r.Name == defaultAdapterCodeByCBI))
                    selected = _adapters.FirstOrDefault(r => r.Name == defaultAdapterCodeByCBI);
            }          

            SelectedAdapter = selected;


			// ==============  Export Erp	Adapter From Customer
			//this._selectedExportErpName = "";
			//if (base.CurrentCustomer != null)
			//{
			//	string exportErpCode = base.CurrentCustomer.ExportERPAdapterCode;
			//	IExportErpModuleInfo defaultExportErp = _adapters.FirstOrDefault(r => r.IsDefault);
			//	if (String.IsNullOrWhiteSpace(exportErpCode) == false)
			//	{
			//		if (this._adapters.Any(r => r.Name == exportErpCode))
			//			defaultExportErp = _adapters.FirstOrDefault(r => r.Name == exportErpCode);
			//	}
			//	this.SelectedExportErpName = defaultExportErp.Name;
				//ConfigFileExportErpExists = IsConfigFileExportErpExists(this.SelectedExportErp);
			//}
			//=================
			this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
			RaisePropertyChanged(() => this.ContextCustomerAdapterName);


            BuildLocation();

			this._isExcludeNotExistingInCatalog = false;
			if (SelectedAdapter != null)
			{
				string keyCode = currentCustomerCode + "|" + SelectedAdapter.Name;
				Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
				if (configDictionary != null)
				{
					this._isExcludeNotExistingInCatalog = configDictionary.GetBoolValue(this._isExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
				}
			}
			RaisePropertyChanged(() => IsEcludeNotExistingInCatalog);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoStartExportErp))
            {
                if (_exportCommand.CanExecute())
                {
                    _exportCommand.Execute();
                }
                else
                {
                    if (_autoClose)
                    {
                        CloseModalWindow();
                    }
                }
            }


			
        }

		private void CompareSelectedAdapterInventorAndCustomer()
		{

			if (this.SelectedAdapter != null)
			{
				string selectedAdapterName = this.SelectedAdapter.Name;
				_errorAdapterName = "";

				string adapterName = this.GetContextCustomerAdaperName();
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

            if (_dynViewModel != null)
            {
                _dynViewModel.OnNavigatedFrom(navigationContext);
            }

			
        }

		//private string GetDefaultAdapterName()
		//{
		//	switch (base.CBIDbContext)
		//	{
		//		case NavigationSettings.CBIDbContextCustomer:
		//			return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
		//		case NavigationSettings.CBIDbContextBranch:
		//			return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportERPAdapterCode;
		//		case NavigationSettings.CBIDbContextInventor:
		//			return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportERPAdapterCode;
		//	}

		//	return String.Empty;
		//}


		private string GetSelectedAdapterNameByCBI(string cbidbContext)
		{
			string exportCatalogCustomerAdapterCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
			string exportBranchCatalogAdapterCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportERPAdapterCode;
			string exportInventorCatalogAdapterCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportERPAdapterCode;
			switch (cbidbContext)
			{
				case NavigationSettings.CBIDbContextCustomer:
					{
						return exportCatalogCustomerAdapterCode;
					}
				case NavigationSettings.CBIDbContextBranch:
					{
						if (string.IsNullOrWhiteSpace(exportBranchCatalogAdapterCode) == false) return exportBranchCatalogAdapterCode;
						else return exportCatalogCustomerAdapterCode;
					}
				case NavigationSettings.CBIDbContextInventor:
					{
						if (string.IsNullOrWhiteSpace(exportInventorCatalogAdapterCode) == false) return exportInventorCatalogAdapterCode;
						if (string.IsNullOrWhiteSpace(exportBranchCatalogAdapterCode) == false) return exportBranchCatalogAdapterCode;
						else return exportCatalogCustomerAdapterCode;
					}
			}

			return String.Empty;
		}


		//ADD
		public string GetExportErpFolderPath(object currentDomainObject)
		{
			//if (string.IsNullOrWhiteSpace(code) == true)
			//	return String.Empty;

			string currentObjectType = "";
			string code = "";
			if (currentDomainObject is Customer)
			{
				Customer customer = currentDomainObject as Customer;
				currentObjectType = "Customer";
				code = customer.Code;
			}
			else if (currentDomainObject is Branch)
			{
				Branch branch = currentDomainObject as Branch;
				currentObjectType = "Branch";
				code = branch.Code;
			}
			else if (currentDomainObject is Inventor)
			{
				Inventor inventor = currentDomainObject as Inventor;
				currentObjectType = "Inventor";
				code = inventor.Code;
			}
			//if (this._isDefaultAdapterFromCustomer == true)
			//{
			//	currentObjectType = "Customer";
			//	code = base.CurrentCustomer.Code;
			//}
			//else if (this._isDefaultAdapterFromBranch == true)
			//{
			//	currentObjectType = "Branch";
			//	code = base.CurrentBranch.Code;
			//}
			//else if (this._isDefaultAdapterFromInventor == true)
			//{
			//	currentObjectType = "Inventor";
			//	code = base.CurrentInventor.Code;
			//}


			return UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);
		}


		private string GetDefaultAdapterName()
		{
			string exportCatalogCustomerAdapterCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportERPAdapterCode;
			string exportBranchCatalogAdapterCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportERPAdapterCode;
			string exportInventorCatalogAdapterCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportERPAdapterCode;
			switch (base.CBIDbContext)
			{
				case NavigationSettings.CBIDbContextCustomer:
					{
						return exportCatalogCustomerAdapterCode;
					}
				case NavigationSettings.CBIDbContextBranch:
					{
						if (string.IsNullOrWhiteSpace(exportBranchCatalogAdapterCode) == false) return exportBranchCatalogAdapterCode;
						else return exportCatalogCustomerAdapterCode;
					}
				case NavigationSettings.CBIDbContextInventor:
					{
						if (string.IsNullOrWhiteSpace(exportInventorCatalogAdapterCode) == false) return exportInventorCatalogAdapterCode;
						if (string.IsNullOrWhiteSpace(exportBranchCatalogAdapterCode) == false) return exportBranchCatalogAdapterCode;
						else return exportCatalogCustomerAdapterCode;
					}
			}

			return String.Empty;
		}

        protected override bool ExportCommandCanExecute()
        {
			if (SelectedAdapter == null) return false;
			if (this._fromConfig == true)
			{
				//string selectedAdapterName = SelectedAdapter.Name;
				//string contextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//if (selectedAdapterName.ToLower() != contextCustomerAdapterName.ToLower()) return false;
				return false;
			}

            bool isOkByIturs = true;

            if (_isFilterByItur)
            {
                isOkByIturs = IsItursTextValid() && !String.IsNullOrWhiteSpace(_itursPrefix) && IsItursPrefixValid();
            }

            return _selectedAdapter != null && isOkByIturs;
	
        }

		protected override bool ClearCommandCanExecute()
        {
			if (SelectedAdapter == null) return false;
			if (this._fromConfig == true)
			{
				//string selectedAdapterName = SelectedAdapter.Name;
				//string contextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//if (selectedAdapterName.ToLower() != contextCustomerAdapterName.ToLower()) return false;
				return false;
			}

			return true;
		 }

		protected override void ExportCommandExecuted()
		{
			ExportWithInitMode(this._fromGUI, this._fromConfig);
		}


		private void ExportWithInitMode(bool initFromGUI = true, bool orInitFromConfig = false)
	    {
			if (initFromGUI == true)
			{
				if (this._dynViewModel != null)
				{

					ExportErpCommandInfo info = new ExportErpCommandInfo();
					info.Callback = () =>
					{
						if (_autoClose)
						{
							CloseModalWindow();
						}
						else
						{
							ShowLog();
						}
					};

					info.IsSaveFileLog = _isWriteLogToFile;

					this._cts = new CancellationTokenSource();
					info.CancellationToken = this._cts.Token;

					FillExportErpInfo(info);
					info.AdapterName = "";
					info.ConfigXDocument = null;
					info.ConfigXDocumentPath = "";
					info.FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;

					this._dynViewModel.RunExportBase(info);
				}
			}
			else if (orInitFromConfig == true)
			{
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				IExportErpModule viewModel = this._dynViewModel;
				ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
				ExportErpFromConfigMode(viewModel, ContextCustomerAdapterName, fromConfigXDoc, base.State, null);
			}
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

        protected override bool LogCommandCanExecute()
        {
            return !String.IsNullOrEmpty(_log);
        }

        protected override void LogCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = ViewNames.ExportLogView;
            payload.WindowTitle = WindowTitles.ViewExportErpLog;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            ExportLogViewData data = new ExportLogViewData();
            data.Log = _log;
            if (_dynViewModel != null)
                data.Path = _dynViewModel.BuildPathToExportErpDataFolder();

			UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, data);
            OnModalWindowRequest(payload);
        }

		protected override bool ConfigCommandCanExecute()
		{
			if (this.CurrentInventor == null) return false;
			return true;
		}

		protected override void ConfigCommandExecuted()
		{

			ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
			payload.Settings = new Dictionary<string, string>();
			payload.ViewName = ViewNames.ConfigEditAndSaveView;
			payload.WindowTitle = WindowTitles.ViewConfig;
			payload.PathInData = this._dbSettings.ImportFolderPath();
			Utils.AddContextToDictionary(payload.Settings, base.Context);
			Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
			XDocument doc = new XDocument();
			string xmlString = "";

			IRegion region = this._regionManager.Regions[Common.RegionNames.ExportErpByModule];
			UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
			if (userControl != null)
			{
				if (userControl.DataContext != null)
				{
					string dataContextViewModelName = userControl.DataContext.GetType().Name;
					ExportErpCommandInfo info = new ExportErpCommandInfo();
					this.FillExportErpInfo(info);
					XElement root = ViewModelConfigRepository.GetXElementExportERPAdapterProperty(userControl.DataContext,
						dataContextViewModelName,
						SelectedAdapter, info);
					doc.Add(root);
					xmlString = doc.ToString();
				}
			}


			string _configXML = "";
			_configXML = _configXML + Environment.NewLine + xmlString;

			ExportLogViewData data = new ExportLogViewData();
			data.ConfigXML = _configXML;
			data.InDataPath = this._dynViewModel.BuildPathToExportErpDataFolder();
			string getConfigFolderPath = this._contextCBIRepository.GetConfigFolderPath(base.State.CurrentCustomer);
			data.DataInConfigPath = getConfigFolderPath;
			//data.DataInConfigPath = this.GetConfigFolderPath(base.State.CurrentCustomer);
			//if (this.DynViewModel != null)
			//data.Path = @"C:\MIS";//this.DynViewModel.BuildPathToExportErpDataFolder();

			data.AdapterType = "ExportErpWithModulesViewModel";
			data.AdapterName = "";
			if (SelectedAdapter != null)
			{
				data.AdapterName = SelectedAdapter.Name;
			}
			UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, data);
			OnModalWindowRequest(payload);
		}

        protected override void ClearCommandExecuted()
        {
            if (this._dynViewModel == null) return;

            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Content = Localization.Resources.Msg_Clear_Export_ERP;
            notification.Image = MessageBoxImage.Warning;
            notification.Settings = this._userSettingsManager;

            ExportClearCommandInfo info = new ExportClearCommandInfo();
            info.Callback = () =>
            {
                Utils.SetCursor(false);
                base.ShowLog();
            };

            this._yesNoRequest.Raise(notification, r =>
            {
                if (r.IsYes)
                {
                    Utils.SetCursor(true);
                    this._dynViewModel.RunClear(info);
                }
            });
        }

		public void SaveConfigByDefaultForCustomer(Customer customer,
			IExportErpModuleInfo selectedAdapter,
			ExportErpModuleBaseViewModel exportErpModuleBaseViewModel,
			DomainObjectType fromDomainObjectType, HowUse pathHowUse,
			bool resave = false)
		{
			//IExportErpModule exportErpModule = exportErpWithModulesViewModel as IExportErpModule;		
			if (customer == null) return;
			if (selectedAdapter == null) return;
			XDocument doc = new XDocument();
			string xmlString = "";

			//IRegion region = this._regionManager.Regions[Common.RegionNames.ExportErpByModule];
			//UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
			//UserControl userControl = exportErpModule as UserControl;
			//UserControl userControl = selectedAdapter.UserControlType as UserControl;
			//if (userControl != null)
			//{
			//	if (userControl.DataContext != null)
			//	{
			string dataContextViewModelName = exportErpModuleBaseViewModel.GetType().Name;
					ExportErpCommandInfo info = new ExportErpCommandInfo();
					this.FillExportErpInfo(info);
					XElement root = ViewModelConfigRepository.GetXElementExportERPAdapterProperty(
						exportErpModuleBaseViewModel,
						dataContextViewModelName,
						selectedAdapter, info);
					doc.Add(root);
					xmlString = doc.ToString();
			//	}

			//}

			//string inDataPath = UtilsPath.ExportErpFolder(this._dbSettings, "Customer", customer.Code);
			string getConfigFolderPath = this._contextCBIRepository.GetConfigFolderPath(customer);
			//string adapterType = "ExportErpWithModulesView";
			string adapterName = "";
			if (selectedAdapter != null)
			{
				adapterName = selectedAdapter.Name;
			}

			// Save file config
			if (String.IsNullOrWhiteSpace(adapterName) == true) return;


			string customerConfigPath = this._contextCBIRepository.GetConfigFolderPath(customer);

			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfig = customerConfigPath + adapterConfigFileName;

			if (Directory.Exists(customerConfigPath) == false)
			{
				Directory.CreateDirectory(customerConfigPath);
			}

			if (resave == true)
			{
				if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
				{
					try
					{

						File.Delete(fileConfig);
					}

					catch { }
				}
			}

			if (File.Exists(fileConfig) == false)	   //заменим файл config.xml
			{
				XDocument configXDocument = XDocument.Parse(xmlString);
				configXDocument = ViewModelConfigRepository.VerifyToAdapter(configXDocument, selectedAdapter);
			//		public static XDocument UpdatePath(XDocument doc, string adapterType, string fromDomainObjectType, HowUse pathHowUse,
			//string absolutePath = @"c:\temp", bool _isDefault = true)

				configXDocument = ViewModelConfigRepository.UpdatePath(configXDocument,
					"ExportErpWithModulesViewModel", fromDomainObjectType, pathHowUse,/* DomainObjectType.inventor, HowUse.relative, */ "");
				configXDocument.Save(fileConfig);
			}
		}
	
        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "ItursText":
                        {
                            if (!this.IsItursTextValid())
                            {
                                return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
                            }
                        }
                        break;
                    case "ItursPrefix":
                        if (!IsItursPrefixValid())
                        {
                            return Localization.Resources.ViewModel_ExportErpWithModules_invalidPrefix;
                        }
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public DelegateCommand SetAsDefaultCommand
        {
            get { return _setAsDefaultCommand; }
        }

        public DelegateCommand NavigateToGridCommand
        {
            get { return _navigateToGridCommand; }
        }

        private bool IsItursTextValid()
        {
            return CommaDashStringParser.IsValid(this._itursText);
        }

        private bool IsItursPrefixValid()
        {
            if (String.IsNullOrWhiteSpace(_itursPrefix))
                return true;

            int dummy;
            return Int32.TryParse(_itursPrefix, out dummy);
        }

        private void BuildLocation()
        {
            try
            {
                _locationItems.Clear();

                _locationListViewModelBuilder.Build(_locationItems, base.GetDbPath, null,  r => { return false; }, r => { return false; });

            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
            }

        }

        private void FillExportErpInfo(ExportErpCommandInfo info)
        {
            if (info == null) return;

            info.IsFull = this._isFull;
			 info.IsExcludeNotExistingInCatalog = this._isExcludeNotExistingInCatalog;
																							
            info.IsFilterByIturs = _isFilterByItur;
            info.IsFilterByLocations = _isFilterByLocation;

            foreach (LocationItemViewModel locationItemViewModel in _locationItems.Where(r => r.IsChecked))
            {
                info.LocationCodeList.Add(locationItemViewModel.Location.Code);
            }

            if (!String.IsNullOrWhiteSpace(_itursPrefix) && !String.IsNullOrWhiteSpace(_itursText))
            {
                foreach (int i in CommaDashStringParser.Parse(_itursText))
                {
                    string prefix = UtilsItur.PrefixFromString(_itursPrefix);
                    string suffix = UtilsItur.SuffixFromNumber(i);
                    info.IturCodeList.Add(UtilsItur.CodeFromPrefixAndSuffix(prefix, suffix));
                }
            }
        }

        private bool SetAsDefaultCommandCanExecute()
        {
            return _selectedAdapter != null && _selectedAdapter.Name != this.GetDefaultAdapterName();
        }

        private void SetAsDefaultCommandExecuted()
        {
			string adapterName = SelectedAdapter.Name;
            switch (base.CBIDbContext)
            {
                case NavigationSettings.CBIDbContextCustomer:
					base.CurrentCustomer.ExportERPAdapterCode = adapterName;
                    this._customerRepository.Update(base.CurrentCustomer);
                    break;
                case NavigationSettings.CBIDbContextBranch:
					base.CurrentBranch.ExportERPAdapterCode = adapterName;
                    this._branchRepository.Update(base.CurrentBranch);
                    break;
                case NavigationSettings.CBIDbContextInventor:
					base.CurrentInventor.ExportERPAdapterCode = adapterName;
                    this._inventorRepository.Update(base.CurrentInventor);
                    break;
            }

            _setAsDefaultCommand.RaiseCanExecuteChanged();
        }

        private void NavigateToGridCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());


            UtilsNavigate.InventProductListOpen(this._regionManager, query);
        }

        private void CloseModalWindow()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

    }

	public static class ExportErpWithModuleParamParse
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