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
    /// Interaction logic for PlanWall.xaml
    /// </summary>
    public partial class PlanWall : PlanObject
    {
        public PlanWall()
        {
            InitializeComponent();
        }

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Wall; } }        
    }
}
