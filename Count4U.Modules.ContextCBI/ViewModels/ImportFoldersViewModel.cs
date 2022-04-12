using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Report.ViewModels.ExportPda;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public enum enInventorAdapterInherit
    {
        InheritFromCustomer,
        InheritFromBranch,
        InheritNothing
    }

    public enum enBranchAdapterInherit
    {
        InheritFromCustomer,
        InheritNothing
    }

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

        private IImportModuleInfo _selectedPDA;
        private ObservableCollection<IImportModuleInfo> _itemsPDA;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        private bool _isEditable;

        private ExportPdaExtraSettingsViewModel _extraSettingsViewModel;

        private bool _isEventFromCode;

        public ImportFoldersViewModel(
            IUnityContainer container,
            IContextCBIRepository contextCbiRepository,
            IImportAdapterRepository importAdapterRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            this._importAdapterRepository = importAdapterRepository;
            this._container = container;

            this._isEditable = true;
        }

        public IImportModuleInfo SelectedCatalogs
        {
            get { return this._selectedCatalogs; }
            set
            {
                this._selectedCatalogs = value;
                RaisePropertyChanged(() => SelectedCatalogs);

                if (!_isEventFromCode)
                {
                    if (_selectedCatalogs != null)
                        this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() {ImportModule = this._selectedCatalogs});
                }
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
            }
        }

        public ObservableCollection<IImportModuleInfo> ItemsSections
        {
            get { return _itemsSections; }
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

        public IImportModuleInfo SelectedPDA
        {
            get { return _selectedPDA; }
            set
            {
                _selectedPDA = value;
                RaisePropertyChanged(() => SelectedPDA);
            }
        }

        public ObservableCollection<IImportModuleInfo> ItemsPDA
        {
            get { return _itemsPDA; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterchanged);

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

            this._itemsPDA = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.ImportInventProduct,
              currentCustomerCode, currentBranchCode, currentInventorCode));

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.CustomerMode))
            {
                this._extraSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaExtraSettingsViewModel>(Common.RegionNames.ExportPdaExtraSettings, _regionManager);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterchanged);
        }

        public void SetIsEditable(bool isEditable)
        {
            this.IsEditable = isEditable;

            if (_extraSettingsViewModel != null)
                _extraSettingsViewModel.IsEditable = this._isEditable;
        }

        public void SetCustomer(Customer customer)
        {
            this._customer = customer;

            _isEventFromCode = true;

            string catalogCode = customer.ImportCatalogProviderCode;
            if (!String.IsNullOrEmpty(catalogCode) && ItemsCatalogs.Any(r => r.Name == catalogCode))
                SelectedCatalogs = ItemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);
            else
                SelectedCatalogs = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);

            string iturCode = customer.ImportIturProviderCode;
            if (!String.IsNullOrEmpty(iturCode) && ItemsIturs.Any(r => r.Name == iturCode))
                SelectedIturs = ItemsIturs.FirstOrDefault(r => r.Name == iturCode);
            else
                SelectedIturs = ItemsIturs.FirstOrDefault(r => r.IsDefault);

            string locationCode = customer.ImportLocationProviderCode;
            if (!String.IsNullOrEmpty(locationCode) && ItemsLocations.Any(r => r.Name == locationCode))
                SelectedLocations = ItemsLocations.FirstOrDefault(r => r.Name == locationCode);
            else
                SelectedLocations = ItemsLocations.FirstOrDefault(r => r.IsDefault);

            string sectionCode = customer.ImportSectionAdapterCode;
            if (!String.IsNullOrEmpty(sectionCode) && ItemsSections.Any(r => r.Name == sectionCode))
                SelectedSections = ItemsSections.FirstOrDefault(r => r.Name == sectionCode);
            else
                SelectedSections = ItemsSections.FirstOrDefault(r => r.IsDefault);

            string pdaCode = customer.ImportPDAProviderCode;
            if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
                SelectedPDA = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
            else
                SelectedPDA = ItemsPDA.FirstOrDefault(r => r.IsDefault);

            if (this._extraSettingsViewModel != null)
            {
                this._extraSettingsViewModel.SetCustomer(this._customer);
            }

            _isEventFromCode = false;
        }

        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
            this._branch = branch;

            _isEventFromCode = true;

            SetSelectedAdapterStateForBranch(mode);

            _isEventFromCode = false;
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
            this._inventor = inventor;

            _isEventFromCode = true;

            SetSelectedAdapterStateForInventor(mode);

            _isEventFromCode = false;
        }

        public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
        {
            IImportModuleInfo info = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);            
            IImportModuleInfo infoItur = ItemsIturs.FirstOrDefault(r => r.IsDefault);
            IImportModuleInfo infoLocation = ItemsLocations.FirstOrDefault(r => r.IsDefault);
            IImportModuleInfo infoSection = ItemsSections.FirstOrDefault(r => r.IsDefault);
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

                string pdaCode = _branch.ImportPDAProviderCode;
                if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
                    infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
            }

            SelectedCatalogs = info;
            SelectedIturs = infoItur;
            SelectedLocations = infoLocation;
            SelectedSections = infoSection;
            SelectedPDA = infoPda;
        }


        public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
        {
            IImportModuleInfo info = ItemsCatalogs.FirstOrDefault(r => r.IsDefault);            
            IImportModuleInfo infoItur = ItemsIturs.FirstOrDefault(r => r.IsDefault);
            IImportModuleInfo infoLocation = ItemsLocations.FirstOrDefault(r => r.IsDefault);
            IImportModuleInfo infoSection = ItemsSections.FirstOrDefault(r => r.IsDefault);
            IImportModuleInfo infoPda = ItemsPDA.FirstOrDefault(r => r.IsDefault);

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

                string pdaCode = _inventor.ImportPDAProviderCode;
                if (!String.IsNullOrEmpty(pdaCode) && ItemsPDA.Any(r => r.Name == pdaCode))
                    infoPda = ItemsPDA.FirstOrDefault(r => r.Name == pdaCode);
            }

            SelectedCatalogs = info;
            SelectedIturs = infoItur;
            SelectedLocations = infoLocation;
            SelectedSections = infoSection;
            SelectedPDA = infoPda;
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

                this._customer.ImportPDAProviderCode = this.SelectedPDA == null ?
                                                       String.Empty :
                                                       this.SelectedPDA.Name;
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

                this._inventor.ImportPDAProviderCode = this.SelectedPDA == null ?
                                                     String.Empty :
                                                     this.SelectedPDA.Name;
            }

            if (this._extraSettingsViewModel != null && this._customer != null)
            {
                this._extraSettingsViewModel.ApplyChanges(this._customer);
            }
        }

        private void ImportExportAdapterchanged(ImportExportAdapterChangedEventPayload payload)
        {
            if (payload.ExportModule == null) //event from self
                return;

            string importAdapterName = String.Empty;

            switch (payload.ExportModule.Name)
            {
                case ExportErpAdapterName.ExportErpComaxAdapter:
                    importAdapterName = ImportAdapterName.ImportCatalogComaxASPAdapter;
                    break;
                case ExportErpAdapterName.ExportErpGazitAdapter:
                    importAdapterName = ImportAdapterName.ImportCatalogGazitVerifoneAdapter;
                    break;
                case ExportErpAdapterName.ExportErpPosSuperPharmAdapter:
                    importAdapterName = ImportAdapterName.ImportCatalogPosSuperPharmAdapter;
                    break;
                case ExportErpAdapterName.ExportErpPriorityRenuarAdapter:
                    importAdapterName = ImportAdapterName.ImportCatalogPriorityRenuarAdapter;
                    break;
                case ExportErpAdapterName.ExportErpUnizagAdapter:
                    importAdapterName = ImportAdapterName.ImportCatalogUnizagAdapter;
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(importAdapterName) && _itemsCatalogs.Any(r => r.Name == importAdapterName))
            {
                _selectedCatalogs = _itemsCatalogs.FirstOrDefault(r => r.Name == importAdapterName);
                RaisePropertyChanged(() => SelectedCatalogs);
            }
        }
    }
}