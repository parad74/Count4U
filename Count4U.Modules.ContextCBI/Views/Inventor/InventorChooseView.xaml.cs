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

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorChooseView.xaml
    /// </summary>
    public partial class InventorChooseView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly InventorChooseViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly INavigationRepository _navigationRepository;
        private readonly PopupExtFilter _popupExtFilter;

        public InventorChooseView(
            InventorChooseViewModel viewModel, 
            IRegionManager regionManager, 
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
        {
            _popupExtFilter = popupExtFilter;
            _navigationRepository = navigationRepository;
            _popupExtSearch = popupExtSearch;

            this.InitializeComponent();

            this._modalWindowLauncher = modalWindowLauncher;
			this._regionManager = regionManager;
            this._viewModel = viewModel;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this.DataContext = this._viewModel;
            this.Loaded += InventorChooseView_Loaded;

#if DEBUG
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Visible;
#else
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Collapsed;
#endif

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = new InventorSearchData();
            _popupExtSearch.Region = Common.RegionNames.PopupSearchInventorChoose;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterInventor;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();
        }

        public bool KeepAlive { get { return false; } }

        void InventorChooseView_Loaded(object sender, RoutedEventArgs e)
        {
            this._viewModel.ReportButton.BuildMenu(btnReport.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {            
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
           
            Utils.MainWindowTitleSet(WindowTitles.ChooseInventor, this._eventAggregator);
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeEmpty);

            RegionManager.SetRegionName(backForward, Common.RegionNames.InventorChooseBackForward);        
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.InventorChooseBackForward);       

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

            this._regionManager.Regions.Remove(Common.RegionNames.InventorChooseBackForward);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;

        }     

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            var row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                InventorItemViewModel itemViewModel = row.DataContext as InventorItemViewModel;
                if (itemViewModel == null) return;

                InventorChooseViewModel viewModel = this.DataContext as InventorChooseViewModel;
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
    }
}
