using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.Constants;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchChooseViewModel : ContextCBIChooseViewModelCommon
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly UICommandRepository<BranchItemViewModel> _commandRepositoryObject;

        private readonly DelegateCommand _chooseCloseCommand;
        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand _deleteWithoutChildCommand;
        private readonly DelegateCommand _deleteAllWithoutChildCommand;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;

        private readonly DelegateCommand<BranchItemViewModel> _editSelectedCommand;
        private readonly DelegateCommand<BranchItemViewModel> _deleteSelectedCommand;
        private readonly DelegateCommand<BranchItemViewModel> _viewCommand;
        private readonly DelegateCommand<BranchItemViewModel> _openCustomerDashboardCommand;
        private readonly DelegateCommand<BranchItemViewModel> _doubleClickCommand;
        private readonly DelegateCommand<BranchItemViewModel> _childSelectedCommand;
        private readonly DelegateCommand _repairCommand;
        private readonly DelegateCommand _searchCommand;
        private readonly DelegateCommand _reportCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private BranchItemViewModel _current;
        private readonly ObservableCollection<BranchItemViewModel> _list;

        private InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private readonly ReportButtonViewModel _reportButton;

        private BranchFilterData _filter;

        private List<BranchItemViewModel> _selectedMulti;     

        public BranchChooseViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
            ReportButtonViewModel reportButton,
            UICommandRepository commandRepository,
            UICommandRepository<BranchItemViewModel> commandRepositoryObject)
            : base(contextCBIRepository, eventAggregator, regionManager, navigationRepository, commandRepository)
        {
            this._inventorRepository = inventorRepository;
            this._branchRepository = branchRepository;
            this._serviceLocator = serviceLocator;
            this._commandRepositoryObject = commandRepositoryObject;
            this._reportButton = reportButton;
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator.GetEvent<BranchAddedEvent>().Subscribe(this.Added);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Subscribe(this.Edited);
            this._eventAggregator.GetEvent<BranchSelectedEvent>().Subscribe(this.SelectedEventHandler);

            this._chooseCloseCommand = new DelegateCommand(this.ChooseCloseCommandExecuted, this.ChooseCloseCommandCanExecute);

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted, this.AddCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._deleteWithoutChildCommand = _commandRepository.Build(enUICommand.DeleteWithoutChild, DeleteWithoutChildCommandExecuted, DeleteWithoutChildCommandCanExecute);
            this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, this.EditSelectedCommandExecuted);
            this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, this.DeleteSelectedCommandExecuted);
            this._deleteAllWithoutChildCommand = _commandRepository.Build(enUICommand.DeleteAllWithoutChild, DeleteAllWithoutChildCommandExecuted, DeleteAllWithoutChildCommandCanExecute);
            this._viewCommand = new DelegateCommand<BranchItemViewModel>(this.ViewCommandExecuted);
            this._openCustomerDashboardCommand = new DelegateCommand<BranchItemViewModel>(OpenCustomerDashboardCommandExecuted);
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
            this._reportCommand = _commandRepository.Build(enUICommand.Report, delegate { });

            this._list = new ObservableCollection<BranchItemViewModel>();
            this._doubleClickCommand = new DelegateCommand<BranchItemViewModel>(DoubleClickCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
            this._childSelectedCommand = _commandRepositoryObject.Build(enUICommand.ChildrenForBranch, ChildSelectedCommandExecuted);         
        }     

        #region public properties

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get
            {
                this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
                return this._yesNoRequest;
            }
        }

        public ObservableCollection<BranchItemViewModel> List
        {
            get
            {
                return this._list;
            }
        }

        public BranchItemViewModel Current
        {
            get { return _current; }
            set
            {
                this._current = value;
                this.RaisePropertyChanged(() => this.Current);
                this._chooseCloseCommand.RaiseCanExecuteChanged();
                this._addCommand.RaiseCanExecuteChanged();
                _editCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
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

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand<BranchItemViewModel> EditSelectedCommand
        {
            get { return this._editSelectedCommand; }
        }

        public DelegateCommand<BranchItemViewModel> DeleteSelectedCommand
        {
            get { return this._deleteSelectedCommand; }
        }

        public DelegateCommand<BranchItemViewModel> ViewCommand
        {
            get { return this._viewCommand; }
        }

        public DelegateCommand<BranchItemViewModel> OpenCustomerDashboardCommand
        {
            get { return _openCustomerDashboardCommand; }
        }

        public DelegateCommand<BranchItemViewModel> DoubleClickCommand
        {
            get { return _doubleClickCommand; }
        }

        protected override enCBIScriptMode CBIScriptMode { get { return enCBIScriptMode.Branch; } }

        protected override SelectParams SelectParams
        {
            get
            {
                var sp = this.SelectParamsBuild();
                sp.IsEnablePaging = false;
                return sp;
            }
        }

        public ReportButtonViewModel ReportButton
        {
            get { return _reportButton; }
        }

        public DelegateCommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public DelegateCommand<BranchItemViewModel> ChildSelectedCommand
        {
            get { return _childSelectedCommand; }
        }

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
        }

        public BranchFilterData Filter
        {
            get { return _filter; }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public DelegateCommand ReportCommand
        {
            get { return _reportCommand; }
        }

        public DelegateCommand DeleteWithoutChildCommand
        {
            get { return _deleteWithoutChildCommand; }
        }

        public DelegateCommand DeleteAllWithoutChildCommand
        {
            get { return _deleteAllWithoutChildCommand; }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterBranch);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionCBIGet();

            this._reportButton.OnNavigatedTo(navigationContext);
            this._reportButton.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = SelectParamsBuild();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Branch);

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, base._navigationRepository, Common.NavigationObjects.Filter, true) as BranchFilterData;
            if (_filter == null)
                _filter = new BranchFilterData();

			Task.Factory.StartNew(this.BuildList).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._reportButton.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<BranchAddedEvent>().Unsubscribe(this.Added);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Unsubscribe(this.Edited);
            this._eventAggregator.GetEvent<BranchSelectedEvent>().Unsubscribe(this.SelectedEventHandler);
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterBranch);
        }

        private SelectParams SelectParamsBuild()
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

        private void BuildList()
        {
            this.ItemsTotal = 0;

            SelectParams selectParams = null;

            try
            {
                selectParams = this.SelectParamsBuild();

                Branches branches = null;

                branches = base.ContextCBIRepository.GetBranches(selectParams);

                if (branches == null) return;

                Dictionary<string, Customer> customersCache = new Dictionary<string, Customer>();
                List<BranchItemViewModel> toAdd = new List<BranchItemViewModel>();

                foreach (var branch in branches)
                {
                    string customerCode = branch.CustomerCode;
                    if (!customersCache.ContainsKey(customerCode))
                        customersCache.Add(branch.CustomerCode, base.ContextCBIRepository.GetCustomerByCode(customerCode));
                    Customer customer = customersCache[customerCode];

                    toAdd.Add(new BranchItemViewModel(branch, customer));
                }

                Utils.RunOnUI(() =>
                    {
                        this._list.Clear();
                        toAdd.ForEach(r => this._list.Add(r));
                        this.ItemsTotal = (int)branches.TotalCount;

                        if ((branches.TotalCount > 0) && (branches.Count == 0))  //do not show empty space - move on previous page
                        {
                            this.PageCurrent = this._pageCurrent - 1;
                        }

                        _deleteAllWithoutChildCommand.RaiseCanExecuteChanged();
                    });
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

        private void BranchSelected(Branch branch)
        {
            Customer customer = base.ContextCBIRepository.GetCustomerByCode(branch.CustomerCode);
            base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetAuditConfigByCurrentContext());

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentBranch(branch, this.GetCreateAuditConfig());
                    this.BranchSelectedHandler(branch);
                    break;
                case CBIContext.History:
                    base.ContextCBIRepository.SetCurrentBranch(branch, this.GetHistoryAuditConfig());
                    this.BranchSelectedHandler(branch);
                    break;
                case CBIContext.Main:
                    base.ContextCBIRepository.SetCurrentBranch(branch, this.GetMainAuditConfig());
                    this.BranchSelectedHandler(branch);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectedEventHandler(BranchSelectedEventPayload branch)
        {
            if (this.Context == branch.Context) this.BranchSelectedHandler(branch.Branch);
        }

        private void BranchSelectedHandler(Branch branch)
        {
            this.BranchCurrentUpdate(branch);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    break;
                case CBIContext.History:
                    this.RaisePropertyChanged(() => this.CurrentBranch);

                    break;
                case CBIContext.Main:
                    this.RaisePropertyChanged(() => this.CurrentBranch);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ChooseCloseCommandCanExecute()
        {
            return this._current != null;
        }

        private void ChooseCloseCommandExecuted()
        {
            //not used in CreateInventor context            
            this.BranchSelected(this._current.Branch);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                case CBIContext.History:
                case CBIContext.Main:
                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.BranchDashboardOpen(this.Context, this._regionManager, query);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Added(Branch branch)
        {
            BuildList();
            this.BranchCurrentUpdate(branch);
            this.RaisePropertyChanged(() => this.CurrentBranch);
        }

        private void BranchCurrentUpdate(Branch branch)
        {
            if (branch == null) return;
            this.Current = this._list.FirstOrDefault(r => r.Branch.Code == branch.Code);
        }

        private void AddCommandExecuted()
        {
            this._eventAggregator.GetEvent<BranchAddEvent>().Publish(new BranchAddEventPayload() { Context = this.Context, IsCustomerComboVisible = true });
        }

        private bool AddCommandCanExecute()
        {
            return true;
        }

        private void Edited(Branch branch)
        {
            BranchItemViewModel listBranch = this._list.FirstOrDefault(r => r.Branch.Code == branch.Code);
            if (listBranch != null)
            {
                listBranch.UpdateViewModelWithBranch(branch);
            }
        }

        private void EditSelectedCommandExecuted(BranchItemViewModel branch)
        {
            this.BranchSelected(branch.Branch);
            this._eventAggregator.GetEvent<BranchEditEvent>().Publish(new BranchEditEventPayload() { Branch = branch.Branch, Context = this.Context });
        }

        private void DeleteSelectedCommandExecuted(BranchItemViewModel branchItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    throw new InvalidOperationException();
                case CBIContext.History:
                case CBIContext.Main:
                    string message = String.Format(Localization.Resources.Msg_Delete_Branch, branchItemViewModel.Name);
                    this._yesNoRequest.Raise(new MessageBoxYesNoNotification() { Content = message, Settings = this._userSettingsManager }, r =>
                    {
                        if (r.IsYes)
                        {

                            base.ContextCBIRepository.Delete(branchItemViewModel.Branch);
                            BuildList();
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ViewCommandExecuted(BranchItemViewModel branchItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentBranch(branchItemViewModel.Branch, this.GetCreateAuditConfig());
                    this._eventAggregator.GetEvent<BranchViewEvent>()
                        .Publish(new BranchViewEventPayload() { Branch = branchItemViewModel.Branch, Context = base.Context });
                    break;
                case CBIContext.History:
                case CBIContext.Main:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenCustomerDashboardCommandExecuted(BranchItemViewModel branchItemViewModel)
        {
            Customer customer = base.ContextCBIRepository.GetCustomerByCode(branchItemViewModel.CustomerCode);

            AuditConfig config = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (config != null)
            {
                base.ContextCBIRepository.SetCurrentCustomer(customer, config);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearBranch();
            }

            AuditConfig newMainAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            if (newMainAuditConfig != null)
            {
                base.ContextCBIRepository.SetCurrentCustomer(customer, newMainAuditConfig);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearBranch();
            }

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            UtilsNavigate.CustomerDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void DoubleClickCommandExecuted(BranchItemViewModel branchItemViewModel)
        {
            if (_chooseCloseCommand.CanExecute())
                _chooseCloseCommand.Execute();
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.Main);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Branch);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.SelectParamsBuild();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        // восстановление домайн объектов Branch в БД MainDB, 
		// по файловой системе (файлов Count4UDB.sdf и AnalyticDB.sdf в папке Branch)
        // если есть домайн объект в БД - он не заменяется
        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
                IInventorConfigRepository inventorConfigRepository = this._serviceLocator.GetInstance<IInventorConfigRepository>();
                inventorConfigRepository.RepairDomainBranch();
                IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
                alterAdoProvider.UpdateCount4UDBViaScript();
				alterAdoProvider.UpdateAnalyticDBViaScript();
				UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);

                this.BuildList();
            }
          }

        private bool EditCommandCanExecute()
        {
            return _current != null && (_selectedMulti == null || _selectedMulti.Count == 1);
        }

        private void EditCommandExecuted()
        {
            _editSelectedCommand.Execute(_current);
        }

        private bool DeleteCommandCanExecute()
        {
            return _current != null && (_selectedMulti == null || _selectedMulti.Count == 1);
        }

        private void DeleteCommandExecuted()
        {
            _deleteSelectedCommand.Execute(_current);
        }

        private void ChildSelectedCommandExecuted(BranchItemViewModel branchItemViewModel)
        {
            this.BranchSelected(branchItemViewModel.Branch);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                case CBIContext.History:
                case CBIContext.Main:

                    UriQuery query = new UriQuery();

                    InventorFilterData filter = new InventorFilterData();
                    filter.CustomerCode = branchItemViewModel.Branch.CustomerCode;
                    filter.BranchCode = branchItemViewModel.Branch.Code;
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filter, Common.NavigationObjects.Filter);

                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

                    UtilsNavigate.InventorChooseOpen(this.Context, this._regionManager, query);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FilterBranch(IFilterData branchFilterData)
        {
            this._filter = branchFilterData as BranchFilterData;

            RaisePropertyChanged(() => IsFilterAnyField);

            BuildList();
        }

        private bool DeleteWithoutChildCommandCanExecute()
        {
            return _selectedMulti != null && _selectedMulti.Any();
        }

		//селектиованные в интерфейсе
        private void DeleteWithoutChildCommandExecuted()
        {
            Inventors inventors = _inventorRepository.GetInventors();

            bool isWithChild = _selectedMulti.Any(r => inventors.Any(z => z.BranchCode == r.Branch.Code));

            if (isWithChild)
            {
                UtilsMisc.ShowMessageBox(Localization.Resources.ViewModel_BranchChoose_msgDeleteWithoutChild, MessageBoxButton.OK, MessageBoxImage.Warning, _userSettingsManager);
            }

            using (new CursorWait())
            {
				var branchCodeList = this._selectedMulti.Select(r => r.Branch.Code).ToList();

                this._branchRepository.Delete(branchCodeList); //need back test !!!

                BuildList();
            }
        }

        public void SetSelected(List<BranchItemViewModel> list)
        {
            _selectedMulti = list;

            _editCommand.RaiseCanExecuteChanged();
            _deleteCommand.RaiseCanExecuteChanged();
            _deleteWithoutChildCommand.RaiseCanExecuteChanged();
        }

        private bool DeleteAllWithoutChildCommandCanExecute()
        {
            return _list != null && _list.Any();
        }

		// сейчас просто все из БД
        private void DeleteAllWithoutChildCommandExecuted()
        {
            string message = Localization.Resources.ViewModel_BranchChoose_msgDeleteAllWithoutChildren;

			string currentCustomerCode = "";
			if (this.Filter != null) currentCustomerCode = this.Filter.CustomerCode;
			if (string.IsNullOrWhiteSpace(currentCustomerCode) == false)
			{
				MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

				if (messageBoxResult == MessageBoxResult.Yes)
				{
					using (new CursorWait())
					{
						List<string> branchaesCodeList = this._branchRepository.GetBranchCodeListByCustomerCode(currentCustomerCode);
						//SelectParams sp = new SelectParams();
						//Branches branches = _branchRepository.GetBranches(sp);
						//	var branchaesCodeList = branches.Where(e => e.CustomerCode == currentCustomerCode).Select(r => r.Code).ToList();

						this._branchRepository.Delete(branchaesCodeList); // need back !!!
						BuildList();
			
					}
				}
			}
        }
    }
}

