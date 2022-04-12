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
using Count4U.Common.Behaviours;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Catalog;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Catalog
{
    /// <summary>
    /// Interaction logic for CatalogFormView.xaml
    /// </summary>
    public partial class CatalogFormView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly Guid _guid;

        public CatalogFormView(
            CatalogFormViewModel viewModel, 
            IRegionManager regionManager, 
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository)
        {
            InitializeComponent();

            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.DataContext = viewModel;

            this.masterDataGrid.SelectionChanged += listView_SelectionChanged;
            this.Loaded += CatalogFormView_Loaded;

            _guid = Guid.NewGuid();
            backForwardRegionName = Common.RegionNames.CatalogFormBackForward+ _guid.ToString();
            searchFilterRegionName = Common.RegionNames.ProductSearchFilter + _guid.ToString();

            viewModel.SearchFilterRegionKey = _guid.ToString();
			System.Windows.Interactivity.Interaction.GetBehaviors(txtGoToPage).Add(new TextBoxPropertyBehavior());
        }

        public bool KeepAlive { get { return false; } }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CatalogFormViewModel viewModel = this.DataContext as CatalogFormViewModel;
            if (viewModel != null)
                viewModel.SelectedItemsSet(masterDataGrid.SelectedItems.Cast<ProductItemViewModel>().ToList());
        }

        void CatalogFormView_Loaded(object sender, RoutedEventArgs e)
        {
            CatalogFormViewModel viewModel = this.DataContext as CatalogFormViewModel;
            if (viewModel != null)
                viewModel.ReportButton.BuildMenu(btnReport.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            Utils.MainWindowTitleSet(WindowTitles.Catalog, this._eventAggregator);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, backForwardRegionName);
            UtilsNavigate.SearchFilterNavigate(this._regionManager, searchFilterRegionName);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      backForward,
                                                      searchFilter
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(backForwardRegionName);
            this._regionManager.Regions.Remove(searchFilterRegionName);
        }
    }
}
