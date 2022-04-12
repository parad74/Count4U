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
using Count4U.Modules.Audit.ViewModels;
using Count4U.Common.Behaviours;
using System.Windows.Interactivity;

namespace Count4U.Modules.Audit.Views.Itur
{
   	public partial class ShowShelfView : UserControl // см InventProductListDetailsView
    {
		private bool _escKeyByQuantityEditTextBox;

		public ShowShelfView(ShowShelfViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
			viewModel.ShelfDataGrid = dataGrid;

			this.dataGrid.PreviewKeyDown += dataGrid_KeyDown;
			this.PreviewKeyUp += ShowShelfView_PreviewKeyUp;
			this.PreviewKeyDown += ShowShelfView_PreviewKeyDown;
			this.Loaded += ShowShelfView_Loaded;
			this.dataGrid.PreviewKeyDown += dataGridInventProduct_KeyDown;


			Interaction.GetBehaviors(dataGrid).Add(new GridCancelEditBehavior());
			Interaction.GetBehaviors(dataGrid).Add(new DataGridBehavior());

        }

		void ShowShelfView_Loaded(object sender, RoutedEventArgs e)
		{

			System.Windows.Interactivity.Interaction.GetBehaviors(txtIturWidth).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			System.Windows.Interactivity.Interaction.GetBehaviors(txtIturHeight).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			System.Windows.Interactivity.Interaction.GetBehaviors(txtShelvesInItur).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });

			
		}

		void ShowShelfView_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				if (_escKeyByQuantityEditTextBox)
				{
					_escKeyByQuantityEditTextBox = false;
					return;
				}

				//if (UtilsNavigate.CanGoBack(this._regionManager))
				//	UtilsNavigate.GoBack(this._regionManager);
			}
		}

		void dataGridInventProduct_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				//InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
				//if (viewModel != null)
				//{
					//if (viewModel.DeleteInventProductCommand.CanExecute())
					//{
					//	viewModel.DeleteInventProductCommandExecute();
					//}
				//}
			}
		}

		void ShowShelfView_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && e.OriginalSource is TextBox)
			{
				_escKeyByQuantityEditTextBox = true;
			}
		}

		private void QuantityEditTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			//            TextBox textBox = sender as TextBox;
			//            if (textBox == null) return;
			//
			//            textBox.SelectAll();
		}

		private void QuantityEditTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			//            if (e.Key == Key.Escape)
			//                _escKeyByQuantityEditTextBox = true;
		}

		void dataGrid_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				ShelfItemViewModel viewModel = this.DataContext as ShelfItemViewModel;
				if (viewModel != null)
				{
					//if (viewModel.DeleteInventProductCommand.CanExecute())
					//{
					//	viewModel.DeleteInventProductCommandExecute();
					//}
				}
			}
		}
    }
}
