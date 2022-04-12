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
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems;
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems
{   
    public partial class MenuDashboardPartView : UserControl
    {
        public MenuDashboardPartView(MenuDashboardPartViewModel partViewModel)
        {
			this.InitializeComponent();

            this.DataContext = partViewModel;

            this.Loaded += MenuDashboardPartView_Loaded;
        }

        void MenuDashboardPartView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
