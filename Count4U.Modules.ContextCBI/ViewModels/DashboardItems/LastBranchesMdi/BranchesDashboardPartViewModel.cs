using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class BranchesDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IBranchRepository _branchRepository;
        private readonly UICommandRepository _commandRepository;


        private readonly DelegateCommand<LastBranchesListItem> _branchNavigateCommand;
        private readonly DelegateCommand _moreBranchesCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _searchCommand;

        private readonly ObservableCollection<LastBranchesListItem> _items;
        private string _totalBranches;
        private readonly INavigationRepository _navigationRepository;

        public BranchesDashboardPartViewModel(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            IBranchRepository branchRepository,
            UICommandRepository commandRepository,
            INavigationRepository navigationRepository)
            : base(contextCBIRepository)
        {
            _navigationRepository = navigationRepository;
            this._commandRepository = commandRepository;
            this._branchRepository = branchRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._branchNavigateCommand = new DelegateCommand<LastBranchesListItem>(BranchNavigateCommandExecuted);
            this._moreBranchesCommand = _commandRepository.Build(enUICommand.More, this.MoreBranchesCommandExecuted);
            this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            this._addCommand = _commandRepository.Build(enUICommand.AddBranch, AddBranchCommandExecuted);
            this._searchCommand = _commandRepository.Build(enUICommand.SearchBranch, SearchBranchCommandExecuted);

            this._items = new ObservableCollection<LastBranchesListItem>();
        }

        public ObservableCollection<LastBranchesListItem> Items
        {
            get { return _items; }
        }

        public string TotalBranches
        {
            get { return _totalBranches; }
            set
            {
                _totalBranches = value;
                RaisePropertyChanged(() => TotalBranches);
            }
        }

        public DelegateCommand<LastBranchesListItem> BranchNavigateCommand
        {
            get { return _branchNavigateCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<BranchAddedEvent>().Subscribe(BranchAdded);

			Task.Factory.StartNew(ItemsBuild).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public DelegateCommand MoreBranchesCommand
        {
            get { return this._moreBranchesCommand; }
        }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
        }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this.Clear();
        }

        private void ItemsBuild()
        {
            Utils.RunOnUI(() => _items.Clear());

            List<LastBranchesListItem> list = new List<LastBranchesListItem>();
            if (base.CurrentCustomer != null)
            {
                foreach (Branch branch in _branchRepository.GetBranchesByCustomer(base.CurrentCustomer).OrderBy(r => r.BranchCodeLocal))
                {
                    LastBranchesListItem item = new LastBranchesListItem(branch);
                    list.Add(item);
                }

            }
            Utils.RunOnUI(() =>
                {
                    list.ForEach(r => _items.Add(r));
                    TotalBranches = String.Format(Localization.Resources.ViewModel_BranchesDashboardPart_TotalBranches, _items.Count);
                });
            
        }

        private void BranchNavigateCommandExecuted(LastBranchesListItem item)
        {
            AuditConfig newMainAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            Customer customer = base.ContextCBIRepository.GetCustomerByCode(item.Branch.CustomerCode);

            base.ContextCBIRepository.SetCurrentCustomer(customer, newMainAuditConfig);
            base.ContextCBIRepository.SetCurrentBranch(item.Branch, newMainAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearInventor();

            AuditConfig newHistoryAuditConfig = this.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (newHistoryAuditConfig != null)
            {
                base.ContextCBIRepository.SetCurrentCustomer(customer, newHistoryAuditConfig);
                base.ContextCBIRepository.SetCurrentBranch(item.Branch, newHistoryAuditConfig);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();
            }

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, newMainAuditConfig);
            UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void BranchAdded(Branch branch)
        {
            ItemsBuild();
        }

        private void MoreBranchesCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            BranchFilterData filter = new BranchFilterData();
            filter.CustomerCode = base.CurrentCustomer.Code;
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, filter, Common.NavigationObjects.Filter);
            UtilsNavigate.BranchChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        #region Implementation of IMdiChild

        public void Refresh()
        {

        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<BranchAddedEvent>().Unsubscribe(BranchAdded);
        }

        #endregion

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeBranch);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void SearchBranchCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.BranchChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        private void AddBranchCommandExecuted()
        {
            this._eventAggregator.GetEvent<BranchAddEvent>().Publish(new BranchAddEventPayload() { IsCustomerComboVisible = false, Context = CBIContext.Main });
        }
    }
}