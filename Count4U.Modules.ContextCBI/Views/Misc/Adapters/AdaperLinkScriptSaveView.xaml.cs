using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Report.ViewModels.Script;

namespace Count4U.Modules.ContextCBI.Views.Misc.Adapters
{
    /// <summary>
    /// Interaction logic for ReportScriptSaveView.xaml
    /// </summary>
    public partial class AdapterLinkScriptSaveView : UserControl
    {
        public AdapterLinkScriptSaveView(AdapterLinkScriptSaveViewModel viewModel)        
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
