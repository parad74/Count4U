﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


namespace Count4U.UpdateCatalogGeneralOldAdapter
{
	//OLD - не использутся
 public partial class UpdateCatalogERPQuantityGeneralOldView : UserControl
    {
	 public UpdateCatalogERPQuantityGeneralOldView(UpdateCatalogERPQuantityGeneralOldViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            RegionManager.SetRegionName(maskControl, viewModel.BuildMaskRegionName());            
        }
    }
}