using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Modules.Audit.ViewModels
{
    public class TagItemViewModel : NotificationObject
    {
        private readonly string _tag;
        private bool _isChecked;

		public TagItemViewModel(string tag)
        {
			this._tag = tag;
			this._isChecked = false; //дабавила
        }

        public string Tag
        {
			get { return this._tag; }
        }

		public string Code
		{
			get { return this._tag; }
		}

        public string Name
        {
			get { return this._tag; }
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

	        public bool IsAddVisible { get; set; }
    }
}