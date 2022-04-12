using System;
using System.Windows;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.ViewModel.Filter
{
    public class FilterViewModel : CBIContextBaseViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;

        private readonly DelegateCommand _resetCommand;
        private readonly DelegateCommand _applyCommand;
        private readonly DelegateCommand _closeCommand;

        private ISearchFieldViewModel _fieldViewModel;

        private readonly IEventAggregator _eventAggregator;

        public FilterViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            )
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;

            _resetCommand = new DelegateCommand(ResetCommandExecuted);
            _applyCommand = new DelegateCommand(ApplyCommandExecuted, ApplyCommandCanExecute);
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
        }

        public FrameworkElement View { get; set; }

        public DelegateCommand ResetCommand
        {
            get { return _resetCommand; }
        }

        public DelegateCommand ApplyCommand
        {
            get { return _applyCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            IFilterData filterData = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Filter, true) as IFilterData;
            if (filterData == null)
                return;

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.FilterFieldGround, new Uri(filterData.FieldViewName + query, UriKind.Relative));
            _fieldViewModel = Utils.GetViewModelFromRegion<ISearchFieldViewModel>(Common.RegionNames.FilterFieldGround, _regionManager);
            _fieldViewModel.ApplyFilterData(filterData);

            _fieldViewModel.SearchCommand = _applyCommand;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void CloseCommandExecuted()
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            UtilsPopup.Close(View);
        }

        public void EnterPressed()
        {
            _applyCommand.Execute();
            _closeCommand.Execute();
        }


        private bool ApplyCommandCanExecute()
        {
			if (_fieldViewModel == null) return false;
            return _fieldViewModel.CanSearch();
        }

        private void ApplyCommandExecuted()
        {
            using (new CursorWait())
            {
                _eventAggregator.GetEvent<FilterEvent<IFilterData>>().Publish(_fieldViewModel.BuildFilterData());
            }
        }

        private void ResetCommandExecuted()
        {
            using (new CursorWait())
            {
                _fieldViewModel.Reset();
                _applyCommand.Execute();
            }
        }
    }
}