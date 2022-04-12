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

namespace Count4U.ImportSectionSapb1ZometsfarimAdapter
{
   
    public partial class ImportSectionSapb1ZometsfarimAdapterView : UserControl
    {
		public ImportSectionSapb1ZometsfarimAdapterView(ImportSectionSapb1ZometsfarimAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
