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
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems.InventProductMdi
{
    /// <summary>
    /// Interaction logic for InventProductSumPartView.xaml
    /// </summary>
    public partial class InventProductSumPartView : UserControl
    {
        public InventProductSumPartView(InventProductSumPartViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
