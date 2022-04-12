using System;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Section;
using Count4U.Modules.Audit.ViewModels.Supplier;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.SupplierControl
{
    public class SearchSupplierViewModel: CBIContextBaseViewModel, ISearchGridViewModel
    {
         private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ISupplierRepository _supplierRepository;

        private readonly ObservableCollection<SupplierItemViewModel> _list;
        private SupplierItemViewModel _chooseCurrent;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchSupplierViewModel(IContextCBIRepository contextCbiRepository,
            ISupplierRepository supplierRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCbiRepository)
        {
            _supplierRepository = supplierRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;

            _list = new ObservableCollection<SupplierItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<SupplierItemViewModel> List
        {
            get { return _list; }
        }

        public SupplierItemViewModel ChooseCurrent
        {
            get { return _chooseCurrent; }
            set
            {
                _chooseCurrent = value;
                RaisePropertyChanged(() => ChooseCurrent);
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }

        public int PageCurrent
        {
            get { return _pageCurrent; }
            set
            {
                _pageCurrent = value;
                RaisePropertyChanged(() => PageCurrent);

                using (new CursorWait())
                {
                    Build();
                }
            }
        }

        public int ItemsTotal
        {
            get { return _itemsTotal; }
            set
            {
                _itemsTotal = value;
                RaisePropertyChanged(() => ItemsTotal);
                RaisePropertyChanged(() => TotalString);
            }
        }

        public string TotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchSupplierTotal, _itemsTotal); }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _pageCurrent = 1;
            //_pageSize = this._userSettingsManager.PortionItursGet();
            _pageSize = 50;
        }

        private void Build()
        {
            Utils.RunOnUI(() => _list.Clear());

            SelectParams sp = SelectParamsBuild();

            Suppliers suppliers = _supplierRepository.GetSuppliers(sp, base.GetDbPath);

            if (suppliers == null) return;

            List<SupplierItemViewModel> toAdd = new List<SupplierItemViewModel>();

            foreach (Count4U.Model.Count4U.Supplier suppler in suppliers)
            {
                SupplierItemViewModel viewModel = new SupplierItemViewModel(suppler);
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                _list.Clear();
                toAdd.ForEach(r => _list.Add(r));
                this.ItemsTotal = (int)suppliers.TotalCount;

                if ((suppliers.TotalCount > 0)
                    && (suppliers.Count == 0)) //do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsBuild()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Name ASC";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _pageSize;
            result.CurrentPage = _pageCurrent;

            if (_searchFieldViewModel != null)
            {
                SupplierFilterData filterData = _searchFieldViewModel.BuildFilterData() as SupplierFilterData;
                if (filterData != null)
                {
                    filterData.ApplyToSelectParams(result);
                }
            }

            return result;
        }

        public void Search()
        {
            _pageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => PageCurrent));
            Build();
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
            return SelectParamsBuild();
        }

        private void MoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            if (_searchFieldViewModel != null)
            {
                SupplierFilterData filterData = _searchFieldViewModel.BuildFilterData() as SupplierFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.SupplierAddEditDeleteOpen(_regionManager, query);

        } 
    }
}