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
using Count4U.Common.Behaviours;
using Count4U.Planogram.ViewModel;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanPropertiesView.xaml
    /// </summary>
    public partial class PlanPropertiesView : UserControl
    {
        public PlanPropertiesView(PlanPropertiesViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            System.Windows.Interactivity.Interaction.GetBehaviors(txtSelectedX).Add(new TextBoxPropertyBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtSelectedY).Add(new TextBoxPropertyBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtSelectedWidth).Add(new TextBoxPropertyBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtSelectedHeight).Add(new TextBoxPropertyBehavior());
            System.Windows.Interactivity.Interaction.GetBehaviors(txtSelectedAngle).Add(new TextBoxPropertyBehavior());
        }
    }
}
