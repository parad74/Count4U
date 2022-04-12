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
using Count4U.Modules.Audit.ViewModels.Export;

namespace Count4U.Modules.Audit.Views.Export
{
    /// <summary>
    /// Interaction logic for UploadToPdaView.xaml
    /// </summary>
    public partial class DownloadFromPDAView : UserControl
    {
		//public DownloadFromPDAView(DownloadFromPDAViewModel viewModel)
		public DownloadFromPDAView(DownloadBaseFromPdaViewModel viewModel)
      {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
