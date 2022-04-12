using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.MirkamTen
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpMirkamTenAdapterView : UserControl
    {
        public ExportErpMirkamTenAdapterView(ExportErpMirkamTenAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
