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

namespace Count4U.ImportBranchAS400_LeumitAdapter
{
    /// <summary>
    /// Interaction logic for ImportBranchAS400_LeumitAdapterView.xaml
    /// </summary>
    public partial class ImportBranchAS400_LeumitAdapterView : UserControl
    {
        public ImportBranchAS400_LeumitAdapterView(ImportBranchAS400_LeumitAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
