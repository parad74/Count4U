using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Count4U.Common.Extensions;
using Count4U.Model.Interface.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastBranchesDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IRegionManager _regionManager;
        private readonly DelegateCommand _moreBranchesCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly DelegateCommand<LastBranchesListItem> _branchNavigateCommand;
        private readonly UICommandRepository _commandRepository;
        private readonly INavigationRepository _navigationRepository;
		private readonly IBranchRepository _branchRepository;

        private readonly ObservableCollection<LastBranchesListItem> _items;

        public LastBranchesDashboardPartViewModel(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            UICommandRepository commandRepository,
			IBranchRepository branchRepository ,
            INavigationRepository navigationRepository)
            : base(contextCBIRepository)
        {
            _navigationRepository = navigationRepository;
            _commandRepository = commandRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
			this._branchRepository = branchRepository;
            this._moreBranchesCommand = _commandRepository.Build(enUICommand.More, this.MoreBranchesCommandExecuted);
            this._branchNavigateCommand = new DelegateCommand<LastBranchesListItem>(BranchNavigateCommandExecuted);

            this._items = new ObservableCollection<LastBranchesListItem>();
        }

        public string TotalBranches { get; set; }

        public DelegateCommand MoreBranchesCommand
        {
            get { return this._moreBranchesCommand; }
        }

        public DelegateCommand<LastBranchesListItem> BranchNavigateCommand
        {
            get { return this._branchNavigateCommand; }
        }

        public ObservableCollection<LastBranchesListItem> Items
        {
            get { return _items; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(ItemsBuild).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void ItemsBuild()
        {
            //Utils.RunOnUI(() => this._items.Clear());

			Branches all = this._branchRepository.GetBranches();
			Dictionary<string, Branch> dictionary = all.Select(x => x).Distinct().ToDictionary(x => x.Code);

            AuditConfig config = GetAuditConfigByCurrentContext();

            List<LastBranchesListItem> list = new List<LastBranchesListItem>();
			int k = 0;
			List<Branch>  brans = base.ContextCBIRepository.GetHistoryBranchForCustomerLast(config.CustomerCode, 100).ToList();
			foreach (Branch branch in brans)
            {
				if (branch != null)
				{
					if (dictionary.ContainsKey(branch.Code) == true)
					{
						if (k <= 20)
						{
							LastBranchesListItem item = new LastBranchesListItem(branch);
							list.Add(item);
							k++;
						}
					}
				}
            }

            Utils.RunOnUI(() =>
             {
                 list.ForEach(r => _items.Add(r));
                 TotalBranches = String.Format("Total {0} branches", _items.Count);
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
            base.ContextCBIRepository.SetCurrentCustomer(customer, newHistoryAuditConfig);
            base.ContextCBIRepository.SetCurrentBranch(item.Branch, newHistoryAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, newMainAuditConfig);
            UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void MoreBranchesCommandExecuted()
        {
            BranchFilterData filterData = new BranchFilterData();

            if (base.CurrentCustomer != null)
            {
                filterData.CustomerCode = base.CurrentCustomer.Code;
            }

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            UtilsNavigate.BranchChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        public void Refresh()
        {

        }

        public void Clear()
        {

        }
    }
}