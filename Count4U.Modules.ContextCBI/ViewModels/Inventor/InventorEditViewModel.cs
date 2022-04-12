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

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorEditViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly InventorFormViewModel _inventorFormViewModel;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;

        private ImportFoldersViewModel _importFoldersViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private bool _isReadOnly;
        private Inventor _inventor;

        public InventorEditViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
            InventorFormViewModel inventorFormViewModel,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IContextCBIRepository contextCBIRepository)
            : base(contextCBIRepository)
        {
            _branchRepository = branchRepository;
            _customerRepository = customerRepository;
            this._inventorFormViewModel = inventorFormViewModel;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._okCommand = new DelegateCommand(this.OkCommandExecuted);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);
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
            get { return Localization.Resources.View_InventorEdit_tbEdit; }
        }

        public bool IsCustomerComboVisible
        {
            get { return false; }
        }

        public bool IsBranchComboVisible
        {
            get { return false; }
        }

        public string FilterCustomer { get; set; }
        public string FilterBranch { get; set; }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public InventorFormViewModel InventorFormVM
        {
            get { return this._inventorFormViewModel; }
        }

        public ImportFoldersViewModel ImportFoldersViewModel
        {
            get { return this._importFoldersViewModel; }
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

        public new CustomerSimpleItemViewModel CurrentCustomer { get; set; }
        public new BranchSimpleItemViewModel CurrentBranch { get; set; }

        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            this.IsReadOnly = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    this._inventor = base.ContextCBIRepository.GetCurrentInventor(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    this._inventor = base.ContextCBIRepository.GetCurrentInventor(this.GetHistoryAuditConfig());
                    break;
                case CBIContext.Main:
                    this._inventor = base.ContextCBIRepository.GetCurrentInventor(this.GetMainAuditConfig());

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this._inventor == null) return;

            Customer customer = _customerRepository.GetCustomerByCode(_inventor.CustomerCode);
            Branch branch = _branchRepository.GetBranchByCode(_inventor.BranchCode);
            this.CurrentCustomer = customer == null ? null : new CustomerSimpleItemViewModel(customer);
            this.CurrentBranch = branch == null ? null : new BranchSimpleItemViewModel(branch);

			this._inventorFormViewModel.InventorSet(customer, this._inventor, this.IsReadOnly, false, this.Context);

            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderInventorEdit, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
                this._importFoldersViewModel.SetIsEditable(!_isReadOnly);
				this._importFoldersViewModel.SetIsShowConfig(false);
                this._importFoldersViewModel.SetInventor(this._inventor, enInventorAdapterInherit.InheritNothing);
            }

            this._updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderInventorEdit, this._regionManager);
            if (this._updateViewModel != null)
            {
				this._updateViewModel.SetIsEditable(!_isReadOnly);
				this._updateViewModel.SetIsShowConfig(false);
				this._updateViewModel.SetInventor(_inventor, enInventorAdapterInherit.InheritNothing);
            }

            this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsInventorEdit, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
				this._exportErpSettingsViewModel.IsEditable = !_isReadOnly;
				this._exportErpSettingsViewModel.SetInventor(_inventor, enInventorAdapterInherit.InheritNothing);
            }

            this._dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsInventorEdit, this._regionManager);
            if (this._dynamicColumnsViewModel != null)
            {
				//this._dynamicColumnsViewModel.SetIsShowConfig(false);
				this._dynamicColumnsViewModel.SetIsEditable(true);
				_dynamicColumnsViewModel.SetIsCustomer(false);
				this._dynamicColumnsViewModel.SetInventor(_inventor, enInventorAdapterInherit.InheritNothing);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

			if (this._importFoldersViewModel != null)
			{
				_importFoldersViewModel.OnNavigatedFrom(navigationContext);
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
        }

        #endregion

        private void OkCommandExecuted()
        {
            SaveInventor();
        }

        private void SaveInventor()
        {
            if (this._inventorFormViewModel != null)
            {
                AuditConfig auditConfig = _inventorFormViewModel.ApplyChanges();
                this._contextCBIRepository.SaveCurrentCBIConfig(this._inventorFormViewModel._context, auditConfig);
            }
			if (this._importFoldersViewModel != null)
			{
				_importFoldersViewModel.ApplyChanges();
			}
			if (this._updateViewModel != null)
			{
				_updateViewModel.ApplyChanges();
			}
			if (this._exportErpSettingsViewModel != null)
			{
				_exportErpSettingsViewModel.ApplyChanges();
			}

			if (this._dynamicColumnsViewModel != null)
			{
				_dynamicColumnsViewModel.ApplyChangesNonDynColumns();
				_dynamicColumnsViewModel.ApplyChanges();
			}

            base.ContextCBIRepository.Update(this._inventor);
            this._eventAggregator.GetEvent<InventorEditedEvent>().Publish(this._inventor);
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }


    }
}