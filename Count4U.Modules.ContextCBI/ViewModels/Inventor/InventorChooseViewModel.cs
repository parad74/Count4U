using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
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
using Microsoft.Practices.Unity;
using NLog;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.Constants;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorChooseViewModel : ContextCBIChooseViewModelCommon
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IUnityContainer _unityContainer;
        private readonly ReportButtonViewModel _reportButton;
        private readonly UICommandRepository<InventorItemViewModel> _commandRepositoryObject;

        private readonly DelegateCommand _searchCommand;
        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _chooseCloseCommand;
        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _deleteCommand;
        private readonly DelegateCommand<InventorItemViewModel> _editSelectedCommand;
        private readonly DelegateCommand<InventorItemViewModel> _deleteSelectedCommand;
        private readonly DelegateCommand<InventorItemViewModel> _viewCommand;
        private readonly DelegateCommand<InventorItemViewModel> _doubleClickCommand;
        private readonly DelegateCommand _repairCommand;
        private readonly DelegateCommand<InventorItemViewModel> _openCustomerDashboardCommand;
        private readonly DelegateCommand<InventorItemViewModel> _openBranchDashboardCommand;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private InventorItemViewModel _current;

        private readonly ObservableCollection<InventorItemViewModel> _list;

        private InventorFilterData _filter;

        public InventorChooseViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IUnityContainer unityContainer,
            INavigationRepository navigationRepository,
            ReportButtonViewModel reportButton,
            UICommandRepository commandRepository,
            UICommandRepository<InventorItemViewModel> commandRepositoryObject)
            : base(contextCBIRepository, eventAggregator, regionManager, navigationRepository, commandRepository)
        {
            this._commandRepositoryObject = commandRepositoryObject;
            this._serviceLocator = serviceLocator;
            this._reportButton = reportButton;
            this._unityContainer = unityContainer;
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator.GetEvent<InventorAddedEvent>().Subscribe(Added);
            this._eventAggregator.GetEvent<InventorEditedEvent>().Subscribe(Edited);
            this._chooseCloseCommand = new DelegateCommand(ChooseCloseCommandExecuted, ChooseCloseCommandCanExecute);

            this._addCommand = _commandRepository.Build(enUICommand.Add, AddCommandExecuted, AddCommandCanExecute);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted, EditCommandCanExecute);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, DeleteCommandExecuted, DeleteCommandCanExecute);
            this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, EditSelectedCommandExecuted);
            this._deleteSelectedCommand = _commandRepositoryObject.Build(enUICommand.Delete, DeleteSelectedCommandExecuted);
            this._viewCommand = new DelegateCommand<InventorItemViewModel>(InventorViewCommandExecuted);
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
            this._reportCommand = _commandRepository.Build(enUICommand.Report, delegate { });

            this._list = new ObservableCollection<InventorItemViewModel>();

            this._doubleClickCommand = new DelegateCommand<InventorItemViewModel>(DoubleClickCommandExecuted);
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._openCustomerDashboardCommand = new DelegateCommand<InventorItemViewModel>(OpenCustomerDashboardCommandExecuted);
            this._openBranchDashboardCommand = new DelegateCommand<InventorItemViewModel>(OpenBranchDashboardCommandExecuted);
        }

        #region properties

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get
            {
                return this._yesNoRequest;
            }
        }

        protected override enCBIScriptMode CBIScriptMode { get { return enCBIScriptMode.Inventor; } }

        protected override SelectParams SelectParams
        {
            get
            {
                var sp = this.SelectParamsBuild();
                sp.IsEnablePaging = false;
                return sp;
            }
        }

        public ObservableCollection<InventorItemViewModel> List
        {
            get
            {
                return this._list;
            }
        }

        public InventorItemViewModel Current
        {
            get
            {
                if (this._current == null || this._list == null)
                    return null;
                return this._list.FirstOrDefault(
                    r => r.Inventor.Code == this._current.Inventor.Code);
            }
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

        public DelegateCommand ChooseCloseCommand
        {
            get { return this._chooseCloseCommand; }
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

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand<InventorItemViewModel> EditSelectedCommand
        {
            get { return this._editSelectedCommand; }
        }

        public DelegateCommand<InventorItemViewModel> DeleteSelectedCommand
        {
            get { return this._deleteSelectedCommand; }
        }

        public DelegateCommand<InventorItemViewModel> ViewCommand
        {
            get { return this._viewCommand; }
        }

        public DelegateCommand<InventorItemViewModel> DoubleClickCommand
        {
            get { return _doubleClickCommand; }
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

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
        }

        public InventorFilterData Filter
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


        public DelegateCommand<InventorItemViewModel> OpenCustomerDashboardCommand
        {
            get { return _openCustomerDashboardCommand; }
        }

        public DelegateCommand<InventorItemViewModel> OpenBranchDashboardCommand
        {
            get { return _openBranchDashboardCommand; }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterInventor);

            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionCBIGet();

            this._reportButton.OnNavigatedTo(navigationContext);
            this._reportButton.Initialize(this.ReportCommandExecuted, () =>
            {
                SelectParams sp = SelectParamsBuild();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Inventor);

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, base._navigationRepository, Common.NavigationObjects.Filter, true) as InventorFilterData;
            if (_filter == null)
                _filter = new InventorFilterData();

            BuildList();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterInventor);

            this._reportButton.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<InventorAddedEvent>().Unsubscribe(this.Added);
            this._eventAggregator.GetEvent<InventorEditedEvent>().Unsubscribe(this.Edited);
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
            this._list.Clear();
            this.ItemsTotal = 0;

            SelectParams selectParams = null;

            try
            {
                selectParams = SelectParamsBuild();
                Inventors inventors = null;
                inventors = base.ContextCBIRepository.GetInventors(selectParams);

				if (inventors == null) return;

				IAuditConfigRepository auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>();
				List<string> inventorsCodeFromAudit = auditConfigRepository.GetInventorCodeList();

                Dictionary<string, Customer> customersCache = new Dictionary<string, Customer>();
                Dictionary<string, Branch> branchesCache = new Dictionary<string, Branch>();

                foreach (Inventor inventor in inventors)
                {
					if (inventorsCodeFromAudit.Contains(inventor.Code) == false) continue;

                    string customerCode = inventor.CustomerCode;
                    if (!customersCache.ContainsKey(customerCode))
                        customersCache.Add(customerCode, base.ContextCBIRepository.GetCustomerByCode(customerCode));
                    Customer customer = customersCache[customerCode];

                    string branchCode = inventor.BranchCode;
                    if (!branchesCache.ContainsKey(branchCode))
                        branchesCache.Add(branchCode, base.ContextCBIRepository.GetBranchByCode(branchCode));
                    Branch branch = branchesCache[branchCode];

                    this._list.Add(new InventorItemViewModel(inventor, customer, branch, _userSettingsManager.LanguageGet()));
                }

                this.ItemsTotal = (int)inventors.TotalCount;

                if ((inventors.TotalCount > 0) 		   //do not show empty space - move on previous page
                    && (inventors.Count == 0))
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

        private void InventorSelected(Inventor inventor)
        {

            Customer customer = base.ContextCBIRepository.GetCustomerByCode(inventor.CustomerCode);
            base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetAuditConfigByCurrentContext());

            Branch branch = base.ContextCBIRepository.GetBranchByCode(inventor.BranchCode);
            base.ContextCBIRepository.SetCurrentBranch(branch, this.GetAuditConfigByCurrentContext());

            this.InventorCurrentUpdate(inventor);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentInventor(inventor, this.GetCreateAuditConfig());
                    break;
                case CBIContext.History:
                    base.ContextCBIRepository.SetCurrentInventor(inventor, this.GetHistoryAuditConfig());
                    this.RaisePropertyChanged(() => this.CurrentInventor);
                    break;
                case CBIContext.Main:
                    base.ContextCBIRepository.SetCurrentInventor(inventor, this.GetMainAuditConfig());
                    this.RaisePropertyChanged(() => this.CurrentInventor);
                    this._chooseCloseCommand.RaiseCanExecuteChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ChooseCloseCommandCanExecute()
        {
            if (this._current == null)
                return false;
            else
                return true;
        }

        private void ChooseCloseCommandExecuted()
        {
            this.InventorSelected(this._current.Inventor);

            AuditConfig config = base.ContextCBIRepository.GetCBIConfigByInventorCode(CBIContext.History, this._current.Inventor.Code);
            if (config != null)
            {
                base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, config);
                base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, config);
            }

            Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                case CBIContext.History:
                case CBIContext.Main:
                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddCommandExecuted()
        {
            this._eventAggregator.GetEvent<InventorAddEvent>().Publish(new InventorAddEventPayload()
            {
                Context = base.Context,
                IsCustomerComboVisible = true,
                IsBranchComboVisible = true,
				WithoutNavigate = false
            });
        }

        private bool AddCommandCanExecute()
        {
            return true;
        }

        private void Added(Inventor inventor)
        {
            BuildList();
            this.InventorCurrentUpdate(inventor);
            this.RaisePropertyChanged(() => this.CurrentInventor);
        }

        private void InventorCurrentUpdate(Inventor inventor)
        {
            if (inventor == null) return;
            this.Current = this._list.FirstOrDefault(
                r => r.Inventor.Code == inventor.Code);
        }

        private void Edited(Inventor inventor)
        {
            InventorItemViewModel listInventor = this._list.FirstOrDefault(
                r => r.Inventor.Code == inventor.Code);
            if (listInventor != null) listInventor.UpdateViewModelWithInventor(inventor);
        }

        private void EditSelectedCommandExecuted(InventorItemViewModel inventor)
        {
            this.InventorSelected(inventor.Inventor);
            this._eventAggregator.GetEvent<InventorEditEvent>().Publish(new InventorEditEventPayload() { Context = base.Context, Inventor = inventor.Inventor });
        }

        private void DeleteSelectedCommandExecuted(InventorItemViewModel inventorItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    throw new InvalidOperationException();
                case CBIContext.History:
                case CBIContext.Main:
                    string message = String.Format(Localization.Resources.Msg_Delete_Inventor, inventorItemViewModel.Name);
                    this._yesNoRequest.Raise(new MessageBoxYesNoNotification() { Content = message, Settings = this._userSettingsManager }, r =>
                    {
                        if (r.IsYes)
                        {

                            base.ContextCBIRepository.Delete(inventorItemViewModel.Inventor);
                            BuildList();
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InventorViewCommandExecuted(InventorItemViewModel inventorItemViewModel)
        {
            switch (this.Context)
            {
                case CBIContext.CreateInventor:
                    base.ContextCBIRepository.SetCurrentInventor(
                        inventorItemViewModel.Inventor, this.GetCreateAuditConfig());
                    this._eventAggregator.GetEvent<InventorViewEvent>().Publish(
                        new InventorViewEventPayload() { Inventor = inventorItemViewModel.Inventor, Context = base.Context });
                    break;
                case CBIContext.History:
                case CBIContext.Main:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoubleClickCommandExecuted(InventorItemViewModel inventorItemViewModel)
        {
            if (_chooseCloseCommand.CanExecute())
                _chooseCloseCommand.Execute();
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.Main);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Inventor);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            var sp = this.SelectParamsBuild();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        // восстановление домайн объектов inventor, auditConfig в БД AuditDB, 
		// по файловой системе (файлов Count4UDB.sdf AnalyticDB.sdf в папке Inventor)
        // если есть домайн объект в БД - он не заменяется
        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
                IInventorConfigRepository inventorConfigRepository = this._serviceLocator.GetInstance<IInventorConfigRepository>();
                inventorConfigRepository.RepairDomainInventor();
                //inventorConfigRepository.TestRestoreDomainObject_Inventor();
                IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
                alterAdoProvider.UpdateCount4UDBViaScript();
				alterAdoProvider.UpdateAnalyticDBViaScript();
                UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                this.BuildList();
            }
        }

        private bool EditCommandCanExecute()
        {
            return _current != null;
        }

        private void EditCommandExecuted()
        {
            if (_current != null)
                _editSelectedCommand.Execute(_current);
        }

        private bool DeleteCommandCanExecute()
        {
            return _current != null;
        }

        private void DeleteCommandExecuted()
        {
            if (_current != null)
                _deleteSelectedCommand.Execute(_current);
        }

        private void FilterInventor(IFilterData inventorFilterData)
        {
            this._filter = inventorFilterData as InventorFilterData;

            RaisePropertyChanged(() => IsFilterAnyField);

            BuildList();
        }

        private void OpenCustomerDashboardCommandExecuted(InventorItemViewModel inventorItemViewModel)
        {
            Customer customer = base.ContextCBIRepository.GetCustomerByCode(inventorItemViewModel.CustomerCode);

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

        private void OpenBranchDashboardCommandExecuted(InventorItemViewModel item)
        {
            Customer customer = base.ContextCBIRepository.GetCustomerByCode(item.CustomerCode);
            Branch branch = base.ContextCBIRepository.GetBranchByCode(item.BranchCode);

            AuditConfig newAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);

            base.ContextCBIRepository.SetCurrentCustomer(customer, newAuditConfig);
            base.ContextCBIRepository.SetCurrentBranch(branch, newAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearInventor();

            AuditConfig newHistoryAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            base.ContextCBIRepository.SetCurrentCustomer(customer, newHistoryAuditConfig);
            base.ContextCBIRepository.SetCurrentBranch(branch, newHistoryAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, newAuditConfig);
            UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
        }
    }
}