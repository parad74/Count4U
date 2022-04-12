using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.CustomerControl;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.CustomerControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomerField.xaml
    /// </summary>
    public partial class SearchViewCustomerField : UserControl
    {
        public SearchViewCustomerField(SearchCustomerFieldViewModel viewModel)
        {
            InitializeComponent();
            
            this.DataContext = viewModel;
        }
    }
}
