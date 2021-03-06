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

namespace Count4U.Modules.Audit.Views.Itur
{
    /// <summary>
    /// Interaction logic for IturSelectView.xaml
    /// </summary>
    public partial class IturDeleteView : UserControl
    {
		public IturDeleteView(IturDeleteViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;            

            this.Loaded += IturSelectView_Loaded;
			this.Unloaded += IturSelectView_Unloaded;
        }

        void IturSelectView_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumberPrefix.Focus();

            System.Windows.Interactivity.Interaction.GetBehaviors(txtNumbers).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
            System.Windows.Interactivity.Interaction.GetBehaviors(txtNumberPrefix).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
        }

		private void IturSelectView_Unloaded(object sender, RoutedEventArgs e)
		{
			//IturSelectViewModel viewModel = this.DataContext as IturSelectViewModel;
			//Task.Factory.StartNew(viewModel.ClearIturAnalysis, "").LogTaskFactoryExceptions("IturSelectView_Unloaded");
		}
    }
}
