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

namespace Count4U.ImportPdaAdvancedAdapter
{
    /// <summary>
    /// Interaction logic for ImportPdaAdvancedAdapter1View.xaml
    /// </summary>
    public partial class ImportPdaAdvancedAdapter1View : UserControl
    {
        public ImportPdaAdvancedAdapter1View(ImportPdaAdvancedAdapter1ViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
