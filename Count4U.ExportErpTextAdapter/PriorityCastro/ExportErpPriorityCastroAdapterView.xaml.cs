using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.PriorityCastro
{
 
    public partial class ExportErpPriorityCastroAdapterView : UserControl
    {
		public ExportErpPriorityCastroAdapterView(ExportErpPriorityCastroAdapterViewModel viewModel)
        {
            InitializeComponent();
			this.DataContext = viewModel;
        }
    }
}
