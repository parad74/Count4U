using System.Windows.Controls;
using System.Windows;
using Count4U.Report.ViewModels;
using Count4U.Report.ViewModels.Script;

namespace Count4U.Report.Views.Script
{
    /// <summary>
    /// Interaction logic for ReportScriptView.xaml
    /// </summary>
    public partial class ReportScriptView : UserControl
    {
        public ReportScriptView(ReportScriptOpenViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
#if DEBUG
			this.toSetupDB.Visibility = Visibility.Visible;
#else
            this.toSetupDB.Visibility = Visibility.Collapsed;
#endif
        }
    }
}
