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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Common.Extensions;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Regions;


namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for InventorChangeStatusView.xaml
    /// </summary>
	public partial class AutoGenerateResultSettingsView : UserControl, INavigationAware
    {
		public AutoGenerateResultSettingsView(AutoGenerateResultSettingsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

          // TO DO перенести в общий Path  Interaction.GetBehaviors(txtZipPath).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
        	this.Unloaded += InventorChangeStatusView_Unloaded;
			this.IsVisibleChanged += AutoGenerateResultSettingsView_IsVisibleChanged;
        }

		void AutoGenerateResultSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AutoGenerateResultSettingsViewModel vm = this.DataContext as AutoGenerateResultSettingsViewModel;
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

		private void InventorChangeStatusView_Unloaded(object sender, RoutedEventArgs e)
		{
			//AutoGenerateResultSettingsViewModel viewModel = this.DataContext as AutoGenerateResultSettingsViewModel;
			//Task.Factory.StartNew(viewModel.ClearIturAnalysis, "").LogTaskFactoryExceptions("AutoGenerateResultSettingsViewModel_Unloaded");
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{

		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{

		}
    }
}
