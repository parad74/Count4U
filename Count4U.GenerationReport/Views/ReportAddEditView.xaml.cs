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
using Count4U.Report.ViewModels;

namespace Count4U.Report.Views
{
    /// <summary>
    /// Interaction logic for ReportAddEditView.xaml
    /// </summary>
    public partial class ReportAddEditView : UserControl
    {
        public ReportAddEditView(ReportAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
