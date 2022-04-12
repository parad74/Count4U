using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup
{
    public class MdiFilterViewModel : CBIContextBaseViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _resetCommand;
        private readonly DelegateCommand _applyCommand;
        private readonly DelegateCommand _closeCommand;

        private readonly ObservableCollection<MdiItemViewModel> _items;

        public MdiFilterViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;
            _items = new ObservableCollection<MdiItemViewModel>();
            _resetCommand = new DelegateCommand(ResetCommandExecuted);
            _applyCommand = new DelegateCommand(ApplyCommandExecuted);
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
        }

        public ObservableCollection<MdiItemViewModel> Items
        {
            get { return _items; }
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

            object obj = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.DashboardManagerItems, true);
            List<MdiFilterItem> mdiItems = obj as List<MdiFilterItem>;

            if (mdiItems != null)
            {
                _items.Clear();
                foreach (MdiFilterItem mdiItem in mdiItems)
                {
                    MdiItemViewModel viewModel = new MdiItemViewModel();
                    viewModel.Text = mdiItem.Title;
                    viewModel.IsChecked = mdiItem.IsOpen;
                    viewModel.State = mdiItem;

                    _items.Add(viewModel);
                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        private void ApplyCommandExecuted()
        {
            foreach (MdiItemViewModel item in _items)
            {
                item.State.IsOpen = item.IsChecked;
            }

            _eventAggregator.GetEvent<MdiChangedEvent>().Publish(_items.Select(r => r.State).ToList());

        }

        private void ResetCommandExecuted()
        {
            using (new CursorWait())
            {
                foreach (MdiItemViewModel viewModel in _items)
                {
                    viewModel.IsChecked = true;
                }

                _applyCommand.Execute();
            }
        }

        private void CloseCommandExecuted()
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            UtilsPopup.Close(View);
        }
    }
}