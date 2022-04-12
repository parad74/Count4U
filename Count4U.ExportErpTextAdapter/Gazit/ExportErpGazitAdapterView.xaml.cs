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

namespace Count4U.ExportErpTextAdapter.Gazit
{
    /// <summary>
    /// Interaction logic for ExportErpGazitAdapterView.xaml
    /// </summary>
    public partial class ExportErpGazitAdapterView : UserControl
    {
        public ExportErpGazitAdapterView(ExportErpGazitAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
