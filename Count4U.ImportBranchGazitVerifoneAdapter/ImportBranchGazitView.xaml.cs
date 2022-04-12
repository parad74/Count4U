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

namespace Count4U.ImportBranchGazitVerifoneAdapter
{
    /// <summary>
    /// Interaction logic for ImportBranchGazitView.xaml
    /// </summary>
    public partial class ImportBranchGazitView : UserControl
    {
        public ImportBranchGazitView(ImportBranchGazitViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
