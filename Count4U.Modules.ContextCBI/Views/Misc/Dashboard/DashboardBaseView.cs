using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.CustomControls.ImageButton;
using Count4U.Model;
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.Views.Misc.Dashboard
{
    public class DashboardBaseView : UserControl, INavigationAware
    {
        protected readonly INavigationRepository _navigationRepository;
        protected readonly IUnityContainer _unityContainer;
        protected readonly IRegionManager _regionManager;

        protected MdiManager _mdiManager;

        public DashboardBaseView()
        {

        }

        public DashboardBaseView(
            INavigationRepository navigationRepository,
            IUnityContainer unityContainer,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _navigationRepository = navigationRepository;
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        protected Action<UriQuery> ApplyToFilterQuery(string dashboardName)
        {
            return query =>
                {
                    MdiFilterState state = new MdiFilterState();
                    List<MdiFilterItem> queryMdis = new List<MdiFilterItem>();
                    foreach (MdiRegion mdiRegion in _mdiManager.Regions)
                    {
                        queryMdis.Add(new MdiFilterItem()
                            {
                                DashboardName = dashboardName,
                                IsOpen = mdiRegion.IsOpen,
                                RegionName = mdiRegion.RegionName,
                                Title = mdiRegion.Title,
                                ViewName = mdiRegion.ViewName,
                            });
                    }

                    List<MenuFilterItem> queryMenus = new List<MenuFilterItem>();
                    MenuDashboardPartViewModel menuViewModel = Utils.GetViewModelFromRegion<MenuDashboardPartViewModel>(Common.RegionNames.DashboardPartMenu, _regionManager);
                    if (menuViewModel != null)
                    {
                        foreach (MenuDashboardCommandViewModel command in menuViewModel.Commands)
                        {
                            string title = command.Name;
                            UICommand uiCommand = command.Command as UICommand;
                            if (uiCommand != null)
                            {
                                title = uiCommand.Title;
                            }
                            queryMenus.Add(new MenuFilterItem()
                                {
                                    DashboardName = command.DashboardName,
                                    IsOpen = command.IsVisible,
                                    Name = command.Name,
                                    PartName = command.PartName,
                                    SortIndex = command.SortIndex,
                                    SortIndexOriginal = command.SortIndexOriginal,
                                    Title = title,
                                    Color = UserSettingsHelpers.ColorToString(command.BackgroundColor),
                                });
                        }
                    }

                    state.Mdis = queryMdis;
                    state.Menus = queryMenus;
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, state, Common.NavigationObjects.DashboardManagerItems);
                };
        }
    }
}