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

namespace Count4U.Modules.Audit.Views.InventProduct
{
    /// <summary>
    /// Interaction logic for InventProductCloneView.xaml
    /// </summary>
    public partial class InventProductCloneView : UserControl
    {
        private readonly InventProductCloneViewModel _viewModel;

        public InventProductCloneView(InventProductCloneViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();

            this.DataContext = viewModel;

            System.Windows.Interactivity.Interaction.GetBehaviors(txtMakat).Add(new TextBoxHebrewAlignmentBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantity).Add(new TextBoxHebrewAlignmentBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantityAfter).Add(new TextBoxHebrewAlignmentBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtQuantityBefore).Add(new TextBoxHebrewAlignmentBehavior());

            this.PreviewKeyUp += InventProductCloneView_KeyUp;
            this.Loaded += InventProductCloneView_Loaded;
        }

        void InventProductCloneView_Loaded(object sender, RoutedEventArgs e)
        {
            txtQuantity.Focus();
            txtQuantity.SelectAll();
        }

        void InventProductCloneView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.CancelCommand.Execute();

                e.Handled = true;
            }
        }
    }
}
