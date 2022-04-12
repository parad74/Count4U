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
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.ViewModels.Zip;

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for ZipImportView.xaml
    /// </summary>
    public partial class ProcessAddEditGridView : UserControl
    {
		public ProcessAddEditGridView(ProcessAddEditGridViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
//			ComplexOperationViewModel viewModel = this.DataContext as ComplexOperationViewModel;
//			if (viewModel == null) return;

//			DependencyObject depObj = e.OriginalSource as DependencyObject;
//			if (depObj == null) return;
//			DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

//			if (row != null)
//			{
//				FileItemViewModel inventorFile = row.DataContext as FileItemViewModel;
//				if (inventorFile != null)
//				{
//#if DEBUG
//					viewModel.AuditNavigateCommandExecuted(inventorFile);
//#else
					
//#endif

//				}
//			}

		}
    }
}
