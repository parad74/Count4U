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
using Count4U.Common;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorEditView.xaml
    /// </summary>
    public partial class InventorEditView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public InventorEditView(InventorEditViewModel viewModel, IRegionManager regionManager)
        {            
            InitializeComponent();

            this.DataContext = viewModel;
            this._regionManager = regionManager;
            RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderInventorEdit);
            RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderInventorEdit);
            RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsInventorEdit);
            RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsInventorEdit);
        }      

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly))
            {
				common.btnOK.Visibility = Visibility.Collapsed;
                common.btnCancel.Content = "OK";
            }

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderInventorEdit, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderInventorEdit, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsInventorEdit, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsInventorEdit, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
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

        private void Clear()
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,     
                                                        common.updateFolders,
                                                        common.exportErpSettings,
                                                        common.dynamicColumns
                                                      
                                                  }, null);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderInventorEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderInventorEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsInventorEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsInventorEdit);

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(null);
            }
        }
    }
}
