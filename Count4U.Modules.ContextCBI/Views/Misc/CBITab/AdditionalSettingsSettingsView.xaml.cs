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
    public partial class AdditionalSettingsSettingsView : UserControl
    {
		public AdditionalSettingsSettingsView(AdditionalSettingsSettingsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			this.IsVisibleChanged += AdditionalSettingsSettingsView_IsVisibleChanged;
        }

		void AdditionalSettingsSettingsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AdditionalSettingsSettingsViewModel vm = this.DataContext as AdditionalSettingsSettingsViewModel;
			//vm.GotNewFofusConfig();
		}
    }
}
