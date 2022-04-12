using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskSelectItemViewModel : NotificationObject
    {
        private string _result;
        private string _maskTemplate;
        private string _maskEditTemplate;
        private bool _isChecked;

        public string Result
        {
            get
            {                
                return this._result;
            }
            set
            {
                this._result = value;
                RaisePropertyChanged(()=>Result);
            }
        }

        public string MaskTemplate
        {
            get { return this._maskTemplate; }
            set
            {
                this._maskTemplate = value;
                RaisePropertyChanged(() => MaskTemplate);
            }
        }

        public string MaskEditTemplate
        {
            get { return this._maskEditTemplate; }
            set
            {
                this._maskEditTemplate = value;
                RaisePropertyChanged(() => MaskEditTemplate);
            }
        }

        public bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
                RaisePropertyChanged(()=>IsChecked);
            }
        }

        public bool IsEdit { get; set; }
    }
}