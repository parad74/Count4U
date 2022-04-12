using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.ExportPda;
using Count4U.Localization;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Xml.Config;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.Audit.ViewModels.Export
{
    public class ExportPdaWithModulesViewModel : ExportWithModulesBaseViewModel
    {
        private readonly IUnityContainer _container;
		private readonly IServiceLocator _serviceLocator;
		private readonly IRegionManager _regionManager;
        private readonly ICustomerConfigRepository _customerConfigRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ModalWindowLauncher _modalWindowLauncher;
		private readonly IEventAggregator _eventAggregator;
		private readonly ICustomerRepository _customerRepository;
		private readonly IBranchRepository _branchRepository;
		private readonly IInventorRepository _inventorRepository;
		protected readonly DBSettings _dbSettings;

        private readonly DelegateCommand _setAsDefaultCommand;
        private readonly DelegateCommand _navigateToGridCommand;

        private ObservableCollection<IExportPdaModuleInfo> _adapters;
        private IExportPdaModuleInfo _selectedAdapter;

         private ExportPdaModuleBaseViewModel _dynViewModel;

		
        //private IExportPdaModule _dynViewModel;
        private readonly DelegateCommand _uploadToPDACommand;
		private readonly DelegateCommand _downloadFromPDACommand;

        private bool _isWithoutProductName;
        private bool _isBarcodeWithoutMask;
        private bool _isMakatWithoutMask;
		 private bool _isCheckBaudratePDA;
		 private bool _isMISVisible;
		 private bool _isHT360Visible;

		 private bool _isMaskShow;

		 public string _adapterName;
		 private bool _exportPdaVisibility = false;
		 private bool _exportSettingsVisibility = true;
		 private bool _programTypeVisibility = true;

		 private ExportPdaSettingsControlViewModel _exportPdaSettingsControlViewModel;
		 private ExportPdaProgramTypeViewModel _exportPdaProgramTypeControlViewModel;
		 private ExportPdaMerkavaAdapterViewModel _exportPdaMerkavaAdapterControlViewModel;

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

		//для процесса экпорта и отправки в оффис
		//private bool _autoClose;
        public ExportPdaWithModulesViewModel(
            IContextCBIRepository contextCbiRepository,
            IUnityContainer container,
			IServiceLocator serviceLocator,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            ICustomerConfigRepository customerConfigRepository,
            INavigationRepository navigationRepository,
            IImportAdapterRepository importAdapterRepository,
            IUserSettingsManager userSettingsManager,
            UICommandRepository commandRepository,
			ICustomerRepository customerRepository,
			IBranchRepository branchRepository,
			IInventorRepository inventorRepository,
            ModalWindowLauncher modalWindowLauncher,
			DBSettings dbSettings)
            : base(contextCbiRepository, eventAggregator, commandRepository)
        {
             this._modalWindowLauncher = modalWindowLauncher;
            this. _userSettingsManager = userSettingsManager;
             this._importAdapterRepository = importAdapterRepository;
            this._navigationRepository = navigationRepository;
            this._customerConfigRepository = customerConfigRepository;
            this._regionManager = regionManager;
            this._container = container;
			this._serviceLocator = serviceLocator;
			this._eventAggregator = eventAggregator;
			this._inventorRepository = inventorRepository;
			this._branchRepository = branchRepository;
			this._customerRepository = customerRepository;
			this._dbSettings = dbSettings;

            this._adapters = new ObservableCollection<IExportPdaModuleInfo>();
            this._setAsDefaultCommand = new DelegateCommand(SetAsDefaultCommandExecuted, SetAsDefaultCommandCanExecute);
			
            this._navigateToGridCommand = _commandRepository.Build(enUICommand.FromImportToGrid, NavigateToGridCommandExecuted);
            this._uploadToPDACommand = commandRepository.Build(enUICommand.UploadToPDA, UploadToPDACommandExecuted, UploadToPDACommandCanExecute);
			this._downloadFromPDACommand = commandRepository.Build(enUICommand.DownloadFromPDA, DownloadFromPDACommandExecuted, DownloadFromPDACommandCanExecute);

			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			this._saveConfigToCustomerCommand = new DelegateCommand(SaveConfigToCustomerCommandExecuted, SaveConfigToCustomerCommandCanExecute);
			this._exportByConfigCommand = new DelegateCommand(ExportByConfigCommandExecuted, ExportByConfigCommandCanExecute);


			this._isCheckBaudratePDA = userSettingsManager.UploadOptionsHT630_BaudratePDAGet();
			this.IsMISVisible	 = false;
			this.IsHT360Visible = false;
			this.IsMaskShow = false;

			this._isFromGUIEnabled = true;
			this._isFromConfigEnabled = false;

			this._fromGUI = true;
			this._fromConfig = false;

			this._fromAdapter = false;
			this._fromCustomer = true;
			this._fromBranch = false;
			this._fromInventor = false;
			
        }

		protected IServiceLocator ServiceLocator
		{
			get { return _serviceLocator; }
		} 

		protected ExportPdaModuleBaseViewModel DynViewModel
		{
			get { return _dynViewModel; }
			set { _dynViewModel = value; }
		}

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
				this.ExportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
				_uploadToPDACommand.RaiseCanExecuteChanged();
				_downloadFromPDACommand.RaiseCanExecuteChanged();
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
				this.ExportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
				_uploadToPDACommand.RaiseCanExecuteChanged();
				_downloadFromPDACommand.RaiseCanExecuteChanged();
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
				this.ExportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
			}
		}

		public DelegateCommand ExportByConfigCommand
		{
			get { return _exportByConfigCommand; }
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

		//_errorAdapterName = "Selected Adapter not equals Adapter for Customer";		 = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; 
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


		//из формы Adaptera export to PDA init form config
		private void ExportByConfigCommandExecuted()
		{
			// this.DataInConfigPath похоже что не используется, НО строится 
			// и используется у каждого адаптера путь по умолчанию 
			// если путь меняется, то надо все перетестировать
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;

			using (new CursorWait("RunExportPdaByConfig"))
			{
				IExportPdaModule viewModel = this._dynViewModel;		//   это из GUI
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				//this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//IExportPdaModule viewModel = Get_ExportPdaModuleBaseViewModel_AsIExportPdaModule(this.ContextCustomerAdapterName);
				//this.SetSelectedAdapterWithoutGUI(this._dynViewModel);		  !!ToDO
				this.ExportFromConfigMode(viewModel, this.ContextCustomerAdapterName, fromConfigXDoc, base.State, null);
			}
		}

		private bool ExportByConfigCommandCanExecute()
		{
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

			if (adapterName == WarningAdapter.AdapterInCustomerNotSet) return false;
			string adapterConfigFileName = @"\" + adapterName + ".config";

			if (File.Exists(this.DataInConfigPath + adapterConfigFileName) == true) return true;

			return false;
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


		//Not use naow ?
		private void SaveConfigCommandExecuted()
		{
			//if (this.SelectedAdapter == null) return;
			//string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
			//string adapterName = this.SelectedAdapter.Name;
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

		//Not use now?
		private void SaveConfigToCustomerCommandExecuted()
		{
			if (this.CurrentCustomer == null) return;
			if (this.SelectedAdapter == null) return;
			string adapterName = this.SelectedAdapter.Name;

			Customer currentDomainObject = this.CurrentCustomer;
			string customerConfigPath = this._contextCBIRepository.GetConfigFolderPath(currentDomainObject);
			//string customerConfigPath = this.GetConfigFolderPath(currentDomainObject);
		
			string customerConfigFileName = @"\" + adapterName + ".config";
			string fileConfig = customerConfigPath + customerConfigFileName;

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

		//not use now ?
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

		//not use now ?
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
					//this.DataInConfigPath = this.GetConfigFolderPath(currentDomainObject);
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

		//public  string GetConfigFolderPath(object currentDomainObject)
		//{
		//	string subFolder = @"\Config";
		//	string path = "";
		//	if (currentDomainObject == null) return path;
		//	path = base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
		//	if (string.IsNullOrWhiteSpace(subFolder) == true) return path;

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

		public bool IsHT360Visible
		{
			get { return _isHT360Visible; }
			set
			{
				_isHT360Visible = value;
				RaisePropertyChanged(() => IsHT360Visible);
			}
		}

		public bool IsMISVisible
		{
			get { return _isMISVisible; }
			set
			{
				_isMISVisible = value;
				RaisePropertyChanged(() => IsMISVisible);
			}
		}




		public bool IsMaskShow
		{
			get { return _isMaskShow; }
			set
			{
				_isMaskShow = value;
				RaisePropertyChanged(() => IsMaskShow);
			}
		}
		public ExportPdaSettingsControlViewModel ExportPdaSettingsControl
		{
			get { return _exportPdaSettingsControlViewModel; }
			//set { _exportPdaSettingsControlViewModel = value; }
		}

		public ExportPdaProgramTypeViewModel ExportPdaProgramTypeControl
		{
			get { return _exportPdaProgramTypeControlViewModel; }
			//set { _exportPdaProgramTypeViewModel = value; }
		}

		public ExportPdaMerkavaAdapterViewModel ExportPdaMerkavaAdapterControl
		{
			get { return _exportPdaMerkavaAdapterControlViewModel; }
			//set { _exportPdaAdapterControlViewModel = value; }
		}

		public bool UploadToPDACommandCanExecute()
		{
			if (this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter	 )
				//|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
				//|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter)
			{
				//RaisePropertyChanged(() => SelectedAdapter);
				return false;
			}
			else { return true; }
			
		}


	

		private bool DownloadFromPDACommandCanExecute()
		{
			if (this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter
				|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
				|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
				|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
				|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
				|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter
				)
			{
					return false;
			}
			else
			{
				return true; 
			}

		}

	

        public ObservableCollection<IExportPdaModuleInfo> Adapters
        {
            get { return _adapters; }
			set { _adapters = value; }
        }

        public IExportPdaModuleInfo SelectedAdapter
        {
            get { return _selectedAdapter; }
			set
			{
				_selectedAdapter = value;
				RaisePropertyChanged(() => SelectedAdapter);

				using (new CursorWait())
				{
					if (this._dynViewModel != null)
						this._dynViewModel.ClearRegions();

					if (this.SelectedAdapter != null)
					{
						UriQuery query = Utils.UriQueryFromNavigationContext(base.NavigationContext);

						query.Add(Common.NavigationSettings.AdapterName, this._selectedAdapter.Name);

						this._container.RegisterType(typeof(object), this._selectedAdapter.UserControlType, Common.ViewNames.ExportByModuleView);
						this._regionManager.RequestNavigate(Common.RegionNames.ExportByModule, new Uri(Common.ViewNames.ExportByModuleView + query, UriKind.Relative));

						IRegion region = this._regionManager.Regions[Common.RegionNames.ExportByModule];
						UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
						if (userControl != null)
						{
							this._dynViewModel = userControl.DataContext as ExportPdaModuleBaseViewModel;
							if (this._dynViewModel != null)
							{
								this._dynViewModel.RaiseCanExport = () => this._exportCommand.RaiseCanExecuteChanged();
								this._dynViewModel.UpdateLog = r => Application.Current.Dispatcher.Invoke(new Action(() =>
								{
									_log = r;
									_logCommand.RaiseCanExecuteChanged();
									_configCommand.RaiseCanExecuteChanged();
								}));
								this._dynViewModel.SetIsBusy = r => Application.Current.Dispatcher.Invoke(new Action(() =>
								{
									this.IsBusy = r;
								}));
							}
						}
					}

					this._exportPdaSettingsControlViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsControlViewModel>(
						Common.RegionNames.ExportPdaSettings, this._regionManager);
					this._exportPdaProgramTypeControlViewModel = Utils.GetViewModelFromRegion<ExportPdaProgramTypeViewModel>(
					Common.RegionNames.ExportPdaProgramType, this._regionManager);
					this._exportPdaMerkavaAdapterControlViewModel = Utils.GetViewModelFromRegion<ExportPdaMerkavaAdapterViewModel>(
						Common.RegionNames.ExportPdaAdapter, this._regionManager);

					
					this.FillSelectExportPdaAdapter(this._selectedAdapter, base.CurrentCustomer);

				
					_uploadToPDACommand.RaiseCanExecuteChanged();
					_downloadFromPDACommand.RaiseCanExecuteChanged();
					_exportCommand.RaiseCanExecuteChanged();
					_setAsDefaultCommand.RaiseCanExecuteChanged();
				}
				CompareSelectedAdapterInventorAndCustomer();
			}
        }

		public void SetSelectedAdapterWithoutGUI( ExportPdaModuleBaseViewModel viewModel /*string comment, CBIState state, IImportModuleInfo info, ImportModuleBaseViewModel viewModel*/)
		{
			if (this._dynViewModel != null)
			{
				this._dynViewModel.RaiseCanExport = () => this._exportCommand.RaiseCanExecuteChanged();
				this._dynViewModel.UpdateLog = r => Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					_log = r;
					_logCommand.RaiseCanExecuteChanged();
					_configCommand.RaiseCanExecuteChanged();
				}));
				this._dynViewModel.SetIsBusy = r => Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					this.IsBusy = r;
				}));
			}
		  	
			OnSelectedAdapterChanged();
		}

		public void FillSelectExportPdaAdapter(IExportPdaModuleInfo selectedAdapter, Customer customer)
		{
			this._adapterName = selectedAdapter.Name;
			this.IsMISVisible = false;
			this.IsHT360Visible = false;
			this.IsMaskShow = false;
			
			ExportCommandInfo infoExportDefault = UtilsExport.GetExportPdaCommandInfoDefaultData(this._adapterName, this._userSettingsManager);
			//Customer customer = base.CurrentCustomer;
			if (customer != null)
			{
				ExportCommandInfo infoCustomer =
					UtilsExport.GetExportPdaCustomerData(this._customerConfigRepository, customer, infoExportDefault, this._adapterName);
				infoExportDefault = infoCustomer;
			}

			//================ ExportHT630Adapter =============================
			//================ ExportPdaMISAdapter =============================
			if (this._adapterName == ExportPdaAdapterName.ExportHT630Adapter
				|| this._adapterName == ExportPdaAdapterName.ExportPdaMISAdapter)
			{
				this.IsExportSettingsVisibility = true;
				this.IsProgramTypeVisibility = true;
				this.IsExportPdaVisibility = false;
				this.IsMaskShow = true;

				 //Вынесла для того чтобы без GUI можно было работать
				//this._exportPdaSettingsControlViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsControlViewModel>(
				//	Common.RegionNames.ExportPdaSettings, this._regionManager);

				if (this._exportPdaSettingsControlViewModel != null)
				{
			
						//UtilsExport.GetExportPdaMISDefaultData(this._userSettingsManager);
					//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
					this._exportPdaSettingsControlViewModel.FillGUIAdapterData(selectedAdapter, infoExportDefault);
					//заполняет из кастомера (? и по умолчанию)
					//this._exportPdaSettingsControlViewModel.FillAdapterCustomerData(base.CurrentCustomer, this.SelectedAdapter.Name);

					//UtilsExport.FillExportPdaSettingsControl(this._exportPdaSettingsControlViewModel, base.CurrentCustomer, this._customerConfigRepository, SelectedAdapter);

					this._exportPdaSettingsControlViewModel.PropertyChanged += ExportControlViewModel_PropertyChanged;
				}

				//Вынесла для того чтобы без GUI можно было работать
				//this._exportPdaProgramTypeControlViewModel = Utils.GetViewModelFromRegion<ExportPdaProgramTypeViewModel>(
				//	Common.RegionNames.ExportPdaProgramType, this._regionManager);

				if (_exportPdaProgramTypeControlViewModel != null)
				{
					//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
					this._exportPdaProgramTypeControlViewModel.FillGUIAdapterData(selectedAdapter, infoExportDefault);
					//заполняет из кастомера (? И по умолчанию)			   //здесь оставили пока как было
					this._exportPdaProgramTypeControlViewModel.FillAdapterCustomerData(customer);
				}
				//================ ExportHT630Adapter =============================
				if (this._adapterName == ExportPdaAdapterName.ExportHT630Adapter)
				{
					this.IsMISVisible = false;
					this.IsHT360Visible = true; 
					if (this._exportPdaProgramTypeControlViewModel != null)
					{
						this._exportPdaProgramTypeControlViewModel.IsEditable = true;

						this._exportPdaProgramTypeControlViewModel.IsMISVisible = this.IsMISVisible;
						this._exportPdaProgramTypeControlViewModel.IsHT360Visible = this.IsHT360Visible;
					}

					if (this._exportPdaSettingsControlViewModel != null)
					{
						this._exportPdaSettingsControlViewModel.IsMISVisible = this.IsMISVisible;
						this._exportPdaSettingsControlViewModel.IsHT360Visible = this.IsHT360Visible;
					}
				}
				//================ ExportPdaMISAdapter =============================
				else if (this._adapterName == ExportPdaAdapterName.ExportPdaMISAdapter)
				{
					this.IsMISVisible = true;
					this.IsHT360Visible = false; 

					if (this._exportPdaProgramTypeControlViewModel != null)
					{
						this._exportPdaProgramTypeControlViewModel.IsEditable = true;

						this._exportPdaProgramTypeControlViewModel.IsMISVisible = this.IsMISVisible;
						this._exportPdaProgramTypeControlViewModel.IsHT360Visible = this.IsHT360Visible;
					}
					if (this._exportPdaSettingsControlViewModel != null)
					{
						this._exportPdaSettingsControlViewModel.IsMISVisible = this.IsMISVisible;
						this._exportPdaSettingsControlViewModel.IsHT360Visible = this.IsHT360Visible;
					}
				}
			}
			//================ ExportPdaAdapter  - SQLite =============================
			else if (this._adapterName == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
				|| this._adapterName == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
				|| this._adapterName == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
				|| this._adapterName == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
				|| this._adapterName == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
			{	  // визуализация регионов 
				this.IsExportSettingsVisibility = false;
				this.IsProgramTypeVisibility = false;
				this.IsExportPdaVisibility = true;
				this.IsMaskShow = false;

				//Вынесла для того чтобы без GUI можно было работать
				//this._exportPdaMerkavaAdapterControlViewModel = Utils.GetViewModelFromRegion<ExportPdaMerkavaAdapterViewModel>(Common.RegionNames.ExportPdaAdapter, this._regionManager);


				if (this._exportPdaMerkavaAdapterControlViewModel != null)
				{
					//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
					this._exportPdaMerkavaAdapterControlViewModel.FillGUIAdapterData(selectedAdapter, infoExportDefault);

					//заполняет из кастомера (? и по умолчанию)
					//this._exportPdaMerkavaAdapterControlViewModel.FillAdapterCustomerData(base.CurrentCustomer);
				}
			}
		}

		protected  void OnSelectedAdapterChanged()
		{

		}

        public DelegateCommand SetAsDefaultCommand
        {
            get { return _setAsDefaultCommand; }
        }

        public bool IsWithoutProductName
        {
            get { return _isWithoutProductName; }
            set
            {
                _isWithoutProductName = value;
                RaisePropertyChanged(() => IsWithoutProductName);
            }
        }

        public bool IsBarcodeWithoutMask
        {
            get { return _isBarcodeWithoutMask; }
            set
            {
                _isBarcodeWithoutMask = value;
                RaisePropertyChanged(() => IsBarcodeWithoutMask);
            }
        }

        public bool IsMakatWithoutMask
        {
            get { return _isMakatWithoutMask; }
            set
            {
                _isMakatWithoutMask = value;
                RaisePropertyChanged(() => IsMakatWithoutMask);
            }
        }

		public bool IsCheckBaudratePDA
        {
			get { return _isCheckBaudratePDA; }
            set
            {
				_isCheckBaudratePDA = value;
				RaisePropertyChanged(() => IsCheckBaudratePDA);
            }
        }

		public bool IsExportSettingsVisibility
		{
			get { return _exportSettingsVisibility; }
			set
			{
				_exportSettingsVisibility = value;

				RaisePropertyChanged(() => IsExportSettingsVisibility);
				if (_exportSettingsVisibility == true)
				{

				}
			}
		}

	

		public bool IsProgramTypeVisibility
		{
			get { return _programTypeVisibility; }
			set
			{
				_programTypeVisibility = value;

				RaisePropertyChanged(() => IsProgramTypeVisibility);
				if (_programTypeVisibility == true)
				{

				}
			}
		}

		public bool IsExportPdaVisibility
		{
			get { return _exportPdaVisibility; }
			set
			{
				_exportPdaVisibility = value;

				RaisePropertyChanged(() => IsExportPdaVisibility);
				if (_exportPdaVisibility == true)
				{

				}
			}
		}

		public string AdapterName
		{
			get { return _adapterName; }
		}

        public DelegateCommand NavigateToGridCommand
        {
            get { return _navigateToGridCommand; }
        }

        public DelegateCommand UploadToPDACommand
        {
            get { return this._uploadToPDACommand; }
        }

		public DelegateCommand DownloadFromPDACommand
		{
			get { return this._downloadFromPDACommand; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			 //для процесса экпорта и отправки в оффис
			//this._autoClose = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoCloseExportPdaWindow);
			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
			{
				this._adapterName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
			}

            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

			this._isFromConfigEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;

			this.Adapters =
				new ObservableCollection<IExportPdaModuleInfo>(Utils.GetExportPdaAdapters(this._container, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode));

			this._exportPdaSettingsControlViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsControlViewModel>(Common.RegionNames.ExportPdaSettingsInner, _regionManager);
		
			this._exportPdaProgramTypeControlViewModel = Utils.GetViewModelFromRegion<ExportPdaProgramTypeViewModel>(Common.RegionNames.ExportPdaProgramType, _regionManager);

			this._exportPdaMerkavaAdapterControlViewModel = Utils.GetViewModelFromRegion<ExportPdaMerkavaAdapterViewModel>(
				Common.RegionNames.ExportPdaAdapter, this._regionManager);

			IExportPdaModuleInfo defaultAdapter = Adapters.FirstOrDefault(r => r.IsDefault);

			string defaultAdapterCodeByCBI = GetDefaultAdapterName();
			if (!String.IsNullOrWhiteSpace(defaultAdapterCodeByCBI))
			{
				if (this._adapters.Any(r => r.Name == defaultAdapterCodeByCBI))
					defaultAdapter = _adapters.FirstOrDefault(r => r.Name == defaultAdapterCodeByCBI);
			}

			SelectedAdapter = defaultAdapter;

			this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
			RaisePropertyChanged(() => this.ContextCustomerAdapterName);



		
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

		//Add
		protected void UploadToPDACommandExecuted()
		{
			var settings = new Dictionary<string, string>();
			Utils.AddContextToDictionary(settings, base.Context);
			Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
			if (this._selectedAdapter.Name == ExportPdaAdapterName.ExportHT630Adapter)
			{
				settings.Add(NavigationSettings.CheckBoundrate, IsCheckBaudratePDA.ToString());

				using (new CursorWait())
				{
					object result = _modalWindowLauncher.StartModalWindow(
							   Common.ViewNames.UploadToPdaView,
							   WindowTitles.UploadPda,
							   650, 450,
							   ResizeMode.CanResize, settings,
							   null,
							   650, 450);
				}
			}
			else if (this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
			|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
			|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
			|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
			|| this._selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
			{
				object result = _modalWindowLauncher.StartModalWindow(
					 Common.ViewNames.ToFtpView,
					 WindowTitles.ToFtp,
					 800, 600,
					 ResizeMode.CanResize, settings,
					 null,
					 650, 450);
			}
		}
       

		//TODO
		protected void DownloadFromPDACommandExecuted()
        {
            var settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
			settings.Add(NavigationSettings.CheckBoundrate, IsCheckBaudratePDA.ToString());

            using (new CursorWait())
            {
                object result = _modalWindowLauncher.StartModalWindow(
                           Common.ViewNames.DownloadFromPDAView,
                           WindowTitles.DownloadPda,
                           650, 450,
                           ResizeMode.CanResize, settings,
                           null,
                           650, 450);
            }
        }

        void ExportControlViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxCharacters" || e.PropertyName == "FileTypeSelected")
                this._exportCommand.RaiseCanExecuteChanged();
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

            if (this._exportPdaSettingsControlViewModel != null && !this._exportPdaSettingsControlViewModel.IsFormValid())
                return false;

            return true;
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

		#region  fromConfig
		// ============= Config

		//передаются данные в export из формы exporta
		// НО формы теперь разные и разные данные 
		// объект  info - оставим один на все адаптеры
		protected override void ExportCommandExecuted()
		{
			ExportWithInitMode(this._fromGUI, this._fromConfig);
		}

		private void ExportWithInitMode(
			bool initFromGUI = true, 
			bool orInitFromConfig = false)
	    {
			if (initFromGUI == true)
			{
				if (this._dynViewModel != null)
				{
					if (this._dynViewModel.CanExport())
					{
						
						ExportCommandInfo info = new ExportCommandInfo();
						info.Callback = ShowLog;
						info.IsSaveFileLog = _isWriteLogToFile;
						this._cts = new CancellationTokenSource();
						info.CancellationToken = this._cts.Token;
						info = this.FillExportPdaInfo(info);
						info.AdapterName = "";
						info.ConfigXDocument = null;
						info.ConfigXDocumentPath = "";
						info.FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;

						this._dynViewModel.RunExportPdaBase(info);
					}
				}
			}
			else if (orInitFromConfig == true)		//закрыла доступ из 	GUI
			{
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				IExportPdaModule viewModel = this._dynViewModel;	 //  Это с GUI	 для тестирования должны быть равны 
				this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
				//IExportPdaModule viewModel = Get_ExportPdaModuleBaseViewModel_AsIExportPdaModule(this.ContextCustomerAdapterName);
				ExportFromConfigMode(viewModel, ContextCustomerAdapterName, fromConfigXDoc, base.State, null);
			}
		}

		
		//Для вызова из-вне. 
		public void ExportPdaByConfig(
			IExportPdaModule viewModel,
			CBIState state,
			string adapterName, 
			ConfigXDocFromEnum fromConfigXDoc,
			string comment,  //? это откуда запускаем, предположительно для process
			ImportModuleBaseViewModel complexViewModel)
		{
			//this.SetSelectedAdapterWithoutGUI(comment, state);
			this.ExportFromConfigMode(viewModel, adapterName, fromConfigXDoc, state, null);
		}

		public void ClearExportPdaByConfig(
			IExportPdaModule viewModel,
			CBIState state,
			string adapterName,
			ConfigXDocFromEnum fromConfigXDoc )
		{
			ClearFromConfigMode(viewModel, adapterName, fromConfigXDoc, state);
		}

		private void ExportFromConfigMode(
			IExportPdaModule viewModel,
			string adapterName, 
			ConfigXDocFromEnum fromConfigXDoc, 
			CBIState state,
			ExportCommandInfo infoParam)
		{
			if (viewModel != null)
			{
				ExportCommandInfo info = new ExportCommandInfo();
				info.Callback = ShowLog;
				info.IsSaveFileLog = _isWriteLogToFile;
				this._cts = new CancellationTokenSource();
				info.CancellationToken = this._cts.Token;
				//info = this.FillExportPdaInfo(info);
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Customer = state.CurrentCustomer;
				if (infoParam != null)
				{
					// f. e. info.FromConfigXDoc = infoParam.FromConfigXDoc;
				}
	
				viewModel.RunExportPdaWithoutGUIBase(info, state);
			}
		}

		private void ClearFromConfigMode(
			IExportPdaModule viewModel,
			string adapterName,
			ConfigXDocFromEnum fromConfigXDoc,
			CBIState state)
		{
			if (viewModel != null)
			{
				ExportCommandInfo info = new ExportCommandInfo();
				info.Callback = ShowLog;
 				info.IsSaveFileLog = _isWriteLogToFile;
				this._cts = new CancellationTokenSource();
				info.CancellationToken = this._cts.Token;
 				//info = this.FillExportPdaInfo(info);
  				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Customer = state.CurrentCustomer;

				viewModel.RunExportPdaClearWithoutGUIBase(info, state);
			}
		}

		//public IExportPdaModule Get_ExportPdaModuleBaseViewModel_AsIExportPdaModule(string adapterName)
		//{
		//	ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel = null;

		//	try
		//	{
		//		exportPdaModuleBaseViewModel = this.ServiceLocator.GetInstance<ExportPdaModuleBaseViewModel>(adapterName);
		//	}
		//	catch { }

		//	if (exportPdaModuleBaseViewModel == null)
		//	{
		//		return null;
		//	}

		//	//exportPdaModuleBaseViewModel.UpdateBusyText = UpdateBusyText;
		//	//exportPdaModuleBaseViewModel.SetIsCancelOk = SetIsCancelOk;

		//	IExportPdaModule viewModel = exportPdaModuleBaseViewModel as IExportPdaModule;
		//	return viewModel;
		//}

		public void SaveConfigByDefaultForCustomer(Customer customer,
			IExportPdaModuleInfo selectedAdapter,
			ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel,
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
			//string dataContextViewModelName = exportPdaModuleBaseViewModel.GetType().Name;
			//XElement root = ViewModelConfigRepository.GetXElementExportPdaAdapterProperty(exportPdaModuleBaseViewModel,
			//	dataContextViewModelName,
			//	selectedAdapter, info);

			this._exportPdaSettingsControlViewModel = this.ServiceLocator.GetInstance<ExportPdaSettingsControlViewModel>();
			this._exportPdaProgramTypeControlViewModel = this.ServiceLocator.GetInstance<ExportPdaProgramTypeViewModel>();
			this._exportPdaMerkavaAdapterControlViewModel = this.ServiceLocator.GetInstance<ExportPdaMerkavaAdapterViewModel>();


			if (this._exportPdaSettingsControlViewModel != null)
			{
				this._exportPdaSettingsControlViewModel.BuildEncoding();
				//this._exportPdaSettingsControlViewModel.FillAdapterCustomerData(customer, selectedAdapter.Name);
			}
			if (this._exportPdaProgramTypeControlViewModel != null)
			{
				this._exportPdaProgramTypeControlViewModel.Build();
			//	this._exportPdaProgramTypeControlViewModel.FillAdapterCustomerData(customer);
			}
			//if (this._exportPdaMerkavaAdapterControlViewModel != null)
			//{
			//	this._exportPdaMerkavaAdapterControlViewModel.Build();
			//	this._exportPdaMerkavaAdapterControlViewModel.FillAdapterCustomerData(customer);
			//}

			this.FillSelectExportPdaAdapter(selectedAdapter, customer);

			string dataContextViewModelName = exportPdaModuleBaseViewModel.GetType().Name;
			XElement root = ViewModelConfigRepository.GetXElementExportPdaAdapterProperty(
				exportPdaModuleBaseViewModel,
				dataContextViewModelName,
				selectedAdapter,
				this.ServiceLocator,
				this._exportPdaSettingsControlViewModel, this._exportPdaProgramTypeControlViewModel, this._exportPdaMerkavaAdapterControlViewModel);

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
					 "ExportPdaWithModulesViewModel", fromDomainObjectType, pathHowUse, /*DomainObjectType.inventor, HowUse.relative, */ "");
				configXDocument.Save(fileConfig);
			}
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

			IRegion region = this._regionManager.Regions[Common.RegionNames.ExportByModule];
			UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
			if (userControl != null)
			{
				if (userControl.DataContext != null)
				{

					string dataContextViewModelName = userControl.DataContext.GetType().Name;
					XElement root = ViewModelConfigRepository.GetXElementExportPdaAdapterProperty(
						userControl.DataContext,
						dataContextViewModelName,
						SelectedAdapter,
						ServiceLocator,
						_exportPdaSettingsControlViewModel, _exportPdaProgramTypeControlViewModel, _exportPdaMerkavaAdapterControlViewModel);
					doc.Add(root);
					xmlString = doc.ToString();
				}

			}

			string _configXML = "";
			_configXML = _configXML + Environment.NewLine + xmlString;

			ExportLogViewData data = new ExportLogViewData();
			data.ConfigXML = _configXML;
			//if (this.DynViewModel != null)
			//data.Path = @"C:\MIS";//this.DynViewModel.BuildPathToExportErpDataFolder();
			data.InDataPath = Path.Combine(_dynViewModel.GetExportToPDAFolderPath(true));
			data.DataInConfigPath = this._contextCBIRepository.GetConfigFolderPath(base.State.CurrentCustomer);
			//data.DataInConfigPath = this.GetConfigFolderPath(base.State.CurrentCustomer);
			//data.InDataPath = base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
			data.AdapterType = "ExportPdaWithModulesViewModel";
			data.AdapterName = "";
			if (this.SelectedAdapter != null)
			{
				data.AdapterName = this.SelectedAdapter.Name;
			}
			UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, data);
			OnModalWindowRequest(payload);
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
		#endregion fromConfig
		// ============= End Config


		private string GetContextCustomerAdaperName()
		{
			string contextAdapterName = "";
			//if (this.SelectedAdapter != null)
			//{
			//	contextAdapterName = SelectedAdapter.Name;
			//}
			//if (this.FromCustomer == true)
			//{
			contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextCustomer.ToString());
			//}
			//else if (this.FromBranch == true)
			//{
			//	contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextBranch.ToString());
			//}
			//else if (this.FromInventor == true)
			//{
			//	contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextInventor.ToString());
			//}
			if (string.IsNullOrWhiteSpace(contextAdapterName) == true)
				contextAdapterName = WarningAdapter.AdapterInCustomerNotSet; 
			return contextAdapterName;
		}

		//заполняем данные для ехпорта
		private ExportCommandInfo FillExportPdaInfo(ExportCommandInfo info)
		{
			if (info == null) return info;
			//оновная часть формы Export PDA Adapter
			info.WithoutProductName = this.IsWithoutProductName;
			info.BarcodeWithoutMask = this.IsBarcodeWithoutMask;
			info.MakatWithoutMask = this.IsMakatWithoutMask;
			info.CheckBaudratePDA = this.IsCheckBaudratePDA;
			//exportPdaSettingsControl часть формы Export PDA Adapter
			if (this._exportPdaSettingsControlViewModel != null)
			{
				if (this.IsExportSettingsVisibility == true)
				{
					info = this._exportPdaSettingsControlViewModel.FillExportSettingsControlInfo(info);
				}
			}

			//_exportPdaProgram часть формы Export PDA Adapter
			if (this._exportPdaProgramTypeControlViewModel != null)
			{
				if (this.IsProgramTypeVisibility == true)
				{
				info = this._exportPdaProgramTypeControlViewModel.FillExportProgramTypeInfo(info);
				}
			}

			if (this._exportPdaMerkavaAdapterControlViewModel != null)
			{
				if (this.IsExportPdaVisibility == true)
				{
					info = this._exportPdaMerkavaAdapterControlViewModel.FillExportInfo(info);
				}
			}

			if (base.CurrentCustomer != null)
			{
				info.Customer = base.CurrentCustomer;
			}

			return info;
		}

	

        private bool SetAsDefaultCommandCanExecute()
        {
			return _selectedAdapter != null && _selectedAdapter.Name != this.GetDefaultAdapterName();
        }

		private string GetSelectedAdapterNameByCBI(string cbidbContext)
		{
			string exportCatalogCustomerAdapterCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportCatalogAdapterCode;
			string exportBranchCatalogAdapterCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportCatalogAdapterCode;
			string exportInventorCatalogAdapterCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportCatalogAdapterCode;
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
			return "";
		}

		private string GetDefaultAdapterName()
		{
			string exportCatalogCustomerAdapterCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ExportCatalogAdapterCode;
			string exportBranchCatalogAdapterCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ExportCatalogAdapterCode;
			string exportInventorCatalogAdapterCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ExportCatalogAdapterCode;
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

   
		private void SetAsDefaultCommandExecuted()
		{
			string adapterName = "";
			if (SelectedAdapter != null)	adapterName = SelectedAdapter.Name;
			switch (base.CBIDbContext)
			{
				case NavigationSettings.CBIDbContextCustomer:
					base.CurrentCustomer.ExportCatalogAdapterCode = adapterName;
					this._customerRepository.Update(base.CurrentCustomer);
					break;
				case NavigationSettings.CBIDbContextBranch:
					base.CurrentBranch.ExportCatalogAdapterCode = adapterName;
					this._branchRepository.Update(base.CurrentBranch);
					break;
				case NavigationSettings.CBIDbContextInventor:
					base.CurrentInventor.ExportCatalogAdapterCode = adapterName;	   // ops 
					this._inventorRepository.Update(base.CurrentInventor);
					break;
			}

			_setAsDefaultCommand.RaiseCanExecuteChanged();
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
            payload.WindowTitle = WindowTitles.ViewExportLog;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);

            ExportLogViewData data = new ExportLogViewData();
            data.Log = _log;
            data.Path = Path.Combine(_dynViewModel.GetExportToPDAFolderPath(false), "Process");

            UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, data);
            OnModalWindowRequest(payload);
        }

		protected override bool ConfigCommandCanExecute()
		{
			if (this.State == null) return false;
			if (this.CurrentInventor == null) return false;
			string contextCustomerAdapterName = this.GetContextCustomerAdaperName();
			if (string.IsNullOrWhiteSpace(contextCustomerAdapterName) == true) return false;

			if (this._fromConfig == true)
			{
				string selectedAdapterName = SelectedAdapter.Name;
				if (selectedAdapterName.ToLower() != contextCustomerAdapterName.ToLower()) return false;
			}
			return true;
		}

	

        protected override void ClearCommandExecuted()
        {
            if (this._dynViewModel == null) return;


            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
            notification.Content = Localization.Resources.Msg_Clear_Export_PDA;
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
						//this._dynViewModel.RunClear();
                    }
                });
        }

        private void NavigateToGridCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            UtilsNavigate.CatalogOpen(_regionManager, query);
        }

		private void CloseModalWindow()
		{
			_eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
		}
    }
}