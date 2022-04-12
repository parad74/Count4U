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

namespace Count4U.ImportSupplierComaxASPAdapter
{
    /// <summary>
    /// Interaction logic for ImportSupplierComaxASPAdapterView.xaml
    /// </summary>
    public partial class ImportSupplierComaxASPAdapterView : UserControl
    {
        public ImportSupplierComaxASPAdapterView(ImportSupplierComaxASPAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
