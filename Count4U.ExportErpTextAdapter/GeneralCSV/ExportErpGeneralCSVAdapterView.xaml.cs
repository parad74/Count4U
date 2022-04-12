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

namespace Count4U.ExportErpTextAdapter.GeneralCSV
{
    /// <summary>
    /// Interaction logic for ExportErpGeneralCSVView.xaml
    /// </summary>
	public partial class ExportErpGeneralCSVAdapterView : UserControl
    {
		public ExportErpGeneralCSVAdapterView(ExportErpGeneralCSVAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
