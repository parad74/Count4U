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
using Count4U.Modules.Audit.ViewModels;
using Count4U.Common.Behaviours;

namespace Count4U.Modules.Audit.Views.Itur
{
    public partial class IturPrefixChangeView : UserControl
    {
		public IturPrefixChangeView(IturPrefixChangeViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
