using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Report.ViewModels;
using Count4U.Report.ViewModels.Script;

namespace Count4U.Modules.ContextCBI.Views.Misc.Adapters
{
    /// <summary>
    /// Interaction logic for ReportScriptView.xaml
    /// </summary>
    public partial class AdapterLinkScriptOpenView : UserControl
    {
        public AdapterLinkScriptOpenView(AdapterLinkScriptOpenViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

#if DEBUG
            this.toSetupDB.Visibility = System.Windows.Visibility.Visible;
#else
            this.toSetupDB.Visibility =System.Windows.Visibility.Collapsed;
#endif
        }
    }
}
