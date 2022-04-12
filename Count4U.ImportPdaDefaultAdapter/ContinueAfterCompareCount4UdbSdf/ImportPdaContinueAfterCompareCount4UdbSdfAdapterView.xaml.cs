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
using Count4U.Common.Helpers;
using Count4U.Model.ExportImport.Items;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportPdaContinueAfterCompareCount4UdbSdfAdapter
{

    public partial class ImportPdaContinueAfterCompareCount4UdbSdfAdapterView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
		public ImportPdaContinueAfterCompareCount4UdbSdfAdapterView(ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
		{
//#if DEBUG
//			deleteInv.Visibility = System.Windows.Visibility.Visible;
//#else
//			deleteInv.Visibility = System.Windows.Visibility.Hidden; 
//#endif


		}

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel viewModel = this.DataContext as ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel;
			if (viewModel == null) return;

			DependencyObject depObj = e.OriginalSource as DependencyObject;
			if (depObj == null) return;
			DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

			if (row != null)
			{
				FileItemViewModel inventorFile = row.DataContext as FileItemViewModel;
				if (inventorFile != null)
				{
					viewModel.AuditNavigateCommandExecuted(inventorFile);
				}
			}
		
		}

        public bool KeepAlive
        {
            get { return false; }
        }
		//withSerialNumberCheckBox
    }
}
