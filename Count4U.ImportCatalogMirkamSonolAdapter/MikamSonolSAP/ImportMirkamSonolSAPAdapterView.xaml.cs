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
using Count4U.ImportCatalogMirkamSonolAdapter.MikamSonolSAP;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportCatalogMirkamSonolAdapter.MikamSonolSAP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImportMirkamSonolSAPAdapterView : UserControl
    {
        public ImportMirkamSonolSAPAdapterView(ImportMirkamSonolSAPAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            RegionManager.SetRegionName(maskControl, viewModel.BuildMaskRegionName());
        }
    }
}
