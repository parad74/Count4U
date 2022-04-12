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
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.SpeedLink;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.SpeedLink
{
    /// <summary>
    /// Interaction logic for SpeedLinkView.xaml
    /// </summary>
    public partial class SpeedLinkView : UserControl
    {
        public SpeedLinkView(SpeedLinkViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.View = this;
        }
    }
}
