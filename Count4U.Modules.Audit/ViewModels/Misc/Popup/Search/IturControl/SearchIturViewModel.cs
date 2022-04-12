using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

namespace Count4U.Modules.Audit.ViewModels
{
    public class SearchIturViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly IIturRepository _iturRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ILocationRepository _locationRepository;
        private readonly IStatusIturRepository _statusIturRepository;

        private readonly ObservableCollection<IturItemViewModel> _list;
        private IturItemViewModel _chooseCurrent;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchIturViewModel(IContextCBIRepository contextCbiRepository,
            IIturRepository iturRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            ILocationRepository locationRepository,
            IStatusIturRepository statusIturRepository)
            : base(contextCbiRepository)
        {
            _statusIturRepository = statusIturRepository;
            _locationRepository = locationRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;
            _iturRepository = iturRepository;

            _list = new ObservableCollection<IturItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public ObservableCollection<IturItemViewModel> List
        {
            get { return _list; }
        }

        public IturItemViewModel ChooseCurrent
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
            get { return String.Format(Localization.Resources.ViewModel_SearchIturTotal, _itemsTotal); }
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
            SelectParams sp = SelectParamsBuild();

            Iturs iturs = _iturRepository.GetIturs(sp, base.GetDbPath);

            if (iturs == null) return;

            Locations locations = this._locationRepository.GetLocations(base.GetDbPath);

            Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;

            List<IturItemViewModel> toAdd = new List<IturItemViewModel>();
            foreach (var itur in iturs)
            {
                IturItemViewModel viewModel = new IturItemViewModel(itur,
                                                                              locations.FirstOrDefault(r => r.Code == itur.LocationCode),
                                                                              statuses.Values.FirstOrDefault(r => r.Bit == itur.StatusIturBit));
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                _list.Clear();
                toAdd.ForEach(r => _list.Add(r));
                this.ItemsTotal = (int)iturs.TotalCount;

                if ((iturs.TotalCount > 0)
                    && (iturs.Count == 0)) //do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsBuild()
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Number ASC";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = _pageSize;
            result.CurrentPage = _pageCurrent;

            if (_searchFieldViewModel != null)
            {
                IturFilterData filterData = _searchFieldViewModel.BuildFilterData() as IturFilterData;
                if (filterData != null)
                {
                    Locations locations = this._locationRepository.GetLocations(base.GetDbPath);
//                    Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;

                    filterData.ApplyToSelectParams(result, locations/*, statuses*/);
                }
            }

            return result;
        }

        public void Navigate(IturItemViewModel viewModel)
        {
            // UtilsPopup.Close(View);

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
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            if (_searchFieldViewModel != null)
            {
                IturFilterData filterData = _searchFieldViewModel.BuildFilterData() as IturFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.IturimAddEditDeleteOpen(_regionManager, query);

        }
    }
}