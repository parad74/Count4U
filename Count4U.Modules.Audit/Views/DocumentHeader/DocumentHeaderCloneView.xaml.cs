using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Modules.Audit.ViewModels;
using System.Windows.Threading;

namespace Count4U.Modules.Audit.Views.DocumentHeader
{
    /// <summary>
    /// Interaction logic for InventProductCloneView.xaml
    /// </summary>
    public partial class DocumentHeaderCloneView : UserControl
    {
		private readonly DocumentHeaderCloneViewModel _viewModel;

		public DocumentHeaderCloneView(DocumentHeaderCloneViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();

            this.DataContext = viewModel;

			System.Windows.Interactivity.Interaction.GetBehaviors(txtIturCode).Add(new TextBoxHebrewAlignmentLeftBehavior());
			System.Windows.Interactivity.Interaction.GetBehaviors(txtERPIturCode).Add(new TextBoxHebrewAlignmentLeftBehavior());
			System.Windows.Interactivity.Interaction.GetBehaviors(txtFromIturCode).Add(new TextBoxHebrewAlignmentLeftBehavior());
			System.Windows.Interactivity.Interaction.GetBehaviors(txtFromERPIturCode).Add(new TextBoxHebrewAlignmentLeftBehavior());
			
			//System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantity).Add(new TextBoxHebrewAlignmentBehavior());
			//System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantityAfter).Add(new TextBoxHebrewAlignmentBehavior());
			//System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantityBefore).Add(new TextBoxHebrewAlignmentBehavior());

            this.PreviewKeyUp += DocumentHeaderCloneView_KeyUp;
            this.Loaded += DocumentHeaderCloneView_Loaded;
        }

        void DocumentHeaderCloneView_Loaded(object sender, RoutedEventArgs e)
        {
			//txtQuantity.Focus();
			//txtQuantity.SelectAll();

            DocumentHeaderCloneViewModel viewModel = this.DataContext as DocumentHeaderCloneViewModel;
            Dispatcher.BeginInvoke(new Action(() => viewModel.IsTimerEnabled = true), DispatcherPriority.Background);

			txtIturCode.Focus();
			txtIturCode.SelectAll();
        }

        void DocumentHeaderCloneView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.CancelCommand.Execute();

                e.Handled = true;
            }
        }
    }
}
