using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportCatalogKitAdapter.NativXslx
{

	public partial class ImportCatalogNativXslxView : UserControl
    {

		public ImportCatalogNativXslxView(ImportCatalogNativXslxViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

		//	RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
