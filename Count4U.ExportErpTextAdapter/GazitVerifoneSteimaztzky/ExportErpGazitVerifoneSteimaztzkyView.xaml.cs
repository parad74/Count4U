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

namespace Count4U.ExportErpTextAdapter.GazitVerifoneSteimaztzky
{
   
    public partial class ExportErpGazitVerifoneSteimaztzkyView : UserControl
    {
		public ExportErpGazitVerifoneSteimaztzkyView(ExportErpGazitVerifoneSteimaztzkyAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
