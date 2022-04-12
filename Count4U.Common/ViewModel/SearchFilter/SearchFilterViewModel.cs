using System;
using System.Windows.Controls;
using Count4U.Common.Events.Filter;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel.Filter.Data;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.SearchFilter
{
    public class SearchFilterViewModel : NotificationObject, INavigationAware, IRegionMemberLifetime
    {
        private readonly UICommandRepository _commandRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly UICommand _searchCommand;

        private IFilterData _filter;

        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;

        private Action _filterAction;

        public SearchFilterViewModel(
              UICommandRepository commandRepository,
              PopupExtSearch popupExtSearch,
              PopupExtFilter popupExtFilter,
              IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _popupExtFilter = popupExtFilter;
            _popupExtSearch = popupExtSearch;
            _commandRepository = commandRepository;
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
        }

        public Button BtnSearch { get; set; }
        public Button BtnFilter { get; set; }

        public UICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public bool IsFilterAnyField
        {
            get { return Filter == null ? false : Filter.IsAnyField(); }
        }

        public IFilterData Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public PopupExtSearch PopupExtSearch
        {
            get { return _popupExtSearch; }
        }

        public PopupExtFilter PopupExtFilter
        {
            get { return _popupExtFilter; }
        }

        public Action FilterAction
        {
            get { return _filterAction; }
            set { _filterAction = value; }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _popupExtFilter.Button = BtnFilter;
            _popupExtSearch.Button = BtnSearch;

            _eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterData);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterData);
        }

        private void FilterData(IFilterData filterData)
        {
            _filter = filterData;

            if (_filterAction != null)
            {
                _filterAction();

                RaisePropertyChanged(() => IsFilterAnyField);
            }
        }

        public bool KeepAlive { get { return false; } }
    }
}