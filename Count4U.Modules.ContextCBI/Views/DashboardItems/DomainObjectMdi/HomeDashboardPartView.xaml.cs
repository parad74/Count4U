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
using Count4U.Modules.ContextCBI.ViewModels.DashboardItems.DomainObject;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems.DomainObject
{
    /// <summary>
    /// Interaction logic for HomeDashboardPartView.xaml
    /// </summary>
    public partial class HomeDashboardPartView : UserControl
    {
        public HomeDashboardPartView(HomeDashboardPartViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
