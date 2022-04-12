using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface;
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
using Count4U.Model.Audit;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchEditViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceLocator _serviceLocator;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelComand;
        public readonly List<IAuditConfigRepository> _auditConfigsRepository;
        public readonly IAuditConfigRepository _historyCBIConfigRepository;
        private readonly BranchFormViewModel _branchFormViewModel;
        private readonly IDBSettings _dbSettings;

        private Branch _branch;
        private ImportFoldersViewModel _importFoldersViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnSettingsViewModel;

        public BranchEditViewModel(
            IServiceLocator serviceLocator,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            BranchFormViewModel branchFormViewModel,
            IContextCBIRepository contextCBIRepository,
            IDBSettings dbSettings,
            IUserSettingsManager userSettingsManager,
            ICustomerRepository customerRepository
            )
            : base(contextCBIRepository)
        {
            _customerRepository = customerRepository;
            _userSettingsManager = userSettingsManager;
            _dbSettings = dbSettings;
            _serviceLocator = serviceLocator;
            _branchFormViewModel = branchFormViewModel;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _auditConfigsRepository = new List<IAuditConfigRepository>(
            _serviceLocator.GetAllInstances<IAuditConfigRepository>());
            if (this._auditConfigsRepository.Count < 1) throw new ArgumentNullException("auditConfigsRepository");

            _historyCBIConfigRepository = this._auditConfigsRepository[(int)CBIContext.History];		 //1

            _okCommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
            _cancelComand = new DelegateCommand(this.CancelCommandExecuted);

            _branchFormViewModel.PropertyChanged += this._branchFormViewModel_PropertyChanged;
        }

        void _branchFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this._okCommand.RaiseCanExecuteChanged();
        }

        #region public properties

        public bool IsAdd
        {
            get { return false; }
        }

        public bool IsEdit
        {
            get { return true; }
        }

        public bool IsCustomerComboVisible
        {
            get { return false; }
        }

        public string Title
        {
            get { return Localization.Resources.View_BranchEdit_tbTitle; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/branch_edit.png"; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Update; }
        }

        //dummy - to prevent binding errors
        public ObservableCollection<CustomerSimpleItemViewModel> Customers { get; set; }
        //dummy - to prevent binding errors
        public bool IsInheritFromCustomer { get; set; }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelComand; }
        }

        public BranchFormViewModel BranchFormVM
        {
            get { return this._branchFormViewModel; }
        }

        public new CustomerSimpleItemViewModel CurrentCustomer { get; set; }

        public Window Owner { get; set; }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            bool isReadOnly = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly);

            this._branch = null;

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    this._branch = base.ContextCBIRepository.GetCurrentBranch(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new NotImplementedException();
                case CBIContext.Main:
                    this._branch = base.ContextCBIRepository.GetCurrentBranch(this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.BranchFormVM.BranchSet(this._branch, isReadOnly, false, this.Context);

            Customer customer = _customerRepository.GetCustomerByCode(_branch.CustomerCode);
            this.CurrentCustomer = customer == null ? null : new CustomerSimpleItemViewModel(customer);

            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderBranchEdit, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
                this._importFoldersViewModel.SetIsEditable(!isReadOnly);
				this._importFoldersViewModel.SetIsShowConfig(false);
                this._importFoldersViewModel.SetBranch(this._branch, enBranchAdapterInherit.InheritNothing);
            }

            this._updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderBranchEdit, this._regionManager);
            if (this._updateViewModel != null)
            {
				this._updateViewModel.SetIsEditable(!isReadOnly);
				this._updateViewModel.SetIsShowConfig(false);
				this._updateViewModel.SetBranch(_branch, enBranchAdapterInherit.InheritNothing);
            }

            this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsBranchEdit, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
                this._exportErpSettingsViewModel.SetBranch(this._branch, enBranchAdapterInherit.InheritNothing);
                this._exportErpSettingsViewModel.IsEditable = !isReadOnly;
            }

            this._dynamicColumnSettingsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsBranchEdit, this._regionManager);
            if (this._dynamicColumnSettingsViewModel != null)
            {
				//this._dynamicColumnSettingsViewModel.SetIsShowConfig(false);
                _dynamicColumnSettingsViewModel.SetIsEditable(true);
				_dynamicColumnSettingsViewModel.SetIsCustomer(false);
                _dynamicColumnSettingsViewModel.SetBranch(_branch, enBranchAdapterInherit.InheritNothing);
            }
        }

        private bool OkCommandCanExecute()
        {
            return this._branchFormViewModel.IsFormValid();
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            UtilsNavigate.DataFileMissed(this._branch, base.ContextCBIRepository, this._dbSettings, this.Owner, this._userSettingsManager);
            SaveBranch();
        }

        private void SaveBranch()
        {
            this.BranchFormVM.ApplyChanges();
			
			if (this._importFoldersViewModel != null)
			{
				this._importFoldersViewModel.ApplyChanges();
			}
			if (this._updateViewModel != null)
			{
				this._updateViewModel.ApplyChanges();
			}

			if (this._exportErpSettingsViewModel != null)
			{
				this._exportErpSettingsViewModel.ApplyChanges();
			}

			if (this._dynamicColumnSettingsViewModel != null)
			{
				this._dynamicColumnSettingsViewModel.ApplyChangesNonDynColumns();
				this._dynamicColumnSettingsViewModel.ApplyChanges();
			}

            base.ContextCBIRepository.Update(this._branch);
            this._historyCBIConfigRepository.UpdateBranchNameByCode(this._branch.Code, this._branch.Name, CBIContext.History);

            this._eventAggregator.GetEvent<BranchEditedEvent>().Publish(this._branch);
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

    }
}