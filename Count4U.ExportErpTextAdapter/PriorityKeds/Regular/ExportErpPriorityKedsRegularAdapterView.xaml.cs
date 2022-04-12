using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.PriorityKeds.Regular
{
    /// <summary>
    /// Interaction logic for ExportErpPriorityRenuarAdapterView.xaml
    /// </summary>
    public partial class ExportErpPriorityKedsRegularAdapterView : UserControl
    {
        public ExportErpPriorityKedsRegularAdapterView(ExportErpPriorityKedsRegularAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
