using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.H_M_New
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpH_M_NewAdapterView : UserControl
    {
		public ExportErpH_M_NewAdapterView(ExportErpH_M_NewAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
