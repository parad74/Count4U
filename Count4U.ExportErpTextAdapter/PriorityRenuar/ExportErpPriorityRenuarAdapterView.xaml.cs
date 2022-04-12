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

namespace Count4U.ExportErpTextAdapter.PriorityRenuar
{
    /// <summary>
    /// Interaction logic for ExportErpPriorityRenuarAdapterView.xaml
    /// </summary>
    public partial class ExportErpPriorityRenuarAdapterView : UserControl
    {
        public ExportErpPriorityRenuarAdapterView(ExportErpPriorityRenuarAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
