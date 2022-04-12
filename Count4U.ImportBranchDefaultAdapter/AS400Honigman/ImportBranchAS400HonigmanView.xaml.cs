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

namespace Count4U.ImportBranchAS400HonigmanAdapter
{
    /// <summary>
    /// Interaction logic for ImportBranchXTechMeuhedetView.xaml
    /// </summary>
    public partial class ImportBranchAS400HonigmanView : UserControl
    {
		public ImportBranchAS400HonigmanView(ImportBranchAS400HonigmanViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
