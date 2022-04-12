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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for CustomerAddView.xaml
    /// </summary>
    public partial class CustomerAddView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public CustomerAddView(CustomerAddViewModel viewModel, IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            this.InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += CustomerAddView_Loaded;

            RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderCustomerAdd);
            RegionManager.SetRegionName(common.exportPdaSettings, Common.RegionNames.ExportPdaSettingsCustomerAdd);
            RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsCustomerAdd);
            RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderCustomerAdd);
            RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsCustomerAdd);        
        }

        private void Clear(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,  
                                                        common.exportPdaSettings,
                                                        common.exportErpSettings,
                                                        common.updateFolders,
                                                        common.dynamicColumns
                                                      
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderCustomerAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsCustomerAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsCustomerAdd);
            //this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsInner);
            this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderCustomerAdd);
            this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsCustomerAdd);

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(navigationContext);
            }
        }

        void CustomerAddView_Loaded(object sender, RoutedEventArgs e)
        {
            common.ctrForm.txtName.Focus();
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            query.Add(Common.NavigationSettings.CustomerMode, String.Empty);
            this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderCustomerAdd, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettingsCustomerAdd, new Uri(Common.ViewNames.ExportPdaSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsCustomerAdd, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderCustomerAdd, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsCustomerAdd, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Clear(navigationContext);
        }

        #endregion

        
    }
}
