using System;
using Count4U.Common.Constants;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SearchInventProductAdvancedFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

		public bool _isAggregate;
        private ISearchFieldViewModel _subViewModel;

        private DelegateCommand _searchCommand;

        public SearchInventProductAdvancedFieldViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

           // _isAggregate = true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildControl();
            BuildAnalyzeTable();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        public bool IsAggregate
        {
            get { return _isAggregate; }
            set
            {
                _isAggregate = value;
                RaisePropertyChanged(() => IsAggregate);

                using (new CursorWait())
                {
                    if (_subViewModel != null)
                    {
                        SearchInventProductAdvancedFieldSimpleViewModel simple = _subViewModel as SearchInventProductAdvancedFieldSimpleViewModel;
                        if (simple != null)
                        {
                            simple.Cancel();
                        }

                        SearchInventProductAdvancedFieldSumViewModel sum = _subViewModel as SearchInventProductAdvancedFieldSumViewModel;
                        if (sum != null)
                        {
                            sum.Cancel();
                        }
                    }
                    BuildControl();
                    BuildGrid();
                    _eventAggregator.GetEvent<SearchModeChangedEvent>().Publish(null); //rebuild reports menu
                    BuildAnalyzeTable();
                }
            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set
            {
                _searchCommand = value;

                if (_subViewModel != null)
                    _subViewModel.SearchCommand = _searchCommand;
            }
        }

        public bool CanSearch()
        {
            if (_subViewModel != null)
                return _subViewModel.CanSearch();

            return false;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            if (_subViewModel != null)
                return _subViewModel.GetReportContext();

            return ViewDomainContextEnum.InventProductAdvancedSearch;
        }

        public IFilterData BuildFilterData()
        {
            if (_subViewModel != null)
                return _subViewModel.BuildFilterData();

            return null;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			if (_subViewModel != null)
				return _subViewModel.BuildFilterData();

			return null;
		}

        public void Reset()
        {
            if (_subViewModel != null)
                _subViewModel.Reset();
        }

        public void ApplyFilterData(IFilterData data)
        {
            if (_subViewModel != null)
                _subViewModel.ApplyFilterData(data);
        }

        private void BuildControl()
        {
            if (_subViewModel != null)
            {
                _subViewModel.OnNavigatedFrom(null);
            }

            string viewName = _isAggregate ? Common.ViewNames.SearchInventProductAdvancedFieldSumView : Common.ViewNames.SearchInventProductAdvancedFieldSimpleView;

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.SearchFieldInventProductAdvancedGround, new Uri(viewName + query, UriKind.Relative));

            _subViewModel = Utils.GetViewModelFromRegion<ISearchFieldViewModel>(Common.RegionNames.SearchFieldInventProductAdvancedGround, _regionManager);

            _subViewModel.SearchCommand = _searchCommand;

            if (_searchCommand != null)
                _searchCommand.RaiseCanExecuteChanged();
        }

        private void BuildAnalyzeTable()
        {
            SearchInventProductAdvancedFieldSimpleViewModel simple = _subViewModel as SearchInventProductAdvancedFieldSimpleViewModel;
            if (simple != null)
            {
                simple.BuildAnalyzeTable();
            }

            SearchInventProductAdvancedFieldSumViewModel sum = _subViewModel as SearchInventProductAdvancedFieldSumViewModel;
            if (sum != null)
            {
                sum.BuildAnalyzeTable();
            }
        }

        private void BuildGrid()
        {
            SearchInventProductAdvancedViewModel gridViewModel = Utils.GetViewModelFromRegion<SearchInventProductAdvancedViewModel>(Common.RegionNames.SearchGridGround, _regionManager);
            gridViewModel.BuildControl(!_isAggregate);
        }
    }
}