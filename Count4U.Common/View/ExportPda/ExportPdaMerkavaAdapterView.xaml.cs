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
using Count4U.Common.ViewModel.ExportPda;

namespace Count4U.Common.View.ExportPda
{
 

    public partial class ExportPdaMerkavaAdapterView : UserControl
    {
		public ExportPdaMerkavaAdapterView(
			ExportPdaMerkavaAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
