using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Script;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using NLog;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.Constants;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerChooseViewModel : ContextCBIChooseViewModelCommon
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ReportButtonViewModel _reportButton;

        private readonly DelegateCommand<CustomerItemViewModel> _doubleClickCommand;

        private readonly DelegateCommand _chooseCloseCommand;        
        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _deleteWithoutChildCommand;
        private readonly DelegateCommand _repairCommand;
        private readonly UICommand _reportCommand;
        private readonly UICommand _searchCommand;
        private readonly DelegateCommand<CustomerItemViewModel> _editSelectedCommand;
        private readonly DelegateCommand<CustomerItemViewModel> _deleteSelectedCommand;
        private readonly DelegateCommand<CustomerItemViewModel> _viewCommand;
        private readonly DelegateCommand<CustomerItemViewModel> _childSelectedCommand;
        private readonly UICommandRepository<CustomerItemViewModel> _commandRepositoryObject;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private CustomerItemViewModel _current;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        private readonly ObservableCollection<CustomerItemViewModel> _list;

        private CustomerFilterData _filter;

        private List<CustomerItemViewModel> _selectedMulti;

        public CustomerChooseViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository,
            ReportButtonViewModel reportButton,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            UICommandRepository commandRepository,
            UICommandRepository<CustomerItemViewModel> commandRepositoryObject)
            : base(contextCBIRepository, eventAggregator, regionManager, navigationRepository, commandRepository)
        {
            _customerRepository = customerRepository;
            _branchRepository = branchRepository;
            this._serviceLocator = serviceLocator;
            this._commandRepositoryObject = commandRepositoryObject;
            this._reportButton = reportButton;
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator.GetEvent<CustomerAddedEvent>().Subscribe(this.Added);
            this._eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(this.Edited);
            this._eventAggregator.GetEvent<CustomerSelectedEvent>().Subscribe(this.SelectedEventHandler);

            this._chooseCloseCommand = new DelegateCommand(this.ChooseCloseCommandExecuted, this.ChooseCloseCommandCanExecute);

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._deleteWithoutChildCommand = _commandRepository.Build(enUICommand.DeleteWithoutChild, DeleteWithoutChildCommandExecuted, DeleteWithoutChildCommandCanExecute);
            this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, this.EditSelectedCommandExecuted);
            this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, this.DeleteSelectedCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, delegate { });
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });

            this._viewCommand = _commandRepositoryObject.Build(enUICommand.View, ViewCommandExecuted);

            this._list = new ObservableCollection<CustomerItemViewModel>();

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._doubleClickCommand = new DelegateCommand<CustomerItemViewModel>(DoubleClickCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
            this._childSelectedCommand = _commandRepositoryObject.Build(enUICommand.ChildrenForCustomer, ChildSelectedCommandExecuted);
        }        

        #region public properties

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get
            {
                return this._yesNoRequest;
            }
        }

        public ObservableCollection<CustomerItemViewModel> List
        {
            get
            {
                return this._list;
            }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => this.PageCurrent);

                BuildList();
            }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);
            }
        }

        public DelegateCommand ChooseCloseCommand
        {
            get { return this._chooseCloseCommand; }
        }

        public CustomerItemViewModel Current
        {
            get { return _current; }
            set
            {
                this._current = value;
                this.RaisePropertyChanged(() => this.Current);
                this._chooseCloseCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand<CustomerItemViewModel> EditSelectedCommand
        {
            get { return this._editSelectedCommand; }
        }

        public DelegateCommand<CustomerItemViewModel> DeleteSelectedCommand
        {
            get { return this._deleteSelectedCommand; }
        }

        public DelegateCommand<CustomerItemViewModel> ViewCommand
        {
            get { return this._viewCommand; }
        }

        public DelegateCommand<CustomerItemViewModel> DoubleClickCommand
        {
            get { return _doubleClickCommand; }
        }

        protected override enCBIScriptMode CBIScriptMode { get { return enCBIScriptMode.Customer; } }

        protected override SelectParams SelectParams
        {
            get
            {
                var sp = this.BuildSelectParams();
                sp.IsEnablePaging = false;
                return sp;
            }
        }

        public DelegateCommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public ReportButtonViewModel ReportButton
        {
            get { return _reportButton; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public DelegateCommand<CustomerItemViewModel> ChildSelectedCommand
        {
            get { return _childSelectedCommand; }
        }

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
        }

        public CustomerFilterData Filter
        {
            get { return _filter; }
        }

        public UICommand ReportCommand
        {
            get { return _reportCommand; }
        }

        public UICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public DelegateCommand DeleteWithoutChildCommand
        {
            get { return _deleteWithoutChildCommand; }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterCustomer);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionCBIGet();

            this._reportButton.OnNavigatedTo(navigationContext);
            this._reportButton.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = BuildSelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Customer);

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, base._navigationRepository, Common.NavigationObjects.Filter, true) as CustomerFilterData;
            if (_filter == null)
                _filter = new CustomerFilterData();

            BuildList();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._reportButton.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<CustomerAddedEvent>().Unsubscribe(this.Added);
            this._eventAggregator.GetEvent<CustomerEditedEvent>().Unsubscribe(this.Edited);
            this._eventAggregator.GetEvent<CustomerSelectedEvent>().Unsubscribe(this.SelectedEventHandler);
            _eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterCustomer);
        }

        private SelectParams BuildSelectParams()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;

            if (_filter != null)
            {
                _filter.ApplyToSelectParams(result);
            }

            return result;
        }

        void BuildList()
        {
            this._list.Clear();
            this.ItemsTotal = 0;

            SelectParams selectParams = null;

            try
            {
                selectParams = BuildSelectParams();
                AuditConfig auditConfig = null;

                switch (this.Context)
                {
                    case CBIContext.CreateInventor:
                        auditConfig = this.GetMainAuditConfig();
                        break;
                    case CBIContext.History:
                        auditConfig = this.GetHistoryAuditConfig();

                        break;
                    case CBIContext.Main:
                        auditConfig = this.GetMainAuditConfig();

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Customers customers = base.ContextCBIRepository.GetContextCustomers(selectParams, auditConfig);

                if (customers == null) return;

                foreach (var customer in customers)
                {
                    this._list.Add(new CustomerItemViewModel(customer, base.ContextCBIRepository));
                }

                this.ItemsTotal = (int)customers.TotalCount;

                if ((customers.TotalCount > 0)
                    && (customers.Count == 0))	//do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildList", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                if (selectParams != null)
                    _logger.Error("SelectParams: {0}", selectParams.ToString());
                throw;
            }
        }

        private bool ChooseCloseCommandCanExecute()
        {
            if (this._current == null) return false;
            else return true;
        }

        private void ChooseCloseCommandExecuted()
        {
            //don't used in CreateInventor context
            this.CustomerSelected(this._current.Customer);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                case CBIContext.History:
                case CBIContext.Main:
                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.CustomerDashboardOpen(this.Context, this._regionManager, query);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CustomerSelected(Customer customer)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetCreateAuditConfig());
                    this.CustomerSelectedHandler(customer);
                    break;
                case CBIContext.History:
                    base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetHistoryAuditConfig());
                    this.CustomerSelectedHandler(customer);
                    break;
                case CBIContext.Main:
                    base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetMainAuditConfig());
                    this.CustomerSelectedHandler(customer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectedEventHandler(CustomerSelectedEventPayload customer)
        {
            if (customer.Context == this.Context)
                this.CustomerSelectedHandler(customer.Customer);
        }

        private void CustomerSelectedHandler(Customer customer)
        {
            this.CustomerCurrentUpdate(customer);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    break;
                case CBIContext.History:

                    break;
                case CBIContext.Main:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddCommandExecuted()
        {
            this._eventAggregator.GetEvent<CustomerAddEvent>().Publish(new CustomerAddEventPayload() { Context = this.Context });
        }

        private void Added(Customer customer)
        {
            if (customer == null) return;
            BuildList();
            this.CustomerCurrentUpdate(customer);
        }

        private void EditSelectedCommandExecuted(CustomerItemViewModel customer)
        {
            this.CustomerSelected(customer.Customer);
            this._eventAggregator.GetEvent<CustomerEditEvent>().Publish(
                new CustomerEditEventPayload() { Customer = customer.Customer, Context = this.Context });
        }

        private void Edited(Customer customer)
        {
            CustomerItemViewModel listCustomer = this._list.FirstOrDefault(
                r => r.Customer.Code == customer.Code);

            if (listCustomer != null) listCustomer.UpdateViewModelViewCustomer(customer);
        }

        private void CustomerCurrentUpdate(Customer customer)
        {
            if ((customer == null)
                || (this._list == null)) return;

            this.Current = this._list.FirstOrDefault
                (r => r.Customer.Code == customer.Code);
        }

        private void DeleteSelectedCommandExecuted(CustomerItemViewModel customerItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    throw new InvalidOperationException();
                case CBIContext.History:
                case CBIContext.Main:
                    string message = String.Format(Localization.Resources.Msg_Delete_Customer,
                        customerItemViewModel.Name);
                    if (this._yesNoRequest != null)
                    {
                        this._yesNoRequest.Raise(new MessageBoxYesNoNotification() { Content = message, Settings = this._userSettingsManager }, r =>
                        {
                            if (r.IsYes == true)
                            {

                                base.ContextCBIRepository.Delete(customerItemViewModel.Customer);
                                BuildList();
                            }
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ViewCommandExecuted(CustomerItemViewModel customerItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentCustomer(
                        customerItemViewModel.Customer, this.GetCreateAuditConfig());
                    this._eventAggregator.GetEvent<CustomerViewEvent>().Publish(
                        new CustomerViewEventPayload()
                            {
                                Customer = customerItemViewModel.Customer,
                                Context = base.Context
                            });
                    break;
                case CBIContext.History:
                case CBIContext.Main:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoubleClickCommandExecuted(CustomerItemViewModel customerItemViewModel)
        {
            if (_chooseCloseCommand.CanExecute())
                _chooseCloseCommand.Execute();
        }

        // восстановление домайн объектов Customer в БД MainDB, 
		// по файловой системе (файлов Count4UDB.sdf и AnalyticDB.sdf в папке Customer)
        // если есть домайн объект в БД - он не заменяется
        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
                ////base.ContextCBIRepository.RefillAllCBIInventorConfigs();
                //List<string> count4UDBPathList = new List<string>();
                //IContextCBIRepository contextCBIRepository = this._serviceLocator.GetInstance<IContextCBIRepository>();
                //IDBSettings dbSettings = this._serviceLocator.GetInstance<IDBSettings>();
                //ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
                IInventorConfigRepository inventorConfigRepository = this._serviceLocator.GetInstance<IInventorConfigRepository>();

                /////test  ret = inventorConfigRepository.IsCustomerCount4UDB(@"Customer\\CustomerCode1");

                inventorConfigRepository.RepairDomainCustomer();
                IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
                alterAdoProvider.UpdateCount4UDBViaScript();
				alterAdoProvider.UpdateAnalyticDBViaScript();
                //восстановить версии всех CountDB
				UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                this.BuildList();


            }
        }


        // восстановление (добавление или изменение) домайн объектов Customer в БД MainDB, 
        // по файловой системе (файлов Count4UDB.sdf в папке Customer)
        // если есть домайн объект в БД - он не заменяется
        private void RepairCommandExecuted(string pathDB, bool updateByInventorConfig = false)
        {
            //if (pathDB.Contains("removed") == true) return;
            //IContextCBIRepository contextCBIRepository = this._serviceLocator.GetInstance<IContextCBIRepository>();
            //IDBSettings dbSettings = this._serviceLocator.GetInstance<IDBSettings>();
            //ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();

            IInventorConfigRepository inventorConfigRepository = this._serviceLocator.GetInstance<IInventorConfigRepository>();
            inventorConfigRepository.RepairDomainCustomer(pathDB, updateByInventorConfig);
            //// надо достать релейтив DBPath а потом реботать через репозиторий

            //Customers customers = new Customers();
            //string count4UDBPath = contextCBIRepository.FindCount4UDBPathDB(dbSettings.FolderCustomer.Trim('\\'));
            //if (string.IsNullOrWhiteSpace(count4UDBPath) == true) return;

            //Customer customer = inventorConfigRepository.CustomerFromInventorConfigs(pathDB);
            //if (customer != null) customers.Add(customer);
            //if (updateByInventorConfig == false)
            //{
            //    //добавление домайн объектов Customer в БД MainDB
            //    customerRepository.InsertDomainСustomerFromInventorConfig(customers);
            //}
            //else
            //{
            //    // Update Customer в БД MainDB по InventorConfig взятому из БД Count4U, по заданному пути  pathDB
            //    customerRepository.UpdateDomainСustomerByInventorConfig(customers);
            //}
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.Main);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextCustomer);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Customer);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.BuildSelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private bool EditCommandCanExecute()
        {
            return _current != null && (_selectedMulti == null || _selectedMulti.Count == 1);
        }

        private void EditCommandExecuted()
        {
            if (_current != null)
            {
                _editSelectedCommand.Execute(_current);
            }
        }

        private bool DeleteCommandCanExecute()
        {
            return _current != null && (_selectedMulti == null || _selectedMulti.Count == 1);
        }

        private void DeleteCommandExecuted()
        {
            if (_current != null)
            {
                _deleteSelectedCommand.Execute(_current);
            }
        }

        private void ChildSelectedCommandExecuted(CustomerItemViewModel customerItemViewModel)
        {
            this.CustomerSelected(customerItemViewModel.Customer);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                case CBIContext.History:
                case CBIContext.Main:
                    UriQuery query = new UriQuery();

                    BranchFilterData filter = new BranchFilterData();
                    filter.CustomerCode = customerItemViewModel.Customer.Code;
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filter, Common.NavigationObjects.Filter);

                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.BranchChooseOpen(this.Context, this._regionManager, query);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FilterCustomer(IFilterData customerFilterData)
        {
            this._filter = customerFilterData as CustomerFilterData;

            RaisePropertyChanged(() => IsFilterAnyField);

            BuildList();
        }

        private bool DeleteWithoutChildCommandCanExecute()
        {
            return _selectedMulti != null && _selectedMulti.Any();
        }

        private void DeleteWithoutChildCommandExecuted()
        {
            Branches branches = _branchRepository.GetBranches();

            bool isWithChild = _selectedMulti.Any(r => branches.Any(z => z.CustomerCode == r.Customer.Code));

            if (isWithChild)
            {
                UtilsMisc.ShowMessageBox(Localization.Resources.ViewModel_CustomerChoose_msgDeleteWithoutChild, MessageBoxButton.OK, MessageBoxImage.Warning, _userSettingsManager);
            }

            using (new CursorWait())
            {
                _customerRepository.Delete(_selectedMulti.Select(r => r.Customer.Code).ToList());

                BuildList();
            }
        }

        public void SetSelected(List<CustomerItemViewModel> list)
        {
            _selectedMulti = list;

            _editCommand.RaiseCanExecuteChanged();
            _deleteCommand.RaiseCanExecuteChanged();
            _deleteWithoutChildCommand.RaiseCanExecuteChanged();
        }
    }
}