using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model;
using System.ComponentModel;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchAddViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;

        private Branch _branch;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly BranchFormViewModel _branchFormViewModel;
        private ImportFoldersViewModel _importFoldersViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnSettingsViewModel;

        private bool _isCustomerComboVisible;

        private bool _isInheritFromCustomer;

        private readonly ObservableCollection<CustomerSimpleItemViewModel> _customers;
        private CustomerSimpleItemViewModel _currentCustomer;

        public BranchAddViewModel(
            IRegionManager regionManager,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            BranchFormViewModel branchFormViewModel,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository
            )
            : base(contextCBIRepository)
        {
            this._navigationRepository = navigationRepository;
            this._userSettingsManager = userSettingsManager;
            this._branchFormViewModel = branchFormViewModel;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);

            this._branchFormViewModel.PropertyChanged += this.BranchFormViewModel_PropertyChanged;

            this._isInheritFromCustomer = true;

            _customers = new ObservableCollection<CustomerSimpleItemViewModel>();
        }

        #region public properties

        public bool IsAdd
        {
            get { return true; }
        }

        public bool IsEdit
        {
            get { return false; }
        }

        public ObservableCollection<CustomerSimpleItemViewModel> Customers
        {
            get { return _customers; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Create; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/branch_add.png"; }
        }

        public string Title
        {
            get { return Localization.Resources.View_BranchAdd_tbCreateNewBranch; }
        }

        public new CustomerSimpleItemViewModel CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
                this.RaisePropertyChanged(() => this.CurrentCustomer);

                using (CursorWait wait = new CursorWait())
                {
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            base.ContextCBIRepository.SetCurrentCustomer(_currentCustomer.Customer, this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            throw new NotImplementedException();
                        case CBIContext.Main:
                            base.ContextCBIRepository.SetCurrentCustomer(_currentCustomer.Customer, this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

			    
                    this._okCommand.RaiseCanExecuteChanged();

                    this._branchFormViewModel.InvalidateCopyCommand();

                    this._branchFormViewModel.AssignNewCodeAccordingToCustomer();
					if (this._importFoldersViewModel != null)
					{
						this._importFoldersViewModel.ResetCurrentCustomer();
					}

					if (this._updateViewModel != null)
					{
						this._updateViewModel.ResetCurrentCustomer();
					}

					if (this._exportErpSettingsViewModel != null)
					{
						this._exportErpSettingsViewModel.ResetCurrentCustomer();
					}

					if (this._dynamicColumnSettingsViewModel != null)
					{
						this._dynamicColumnSettingsViewModel.ResetCurrentCustomer();
					}

					if (this._importFoldersViewModel != null)
					{
						this._importFoldersViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
					}

					if (this._updateViewModel != null)
					{
						this._updateViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
					}

					if (this._exportErpSettingsViewModel != null)
					{
						this._exportErpSettingsViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
					}

					if (this._dynamicColumnSettingsViewModel != null)
					{
						this._dynamicColumnSettingsViewModel.SetStateForBranch(this.CalculateAdapterInheritMode());
					}
                }
            }
        }

        public bool IsCustomerComboVisible
        {
            get { return this._isCustomerComboVisible; }
            set
            {
                this._isCustomerComboVisible = value;
                this.RaisePropertyChanged(() => this.IsCustomerComboVisible);
            }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public BranchFormViewModel BranchFormVM
        {
            get { return this._branchFormViewModel; }
        }

        public bool IsInheritFromCustomer
        {
            get { return _isInheritFromCustomer; }
            set
            {
                _isInheritFromCustomer = value;
                RaisePropertyChanged(() => IsInheritFromCustomer);

				if (this._importFoldersViewModel != null)
				{
					this._importFoldersViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
				}

				if (this._updateViewModel != null)
				{
					this._updateViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
				}

				if (this._exportErpSettingsViewModel != null)
				{
					this._exportErpSettingsViewModel.SetSelectedAdapterStateForBranch(this.CalculateAdapterInheritMode());
				}

				if (this._dynamicColumnSettingsViewModel != null)
				{
					this._dynamicColumnSettingsViewModel.SetStateForBranch(this.CalculateAdapterInheritMode());
				}
            }
        }

        #endregion

        void BranchFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this._okCommand.RaiseCanExecuteChanged();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			Utils.AddDbContextToQuery(navigationContext.Parameters, NavigationSettings.CBIDbContextBranch);
			
            this._branch = InitBranch();

            this._branchFormViewModel.BranchSet(_branch, false, true, this.Context);

            BuildCustomerList();

            Customer customer;
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new NotImplementedException();

                case CBIContext.Main:
                    customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentCustomer = customer == null ? null : _customers.FirstOrDefault(r => r.Customer.Code == customer.Code);

            this._isCustomerComboVisible = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.IsCustomerComboVisible);


            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderBranchAdd, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
                this._importFoldersViewModel.SetIsEditable(true);
				this._importFoldersViewModel.SetIsShowConfig(false);
                this._importFoldersViewModel.SetBranch(_branch, this.CalculateAdapterInheritMode());
            }


            this._updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderBranchAdd, this._regionManager);
            if (this._updateViewModel != null)
            {
				this._updateViewModel.SetIsEditable(true);
				this._updateViewModel.SetIsShowConfig(false);
				this._updateViewModel.SetBranch(_branch, CalculateAdapterInheritMode());
            }

            this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsBranchAdd, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
                this._exportErpSettingsViewModel.SetBranch(this._branch, CalculateAdapterInheritMode());
                this._exportErpSettingsViewModel.IsEditable = true;
            }

            this._dynamicColumnSettingsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsBranchAdd, this._regionManager);
            if (this._dynamicColumnSettingsViewModel != null)
            {
				//this._dynamicColumnSettingsViewModel.SetIsShowConfig(false);
                _dynamicColumnSettingsViewModel.SetIsEditable(true);
				_dynamicColumnSettingsViewModel.SetIsCustomer(false);
                _dynamicColumnSettingsViewModel.SetBranch(_branch, CalculateAdapterInheritMode());
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

			if (this._dynamicColumnSettingsViewModel != null)
			{
				_dynamicColumnSettingsViewModel.OnNavigatedFrom(navigationContext);
			}
        }

        private Branch InitBranch()
        {
            Branch result = new Branch();
            result.ID = 0;

            return result;
        }

        private bool OkCommandCanExecute()
        {
            if (IsCurrentCustomerNotNull() && _branchFormViewModel.IsFormValid())
                return true;

            return false;
        }

        private bool IsCurrentCustomerNotNull()
        {
            if (this.CurrentCustomer == null) return false;
            else return true;
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            this.SaveBranch();

            this._eventAggregator.GetEvent<BranchAddedEvent>().Publish(this._branch);

            BranchPostData data = new BranchPostData() { BranchCode = _branch.Code, IsNew = true };
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, this.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, data, Common.NavigationObjects.BranchPost);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.BranchPostView + query, UriKind.Relative));
        }

        private void SaveBranch()
        {
            try
            {
                _branchFormViewModel.ApplyChanges();
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

				if (this._dynamicColumnSettingsViewModel != null)
				{
					_dynamicColumnSettingsViewModel.ApplyChangesNonDynColumns();
				}
                
                switch (this.Context)
                {
                    case CBIContext.CreateInventor:
                        base.ContextCBIRepository.CreateContextBranch(this._branch, this.GetCreateAuditConfig(), true, this._isInheritFromCustomer);
                        break;
                    case CBIContext.Main:
                        base.ContextCBIRepository.CreateContextBranch(this._branch, this.GetMainAuditConfig(), true, this._isInheritFromCustomer);
                        break;
                    case CBIContext.History:
                        throw new InvalidOperationException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

				if (this._dynamicColumnSettingsViewModel != null)
				{
					_dynamicColumnSettingsViewModel.ApplyChanges();
				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("SaveBranch", exc);
            }
        }

        private enBranchAdapterInherit CalculateAdapterInheritMode()
        {
            return this._isInheritFromCustomer ? enBranchAdapterInherit.InheritFromCustomer : enBranchAdapterInherit.InheritNothing;
        }

        private void BuildCustomerList()
        {
            _customers.Clear();
            Customers customers = base.ContextCBIRepository.GetContextCustomers(this.GetMainAuditConfig(), CBIContext.Main);
            if (customers != null)
            {
                foreach (Customer customer in customers)
                {
                    _customers.Add(new CustomerSimpleItemViewModel(customer));
                }
            }
        }
    }
}