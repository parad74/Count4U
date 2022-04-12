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
using Count4U.Common.ViewModel.Adapters.Export;

namespace Count4U.ExportPdaNativPlusMISSQLiteAdapter
{
	// см ExportPdaSettingsView
    [System.ComponentModel.ToolboxItem(false)]
	public partial class ExportPdaNativPlusMISSQLiteAdapterView : UserControl
    {
		public ExportPdaNativPlusMISSQLiteAdapterView(ExportPdaNativPlusMISSQLiteAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			//this.Loaded += ExportPdaNativPlusMISSQLiteAdapterView_Loaded;
        }

		//void ExportPdaNativPlusMISSQLiteAdapterView_Loaded(object sender, RoutedEventArgs e)
		//{
		//	ExportPdaModuleBaseViewModel viewModel = this.DataContext as ExportPdaModuleBaseViewModel;
		//	if (viewModel == null) return;
		//	viewModel.IsShowIncludePreviousInventor = false;
		//	viewModel.IsShowIncludeCurrentInventor = false;
		//}
    }
}
