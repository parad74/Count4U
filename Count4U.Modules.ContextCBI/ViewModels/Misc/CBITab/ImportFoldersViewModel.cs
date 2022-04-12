using System;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Constants.AdapterNames;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Report.ViewModels.ExportPda;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
	public class ImportFoldersViewModel : CBIContextBaseViewModel
	{
		private readonly IUnityContainer _container;
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;

		private readonly IImportAdapterRepository _importAdapterRepository;

		private IImportModuleInfo _selectedCatalogs;
		private ObservableCollection<IImportModuleInfo> _itemsCatalogs;

		private IImportModuleInfo _selectedIturs;
		private ObservableCollection<IImportModuleInfo> _itemsIturs;

		private IImportModuleInfo _selectedLocations;
		private ObservableCollection<IImportModuleInfo> _itemsLocations;

		private IImportModuleInfo _selectedSections;
		private ObservableCollection<IImportModuleInfo> _itemsSections;


		private IImportModuleInfo _selectedFamily;
		private ObservableCollection<IImportModuleInfo> _itemsFamilys;

		private IImportModuleInfo _selectedSuppliers;
		private ObservableCollection<IImportModuleInfo> _itemsSuppliers;

		private IImportModuleInfo _selectedPDA;
		private ObservableCollection<IImportModuleInfo> _itemsPDA;

		private Customer _customer;
		private Branch _branch;
		private Inventor _inventor;

		private bool _isEditable;
		private bool _isShowConfig;

		private bool _makat;
        private bool _barcode;

		private ExportPdaExtraSettingsViewModel _extraSettingsViewModel;

		private bool _isEventFromCode;

		private readonly UICommandRepository<IImportModuleInfo> _commandRepositoryObject;
		//private readonly DelegateCommand<IImportModuleInfo> _showConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showIturConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showLocationConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showSectionConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showSupplierConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showFamilyConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showCatalogConfigCommand;
		private readonly DelegateCommand<IImportModuleInfo> _showPDAConfigCommand;
		//	private bool _configFileImportCatalogExists;

		//public IImportModuleInfo _selectedAdapter;

		public ImportFoldersViewModel(
			IUnityContainer container,
			IContextCBIRepository contextCbiRepository,
			IImportAdapterRepository importAdapterRepository,
			IEventAggregator eventAggregator,
			UICommandRepository<IImportModuleInfo> commandRepositoryObject,
			IRegionManager regionManager)
			: base(contextCbiRepository)
		{
			this._regionManager = regionManager;
			this._eventAggregator = eventAggregator;
			this._commandRepositoryObject = commandRepositoryObject;
			this._importAdapterRepository = importAdapterRepository;
			this._container = container;
			//this._showConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowConfigCommandCanExecuted);
			this._showIturConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowIturConfigCommandCanExecuted);
			this._showLocationConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowLocationConfigCommandCanExecuted);
			this._showSectionConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowSectionConfigCommandCanExecuted);
			this._showSupplierConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowSupplierConfigCommandCanExecuted);
			this._showFamilyConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowFamilyConfigCommandCanExecuted);
			this._showCatalogConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowCatalogConfigCommandCanExecuted);
			this._showPDAConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowPDAConfigCommandCanExecuted);
			this._isEditable = true;
			this._isShowConfig = true;
			this._makat = true;
			this._barcode = false;
			this.IsMakatRadioVisible = true;
		}

		public bool ShowConfigCommandCanExecuted(IImportModuleInfo info)
		{
			if (info == null) return false;
			bool configFileExists = base.IsConfigFileImportExists(info.Name);
			return configFileExists;
		}


		public bool Makat
        {
            get { return _makat; }
            set
            {
                this._makat = value;
                RaisePropertyChanged(() => Makat);

                this._barcode = !value;
                RaisePropertyChanged(() => Barcode);
            }
        }

        public bool Barcode
        {
            get { return _barcode; }
            set
            {
                this._barcode = value;
                RaisePropertyChanged(() => Barcode);

                this._makat = !value;
                RaisePropertyChanged(() => Makat);
            }
        }

		public bool _isMakatRadioVisible;
        public bool IsMakatRadioVisible
        {
            get { return _isMakatRadioVisible; }
            set
            {
                _isMakatRadioVisible = value;
                RaisePropertyChanged(() => IsMakatRadioVisible);
            }
        }

		public bool ShowIturConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedIturs);
		}

		public bool ShowLocationConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedLocations);
		}
		public bool ShowSectionConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedSections);
		}

		public bool ShowSupplierConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedSuppliers);
		}


		public bool ShowFamilyConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedFamily);
		}

		public bool ShowCatalogConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedCatalogs);
		}

		public bool ShowPDAConfigCommandCanExecuted(IImportModuleInfo info)
		{
			return ShowConfigCommandCanExecuted(this.SelectedPDA);
		}

		//public bool ShowConfigCommandCanExecuted(IImportModuleInfo info)
		//{
		//	bool configFileExists = base.IsConfigFileImportExists(info);
		//	return configFileExists;
		//}

		//public bool ConfigFileImportCatalogExists
		//{
		//	get
		//	{

		//		return this._configFileImportCatalogExists;
		//	}
		//	set { this._configFileImportCatalogExists = value; }
		//}

		//public IImportModuleInfo SelectedAdapter
		//{
		//	get { return this._selectedAdapter; }
		//	set
		//	{
		//		this._selectedAdapter = value;
		//		RaisePropertyChanged(() => SelectedAdapter);
		//	}
		//}

		//public DelegateCommand<IImportModuleInfo> ShowConfigCommand
		//{
		//	get { return this._showConfigCommand; }
		//}

		public DelegateCommand<IImportModuleInfo> ShowIturConfigCommand
		{
			get { return this._showIturConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowLocationConfigCommand
		{
			get { return this._showLocationConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowSectionConfigCommand
		{
			get { return this._showSectionConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowSupplierConfigCommand
		{
			get { return this._showSupplierConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowFamilyConfigCommand
		{
			get { return this._showFamilyConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowCatalogConfigCommand
		{
			get { return this._showCatalogConfigCommand; }
		}

		public DelegateCommand<IImportModuleInfo> ShowPDAConfigCommand
		{
			get { return this._showPDAConfigCommand; }
		}

		private void ShowConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			// this.SelectedAdapter = importModuleInfo;
			// if (importModuleInfo != null)
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = importModuleInfo });

		}

		public void GotNewFofusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });
			}
		}

		public void LostFocusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = null });
			}
		}

		public IImportModuleInfo SelectedCatalogs
		{
			get { return this._selectedCatalogs; }
			set
			{
				this._selectedCatalogs = value;
				RaisePropertyChanged(() => SelectedCatalogs);
				this._showCatalogConfigCommand.RaiseCanExecuteChanged();

				if (!_isEventFromCode)
				{
					if (_selectedCatalogs != null)
						this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedCatalogs });
				}

				SetSupplierAdapterOnCatalogAdapterChange();
			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsCatalogs
		{
			get { return this._itemsCatalogs; }
		}

		public IImportModuleInfo SelectedIturs
		{
			get { return this._selectedIturs; }
			set
			{
				this._selectedIturs = value;
				RaisePropertyChanged(() => SelectedIturs);
				this._showIturConfigCommand.RaiseCanExecuteChanged();

				if (_selectedIturs != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedIturs });

			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsIturs
		{
			get { return this._itemsIturs; }
		}

		public IImportModuleInfo SelectedLocations
		{
			get { return this._selectedLocations; }
			set
			{
				this._selectedLocations = value;
				RaisePropertyChanged(() => SelectedLocations);
				this._showLocationConfigCommand.RaiseCanExecuteChanged();

				if (_selectedLocations != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedLocations });

			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsLocations
		{
			get { return this._itemsLocations; }
		}

		public IImportModuleInfo SelectedSections
		{
			get { return _selectedSections; }
			set
			{
				_selectedSections = value;
				RaisePropertyChanged(() => SelectedSections);
				this._showSectionConfigCommand.RaiseCanExecuteChanged();

				if (_selectedSections != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedSections });

			}
		}


		public IImportModuleInfo SelectedFamily
		{
			get { return _selectedFamily; }
			set
			{
				_selectedFamily = value;
				RaisePropertyChanged(() => SelectedFamily);
				this._showFamilyConfigCommand.RaiseCanExecuteChanged();

				if (_selectedFamily != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedFamily });

			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsSections
		{
			get { return _itemsSections; }
		}

		public ObservableCollection<IImportModuleInfo> ItemsFamilys
		{
			get { return _itemsFamilys; }
		}


		public IImportModuleInfo SelectedSuppliers
		{
			get { return _selectedSuppliers; }
			set
			{
				_selectedSuppliers = value;
				RaisePropertyChanged(() => SelectedSuppliers);
				this._showSupplierConfigCommand.RaiseCanExecuteChanged();

				if (_selectedSuppliers != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedSuppliers });

			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsSuppliers
		{
			get { return _itemsSuppliers; }
		}

		public bool IsEditable
		{
			get { return this._isEditable; }
			set
			{
				this._isEditable = value;
				RaisePropertyChanged(() => IsEditable);
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

		public IImportModuleInfo SelectedPDA
		{
			get { return _selectedPDA; }
			set
			{
				_selectedPDA = value;
				RaisePropertyChanged(() => SelectedPDA);
				this._showPDAConfigCommand.RaiseCanExecuteChanged();

				if (_selectedPDA != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedPDA });

			}
		}

		public ObservableCollection<IImportModuleInfo> ItemsPDA
		{
			get { return _itemsPDA; }
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
			string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
			string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

			this._itemsCatalogs = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportCatalog,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			this._itemsIturs = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportItur,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			this._itemsLocations = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportLocation,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			this._itemsSections = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportSection,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			this._itemsFamilys = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportFamily,
				currentCustomerCode, currentBranchCode, currentInventorCode));


			this._itemsSuppliers = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportSupplier,
				currentCustomerCode, currentBranchCode, currentInventorCode));

			this._itemsPDA = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportInventProduct,
			  currentCustomerCode, currentBranchCode, currentInventorCode));

			//            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.CustomerMode))
			//            {
			this._extraSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaExtraSettingsViewModel>(Common.RegionNames.ExportPdaExtraSettings, _regionManager);
			//            }
		}

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);

		}

		public void SetIsEditable(bool isEditable)
		{
			this.IsEditable = isEditable;

			if (_extraSettingsViewModel != null)
				_extraSettingsViewModel.IsEditable = this._isEditable;
		}

		public void SetExtraSettingsDissable(bool isEditable)
		{
			if (_extraSettingsViewModel != null)
			{
				_extraSettingsViewModel.IsEditable = isEditable;
			}
		}


		public void SetIsShowConfig(bool isShowConfig)
		{
			this.IsShowConfig = isShowConfig;
		}


		public void SetCustomer(Customer customer)
		{
			this._customer = customer;
		   this.IsMakatRadioVisible = true;
			_isEventFromCode = true;

			string catalogCode = customer.ImportCatalogProviderCode;
			if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
				_selectedCatalogs = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);
			else
				_selectedCatalogs = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);


			string iturCode = customer.ImportIturProviderCode;
			if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
				_selectedIturs = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);
			else
				_selectedIturs = ItemsIturs.FirstOrDefault(r => r.IsDefault);

			string locationCode = customer.ImportLocationProviderCode;
			if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
				_selectedLocations = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);
			else
				_selectedLocations = ItemsLocations.FirstOrDefault(r => r.IsDefault);

			string sectionCode = customer.ImportSectionAdapterCode;
			if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
				_selectedSections = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);
			else
				_selectedSections = ItemsSections.FirstOrDefault(r => r.IsDefault);

			string familyCode = customer.ImportFamilyAdapterCode;
			if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
				_selectedFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);
			else
				_selectedFamily = ItemsFamilys.FirstOrDefault(r => r.IsDefault);

			string supplierCode = customer.ImportSupplierAdapterCode;
			if (!String.IsNullOrEmpty(supplierCode) && ItemsSuppliers.Any(r => r.Name == supplierCode))
				_selectedSuppliers = ItemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);
			else
				_selectedSuppliers = ItemsSuppliers.FirstOrDefault(r => r.IsDefault);

			string pdaCode = customer.ImportPDAProviderCode;
			if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
				_selectedPDA = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			else
				_selectedPDA = ItemsPDA.FirstOrDefault(r => r.IsDefault);

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.SetCustomer(this._customer);
			}

			if (customer.Tag3 == Common.Constants.Misc.MakatValue)
                    Makat = true;
			else if (customer.Tag3 == Common.Constants.Misc.BarcodeValue)
                    Barcode = true;
            else
					Makat = true;

			this._showIturConfigCommand.RaiseCanExecuteChanged();
			this._showLocationConfigCommand.RaiseCanExecuteChanged();
			this._showSectionConfigCommand.RaiseCanExecuteChanged();
			this._showSupplierConfigCommand.RaiseCanExecuteChanged();
			this._showFamilyConfigCommand.RaiseCanExecuteChanged();
			this._showCatalogConfigCommand.RaiseCanExecuteChanged();
			this._showPDAConfigCommand.RaiseCanExecuteChanged();

			_isEventFromCode = false;
		}

		public void SetBranch(Branch branch, enBranchAdapterInherit mode)
		{
			this._branch = branch;
			this.IsMakatRadioVisible = false;
			_isEventFromCode = true;

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.SetBranch(this._branch, mode);
			}

			SetSelectedAdapterStateForBranch(mode);

			this._isEventFromCode = false;
		}

		public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
		{
			this._inventor = inventor;
			this.IsMakatRadioVisible = false;
			this._isEventFromCode = true;

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.SetInventor(this._inventor, mode);
			}

			SetSelectedAdapterStateForInventor(mode);

			_isEventFromCode = false;
		}

		public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
		{
			_isEventFromCode = true;

			IImportModuleInfo info = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoItur = ItemsIturs.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoLocation = ItemsLocations.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoSection = ItemsSections.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoFamily = ItemsFamilys.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoPda = ItemsPDA.FirstOrDefault(r => r.IsDefault);

			Customer customer = this.CurrentCustomer;

			if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
			{
				string catalogCode = customer.ImportCatalogProviderCode;
				if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
					info = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);

				string iturCode = customer.ImportIturProviderCode;
				if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
					infoItur = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);

				string locationCode = customer.ImportLocationProviderCode;
				if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
					infoLocation = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);

				string sectionCode = customer.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
					infoSection = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);

				string familyCode = customer.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
					infoFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);


				string supplierCode = customer.ImportSupplierAdapterCode;
				if (!String.IsNullOrEmpty(supplierCode) && ItemsSuppliers.Any(r => r.Name == supplierCode))
					infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);

				string pdaCode = customer.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
					infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			}
			else
			{
				string catalogCode = _branch.ImportCatalogProviderCode;
				if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
					info = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);

				string iturCode = _branch.ImportIturProviderCode;
				if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
					infoItur = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);

				string locationCode = _branch.ImportLocationProviderCode;
				if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
					infoLocation = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);

				string sectionCode = _branch.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
					infoSection = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);

				string familyCode = _branch.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
					infoFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);

				string pdaCode = _branch.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
					infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			}

			SelectedCatalogs = info;
			SelectedIturs = infoItur;
			SelectedLocations = infoLocation;
			SelectedFamily = infoFamily;
			SelectedSections = infoSection;
			SelectedSuppliers = infoSupplier;
			SelectedPDA = infoPda;

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.SetSelectedAdapterStateForBranch(mode);
			}

			_isEventFromCode = false;
		}


		public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
		{
			_isEventFromCode = true;

			IImportModuleInfo info = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoItur = ItemsIturs.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoLocation = ItemsLocations.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoSection = ItemsSections.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoPda = ItemsPDA.FirstOrDefault(r => r.IsDefault);
			IImportModuleInfo infoFamily = ItemsFamilys.FirstOrDefault(r => r.IsDefault);

			Customer customer = this.CurrentCustomer;
			Branch branch = this.CurrentBranch;

			if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
			{
				string catalogCode = customer.ImportCatalogProviderCode;
				if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
					info = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);

				string iturCode = customer.ImportIturProviderCode;
				if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
					infoItur = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);

				string locationCode = customer.ImportLocationProviderCode;
				if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
					infoLocation = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);

				string sectionCode = customer.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
					infoSection = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);

				string familyCode = customer.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
					infoFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);

				string supplierCode = customer.ImportSupplierAdapterCode;
				if (!String.IsNullOrEmpty(supplierCode) && ItemsSuppliers.Any(r => r.Name == supplierCode))
					infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);

				string pdaCode = customer.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
					infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			}
			else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
			{
				string catalogCode = branch.ImportCatalogProviderCode;
				if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
					info = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);

				string iturCode = branch.ImportIturProviderCode;
				if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
					infoItur = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);

				string locationCode = branch.ImportLocationProviderCode;
				if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
					infoLocation = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);

				string sectionCode = branch.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
					infoSection = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);

				string familyCode = branch.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
					infoFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);


				string supplierCode = branch.ImportSupplierAdapterCode;
				if (!String.IsNullOrEmpty(supplierCode) && ItemsSuppliers.Any(r => r.Name == supplierCode))
					infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);

				string pdaCode = branch.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
					infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			}
			else
			{
				string catalogCode = _inventor.ImportCatalogAdapterCode;
				if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
					info = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);

				string iturCode = _inventor.ImportIturAdapterCode;
				if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
					infoItur = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);

				string locationCode = _inventor.ImportLocationAdapterCode;
				if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
					infoLocation = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);

				string sectionCode = _inventor.ImportSectionAdapterCode;
				if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
					infoSection = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);

				string familyCode = _inventor.ImportFamilyAdapterCode;
				if (!String.IsNullOrEmpty(familyCode) && ItemsFamilys.Any(r => r.Name == familyCode))
					infoFamily = ItemsFamilys.FirstOrDefault(r => r.Name == familyCode);


				string supplierCode = _inventor.ImportSupplierAdapterCode;
				if (!String.IsNullOrEmpty(supplierCode) && ItemsSuppliers.Any(r => r.Name == supplierCode))
					infoSupplier = ItemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);

				string pdaCode = _inventor.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
					infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
			}

			SelectedCatalogs = info;
			SelectedIturs = infoItur;
			SelectedLocations = infoLocation;
			SelectedSections = infoSection;
			SelectedFamily = infoFamily;
			SelectedSuppliers = infoSupplier;
			SelectedPDA = infoPda;

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.SetSelectedAdapterStateForInventor(mode);
			}

			_isEventFromCode = false;
		}

		public void ApplyChanges()
		{
			if (this._customer != null)
			{
				this._customer.ImportCatalogProviderCode = this.SelectedCatalogs == null ?
															String.Empty :
															this.SelectedCatalogs.Name;

				this._customer.ImportIturProviderCode = this.SelectedIturs == null ?
														  String.Empty :
														  this.SelectedIturs.Name;

				this._customer.ImportLocationProviderCode = this.SelectedLocations == null ?
														  String.Empty :
														  this.SelectedLocations.Name;

				this._customer.ImportSectionAdapterCode = this.SelectedSections == null ?
														  String.Empty :
														  this.SelectedSections.Name;

				this._customer.ImportFamilyAdapterCode = this.SelectedFamily == null ?
												   String.Empty :
												   this.SelectedFamily.Name;

				this._customer.ImportSupplierAdapterCode = this.SelectedSuppliers == null ?
														   String.Empty :
														   this.SelectedSuppliers.Name;

				this._customer.ImportPDAProviderCode = this.SelectedPDA == null ?
													   String.Empty :
													   this.SelectedPDA.Name;
				
					
				if (Makat == true)
				{
					  this._customer.Tag3	= Common.Constants.Misc.MakatValue;
				}
				else
				{
					 this._customer.Tag3	= Common.Constants.Misc.BarcodeValue;
				}
         
			}

			if (this._branch != null)
			{
				this._branch.ImportCatalogProviderCode = this.SelectedCatalogs == null ?
															String.Empty :
															this.SelectedCatalogs.Name;

				this._branch.ImportIturProviderCode = this.SelectedIturs == null ?
														  String.Empty :
														  this.SelectedIturs.Name;

				this._branch.ImportLocationProviderCode = this.SelectedLocations == null ?
														  String.Empty :
														  this.SelectedLocations.Name;

				this._branch.ImportSectionAdapterCode = this.SelectedSections == null ?
														String.Empty :
														this.SelectedSections.Name;

				this._branch.ImportFamilyAdapterCode = this.SelectedFamily == null ?
										   String.Empty :
										   this.SelectedFamily.Name;

				this._branch.ImportSupplierAdapterCode = this.SelectedSuppliers == null ?
														 String.Empty :
														 this.SelectedSuppliers.Name;

				this._branch.ImportPDAProviderCode = this.SelectedPDA == null ?
													 String.Empty :
													 this.SelectedPDA.Name;
			}

			if (this._inventor != null)
			{
				this._inventor.ImportCatalogAdapterCode = this.SelectedCatalogs == null ?
															String.Empty :
															this.SelectedCatalogs.Name;

				this._inventor.ImportIturAdapterCode = this.SelectedIturs == null ?
														  String.Empty :
														  this.SelectedIturs.Name;

				this._inventor.ImportLocationAdapterCode = this.SelectedLocations == null ?
														  String.Empty :
														  this.SelectedLocations.Name;

				this._inventor.ImportSectionAdapterCode = this.SelectedSections == null ?
														  String.Empty :
														  this.SelectedSections.Name;

				this._inventor.ImportFamilyAdapterCode = this.SelectedFamily == null ?
													   String.Empty :
													   this.SelectedFamily.Name;

				this._inventor.ImportSupplierAdapterCode = this.SelectedSuppliers == null ?
														   String.Empty :
														   this.SelectedSuppliers.Name;

				this._inventor.ImportPDAProviderCode = this.SelectedPDA == null ?
													 String.Empty :
													 this.SelectedPDA.Name;
			}

			if (this._extraSettingsViewModel != null)
			{
				this._extraSettingsViewModel.ApplyChanges();
			}
		}

		private void SetSupplierAdapterOnCatalogAdapterChange()
		{
			if (this._selectedCatalogs == null) return;

			IImportModuleInfo supplierAdapter = null;
			string related = AdapterConnections.GetRelated(this._selectedCatalogs.Name, ImportDomainEnum.ImportSupplier);

			if (String.IsNullOrWhiteSpace(related) == false)
			{
				supplierAdapter = _itemsSuppliers.FirstOrDefault(r => r.Name == related);
			}

			if (supplierAdapter == null)
			{
				supplierAdapter = _itemsSuppliers.FirstOrDefault(r => r.IsDefault);
			}

			this._selectedSuppliers = supplierAdapter;
		}
	}
}