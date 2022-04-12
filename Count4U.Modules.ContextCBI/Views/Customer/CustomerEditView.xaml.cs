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

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for ContextCustomerListView.xaml
    /// </summary>
    public partial class CustomerEditView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public CustomerEditView(CustomerEditViewModel viewModel, IRegionManager regionManager)
        {            
            InitializeComponent();

            this._regionManager = regionManager;
			this.DataContext = viewModel;            

            this.Loaded += CustomerEditView_Loaded;

			//RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderCustomerEdit);
			string nameImportFolders = RegionManager.GetRegionName(common.importFolders);
			if (string.IsNullOrWhiteSpace(nameImportFolders) == true)
				RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderCustomerEdit);

			//RegionManager.SetRegionName(common.exportPdaSettings, Common.RegionNames.ExportPdaSettingsCustomerEdit);
			string nameexportPdaSettings = RegionManager.GetRegionName(common.exportPdaSettings);
			if (string.IsNullOrWhiteSpace(nameexportPdaSettings) == true)
				RegionManager.SetRegionName(common.exportPdaSettings, Common.RegionNames.ExportPdaSettingsCustomerEdit);

			//RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsCustomerEdit);
			string nameexportErpSettings = RegionManager.GetRegionName(common.exportErpSettings);
			if (string.IsNullOrWhiteSpace(nameexportErpSettings) == true)
				RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsCustomerEdit);

			//RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderCustomerEdit);
			string nameupdateFolders = RegionManager.GetRegionName(common.updateFolders);
			if (string.IsNullOrWhiteSpace(nameupdateFolders) == true)
				RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderCustomerEdit);

			//  RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsCustomerEdit); 
			string namedynamicColumns = RegionManager.GetRegionName(common.dynamicColumns);
			if (string.IsNullOrWhiteSpace(namedynamicColumns) == true)
				RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsCustomerEdit);
  	
        }       

        void CustomerEditView_Loaded(object sender, RoutedEventArgs e)
        {
            common.ctrForm.txtName.Focus();

            CustomerEditViewModel viewModel = this.DataContext as CustomerEditViewModel;
            if (viewModel != null)
                viewModel.Owner = Window.GetWindow(this);
	       
            
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.Any(r=>r.Key == NavigationSettings.ViewOnly))
            {
				this.common.btnOK.Visibility = Visibility.Collapsed;
				this.common.btnCancel.Content = "OK";
            }

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            query.Add(Common.NavigationSettings.CustomerMode, String.Empty);

			//this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderCustomerEdit, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
			//this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettingsCustomerEdit, new Uri(Common.ViewNames.ExportPdaSettingsView + query, UriKind.Relative));
			//this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsCustomerEdit, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
			//this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderCustomerEdit, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
			//this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsCustomerEdit, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));

			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderCustomerEdit, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
			}
			catch { }
			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettingsCustomerEdit, new Uri(Common.ViewNames.ExportPdaSettingsView + query, UriKind.Relative));
			}
			catch { }
			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsCustomerEdit, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
			}
			catch { }
			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderCustomerEdit, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
			}
			catch { }
			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsCustomerEdit, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
			}
			catch { }
	
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

        private void Clear(NavigationContext navigationContext)
        {

			// у всех компанентов запускаем  NavigateFrom
			Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,   
  														common.exportPdaSettings,
														common.exportErpSettings,
                                                        common.updateFolders,
                                                        common.dynamicColumns
                                                                 
                                                  }, null);

			//Utils.NavigateFromForInnerRegions(new List<ContentControl>
			//									  {
			//											common.importFolders,  
			//											common.exportPdaSettings,
			//											common.exportErpSettings,
			//											common.updateFolders,
			//											common.dynamicColumns
                                                      
			//									  }, navigationContext);

			//this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderCustomerEdit);
			//this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsCustomerEdit);
			//this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsCustomerEdit);
			//this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsInner);
			//this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderCustomerEdit);
			//this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsCustomerEdit);

			//удаляем компанент из региона
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderCustomerEdit);
			}
			catch { }
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsCustomerEdit);
			}
			catch { }
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsCustomerEdit);
			}
			catch { }
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderCustomerEdit);
			}
			catch { }
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsCustomerEdit);
			}
			catch { }

            INavigationAware navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigatedFrom(navigationContext);
            }
        }
    }
}
