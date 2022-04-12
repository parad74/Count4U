using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for CustomerChooseView.xaml
    /// </summary>
    public partial class CustomerChooseView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly CustomerChooseViewModel _viewModel;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;
        private readonly INavigationRepository _navigationRepository;

        public CustomerChooseView(
            CustomerChooseViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
        {            
            this.InitializeComponent();

            this._viewModel = viewModel;
            this.DataContext = _viewModel;
            this._regionManager = regionManager;
            this._modalWindowLauncher = modalWindowLauncher;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._navigationRepository = navigationRepository;
            this._popupExtFilter = popupExtFilter;
            this._popupExtSearch = popupExtSearch;

            this.Loaded += CustomerChooseView_Loaded;
            this.dataGrid.SelectionChanged += dataGrid_SelectionChanged;

#if DEBUG
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Visible;
#else
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Collapsed;
#endif

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = new CustomerSearchData();
            _popupExtSearch.Region = Common.RegionNames.PopupSearchCustomerChoose;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterCustomer;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();
        }       

        public bool KeepAlive { get { return false; } }

        void CustomerChooseView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ReportButton.BuildMenu(btnReport.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);            

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);            

            Utils.MainWindowTitleSet(WindowTitles.ChooseCustomer, this._eventAggregator);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);            
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeEmpty);

            RegionManager.SetRegionName(backForward, Common.RegionNames.CustomerChooseBackForward);            
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.CustomerChooseBackForward);         

            this._viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
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

            this._regionManager.Regions.Remove(Common.RegionNames.CustomerChooseBackForward);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;

        }       

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            var row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                CustomerItemViewModel itemViewModel = row.DataContext as CustomerItemViewModel;
                if (itemViewModel == null) return;

                CustomerChooseViewModel viewModel = this.DataContext as CustomerChooseViewModel;
                if (viewModel == null) return;

                viewModel.DoubleClickCommand.Execute(itemViewModel);
            }
        }

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
            object result = null;

            if (e.ViewName == Common.ViewNames.CBIScriptSaveView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.CBIScriptSaveView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.ViewName == Common.ViewNames.CBIScriptOpenView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.CBIScriptOpenView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.Callback != null)
                e.Callback(result);
        }

        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerChooseViewModel viewModel = this.DataContext as CustomerChooseViewModel;
            if (viewModel != null)
            {
                List<CustomerItemViewModel> selected = new List<CustomerItemViewModel>();
                foreach (var item in dataGrid.SelectedItems)
                {
                    CustomerItemViewModel itemViewModel = item as CustomerItemViewModel;
                    if (itemViewModel != null)
                    {
                        selected.Add(itemViewModel);
                    }
                }
                viewModel.SetSelected(selected);
            }
        }
    }
}
