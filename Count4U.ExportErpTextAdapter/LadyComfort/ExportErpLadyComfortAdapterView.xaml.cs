using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.LadyComfort
{
    /// <summary>
    /// Interaction logic for ExportErpComaxAdapterView.xaml
    /// </summary>
    public partial class ExportErpLadyComfortAdapterView : UserControl
    {
		public ExportErpLadyComfortAdapterView(ExportErpLadyComfortAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
