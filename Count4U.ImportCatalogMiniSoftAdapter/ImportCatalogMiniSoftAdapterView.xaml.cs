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

namespace Count4U.ImportCatalogMiniSoftAdapter
{
    /// <summary>
    /// Interaction logic for ImportMiniSoftAdapterView.xaml
    /// </summary>
    public partial class ImportCatalogMiniSoftAdapterView : UserControl
    {
        public ImportCatalogMiniSoftAdapterView(ImportCatalogMiniSoftAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());			
			
        }

    }
}
