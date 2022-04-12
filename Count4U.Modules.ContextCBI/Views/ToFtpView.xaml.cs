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
using Count4U.Common.Web;

namespace Count4U.Common.Web
{

    public partial class ToFtpView : UserControl
    {
		public ToFtpView(ToFtpViewModel viewModel)
      {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
