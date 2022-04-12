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
using Count4U.Modules.Prepare.ViewModules;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Prepare.Views
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : UserControl, IRegionMemberLifetime
    {
        public UpdateView(UpdateViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        public bool KeepAlive
        {
            get { return false; }
        }
    }
}
