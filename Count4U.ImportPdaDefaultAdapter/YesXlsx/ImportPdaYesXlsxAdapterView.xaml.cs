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

namespace Count4U.ImportPdaYesXlsxAdapter
{
   public partial class ImportPdaYesXlsxAdapterView : UserControl
    {
	   public ImportPdaYesXlsxAdapterView(ImportPdaYesXlsxAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}