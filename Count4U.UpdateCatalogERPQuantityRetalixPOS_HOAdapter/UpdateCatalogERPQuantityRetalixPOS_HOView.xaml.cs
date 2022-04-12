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

namespace Count4U.UpdateCatalogERPQuantityRetalixPOS_HOAdapter
{
    /// <summary>
    /// Interaction logic for UpdateCatalogERPuantityXTechMeuhedetView.xaml
    /// </summary>
    public partial class UpdateCatalogERPQuantityRetalixPOS_HOView : UserControl
    {
        public UpdateCatalogERPQuantityRetalixPOS_HOView(UpdateCatalogERPQuantityRetalixPOS_HOViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
