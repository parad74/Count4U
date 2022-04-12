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
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.LocationControl
{
    public class SearchLocationViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly ILocationRepository _locationRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<LocationItemViewModel> _list;
        private LocationItemViewModel _chooseCurrent;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchLocationViewModel(IContextCBIRepository contextCbiRepository,
            ILocationRepository locationRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCbiRepository)
        {
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;
            _locationRepository = locationRepository;

            _list = new ObservableCollection<LocationItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<LocationItemViewModel> List
        {
            get { return _list; }
        }

        public LocationItemViewModel ChooseCurrent
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
            get { return String.Format(Localization.Resources.ViewModel_SearchLocationTotal, _itemsTotal); }
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
            Utils.RunOnUI(()=>_list.Clear());

            SelectParams sp = SelectParamsBuild();

            Locations locations = _locationRepository.GetLocations(sp, base.GetDbPath);

            if (locations == null) return;

            List<LocationItemViewModel> toAdd = new List<LocationItemViewModel>();
            foreach (Location location in locations)
            {
                LocationItemViewModel viewModel = new LocationItemViewModel(location);
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {                
                toAdd.ForEach(r => _list.Add(r));
                this.ItemsTotal = (int)locations.TotalCount;

                if ((locations.TotalCount > 0)
                    && (locations.Count == 0)) //do not show empty space - move on previous page
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
                LocationFilterData filterData = _searchFieldViewModel.BuildFilterData() as LocationFilterData;
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
                LocationFilterData filterData = _searchFieldViewModel.BuildFilterData() as LocationFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.LocationAddEditDeleteOpen(_regionManager, query);

        }
    }
}