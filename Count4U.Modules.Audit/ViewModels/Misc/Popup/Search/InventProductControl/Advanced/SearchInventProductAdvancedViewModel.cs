using System;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SearchInventProductAdvancedViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private ISearchFieldViewModel _searchFieldViewModel;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private ISearchGridViewModel _subViewModel;

        private Action<bool> _isBusy;
		public bool IsSimple { get; set; }

        public SearchInventProductAdvancedViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public FrameworkElement View { get; set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			BuildControl(this.IsSimple);	   //false
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        public void Search()
        {
            if (_subViewModel != null)
                _subViewModel.Search();
        }

        public SelectParams BuildSelectParams()
        {
            if (_subViewModel != null)
                return _subViewModel.BuildSelectParams();

            return null;
        }

        public ISearchFieldViewModel SearchFieldViewModel
        {
            set
            {
                _searchFieldViewModel = value;
                if (_subViewModel != null)
                {
                    _subViewModel.SearchFieldViewModel = _searchFieldViewModel;
                }
            }
        }

        public void CanSearch(bool isCanSearch)
        {
            if (_subViewModel != null)
            {
                _subViewModel.CanSearch(isCanSearch);
            }
        }

        public Action<bool> IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                if (_subViewModel != null)
                {
                    _subViewModel.IsBusy = _isBusy;
                }
            }
        }
       
        public void BuildControl(bool isSimple)
        {
            if (_subViewModel != null)
            {
                _subViewModel.OnNavigatedFrom(null);
            }

            string viewName = isSimple ? Common.ViewNames.SearchInventProductAdvancedGridSimpleView : Common.ViewNames.SearchInventProductAdvancedGridSumView;

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.SearchGridInventProductAdvancedGround, new Uri(viewName + query, UriKind.Relative));

            _subViewModel = Utils.GetViewModelFromRegion<ISearchGridViewModel>(Common.RegionNames.SearchGridInventProductAdvancedGround, _regionManager);
            if (_subViewModel != null)
            {
                _subViewModel.IsBusy = this.IsBusy;
                _subViewModel.SearchFieldViewModel = _searchFieldViewModel;
            }
        }
    }
}