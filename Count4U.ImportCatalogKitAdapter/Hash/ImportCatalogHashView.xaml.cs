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
using Count4U.ImportCatalogKitAdapter.Made4Net;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportCatalogKitAdapter.Hash
{
    public partial class ImportCatalogHashView : UserControl
    {

		public ImportCatalogHashView(ImportCatalogHashViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
