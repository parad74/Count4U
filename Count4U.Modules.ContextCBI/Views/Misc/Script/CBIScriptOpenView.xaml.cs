using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Script;


namespace Count4U.Modules.ContextCBI.Views.Misc.Script
{
    /// <summary>
    /// Interaction logic for CBIScriptOpenView.xaml
    /// </summary>
    public partial class CBIScriptOpenView : UserControl
    {
        public CBIScriptOpenView(CBIScriptOpenViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
