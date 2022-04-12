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

namespace Count4U.ExportErpTextAdapter.RetalixPOS_HO
{
    /// <summary>
    /// Interaction logic for ExportErpPriorityRenuarAdapterView.xaml
    /// </summary>
    public partial class ExportErpRetalixPOS_HOAdapterView : UserControl
    {
        public ExportErpRetalixPOS_HOAdapterView(ExportErpRetalixPOS_HOAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
