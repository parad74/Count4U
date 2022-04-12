using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorCommon.xaml
    /// </summary>
	public partial class InventorOptionsCommon : UserControl
    {
		//private readonly IRegionManager _regionManager;
		public InventorOptionsCommon()
        {
			//_regionManager = regionManager;
            InitializeComponent();

			//tbGeneral.MouseLeftButtonUp += tbGeneral_MouseLeftButtonUp;
			//tbImportAdapters.MouseLeftButtonUp += tbImportAdapters_MouseLeftButtonUp;
			//tbErp.MouseLeftButtonUp += tbErp_MouseLeftButtonUp;
			//tbUpdate.MouseLeftButtonUp += tbUpdate_MouseLeftButtonUp;
			//btnNext.Click += btnNext_Click;
			//tbDynamicColumns.MouseLeftButtonUp += tbDynamicColumns_MouseLeftButtonUp;

			tbGeneral.MouseLeftButtonUp += tbGeneral_MouseLeftButtonUp;
			tbImportAdapters.MouseLeftButtonUp += tbImportAdapters_MouseLeftButtonUp;
			tbPda.MouseLeftButtonUp += tbPda_MouseLeftButtonUp;
			tbErp.MouseLeftButtonUp += tbErp_MouseLeftButtonUp;
			tbAutoResult.MouseLeftButtonUp += tbAutoResult_MouseLeftButtonUp;
			tbUpdate.MouseLeftButtonUp += tbUpdate_MouseLeftButtonUp;
			tbDynamicColumns.MouseLeftButtonUp += tbDynamicColumns_MouseLeftButtonUp;
			btnNext.Click += btnNext_Click;
			MakeVisible(importFolders);

	    }

		void tbGeneral_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(additionalSettings);
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


		void tbAutoResult_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MakeVisible(autoGenerateResultSettings);
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
				MakeVisible(autoGenerateResultSettings);
			}
			else if (autoGenerateResultSettings.Visibility == Visibility.Visible)
			{
				MakeVisible(dynamicColumns);
			}
			else if (dynamicColumns.Visibility == Visibility.Visible)
			{
				MakeVisible(additionalSettings);
			}
			else if (additionalSettings.Visibility == Visibility.Visible)
			{
				MakeVisible(importFolders);
			}
		}

		private void MakeVisible(FrameworkElement fe)
		{
			Thickness to = default(Thickness);
			configAdapterSetting.Visibility = Visibility.Visible;		//?? как узнать мод
			
			//ConfigAdapterSettingViewModel configsetting = Utils.GetViewModelFromRegion<ConfigAdapterSettingViewModel>(Common.RegionNames.ConfigAdapterSettingViewModel, this._regionManager);
			//configsetting.ConfigXDocument = null; 
			if (fe == importFolders)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Visible;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;
				to = new Thickness(-10, 45, 0, 0);
			}
			else if (fe == exportPdaSettings)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Visible;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 89, 0, 0);
			}
			else if (fe == exportErpSettings)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Visible;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 132, 0, 0);
			}
			else if (fe == updateFolders)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Visible;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 177, 0, 0);
			}

			else if (fe == autoGenerateResultSettings)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Visible;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 221, 0, 0);
			}
			else if (fe == dynamicColumns)
			{
				additionalSettings.Visibility = Visibility.Collapsed;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Visible;

				to = new Thickness(-10, 265, 0, 0);
			}
			else if (fe == additionalSettings)
			{
				additionalSettings.Visibility = Visibility.Visible;
				importFolders.Visibility = Visibility.Collapsed;
				exportPdaSettings.Visibility = Visibility.Collapsed;
				exportErpSettings.Visibility = Visibility.Collapsed;
				updateFolders.Visibility = Visibility.Collapsed;
				autoGenerateResultSettings.Visibility = Visibility.Collapsed;
				dynamicColumns.Visibility = Visibility.Collapsed;

				to = new Thickness(-10, 299, 0, 0);
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
