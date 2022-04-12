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
using Count4U.Model.Count4U;
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanWall.xaml
    /// </summary>
    public partial class PlanLocation : PlanObject
    {
        private Location _location;
        private String _planName;

        public PlanLocation()
        {
            InitializeComponent();
        }

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Location; } }

        public string LocationCode { get; set; }

        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public void SetBackgroundColor(SolidColorBrush brush)
        {
            SolidColorBrush borderBrush = null;

            if (brush == null)
            {
                brush = new SolidColorBrush() {Color = Colors.Transparent};
                borderBrush = new SolidColorBrush() { Color = Colors.Gray };
            }
            else
            {
                borderBrush = new SolidColorBrush() { Color = Colors.Transparent };
            }
            this.Background = brush;
            this.border.BorderBrush = borderBrush;
        }

        public override string PlanName
        {
            get
            {
                if (_location != null)
                {
                    return _location.Name;
                }
                return _planName;
            }
            set
            {
                _planName = value;
            }
        }
    }
}
