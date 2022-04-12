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
using System.Windows.Shapes;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for BranchAddView.xaml
    /// </summary>
    public partial class BranchAddView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public BranchAddView(
            BranchAddViewModel viewModel,
            IRegionManager regionManager)
        {
            this.InitializeComponent();

            this._regionManager = regionManager;
            this.DataContext = viewModel;

            this.Loaded += BranchAddView_Loaded;

            RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderBranchAdd);
            RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderBranchAdd);
            RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsBranchAdd);
            RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsBranchAdd);
        }

        private void Clear()
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,  
                                                        common.updateFolders,
                                                        common.exportErpSettings,
                                                        common.dynamicColumns
                                                      
                                                  }, null);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderBranchAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderBranchAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsBranchAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsBranchAdd);

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(null);
            }
        }

        void BranchAddView_Loaded(object sender, RoutedEventArgs e)
        {
            common.ctrForm.txtName.Focus();
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderBranchAdd, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderBranchAdd, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsBranchAdd, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsBranchAdd, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Clear();
        }

        #endregion
    }
}
