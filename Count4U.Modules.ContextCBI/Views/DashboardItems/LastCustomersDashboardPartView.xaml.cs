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
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems
{
    /// <summary>
    /// Interaction logic for LastCustomersDashboardPartViewModel.xaml
    /// </summary>
    public partial class LastCustomersDashboardPartView : UserControl
    {
	
        public LastCustomersDashboardPartView(LastCustomersDashboardPartViewModel viewModel)
        {
			this.InitializeComponent();

            this.DataContext = viewModel;

			this.Loaded += LastCustomersDashboardPartView_Loaded;

			//if (viewModel._mode == Count4U.Modules.ContextCBI.ViewModels.DashboardItems.LastCustomersDashboardPartViewModel.LastCustomersDashboardPartViewModelMode.LastBuild)
			//{
			//	CustomerCheck.Visibility = System.Windows.Visibility.Visible;
			//}
			//else
			//{
			//	CustomerCheck.Visibility = System.Windows.Visibility.Hidden;
			//}
        }

		void LastCustomersDashboardPartView_Loaded(object sender, RoutedEventArgs e)
		{
			LastCustomersDashboardPartViewModel viewModel = this.DataContext as LastCustomersDashboardPartViewModel;
			if (viewModel == null) return;
			if (viewModel._mode == Count4U.Modules.ContextCBI.ViewModels.DashboardItems.LastCustomersDashboardPartViewModel.LastCustomersDashboardPartViewModelMode.LastBuild)
			{
				CustomerCheck.Visibility = System.Windows.Visibility.Visible;
			}
			else if (viewModel._mode == Count4U.Modules.ContextCBI.ViewModels.DashboardItems.LastCustomersDashboardPartViewModel.LastCustomersDashboardPartViewModelMode.LastInInventory)
			{
				CustomerCheck.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				CustomerCheck.Visibility = System.Windows.Visibility.Hidden;
			}
		}
    }
}
