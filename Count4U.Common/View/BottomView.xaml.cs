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
using Count4U.Common.ViewModel;

namespace Count4U.Common.View
{
    /// <summary>
    /// Interaction logic for BottomView.xaml
    /// </summary>
    public partial class BottomView : UserControl
    {
        public BottomView(BottomViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
