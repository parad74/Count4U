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
    public partial class TagSelectView : UserControl
    {
		public TagSelectView(TagSelectViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			this.Loaded += TagSelectView_Loaded;
			this.Unloaded += TagSelectView_Unloaded;
			
        }

		void TagSelectView_Loaded(object sender, RoutedEventArgs e)
        {
			txtTag.Focus();

           // System.Windows.Interactivity.Interaction.GetBehaviors(txtNumbers).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			System.Windows.Interactivity.Interaction.GetBehaviors(txtTag).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
			TagSelectViewModel viewModel = this.DataContext as TagSelectViewModel;
			viewModel.SetDefaultSelectedObjectType();
        }

		private void TagSelectView_Unloaded(object sender, RoutedEventArgs e)
		{
			TagSelectViewModel  viewModel = this.DataContext as TagSelectViewModel;
			Task.Factory.StartNew(viewModel.ClearIturAnalysis, "").LogTaskFactoryExceptions("TagSelectView_Unloaded");
		}
	

	
    }
}
