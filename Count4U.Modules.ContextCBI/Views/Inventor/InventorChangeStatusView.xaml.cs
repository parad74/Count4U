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

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorChangeStatusView.xaml
    /// </summary>
    public partial class InventorChangeStatusView : UserControl
    {
        public InventorChangeStatusView(InventorChangeStatusViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            Interaction.GetBehaviors(txtZipPath).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			//Interaction.GetBehaviors(txtFtpPath).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			
            //dtpInventorDate.FormatString = UtilsConvert.DateFormatLong();
			this.Unloaded += InventorChangeStatusView_Unloaded;
        }

		private void InventorChangeStatusView_Unloaded(object sender, RoutedEventArgs e)
		{
			InventorChangeStatusViewModel viewModel = this.DataContext as InventorChangeStatusViewModel;
			Task.Factory.StartNew(viewModel.ClearIturAnalysis, "").LogTaskFactoryExceptions("InventorChangeStatusView_Unloaded");
		}
    }
}
