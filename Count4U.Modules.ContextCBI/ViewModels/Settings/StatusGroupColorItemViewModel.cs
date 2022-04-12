using System.Windows.Media;
using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings
{
    public class StatusGroupColorItemViewModel : NotificationObject
    {
        private string _statusGroup;
        private Color _color;

        public string StatusGroup
        {
            get { return this._statusGroup; }
            set
            {
                this._statusGroup = value;
                RaisePropertyChanged(() => StatusGroup);
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

        public IturStatusGroupEnum EnumStatusGroup { get; set; }
    }
}