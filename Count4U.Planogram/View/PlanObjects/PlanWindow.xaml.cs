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
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanWindow.xaml
    /// </summary>
    public partial class PlanWindow : PlanObject
    {
        public PlanWindow()
        {
            InitializeComponent();
        }

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Window; } }
    }
}
