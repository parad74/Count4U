using System;
using System.Windows.Media.Animation;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class PlanLocationItemViewModel : NotificationObject
    {
        private readonly Location _location;

        private string _code;
        private string _name;
        private string _color;
        private bool _isSelected;

        public event EventHandler IsSelectedChanged = delegate { };

        protected virtual void OnIsSelectedChanged()
        {
            EventHandler handler = IsSelectedChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public PlanLocationItemViewModel(Location location)
        {
            _location = location;

            Code = location.Code;
            _name = location.Name;
            _color = location.BackgroundColor;

            if (String.IsNullOrWhiteSpace(_color))
            {
                _color = "255,255,255";
            }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);

                OnIsSelectedChanged();
            }
        }
      
        public void SetIsSelectedSilent(bool isSelected)
        {
            _isSelected = isSelected;
            RaisePropertyChanged(() => IsSelected);
        }
    }
}