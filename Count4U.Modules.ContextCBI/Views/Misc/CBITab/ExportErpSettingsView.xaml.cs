using System.Windows;
using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for ExportErpSettingsView.xaml
    /// </summary>
    public partial class ExportErpSettingsView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public ExportErpSettingsView(
            ExportErpSettingsViewModel viewModel, 
            IRegionManager regionManager)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            _regionManager = regionManager;
			this.IsVisibleChanged += ExportErpSettingsView_IsVisibleChanged;
        }

		void ExportErpSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ExportErpSettingsViewModel vm = this.DataContext as ExportErpSettingsViewModel;
			if (vm.IsShowConfig == true)
			{
				if ((bool)e.NewValue == true && (bool)e.OldValue == false) //открываем закладку
				{
					vm.GotNewFofusConfig();
				}
				else if ((bool)e.NewValue == false && (bool)e.OldValue == true) //закрываем закладку
				{
					vm.LostFocusConfig();
				}
			}
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
