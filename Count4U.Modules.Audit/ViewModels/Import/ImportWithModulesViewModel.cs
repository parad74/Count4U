using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Constants.AdapterNames;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.Misc;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.Model;
using NLog;
using Count4U.Common.Extensions;
using System.Xml.Linq;
using Count4U.Localization;
using Count4U.Modules.ContextCBI.Xml.Config;
using Count4U.Common.ViewModel;
using Count4U.Model.Main;

namespace Count4U.Modules.Audit.ViewModels.Import
{
	public class ImportWithModulesViewModel : ImportWithModulesBaseViewModel
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly ICustomerRepository _customerRepository;
		private readonly IBranchRepository _branchRepository;
		private readonly IInventorRepository _inventorRepository;
		private readonly IImportAdapterRepository _importAdapterRepository;
		protected readonly IServiceLocator _serviceLocator;
		protected readonly ILog _logImport;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IProductRepository _productRepository;
		private readonly ISupplierRepository _supplierRepository;

		private readonly DelegateCommand _setAsDefaultCommand;
		private readonly DelegateCommand _navigateToGridCommand;
		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _saveConfigCommand;
		private readonly DelegateCommand _saveConfigToCustomerCommand;
		private readonly DelegateCommand _importByConfigCommand;

		private bool _fromAdapter;
		private bool _fromCustomer;
		private bool _fromBranch;
		private bool _fromInventor;


		private bool _fromFile;
		private bool _fromInternal;
		private bool _fromConfig;

		private string _configXML;
		private XDocument _configXDocument;

		//private ImportDomainEnum _mode;
		private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;

		private bool _isDefaultAdapterFromCustomer;
		private bool _isDefaultAdapterFromBranch;
		private bool _isDefaultAdapterFromInventor;

		private bool _isFromInternalEnabled;
		private bool _isFromConfigEnabled;

		private int _stepTotal;
		private int _stepCurrent;
		private readonly Action<long> _updateProgress;
		private CancellationToken _cancellationToken;

		private readonly ObservableCollection<EncodingItemViewModel> _encodingItems;
		private EncodingItemViewModel _encodingSelectedItem;

		private bool _isInvertLetters;
		private bool _isInvertWords;

		private bool _isClearButtonVisible;

		private string _title;
		private string _startButtonTitle;
		private string _fromTitle;
		private string _adapterTypeTitle;

		private bool _isEncodingVisible;

		private bool _tryFast;
		private bool _tryFastVisible;

		private bool _isInvertCheckBoxVisible;
		private bool _isRadioFileVisible;

		private string _dataInConfigPath = String.Empty;


		#region ctor
		public ImportWithModulesViewModel(
			IContextCBIRepository contextCbiRepository,
			IServiceLocator serviceLocator,
			IEventAggregator eventAggregator,
			IRegionManager regionManager,
			IUnityContainer container,
			ICustomerRepository customerRepository,
			IBranchRepository branchRepository,
			IInventorRepository inventorRepository,
			IImportAdapterRepository importAdapterRepository,
			ILog logImport,
			INavigationRepository navigationRepository,
			IUserSettingsManager userSettingsManager,
			IProductRepository productRepository,
			UICommandRepository commandRepository,
			ISupplierRepository supplierRepository,
			DBSettings dbSettings)
			: base(contextCbiRepository, eventAggregator, regionManager, container, navigationRepository, commandRepository, dbSettings)
		{
			_supplierRepository = supplierRepository;
			this._productRepository = productRepository;
			this._userSettingsManager = userSettingsManager;
			this._inventorRepository = inventorRepository;
			this._branchRepository = branchRepository;
			this._customerRepository = customerRepository;
			this._importAdapterRepository = importAdapterRepository;
			this._serviceLocator = serviceLocator;
			this._logImport = logImport;

			base.ImportCommand = base._commandRepository.Build(enUICommand.Import, this.ImportCommandExecuted, this.ImportCommandCanExecute);
			base.NavigateToGridCommand = base._commandRepository.Build(enUICommand.FromImportToGrid, this.NavigateToGridCommandExecuted, base.NavigateToGridCommandCanExecute);
			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			this._saveConfigToCustomerCommand = new DelegateCommand(SaveConfigToCustomerCommandExecuted, SaveConfigToCustomerCommandCanExecute);
			this._importByConfigCommand = new DelegateCommand(ImportByConfigCommandExecuted, ImportByConfigCommandCanExecute);

			this._fromAdapter = false;
			this._fromCustomer = true;
			this._fromBranch = false;
			this._fromInventor = false;
			this._fromFile = true;
			this._fromInternal = false;
			this._fromConfig = false;

			this._isRadioFileVisible = true;


			this._setAsDefaultCommand = new DelegateCommand(this.SetAsDefaultCommandExecuted, this.SetAsDefaultCommandCanExecute);

			this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();

			this._updateProgress = r =>
									   {
										   this.Progress = r.ToString();
										   this.ProgressStep = String.Format(Localization.Resources.ViewModel_ImportWithModulesBase_Step, _stepCurrent, _stepTotal);
									   };
			this._encodingItems = new ObservableCollection<EncodingItemViewModel>();
			this.BuildEncoding();

			this._tryFast = true;
		}

		#endregion

		#region properties

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
				ImportByConfigCommand.RaiseCanExecuteChanged();
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

			string adapterName = this.GetContextCustomerAdaperName();
			if (selectedAdapterName.ToLower() != adapterName.ToLower())
			{
				_errorAdapterName = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; // "Selected Adapter not equals Adapter for Customer";
				return false;
			}

			if (adapterName == WarningAdapter.AdapterInCustomerNotSet) return false;
			string adapterConfigFileName = @"\" + adapterName + ".config";

			if (File.Exists(this.DataInConfigPath + adapterConfigFileName) == true) return true;

			return false;
		}

		//Для вызова из-вне. 
		public void ImportByConfig(ImportModuleBaseViewModel viewModel,
			CBIState state,
			string adapterName, 
			ImportDomainEnum mode, 
			ConfigXDocFromEnum fromConfigXDoc,
			string comment,
			ImportModuleBaseViewModel complexViewModel) //, string dataInConfigPath)
		{
			//viewModel = userControl.DataContext as ImportModuleBaseViewModel;
			//ImportDomainEnum mode = base.Mode;
			//ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
			//string adapterName = this.GetAdaperName();
			//base.SetSelectedAdapterWithoutGUI(comment/*complexAdapter, complexViewModel*/);
			ImportFromConfigMode(viewModel, adapterName, mode, fromConfigXDoc, state, null);
		}

		public void ClearImportByConfig(ImportModuleBaseViewModel viewModel,
		CBIState state,
		string adapterName,
		ImportDomainEnum mode,
		ConfigXDocFromEnum fromConfigXDoc) //, string dataInConfigPath)
		{
			ClearFromConfigMode(viewModel, adapterName, mode, fromConfigXDoc, state);
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
			get
			{
				if (this.SelectedAdapter == null) return String.Empty;
				return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for " + this.SelectedAdapter.Title; 
			}
		}

		public string SaveConfigForCustomer
		{
			get { return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for Customer"; }
		}



		//not use now?
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
			if (base.SelectedAdapter == null) return false;
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
			else { return; }

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
			//if (base.SelectedAdapter == null) return false;
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
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
			}
		
		}

		public bool FromFile
		{
			get { return this._fromFile; }
			set
			{
				this._fromFile = value;
				this.RaisePropertyChanged(() => this.FromFile);

				if (value == true)
				{
					this._fromInternal = false;
					this._fromConfig = false;
				}


				this._setAsDefaultCommand.RaiseCanExecuteChanged();

				base.ImportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);

				base.NavigateToGridCommand.RaiseCanExecuteChanged();
			
			}
		}

		public bool FromInternal
		{
			get { return this._fromInternal; }
			set
			{
				this._fromInternal = value;

				this.RaisePropertyChanged(() => this.FromInternal);

				if (this._fromInternal == true)
				{
					this._fromFile = false;
					this._fromConfig = false;
				}

				this._setAsDefaultCommand.RaiseCanExecuteChanged();

				base.ImportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				base.NavigateToGridCommand.RaiseCanExecuteChanged();
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
					this._fromFile = false;
					this._fromInternal = false;
					this.LoadXMLConfig();
				}

				//  this._setAsDefaultCommand.RaiseCanExecuteChanged();

				base.ImportCommand.RaiseCanExecuteChanged();
				base.ClearCommand.RaiseCanExecuteChanged();
				base.ConfigCommand.RaiseCanExecuteChanged();
				this.ImportByConfigCommand.RaiseCanExecuteChanged();
				this.RaisePropertyChanged(() => this.ErrorAdapterName);
				base.NavigateToGridCommand.RaiseCanExecuteChanged();
			}
		}

		public DelegateCommand SetAsDefaultCommand
		{
			get { return this._setAsDefaultCommand; }
		}

		public bool IsDefaultAdapterFromCustomer
		{
			get { return this._isDefaultAdapterFromCustomer; }
			set
			{
				this._isDefaultAdapterFromCustomer = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromCustomer);

				if (this._isDefaultAdapterFromCustomer)
				{
					this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextCustomer);
				}
			}
		}

		public bool IsDefaultAdapterFromBranch
		{
			get { return this._isDefaultAdapterFromBranch; }
			set
			{
				this._isDefaultAdapterFromBranch = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromBranch);

				if (this._isDefaultAdapterFromBranch)
				{
					this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextBranch);
				}
			}
		}

		public bool IsDefaultAdapterFromInventor
		{
			get { return this._isDefaultAdapterFromInventor; }
			set
			{
				this._isDefaultAdapterFromInventor = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromInventor);

				if (this._isDefaultAdapterFromInventor)
				{
					this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextInventor);
				}
			}
		}

		public InteractionRequest<MessageBoxNotification> MessageBoxRequest
		{
			get { return this._messageBoxRequest; }
		}

		public bool IsFromInternalEnabled
		{
			get { return this._isFromInternalEnabled; }
			set
			{
				this._isFromInternalEnabled = value;
				RaisePropertyChanged(() => IsFromInternalEnabled);
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

		public ObservableCollection<EncodingItemViewModel> EncodingItems
		{
			get { return _encodingItems; }
		}

		public EncodingItemViewModel EncodingSelectedItem
		{
			get { return _encodingSelectedItem; }
			set
			{
				_encodingSelectedItem = value;
				RaisePropertyChanged(() => EncodingSelectedItem);

				if (base.DynViewModel != null)
					base.DynViewModel.Encoding = this._encodingSelectedItem.Encoding;
			}
		}

		public bool IsCatalogOrSectionOrBranchMode
		{
			get { return this.Mode == ImportDomainEnum.ImportCatalog || this.Mode == ImportDomainEnum.ImportSection || this.Mode == ImportDomainEnum.ImportBranch; }
		}

		public bool IsInvertLetters
		{
			get { return _isInvertLetters; }
			set
			{
				_isInvertLetters = value;
				RaisePropertyChanged(() => IsInvertLetters);

				if (base.DynViewModel != null)
					base.DynViewModel.IsInvertLetters = this._isInvertLetters;
			}
		}

		public bool IsInvertWords
		{
			get { return _isInvertWords; }
			set
			{
				_isInvertWords = value;
				RaisePropertyChanged(() => IsInvertWords);

				if (base.DynViewModel != null)
					base.DynViewModel.IsInvertWords = this._isInvertWords;
			}
		}

		public bool IsClearButtonVisible
		{
			get { return _isClearButtonVisible; }
			set
			{
				_isClearButtonVisible = value;
				RaisePropertyChanged(() => IsClearButtonVisible);
			}
		}

		public string StartButtonTitle
		{
			get { return _startButtonTitle; }
			set
			{
				_startButtonTitle = value;
				RaisePropertyChanged(() => StartButtonTitle);
			}
		}

		public string FromTitle
		{
			get { return _fromTitle; }
			set
			{
				_fromTitle = value;
				RaisePropertyChanged(() => FromTitle);
			}
		}

		public bool IsEncodingVisible
		{
			get { return _isEncodingVisible; }
			set
			{
				_isEncodingVisible = value;
				RaisePropertyChanged(() => IsEncodingVisible);
			}
		}

		public string AdapterTypeTitle
		{
			get { return _adapterTypeTitle; }
			set
			{
				_adapterTypeTitle = value;
				RaisePropertyChanged(() => AdapterTypeTitle);
			}
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				RaisePropertyChanged(() => Title);
			}
		}

		//public DelegateCommand NavigateToGridCommand
		//{
		//	get { return _navigateToGridCommand; }
		//}

		//public ImportDomainEnum Mode
		//{
		//	get { return _mode; }
		//}


		public bool TryFast
		{
			get { return _tryFast; }
			set
			{
				_tryFast = value;
				RaisePropertyChanged(() => TryFast);
			}
		}

		public bool TryFastVisible
		{
			get { return _tryFastVisible; }
			set
			{
				_tryFastVisible = value;
				RaisePropertyChanged(() => TryFastVisible);
			}
		}

		public bool IsInvertCheckBoxVisible
		{
			get { return _isInvertCheckBoxVisible; }
			set
			{
				_isInvertCheckBoxVisible = value;
				RaisePropertyChanged(() => IsInvertCheckBoxVisible);
			}
		}

		public bool IsRadioFileVisible
		{
			get { return _isRadioFileVisible; }
			set
			{
				_isRadioFileVisible = value;
				RaisePropertyChanged(() => IsRadioFileVisible);
			}
		}


		#endregion

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ImportMode))
			{
				string mode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.ImportMode).Value;
				switch (mode)
				{
					case Common.NavigationSettings.ImportModeCatalog:
						{
							this._mode = ImportDomainEnum.ImportCatalog;

							Utils.MainWindowTitleSet(WindowTitles.ImportCatalog, base.EventAggregator);
							break;
						}
					case Common.NavigationSettings.ImportModeLocation:
						{
							this._mode = ImportDomainEnum.ImportLocation;

							Utils.MainWindowTitleSet(WindowTitles.ImportLocations, base.EventAggregator);
							break;
						}
					case Common.NavigationSettings.ImportModeItur:
						{
							this._mode = ImportDomainEnum.ImportItur;

							Utils.MainWindowTitleSet(WindowTitles.ImportIturs, base.EventAggregator);
							break;
						}

					case Common.NavigationSettings.ImportModeSection:
						{
							this._mode = ImportDomainEnum.ImportSection;

							Utils.MainWindowTitleSet(WindowTitles.ImportIturs, base.EventAggregator);
							break;
						}
					case Common.NavigationSettings.ImportModeUpdateCatalog:
						{
							this._mode = ImportDomainEnum.UpdateCatalog;
							break;
						}
					//case Common.NavigationSettings.ComplexModeObject:
					//	{
					//		this._mode = ImportDomainEnum.ComplexOperation;
					//		break;
					//	}

					case Common.NavigationSettings.ImportModeBranch:
						{
							this._mode = ImportDomainEnum.ImportBranch;
							break;
						}
					case Common.NavigationSettings.ImportModeSupplier:
						{
							this._mode = ImportDomainEnum.ImportSupplier;
							break;
						}
					case Common.NavigationSettings.ImportModeFamily:
						{
							this._mode = ImportDomainEnum.ImportFamily;
							break;
						}
					case Common.NavigationSettings.ImportModeUnitPlan:
						{
							this._mode = ImportDomainEnum.ImportUnitPlan;
							break;
						}
				}
			}

			string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
			string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
			string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

			base.Adapters = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(base.Container, this._importAdapterRepository, this.Mode,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			string currentAdapterNameByCBI = this.GetSelectedAdapterNameByCBI();

			IImportModuleInfo defaultAdapter = this.Adapters.FirstOrDefault(r => r.IsDefault);
			if (!String.IsNullOrEmpty(currentAdapterNameByCBI))
			{
				if (base.Adapters.Any(r => r.Name == currentAdapterNameByCBI))
					base.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.Name == currentAdapterNameByCBI);
				else
				{
					Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					 {
						 this.SelectedAdapter = defaultAdapter;
						 string message = Localization.Resources.Msg_Default_Adapter_Unaccessible;
						 MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Warning, Content = message, Settings = this._userSettingsManager };
						 this._messageBoxRequest.Raise(notification);
					 }), DispatcherPriority.Background);
				}
			}
			else
				this.SelectedAdapter = defaultAdapter;

			this._isDefaultAdapterFromCustomer = base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer;
			this._isDefaultAdapterFromBranch = base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch;
			this._isDefaultAdapterFromInventor = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;

			_isInvertCheckBoxVisible = false;

			switch (Mode)
			{
				case ImportDomainEnum.ImportCatalog:
				case ImportDomainEnum.ImportLocation:
				case ImportDomainEnum.ImportItur:
				case ImportDomainEnum.ImportSection:
				case ImportDomainEnum.ImportSupplier:
				case ImportDomainEnum.ImportFamily:
					_startButtonTitle = Localization.Resources.Command_Import;
					_isFromInternalEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch || base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;
					_isClearButtonVisible = true;
					_isEncodingVisible = true;
					_isFromConfigEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;
					_fromTitle = Localization.Resources.View_ImportWithModules_tbImportFrom;
					_progressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;
					_adapterTypeTitle = Localization.Resources.View_ImportWithModules_tbImportAdapter;
					switch (Mode)
					{
						case ImportDomainEnum.ImportCatalog:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportCatalog;
							_tryFastVisible = true;
							_isInvertCheckBoxVisible = true;
							break;
						case ImportDomainEnum.ImportLocation:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportLocation;
							_tryFastVisible = false;
							break;
						case ImportDomainEnum.ImportItur:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportItur;
							_tryFastVisible = false;
							break;
						case ImportDomainEnum.ImportFamily:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportFamily;
							_tryFastVisible = false;
							break;
						case ImportDomainEnum.ImportSection:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportSection;
							_tryFastVisible = false;
							_isInvertCheckBoxVisible = true;
							break;
						case ImportDomainEnum.ImportSupplier:
							_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportSupplier;
							_tryFastVisible = false;
							break;
					}
					break;
				case ImportDomainEnum.UpdateCatalog:
					_isFromInternalEnabled = false;
					_isClearButtonVisible = false;
					_isEncodingVisible = false;
					_startButtonTitle = Localization.Resources.Command_Update;
					_fromTitle = Localization.Resources.View_ImportWithModules_tbUpdateFrom;
					_progressText = Localization.Resources.ViewModel_ImportWithModulesBase_UpdateProgress;
					_adapterTypeTitle = Localization.Resources.View_ImportWithModules_tbUpdateAdapter;
					_title = Localization.Resources.ViewModel_ImportWithModules_TitleUpdate;
					_tryFastVisible = true;
					_isFromConfigEnabled = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;
					break;
				//case ImportDomainEnum.ComplexOperation:
				//	_isFromInternalEnabled = false;
				//	_isClearButtonVisible = false;
				//	_isEncodingVisible = false;
				//	_tryFastVisible = false;
				//	_isInvertCheckBoxVisible = false;
				//	_isRadioFileVisible = false;
				//	_startButtonTitle = Localization.Resources.Command_ImportExport;
				//	_fromTitle = Localization.Resources.View_ImportWithModules_tbImportExportFrom;
				//	_progressText = Localization.Resources.ViewModel_ImportWithModulesBase_Progress;
				//	_adapterTypeTitle = Localization.Resources.View_ImportWithModules_tbImportExportAdapter;
				//	_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportExport;
				//break;
				case ImportDomainEnum.ImportBranch:
					_isFromInternalEnabled = false;
					_isClearButtonVisible = false;
					_isEncodingVisible = true;
					_startButtonTitle = Localization.Resources.Command_Import;
					_fromTitle = Localization.Resources.View_ImportWithModules_tbImportFrom;
					_progressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;
					_adapterTypeTitle = Localization.Resources.View_ImportWithModules_tbImportAdapter;
					_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportBranch;
					_tryFastVisible = false;
					_isInvertCheckBoxVisible = true;
					_isFromConfigEnabled = false;
					break;
				case ImportDomainEnum.ImportUnitPlan:
					_isClearButtonVisible = false;
					_startButtonTitle = Localization.Resources.Command_Import;
					_fromTitle = Localization.Resources.View_ImportWithModules_tbImportFrom;
					_progressText = Localization.Resources.ViewModel_ImportWithModulesBase_ImportProgress;
					_adapterTypeTitle = Localization.Resources.View_ImportWithModules_tbImportAdapter;
					_title = Localization.Resources.ViewModel_ImportWithModules_TitleImportUnitPlan;
					_tryFastVisible = false;
					_isFromInternalEnabled = false;
					_isEncodingVisible = false;
					_isFromConfigEnabled = false;
					break;
			}


			this.ContextCustomerAdapterName = this.GetContextCustomerAdaperName();
			RaisePropertyChanged(() => this.ContextCustomerAdapterName);


			//CompareSelectedAdapterInventorAndCustomer();
		}

		public override void CompareSelectedAdapterInventorAndCustomer()
		{
			if (this.SelectedAdapter != null)
			{
				string selectedAdapterName = this.SelectedAdapter.Name;
				_errorAdapterName = "";

				string adapterName = this.GetContextCustomerAdaperName();
				if (selectedAdapterName.ToLower() != adapterName.ToLower())
				{
					_errorAdapterName = Localization.Resources.Selected_Adapter_Not_Equals_CustomerAdapter; // "Selected Adapter not equals Adapter for Customer";
				}
			}
			this.RaisePropertyChanged(() => this.ErrorAdapterName);
		}

		public override bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		void SetDefaultAdapterByCBI(string cbiContext)
		{
			string currentAdapterNameByCBI = this.GetSelectedAdapterNameByCBI(cbiContext, this.Mode);
			if (String.IsNullOrEmpty(currentAdapterNameByCBI))
				this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.IsDefault);
			else
				this.SelectedAdapter = this.Adapters.FirstOrDefault(r => r.Name == currentAdapterNameByCBI);
		}

		protected IImportProvider GetProviderInstance(string providerName)
		{
			IImportProvider provider = this._serviceLocator.GetInstance<IImportProvider>(providerName);
			return provider;
		}
		protected IImportProvider GetProviderInstance(ImportProviderEnum providerType)
		{
			IImportProvider provider = this._serviceLocator.GetInstance<IImportProvider>(providerType.ToString());
			return provider;
		}

		private string GetSelectedAdapterNameByCBI()
		{
			return this.GetSelectedAdapterNameByCBI(this.CBIDbContext, this.Mode);
		}

		private string GetSelectedAdapterNameByCBI(string cbidbContext, ImportDomainEnum mode)
		{
			switch (mode)
			{
				case ImportDomainEnum.ImportItur:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportIturProviderCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportIturProviderCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportIturAdapterCode;
					}
					break;
				case ImportDomainEnum.ImportLocation:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportLocationProviderCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportLocationProviderCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportLocationAdapterCode;
					}
					break;
				case ImportDomainEnum.ImportCatalog:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportCatalogProviderCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportCatalogProviderCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportCatalogAdapterCode;
					}
					break;
				case ImportDomainEnum.ImportSection:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportSectionAdapterCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportSectionAdapterCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportSectionAdapterCode;
					}
					break;
				case ImportDomainEnum.ImportSupplier:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportSupplierAdapterCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportSupplierAdapterCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportSupplierAdapterCode;
					}
					break;
				case ImportDomainEnum.UpdateCatalog:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.UpdateCatalogAdapterCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.UpdateCatalogAdapterCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.UpdateCatalogAdapterCode;
					}
					break;
				//case ImportDomainEnum.ComplexOperation:
				//	switch (cbidbContext)
				//	{

				//todo
				//case NavigationSettings.CBIDbContextCustomer:
				//	return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.UpdateCatalogAdapterCode;
				//case NavigationSettings.CBIDbContextBranch:
				//	return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.UpdateCatalogAdapterCode;
				//case NavigationSettings.CBIDbContextInventor:
				//	return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.UpdateCatalogAdapterCode;

				//	case NavigationSettings.CBIDbContextCustomer:
				//		return String.Empty ;
				//	case NavigationSettings.CBIDbContextBranch:
				//		return String.Empty ;
				//	case NavigationSettings.CBIDbContextInventor:
				//		return String.Empty;

				//}
				//break;
				case ImportDomainEnum.ImportBranch:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportBranchAdapterCode;
						case NavigationSettings.CBIDbContextBranch:
						case NavigationSettings.CBIDbContextInventor:
							return String.Empty;
					}
					break;
				case ImportDomainEnum.ImportFamily:
					switch (cbidbContext)
					{
						case NavigationSettings.CBIDbContextCustomer:
							return base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportFamilyAdapterCode;
						case NavigationSettings.CBIDbContextBranch:
							return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportFamilyAdapterCode;
						case NavigationSettings.CBIDbContextInventor:
							return base.CurrentInventor == null ? String.Empty : base.CurrentInventor.ImportFamilyAdapterCode;
					}
					break;
				case ImportDomainEnum.ImportUnitPlan:
					return String.Empty;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return String.Empty;
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

			if (this._fromFile)
			{
				bool canImportByDynViewModel = base.DynViewModel == null ? false : base.DynViewModel.CanImport();
				return !base.IsBusy && canImportByDynViewModel;
			}
			else
			{
				return !base.IsBusy;
			}
		}

		#region  fromConfig
		// ============= Config

		private void ImportCommandExecuted()
		{
			ImportWithInitMode(this._fromFile, this._fromInternal, this._fromConfig);
		}

		private void ImportWithInitMode(bool initFromFile = true, bool orInitFromInternal = false, bool orInitFromConfig = false)
		{
			if (initFromFile == true)
			{
				if (base.DynViewModel != null)
				{
					if (base.DynViewModel.CanImport())
					{
						string clearWarningMessage = String.Empty;
						using (new CursorWait())
						{
							if (base.Mode == ImportDomainEnum.ImportCatalog)
							{
								bool isCheckAdapter = (base.DynViewModel.AdapterName == ImportAdapterName.ImportCatalogMerkavaXslxAdapter
									|| base.DynViewModel.AdapterName == ImportAdapterName.ImportCatalogClalitXslxAdapter
									|| base.DynViewModel.AdapterName == ImportAdapterName.ImportCatalogNativXslxAdapter
									|| base.DynViewModel.AdapterName == ImportAdapterName.ImportCatalogNativPlusXslxAdapter
									|| base.DynViewModel.AdapterName == ImportAdapterName.ImportCatalogNativExportErpAdapter);

								if (isCheckAdapter == false)
								{
									if (this._productRepository.IsAnyProductInDb(base.GetDbPath) == true)
										clearWarningMessage = Localization.Resources.Msg_CatalogNotEmpty;
								}
							}
							if (base.Mode == ImportDomainEnum.ImportSupplier)
							{
								if (this._supplierRepository.IsAnyInDb(base.GetDbPath))
									clearWarningMessage = Localization.Resources.Msg_SuppliersNotEmpty;
							}
						}
						if (!String.IsNullOrWhiteSpace(clearWarningMessage))
						{
							MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(clearWarningMessage, MessageBoxButton.OKCancel, MessageBoxImage.Warning, _userSettingsManager);

							if (messageBoxResult == MessageBoxResult.Cancel)
								return;

							using (new CursorWait())
							{
								this.DynViewModel.RunClear(new ImportClearCommandInfo()
								{
									Callback = new Action(() => Utils.SetCursor(false))
								});
							}
						}

						ImportCommandInfo info = new ImportCommandInfo();
						info.IsWriteLogToFile = base.IsWriteLogToFile;
						info.Callback = () =>
						{
							base.Progress = String.Empty;
							base.ProgressStep = String.Empty;
							ShowLog();
						};
						base.Cts = new CancellationTokenSource();
						info.CancellationToken = base.Cts.Token;
						info.IsInvertLetters = this._isInvertLetters;
						info.IsInvertWords = this._isInvertWords;
						info.TryFast = this._tryFast;
						info.AdapterName = "";
						info.ConfigXDocument = null;
						info.ConfigXDocumentPath = "";
						info.FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;
						base.DynViewModel.RunImportBase(info);
					}
				}
			}
			else if (orInitFromInternal == true)
			{
				ImportFromInternalMode();
			}
			else if (orInitFromConfig == true)		  
			{
				ImportDomainEnum mode = base.Mode;
				ConfigXDocFromEnum fromConfigXDoc = this.GetFromConfigXDoc();
				ContextCustomerAdapterName = this.GetContextCustomerAdaperName();

				ImportFromConfigMode(base.DynViewModel, ContextCustomerAdapterName, mode, fromConfigXDoc, base.State, null);
			}
		}

		public void ImportFromConfigMode(
			ImportModuleBaseViewModel viewModel,  
			string adapterName, 
			ImportDomainEnum mode,
			ConfigXDocFromEnum fromConfigXDoc, 
			CBIState state, 
			ImportCommandInfo infoParam,
			bool isWriteLogToFile = true)
		{
			string warningMessage = String.Empty;

			if (viewModel != null)
			{
				ImportCommandInfo info = new ImportCommandInfo();
				info.IsWriteLogToFile = true;//base.IsWriteLogToFile;
				info.Callback = () =>
				{
					base.Progress = String.Empty;
					base.ProgressStep = String.Empty;
					ShowLog();
				};
				base.Cts = new CancellationTokenSource();
				info.CancellationToken = base.Cts.Token;
				info.TryFast = true;
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Mode = mode;
				info.IsWriteLogToFile = isWriteLogToFile;
				if (infoParam != null)
				{
					// f. e info.FromConfigXDoc = infoParam.FromConfigXDoc;
				}

				viewModel.RunImportWithoutGUIBase(info, state);
			}
		}

		public void ClearFromConfigMode(ImportModuleBaseViewModel viewModel,
		string adapterName,
		ImportDomainEnum mode,
		ConfigXDocFromEnum fromConfigXDoc, CBIState state)
		{
			string warningMessage = String.Empty;

			if (viewModel != null)
			{
				ImportCommandInfo info = new ImportCommandInfo();
				info.IsWriteLogToFile = true;//base.IsWriteLogToFile;
				info.Callback = () =>
				{
					base.Progress = String.Empty;
					base.ProgressStep = String.Empty;
					ShowLog();
				};
				base.Cts = new CancellationTokenSource();
				info.CancellationToken = base.Cts.Token;
				info.TryFast = true;
				info.AdapterName = adapterName;
				info.FromConfigXDoc = fromConfigXDoc;
				info.ConfigXDocumentPath = "";
				info.Mode = mode;

				viewModel.RunImportClearWithoutGUIBase(info, state);
			}
		}

		private void ImportFromInternalMode()
		{
			object currentDomainObject = null;
			if (this._fromCustomer)
				currentDomainObject = base.CurrentCustomer;
			if (this._fromBranch)
				currentDomainObject = base.CurrentBranch;
			if (this._fromInventor)
				currentDomainObject = base.CurrentInventor;

			if (currentDomainObject != null)
			{
				IsBusy = true;
				base.Cts = new CancellationTokenSource();
				this._cancellationToken = base.Cts.Token;
				this._stepTotal = 1;
				this._stepCurrent = 1;
				DateTime updateDateTime = DateTime.Now;
				base.SetModifyDateTimeCurrentDomainObject(updateDateTime);
				Task.Factory.StartNew(ImportFromDB, currentDomainObject).LogTaskFactoryExceptions("ImportCommandExecuted : ImportFromInternalMode");
			}
		}

	

		//Сейчас берем адаптер только из Customer , так ограничились
		private string GetContextCustomerAdaperName()
		{
			string contextAdapterName = "";
			//if (this.SelectedAdapter != null)
			//{
			//	contextAdapterName = SelectedAdapter.Name;
			//}
			//object currentDomainObject = null;
			//if (this.FromCustomer == true)
			//{
				//currentDomainObject = base.CurrentCustomer;
				contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextCustomer.ToString(), this.Mode);
			//}
			//else if (this.FromBranch == true)
			//{
			//	currentDomainObject = base.CurrentBranch;
			//	contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextBranch.ToString(), this.Mode);
			//}
			//else if (this.FromInventor == true)
			//{
//currentDomainObject = base.CurrentInventor;
			//	contextAdapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextInventor.ToString(), this.Mode);
			//}
				if (string.IsNullOrWhiteSpace(contextAdapterName) == true)
					contextAdapterName = WarningAdapter.AdapterInCustomerNotSet; 
			return contextAdapterName;
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
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextCustomer.ToString(), this.Mode);
				}
				else if (this.FromBranch == true)
				{
					currentDomainObject = base.CurrentBranch;
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextBranch.ToString(), this.Mode);
				}
				else if (this.FromInventor == true)
				{
					currentDomainObject = base.CurrentInventor;
					adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextInventor.ToString(), this.Mode);
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

	
		//public string GetXDocumentConfigPath(ref ImportCommandInfo info)
		//{
		//	string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultParamFolderPath().TrimEnd(@"\".ToCharArray());
		//	string fileConfigPath = String.Empty;
		//	if (base.SelectedAdapter != null)		   //by default будет браться с адаптера
		//	{
		//		string adapterName = base.SelectedAdapter.Name;
		//		fileConfigPath = adapterDefaultParamFolderPath + @"\" + adapterName + @"\Config.xml";
		//	}

		//	switch (info.FromConfigXDoc)
		//	{
		//		case	ConfigXDocFromEnum.FromConfigXDocument:
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
		//		case ConfigXDocFromEnum.FromBranchInData:
		//		case ConfigXDocFromEnum.FromInventorInData:
		//		case ConfigXDocFromEnum.FromDomainObjectInData:
		//			fileConfigPath = base.GetConfigFolderPath(adapterName) + @"\Config.xml";
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromFullPath:
		//			fileConfigPath = info.ConfigXDocumentPath;
		//			break;


		//	}

			//XDocument configXDoc = new XDocument();
			//if (File.Exists(fileConfigPath) == true)	   //если нет сохраненного файла config.xml
			//{
			//	try
			//	{
			//		configXDoc = XDocument.Load(fileConfigPath);
			//	}
			//	catch (Exception exp)
			//	{
			//	}
			//}
		//	return fileConfigPath;
		//}

		#endregion fromConfig

		private bool SetAsDefaultCommandCanExecute()
		{
			if (base.SelectedAdapter == null) return false;
			string setedAdapter = this.GetSelectedAdapterNameByCBI();
			string selectedAdapter = base.SelectedAdapter.Name;
			bool anyAdapter = base.Adapters.Any();
			return anyAdapter
				&& selectedAdapter != setedAdapter
				&& this._fromFile;
		}

		protected override void OnSelectedAdapterChanged()
		{
			base.OnSelectedAdapterChanged();

			this._setAsDefaultCommand.RaiseCanExecuteChanged();

			if (base.DynViewModel != null)
			{
				if (base.DynViewModel.Encoding != null)
					this._encodingSelectedItem = this.EncodingItems.FirstOrDefault(r => r.Encoding == base.DynViewModel.Encoding);
				else
					this._encodingSelectedItem = this.EncodingItems.FirstOrDefault();

				RaisePropertyChanged(() => EncodingSelectedItem);

				this._isInvertLetters = base.DynViewModel.IsInvertLetters;
				RaisePropertyChanged(() => IsInvertLetters);

				this._isInvertWords = base.DynViewModel.IsInvertWords;
				RaisePropertyChanged(() => IsInvertWords);
			}
		}

		private void SetAsDefaultCommandExecuted()
		{
			using (new CursorWait())
			{
				string adapterName = "";
				if (base.SelectedAdapter != null) adapterName = base.SelectedAdapter.Name;
	
				switch (this.Mode)
				{
						case ImportDomainEnum.ImportItur:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportIturProviderCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportIturProviderCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportIturAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}
						break;
					case ImportDomainEnum.ImportLocation:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportLocationProviderCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportLocationProviderCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportLocationAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;

						}
						break;
					case ImportDomainEnum.ImportCatalog:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportCatalogProviderCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportCatalogProviderCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportCatalogAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}

						UpdateCBIForRelatedAdapters();

						break;
					case ImportDomainEnum.ImportSection:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportSectionAdapterCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportSectionAdapterCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportSectionAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}
						break;
					case ImportDomainEnum.ImportSupplier:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportSupplierAdapterCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportSupplierAdapterCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportSupplierAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}
						break;
					case ImportDomainEnum.UpdateCatalog:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.UpdateCatalogAdapterCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.UpdateCatalogAdapterCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.UpdateCatalogAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}
						break;
					case ImportDomainEnum.ImportBranch:
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportBranchAdapterCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
						}
						break;

					case ImportDomainEnum.ImportFamily:
	
							//TO DO save когда появится место в БД
						switch (base.CBIDbContext)
						{
							case NavigationSettings.CBIDbContextCustomer:
								base.CurrentCustomer.ImportFamilyAdapterCode = adapterName;
								this._customerRepository.Update(base.CurrentCustomer);
								break;
							case NavigationSettings.CBIDbContextBranch:
								base.CurrentBranch.ImportFamilyAdapterCode = adapterName;
								this._branchRepository.Update(base.CurrentBranch);
								break;
							case NavigationSettings.CBIDbContextInventor:
								base.CurrentInventor.ImportFamilyAdapterCode = adapterName;
								this._inventorRepository.Update(base.CurrentInventor);
								break;
						}
					
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				this._setAsDefaultCommand.RaiseCanExecuteChanged();
			}
		}

		private void UpdateCBIForRelatedAdapters()
		{
			if (base.SelectedAdapter == null) return;

			string importCatalogAdapterName = base.SelectedAdapter.Name;
			string updateCatalog = AdapterConnections.GetRelated(importCatalogAdapterName, ImportDomainEnum.UpdateCatalog);
			string exportErp = AdapterConnections.GetRelated(importCatalogAdapterName, ImportDomainEnum.ExportCatalogERP);
			string importBranch = AdapterConnections.GetRelated(importCatalogAdapterName, ImportDomainEnum.ImportBranch);
			string importSupplier = AdapterConnections.GetRelated(importCatalogAdapterName, ImportDomainEnum.ImportSupplier);

			switch (base.CBIDbContext)
			{
				case NavigationSettings.CBIDbContextCustomer:
					base.CurrentCustomer.ImportCatalogProviderCode = importCatalogAdapterName;
					base.CurrentCustomer.UpdateCatalogAdapterCode = updateCatalog;
					base.CurrentCustomer.ExportERPAdapterCode = exportErp;
					base.CurrentCustomer.ImportBranchAdapterCode = importBranch;
					base.CurrentCustomer.ImportSupplierAdapterCode = importSupplier;
					this._customerRepository.Update(base.CurrentCustomer);
					break;
				case NavigationSettings.CBIDbContextBranch:
					base.CurrentBranch.ImportCatalogProviderCode = importCatalogAdapterName;
					base.CurrentBranch.UpdateCatalogAdapterCode = updateCatalog;
					base.CurrentBranch.ExportERPAdapterCode = exportErp;
					base.CurrentBranch.ImportSupplierAdapterCode = importSupplier;
					this._branchRepository.Update(base.CurrentBranch);

					if (base.CurrentCustomer != null)
					{
						base.CurrentCustomer.ImportBranchAdapterCode = importBranch;
						this._customerRepository.Update(base.CurrentCustomer);
					}
					break;
				case NavigationSettings.CBIDbContextInventor:
					base.CurrentInventor.ImportCatalogAdapterCode = importCatalogAdapterName;
					base.CurrentInventor.UpdateCatalogAdapterCode = updateCatalog;
					base.CurrentInventor.ExportERPAdapterCode = exportErp;
					base.CurrentInventor.ImportSupplierAdapterCode = importSupplier;
					this._inventorRepository.Update(base.CurrentInventor);

					if (base.CurrentCustomer != null)
					{
						base.CurrentCustomer.ImportBranchAdapterCode = importBranch;
						this._customerRepository.Update(base.CurrentCustomer);
					}
					break;
			}
		}

		public void ImportFromDB(object fromDomainObject)
		{
			try
			{
				this._logImport.Clear();
				IImportProvider provider = null;
				switch (this.Mode)
				{
					case ImportDomainEnum.ImportItur:
						provider = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBADOProvider);
						break;
					case ImportDomainEnum.ImportLocation:
						provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
						break;
					case ImportDomainEnum.ImportCatalog:
						//provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogFromDBADOProvider);
						provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogFromDBBulkProvider);
						break;
					case ImportDomainEnum.ImportSection:
						provider = this.GetProviderInstance(ImportProviderEnum.ImportSectionFromDBADOProvider);
						break;
					case ImportDomainEnum.ImportSupplier:
						provider = this.GetProviderInstance(ImportProviderEnum.ImportSupplierFromDBADOProvider);
						break;
					case ImportDomainEnum.UpdateCatalog:
						//todo
						//provider = this.GetProviderInstance(ImportProviderEnum.ImportSectionFromDBADOProvider);
						break;
					case ImportDomainEnum.ImportBranch:
						//todo
						break;
				}
				if (provider != null)
				{
					provider.FromPathFile = base.ContextCBIRepository.GetDBPath(fromDomainObject);
					provider.ToPathDB = base.GetDbPath;
					//provider.Clear();	//??Clear()
					provider.Parms.Clear();
					provider.Parms.AddCancellationUpdate(this._updateProgress, this._cancellationToken);
					provider.Import();
					// UpdateLog(this._logImport.PrintLog(encoding: EncodingSelectedItem.Encoding));	   test without encoding
					UpdateLog(this._logImport.PrintLog());
				}

				if (base.IsWriteLogToFile)
				{
					string log = this._logImport.FileLog(this._encodingSelectedItem == null ? null : this._encodingSelectedItem.Encoding);
					string logPath = Path.GetFullPath(this.GetImportPath().Trim('\\') + @"\" + "log.txt");
					File.WriteAllText(logPath, log);
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("ImportFromDB", exc);
			}
			finally
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(() => IsBusy = false), DispatcherPriority.Normal);
				//base.LogText = this._logImport.PrintLog(encoding: EncodingSelectedItem.Encoding);		 //test
				base.LogText = this._logImport.PrintLog();
				Utils.RunOnUI(base.ShowLog);
			}
		}

		protected string GetImportPath()
		{
			if (!String.IsNullOrEmpty(this.CBIDbContext))
			{
				object currentDomainObject = base.GetCurrentDomainObject();

				if (currentDomainObject != null)
				{
					return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

				}
			}
			return String.Empty;
		}

		private void UpdateLog(string log)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() => base.LogText = log), DispatcherPriority.Normal);

		}

		private void BuildEncoding()
		{
			var list = new List<Encoding>
                           {
                               Encoding.GetEncoding(862), Encoding.GetEncoding(1255),
                               Encoding.ASCII, Encoding.Unicode, Encoding.UTF8, Encoding.UTF32, Encoding.UTF7
                           };
			foreach (var encodingInfo in Encoding.GetEncodings().OrderBy(r => r.DisplayName))
			{
				Encoding enc = encodingInfo.GetEncoding();
				if (!list.Contains(enc))
					list.Add(enc);
			}
			foreach (Encoding encoding in list)
			{
				EncodingItemViewModel item = new EncodingItemViewModel(encoding);
				this._encodingItems.Add(item);
			}
		}

		private void NavigateToGridCommandExecuted()
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

			switch (this.Mode)
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
				case ImportDomainEnum.ImportBranch:
					UtilsNavigate.BranchChooseOpen(CBIContext.Main, base._regionManager, query);
					break;
				case ImportDomainEnum.ImportUnitPlan:
					UtilsNavigate.PlanogramAddEditDeleteOpen(base._regionManager, query);
					break;
			}
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
		//}

		//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		//protected string GetConfigFolderPath()
		//{
		//	object currentDomainObject = null;
		//	string adapterName = base.SelectedAdapter.Name;


		//	if (String.IsNullOrEmpty(this.CBIDbContext) == false)
		//	{
		//		if (this.FromCustomer == true)
		//		{
		//			currentDomainObject = base.CurrentCustomer;
		//			adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextCustomer.ToString(), this.Mode);
		//		}
		//		else if (this.FromBranch == true)
		//		{
		//			currentDomainObject = base.CurrentBranch;
		//			adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextBranch.ToString(), this.Mode);
		//		}
		//		else if (this.FromInventor == true)
		//		{
		//			currentDomainObject = base.CurrentInventor;
		//			adapterName = this.GetSelectedAdapterNameByCBI(NavigationSettings.CBIDbContextInventor.ToString(), this.Mode);
		//		}

		//		if (currentDomainObject != null)
		//		{

		//			string dataInPath = base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

		//			if (string.IsNullOrWhiteSpace(dataInPath) == false)
		//			{
		//				if (Directory.Exists(dataInPath + @"\Config") == false)
		//				{
		//					Directory.CreateDirectory(dataInPath + @"\Config");
		//				}
		//			}

		//			string dataInConfigPath = dataInPath + @"\Config\" + adapterName;
		//			if (Directory.Exists(dataInConfigPath) == false)		 //нет - создаем
		//			{
		//				Directory.CreateDirectory(dataInConfigPath);
		//			}
		//			return dataInConfigPath;
		//		}
		//		return String.Empty;
		//	}
		//	return String.Empty;
		//}


		//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		//protected string GetRelativePath(string adapterType) // было GetImportPath()
		//{
		//	object currentDomainObject = null;

		//	if (String.IsNullOrEmpty(this.CBIDbContext) == false)
		//	{
		//		//object currentDomainObject = base.GetCurrentDomainObject();

		//		if (this._isDefaultAdapterFromCustomer == true)
		//		{
		//			currentDomainObject = base.CurrentCustomer;
		//		}
		//		else if (this._isDefaultAdapterFromBranch == true)
		//		{
		//			currentDomainObject = base.CurrentBranch;
		//		}
		//		else if (this._isDefaultAdapterFromInventor == true)
		//		{
		//			currentDomainObject = base.CurrentInventor;
		//		}

		//		if (currentDomainObject != null)
		//		{
		//			if (adapterType == "ImportWithModulesBaseView")
		//			{
		//				return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);
		//			}
		//			else if (adapterType == "ExportPdaWithModulesView")
		//			{
		//				return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
		//			}
		//			else if (adapterType == "ExportErpWithModulesView")
		//			{
		//				return GetExportErpFolderPath();
		//			}
		//			else return String.Empty;
		//		}
		//	}
		//	return String.Empty;
		//}
	}
}

