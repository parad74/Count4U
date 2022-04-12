using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.MirkamSonol
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpMirkamSonolAdapterView : UserControl
    {
        public ExportErpMirkamSonolAdapterView(ExportErpMirkamSonolAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
