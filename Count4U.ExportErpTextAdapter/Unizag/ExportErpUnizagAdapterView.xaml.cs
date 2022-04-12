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

namespace Count4U.ExportErpTextAdapter.Unizag
{
    /// <summary>
    /// Interaction logic for ExportErpUnizagAdapterView.xaml
    /// </summary>
    public partial class ExportErpUnizagAdapterView : UserControl
    {
        public ExportErpUnizagAdapterView(ExportErpUnizagAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
