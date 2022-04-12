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

namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for InventorEditView.xaml
    /// </summary>
	public partial class CustomerEditOptionsView : UserControl, INavigationAware
	{
		private readonly IRegionManager _regionManager;

		public CustomerEditOptionsView(CustomerEditOptionsViewModel viewModel, IRegionManager regionManager)
		{
			InitializeComponent();

			this.DataContext = viewModel;

			this.Loaded += CustomerEditView_Loaded;

			this._regionManager = regionManager;
			//Даем имена регионам и регистрируем их 
			// Во всех регионах здесь CustomerEdit
			string nameImportFolders = RegionManager.GetRegionName(common.importFolders);
			if (string.IsNullOrWhiteSpace(nameImportFolders) == true)
				RegionManager.SetRegionName(common.importFolders, Common.RegionNames.ImportFolderCustomerEdit);

			string nameexportPdaSettings = RegionManager.GetRegionName(common.exportPdaSettings);
			if (string.IsNullOrWhiteSpace(nameexportPdaSettings) == true)
				RegionManager.SetRegionName(common.exportPdaSettings, Common.RegionNames.ExportPdaSettingsCustomerEdit);

			string nameexportErpSettings = RegionManager.GetRegionName(common.exportErpSettings);
			if (string.IsNullOrWhiteSpace(nameexportErpSettings) == true)
				RegionManager.SetRegionName(common.exportErpSettings, Common.RegionNames.ExportErpSettingsCustomerEdit);

			string nameupdateFolders = RegionManager.GetRegionName(common.updateFolders);
			if (string.IsNullOrWhiteSpace(nameupdateFolders) == true)
				RegionManager.SetRegionName(common.updateFolders, Common.RegionNames.UpdateFolderCustomerEdit);


			string nameAutoGenerateResultSettings = RegionManager.GetRegionName(common.autoGenerateResultSettings);
			if (string.IsNullOrWhiteSpace(nameAutoGenerateResultSettings) == true)
				RegionManager.SetRegionName(common.autoGenerateResultSettings, Common.RegionNames.AutoGenerateResultSettingsView);

			string namedynamicColumns = RegionManager.GetRegionName(common.dynamicColumns);
			if (string.IsNullOrWhiteSpace(namedynamicColumns) == true)
				RegionManager.SetRegionName(common.dynamicColumns, Common.RegionNames.DynamicColumnSettingsCustomerEdit);

			string nameadditionalSettings = RegionManager.GetRegionName(common.additionalSettings);
			if (string.IsNullOrWhiteSpace(nameadditionalSettings) == true)
				RegionManager.SetRegionName(common.additionalSettings, Common.RegionNames.AdditionalSettingsCustomerEdit);

			string namexmlConfig = RegionManager.GetRegionName(common.configAdapterSetting);
			if (string.IsNullOrWhiteSpace(namexmlConfig) == true)
				RegionManager.SetRegionName(common.configAdapterSetting, Common.RegionNames.ConfigAdapterSettingView);
			
			
		}

		#region Implementation of INavigationAware

		void CustomerEditView_Loaded(object sender, RoutedEventArgs e)
		{
			

			//CustomerEditViewModel viewModel = this.DataContext as CustomerEditViewModel;
			//if (viewModel != null)
			//	viewModel.Owner = Window.GetWindow(this);


		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.ViewOnly))
			{
				common.btnOK.Visibility = Visibility.Collapsed;
				common.btnCancel.Content = "OK";
			}

			UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
			//к регионам по имена привязываем навигайшин
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
			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.AdditionalSettingsCustomerEdit, new Uri(Common.ViewNames.AdditionalSettingsSettingsView + query, UriKind.Relative));
			}
				catch { }

				try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.AutoGenerateResultSettingsView, new Uri(Common.ViewNames.AutoGenerateResultSettingsView + query, UriKind.Relative));
			}
			catch { }

			try
			{
				this._regionManager.RequestNavigate(Common.RegionNames.ConfigAdapterSettingView, new Uri(Common.ViewNames.ConfigAdapterSettingView + query, UriKind.Relative));
			}
			catch { }

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
			// у всех компанентов запускаем  NavigateFrom
			Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        common.importFolders,   
  														common.exportPdaSettings,
														common.exportErpSettings,
                                                        common.updateFolders,
                                                        common.dynamicColumns,
														common.autoGenerateResultSettings,
                                                        common.additionalSettings,
														common.configAdapterSetting
                                                      
                                                  }, null);

			//удаляем компанент из региона
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderCustomerEdit);
			}
			catch { }
			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.ConfigAdapterSettingView);
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

			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.AdditionalSettingsCustomerEdit);
			}
			catch { }

			try
			{
				this._regionManager.Regions.Remove(Common.RegionNames.AutoGenerateResultSettingsView);
			}
			catch { }


			INavigationAware navigationAware = this.DataContext as INavigationAware;
			if (navigationAware != null)
			{
				navigationAware.OnNavigatedFrom(null);
			}
		}
	}

	public static class Manager
	{
	// public static IEnumerable<object> FindViews(this IRegionManager regionManager,  string regionName)
	//{
	//	  IRegion region = regionManager.Regions[regionName];

	//	//return from view in region.Views
	//	//	   from attr in Attribute.GetCustomAttributes(view.GetType())
	//	//	   where attr is ViewExportAttribute && ((ViewExportAttribute)attr).ViewName == viewName
	//	//	   select view;

	//		var viewToRemove = RegionManager.Regions[RegionNames.WorkspaceTabRegion].Views.FirstOrDefault<dynamic>(v => v.ViewTitle == viewName);
	//}

	//public static void ActivateOrRequestNavigate(this IRegionManager regionManager, string regionName, string viewName, UriQuery navigationParams)
	//{
	//	IRegion region = regionManager.Regions[regionName];

	//	object view = region.FindViews(viewName).FirstOrDefault();
	//	if (view != null)
	//		region.Activate(view);
	//	else
	//		regionManager.RequestNavigate(regionName,
	//			new System.Uri(navigationParams != null ? viewName + navigationParams.ToString() : viewName, UriKind.Relative));
	//}


	//public static void RemoveAndRequestNavigate(this IRegionManager regionManager, string regionName, string viewName, UriQuery navigationParams)
	//{
	//	IRegion region = regionManager.Regions[regionName];

	//	foreach (object view in region.FindViews(viewName))
	//		region.Remove(view);

	//	regionManager.RequestNavigate(regionName,
	//			new System.Uri(navigationParams != null ? viewName + navigationParams.ToString() : viewName, UriKind.Relative));
	//}
	}
}
