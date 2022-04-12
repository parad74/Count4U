using System.Windows.Controls;
using Count4U.Common.ViewModel.ExportPda;

namespace Count4U.Common.View.ExportPda
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
