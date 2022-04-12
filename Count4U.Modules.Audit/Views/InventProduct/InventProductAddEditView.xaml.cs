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
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for InventProductAddEditView.xaml
    /// </summary>
    public partial class InventProductAddEditView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly InventProductAddEditViewModel _viewModel;

        public InventProductAddEditView(
            InventProductAddEditViewModel viewModel, 
            IRegionManager regionManager)
        {            
            this.InitializeComponent();

            this._viewModel = viewModel;
            this._regionManager = regionManager;

            this.DataContext = _viewModel;

            this.Loaded += InventProductAddEditView_Loaded;
            this.PreviewKeyUp += InventProductAddEditView_KeyUp;
        }

        void InventProductAddEditView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.CancelCommand.Execute();
                e.Handled = true;
            }
        }

        void InventProductAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            InventProductAddEditViewModel viewModel = this.DataContext as InventProductAddEditViewModel;
            if (viewModel != null)
            {
				if (viewModel.IsNewMode == true)
				{
					txtMakat.Focus();
				}
				else
				{
					txtProductName.Focus();
				}

                Dispatcher.BeginInvoke(new Action(() => viewModel.IsTimerEnabled = true), DispatcherPriority.Background);
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
