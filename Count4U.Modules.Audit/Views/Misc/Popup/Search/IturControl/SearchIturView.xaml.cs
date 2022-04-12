using System.Windows.Controls;
using Count4U.Modules.Audit.ViewModels;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.IturControl
{
    /// <summary>
    /// Interaction logic for SearchIturView.xaml
    /// </summary>
    public partial class SearchIturView : UserControl
    {
        public SearchIturView(SearchIturViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;

        }        
    }
}
