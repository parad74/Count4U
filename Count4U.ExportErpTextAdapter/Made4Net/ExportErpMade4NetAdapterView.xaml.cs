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

namespace Count4U.ExportErpTextAdapter.Made4Net
{
    /// <summary>
    /// Interaction logic for ExportErpGazitAdapterView.xaml
    /// </summary>
    public partial class ExportErpMade4NetAdapterView : UserControl
    {
		public ExportErpMade4NetAdapterView(ExportErpMade4NetAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
