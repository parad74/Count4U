using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.FRSVisionMirkam
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpFRSVisionMirkamAdapterView : UserControl
    {
		public ExportErpFRSVisionMirkamAdapterView(ExportErpFRSVisionMirkamAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
