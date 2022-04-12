using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
    public class PriceTypeItemViewModel : NotificationObject
    {
        private string _title;
        private PriceCodeEnum _en;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(()=>Title);
            }
        }

        public PriceCodeEnum En
        {
            get { return _en; }
            set
            {
                _en = value;
                RaisePropertyChanged(() => En);
            }
        }
    }
}