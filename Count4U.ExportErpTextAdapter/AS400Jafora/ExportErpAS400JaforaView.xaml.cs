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

namespace Count4U.ExportErpTextAdapter.AS400Jafora
{
    /// <summary>
    /// Interaction logic for ExportErpAvivMulitBarcodesView.xaml
    /// </summary>
    public partial class ExportErpAS400JaforaView : UserControl
    {
		public ExportErpAS400JaforaView(ExportErpAS400JaforaViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
