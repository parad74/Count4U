using System;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using NLog;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Main;
using Count4U.Common.UserSettings;
using Count4U.Common.Web;

namespace Count4U.Modules.ContextCBI.ViewModels
{
	public class InventorAddViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;
        private readonly INavigationRepository _navigationRepository;
		private readonly IBranchRepository _branchRepository;
      	private readonly IUserSettingsManager _userSettingsManager;


        private readonly IInventorRepository _inventorRepository;
        private readonly Inventor _inventor;

        private readonly DelegateCommand _okcommand;
        private readonly DelegateCommand _cancelCommand;

        private readonly InventorFormViewModel _inventorFormViewModel;
        private ImportFoldersViewModel _importFoldersViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;

        private bool _isCustomerComboVisible;
        private bool _isBranchComboVisible;
		private bool _withoutNavigate;
		

        private bool _inheritCustomer;
        private bool _inheritBranch;
        private bool _inheritNothing;

        private string _filterCustomer;
        private string _filterBranch;

        private readonly ObservableCollection<CustomerSimpleItemViewModel> _customerList;
        private readonly ObservableCollection<BranchSimpleItemViewModel> _branchList;

        private CustomerSimpleItemViewModel _currentCustomer;
        private BranchSimpleItemViewModel _currentBranch;
		private IServiceLocator _serviceLocator;

        public InventorAddViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            InventorFormViewModel inventorFormViewModel,
			IBranchRepository branchRepository,
            IUnityContainer unityContainer,
            INavigationRepository navigationRepository ,
            IUserSettingsManager userSettingsManager ,
			IServiceLocator serviceLocator,
            IInventorRepository inventorRepository
            )
            : base(contextCBIRepository)
        {
            this._navigationRepository = navigationRepository;
            this._unityContainer = unityContainer;
            this._inventorFormViewModel = inventorFormViewModel;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
			this._branchRepository = branchRepository;
            this._userSettingsManager = userSettingsManager;
            this._inventorRepository = inventorRepository;



            this._okcommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);

            this._inventor = new Inventor();
            this._inventor.ID = 0;
            this._inventor.Code = Utils.CodeNewGenerate();
            this._inventor.CreateDate = DateTime.Now;
			this._inventor.LastUpdatedCatalog = DateTime.Now;
            this._inventor.CompleteDate = DateTime.Now;
            this._inventor.InventorDate = DateTime.Now;

            this._inventorFormViewModel.PropertyChanged += this.InventorFormViewModel_PropertyChanged;

            this._inheritCustomer = true;

            _customerList = new ObservableCollection<CustomerSimpleItemViewModel>();
            _branchList = new ObservableCollection<BranchSimpleItemViewModel>();
        }

        public bool IsAdd
        {
            get { return true; }
        }

        public bool IsEdit
        {
            get { return false; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Create; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/inventor_add.png"; }
        }

        public string Title
        {
            get { return Localization.Resources.View_InventorAdd_tbCreateNew; }
        }

        void InventorFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Code")
            {
                this._okcommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<CustomerSimpleItemViewModel> CustomerList
        {
            get { return _customerList; }
        }

        public new CustomerSimpleItemViewModel CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
                RaisePropertyChanged(() => CurrentCustomer);
                Customer customer = null;
                if (_currentCustomer != null)
                {
                    CustomerSimpleItemViewModel item = _customerList.FirstOrDefault(r => r.Customer.Code == _currentCustomer.Customer.Code);
                    if (item != null)
                    {
                        customer = item.Customer;
                    }
                }
                CustomerSelected(customer);
            }
        }

        public ObservableCollection<BranchSimpleItemViewModel> BranchList
        {
            get { return _branchList; }
        }

        public new BranchSimpleItemViewModel CurrentBranch
        {
            get
            {
                return _currentBranch;
            }
            set
            {
                _currentBranch = value;
                RaisePropertyChanged(() => CurrentBranch);
                Branch branch = null;
                if (_currentBranch != null)
                {
                    BranchSimpleItemViewModel item = _branchList.FirstOrDefault(r => r.Branch.Code == _currentBranch.Branch.Code);
                    if (item != null)
                    {
                        branch = item.Branch;
                    }
                }

                this.BranchSelected(branch);
            }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okcommand; }
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

        public bool IsCustomerComboVisible
        {
            get { return this._isCustomerComboVisible; }
            set
            {
                this._isCustomerComboVisible = value;
                this.RaisePropertyChanged(() => this.IsCustomerComboVisible);
            }
        }

        public bool IsBranchComboVisible
        {
            get { return this._isBranchComboVisible; }
            set
            {
                this._isBranchComboVisible = value;
                this.RaisePropertyChanged(() => this.IsBranchComboVisible);
            }
        }


		public bool WithoutNavigate
        {
			get { return this._withoutNavigate; }
            set
            {
				this._withoutNavigate = value;
				this.RaisePropertyChanged(() => this.WithoutNavigate);
            }
        }

        public bool InheritCustomer
        {
            get { return _inheritCustomer; }
            set
            {
                _inheritCustomer = value;
                RaisePropertyChanged(() => InheritCustomer);

                _inheritBranch = false;
                _inheritNothing = false;

                RaisePropertyChanged(() => InheritBranch);
                RaisePropertyChanged(() => InheritNothing);

				if (this._importFoldersViewModel != null)
				{
					this._importFoldersViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromCustomer);
				}
				if (this._updateViewModel != null)
				{
					this._updateViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromCustomer);
				}
				if (this._exportErpSettingsViewModel != null)
				{
					this._exportErpSettingsViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromCustomer);
				}
				if (this._dynamicColumnsViewModel != null)
				{
					this._dynamicColumnsViewModel.SetStateForInventor(enInventorAdapterInherit.InheritFromCustomer);
				}
            }
        }

        public bool InheritBranch
        {
            get { return _inheritBranch; }
            set
            {
                _inheritBranch = value;
                RaisePropertyChanged(() => InheritBranch);

                _inheritCustomer = false;
                _inheritNothing = false;

                RaisePropertyChanged(() => InheritCustomer);
                RaisePropertyChanged(() => InheritNothing);

				if (this._importFoldersViewModel != null)
				{
					this._importFoldersViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromBranch);
				}

				if (this._updateViewModel != null)
				{
					this._updateViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromBranch);
				}
				if (this._exportErpSettingsViewModel != null)
				{
					this._exportErpSettingsViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritFromBranch);
				}
				if (this._dynamicColumnsViewModel != null)
				{
					this._dynamicColumnsViewModel.SetStateForInventor(enInventorAdapterInherit.InheritFromBranch);
				}
            }
        }

        public bool InheritNothing
        {
            get { return _inheritNothing; }
            set
            {
                _inheritNothing = value;
                RaisePropertyChanged(() => InheritNothing);

                _inheritCustomer = false;
                _inheritBranch = false;

                RaisePropertyChanged(() => InheritCustomer);
                RaisePropertyChanged(() => InheritBranch);

				if (this._importFoldersViewModel != null)
				{
					this._importFoldersViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritNothing);
				}
				if (this._updateViewModel != null)
				{
					this._updateViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritNothing);
				}
				if (this._exportErpSettingsViewModel != null)
				{
					this._exportErpSettingsViewModel.SetSelectedAdapterStateForInventor(enInventorAdapterInherit.InheritNothing);
				}
				if (this._dynamicColumnsViewModel != null)
				{
					this._dynamicColumnsViewModel.SetStateForInventor(enInventorAdapterInherit.InheritNothing);
				}
            }
        }

        public string FilterCustomer
        {
            get { return _filterCustomer; }
            set
            {
                _filterCustomer = value;
                RaisePropertyChanged(() => FilterCustomer);

                BuildCustomerList();
            }
        }

        public string FilterBranch
        {
            get { return _filterBranch; }
            set
            {
                _filterBranch = value;
                RaisePropertyChanged(() => FilterBranch);
				string customerCode = "";
				if (this.CurrentCustomer != null && this.CurrentCustomer.Customer != null)
				{
					customerCode = this.CurrentCustomer.Customer.Code;
				}
				BuildBranchList(customerCode);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this.IsCustomerComboVisible = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.IsCustomerComboVisible);
            this.IsBranchComboVisible = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.IsBranchComboVisible);
			 this.WithoutNavigate = navigationContext.Parameters.Any(r => r.Key == NavigationSettings.WithoutNavigate);


            //current customer
            Customer customer;
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new InvalidOperationException();
                case CBIContext.Main:
                    customer = base.ContextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (customer != null)
            {
                 this.BuildCustomerList();
                _currentCustomer = _customerList.FirstOrDefault(r => r.Customer.Code == customer.Code);
				BuildBranchList(customer.Code);
            }
			else
			{
              this._filterCustomer = this._userSettingsManager.CustomerFilterCodeGet();
               this.BuildCustomerList();
			}

            //current branch
            Branch branch;
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    branch = base.ContextCBIRepository.GetCurrentBranch(this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new InvalidOperationException();
                case CBIContext.Main:
                    branch = base.ContextCBIRepository.GetCurrentBranch(this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

			if (branch != null && _branchList != null)
            {
                _currentBranch = _branchList.FirstOrDefault(r => r.Branch.Code == branch.Code);
            }

			//if (branch == null && customer != null && IsCustomerComboVisible == false) //opened in add mode from customer dashboard
			//{
			//	BuildBranchList();
			//}

			this._inventorFormViewModel.InventorSet(customer, this._inventor, false, true, this.Context);

            this._importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderInventorAdd, this._regionManager);
            if (this._importFoldersViewModel != null)
            {
                this._importFoldersViewModel.SetIsEditable(true);
				this._importFoldersViewModel.SetIsShowConfig(false);
                this._importFoldersViewModel.SetInventor(_inventor, CalculateAdapterInheritMode());
            }

            this._updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderInventorAdd, this._regionManager);
            if (this._updateViewModel != null)
            {
				this._updateViewModel.SetIsEditable(true);
				this._updateViewModel.SetIsShowConfig(false);
				this._updateViewModel.SetInventor(_inventor, CalculateAdapterInheritMode());
            }

            this._exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsInventorAdd, this._regionManager);
            if (this._exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.IsEditable = true;
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
				this._exportErpSettingsViewModel.SetInventor(_inventor, CalculateAdapterInheritMode());
            }

            this._dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsInventorAdd, this._regionManager);
            if (this._dynamicColumnsViewModel != null)
            {
				this._dynamicColumnsViewModel.SetIsEditable(true);
				_dynamicColumnsViewModel.SetIsCustomer(false);
				//this._dynamicColumnsViewModel.SetIsShowConfig(false);
				this._dynamicColumnsViewModel.SetInventor(_inventor, CalculateAdapterInheritMode());
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
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

        private void CustomerSelected(Customer customer)
        {
            switch (base.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new InvalidOperationException();
                case CBIContext.Main:
                    base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

			string customerCode = "";
			if (customer != null)
			{
				customerCode = customer.Code;
				BuildBranchList(customerCode);
			}
			

            CurrentBranch = null;
            this._okcommand.RaiseCanExecuteChanged();
			this._inventorFormViewModel.UpdateAskProfileFromCustomer(customer);

            this._eventAggregator.GetEvent<CustomerSelectedEvent>().Publish(
                new CustomerSelectedEventPayload() { Customer = customer, Context = this.Context });
            this._eventAggregator.GetEvent<BranchSelectedEvent>().Publish(
                new BranchSelectedEventPayload() { Branch = null, Context = this.Context });

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
			if (this._dynamicColumnsViewModel != null)
			{
				this._dynamicColumnsViewModel.ResetCurrentCustomer();
			}

			if (this._importFoldersViewModel != null)
			{
				this._importFoldersViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._updateViewModel != null)
			{
				this._updateViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._exportErpSettingsViewModel != null)
			{
				this._exportErpSettingsViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._dynamicColumnsViewModel != null)
			{
				this._dynamicColumnsViewModel.SetStateForInventor(CalculateAdapterInheritMode());
			}
        }

        private void BranchSelected(Branch branch)
        {
            switch (base.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentBranch(branch, this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    throw new InvalidOperationException();
                case CBIContext.Main:
                    base.ContextCBIRepository.SetCurrentBranch(branch, this.GetMainAuditConfig());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this._okcommand.RaiseCanExecuteChanged();
            this._eventAggregator.GetEvent<BranchSelectedEvent>().Publish(
                new BranchSelectedEventPayload() { Branch = branch, Context = this.Context });
			
			if (this._importFoldersViewModel != null)
			{
				this._importFoldersViewModel.ResetCurrentCustomer();
				this._importFoldersViewModel.ResetCurrentBranch();
			}
			if (this._updateViewModel != null)
			{
				this._updateViewModel.ResetCurrentCustomer();
				this._updateViewModel.ResetCurrentBranch();
			}
			if (this._exportErpSettingsViewModel != null)
			{
				this._exportErpSettingsViewModel.ResetCurrentCustomer();
				this._exportErpSettingsViewModel.ResetCurrentBranch();
			}
			if (this._dynamicColumnsViewModel != null)
			{
				this._dynamicColumnsViewModel.ResetCurrentCustomer();
				this._dynamicColumnsViewModel.ResetCurrentBranch();
			}

			if (this._importFoldersViewModel != null)
			{
				this._importFoldersViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._updateViewModel != null)
			{
				this._updateViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._exportErpSettingsViewModel != null)
			{
				this._exportErpSettingsViewModel.SetSelectedAdapterStateForInventor(CalculateAdapterInheritMode());
			}
			if (this._dynamicColumnsViewModel != null)
			{
				this._dynamicColumnsViewModel.SetStateForInventor(CalculateAdapterInheritMode());
			}
        }

        private enInventorAdapterInherit CalculateAdapterInheritMode()
        {
            if (this._inheritCustomer) return enInventorAdapterInherit.InheritFromCustomer;
            if (this._inheritBranch) return enInventorAdapterInherit.InheritFromBranch;

            return enInventorAdapterInherit.InheritNothing;
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            if ((this.CurrentCustomer != null)
                && (this.CurrentBranch != null)
                && (this._inventorFormViewModel.IsCodeUnique() == true)) return true;

            return false;
        }

        private void OkCommandExecuted()
        {
            try
            {
                FtpCommandResult ftpCommandResult = SaveInventor();

                this._eventAggregator.GetEvent<InventorAddedEvent>().Publish(this._inventor);

		
				NavigateAfterSave(ftpCommandResult);
			
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }
        }

        private FtpCommandResult SaveInventor()
        {
            FtpCommandResult ftpCommandResult = new FtpCommandResult();
            ftpCommandResult.Successful = SuccessfulEnum.Successful;
            try
            {
                switch (base.Context)
                {
                    case CBIContext.CreateInventor:
                    case CBIContext.History:
                    case CBIContext.Main:
                        {
                            this._inventor.CreateDate = DateTime.Now;
                            this._inventor.CompleteDate = DateTime.Now;
							this._inventor.LastUpdatedCatalog = DateTime.Now;
                            if (this._inventorFormViewModel != null)
                            {
                                AuditConfig auditConfig1 = this._inventorFormViewModel.ApplyChanges();
                                ftpCommandResult = _inventorFormViewModel.CheckFtpAfterApplyChanges(auditConfig1);
                                if (ftpCommandResult.Successful != SuccessfulEnum.Successful)
                                {
                                    return ftpCommandResult;
                                }
								else
								{
                                    this._contextCBIRepository.SaveCurrentCBIConfig(this._inventorFormViewModel._context, auditConfig1);
                                }
                            }

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
							if (this._dynamicColumnsViewModel != null)
							{
								this._dynamicColumnsViewModel.ApplyChangesNonDynColumns();
							}
                            object domainObject = null;
                            if (this._inheritCustomer) domainObject = this.CurrentCustomer.Customer;
                            if (this._inheritBranch) domainObject = this.CurrentBranch.Branch;

                            AuditConfig auditConfig = base.GetAuditConfigByCurrentContext();
                            string newCode = Utils.CodeNewGenerate();
                            auditConfig.Code = newCode;
                            auditConfig.Description = this._inventor.Description;
                            auditConfig.BranchCode = this.CurrentBranch.Branch.Code;
                            auditConfig.BranchName = this.CurrentBranch.Branch.Name;
                            auditConfig.CustomerCode = this.CurrentCustomer.Customer.Code;
                            auditConfig.CustomerName = this.CurrentCustomer.Customer.Name;
                            auditConfig.InventorCode = this._inventor.Code;
                            auditConfig.InventorName = this._inventor.Name;
                            auditConfig.InventorDate = this._inventor.InventorDate;

                            auditConfig.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();
                            auditConfig.StatusInventorCode = StatusInventorEnum.New.ToString();
                            base.ContextCBIRepository.CreateContextInventor(this._inventor, auditConfig, true, domainObject);
							if (this._dynamicColumnsViewModel != null)
							{
								this._dynamicColumnsViewModel.ApplyChanges();
							}

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exc)
            {
                ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
                ftpCommandResult.Error = "SaveInventor : with error" + exc.Message;
                _logger.ErrorException("SaveInventor", exc);
            }
            return ftpCommandResult;
        }

        private void NavigateAfterSave(FtpCommandResult ftpCommandResult)
        {
            try
            {
                using (new CursorWait())
                {
                    AuditConfig auditConfig = base.GetAuditConfigByCurrentContext();
                    Inventor testInventor = this._inventorRepository.GetInventorByCode(_inventor.Code);
                    if (testInventor != null)
                    {
                        base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, auditConfig);

                        //	this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaMISAdapterViewModel), ExportPdaAdapterName.ExportPdaMISAdapter);
                        ExportPdaModuleBaseViewModel exportPda = _serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(ExportPdaAdapterName.ExportPdaMISAdapter);

                        exportPda.ClearFolders(base.State);

                        base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, auditConfig);

                        Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);
                    }

                    InventorPostData data = new InventorPostData() { InventorCode = _inventor.Code, IsNew = true, ftpCommandResult = ftpCommandResult };
                    UriQuery query = new UriQuery();
                    Utils.AddContextToQuery(query, this.Context);
                    Utils.AddDbContextToQuery(query, base.CBIDbContext);
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, data, Common.NavigationObjects.InventorPost);

					if (WithoutNavigate == false)
					{
						_regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.InventorPostView + query, UriKind.Relative));
					}
					else
					{
						_eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
					}
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("NavigateAfterSave", exc);
            }
        }

        private void BuildCustomerList()
        {
            _customerList.Clear();

            Customers result;
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    result = base.ContextCBIRepository.GetContextCustomers(this.GetCreateAuditConfig(), CBIContext.Main);
                    break;
                case CBIContext.History:
                    throw new InvalidOperationException();
                case CBIContext.Main:
                    result = base.ContextCBIRepository.GetContextCustomers(this.GetMainAuditConfig(), CBIContext.Main);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Customers customers = UtilsMisc.FilterCustomers(result, _filterCustomer);
            if (customers != null)
            {
                foreach (Customer customer in customers)
                {
                    _customerList.Add(new CustomerSimpleItemViewModel(customer));
                }
            }
        }

        private void BuildBranchList(string customerCode)
        {
            Branches result = new Branches();
			if (string.IsNullOrWhiteSpace(customerCode) ==  false)
			{
				result = _branchRepository.GetBranchesByCustomerCode(customerCode, CBIContext.Main);
			}

			//switch (this.Context)
			//{
			//	case CBIContext.CreateInventor:
			//		result = base.ContextCBIRepository.GetContextBranches(this.GetCreateAuditConfig(),
			//			CBIContext.Main);
			//		break;
			//	case CBIContext.History:
			//		throw new InvalidOperationException();
			//	case CBIContext.Main:
			//		result = base.ContextCBIRepository.GetContextBranches(this.GetMainAuditConfig(),
			//			CBIContext.Main);
			//		break;
			//	default:
			//		throw new ArgumentOutOfRangeException();
			//}


			_branchList.Clear();
            Branches branches = UtilsMisc.FilterBranches(result, _filterBranch);
            if (branches != null)
            {
                foreach (Branch branch in branches)
                {
                    _branchList.Add(new BranchSimpleItemViewModel(branch));
                }
            }
        }
    }
}