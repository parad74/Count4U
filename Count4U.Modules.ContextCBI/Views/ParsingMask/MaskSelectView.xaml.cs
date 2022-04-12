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
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask
{
    /// <summary>
    /// Interaction logic for MaskSelectView.xaml
    /// </summary>
    public partial class MaskSelectView : UserControl
    {
        public MaskSelectView(MaskSelectViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += MaskSelectView_Loaded;
        }

        void MaskSelectView_Loaded(object sender, RoutedEventArgs e)
        {
            txtInput.Focus();
        }
    }
}
