using System;
using System.Linq;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Common.UserSettings;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Count4U.Modules.ContextCBI.Views.Misc.CBITab;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerEditOptionsViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly InventorFormViewModel _inventorFormViewModel;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;

		public readonly List<IAuditConfigRepository> _auditConfigsRepository;
		public readonly IAuditConfigRepository _historyCBIConfigRepository;
		private readonly IDBSettings _dbSettings; 
		private  readonly IUserSettingsManager _userSettingsManager;


        private ImportFoldersViewModel _importFoldersViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;
		private ExportPdaSettingsViewModel _exportPdaSettingsViewModel;
		private AdditionalSettingsSettingsViewModel _additionalSettingsSettingsViewModel;
		private InventorChangeStatusViewModel _inventorChangeStatusViewModel;
		
		private ConfigAdapterSettingViewModel _configAdapterSettingViewModel;
		

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private bool _isReadOnly;
        //private Inventor _inventor;
		private Customer _customer;

		public CustomerEditOptionsViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
			IServiceLocator serviceLocator,
            ICustomerRepository customerRepository,
            IContextCBIRepository contextCBIRepository,
			IDBSettings dbSettings,
			IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
			this._userSettingsManager = userSettingsManager;
			this._dbSettings = dbSettings;
			this._customerRepository = customerRepository;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

			this._okCommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);

			this._auditConfigsRepository = new List<IAuditConfigRepository>(serviceLocator.GetAllInstances<IAuditConfigRepository>());
			if (this._auditConfigsRepository.Count < 1) throw new ArgumentNullException("auditConfigsRepository");

			this._historyCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.History];		 //1

			//this._customerFormViewModel.PropertyChanged += this._customerFormViewModel_PropertyChanged;
        }

		//void _customerFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//	this._okCommand.RaiseCanExecuteChanged();
		//}


		private bool OkCommandCanExecute()
		{
			bool isExportFormOk = true;
			if (this._exportPdaSettingsViewModel != null)
				isExportFormOk = this._exportPdaSettingsViewModel.IsFormValid();

			return isExportFormOk;
		}


        public bool IsAdd
        {
            get { return false; }
        }

        public bool IsEdit
        {
            get { return true; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Update; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/inventor_edit.png"; }
        }

        public string Title
        {
			get { return Localization.Resources.ViewModel_CustomerEdit_EditCustomer; }
        }

    
        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

    
        public ImportFoldersViewModel ImportFoldersViewModel
        {
            get { return this._importFoldersViewModel; }
        }


		public ConfigAdapterSettingViewModel ConfigAdapterSettingViewModel
        {
			get { return this._configAdapterSettingViewModel; }
        }

        public bool IsReadOnly
        {
            get { return this._isReadOnly; }
            set
            {
                this._isReadOnly = value;
                this.RaisePropertyChanged(() => this.IsReadOnly);
            }
        }

    
        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this.IsReadOnly = false; //navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly);
			this._customer = base.CurrentCustomer;
			//switch (this.Context)
			//{
			//	case CBIContext.CreateInventor:
			//		this._customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
			//		break;
			//	case CBIContext.History:
			//		throw new NotImplementedException();
			//	case CBIContext.Main:
			//		this._customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
			//		break;
			//	default:
			//		throw new ArgumentOutOfRangeException();
			//}
	
			//this._inventorFormViewModel.InventorSet(this._customer, this.IsReadOnly, false, this.Context);

            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderCustomerEdit, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
				this._importFoldersViewModel.SetIsEditable(true);
				this._importFoldersViewModel.SetIsShowConfig(true);
				this._importFoldersViewModel.SetExtraSettingsDissable(false);
				this._importFoldersViewModel.SetCustomer(this._customer);
            }

			this._configAdapterSettingViewModel = Utils.GetViewModelFromRegion<ConfigAdapterSettingViewModel>(Common.RegionNames.ConfigAdapterSettingView, this._regionManager);
			if (this._configAdapterSettingViewModel != null)
            {
				this._configAdapterSettingViewModel.SetIsEditable(true);
				//this._configAdapterSettingViewModel.SetIsShowConfig(true);
				this._configAdapterSettingViewModel.SetCustomer(this._customer);
            }

			this._exportPdaSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsViewModel>(Common.RegionNames.ExportPdaSettingsCustomerEdit, this._regionManager);
			if (this._exportPdaSettingsViewModel != null)
			{
				this._exportPdaSettingsViewModel.SetIsEditable(true);
				this._exportPdaSettingsViewModel.SetIsShowConfig(true);
				this._exportPdaSettingsViewModel.SetCustomer(this._customer);
				this._exportPdaSettingsViewModel.CheckValidation += ExportPdaSettingsViewModel_CheckValidation;

			}

			this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsCustomerEdit, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsShowConfig(true);
				this._exportErpSettingsViewModel.SetCustomer(this._customer);
				this._exportErpSettingsViewModel.SetIsEditable(true);
			}

			this._updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderCustomerEdit, this._regionManager);
			if (this._updateViewModel != null)
			{
				this._updateViewModel.SetIsShowConfig(true);
				this._updateViewModel.SetIsEditable(true);
				this._updateViewModel.SetCustomer(this._customer);
			}


			this._dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsCustomerEdit, this._regionManager);
            if (this._dynamicColumnsViewModel != null)
            {
				//this._dynamicColumnsViewModel.SetIsShowConfig(true);
				this._dynamicColumnsViewModel.SetIsEditable(true);
				this._dynamicColumnsViewModel.SetIsCustomer(true);
				
				this._dynamicColumnsViewModel.SetCustomer(_customer);
			}

			//AdditionalSettingsCustomerEdit
			//private AdditionalSettingsSettingsViewModel _additionalSettingsSettingsViewModel;
			this._additionalSettingsSettingsViewModel = Utils.GetViewModelFromRegion<AdditionalSettingsSettingsViewModel>(Common.RegionNames.AdditionalSettingsCustomerEdit, this._regionManager);
			if (this._additionalSettingsSettingsViewModel != null)
			{
				this._additionalSettingsSettingsViewModel.SetIsEditable(true);
				this._additionalSettingsSettingsViewModel.SetCustomer(this._customer);
			}

			//InventorChangeStatusViewModel _inventorChangeStatusViewModel;
			this._inventorChangeStatusViewModel = Utils.GetViewModelFromRegion<InventorChangeStatusViewModel>(Common.RegionNames.AutoGenerateResultSettingsView, this._regionManager);
			if (this._inventorChangeStatusViewModel != null)
			{
				this._inventorChangeStatusViewModel.SetIsEditable(true);
				this._inventorChangeStatusViewModel.SetCustomer(this._customer);
			}

        }

		void ExportPdaSettingsViewModel_CheckValidation(object sender, EventArgs e)
		{
			this._okCommand.RaiseCanExecuteChanged();
		}

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

			if (this._importFoldersViewModel != null)
			{
				_importFoldersViewModel.OnNavigatedFrom(navigationContext);
			}

			if (this._configAdapterSettingViewModel != null)
			{
				_configAdapterSettingViewModel.OnNavigatedFrom(navigationContext);
			}

			if (this._exportErpSettingsViewModel != null)
			{
				_exportErpSettingsViewModel.OnNavigatedFrom(navigationContext);
			}
			if (this._updateViewModel != null)
			{
				_updateViewModel.OnNavigatedFrom(navigationContext);
			}
			if (this._dynamicColumnsViewModel != null)
			{
				_dynamicColumnsViewModel.OnNavigatedFrom(navigationContext);
			}

			if (this._exportPdaSettingsViewModel != null)
			{
				this._exportPdaSettingsViewModel.CheckValidation -= ExportPdaSettingsViewModel_CheckValidation;
				this._exportPdaSettingsViewModel.OnNavigatedFrom(navigationContext);
			}
		
			if (this._additionalSettingsSettingsViewModel != null)
			{
				_additionalSettingsSettingsViewModel.OnNavigatedFrom(navigationContext);
			}



			if (this._inventorChangeStatusViewModel != null)
			{
				_inventorChangeStatusViewModel.OnNavigatedFrom(navigationContext);
			}
        }

        #endregion

        private void OkCommandExecuted()
        {
			SaveCustomer();
        }

		private void SaveCustomer()
		{
			using (new CursorWait())
			{
				UtilsNavigate.DataFileMissed(this._customer, base.ContextCBIRepository, this._dbSettings, null, this._userSettingsManager);

				if (this._exportPdaSettingsViewModel != null)
				{
					this._exportPdaSettingsViewModel.ApplyChanges();
				}

				if (this._importFoldersViewModel != null)
				{
					this._importFoldersViewModel.ApplyChanges();
				}

				if (this._exportErpSettingsViewModel != null)
				{
					this._exportErpSettingsViewModel.ApplyChanges();
				}

				if (_updateViewModel != null)
				{
					this._updateViewModel.ApplyChanges();
				}

				if (_dynamicColumnsViewModel != null)
				{
					this._dynamicColumnsViewModel.ApplyChangesNonDynColumns();
					this._dynamicColumnsViewModel.ApplyChanges();
				}

				base.ContextCBIRepository.Update(this._customer);

				this._historyCBIConfigRepository.UpdateCustomerNameByCode(this._customer.Code, this._customer.Name, CBIContext.History);                                                                         


				this._eventAggregator.GetEvent<CustomerEditedEvent>().Publish(this._customer);
				this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
			}
		}


        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }


    }
}