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
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Branch
{
    /// <summary>
    /// Interaction logic for BranchChooseView.xaml
    /// </summary>
    public partial class BranchChooseView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly BranchChooseViewModel _viewModel;
        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;        

        public BranchChooseView(
            BranchChooseViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter)
        {
            this.InitializeComponent();

            this._navigationRepository = navigationRepository;
            this._popupExtFilter = popupExtFilter;            

            this._viewModel = viewModel;
            this._modalWindowLauncher = modalWindowLauncher;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.DataContext = this._viewModel;

            this.Loaded += BranchChooseView_Loaded;
            this.dataGrid.SelectionChanged += dataGrid_SelectionChanged;
#if DEBUG
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Visible;
#else
            btnOpenScript.Visibility = btnSaveScript.Visibility = btnRepair.Visibility = Visibility.Collapsed;
#endif
            this._popupExtSearch = popupExtSearch;
            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = new BranchSearchData();
            _popupExtSearch.Region = Common.RegionNames.PopupSearchBranchChoose;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterBranch;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();

            btnDeleteAllWithoutChild.Visibility = FileSystem.IsAppRedactionOffice() ? Visibility.Visible : Visibility.Collapsed;
			
        }

        public bool KeepAlive { get { return false; } }

        void BranchChooseView_Loaded(object sender, RoutedEventArgs e)
        {
			//if (this._viewModel.State.CurrentCustomer == null)
			BranchFilterData filterData = this._viewModel.Filter;
			string currentCustomerCode = "";
			if (filterData != null)  currentCustomerCode = filterData.CustomerCode;
			if (string.IsNullOrWhiteSpace(currentCustomerCode) == true)
			{
				btnDeleteAllWithoutChild.Visibility = Visibility.Collapsed;
			}
            this._viewModel.ReportButton.BuildMenu(btnReport.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {            
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);          

            Utils.MainWindowTitleSet(WindowTitles.ChooseBranch, this._eventAggregator);

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeEmpty);

            RegionManager.SetRegionName(backForward, Common.RegionNames.BranchChooseBackForward);        
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.BranchChooseBackForward);

	

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

            this._regionManager.Regions.Remove(Common.RegionNames.BranchChooseBackForward);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;

        }     

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            var row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                BranchItemViewModel itemViewModel = row.DataContext as BranchItemViewModel;
                if (itemViewModel == null) return;

                BranchChooseViewModel viewModel = this.DataContext as BranchChooseViewModel;
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
            BranchChooseViewModel viewModel = this.DataContext as BranchChooseViewModel;
            if (viewModel != null)
            {
                List<BranchItemViewModel> selected = new List<BranchItemViewModel>();
                foreach (var item in dataGrid.SelectedItems)
                {
                    BranchItemViewModel itemViewModel = item as BranchItemViewModel;
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
