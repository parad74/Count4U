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

namespace Count4U.ExportHT630Adapter
{
    /// <summary>
    /// Interaction logic for ExportHT630AdapterView.xaml
	/// см Count4U.Common.View.ExportPda.ExportPdaProgramTypeView
	/// см Count4U.Common.View.ExportPda.ExportPdaSettingsControlView
	/// см ExportPdaSettingsControlViewModel
    /// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public partial class ExportHT630AdapterView : UserControl
    {
        public ExportHT630AdapterView(ExportPdaHt630AdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
