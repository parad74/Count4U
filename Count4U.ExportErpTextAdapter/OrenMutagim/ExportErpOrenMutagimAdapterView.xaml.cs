using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.OrenMutagim
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpOrenMutagimAdapterView : UserControl
    {
		public ExportErpOrenMutagimAdapterView(ExportErpOrenMutagimAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
