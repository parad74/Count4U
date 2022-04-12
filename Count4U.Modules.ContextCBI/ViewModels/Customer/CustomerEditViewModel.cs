using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using Count4U.Modules.ContextCBI.Views;
using System.Windows;
using Count4U.Model;
using Count4U.Modules.ContextCBI.Controllers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Model.Audit;
using Count4U.Common.Web;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerEditViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly CustomerFormViewModel _customerFormViewModel;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        public readonly List<IAuditConfigRepository> _auditConfigsRepository;
        public readonly IAuditConfigRepository _historyCBIConfigRepository;
        private readonly IDBSettings _dbSettings;

        private bool _isReadOnly;
        private Customer _customer;
        private ImportFoldersViewModel _importFoldersViewModel;
        private ExportPdaSettingsViewModel _exportPdaSettingsViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;

        #region Constructor
        public CustomerEditViewModel(IEventAggregator eventAggregator,
            IServiceLocator serviceLocator,
			IRegionManager regionManager, 
			CustomerFormViewModel customerFormViewModel, 
			IContextCBIRepository contextCBIRepository,
            IDBSettings dbSettings,
            IUserSettingsManager userSettingsManager
         )
             : base(contextCBIRepository)
        {
            this._userSettingsManager = userSettingsManager;
			this._dbSettings = dbSettings;
            this._customerFormViewModel = customerFormViewModel;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

			this._okCommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);

            this._auditConfigsRepository = new List<IAuditConfigRepository>(serviceLocator.GetAllInstances<IAuditConfigRepository>());
            if (this._auditConfigsRepository.Count < 1) throw new ArgumentNullException("auditConfigsRepository");

            this._historyCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.History];		 //1

			this._customerFormViewModel.PropertyChanged += this._customerFormViewModel_PropertyChanged;
        }

        void _customerFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {          
			this._okCommand.RaiseCanExecuteChanged();
        }

        private bool OkCommandCanExecute()
        {
            bool isExportFormOk = true;
            if (this._exportPdaSettingsViewModel != null)
                isExportFormOk = this._exportPdaSettingsViewModel.IsFormValid();

            return this._customerFormViewModel.IsFormValid() && isExportFormOk;
        }

        #endregion

        public string Title
        {
            get { return Localization.Resources.ViewModel_CustomerEdit_EditCustomer; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Update; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/customer_edit.png"; }
        }      

        public CustomerFormViewModel CustomerFormVM
        {
            get { return this._customerFormViewModel; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
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

        public Window Owner { get; set; }

        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            bool isWizard = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly);
            this.IsReadOnly = isWizard;

            this._customer = null;
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    this._customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new NotImplementedException();
                case CBIContext.Main:
                    this._customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this._customerFormViewModel.CustomerSet(this._customer, this.IsReadOnly, false);

            int branchesCount = 0;
            Branches branches = base.ContextCBIRepository.GetContextBranches(this.GetAuditConfigByCurrentContext(), Context);
            if (branches != null) branchesCount = branches.Count;

            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderCustomerEdit, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
                this._importFoldersViewModel.SetIsEditable(!_isReadOnly);
				this._importFoldersViewModel.SetIsShowConfig(false);
                this._importFoldersViewModel.SetCustomer(this._customer);
            }

            this._exportPdaSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsViewModel>(Common.RegionNames.ExportPdaSettingsCustomerEdit, this._regionManager);
            if (this._exportPdaSettingsViewModel != null)
            {
                this._exportPdaSettingsViewModel.SetCustomer(this._customer);
				this._exportPdaSettingsViewModel.SetIsShowConfig(false);
                this._exportPdaSettingsViewModel.SetIsEditable(!this._isReadOnly);
				this._exportPdaSettingsViewModel.SetIsNew(false);              

                this._exportPdaSettingsViewModel.CheckValidation += ExportPdaSettingsViewModel_CheckValidation;
            }

            this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsCustomerEdit, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
                this._exportErpSettingsViewModel.SetCustomer(this._customer);
                this._exportErpSettingsViewModel.SetIsEditable(!this._isReadOnly);
            }

            _updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderCustomerEdit, this._regionManager);
            if (_updateViewModel != null)
            {
				this._updateViewModel.SetIsShowConfig(false);
                this._updateViewModel.IsEditable = !this._isReadOnly;
                this._updateViewModel.SetCustomer(this._customer);                
            }

            _dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsCustomerEdit, this._regionManager);
			if (_dynamicColumnsViewModel != null)
            {
				//this._dynamicColumnsViewModel.SetIsShowConfig(false);
				this._dynamicColumnsViewModel.SetIsEditable(true);
				this._dynamicColumnsViewModel.SetIsCustomer(true);
				this._dynamicColumnsViewModel.SetCustomer(_customer);
            }

		
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

			if (this._exportPdaSettingsViewModel != null)
			{
				this._exportPdaSettingsViewModel.CheckValidation -= ExportPdaSettingsViewModel_CheckValidation;
				this._exportPdaSettingsViewModel.OnNavigatedFrom(navigationContext);
			}

			if (this._exportErpSettingsViewModel != null)
			{
				this._exportErpSettingsViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_importFoldersViewModel != null)
			{
				this._importFoldersViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_updateViewModel != null)
			{
				this._updateViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_dynamicColumnsViewModel != null)
			{
				this._dynamicColumnsViewModel.OnNavigatedFrom(navigationContext);
			}
        }

        void ExportPdaSettingsViewModel_CheckValidation(object sender, EventArgs e)
        {
            this._okCommand.RaiseCanExecuteChanged();
        }

        #endregion

        private void OkCommandExecuted()
        {
            using (new CursorWait())
            {
                UtilsNavigate.DataFileMissed(this._customer, base.ContextCBIRepository, this._dbSettings, this.Owner, this._userSettingsManager);

				if (this._customerFormViewModel != null)
				{
					this._customerFormViewModel.ApplyChanges();
                    FtpCommandResult ftpCommandResult = _customerFormViewModel.CheckFtpAfterApplyChanges();
                    if (ftpCommandResult.Successful != SuccessfulEnum.Successful) 
                   {
                        this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
                        return;
                    }
                }

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
