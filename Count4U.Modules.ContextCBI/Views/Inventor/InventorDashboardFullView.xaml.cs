using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.Interfaces;
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
using WPF.MDI;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Model.Interface;

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorDashboardFullView.xaml
    /// </summary>
    public partial class InventorDashboardFullView : DashboardBaseView, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupFilterRegion = Common.RegionNames.PopupFilterInventorDashboard;
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchInventorDashboard;

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;
		private readonly IDBSettings _dbSettings;

        private const string DashboardName = "InventorDashboard";

        private NavigationContext _navigationContext;        

        public InventorDashboardFullView()
        {
            
        }

        public InventorDashboardFullView(
            InventorDashboardFullViewModel viewModel,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            IUnityContainer unityContainer,
            INavigationRepository navigationRepository,
			 IDBSettings dbSettings,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
            : base(navigationRepository, unityContainer, regionManager)
        {     
            this.InitializeComponent();

            this.DataContext = viewModel;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._userSettingsManager = userSettingsManager;
            this._popupExtSearch = popupExtSearch;
            this._popupExtFilter = popupExtFilter;

			this._dbSettings = dbSettings;

            this.Loaded += InventorDashboardFullView_Loaded;

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = null;
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.NavigationData = new InventorSearchData();
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = PopupFilterRegion;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.MdiFilterView;
//            _popupExtFilter.Height = 500;
            _popupExtFilter.ApplyForQuery = base.ApplyToFilterQuery(DashboardName);          
            _popupExtFilter.Init();
        }

        void InventorDashboardFullView_Loaded(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this._mdiManager.MdiAllAppearanceApply();
            stopwatch.Stop();
            System.Diagnostics.Debug.Print("InventorDashboardFullView_Loaded " + stopwatch.Elapsed.TotalSeconds.ToString());
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
                Width = 300,
                Height = 180
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartCatalogInfoForCBI,
                ViewName = Common.ViewNames.CatalogInfoForCBIDashboardPartView,
                X = 10,
                Y = 200,
                Width = 300,
                Height = 175
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartPda,
                ViewName = Common.ViewNames.PdaDashboardPartView,
                X = 320,
                Y = 10,
                Width = 315,
                Height = 180
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartInventProduct,
                ViewName = Common.ViewNames.InventProductPartView,
                X = 320,
                Y = 10,
                Width = 315,
                Height = 180
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartFromPda,
                ViewName = Common.ViewNames.FromPdaDashboardPartView,
                X = 320,
                Y = 200,
                Width = 315,
                Height = 175
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartInventorStatus,
                ViewName = Common.ViewNames.InventorStatusDashboardPartView,
                X = 645,
                Y = 10,
                Width = 300,
                Height = 180
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartReports,
                ViewName = Common.ViewNames.ReportsDashboardPartView,
                X = 645,
                Y = 200,
                Width = 300,
                Height = 175
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartIturim,
                ViewName = Common.ViewNames.IturimDashboardPartView,
                X = 10,
                Y = 385,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLocation,
                ViewName = Common.ViewNames.LocationDashboardPartView,
                X = 645,
                Y = 385,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSection,
                ViewName = Common.ViewNames.SectionDashboardPartView,
                X = 320,
                Y = 385,
                Width = 315,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartInventor,
                ViewName = Common.ViewNames.InventorDashboardPartView,
                X = 10,
                Y = 560,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardParthInventProductSimple,
                ViewName = Common.ViewNames.InventProductSimplePartView,
                X = 320,
                Y = 560,
                Width = 315,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardParthInventProductSum,
                ViewName = Common.ViewNames.InventProductSumPartView,
                X = 645,
                Y = 560,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSupplier,
                ViewName = Common.ViewNames.SupplierDashboardPartView,
				X = 320,
                Y = 735,
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
                X = 320,
                Y = 735,
                Width = 315,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartStatistic,
                ViewName = Common.ViewNames.StatisticDashboardPartView,
                X = 645,
                Y = 735,
                Width = 300,
                Height = 165
            });

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
                menu.Settings.Add(Common.NavigationSettings.MenuDashboardInventor, String.Empty);
                Utils.AddContextToDictionary(menu.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(menu.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, menu.Settings);
                result.Add(menu);
            }

            {
                MdiRegion catalog = new MdiRegion();
                catalog.RegionName = Common.RegionNames.DashboardPartCatalogInfoForCBI;
                catalog.ViewName = Common.ViewNames.CatalogInfoForCBIDashboardPartView;
                catalog.Title = MdiTitles.CatalogInfoForInventor;
                layout = defaults.Get(catalog);
                catalog.Position = new Point(layout.X, layout.Y);
                catalog.Width = layout.Width;
                catalog.Height = layout.Height;
                catalog.DashboardName = DashboardName;
                catalog.Settings.Add(Common.NavigationSettings.MdiPartOwnerDashboard, Common.NavigationSettings.MdiPartOwnerDashboardInventor);
                Utils.AddContextToDictionary(catalog.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(catalog.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, catalog.Settings);
                result.Add(catalog);
            }

            {
//            MdiRegion pda = new MdiRegion();
//            pda.RegionName = Common.RegionNames.DashboardPartPda;
//            pda.ViewName = Common.ViewNames.PdaDashboardPartView;
//            pda.Title = MdiTitles.PdaSendData;
//                layout = defaults.Get(pda);
//                pda.Position = new Point(layout.X, layout.Y);
//                pda.Width = layout.Width;
//                pda.Height = layout.Height;
//            pda.DashboardName = DashboardName;
//            pda.Brush = new SolidColorBrush(Color.FromRgb(0x99, 0xcc, 0x0));
//            Utils.AddContextToDictionary(pda.Settings, CBIContext.History);
//            Utils.AddDbContextToDictionary(pda.Settings, Common.NavigationSettings.CBIDbContextInventor);
//            NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext,
//                pda.Settings);
//                result.Add(pda);
            }

            {
                MdiRegion inventProduct = new MdiRegion();
                inventProduct.RegionName = Common.RegionNames.DashboardPartInventProduct;
                inventProduct.ViewName = Common.ViewNames.InventProductPartView;
                inventProduct.Title = MdiTitles.InventProduct;
                layout = defaults.Get(inventProduct);
                inventProduct.Position = new Point(layout.X, layout.Y);
                inventProduct.Width = layout.Width;
                inventProduct.Height = layout.Height;
                inventProduct.DashboardName = DashboardName;
                inventProduct.Brush = new SolidColorBrush(Color.FromRgb(0x99, 0xcc, 0x0));
                Utils.AddContextToDictionary(inventProduct.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(inventProduct.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventProduct.Settings);
                result.Add(inventProduct);
            }

            {
                MdiRegion fromPda = new MdiRegion();
                fromPda.RegionName = Common.RegionNames.DashboardPartFromPda;
                fromPda.ViewName = Common.ViewNames.FromPdaDashboardPartView;
                fromPda.Title = MdiTitles.GetFromPdaLog;
                layout = defaults.Get(fromPda);
                fromPda.Position = new Point(layout.X, layout.Y);
                fromPda.Width = layout.Width;
                fromPda.Height = layout.Height;
                fromPda.DashboardName = DashboardName;
                fromPda.Brush = Brushes.MediumSeaGreen;
                Utils.AddContextToDictionary(fromPda.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(fromPda.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, fromPda.Settings);
                result.Add(fromPda);
            }

            {
                MdiRegion inventorStatus = new MdiRegion();
                inventorStatus.RegionName = Common.RegionNames.DashboardPartInventorStatus;
                inventorStatus.ViewName = Common.ViewNames.InventorStatusDashboardPartView;
                inventorStatus.Title = MdiTitles.InventorStatus;
                layout = defaults.Get(inventorStatus);
                inventorStatus.Position = new Point(layout.X, layout.Y);
                inventorStatus.Width = layout.Width;
                inventorStatus.Height = layout.Height;
                inventorStatus.DashboardName = DashboardName;
                inventorStatus.Brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x68, 0x20));
                Utils.AddContextToDictionary(inventorStatus.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(inventorStatus.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventorStatus.Settings);
                result.Add(inventorStatus);
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
                Utils.AddContextToDictionary(reports.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(reports.Settings, Common.NavigationSettings.CBIDbContextInventor);
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
                Utils.AddContextToDictionary(iturim.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(iturim.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, iturim.Settings);
                result.Add(iturim);
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
                location.Settings.Add(Common.NavigationSettings.LocationsPart, Common.NavigationSettings.LocationsPartInventor);
                Utils.AddContextToDictionary(location.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(location.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, location.Settings);
                result.Add(location);
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
                Utils.AddContextToDictionary(section.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(section.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, section.Settings);
                result.Add(section);
            }

            {
                MdiRegion inventor = new MdiRegion();
                inventor.RegionName = Common.RegionNames.DashboardPartInventor;
                inventor.ViewName = Common.ViewNames.InventorDashboardPartView;
                inventor.Title = MdiTitles.Inventor;
                layout = defaults.Get(inventor);
                inventor.Position = new Point(layout.X, layout.Y);
                inventor.Width = layout.Width;
                inventor.Height = layout.Height;
                inventor.DashboardName = DashboardName;
                Utils.AddContextToDictionary(inventor.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(inventor.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventor.Settings);
                result.Add(inventor);
            }

            {
                MdiRegion inventProductSimple = new MdiRegion();
                inventProductSimple.RegionName = Common.RegionNames.DashboardParthInventProductSimple;
                inventProductSimple.ViewName = Common.ViewNames.InventProductSimplePartView;
                inventProductSimple.Title = MdiTitles.InventProductSimple;
                layout = defaults.Get(inventProductSimple);
                inventProductSimple.Position = new Point(layout.X, layout.Y);
                inventProductSimple.Width = layout.Width;
                inventProductSimple.Height = layout.Height;
                inventProductSimple.DashboardName = DashboardName;
                Utils.AddContextToDictionary(inventProductSimple.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(inventProductSimple.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventProductSimple.Settings);
                result.Add(inventProductSimple);
            }

            {
                MdiRegion inventProductSum = new MdiRegion();
                inventProductSum.RegionName = Common.RegionNames.DashboardParthInventProductSum;
                inventProductSum.ViewName = Common.ViewNames.InventProductSumPartView;
                inventProductSum.Title = MdiTitles.InventProductSum;
                layout = defaults.Get(inventProductSum);
                inventProductSum.Position = new Point(layout.X, layout.Y);
                inventProductSum.Width = layout.Width;
                inventProductSum.Height = layout.Height;
                inventProductSum.DashboardName = DashboardName;
                Utils.AddContextToDictionary(inventProductSum.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(inventProductSum.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventProductSum.Settings);
                result.Add(inventProductSum);
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
                Utils.AddContextToDictionary(supplier.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(supplier.Settings, Common.NavigationSettings.CBIDbContextInventor);
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
				Utils.AddContextToDictionary(family.Settings, CBIContext.History);
				Utils.AddDbContextToDictionary(family.Settings, Common.NavigationSettings.CBIDbContextInventor);
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
                Utils.AddContextToDictionary(planogram.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(planogram.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, planogram.Settings);
                result.Add(planogram);
            }

            {
                MdiRegion statistic = new MdiRegion();
                statistic.RegionName = Common.RegionNames.DashboardPartStatistic;
                statistic.ViewName = Common.ViewNames.StatisticDashboardPartView;
                statistic.Title = MdiTitles.Statistic;
                layout = defaults.Get(statistic);
                statistic.Position = new Point(layout.X, layout.Y);
                statistic.Width = layout.Width;
                statistic.Height = layout.Height;
                statistic.DashboardName = DashboardName;
                Utils.AddContextToDictionary(statistic.Settings, CBIContext.History);
                Utils.AddDbContextToDictionary(statistic.Settings, Common.NavigationSettings.CBIDbContextInventor);
                NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, statistic.Settings);
                result.Add(statistic);
            }

            return result;
        }


        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

			//New add 25/04/2019
			bool dbFileMissed = UtilsNavigate.DataFileMissed(navigationContext, this._contextCbiRepository, this._dbSettings, Window.GetWindow(this), this._userSettingsManager);           

            Utils.MainWindowTitleSet(WindowTitles.InventorDashboard, this._eventAggregator);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);
            RegionManager.SetRegionName(backForward, RegionNames.InventorDashboardBackForward);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.InventorDashboardBackForward);

            Stopwatch watch = Stopwatch.StartNew();

            this._navigationContext = navigationContext;
            this._mdiManager = new MdiManager(this.mdiContainer, this._regionManager,
                                                          this.RegionsInfoBuild(), DashboardName,
                                                          this._userSettingsManager, this._eventAggregator,
                                                          this.BuildDefaultLayouts());
            this._mdiManager.MdiListChanged += MdiManager_MdiListChanged;
            this._mdiManager.BuildRegions();
            

            watch.Stop();
            System.Diagnostics.Debug.Print("this._mdiManager.BuildRegions() " + watch.Elapsed.TotalSeconds.ToString());

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

            this._regionManager.Regions.Remove(Common.RegionNames.InventorDashboardBackForward);
			this.Loaded -= InventorDashboardFullView_Loaded;

			this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Unsubscribe(MdiFilterRecalculate);
        }

        #endregion
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
            InventorDashboardFullViewModel viewModel = this.DataContext as InventorDashboardFullViewModel;
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
