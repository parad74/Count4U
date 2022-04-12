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

namespace Count4U.ExportErpTemplateAdapter.NativPlusMateAshe
{

	public partial class ExportErpNativPlusMateAsheAdapterView : UserControl
    {
		public ExportErpNativPlusMateAsheAdapterView(ExportErpNativPlusMateAsheAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
