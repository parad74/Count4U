using System.Windows.Controls;
using Count4U.Common.ViewModel.ExportPda;
using Count4U.Report.ViewModels.ExportPda;

namespace Count4U.Report.Views.ExportPda
{
    /// <summary>
    /// Interaction logic for ExportPdaExtraSettingsView.xaml
    /// </summary>
    public partial class ExportPdaExtraSettingsView : UserControl
    {
        public ExportPdaExtraSettingsView(ExportPdaExtraSettingsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
