using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels
{
    public class StatusGroupItemViewModel : NotificationObject
    {
        private readonly StatusIturGroup _statusGroup;
        private bool _isChecked;

        public StatusGroupItemViewModel(StatusIturGroup statusGroup)
        {
            this._statusGroup = statusGroup;
			this._isChecked = false;
        }

        public string Name
        {
            get { return this._statusGroup.Name; }
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

        public string BackgroundColor { get; set; }

        public StatusIturGroup StatusIturGroup
        {
            get { return this._statusGroup; }
        }
    }
}