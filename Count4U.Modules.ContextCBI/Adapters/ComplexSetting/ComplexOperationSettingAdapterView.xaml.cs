using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Adapters.ComplexSetting
{
    /// <summary>
	/// ComplexOperationEmpty.ComplexOperationEmptyAdapterView
    /// Interaction logic for UpdateCatalogEmptyView.xaml
    /// </summary>
	public partial class ComplexOperationSettingAdapterView : UserControl
    {
		private readonly IRegionManager _regionManager;

		//private ImportFoldersViewModel _importFoldersViewModel;
		//private ExportPdaSettingsViewModel _exportPdaSettingsViewModel;
		//private ExportErpSettingsViewModel _exportErpSettingsViewModel;
		//private UpdateAdaptersViewModel _updateViewModel;
		//private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;

		public ComplexOperationSettingAdapterView()
        {
            InitializeComponent();

			//this.DataContext = viewModel;
			//_regionManager = regionManager;
			tbGeneral.MouseLeftButtonUp += tbGeneral_MouseLeftButtonUp;
			tbImportAdapters.MouseLeftButtonUp += tbImportAdapters_MouseLeftButtonUp;
			tbPda.MouseLeftButtonUp += tbPda_MouseLeftButtonUp;
			tbErp.MouseLeftButtonUp += tbErp_MouseLeftButtonUp;
			tbUpdate.MouseLeftButtonUp += tbUpdate_MouseLeftButtonUp;
			tbDynamicColumns.MouseLeftButtonUp += tbDynamicColumns_MouseLeftButtonUp;
			btnNext.Click += btnNext_Click;

			this.Loaded += CustomerAddView_Loaded;

			RegionManager.SetRegionName(importFolders, Common.RegionNames.ImportFolderCustomerAdd);
			RegionManager.SetRegionName(exportPdaSettings, Common.RegionNames.ExportPdaSettingsCustomerAdd);
			RegionManager.SetRegionName(exportErpSettings, Common.RegionNames.ExportErpSettingsCustomerAdd);
			RegionManager.SetRegionName(updateFolders, Common.RegionNames.UpdateFolderCustomerAdd);
			RegionManager.SetRegionName(dynamicColumns, Common.RegionNames.DynamicColumnSettingsCustomerAdd);
			RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);

			
        }


		private void Clear(NavigationContext navigationContext)
		{
			Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        importFolders,  
                                                        exportPdaSettings,
                                                        exportErpSettings,
                                                        updateFolders,
                                                        dynamicColumns ,
														contentModule
                                                      	//ctrForm
                                                  }, navigationContext);

			this._regionManager.Regions.Remove(Common.RegionNames.ImportFolderCustomerAdd);
			this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsCustomerAdd);
			this._regionManager.Regions.Remove(Common.RegionNames.ExportErpSettingsCustomerAdd);
			//this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettingsInner);
			this._regionManager.Regions.Remove(Common.RegionNames.UpdateFolderCustomerAdd);
			this._regionManager.Regions.Remove(Common.RegionNames.DynamicColumnSettingsCustomerAdd);
			this._regionManager.Regions.Remove(Common.RegionNames.ImportByModule);

			INavigationAware navigationAware = this.DataContext as INavigationAware;
			if (navigationAware != null)
			{
				navigationAware.OnNavigatedFrom(navigationContext);
			}
		}

		void CustomerAddView_Loaded(object sender, RoutedEventArgs e)
		{
			importFolders.Focus();
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
			//query.Add(Common.NavigationSettings.CustomerMode, String.Empty);
			this._regionManager.RequestNavigate(Common.RegionNames.ImportFolderCustomerAdd, new Uri(Common.ViewNames.ImportFoldersView + query, UriKind.Relative));
			this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettingsCustomerAdd, new Uri(Common.ViewNames.ExportPdaSettingsView + query, UriKind.Relative));
			this._regionManager.RequestNavigate(Common.RegionNames.ExportErpSettingsCustomerAdd, new Uri(Common.ViewNames.ExportErpSettingsView + query, UriKind.Relative));
			this._regionManager.RequestNavigate(Common.RegionNames.UpdateFolderCustomerAdd, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
			this._regionManager.RequestNavigate(Common.RegionNames.DynamicColumnSettingsCustomerAdd, new Uri(Common.ViewNames.DynamicColumnSettingsView + query, UriKind.Relative));
			//!!!			    TODO add new View
			this._regionManager.RequestNavigate(Common.RegionNames.ImportByModule, new Uri(Common.ViewNames.UpdateAdaptersView + query, UriKind.Relative));
			
			//InitControls();
		}

		//private void InitControls()
		//{
		//	_importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderCustomerAdd, this._regionManager);
		//	if (_importFoldersViewModel != null)
		//	{
		//		_importFoldersViewModel.SetIsEditable(true);
		//		//_importFoldersViewModel.SetCustomer(_customer);
		//	}

		//	_exportPdaSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsViewModel>(Common.RegionNames.ExportPdaSettingsCustomerAdd, this._regionManager);
		//	if (_exportPdaSettingsViewModel != null)
		//	{
		//		_exportPdaSettingsViewModel.SetIsEditable(true);
		//		_exportPdaSettingsViewModel.SetIsNew(true);
		//		//_exportPdaSettingsViewModel.SetCustomer(_customer, true);
		//		//_exportPdaSettingsViewModel.CheckValidation += ExportPdaSettingsViewModel_CheckValidation;
		//	}

		//	_exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsCustomerAdd, this._regionManager);
		//	if (_exportErpSettingsViewModel != null)
		//	{
		//		_exportErpSettingsViewModel.SetIsEditable(true);
		//		//_exportErpSettingsViewModel.SetCustomer(_customer);
		//	}

		//	_updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderCustomerAdd, this._regionManager);
		//	if (_updateViewModel != null)
		//	{
		//		_updateViewModel.SetIsEditable(true);
		//		//_updateViewModel.SetCustomer(_customer);
		//	}

		//	_dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsCustomerAdd, this._regionManager);
		//	if (_dynamicColumnsViewModel != null)
		//	{
		//		_dynamicColumnsViewModel.SetIsEditable(true);
		//		//_dynamicColumnsViewModel.SetCustomer(_customer);
		//	}
		//}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		//public void OnNavigatedFrom(NavigationContext navigationContext)
		//{
		//	if (_exportPdaSettingsViewModel != null)
		//	{
		//		//_exportPdaSettingsViewModel.CheckValidation -= ExportPdaSettingsViewModel_CheckValidation;
		//		_exportPdaSettingsViewModel.OnNavigatedFrom(navigationContext);
		//	}

		//	if (_importFoldersViewModel != null)
		//	{
		//		_importFoldersViewModel.OnNavigatedFrom(navigationContext);
		//	}

		//	if (_exportErpSettingsViewModel != null)
		//	{
		//		_exportErpSettingsViewModel.OnNavigatedFrom(navigationContext);
		//	}

		//	if (_updateViewModel != null)
		//	{
		//		_updateViewModel.OnNavigatedFrom(navigationContext);
		//	}

		//	if (_dynamicColumnsViewModel != null)
		//	{
		//		_dynamicColumnsViewModel.OnNavigatedFrom(navigationContext);
		//	}

		//	Clear(navigationContext);
		//}

		void tbGeneral_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(contentModule);
		}

		void tbImportAdapters_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(importFolders);
		}

		void tbErp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(exportErpSettings);
		}

		void tbPda_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(exportPdaSettings);
		}

		void tbUpdate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(updateFolders);
		}

		void tbDynamicColumns_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(dynamicColumns);
		}

		private void btnNext_Click(object sender, RoutedEventArgs e)
		{
			if (importFolders.Visibility == Visibility.Visible)
			{
				MakeVisible(exportPdaSettings);
			}
			else if (exportPdaSettings.Visibility == Visibility.Visible)
			{
				MakeVisible(exportErpSettings);
			}
			else if (exportErpSettings.Visibility == Visibility.Visible)
			{
				MakeVisible(updateFolders);
			}
			else if (updateFolders.Visibility == Visibility.Visible)
			{
				MakeVisible(dynamicColumns);
			}
			else if (dynamicColumns.Visibility == Visibility.Visible)
			{
				MakeVisible(contentModule);
			}
			else if (contentModule.Visibility == Visibility.Visible)
			{
				MakeVisible(importFolders);
			}
		}

		private void MakeVisible(FrameworkElement fe)
		{
			Thickness to = default(Thickness);

		if (fe == importFolders)
			{
				contentModule.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Visible;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 45, 0, 0);
			}
			else if (fe == exportPdaSettings)
			{
				contentModule.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Visible;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 89, 0, 0);
			}
			else if (fe == exportErpSettings)
			{
				contentModule.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Visible;
				updateFolders.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 132, 0, 0);
			}
			else if (fe == updateFolders)
			{
				contentModule.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Visible;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 177, 0, 0);
			}
			else if (fe == dynamicColumns)
			{
				contentModule.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Visible;

				to = new Thickness(-10, 221, 0, 0);
			}
		else if (fe == contentModule)
			{
				contentModule.Visibility = Visibility.Visible;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 265, 0, 0);
			}

			//175

			ThicknessAnimation animation = new ThicknessAnimation();
			animation.From = pathArrow.Margin;
			animation.To = to;
			animation.Duration = TimeSpan.FromMilliseconds(200);
			pathArrow.BeginAnimation(MarginProperty, animation);
		}
    }
}
