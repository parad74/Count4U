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

namespace Count4U.ImportCatalogGeneralXLSXAdapter
{
    public partial class ImportCatalogGeneralXLSXView: UserControl
    {
		public ImportCatalogGeneralXLSXView(ImportCatalogGeneralXLSXViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            RegionManager.SetRegionName(maskControl1, viewModel.BuildMaskRegionName("1"));
            RegionManager.SetRegionName(maskControl2, viewModel.BuildMaskRegionName("2"));
        }
    }
}
