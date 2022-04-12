using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings.Items
{
    public class ReportRepositoryItemViewModel : NotificationObject
    {
        private string _text;
        private IturAnalyzesRepositoryEnum _enum;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(()=>Text);
            }
        }

        public IturAnalyzesRepositoryEnum Enum
        {
            get { return _enum; }
            set { _enum = value; }
        }
    }
}