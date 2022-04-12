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

namespace Count4U.ComplexAutoDocInvAdapter
{

	public partial class ComplexAutoDocInvAdapterView : UserControl, INavigationAware, IRegionMemberLifetime 
    {

		public ComplexAutoDocInvAdapterView(ComplexAutoDocInvAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			btnFromFtp.PreviewMouseDoubleClick += btnFromFtp_PreviewMouseDoubleClick;
			btntToFtp.PreviewMouseDoubleClick += btntToFtp_PreviewMouseDoubleClick;
        }

		void btnFromFtp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		void btntToFtp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
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
