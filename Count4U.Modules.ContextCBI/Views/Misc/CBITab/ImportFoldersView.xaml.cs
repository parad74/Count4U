using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for ImportFoldersView.xaml
    /// </summary>
    public partial class ImportFoldersView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public ImportFoldersView(ImportFoldersViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            this.DataContext = viewModel;

            _regionManager = regionManager;
			this.IsVisibleChanged += ImportFoldersView_IsVisibleChanged;

            RegionManager.SetRegionName(extraSettings, Common.RegionNames.ExportPdaExtraSettings);
        }

		void ImportFoldersView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ImportFoldersViewModel vm = this.DataContext as ImportFoldersViewModel;
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
//            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.CustomerMode))
//            {
                UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
                this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaExtraSettings, new Uri(Common.ViewNames.ExportPdaExtraSettingsView + query, UriKind.Relative));
//            }            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        extraSettings,
                                                      
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaExtraSettings);
        }
    }
}
