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
using Count4U.Planogram.ViewModel;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanIturAddView.xaml
    /// </summary>
    public partial class PlanIturAddView : UserControl
    {
        public PlanIturAddView(PlanIturAddViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            Interaction.GetBehaviors(txtPrefix).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
            Interaction.GetBehaviors(txtNumbers).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });            

            this.Loaded += PlanIturAddView_Loaded;
        }

        void PlanIturAddView_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumbers.Focus();
        }
    }
}
