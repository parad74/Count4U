using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for ExportPdaSettingsView.xaml
    /// </summary>
    public partial class ExportPdaSettingsView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public ExportPdaSettingsView(ExportPdaSettingsViewModel viewModel, IRegionManager regionManager)
        {
            InitializeComponent();

            this._regionManager = regionManager;

            this.DataContext = viewModel;
			this.IsVisibleChanged += ExportPdaSettingsView_IsVisibleChanged;
			//this.LostFocus += CustomerEditView_LostFocus;

			RegionManager.SetRegionName(exportPdaAdapter, Common.RegionNames.ExportPdaAdapter);
            RegionManager.SetRegionName(exportControl, Common.RegionNames.ExportPdaSettingsInner);
            RegionManager.SetRegionName(programTypeControl, Common.RegionNames.ExportPdaProgramType);
        }


		void ExportPdaSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ExportPdaSettingsViewModel vm = this.DataContext as ExportPdaSettingsViewModel;
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

		//void CustomerEditView_LostFocus(object sender, RoutedEventArgs e)
		//{
		//	ExportPdaSettingsViewModel vm = this.DataContext as ExportPdaSettingsViewModel;
		//	vm.ClearConfig();
		//}
        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);

			this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaAdapter, new Uri(Common.ViewNames.ExportPdaMerkavaAdapterView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettingsInner, new Uri(Common.ViewNames.ExportPdaSettingsControlView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaProgramType, new Uri(Common.ViewNames.ExportPdaProgramTypeView + query, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {		
														exportPdaAdapter,
                                                        exportControl,      
                                                        programTypeControl 
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsInner);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaProgramType);
			this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaAdapter);
	
        }

        #endregion
    }
}
