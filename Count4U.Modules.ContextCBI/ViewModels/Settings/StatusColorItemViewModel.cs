using System.Windows.Media;
using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings
{
    public class StatusColorItemViewModel : NotificationObject
    {
        private string _status;
        private Color _color;

        public string Status
        {
            get { return this._status; }
            set
            {
                this._status = value;
                RaisePropertyChanged(()=>Status);
            }
        }

        public Color Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
				this.RaisePropertyChanged(() => this.Color);
            }
        }

        public IturStatusEnum EnumStatus { get; set; }
    }
}