using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.InventorControl;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.InventorControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomer.xaml
    /// </summary>
    public partial class SearchInventorView : UserControl
    {
        public SearchInventorView(SearchInventorViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;
            
            dataGridInventor.MouseDoubleClick += dataGrid_MouseDoubleClick;
        }

        void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchInventorViewModel viewModel = this.DataContext as SearchInventorViewModel;
            if (viewModel == null) return;

            DependencyObject depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                InventorItemViewModel inventor = row.DataContext as InventorItemViewModel;
                if (inventor != null)
                {
                    viewModel.InventorNavigate(inventor);
                }              
            }
        }
    }
}
