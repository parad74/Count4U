using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportCatalogGazitVerifonePriceAdapter
{
    /// <summary>
    /// Interaction logic for ImportView.xaml
    /// </summary>
    public partial class ImportGazitVerifonePriceAdapterView : UserControl, INavigationAware
    {
		public ImportGazitVerifonePriceAdapterView(ImportGazitVerifonePriceAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            RegionManager.SetRegionName(control.maskControl1, viewModel.BuildMaskRegionName("1"));
            RegionManager.SetRegionName(control.maskControl2, viewModel.BuildMaskRegionName("2"));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
