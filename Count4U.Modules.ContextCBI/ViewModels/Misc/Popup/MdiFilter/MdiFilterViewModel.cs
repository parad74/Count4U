using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter
{
    public class MdiFilterViewModel : CBIContextBaseViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _resetCommand;
        private readonly DelegateCommand _applyCommand;
        private readonly DelegateCommand _closeCommand;

        private readonly ObservableCollection<MdiFilterItemViewModel> _items;
        private readonly ObservableCollection<MenuFilterItemViewModel> _menuItems;

        public MdiFilterViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;

            _resetCommand = new DelegateCommand(ResetCommandExecuted);
            _applyCommand = new DelegateCommand(ApplyCommandExecuted);
            _closeCommand = new DelegateCommand(CloseCommandExecuted);

            _items = new ObservableCollection<MdiFilterItemViewModel>();
            _menuItems = new ObservableCollection<MenuFilterItemViewModel>();
        }

        public ObservableCollection<MdiFilterItemViewModel> Items
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

        public ObservableCollection<MenuFilterItemViewModel> MenuItems
        {
            get { return _menuItems; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            object obj = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.DashboardManagerItems, true);
            MdiFilterState state = obj as MdiFilterState;

            if (state != null)
            {
                Build(state);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        private void Build(MdiFilterState state)
        {
            if (state.Mdis != null)
            {
                _items.Clear();
                foreach (MdiFilterItem mdiItem in state.Mdis)
                {
                    MdiFilterItemViewModel viewModel = new MdiFilterItemViewModel();
                    viewModel.Text = mdiItem.Title;
                    viewModel.IsChecked = mdiItem.IsOpen;
                    viewModel.State = mdiItem;

                    _items.Add(viewModel);
                }
            }
            if (state.Menus != null)
            {
                _menuItems.Clear();

                foreach (MenuFilterItem menuItem in state.Menus.OrderBy(r => r.SortIndex))
                {
                    MenuFilterItemViewModel viewModel = new MenuFilterItemViewModel();
                    viewModel.Text = menuItem.Title;
                    viewModel.IsChecked = menuItem.IsOpen;
                    viewModel.Color = UserSettingsHelpers.StringToColor(menuItem.Color);
                    viewModel.State = menuItem;

                    _menuItems.Add(viewModel);
                }
            }
        }

        private void ApplyCommandExecuted()
        {
            foreach (MdiFilterItemViewModel item in _items)
            {
                item.State.IsOpen = item.IsChecked;
            }
            foreach (MenuFilterItemViewModel item in _menuItems)
            {
                item.State.Color = UserSettingsHelpers.ColorToString(item.Color);
                item.State.IsOpen = item.IsChecked;
            }

            MdiFilterState state = new MdiFilterState();
            state.Mdis = _items.Select(r => r.State).ToList();
            state.Menus = _menuItems.Select(r => r.State).ToList();
            _eventAggregator.GetEvent<MdiFilterChangedEvent>().Publish(state);
        }

        private void ResetCommandExecuted()
        {
            using (new CursorWait())
            {
                foreach (MdiFilterItemViewModel viewModel in _items)
                {
                    viewModel.IsChecked = true;
                }

                foreach (MenuFilterItemViewModel viewModel in _menuItems)
                {
                    viewModel.Color = Color.FromRgb(100, 193, 255);
                    viewModel.IsChecked = true;
                    viewModel.State.SortIndex = viewModel.State.SortIndexOriginal;
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