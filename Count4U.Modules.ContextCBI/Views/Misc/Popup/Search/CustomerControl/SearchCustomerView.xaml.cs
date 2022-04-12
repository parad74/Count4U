using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.CustomerControl;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.CustomerControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomer.xaml
    /// </summary>
    public partial class SearchCustomerView : UserControl
    {
        public SearchCustomerView(SearchCustomerViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;

            dataGridCustomer.MouseDoubleClick += dataGrid_MouseDoubleClick;
        }        

        void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchCustomerViewModel viewModel = this.DataContext as SearchCustomerViewModel;
            if (viewModel == null) return;

            DependencyObject depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                CustomerItemViewModel customer = row.DataContext as CustomerItemViewModel;
                if (customer != null)
                {
                    viewModel.CustomerNavigate(customer);
                }            
            }
        }
    }
}
