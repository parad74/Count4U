using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.Nesher
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpNesherAdapterView : UserControl
    {
		public ExportErpNesherAdapterView(ExportErpNesherAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
