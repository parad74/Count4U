namespace Count4U.Modules.Audit.ViewModels
{
    public class IturItemGroupTag
    {
        private readonly IturDashboardItemViewModel _itemViewModel;

		public IturItemGroupTag(IturDashboardItemViewModel itemViewModel)
        {
            this._itemViewModel = itemViewModel;
        }

        public string Tag
        {
			get { return this._itemViewModel.Tag; }
        }

        public string Value
        {
			get { return this._itemViewModel.Tag; }
        }

        public bool IsVisible
        {
            get { return true; }
        }

        public override bool Equals(object obj)
        {
            IturItemGroupTag gr = obj as IturItemGroupTag;

            return Tag.Equals(gr.Tag);
        }

        public string Color
        {
            get { return ""; }
        }

        public int Total { get { return this._itemViewModel.TotalItursWithSuchTag; } }

		public bool Equals(IturItemGroupTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Tag, Tag);
        }

        public override int GetHashCode()
        {
            return (_itemViewModel != null ? _itemViewModel.GetHashCode() : 0);
        }
    }
}