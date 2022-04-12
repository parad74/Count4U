using System.Windows.Controls;

namespace Count4U.ImportCatalogNativPlusLadpcAdapter
{
	public partial class ImportCatalogNativPlusLadpcAdapterView : UserControl
    {
	   public ImportCatalogNativPlusLadpcAdapterView(ImportCatalogNativPlusLadpcAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
