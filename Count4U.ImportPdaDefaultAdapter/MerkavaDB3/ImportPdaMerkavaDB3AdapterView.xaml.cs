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

namespace Count4U.ImportPdaMerkavaDB3Adapter
{

	public partial class ImportPdaMerkavaDB3AdapterView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
		public ImportPdaMerkavaDB3AdapterView(ImportPdaMerkavaDB3AdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
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
    }
}
