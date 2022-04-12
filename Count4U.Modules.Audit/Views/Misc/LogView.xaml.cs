using System.Windows.Controls;
using Count4U.Modules.Audit.ViewModels.Misc;

namespace Count4U.Modules.Audit.Views.Misc
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView(LogViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
