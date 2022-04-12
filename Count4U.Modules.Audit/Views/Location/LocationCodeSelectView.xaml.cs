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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Modules.Audit.ViewModels;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.Views.Location
{
    /// <summary>
    /// Interaction logic for IturSelectView.xaml
    /// </summary>
    public partial class LocationCodeSelectView : UserControl
    {
		public LocationCodeSelectView(LocationCodeSelectViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			this.Loaded += LocationCodeSelectView_Loaded;
			this.Unloaded += LocationCodeSelectView_Unloaded;
        }

		void LocationCodeSelectView_Loaded(object sender, RoutedEventArgs e)
        {
			txtLocationCode.Focus();

           // System.Windows.Interactivity.Interaction.GetBehaviors(txtNumbers).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			System.Windows.Interactivity.Interaction.GetBehaviors(txtLocationCode).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			
        }

		private void LocationCodeSelectView_Unloaded(object sender, RoutedEventArgs e)
		{
			LocationCodeSelectViewModel viewModel = this.DataContext as LocationCodeSelectViewModel;
			Task.Factory.StartNew(viewModel.ClearIturAnalysis, "").LogTaskFactoryExceptions("LocationCodeSelectView_Unloaded");
		}
    }
}
