using System.Windows.Controls;

namespace Count4U.ImportCatalogKitAdapter.NativPlusXslx
{

	public partial class ImportCatalogNativPlusXslxView : UserControl
    {

		public ImportCatalogNativPlusXslxView(ImportCatalogNativPlusXslxViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

		//	RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
