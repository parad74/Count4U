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

namespace Count4U.UpdateCatalogKitAdapter.RetalixNext
{
    /// <summary>
    /// Interaction logic for UpdateCatalogRetalixNextView.xaml
    /// </summary>
    public partial class UpdateCatalogRetalixNextView : UserControl
    {
        public UpdateCatalogRetalixNextView(UpdateCatalogRetalixNextViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
