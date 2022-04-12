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
    /// Interaction logic for PlanSizeChangeView.xaml
    /// </summary>
    public partial class PlanSizeChangeView : UserControl
    {
        public PlanSizeChangeView(PlanSizeChangeViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += PlanSizeChangeView_Loaded;

            Interaction.GetBehaviors(txtWidth).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
            Interaction.GetBehaviors(txtHeight).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });

            txtWidth.GotFocus += txtWidth_GotFocus;
            txtHeight.GotFocus += txtHeight_GotFocus;
        }

        void txtHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHeight.SelectAll();
        }

        void txtWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            txtWidth.SelectAll();
        }

        void PlanSizeChangeView_Loaded(object sender, RoutedEventArgs e)
        {
            txtWidth.Focus();
        }
    }
}
