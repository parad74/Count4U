using System;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class UnitPlanItemViewModel : NotificationObject
    {
        private readonly UnitPlan _unitPlan;

        private string _code;
        private string _type;
        private string _x;
        private string _y;
        private string _width;
        private string _height;
        private string _angle;
        private string _name;

        public UnitPlanItemViewModel(UnitPlan unitPlan)
        {
            _unitPlan = unitPlan;

            _code = unitPlan.UnitPlanCode;
            _type = unitPlan.ObjectCode;
            _x = Round(unitPlan.StartX).ToString();
            _y = Round(unitPlan.StartY).ToString();
            _width = Round(unitPlan.Width).ToString();
            _height = Round(unitPlan.Height).ToString();
            _angle = Round(unitPlan.Rotate, 0).ToString();
            _name = unitPlan.Name;
        }

        public UnitPlan UnitPlan
        {
            get { return _unitPlan; }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
            }
        }

        public string X
        {
            get { return _x; }
            set { _x = value; }
        }

        public string Y
        {
            get { return _y; }
            set
            {
                _y = value;
                RaisePropertyChanged(() => Y);
            }
        }

        public string Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged(() => Width);
            }
        }

        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged(() => Height);
            }
        }

        public string Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                RaisePropertyChanged(() => Angle);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private double Round(double d, int n = 2)
        {
            return Math.Round(d, n);
        }
    }
}