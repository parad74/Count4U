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
using Count4U.Planogram.ViewModel;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanLocationAssignView.xaml
    /// </summary>
    public partial class PlanPictureAssignView : UserControl
    {
        public PlanPictureAssignView(PlanPictureAssignViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
