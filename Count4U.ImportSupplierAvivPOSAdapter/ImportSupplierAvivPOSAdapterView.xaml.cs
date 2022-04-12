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

namespace Count4U.ImportSupplierAvivPOSAdapter
{
    /// <summary>
    /// Interaction logic for ImportSupplierAvivPOSAdapterView.xaml
    /// </summary>
    public partial class ImportSupplierAvivPOSAdapterView : UserControl
    {
        public ImportSupplierAvivPOSAdapterView(ImportSupplierAvivPOSAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
