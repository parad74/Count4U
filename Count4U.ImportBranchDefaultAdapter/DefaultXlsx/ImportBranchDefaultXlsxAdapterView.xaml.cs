﻿using System;
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

namespace Count4U.ImportBranchDefaultXlsxAdapter
{
    /// <summary>
    /// Interaction logic for ImportBranchDefaultAdapterView.xaml
    /// </summary>
    public partial class ImportBranchDefaultXlsxAdapterView : UserControl
    {
		public ImportBranchDefaultXlsxAdapterView(ImportBranchDefaultXlsxAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            //  RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
