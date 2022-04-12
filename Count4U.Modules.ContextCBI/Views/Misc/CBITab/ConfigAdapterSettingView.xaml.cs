using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for ExportErpSettingsView.xaml
    /// </summary>
    public partial class ConfigAdapterSettingView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

		public ConfigAdapterSettingView(
			ConfigAdapterSettingViewModel viewModel, 
            IRegionManager regionManager)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            _regionManager = regionManager;
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
