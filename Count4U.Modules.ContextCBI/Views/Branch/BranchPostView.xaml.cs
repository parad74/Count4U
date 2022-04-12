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

namespace Count4U.Modules.ContextCBI.Views.Branch
{
    /// <summary>
    /// Interaction logic for BranchPostView.xaml
    /// </summary>
    public partial class BranchPostView : UserControl
    {
        public BranchPostView(BranchPostViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
