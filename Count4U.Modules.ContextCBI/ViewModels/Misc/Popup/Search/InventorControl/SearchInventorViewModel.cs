using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.InventorControl
{
    public class SearchInventorViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
		private readonly IServiceLocator _serviceLocator;
        private readonly IInventorRepository _inventorRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IUnityContainer _unityContainer;

        private readonly ObservableCollection<InventorItemViewModel> _inventorList;
        private InventorItemViewModel _inventorChooseCurrent;
        private int _inventorPageSize;
        private int _inventorPageCurrent;
        private int _inventorItemsTotal;

        private readonly DelegateCommand _inventorMoreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchInventorViewModel(
			IServiceLocator serviceLocator,
			IContextCBIRepository contextCbiRepository,
            IInventorRepository inventorRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IUnityContainer unityContainer)
            : base(contextCbiRepository)
        {
			this._serviceLocator = serviceLocator;
            _unityContainer = unityContainer;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;
            _inventorRepository = inventorRepository;

            _inventorList = new ObservableCollection<InventorItemViewModel>();
            _inventorMoreCommand = new DelegateCommand(InventorMoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<InventorItemViewModel> InventorList
        {
            get { return _inventorList; }
        }

        public InventorItemViewModel InventorChooseCurrent
        {
            get { return _inventorChooseCurrent; }
            set
            {
                _inventorChooseCurrent = value;
                RaisePropertyChanged(() => InventorChooseCurrent);
            }
        }

        public int InventorPageSize
        {
            get { return _inventorPageSize; }
            set
            {
                _inventorPageSize = value;
                RaisePropertyChanged(() => InventorPageSize);
            }
        }

        public int InventorPageCurrent
        {
            get { return _inventorPageCurrent; }
            set
            {
                _inventorPageCurrent = value;
                RaisePropertyChanged(() => InventorPageCurrent);

                BuildInventor();
            }
        }

        public int InventorItemsTotal
        {
            get { return _inventorItemsTotal; }
            set
            {
                _inventorItemsTotal = value;
                RaisePropertyChanged(() => InventorItemsTotal);

                RaisePropertyChanged(() => InventorTotalString);
            }
        }

        public string InventorTotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventorTotal, _inventorItemsTotal); }
        }

        public DelegateCommand InventorMoreCommand
        {
            get { return _inventorMoreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _inventorPageCurrent = 1;
            _inventorPageSize = this._userSettingsManager.PortionCBIGet();            
        }

        private void BuildInventor()
        {
            SelectParams selectParams = null;

            selectParams = SelectParamsInventor();
            Inventors inventors = null;
            inventors = base.ContextCBIRepository.GetInventors(selectParams);

            if (inventors == null) return;

			IAuditConfigRepository auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>();
			List<string> inventorsCodeFromAudit = auditConfigRepository.GetInventorCodeList();

            Dictionary<string, Customer> customersCache = new Dictionary<string, Customer>();
            Dictionary<string, Branch> branchesCache = new Dictionary<string, Branch>();

            List<InventorItemViewModel> toAdd = new List<InventorItemViewModel>();
            foreach (Inventor inventor in inventors)
            {
				if (inventorsCodeFromAudit.Contains(inventor.Code) == false) continue;

                Customer customer;
                string customerCode = inventor.CustomerCode;
                if (!customersCache.ContainsKey(customerCode))
                    customersCache.Add(customerCode, base.ContextCBIRepository.GetCustomerByCode(customerCode));
                customer = customersCache[customerCode];

                Branch branch;
                string branchCode = inventor.BranchCode;
                if (!branchesCache.ContainsKey(branchCode))
                    branchesCache.Add(branchCode, base.ContextCBIRepository.GetBranchByCode(branchCode));
                branch = branchesCache[branchCode];

                toAdd.Add(new InventorItemViewModel(inventor, customer, branch, _userSettingsManager.LanguageGet()));
            }

            Utils.RunOnUI(() =>
            {
                _inventorList.Clear();
                toAdd.ForEach(r => _inventorList.Add(r));
                InventorItemsTotal = (int)inventors.TotalCount;

                if ((inventors.TotalCount > 0) //do not show empty space - move on previous page
                    && (inventors.Count == 0))
                {
                    InventorPageCurrent = this._inventorPageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsInventor()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _inventorPageSize;
            result.CurrentPage = _inventorPageCurrent;

            if (_searchFieldViewModel != null)
            {
                InventorFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventorFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result);
            }


            return result;
        }

        public void InventorNavigate(InventorItemViewModel viewModel)
        {
            UtilsPopup.Close(View);

            Customer customer = base.ContextCBIRepository.GetCustomerByCode(viewModel.Inventor.CustomerCode);
            base.ContextCBIRepository.SetCurrentCustomer(customer, this.GetAuditConfigByCurrentContext());

            Branch branch = base.ContextCBIRepository.GetBranchByCode(viewModel.Inventor.BranchCode);
            base.ContextCBIRepository.SetCurrentBranch(branch, this.GetAuditConfigByCurrentContext());

            AuditConfig config = base.ContextCBIRepository.GetCBIConfigByInventorCode(CBIContext.History, this._inventorChooseCurrent.Inventor.Code);
            if (config != null)
            {
                base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.History, config);
                base.ContextCBIRepository.SetCurrentCBIConfig(CBIContext.Main, config);
            }

            Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
        }

        private void InventorMoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));

            if (_searchFieldViewModel != null)
            {
                InventorFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventorFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        public void Search()
        {
            _inventorPageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => InventorPageCurrent));
            BuildInventor(); 
        }

        public Action<bool> IsBusy { get; set; }

        public ISearchFieldViewModel SearchFieldViewModel
        {
            set { _searchFieldViewModel = value; }
        }

        public void CanSearch(bool isCanSearch)
        {
            
        }

        public SelectParams BuildSelectParams()
        {
            return SelectParamsInventor();
        }
    }
}