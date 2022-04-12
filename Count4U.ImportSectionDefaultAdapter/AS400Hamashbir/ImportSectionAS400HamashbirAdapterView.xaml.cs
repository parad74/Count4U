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

namespace Count4U.ImportSectionAS400HamashbirAdapter
{
   
    public partial class ImportSectionAS400HamashbirAdapterView : UserControl
    {
		public ImportSectionAS400HamashbirAdapterView(ImportSectionAS400HamashbirAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
