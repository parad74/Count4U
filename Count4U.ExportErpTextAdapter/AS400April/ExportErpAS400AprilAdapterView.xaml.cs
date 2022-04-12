using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.AS400April
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpAS400AprilAdapterView : UserControl
    {
		public ExportErpAS400AprilAdapterView(ExportErpAS400AprilAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
