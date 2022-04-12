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
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportPdaCompareCount4UdbSdfAdapter
{

    public partial class ImportPdaCompareCount4UdbSdfAdapterView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
		public ImportPdaCompareCount4UdbSdfAdapterView(ImportPdaCompareCount4UdbSdfAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
		{
            byMakatCheckBox.Visibility = System.Windows.Visibility.Visible; 
#if DEBUG
			withSerialNumberCheckBox.Visibility = System.Windows.Visibility.Visible;
			//byMakatCheckBox.Visibility = System.Windows.Visibility.Visible; 
#else
			withSerialNumberCheckBox.Visibility = System.Windows.Visibility.Hidden; 
			//byMakatCheckBox.Visibility = System.Windows.Visibility.Hidden; 
#endif

		}

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public bool KeepAlive
        {
            get { return false; }
        }
		//withSerialNumberCheckBox
    }
}
