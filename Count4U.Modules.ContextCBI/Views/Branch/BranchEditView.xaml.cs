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

namespace Count4U.Modules.ContextCBI.Views.Branch
{
    /// <summary>
    /// Interaction logic for BranchEditView.xaml
    /// </summary>
    public partial class BranchEditView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public BranchEditView(BranchEditViewModel viewModel, IRegionManager regionManager)
        {
            this.InitializeComponent();

            this._regionManager = regionManager;

            this.DataContext = viewModel;

            this.Loaded += BranchEditView_Loaded;

            RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderBranchEdit);
            RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderBranchEdit);
            RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsBranchEdit);
            RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsBranchEdit);
        }

        private void Clear()
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,  
                                                        common.updateFolders,
                                                        common.exportErpSettings,
                                                        common.dynamicColumns,
                                                      
                                                  }, null);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderBranchEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderBranchEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsBranchEdit);
            this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsBranchEdit);

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(null);
            }
        }

        void BranchEditView_Loaded(object sender, RoutedEventArgs e)
        {
            common.ctrForm.txtName.Focus();

            BranchEditViewModel viewModel = this.DataContext as BranchEditViewModel;
            if (viewModel != null)
                viewModel.Owner = Window.GetWindow(this);
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
            this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderBranchEdit, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderBranchEdit, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsBranchEdit, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
            this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsBranchEdit, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
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
