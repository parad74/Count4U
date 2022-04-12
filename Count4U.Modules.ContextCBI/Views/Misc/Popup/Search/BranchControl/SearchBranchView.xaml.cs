using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.BranchControl;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.BranchControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomer.xaml
    /// </summary>
    public partial class SearchBranchView : UserControl
    {
        public SearchBranchView(SearchBranchViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;

            dataGridBranch.MouseDoubleClick += dataGrid_MouseDoubleClick;
        }

        void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchBranchViewModel viewModel = this.DataContext as SearchBranchViewModel;
            if (viewModel == null) return;

            DependencyObject depObj = e.OriginalSource as DependencyObject;
            if (depObj == null) return;
            DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

            if (row != null)
            {
                BranchItemViewModel branch = row.DataContext as BranchItemViewModel;
                if (branch != null)
                {
                    viewModel.BranchNavigate(branch);
                }            
            }
        }
    }
}
