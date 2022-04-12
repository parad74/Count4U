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
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for IturimAddEditView.xaml
    /// </summary>
    public partial class IturimAddEditDeleteView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;
        private readonly INavigationRepository _navigationRepository;
		private string currentCustomer = "";

        public IturimAddEditDeleteView(
            IturimAddEditDeleteViewModel viewModel, 
            IRegionManager regionManager, 
            IContextCBIRepository contextCbiRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter,
            INavigationRepository navigationRepository)
        {            
            this.InitializeComponent();

            this.DataContext = viewModel;

            this._navigationRepository = navigationRepository;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;            

            this._popupExtFilter = popupExtFilter;
            this._popupExtSearch = popupExtSearch;

            this.dataGrid.SelectionChanged += listView_SelectionChanged;
            this.Loaded += IturimAddEditDeleteView_Loaded;

            _popupExtSearch.Button = btnSearch;
            //_popupExtSearch.NavigationData = new CustomerSearchData();
            _popupExtSearch.Region = Common.RegionNames.PopupSearchIturAddEditDelete;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterIturAddEditDelete;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();
        }

        public bool KeepAlive { get { return false; } }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IturimAddEditDeleteViewModel viewModel = this.DataContext as IturimAddEditDeleteViewModel;
			if (viewModel != null)
			{
				currentCustomer = "";
				if (viewModel.CurrentCustomer != null) currentCustomer = viewModel.CurrentCustomer.Code;
				viewModel.SelectedItemsSet(dataGrid.SelectedItems.Cast<IturItemViewModel>().ToList());
			}
        }


        void IturimAddEditDeleteView_Loaded(object sender, RoutedEventArgs e)
        {
            IturimAddEditDeleteViewModel viewModel = this.DataContext as IturimAddEditDeleteViewModel;
			if (viewModel != null)
			{
				currentCustomer = "";
				if (viewModel.CurrentCustomer != null) currentCustomer = viewModel.CurrentCustomer.Code;
				viewModel.ReportButtonViewModel.BuildMenu(btnReport.ContextMenu);
			}
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.IturAddEditDeleteBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.IturAddEditDeleteBackForward);
			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigCustomer))
			{
				currentCustomer = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigCustomer).Value;
			}

#if DEBUG
			realDelete.Visibility = System.Windows.Visibility.Visible;
			//Test
#else
	if (currentCustomer == "610") { realDelete.Visibility = System.Windows.Visibility.Visible; }
			else { realDelete.Visibility = System.Windows.Visibility.Collapsed; }
#endif
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

            this._regionManager.Regions.Remove(Common.RegionNames.IturAddEditDeleteBackForward);
			this.dataGrid.SelectionChanged -= listView_SelectionChanged;
			this.Loaded -= IturimAddEditDeleteView_Loaded;
		}


    }
}
