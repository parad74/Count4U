using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Model.Interface.Audit;
using Count4U.Report.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Report.Views
{
    /// <summary>
    /// Interaction logic for ReportFavoritesView.xaml
    /// </summary>
    public partial class ReportFavoritesView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ReportFavoritesViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public ReportFavoritesView(
            ReportFavoritesViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this._modalWindowLauncher = modalWindowLauncher;
            this._regionManager = regionManager;
            this._contextCbiRepository = contextCbiRepository;
            this._viewModel = viewModel;
            this.DataContext = _viewModel;

          //  this.Loaded += InventProductView_Loaded;
            this.masterDataGrid.SelectionChanged += listView_SelectionChanged;

            Interaction.GetBehaviors(masterDataGrid).Add(new GridCancelEditBehavior());
            Interaction.GetBehaviors(masterDataGrid).Add(new DataGridBehavior());

#if DEBUG
            btnRepair.Visibility = Visibility.Visible;
#else
            btnRepair.Visibility = Visibility.Visible;
#endif
		}

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ReportFavoritesBackForward);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);

            _viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
        }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {                                                                                                                   //detailDataGrid
            ReportItemViewModel viewModel = this.DataContext as ReportItemViewModel;
            if (viewModel != null)
            {
                //viewModel.SelectedItemSet(viewModel.SelectedItem);
                //if (viewModel.SelectedItem != null)
                //{
                //    masterDataGrid.ScrollIntoView(viewModel.SelectedItem);
                //}

                //var selectedItems = masterDataGrid.SelectedItems.Cast<InventProductItemViewModel>().ToList();
                //viewModel.SelectedItemSet(selectedItems.FirstOrDefault());

                //if (viewModel.Items != null)
                //{
                //	InventProductItemViewModel selectedItem = viewModel.SelectedItem;
                //	InventProductItemViewModel firstItem = viewModel.ItemsFirstOrDefault();

                //if (selectedItem != null)
                //{
                //	if (viewModel.SelectedItem != null)
                //	{
                //		if (selectedItem.Num != viewModel.SelectedItem.Num
                //			&& selectedItem.Makat != viewModel.SelectedItem.Makat)
                //		{
                //			viewModel.SelectedItem = selectedItem;
                //		}
                //	}
                //	else
                //	{
                //		viewModel.SelectedItem = selectedItem;
                //	}
                //}
                //else if (firstItem != null)
                //{
                //	viewModel.SelectedItem = firstItem;
                //}
                //}
            }
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
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ReportFavoritesBackForward);

            _viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
            object result = null;
            if (e.ViewName == Common.ViewNames.ReportAddEditView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.ReportAddEditView, e.WindowTitle, 500, 312,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minHeight:312);
            }
            if (e.ViewName == Common.ViewNames.ReportScriptView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.ReportScriptView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.ViewName == Common.ViewNames.ReportScriptSaveView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.ReportScriptSaveView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.Callback != null)
                e.Callback(result);
        }



        private void QuantityEditTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            textBox.SelectAll();
        }
    }


}
