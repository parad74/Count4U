using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack.Unpack
{
    public class UnpackOverwriteItem : UnpackBaseItem
    {
        private bool _isOverwrite;

        public bool IsOverwrite
        {
            get { return _isOverwrite; }
            set
            {
                _isOverwrite = value;
                RaisePropertyChanged(() => IsOverwrite);
            }
        }
    }
}