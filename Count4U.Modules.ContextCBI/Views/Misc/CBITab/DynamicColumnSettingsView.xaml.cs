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
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for DynamicColumnSettingsView.xaml
    /// </summary>
    public partial class DynamicColumnSettingsView : UserControl
    {
        public DynamicColumnSettingsView(DynamicColumnSettingsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			this.IsVisibleChanged += DynamicColumnSettingsView_IsVisibleChanged;
        }

		void DynamicColumnSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			DynamicColumnSettingsViewModel vm = this.DataContext as DynamicColumnSettingsViewModel;
			//if (vm.IsShowConfig == true)
			//{
			//	if ((bool)e.NewValue == true && (bool)e.OldValue == false) //открываем закладку
			//	{
			//		vm.GotNewFofusConfig();
			//	}
			//	else if ((bool)e.NewValue == false && (bool)e.OldValue == true) //закрываем закладку
			//	{
			//		vm.LostFocusConfig();
			//	}
			//}
		}
    }
}
