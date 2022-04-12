using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.ImportCatalogKitAdapter.AS400Jafora;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportCatalogKitAdapter.AS400Jafora
{
    /// <summary>
    /// Interaction logic for ImportCatalogRetalixNextView.xaml
    /// </summary>
    public partial class ImportCatalogAS400JaforaView : UserControl
    {

		public ImportCatalogAS400JaforaView(ImportCatalogAS400JaforaViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			RegionManager.SetRegionName(control.maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
