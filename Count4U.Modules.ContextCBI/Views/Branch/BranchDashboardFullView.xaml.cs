using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events.Misc;
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
using Microsoft.Practices.Unity;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.Views.Branch
{
    /// <summary>
    /// Interaction logic for BranchDashboardFullView.xaml
    /// </summary>
    public partial class BranchDashboardFullView : DashboardBaseView, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupFilterRegion = Common.RegionNames.PopupFilterBranchDashboard;
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchBranchDashboard;
        private const string DashboardName = "BranchDashboard";

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IDBSettings _dbSettings;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;

        private NavigationContext _navigationContext;

        public BranchDashboardFullView(
            BranchDashboardFullViewModel viewModel,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            IDBSettings dbSettings,
            INavigationRepository navigationRepository,
            IUnityContainer unityContainer,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
            : base(navigationRepository, unityContainer, regionManager)
        {            
            this.InitializeComponent();

            this._popupExtSearch = popupExtSearch;
            this._popupExtFilter = popupExtFilter;
            this._dbSettings = dbSettings;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._userSettingsManager = userSettingsManager;          

            this.DataContext = viewModel;

            this.Loaded += BranchDashboardFullView_Loaded;

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = null;
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.NavigationData = new BranchSearchData();
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = PopupFilterRegion;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.MdiFilterView;
//            _popupExtFilter.Height = 500;
            _popupExtFilter.ApplyForQuery = base.ApplyToFilterQuery(DashboardName);          
            _popupExtFilter.Init();
        }

        public CBIContext Context { get; set; }

        void BranchDashboardFullView_Loaded(object sender, RoutedEventArgs e)
        {
          
        }        

        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            bool dbFileMissed = UtilsNavigate.DataFileMissed(navigationContext, _contextCbiRepository, this._dbSettings, Window.GetWindow(this), this._userSettingsManager);           

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
            RegionManager.SetRegionName(backForward, Common.RegionNames.BranchDashboardBackForward);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.BranchDashboardBackForward);

            this._navigationContext = navigationContext;
            CBIContext? cbiContext = Utils.CBIContextFromNavigationParameters(navigationContext);
            if (cbiContext != null)
                Context = cbiContext.Value;

            this._mdiManager = new MdiManager(this.mdiContainer, 
                this._regionManager, 
                RegionsInfoBuild(), 
                DashboardName,
                this._userSettingsManager, 
                this._eventAggregator,
                this.BuildDefaultLayouts());
            this._mdiManager.MdiListChanged += MdiManager_MdiListChanged;
            this._mdiManager.BuildRegions();

            Utils.MainWindowTitleSet(WindowTitles.BranchDashboard, this._eventAggregator);


            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this._mdiManager != null)
                    this._mdiManager.MdiAllAppearanceApply();

                if (dbFileMissed)
                {
                    BranchDashboardFullViewModel viewModel = this.DataContext as BranchDashboardFullViewModel;
                    if (viewModel != null)
                    {
                        viewModel.RaisePropertyChanged();
                    }
                }
            }));

            this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Subscribe(MdiFilterRecalculate);
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

            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      backForward
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.BranchDashboardBackForward);

            this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Unsubscribe(MdiFilterRecalculate);
			this.Loaded -= BranchDashboardFullView_Loaded;
		}

        #endregion

        private MdiRegionLayoutCollection BuildDefaultLayouts()
        {
            MdiRegionLayoutCollection result = new MdiRegionLayoutCollection();

            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartMenu,
                ViewName = Common.ViewNames.MenuDashboardPartView,
                X = 10,
                Y = 10,
                Width = 300,
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLocation,
                ViewName = Common.ViewNames.LocationDashboardPartView,
                X = 320,
                Y = 10,
                Width = 315,
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLastInventors,
                ViewName = Common.ViewNames.LastInventorsDashboardPartView,
                X = 645,
                Y = 10,
                Width = 300,
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartCatalogInfoForCBI,
                ViewName = Common.ViewNames.CatalogInfoForCBIDashboardPartView,
                X = 10,
                Y = 220,
                Width = 300,
                Height = 185
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartReports,
                ViewName = Common.ViewNames.ReportsDashboardPartView,
                X = 645,
                Y = 220,
                Width = 300,
                Height = 185
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartIturim,
                ViewName = Common.ViewNames.IturimDashboardPartView,
                X = 320,
                Y = 220,
                Width = 300,
                Height = 185
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSection,
                ViewName = Common.ViewNames.SectionDashboardPartView,
                X = 10,
                Y = 420,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartBranch,
                ViewName = Common.ViewNames.BranchDashboardPartView,
                X = 320,
                Y = 420,
                Width = 315,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSupplier,
                ViewName = Common.ViewNames.SupplierDashboardPartView,
                X = 645,
                Y = 420,
                Width = 300,
                Height = 165
            });
			result.Items.Add(new MdiRegionLayout()
			{
				RegionName = Common.RegionNames.DashboardPartFamily,
				ViewName = Common.ViewNames.FamilyDashboardPartView,
				X = 695,
				Y = 450,
				Width = 300,
				Height = 165
			});
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartPlanogram,
                ViewName = Common.ViewNames.PlanogramPartView,
                X = 10,
                Y = 600,
                Width = 300,
                Height = 165
            });


			//result.Items.Add(new MdiRegionLayout()
			//{
			//	RegionName = Common.RegionNames.DashboardPartFromPda,
			//	ViewName = Common.ViewNames.FromPdaDashboardPartView,
			//	X = 320,
			//	Y = 220,
			//	Width = 315,
			//	Height = 175
			//});

            return result;
        }

        private List<MdiRegion> RegionsInfoBuild()
        {
            List<MdiRegion> result = new List<MdiRegion>();

            MdiRegionLayoutCollection defaults = BuildDefaultLayouts();
            MdiRegionLayout layout = null;

            {
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
                menu.Settings.Add(Common.NavigationSettings.MenuDashboardBranch, String.Empty);
                Utils.AddContextToDictionary(menu.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(menu.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, menu.Settings);
                result.Add(menu);
            }

            {
                MdiRegion location = new MdiRegion();
                location.RegionName = Common.RegionNames.DashboardPartLocation;
                location.ViewName = Common.ViewNames.LocationDashboardPartView;
                location.Title = MdiTitles.Location;
                layout = defaults.Get(location);
                location.Position = new Point(layout.X, layout.Y);
                location.Width = layout.Width;
                location.Height = layout.Height;
                location.DashboardName = DashboardName;
                location.Settings.Add(Common.NavigationSettings.LocationsPart, Common.NavigationSettings.LocationsPartBranch);
                Utils.AddDbContextToDictionary(location.Settings, Common.NavigationSettings.CBIDbContextBranch);
                Utils.AddContextToDictionary(location.Settings, CBIContext.Main);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, location.Settings);
                result.Add(location);
            }

            {
                MdiRegion inventors = new MdiRegion();
                inventors.RegionName = Common.RegionNames.DashboardPartLastInventors;
                inventors.ViewName = Common.ViewNames.LastInventorsDashboardPartView;
                inventors.Title = MdiTitles.LastInventorsForBranch;
                layout = defaults.Get(inventors);
                inventors.Position = new Point(layout.X, layout.Y);
                inventors.Width = layout.Width;
                inventors.Height = layout.Height;
                inventors.DashboardName = DashboardName;
                inventors.Settings.Add(Common.NavigationSettings.LastInventors, Common.NavigationSettings.LastInventorsForBranch);
                Utils.AddContextToDictionary(inventors.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(inventors.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventors.Settings);
                result.Add(inventors);
            }

            {
                MdiRegion catalog = new MdiRegion();
                catalog.RegionName = Common.RegionNames.DashboardPartCatalogInfoForCBI;
                catalog.ViewName = Common.ViewNames.CatalogInfoForCBIDashboardPartView;
                catalog.Title = MdiTitles.CatalogInfoForBranch;
                layout = defaults.Get(catalog);
                catalog.Position = new Point(layout.X, layout.Y);
                catalog.Width = layout.Width;
                catalog.Height = layout.Height;
                catalog.DashboardName = DashboardName;
                catalog.Settings.Add(Common.NavigationSettings.MdiPartOwnerDashboard, Common.NavigationSettings.MdiPartOwnerDashboardBranch);
                Utils.AddContextToDictionary(catalog.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(catalog.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, catalog.Settings);
                result.Add(catalog);
            }

            {
                MdiRegion reports = new MdiRegion();
                reports.RegionName = Common.RegionNames.DashboardPartReports;
                reports.ViewName = Common.ViewNames.ReportsDashboardPartView;
                reports.Title = MdiTitles.Reports;
                layout = defaults.Get(reports);
                reports.Position = new Point(layout.X, layout.Y);
                reports.Width = layout.Width;
                reports.Height = layout.Height;
                reports.DashboardName = DashboardName;
                Utils.AddContextToDictionary(reports.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(reports.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, reports.Settings);
                result.Add(reports);
            }

            {
                MdiRegion iturim = new MdiRegion();
                iturim.RegionName = Common.RegionNames.DashboardPartIturim;
                iturim.ViewName = Common.ViewNames.IturimDashboardPartView;
                iturim.Title = MdiTitles.Iturim;
                layout = defaults.Get(iturim);
                iturim.Position = new Point(layout.X, layout.Y);
                iturim.Width = layout.Width;
                iturim.Height = layout.Height;
                iturim.DashboardName = DashboardName;
                Utils.AddContextToDictionary(iturim.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(iturim.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, iturim.Settings);
                result.Add(iturim);
            }

            {
                MdiRegion section = new MdiRegion();
                section.RegionName = Common.RegionNames.DashboardPartSection;
                section.ViewName = Common.ViewNames.SectionDashboardPartView;
                section.Title = MdiTitles.Section;
                layout = defaults.Get(section);
                section.Position = new Point(layout.X, layout.Y);
                section.Width = layout.Width;
                section.Height = layout.Height;
                section.DashboardName = DashboardName;
                Utils.AddContextToDictionary(section.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(section.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, section.Settings);
                result.Add(section);
            }

            {
                MdiRegion branch = new MdiRegion();
                branch.RegionName = Common.RegionNames.DashboardPartBranch;
                branch.ViewName = Common.ViewNames.BranchDashboardPartView;
                branch.Title = MdiTitles.Branch;
                layout = defaults.Get(branch);
                branch.Position = new Point(layout.X, layout.Y);
                branch.Width = layout.Width;
                branch.Height = layout.Height;
                branch.DashboardName = DashboardName;
                Utils.AddContextToDictionary(branch.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(branch.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, branch.Settings);
                result.Add(branch);
            }

            {
                MdiRegion supplier = new MdiRegion();
                supplier.RegionName = Common.RegionNames.DashboardPartSupplier;
                supplier.ViewName = Common.ViewNames.SupplierDashboardPartView;
                supplier.Title = MdiTitles.Supplier;
                layout = defaults.Get(supplier);
                supplier.Position = new Point(layout.X, layout.Y);
                supplier.Width = layout.Width;
                supplier.Height = layout.Height;
                supplier.DashboardName = DashboardName;
                Utils.AddContextToDictionary(supplier.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(supplier.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, supplier.Settings);
                result.Add(supplier);
            }

			{
				MdiRegion family = new MdiRegion();
				family.RegionName = Common.RegionNames.DashboardPartFamily;
				family.ViewName = Common.ViewNames.FamilyDashboardPartView;
				family.Title = MdiTitles.Family;
				layout = defaults.Get(family);
				family.Position = new Point(layout.X, layout.Y);
				family.Width = layout.Width;
				family.Height = layout.Height;
				family.DashboardName = DashboardName;
				Utils.AddContextToDictionary(family.Settings, CBIContext.Main);
				Utils.AddDbContextToDictionary(family.Settings, Common.NavigationSettings.CBIDbContextBranch);
				NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, family.Settings);
				result.Add(family);
			}

            {
                MdiRegion planogram = new MdiRegion();
                planogram.RegionName = Common.RegionNames.DashboardPartPlanogram;
                planogram.ViewName = Common.ViewNames.PlanogramPartView;
                planogram.Title = MdiTitles.Planogram;
                layout = defaults.Get(planogram);
                planogram.Position = new Point(layout.X, layout.Y);
                planogram.Width = layout.Width;
                planogram.Height = layout.Height;
                planogram.DashboardName = DashboardName;
				Utils.AddContextToDictionary(planogram.Settings, CBIContext.Main);
                Utils.AddDbContextToDictionary(planogram.Settings, Common.NavigationSettings.CBIDbContextBranch);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, planogram.Settings);
                result.Add(planogram);
            }

			{

				//MdiRegion fromPda = new MdiRegion();
				//fromPda.RegionName = Common.RegionNames.DashboardPartFromPda;
				//fromPda.ViewName = Common.ViewNames.FromPdaDashboardPartView;
				//fromPda.Title = MdiTitles.GetFromPdaLog;
				//layout = defaults.Get(fromPda);
				//fromPda.Position = new Point(layout.X, layout.Y);
				//fromPda.Width = layout.Width;
				//fromPda.Height = layout.Height;
				//fromPda.DashboardName = DashboardName;
				//fromPda.Brush = System.Windows.Media.Brushes.MediumSeaGreen;
				//Utils.AddContextToDictionary(fromPda.Settings, CBIContext.Main);
				//Utils.AddDbContextToDictionary(fromPda.Settings, Common.NavigationSettings.CBIDbContextBranch);
				//NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, fromPda.Settings);
				//result.Add(fromPda);
			}

            return result;
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
            BranchDashboardFullViewModel viewModel = this.DataContext as BranchDashboardFullViewModel;
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
