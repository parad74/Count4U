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
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.InventProduct
{
    /// <summary>
    /// Interaction logic for InventProductListSimpleView.xaml
    /// </summary>
    public partial class InventProductListSimpleView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;
        private readonly INavigationRepository _navigationRepository;        

        public InventProductListSimpleView(
            InventProductListSimpleViewModel viewModel,
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter
            )
        {            
            InitializeComponent();

            _popupExtFilter = popupExtFilter;
            _popupExtSearch = popupExtSearch;
            _navigationRepository = navigationRepository;

            this.DataContext = viewModel;

            _contextCbiRepository = contextCbiRepository;
            _regionManager = regionManager;

            _popupExtSearch.Button = btnSearch;
            //_popupExtSearch.NavigationData = new CustomerSearchData();
            _popupExtSearch.Region = Common.RegionNames.PopupSearchInventProductListSimple;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterInventProductListSimple;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();

            this.Loaded += InventProductListSimpleView_Loaded;
        }

        public bool KeepAlive { get { return false; } }

        void InventProductListSimpleView_Loaded(object sender, RoutedEventArgs e)
        {
            InventProductListSimpleViewModel viewModel = this.DataContext as InventProductListSimpleViewModel;
            if (viewModel != null)
                viewModel.ReportButtonViewModel.BuildMenu(btnReport.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.InventProductListSimpleBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.InventProductListSimpleBackForward);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      backForward
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.InventProductListSimpleBackForward);
        }
    }
}
