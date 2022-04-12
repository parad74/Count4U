using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.UserSettings.Menu;
using Count4U.Common.View.DragDrop;
using Count4U.Common.View.DragDrop.Utilities;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using NLog;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu
{
    public class MenuDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild, IDropTarget//, IDragSource
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _unityContainer;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<MenuDashboardCommandViewModel> _commands;

        public MenuDashboardPartViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            IUnityContainer unityContainer,
            IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
            _userSettingsManager = userSettingsManager;
            _unityContainer = unityContainer;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            _commands = new ObservableCollection<MenuDashboardCommandViewModel>();
        }

        public ObservableCollection<MenuDashboardCommandViewModel> Commands
        {
            get { return _commands; }
        }

        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigatedTo(navigationContext);
        }

        private void NavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<MdiFilterChangedEvent>().Subscribe(MdiFilter);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MenuDashboardMain))
            {
                this.HomeCommandsBuild();
            }
            else if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MenuDashboardCustomer))
            {
                this.CustomerCommandsBuild();
            }
            else if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MenuDashboardBranch))
            {
                this.BranchCommandsBuild();
            }
            else if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.MenuDashboardInventor))
            {
                this.InventorCommandsBuild();
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        #endregion

        private void CustomerCommandsBuild()
        {
            CustomerDashboardMenuBuilder builder = _unityContainer.Resolve<CustomerDashboardMenuBuilder>();
            IEnumerable<MenuDashboardCommandViewModel> commands = builder.CommandsBuild();
            ApplyCommands(commands);
        }

        private void BranchCommandsBuild()
        {
            BranchDashboardMenuBuilder builder = _unityContainer.Resolve<BranchDashboardMenuBuilder>();
            IEnumerable<MenuDashboardCommandViewModel> commands = builder.CommandsBuild();
            ApplyCommands(commands);
        }

        private void InventorCommandsBuild()
        {
            InventorDashboardMenuBuilder builder = _unityContainer.Resolve<InventorDashboardMenuBuilder>();
            IEnumerable<MenuDashboardCommandViewModel> commands = builder.CommandsBuild();
            ApplyCommands(commands);
        }

        private void HomeCommandsBuild()
        {
            HomeDashboardMenuBuilder builder = _unityContainer.Resolve<HomeDashboardMenuBuilder>();
            IEnumerable<MenuDashboardCommandViewModel> commands = builder.CommandsBuild();
            ApplyCommands(commands);
        }

        private void ApplyCommands(IEnumerable<MenuDashboardCommandViewModel> commands)
        {
            foreach (MenuDashboardCommandViewModel viewModel in commands)
            {
                viewModel.SortIndexOriginal = viewModel.SortIndex;

                MenuElement menuElement = _userSettingsManager.MenuGet(viewModel.Name, viewModel.PartName, viewModel.DashboardName);
                if (menuElement != null)
                {
                    Color def = Color.FromRgb(100, 193, 255);
                    Color settingsColor = UserSettingsHelpers.StringToColor(menuElement.BackgroundColor);
                    if (settingsColor == Color.FromRgb(255, 255, 255))
                        settingsColor = def;
                    viewModel.BackgroundColor = settingsColor;
                    viewModel.IsVisible = menuElement.IsVisible;
                    viewModel.SortIndex = menuElement.SortIndex;
                }
            }

            foreach (MenuDashboardCommandViewModel viewModel in commands.OrderBy(r => r.SortIndex))
            {
                Commands.Add(viewModel);
            }
        }

        public void Refresh()
        {

        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<MdiFilterChangedEvent>().Unsubscribe(MdiFilter);
        }
        #region Drag&Drop
        public void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                                   DragDropEffects.Copy | DragDropEffects.Move :
                                   DragDropEffects.None;


        }

        public void Dropped(IDropInfo dropInfo)
        {

        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (DefaultDropHandler.CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            using (new CursorWait())
            {
                var insertIndex = dropInfo.InsertIndex;
                var destinationList = DefaultDropHandler.GetList(dropInfo.TargetCollection);
                var data = DefaultDropHandler.ExtractData(dropInfo.Data);

                if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
                {
                    var sourceList = DefaultDropHandler.GetList(dropInfo.DragInfo.SourceCollection);

                    foreach (var o in data)
                    {
                        var index = sourceList.IndexOf(o);

                        if (index != -1)
                        {
                            sourceList.RemoveAt(index);

                            if (sourceList == destinationList && index < insertIndex)
                            {
                                --insertIndex;
                            }
                        }
                    }
                }

                foreach (var o in data)
                {
                    destinationList.Insert(insertIndex++, o);
                }

                foreach (MenuDashboardCommandViewModel viewModel in _commands)
                {
                    viewModel.SortIndex = _commands.IndexOf(viewModel);
                }

                SaveToDb();
            }
        }
        #endregion

        private void SaveToDb()
        {
            try
            {
                foreach (MenuDashboardCommandViewModel viewModel in _commands)
                {
                    MenuElement menuElement = _userSettingsManager.MenuGet(viewModel.Name, viewModel.PartName, viewModel.DashboardName);
                    if (menuElement == null)
                    {
                        menuElement = new MenuElement();
                        menuElement.Key = viewModel.Key;
                        menuElement.Name = viewModel.Name;
                        menuElement.PartName = viewModel.PartName;
                        menuElement.DashboardName = viewModel.DashboardName;
                        menuElement.BackgroundColor = UserSettingsHelpers.ColorToString(viewModel.BackgroundColor);
                        menuElement.IsVisible = viewModel.IsVisible;
                        menuElement.SortIndex = viewModel.SortIndex;

                        _userSettingsManager.MenuInsert(menuElement);
                    }
                    else
                    {
                        menuElement.SortIndex = viewModel.SortIndex;
                        menuElement.IsVisible = viewModel.IsVisible;
                        menuElement.BackgroundColor = UserSettingsHelpers.ColorToString(viewModel.BackgroundColor);

                        _userSettingsManager.MenuUpdate(menuElement);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("SaveToDb", exc);
            }
        }

        private void MdiFilter(MdiFilterState mdiFilterState)
        {
            foreach (MenuFilterItem item in mdiFilterState.Menus)
            {
                MenuDashboardCommandViewModel command = _commands.FirstOrDefault(r => r.Name == item.Name && r.PartName == item.PartName && r.DashboardName == item.DashboardName);

                if (command != null)
                {
                    if (item.IsOpen != command.IsVisible)
                    {
                        command.IsVisible = item.IsOpen;
                    }

                    if (item.IsOpen != command.IsVisible)
                    {
                        command.IsVisible = item.IsOpen;
                    }

                    command.SortIndex = item.SortIndex;

                    Color color = UserSettingsHelpers.StringToColor(item.Color);
                    if (color != command.BackgroundColor)
                    {
                        command.BackgroundColor = color;
                    }
                }
            }

            List<MenuDashboardCommandViewModel> items = _commands.ToList();
            _commands.Clear();

            foreach (MenuDashboardCommandViewModel viewModel in items.OrderBy(r=>r.SortIndex))
            {
                _commands.Add(viewModel);
            }

            SaveToDb();

            _eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Publish(null);
        }
    }
}