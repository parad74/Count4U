using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Extensions;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager;
using Count4U.Modules.ContextCBI.Views.Misc.Dashboard;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Helpers;
using Count4U.Common;
using Microsoft.Practices.Unity;
using WPF.MDI;
using Count4U.Common.Services.UICommandService;
using Count4U.Modules.ContextCBI.Events.Misc;

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for HomeDashboardView.xaml
    /// </summary>
    public partial class HomeDashboardView : DashboardBaseView, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupFilterRegion = Common.RegionNames.PopupFilterHomeDashboard;
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchBranchDashboard;
        private const string DashboardName = "MainDashboard";

        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;

        private NavigationContext _navigationContext;

        public HomeDashboardView(
            HomeDashboardViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IUserSettingsManager userSettingsManager,
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IUnityContainer unityContainer,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
            : base(navigationRepository, unityContainer, regionManager)
        {
            this.InitializeComponent();

            this._popupExtFilter = popupExtFilter;
            this._popupExtSearch = popupExtSearch;
            this._contextCbiRepository = contextCbiRepository;
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator = eventAggregator;

            this.DataContext = viewModel;

            this.Loaded += MainDashboardView_Loaded;
            this.tcMain.SelectionChanged += tcMain_SelectionChanged;

#if DEBUG
            tiSqlScript.Visibility = Visibility.Visible;
			//tiProcess.Visibility = Visibility.Visible;
#else 
            tiSqlScript.Visibility = Visibility.Collapsed;
			//tiProcess.Visibility = Visibility.Collapsed;
#endif

			_popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = null;
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = null;
            _popupExtSearch.CBIContext = CBIContext.Main;
            _popupExtSearch.CBIDbContext = Common.NavigationSettings.CBIDbContextCustomer;
            _popupExtSearch.ApplyForQuery = query =>
                {
                    query.Add(Common.NavigationSettings.SearchWithOnlyCBI, String.Empty);
                };

            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = PopupFilterRegion;
            _popupExtFilter.ViewModel = null;
            _popupExtFilter.View = Common.ViewNames.MdiFilterView;
            _popupExtFilter.CBIContext = CBIContext.Main;
            _popupExtFilter.CBIDbContext = Common.NavigationSettings.CBIDbContextCustomer;
            //_popupExtFilter.Height = 700;
            _popupExtFilter.ApplyForQuery = base.ApplyToFilterQuery(DashboardName);
            _popupExtFilter.Init();
        }

        void tcMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != tcMain) return;

            if (tiConfig.IsSelected)
            {
                this._mdiManager.Save();

                if (contentConfig.Content != null) return;

                using (new CursorWait())
                {
                    UriQuery query = new UriQuery();
                    Utils.AddContextToQuery(query, CBIContext.Main);
                    this._regionManager.RequestNavigate(Common.RegionNames.UserSettings, new Uri(Common.ViewNames.UserSettingsView + query, UriKind.Relative));
                }
            }

            if (tiPath.IsSelected)
            {

                if (contentPath.Content != null) return;

                using (new CursorWait())
                {
                    this._regionManager.RequestNavigate(Common.RegionNames.PathSettings, new Uri(Common.ViewNames.PathSettingsView, UriKind.Relative));
                }
            }

            if (tiExport.IsSelected)
            {

                if (contentExport.Content != null) return;

                using (new CursorWait())
                {
                    this._regionManager.RequestNavigate(Common.RegionNames.ZipExport, new Uri(Common.ViewNames.ZipExportView, UriKind.Relative));
                }
            }

            if (tiImport.IsSelected)
            {
                if (contentImport.Content != null) return;

                using (new CursorWait())
                {
					this._regionManager.RequestNavigate(Common.RegionNames.ZipImport, new Uri(Common.ViewNames.ZipImportView, UriKind.Relative));
                }
            }

			if (tiProcess.IsSelected)
			{
				if (contentProcess.Content != null) return;

				using (new CursorWait())
				{
					this._regionManager.RequestNavigate(Common.RegionNames.Process, new Uri(Common.ViewNames.ProcessAddEditGridView, UriKind.Relative));
				}
			}

		
            if (tiSqlScript.IsSelected)
            {
                if (contentSqlScript.Content != null) return;

                using (new CursorWait())
                {
                    this._regionManager.RequestNavigate(Common.RegionNames.SqlScriptSettings, new Uri(Common.ViewNames.SqlScriptSettingsView, UriKind.Relative));
                }
            }

        }

        void MainDashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            this._mdiManager.MdiAllAppearanceApply();
        }

        private MdiRegionLayoutCollection BuildDefaultLayouts()
        {
            MdiRegionLayoutCollection result = new MdiRegionLayoutCollection();

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartMenu,
                ViewName = Common.ViewNames.MenuDashboardPartView,
                X = 10,
                Y = 10,
                Width = 500,
                Height = 200
            });

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLastInventors,
                ViewName = Common.ViewNames.LastInventorsDashboardPartView,
                X = 520,
                Y = 10,
                Width = 430,
                Height = 200
            });

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLastCustomers,
                ViewName = Common.ViewNames.LastCustomersDashboardPartView,
                X = 10,
                Y = 220,
                Width = 500,
                Height = 150
            });

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLastCustomersBuild,
                ViewName = Common.ViewNames.LastCustomersDashboardPartView,
                X = 10,
                Y = 380,
                Width = 500,
                Height = 200
            });

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartHome,
                ViewName = Common.ViewNames.HomeDashboardPartView,
                X = 10,
                Y = 490,
                Width = 500,
                Height = 200
            });

            return result;
        }

        private List<MdiRegion> RegionsInfoBuild()
        {
            MdiRegionLayoutCollection defaults = BuildDefaultLayouts();
            MdiRegionLayout layout = null;

            MdiRegion menu = new MdiRegion();
            menu.RegionName = Common.RegionNames.DashboardPartMenu;
            menu.ViewName = Common.ViewNames.MenuDashboardPartView;
            menu.Title = MdiTitles.Menu;
            layout = defaults.Get(menu);
            menu.Position = new Point(layout.X, layout.Y);
            menu.Width = layout.Width;
            menu.Height = layout.Height;
            menu.MinWidth = 88;
            menu.MinHeight = 114;
            menu.DashboardName = DashboardName;
            menu.Settings.Add(Common.NavigationSettings.MenuDashboardMain, String.Empty);
            Utils.AddContextToDictionary(menu.Settings, CBIContext.Main);
            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, menu.Settings);

            MdiRegion inventors = new MdiRegion();
            inventors.RegionName = Common.RegionNames.DashboardPartLastInventors;
            inventors.ViewName = Common.ViewNames.LastInventorsDashboardPartView;
            inventors.Title = MdiTitles.LastInventors;
            layout = defaults.Get(inventors);
            inventors.Position = new Point(layout.X, layout.Y);
            inventors.Width = layout.Width;
            inventors.Height = layout.Height;
            inventors.DashboardName = DashboardName;
            inventors.Settings.Add(Common.NavigationSettings.LastInventors, Common.NavigationSettings.LastInventorsForMain);
            Utils.AddContextToDictionary(inventors.Settings, CBIContext.History);
            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventors.Settings);

            MdiRegion last = new MdiRegion();
            last.RegionName = Common.RegionNames.DashboardPartLastCustomers;
            last.ViewName = Common.ViewNames.LastCustomersDashboardPartView;
            last.Title = MdiTitles.LastCustomers;
            layout = defaults.Get(last);
            last.Position = new Point(layout.X, layout.Y);
            last.Width = layout.Width;
            last.Height = layout.Height;
            last.DashboardName = DashboardName;
            last.Settings.Add(Common.NavigationSettings.LastCustomers, Common.NavigationSettings.LastCustomersInInventory);
            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, last.Settings);

            MdiRegion lastBuild = new MdiRegion();
            lastBuild.RegionName = Common.RegionNames.DashboardPartLastCustomersBuild;
            lastBuild.ViewName = Common.ViewNames.LastCustomersDashboardPartView;
            lastBuild.Title = MdiTitles.LastCustomersBuild;
            layout = defaults.Get(lastBuild);
            lastBuild.Position = new Point(layout.X, layout.Y);
            lastBuild.Width = layout.Width;
            lastBuild.Height = layout.Height;
            lastBuild.DashboardName = DashboardName;
            lastBuild.Settings.Add(Common.NavigationSettings.LastCustomers, Common.NavigationSettings.LastCustomersBuild);
            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, lastBuild.Settings);

            MdiRegion home = new MdiRegion();
            home.RegionName = Common.RegionNames.DashboardPartHome;
            home.ViewName = Common.ViewNames.HomeDashboardPartView;
            home.Title = MdiTitles.Application;
            layout = defaults.Get(home);
            home.Position = new Point(layout.X, layout.Y);
            home.Width = layout.Width;
            home.Height = layout.Height;
            home.DashboardName = DashboardName;
            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, home.Settings);

            return new List<MdiRegion>
                       {
                           menu,                          
                           inventors,
                           last,
                           lastBuild,
                       //    home
                       };
        }


        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._navigationContext = navigationContext;

            this._eventAggregator.GetEvent<ApplicationConfSetPreChangedEvent>().Subscribe(ApplicationConfigurationPreChanged);
            this._eventAggregator.GetEvent<ApplicationConfSetChangedEvent>().Subscribe(ApplicationConfigurationChanged);
            this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Subscribe(MdiFilterRecalculate);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            Utils.MainWindowTitleSet(WindowTitles.MainDashboard, this._eventAggregator);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeEmpty);

            RegionManager.SetRegionName(backForward, Common.RegionNames.MainDashboardBackForward);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.MainDashboardBackForward);

            this._mdiManager = new MdiManager(this.mdiContainer, 
                                              this._regionManager, 
                                              this.RegionsInfoBuild(),
                                              DashboardName, 
                                              this._userSettingsManager, 
                                              this._eventAggregator,
                                              BuildDefaultLayouts());

            this._mdiManager.MdiListChanged += MdiManager_MdiListChanged;
            this._mdiManager.BuildRegions();

            RegionManager.SetRegionName(contentConfig, Common.RegionNames.UserSettings);
            RegionManager.SetRegionName(contentPath, Common.RegionNames.PathSettings);
            RegionManager.SetRegionName(contentExport, Common.RegionNames.ZipExport);
            RegionManager.SetRegionName(contentImport, Common.RegionNames.ZipImport);
            RegionManager.SetRegionName(contentSqlScript, Common.RegionNames.SqlScriptSettings);
			RegionManager.SetRegionName(contentProcess, Common.RegionNames.Process);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

			this._mdiManager.MdiListChanged -= MdiManager_MdiListChanged;
			this._mdiManager.Close();

            INavigationAware userSettings = contentConfig.Content as INavigationAware;
            if (userSettings != null)
            {
                userSettings.OnNavigatedFrom(navigationContext);
            }

            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        backForward,  
                                                        contentConfig,
                                                        contentPath,
                                                        contentExport,
                                                        contentImport,
                                                        contentSqlScript ,
														contentProcess
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.MainDashboardBackForward);
            this._regionManager.Regions.Remove(Common.RegionNames.UserSettings);
            this._regionManager.Regions.Remove(Common.RegionNames.PathSettings);
            this._regionManager.Regions.Remove(Common.RegionNames.ZipExport);
            this._regionManager.Regions.Remove(Common.RegionNames.ZipImport);
		 this._regionManager.Regions.Remove(Common.RegionNames.SqlScriptSettings);
		 this._regionManager.Regions.Remove(Common.RegionNames.Process);

            this._eventAggregator.GetEvent<ApplicationConfSetPreChangedEvent>().Unsubscribe(ApplicationConfigurationPreChanged);
            this._eventAggregator.GetEvent<ApplicationConfSetChangedEvent>().Unsubscribe(ApplicationConfigurationChanged);
            this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Unsubscribe(MdiFilterRecalculate);

			this.Loaded -= MainDashboardView_Loaded;
			this.tcMain.SelectionChanged -= tcMain_SelectionChanged;

		}

		#endregion

		private void ApplicationConfigurationChanged(object o)
        {
            this._mdiManager.BuildRegions();
            this._mdiManager.MdiAllAppearanceApply();
        }

        private void ApplicationConfigurationPreChanged(object o)
        {
            this._mdiManager.Clear();
        }

        void MdiManager_MdiListChanged(object sender, bool isAllMdiOpen)
        {
            FilterButtonStateRecalculate();
        }

        public bool KeepAlive { get { return false; } }

        private void MdiFilterRecalculate(object state)
        {
            FilterButtonStateRecalculate();
        }

        private void FilterButtonStateRecalculate()
        {
            HomeDashboardViewModel viewModel = this.DataContext as HomeDashboardViewModel;
            if (viewModel != null)
            {
                bool isAll = _mdiManager.IsAllMdi();
                MenuDashboardPartViewModel menuViewModel = Utils.GetViewModelFromRegion<MenuDashboardPartViewModel>(Common.RegionNames.DashboardPartMenu, _regionManager);
                if (menuViewModel != null)
                {
                    isAll = isAll && menuViewModel.Commands.All(r => r.IsVisible);
                }

                viewModel.IsMdiAll = isAll;
            }
        }
    }
}
