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
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Reports;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems
{
    /// <summary>
    /// Interaction logic for ReportsDashboardPartView.xaml
    /// </summary>
    public partial class ReportsDashboardPartView : UserControl
    {
        public ReportsDashboardPartView(ReportsDashboardPartViewModel viewModel)
        {
			this.InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
