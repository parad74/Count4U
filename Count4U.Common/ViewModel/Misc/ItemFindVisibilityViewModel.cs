namespace Count4U.Common.ViewModel.Misc
{
    public class ItemFindVisibilityViewModel : ItemFindViewModel
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged(()=>IsVisible);
            }
        }
    }
}