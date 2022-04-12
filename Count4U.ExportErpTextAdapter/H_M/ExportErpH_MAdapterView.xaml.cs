using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.H_M
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpH_MAdapterView : UserControl
    {
		public ExportErpH_MAdapterView(ExportErpH_MAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
