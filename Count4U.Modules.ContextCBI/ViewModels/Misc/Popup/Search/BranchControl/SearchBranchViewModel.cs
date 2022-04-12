using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.BranchControl
{
    public class SearchBranchViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly IBranchRepository _branchRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<BranchItemViewModel> _branchList;
        private BranchItemViewModel _branchChooseCurrent;

        private int _branchPageSize;
        private int _branchPageCurrent;
        private int _branchItemsTotal;

        private readonly DelegateCommand _branchMoreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchBranchViewModel(IContextCBIRepository contextCbiRepository,
            IBranchRepository branchRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager)
            :base(contextCbiRepository)
        {
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;
            _branchRepository = branchRepository;

            _branchList = new ObservableCollection<BranchItemViewModel>();
            _branchMoreCommand = new DelegateCommand(BranchMoreCommandExecuted);

        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<BranchItemViewModel> BranchList
        {
            get { return _branchList; }
        }

        public BranchItemViewModel BranchChooseCurrent
        {
            get { return _branchChooseCurrent; }
            set
            {
                _branchChooseCurrent = value;
                RaisePropertyChanged(() => BranchChooseCurrent);
            }
        }

        public int BranchPageSize
        {
            get { return _branchPageSize; }
            set
            {
                _branchPageSize = value;
                RaisePropertyChanged(() => BranchPageSize);
            }
        }

        public int BranchPageCurrent
        {
            get { return _branchPageCurrent; }
            set
            {
                _branchPageCurrent = value;
                RaisePropertyChanged(() => BranchPageCurrent);

                BuildBranch();
            }
        }

        public int BranchItemsTotal
        {
            get { return _branchItemsTotal; }
            set
            {
                _branchItemsTotal = value;
                RaisePropertyChanged(() => BranchItemsTotal);
                RaisePropertyChanged(() => BranchTotalString);
            }
        }

        public string BranchTotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchBranchTotal, _branchItemsTotal); }
        }

        public DelegateCommand BranchMoreCommand
        {
            get { return _branchMoreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _branchPageCurrent = 1;
            _branchPageSize = this._userSettingsManager.PortionCBIGet();            
        }

        private void BuildBranch()
        {
            SelectParams sp = SelectParamsBranch();

            Branches branches = _branchRepository.GetBranches(sp);

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
                _branchList.Clear();
                toAdd.ForEach(r => _branchList.Add(r));
                this.BranchItemsTotal = (int)branches.TotalCount;

                if ((branches.TotalCount > 0)
                    && (branches.Count == 0)) //do not show empty space - move on previous page
                {
                    this.BranchPageCurrent = this._branchPageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsBranch()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _branchPageSize;
            result.CurrentPage = _branchPageCurrent;

            if (_searchFieldViewModel != null)
            {
                BranchFilterData filterData = _searchFieldViewModel.BuildFilterData() as BranchFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result);
            }

            return result;
        }

        public void BranchNavigate(BranchItemViewModel viewModel)
        {
            UtilsPopup.Close(View);

            Customer customer = base.ContextCBIRepository.GetCustomerByCode(viewModel.Branch.CustomerCode);
            Branch branch = viewModel.Branch;
            base.ContextCBIRepository.SetCurrentCustomer(customer, GetAuditConfigByCurrentContext());
            base.ContextCBIRepository.SetCurrentBranch(branch, GetAuditConfigByCurrentContext());

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.BranchDashboardOpen(this.Context, this._regionManager, query);
        }

        private void BranchMoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            if (_searchFieldViewModel != null)
            {
                BranchFilterData filterData = _searchFieldViewModel.BuildFilterData() as BranchFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.BranchChooseOpen(this.Context, this._regionManager, query);
        }

        public void Search()
        {
            _branchPageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => BranchPageCurrent));
            BuildBranch();
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
            return SelectParamsBranch();
        }
    }
}