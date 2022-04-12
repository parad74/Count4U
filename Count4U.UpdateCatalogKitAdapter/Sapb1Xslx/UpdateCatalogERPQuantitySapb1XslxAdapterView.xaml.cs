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

namespace Count4U.UpdateCatalogERPQuantitySapb1XslxAdapter
{
  
    public partial class UpdateCatalogERPQuantitySapb1XslxAdapterView : UserControl
    {
		public UpdateCatalogERPQuantitySapb1XslxAdapterView(UpdateCatalogERPQuantitySapb1XslxAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
