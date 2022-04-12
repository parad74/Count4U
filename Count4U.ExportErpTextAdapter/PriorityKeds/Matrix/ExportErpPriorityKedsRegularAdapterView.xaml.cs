using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.PriorityKeds.Matrix
{
    /// <summary>
    /// Interaction logic for ExportErpPriorityRenuarAdapterView.xaml
    /// </summary>
    public partial class ExportErpPriorityKedsMatrixAdapterView : UserControl
    {
        public ExportErpPriorityKedsMatrixAdapterView(ExportErpPriorityKedsMatrixAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
