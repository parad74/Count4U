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
    /// Interaction logic for InventorAddView.xaml
    /// </summary>
    public partial class InventorAddView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public InventorAddView(
            InventorAddViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();
            this.DataContext = viewModel;
            this._regionManager = regionManager;

            this.Loaded += InventorAddView_Loaded;
            RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderInventorAdd);
            RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderInventorAdd);
            RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsInventorAdd);
            RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsInventorAdd);
        }

        void InventorAddView_Loaded(object sender, RoutedEventArgs e)
        {
            common.ctrForm.txtDesc.Focus();
        }       

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {            
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderInventorAdd, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderInventorAdd, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsInventorAdd, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsInventorAdd, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
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

            this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderInventorAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderInventorAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsInventorAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsInventorAdd);

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(null);
            }
        }
    }
}
