using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastInventorsDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
		private readonly IInventorRepository _inventorRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly UICommandRepository _commandRepository;
        private readonly INavigationRepository _navigationRepository;
		private Dictionary<string, Inventor> _inventorDictionary;

        private readonly DelegateCommand _moreInventorsCommand;
        private readonly DelegateCommand _addCommand;
//        private readonly DelegateCommand _searchCommand;
        private readonly DelegateCommand<LastInventorsListItem> _auditNavigateCommand;
        private readonly DelegateCommand<LastInventorsListItem> _customerNavigateCommand;
        private readonly DelegateCommand<LastInventorsListItem> _branchNavigateCommand;

        private readonly ObservableCollection<LastInventorsListItem> _items;

        public LastInventorsDashboardPartViewModel(
            IRegionManager regionManager,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IUnityContainer unityContainer,
            UICommandRepository commandRepository,
            INavigationRepository navigationRepository,
			IInventorRepository inventorRepository)
            : base(contextCBIRepository)
        {
			this._inventorRepository = inventorRepository;
			this._navigationRepository = navigationRepository;
			this._commandRepository = commandRepository;
            this._unityContainer = unityContainer;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._moreInventorsCommand = _commandRepository.Build(enUICommand.More, this.MoreInventorsCommandExecuted);
            this._auditNavigateCommand = new DelegateCommand<LastInventorsListItem>(this.AuditNavigateCommandExecuted);
            this._customerNavigateCommand = new DelegateCommand<LastInventorsListItem>(this.CustomerNavigateCommandExecuted);
            this._branchNavigateCommand = new DelegateCommand<LastInventorsListItem>(this.BranchNavigateCommandExecuted);
            this._addCommand = _commandRepository.Build(enUICommand.AddInventor, AddInventorCommandExecuted);
//            this._searchCommand = _commandRepository.Build(enUICommand.SearchInventor, SearchInventorCommandExecuted);

            this._items = new ObservableCollection<LastInventorsListItem>();
			this._inventorDictionary = new Dictionary<string, Inventor>();

			Inventors inventors = this._inventorRepository.GetInventors();
			this._inventorDictionary = inventors.Select(e => e).Distinct().ToDictionary(k => k.Code);

        }      

        public DelegateCommand MoreInventorsCommand
        {
            get { return this._moreInventorsCommand; }
        }

        public DelegateCommand<LastInventorsListItem> AuditNavigateCommand
        {
            get { return this._auditNavigateCommand; }
        }

        public DelegateCommand<LastInventorsListItem> CustomerNavigateCommand
        {
            get { return this._customerNavigateCommand; }
        }

        public DelegateCommand<LastInventorsListItem> BranchNavigateCommand
        {
            get { return this._branchNavigateCommand; }
        }

        public ObservableCollection<LastInventorsListItem> Items
        {
            get { return _items; }
        }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

//        public DelegateCommand SearchCommand
//        {
//            get { return _searchCommand; }
//        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(CustomerEdited);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Subscribe(BranchEdited);
            this._eventAggregator.GetEvent<InventorInventorsRefreshEvent>().Subscribe(InventorAdded);
            

            BuildOnBackgroundThread();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void AuditNavigateCommandExecuted(LastInventorsListItem item)
        {
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, item.AuditConfig);
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, item.AuditConfig);

            Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
            UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
        }

        private void CustomerNavigateCommandExecuted(LastInventorsListItem item)
        {
            if (item.AuditConfig != null)
            {
                base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, item.AuditConfig);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearBranch();
            }

            AuditConfig newMainAuditConfig = new AuditConfig(item.AuditConfig);
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, newMainAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearBranch();

            AuditConfig newCreateAuditConfig = new AuditConfig(item.AuditConfig);
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.CreateInventor, newCreateAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor).ClearBranch();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            UtilsNavigate.CustomerDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void BranchNavigateCommandExecuted(LastInventorsListItem item)
        {
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, item.AuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearInventor();

            AuditConfig newMainAuditConfig = new AuditConfig(item.AuditConfig);
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, newMainAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearInventor();

            AuditConfig newCreateAuditConfig = new AuditConfig(item.AuditConfig);
            base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.CreateInventor, newCreateAuditConfig);
            base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor).ClearInventor();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, newMainAuditConfig);
            UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        private void Build()
        {

            //open on main dashboard
            if (IsOpenForMain())
            {
                this.BuildItems(base.ContextCBIRepository.GetCBIConfigsLast(CBIContext.History, 50));
            }
            //open on customer dashboard
            if (IsOpenForCustomer())
            {
                Customer customer = base.ContextCBIRepository.GetCurrentCustomer(GetAuditConfigByCurrentContext());
                if (customer != null)
                {
                    this.BuildItems(base.ContextCBIRepository.GetCBIConfigsByCustomerCodeLast(
                        CBIContext.History, 10, customer.Code));
                }
            }

            //open on branch dashboard
            if (IsOpenForBranch())
            {
                Branch branch = base.ContextCBIRepository.GetCurrentBranch(GetAuditConfigByCurrentContext());
                if (branch != null)
                    this.BuildItems(base.ContextCBIRepository.GetCBIConfigsByBranchCodeLast(
                        CBIContext.History, 10, branch.Code));
            }
        }

        private void BuildItems(AuditConfigs configs)
        {
          		

            if (configs == null) return;

            List<LastInventorsListItem> list = new List<LastInventorsListItem>();

            foreach (AuditConfig audit in configs)
            {
				if (audit == null) continue;
                LastInventorsListItem item = new LastInventorsListItem(audit);
				if (string.IsNullOrWhiteSpace(audit.InventorCode) == false)
				{
					if (this._inventorDictionary.ContainsKey(audit.InventorCode) == true)
					{
						Inventor inventor = this._inventorDictionary[audit.InventorCode];
						if (inventor != null)
						{
							item.CompleteDate = inventor.CompleteDate.ToShortDateString() + " " +  inventor.CompleteDate.ToShortTimeString();
						}
					}
				}
                list.Add(item);
            }

            Utils.RunOnUI(() => this._items.Clear());
            Application.Current.Dispatcher.BeginInvoke(new Action(() => list.ForEach(r => _items.Add(r))));
        }

        private bool IsOpenForMain()
        {
            return base.NavigationContext.Parameters.Any(r => r.Value == Common.NavigationSettings.LastInventorsForMain);
        }

        private bool IsOpenForCustomer()
        {
            return base.NavigationContext.Parameters.Any(r => r.Value == Common.NavigationSettings.LastInventorsForCustomer);
        }

        private bool IsOpenForBranch()
        {
            return base.NavigationContext.Parameters.Any(r => r.Value == Common.NavigationSettings.LastInventorsForBranch);
        }

        private void MoreInventorsCommandExecuted()
        {
            InventorFilterData filterData = new InventorFilterData();           

            AuditConfig mainAuditConfig = this.GetMainAuditConfig();
            if (mainAuditConfig != null)
            {
                if (IsOpenForMain() == true)
                {
                    mainAuditConfig.Clear();
                }

                if (IsOpenForCustomer() == true)
                {
                    mainAuditConfig.ClearBranch();
                    filterData.CustomerCode = base.CurrentCustomer.Code;
                }

                if (IsOpenForBranch() == true)
                {
                    mainAuditConfig.ClearInventor();
                    filterData.CustomerCode = base.CurrentCustomer.Code;
                    filterData.BranchCode = base.CurrentBranch.Code;
                }
            }

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, mainAuditConfig);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        private void CustomerEdited(Customer customer)
        {
            BuildOnBackgroundThread();
        }

        private void BranchEdited(Branch branch)
        {
            BuildOnBackgroundThread();
        }

        private void InventorAdded(Inventor inventor)
        {
            BuildOnBackgroundThread();
        }

        private void BuildOnBackgroundThread()
        {
            _items.Clear();
            Task.Factory.StartNew(Build).LogTaskFactoryExceptions("BuildOnBackgroundThread");
        }

        public void Refresh()
        {

        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<CustomerEditedEvent>().Unsubscribe(CustomerEdited);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Unsubscribe(BranchEdited);
            this._eventAggregator.GetEvent<InventorInventorsRefreshEvent>().Unsubscribe(InventorAdded);
            
        }

//        private void SearchInventorCommandExecuted()
//        {
//            UriQuery query = new UriQuery();
//            Utils.AddAuditConfigToQuery(query, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
//            UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
//        }

        private void AddInventorCommandExecuted()
        {
            AuditConfig mainAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
			AuditConfig createInventorConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
            Customer customer = base.ContextCBIRepository.GetCurrentCustomer(mainAuditConfig);
			if (IsOpenForCustomer() == true)
			{
				base.ContextCBIRepository.SetCurrentCustomer(customer, createInventorConfig);

				this._eventAggregator.GetEvent<InventorAddEvent>().Publish(new InventorAddEventPayload()
				{
					IsCustomerComboVisible = false,
					IsBranchComboVisible = true,
					Context = CBIContext.CreateInventor	  ,
					WithoutNavigate = false
				});
			}
			else if (IsOpenForBranch() == true)
			{
				Branch branch = base.ContextCBIRepository.GetCurrentBranch(mainAuditConfig);
				base.ContextCBIRepository.SetCurrentBranch(branch, createInventorConfig);

				this._eventAggregator.GetEvent<InventorAddEvent>().Publish(new InventorAddEventPayload()
				{
					IsCustomerComboVisible = false,
					IsBranchComboVisible = false,
					Context = CBIContext.CreateInventor,
					WithoutNavigate = false
				});
			}
         //   BuildOnBackgroundThread();
        }
    }
}