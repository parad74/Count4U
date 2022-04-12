using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Count4U.ImportCatalogKitAdapter.AS400Hamashbir
{
    public partial class ImportCatalogAS400HamashbirAdapterView: UserControl
    {
		public ImportCatalogAS400HamashbirAdapterView(ImportCatalogAS400HamashbirAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			RegionManager.SetRegionName(maskControl1, viewModel.BuildMaskRegionName("1"));
			RegionManager.SetRegionName(maskControl2, viewModel.BuildMaskRegionName("2"));
        }

	
    }
}
