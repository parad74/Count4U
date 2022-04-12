using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Behaviours;
using Count4U.Modules.Audit.ViewModels;

namespace Count4U.Modules.Audit.Views.DocumentHeader
{
    /// <summary>
    /// Interaction logic for DocumentHeaderAddEditView.xaml
    /// </summary>
    public partial class DocumentHeaderAddEditView : UserControl
    {
        public DocumentHeaderAddEditView(DocumentHeaderAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += DocumentHeaderAddEditView_Loaded;
            this.PreviewKeyUp += DocumentAddEditView_KeyUp;

            TextChangedDelayedBehavior behavior = new TextChangedDelayedBehavior();
            behavior.IsTimerEnabled = true;
            behavior.Attach(txtDocumentCode);            
        }

        void DocumentHeaderAddEditView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            txtName.Focus();
        }

        void DocumentAddEditView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DocumentHeaderAddEditViewModel viewModel = this.DataContext as DocumentHeaderAddEditViewModel;
                if (viewModel != null)
                {
                    viewModel.CancelCommand.Execute();
                    e.Handled = true;
                }
            }
        }
    }
}
