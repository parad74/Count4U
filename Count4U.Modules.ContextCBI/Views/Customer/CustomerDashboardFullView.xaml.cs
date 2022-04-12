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
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Common.Services.UICommandService;
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
using Count4U.Modules.ContextCBI.Views.DashboardItems;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager;
using Count4U.Modules.ContextCBI.Views.Misc.Dashboard;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using WPF.MDI;


namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for CustomerDashboardFullView.xaml
    /// </summary>
    public partial class CustomerDashboardFullView : DashboardBaseView, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupFilterRegion = Common.RegionNames.PopupFilterCustomerDashboard;
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchCustomerDashboard;
        private const string DashboardName = "CustomerDashboard";

        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IDBSettings _dbSettings;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;

        private NavigationContext _navigationContext;        

        public CustomerDashboardFullView(
            CustomerDashboardFullViewModel viewModel,
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

            this._popupExtFilter = popupExtFilter;
            this._popupExtSearch = popupExtSearch;
            this._dbSettings = dbSettings;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._userSettingsManager = userSettingsManager;

            this.DataContext = viewModel;

            this.Loaded += CustomerDashboardFullView_Loaded;          

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = null;
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.NavigationData = new CustomerSearchData();
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = PopupFilterRegion;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.MdiFilterView;
//            _popupExtFilter.Height = 500;
            _popupExtFilter.ApplyForQuery = base.ApplyToFilterQuery(DashboardName);            
            _popupExtFilter.Init();

          //  btnSearch.ImageSource = UICommandIconRepository.IconSearch;
        }

        public CBIContext Context { get; set; }

        void CustomerDashboardFullView_Loaded(object sender, RoutedEventArgs e)
        {

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
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartBranches,
                ViewName = Common.ViewNames.LastBranchesDashboardPartView,
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
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartReports,
                ViewName = Common.ViewNames.ReportsDashboardPartView,
                X = 645,
                Y = 220,
                Width = 300,
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartLocation,
                ViewName = Common.ViewNames.LocationDashboardPartView,
                X = 320,
                Y = 220,
                Width = 315,
                Height = 200
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartIturim,
                ViewName = Common.ViewNames.IturimDashboardPartView,
                X = 10,
                Y = 430,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartAllBranches,
                ViewName = Common.ViewNames.BranchesDashboardPartView,
                X = 320,
                Y = 430,
                Width = 315,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSection,
                ViewName = Common.ViewNames.SectionDashboardPartView,
                X = 645,
                Y = 430,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartCustomer,
                ViewName = Common.ViewNames.CustomerDashboardPartView,
                X = 10,
                Y = 605,
                Width = 300,
                Height = 165
            });
            result.Items.Add(new MdiRegionLayout()
            {
                RegionName = Common.RegionNames.DashboardPartSupplier,
                ViewName = Common.ViewNames.SupplierDashboardPartView,
                X = 320,
                Y = 605,
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
                X = 645,
                Y = 605,
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
			menu.Settings.Add(Common.NavigationSettings.MenuDashboardCustomer, String.Empty);
			Utils.AddContextToDictionary(menu.Settings, CBIContext.Main);
			Utils.AddDbContextToDictionary(menu.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, menu.Settings);
			 result.Add(menu);
            }

            {

			MdiRegion branches = new MdiRegion();
			branches.RegionName = Common.RegionNames.DashboardPartBranches;
			branches.ViewName = Common.ViewNames.LastBranchesDashboardPartView;
			branches.Title = MdiTitles.LastBrachesForCustomer;
			layout = defaults.Get(branches);
			branches.Position = new Point(layout.X, layout.Y);
			branches.Width = layout.Width;
			branches.Height = layout.Height;
			branches.DashboardName = DashboardName;
			Utils.AddContextToDictionary(branches.Settings, CBIContext.Main);
			Utils.AddDbContextToDictionary(branches.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, branches.Settings);
			result.Add(branches);
            }

            {
			MdiRegion inventors = new MdiRegion();
			inventors.RegionName = Common.RegionNames.DashboardPartLastInventors;
			inventors.ViewName = Common.ViewNames.LastInventorsDashboardPartView;
			inventors.Title = MdiTitles.LastInventorsForCustomer;
			layout = defaults.Get(inventors);
			inventors.Position = new Point(layout.X, layout.Y);
			inventors.Width = layout.Width;
			inventors.Height = layout.Height;
			inventors.DashboardName = DashboardName;
			inventors.Settings.Add(Common.NavigationSettings.LastInventors, Common.NavigationSettings.LastInventorsForCustomer);
			Utils.AddDbContextToDictionary(inventors.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			Utils.AddContextToDictionary(inventors.Settings, CBIContext.Main);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, inventors.Settings);
			result.Add(inventors);
            }

            {
			MdiRegion catalog = new MdiRegion();
			catalog.RegionName = Common.RegionNames.DashboardPartCatalogInfoForCBI;
			catalog.ViewName = Common.ViewNames.CatalogInfoForCBIDashboardPartView;
			catalog.Title = MdiTitles.CatalogInfoForCustomer;
			layout = defaults.Get(catalog);
			catalog.Position = new Point(layout.X, layout.Y);
			catalog.Width = layout.Width;
			catalog.Height = layout.Height;
			catalog.DashboardName = DashboardName;
			catalog.Settings.Add(Common.NavigationSettings.MdiPartOwnerDashboard, Common.NavigationSettings.MdiPartOwnerDashboardCustomer);
			Utils.AddDbContextToDictionary(catalog.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			Utils.AddContextToDictionary(catalog.Settings, CBIContext.Main);
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
			Utils.AddDbContextToDictionary(reports.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, reports.Settings);
			result.Add(reports);
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
			location.Settings.Add(Common.NavigationSettings.LocationsPart, Common.NavigationSettings.LocationsPartCustomer);
			Utils.AddContextToDictionary(location.Settings, CBIContext.Main);
			Utils.AddDbContextToDictionary(location.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, location.Settings);
			result.Add(location);
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
			Utils.AddDbContextToDictionary(iturim.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, iturim.Settings);
			result.Add(iturim);
            }

            {
			MdiRegion allBranches = new MdiRegion();
			allBranches.RegionName = Common.RegionNames.DashboardPartAllBranches;
			allBranches.ViewName = Common.ViewNames.BranchesDashboardPartView;
			allBranches.Title = MdiTitles.AllBranches;
			layout = defaults.Get(allBranches);
			allBranches.Position = new Point(layout.X, layout.Y);
			allBranches.Width = layout.Width;
			allBranches.Height = layout.Height;
			allBranches.DashboardName = DashboardName;
			Utils.AddContextToDictionary(allBranches.Settings, CBIContext.Main);
			Utils.AddDbContextToDictionary(allBranches.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, allBranches.Settings);
			result.Add(allBranches);
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
			Utils.AddDbContextToDictionary(section.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, section.Settings);
			result.Add(section);
            }

            {
			MdiRegion customer = new MdiRegion();
			customer.RegionName = Common.RegionNames.DashboardPartCustomer;
			customer.ViewName = Common.ViewNames.CustomerDashboardPartView;
			customer.Title = MdiTitles.Customer;
			layout = defaults.Get(customer);
			customer.Position = new Point(layout.X, layout.Y);
			customer.Width = layout.Width;
			customer.Height = layout.Height;
			customer.DashboardName = DashboardName;
			Utils.AddContextToDictionary(customer.Settings, CBIContext.Main);
			Utils.AddDbContextToDictionary(customer.Settings, Common.NavigationSettings.CBIDbContextCustomer);
			NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, customer.Settings);
			result.Add(customer);
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
			Utils.AddDbContextToDictionary(supplier.Settings, Common.NavigationSettings.CBIDbContextCustomer);
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
			Utils.AddDbContextToDictionary(family.Settings, Common.NavigationSettings.CBIDbContextCustomer);
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
			Utils.AddDbContextToDictionary(planogram.Settings, Common.NavigationSettings.CBIDbContextCustomer);
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
				//fromPda.Brush = Brushes.MediumSeaGreen;
				//Utils.AddContextToDictionary(fromPda.Settings, CBIContext.Main);
				//Utils.AddDbContextToDictionary(fromPda.Settings, Common.NavigationSettings.CBIDbContextCustomer);
				//NavigationAwareViewModel.AddNavigationContextSettings(this._navigationContext, fromPda.Settings);
				//result.Add(fromPda);
            }

       
			//return new List<MdiRegion>
			//		   {
			//			   menu,
			//			   branches,
			//			   inventors,
			//			   catalog,
			//			   reports,
			//			   location,
			//			   iturim,
			//			   allBranches,
			//			   section,
			//			   customer,
			//			   supplier,
			//			   family,
			//			   planogram,
			//			   fromPda
			//		   };
				return result;
		}

        #region Implementation of INavigationAware

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            bool dbFileMissed = UtilsNavigate.DataFileMissed(navigationContext, this._contextCbiRepository, this._dbSettings, Window.GetWindow(this), this._userSettingsManager);           

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomer);
            RegionManager.SetRegionName(backForward, Common.RegionNames.CustomerDashboardBackForward);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.CustomerDashboardBackForward);

            this._navigationContext = navigationContext;
            CBIContext? cbiContext = Utils.CBIContextFromNavigationParameters(navigationContext);
            if (cbiContext != null)
                Context = cbiContext.Value;

            this._mdiManager = new MdiManager(this.mdiContainer,
                this._regionManager,
                this.RegionsInfoBuild(),
                DashboardName,
                this._userSettingsManager,
                this._eventAggregator,
                this.BuildDefaultLayouts());
            this._mdiManager.MdiListChanged += MdiManager_MdiListChanged;
            this._mdiManager.BuildRegions();

            Utils.MainWindowTitleSet(WindowTitles.CustomerDashboard, this._eventAggregator);

            Dispatcher.BeginInvoke(new Action(() =>
                                                  {
                                                      if (this._mdiManager != null)
                                                          this._mdiManager.MdiAllAppearanceApply();

                                                      if (dbFileMissed)
                                                      {
                                                          CustomerDashboardFullViewModel viewModel = this.DataContext as CustomerDashboardFullViewModel;
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

            this._regionManager.Regions.Remove(Common.RegionNames.CustomerDashboardBackForward);

            this._eventAggregator.GetEvent<MdiFilterRecalculateEvent>().Unsubscribe(MdiFilterRecalculate);
			this.Loaded -= CustomerDashboardFullView_Loaded;
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
            CustomerDashboardFullViewModel viewModel = this.DataContext as CustomerDashboardFullViewModel;
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
