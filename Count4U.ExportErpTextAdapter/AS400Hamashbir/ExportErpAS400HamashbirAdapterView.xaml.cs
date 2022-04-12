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


namespace Count4U.ExportErpTextAdapter.AS400Hamashbir
{
    
    public partial class ExportErpAS400HamashbirAdapterView : UserControl
    {
		public ExportErpAS400HamashbirAdapterView(ExportErpAS400HamashbirAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
