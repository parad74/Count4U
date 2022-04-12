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

namespace Count4U.ImportCatalogRetalixPOS_HOAdapter
{
    /// <summary>
    /// Interaction logic for ImportRetalixPOS_HOAdapterView.xaml
    /// </summary>
    public partial class ImportCatalogRetalixPOS_HOAdapterView : UserControl
    {
        public ImportCatalogRetalixPOS_HOAdapterView(ImportCatalogRetalixPOS_HOAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());			
			
        }

    }
}
