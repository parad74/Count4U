using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Modules.Audit.ViewModels;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomer.xaml
    /// </summary>
    public partial class SearchInventProductView : UserControl
    {
        public SearchInventProductView(SearchInventProductViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;
			viewModel.DataGrid = dataGridInventProduct;

			dataGridInventProduct.SelectionChanged += listView_SelectionChanged;
			//this.PreviewKeyDown += SearchInventProductView_PreviewKeyDown;
			
			Interaction.GetBehaviors(dataGridInventProduct).Add(new GridCancelEditBehavior());
			Interaction.GetBehaviors(dataGridInventProduct).Add(new DataGridBehavior());
					
           dataGridInventProduct.MouseDoubleClick += dataGrid_MouseDoubleClick;

        }

	

		void SearchInventProductView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
				e.Handled = true;
           }
		   //  if (e.Key == Key.Enter)
		   // {
		   //	 return;
		   //}
        }

		void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{																													//detailDataGrid
			SearchInventProductViewModel viewModel = this.DataContext as SearchInventProductViewModel;
			if (viewModel != null)
			{
				//viewModel.SelectedItemSet(viewModel.SelectedItem);
				if (viewModel.InventProductChooseCurrent != null)
				{
					dataGridInventProduct.ScrollIntoView(viewModel.InventProductChooseCurrent);
				}

			}
		}

        void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			SearchInventProductViewModel viewModel = this.DataContext as SearchInventProductViewModel;
			if (viewModel == null)
				return;

			DependencyObject depObj = e.OriginalSource as DependencyObject;
			if (depObj == null)
				return;
			DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);
			TextBox textBox = VisualTreeHelpers.FindParent<TextBox>(depObj);

			if (row != null)
			{
				InventProductItemViewModel inventProduct = row.DataContext as InventProductItemViewModel;
				if (inventProduct != null)
				{
					if (textBox == null)
					{
						viewModel.InventProductNavigate(inventProduct);
					}
					else if (textBox.Name != "textBox123")
					{
						viewModel.InventProductNavigate(inventProduct);
					}
				}
			}
		}

		private void QuantityEditTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			TextBox textBox123 = sender as TextBox;
			if (textBox123 == null) return;

			textBox123.SelectAll();
		}

		private void QuantityEditTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{ }
			               // _escKeyByQuantityEditTextBox = true;
		}
    }
}
