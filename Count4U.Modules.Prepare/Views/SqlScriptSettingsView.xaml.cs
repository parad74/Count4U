using System.Windows.Controls;
using Count4U.Modules.Prepare.ViewModules;

namespace Count4U.Modules.Prepare.Views
{
    /// <summary>
    /// Interaction logic for SqlScriptSettingsView.xaml
    /// </summary>
    public partial class SqlScriptSettingsView : UserControl
    {
		
        public SqlScriptSettingsView(SqlScriptSettingsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
