using System.Windows.Controls;
using System.Windows;
using Count4U.Report.ViewModels.Script;

namespace Count4U.Report.Views.Script
{
    /// <summary>
    /// Interaction logic for ReportScriptSaveView.xaml
    /// </summary>
    public partial class ReportScriptSaveView : UserControl
    {
        public ReportScriptSaveView(ReportScriptSaveViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
#if DEBUG
			this.toSetupDB.Visibility = Visibility.Collapsed; //Visibility.Visible;
#else
            this.toSetupDB.Visibility = Visibility.Collapsed;
#endif
		}
    }
}
