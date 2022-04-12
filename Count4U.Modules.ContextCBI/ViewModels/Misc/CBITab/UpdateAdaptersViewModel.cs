using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
    public class UpdateAdaptersViewModel : CBIContextBaseViewModel
    {
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly ICustomerRepository _customerRepository;

        private bool _isEditable;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        private IImportModuleInfo _selectedUpdateCatalog;
        private ObservableCollection<IImportModuleInfo> _itemsUpdateCatalog;
		private bool _configFileImportUpdateCatalogExists;

		private bool _isShowConfig;
		private readonly UICommandRepository<IImportModuleInfo> _commandRepositoryObject;
		private readonly DelegateCommand<IImportModuleInfo> _showConfigCommand;


        public UpdateAdaptersViewModel(IUnityContainer container,
            IContextCBIRepository contextCbiRepository,
            IImportAdapterRepository importAdapterRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
			UICommandRepository<IImportModuleInfo> commandRepositoryObject,
            ICustomerRepository customerRepository)
            : base(contextCbiRepository)
        {
			this._customerRepository = customerRepository;
			this._container = container;
			this._regionManager = regionManager;
			this._eventAggregator = eventAggregator;
			this._importAdapterRepository = importAdapterRepository;
			this._commandRepositoryObject = commandRepositoryObject;
			this._isEditable = true;

			this._itemsUpdateCatalog = new ObservableCollection<IImportModuleInfo>();
			this._isShowConfig = true;
			this._showConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowConfigCommandCanExecuted);
			
        }

		public DelegateCommand<IImportModuleInfo> ShowConfigCommand
		{
			get { return this._showConfigCommand; }
		}

	
		public bool ShowConfigCommandCanExecuted(IImportModuleInfo info)
		{
			if (this.SelectedUpdateCatalog == null) return false;
			bool configFileExists = base.IsConfigFileImportExists(this.SelectedUpdateCatalog.Name);
			return configFileExists;
		}

		public bool ConfigFileImportUpdateCatalogExists
		{
			get
			{

				return this._configFileImportUpdateCatalogExists;
			}
			set { this._configFileImportUpdateCatalogExists = value; }
		}

		//public bool IsConfigFileImportExists(IImportModuleInfo importModuleInfo)
		//{
		//	if (importModuleInfo == null) return false;
		//	string adapterName = importModuleInfo.Name;
		//	if (adapterName == Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter) return false;
		//	if (string.IsNullOrWhiteSpace(adapterName) == true) return false;

		//	// ?? Пока договорились что конфиг всегда берется из Customer
		//	string adapterConfigFileName = @"\" + adapterName + ".config";
		//	string configPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;
		//	if (File.Exists(configPath) == true)
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		private void ShowConfigCommandExecuted(IImportModuleInfo importModuleInfo)
		{
			// this.SelectedAdapter = importModuleInfo;
			// if (importModuleInfo != null)
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { UpdateModule = importModuleInfo });

		}

		public void GotNewFofusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { UpdateModule = this._selectedUpdateCatalog });
			}
		}

		public void LostFocusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { UpdateModule = null });
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


        public IImportModuleInfo SelectedUpdateCatalog
        {
            get { return _selectedUpdateCatalog; }
            set
            {
                _selectedUpdateCatalog = value;
                RaisePropertyChanged(() => SelectedUpdateCatalog);
				this._showConfigCommand.RaiseCanExecuteChanged();

				if (_selectedUpdateCatalog != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { UpdateModule = this._selectedUpdateCatalog });

            }
        }

        public ObservableCollection<IImportModuleInfo> ItemsUpdateCatalog
        {
            get { return _itemsUpdateCatalog; }
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterChanged);

            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            this._itemsUpdateCatalog = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this._container, this._importAdapterRepository, ImportDomainEnum.UpdateCatalog,
                currentCustomerCode, currentBranchCode, currentInventorCode));

			
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);
        }
       
        public void SetIsEditable(bool isEditable)
        {
            this.IsEditable = isEditable;
        }

        public void SetCustomer(Customer customer)
        {
            this._customer = customer;

			this._configFileImportUpdateCatalogExists = false;
            if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
                SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
            else
                SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.IsDefault);

			if (this.SelectedUpdateCatalog != null)	this.ConfigFileImportUpdateCatalogExists = base.IsConfigFileImportExists(this.SelectedUpdateCatalog.Name);
			this._showConfigCommand.RaiseCanExecuteChanged();
        }

        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
            this._branch = branch;

            SetSelectedAdapterStateForBranch(mode);
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
            this._inventor = inventor;

            SetSelectedAdapterStateForInventor(mode);

        }

        public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
        {
            IImportModuleInfo selected = _itemsUpdateCatalog.FirstOrDefault(r => r.IsDefault);

            Customer customer = this.CurrentCustomer;

            if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
            {
                if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
                    selected = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
            }
            else
            {
                if (!String.IsNullOrEmpty(_branch.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == _branch.UpdateCatalogAdapterCode))
                    selected = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == _branch.UpdateCatalogAdapterCode);
            }


            SelectedUpdateCatalog = selected;
        }

        public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
        {
            IImportModuleInfo selected = _itemsUpdateCatalog.FirstOrDefault(r => r.IsDefault);

            Customer customer = this.CurrentCustomer;
            Branch branch = this.CurrentBranch;

            if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
            {
                if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
                    selected = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
            }
            else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
            {
                if (!String.IsNullOrEmpty(branch.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == branch.UpdateCatalogAdapterCode))
                    selected = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == branch.UpdateCatalogAdapterCode);
            }
            else
            {
                if (!String.IsNullOrEmpty(_inventor.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == _inventor.UpdateCatalogAdapterCode))
                    selected = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == _inventor.UpdateCatalogAdapterCode);
            }


            SelectedUpdateCatalog = selected;
        }

        public void ApplyChanges()
        {
            if (this._customer != null)
            {
                this._customer.UpdateCatalogAdapterCode = this.SelectedUpdateCatalog == null ?
                                                            String.Empty :
                                                            this.SelectedUpdateCatalog.Name;
            }

            if (this._branch != null)
            {
                this._branch.UpdateCatalogAdapterCode = this.SelectedUpdateCatalog == null ?
                                                          String.Empty :
                                                          this.SelectedUpdateCatalog.Name;
            }

            if (this._inventor != null)
            {
                this._inventor.UpdateCatalogAdapterCode = this.SelectedUpdateCatalog == null ?
                                                           String.Empty :
                                                           this.SelectedUpdateCatalog.Name;
            }
        }

        private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
        {
			if (payload == null) return;
			string updateAdapterName = "";

			if (payload.UpdateModule != null)
			{
				updateAdapterName = payload.UpdateModule.Name;
			}

			if (string.IsNullOrWhiteSpace(updateAdapterName) == true)
			{
				if (payload.ImportModule != null)
				{
					//string updateAdapterName = Common.Constants.UpdateCatalogAdapterName.GetUpdateByImportAdapterName(payload.ImportModule.Name);
					updateAdapterName = AdapterConnections.GetRelated(payload.ImportModule.Name, ImportDomainEnum.UpdateCatalog);
				}
			}

			//if (string.IsNullOrWhiteSpace(updateAdapterName) == true) return;
	
			if (SelectedUpdateCatalog == null)
			{
				if (!String.IsNullOrEmpty(updateAdapterName) || _itemsUpdateCatalog.Any(r => r.Name == updateAdapterName))
				{
					_selectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == updateAdapterName);
					RaisePropertyChanged(() => SelectedUpdateCatalog);
				}
				else
				{
					_selectedUpdateCatalog = null;
					RaisePropertyChanged(() => SelectedUpdateCatalog);
				}
			}
            
        }

    }
}