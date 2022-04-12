using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Modules.Audit.ViewModels
{
    public class LocationItemViewModel : NotificationObject
    {
        private readonly Location _location;
        private bool _isChecked;

        public LocationItemViewModel(Location location)
        {
            this._location = location;
			this._isChecked = false; //дабавила
        }

        public string Code
        {
            get { return _location.Code; }
        }

        public string Name
        {
            get { return _location.Name; }
        }

        public bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
                this.RaisePropertyChanged(() => this.IsChecked);
            }
        }

		public string Tag
		{
			get { return _location.Tag; }
		}

        public Location Location
        {
            get { return this._location; }
        }

        public string LocationColor
        {
            get
            {
                return String.IsNullOrWhiteSpace(this._location.BackgroundColor) == null ? Common.Constants.DefaultColors.EmptyLocationColor() : this._location.BackgroundColor;
                                                                                       
            }
        }

        public bool IsAddVisible { get; set; }
    }
}