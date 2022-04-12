using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.MirkamSonolSAP
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpMirkamSonolSAPAdapterView : UserControl
    {
        public ExportErpMirkamSonolSAPAdapterView(ExportErpMirkamSonolSAPAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
