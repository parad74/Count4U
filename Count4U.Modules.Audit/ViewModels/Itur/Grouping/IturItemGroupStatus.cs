namespace Count4U.Modules.Audit.ViewModels
{
    public class IturItemGroupStatus
    {
        private IturDashboardItemViewModel _itemViewModel;

        public IturItemGroupStatus(IturDashboardItemViewModel itemViewModel)
        {
            this._itemViewModel = itemViewModel;
        }

        public string StatusCode
        {
            get { return this._itemViewModel.StatusCode; }
        }

        public string Value
        {
            get { return this._itemViewModel.StatusName; }
        }

        public bool IsVisible
        {
            get { return true; }
        }

        public override bool Equals(object obj)
        {
            IturItemGroupStatus gr = obj as IturItemGroupStatus;
            return this.StatusCode.Equals(gr.StatusCode);
        }

		public override int GetHashCode()
		{
			return (this.StatusCode != null ? this.StatusCode.GetHashCode() : 0);
		}

        public string Color
        {
            get { return this._itemViewModel.StatusColor; }
        }

        public int Total { get { return this._itemViewModel.TotalItursWithSuchStatus; }}
    }
}