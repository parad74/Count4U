namespace Count4U.Modules.Audit.ViewModels
{
    public class IturItemGroupLocation
    {
        private readonly IturDashboardItemViewModel _itemViewModel;

        public IturItemGroupLocation(IturDashboardItemViewModel itemViewModel)
        {
            this._itemViewModel = itemViewModel;
        }

        public string LocationCode
        {
            get { return this._itemViewModel.LocationCode; }
        }

        public string Value
        {
            get { return this._itemViewModel.LocationName; }
        }

        public bool IsVisible
        {
            get { return true; }
        }

        public override bool Equals(object obj)
        {
            IturItemGroupLocation gr = obj as IturItemGroupLocation;

            return LocationCode.Equals(gr.LocationCode);
        }

        public string Color
        {
            get { return this._itemViewModel.LocationColor; }
        }

        public int Total { get { return this._itemViewModel.TotalItursWithSuchLocation; } }

        public int EmptyIturs { get { return this._itemViewModel.EmptyItursWithSuchLocation; } }

        public int CountedIturs { get { return this._itemViewModel.CountedItursWithSuchLocation; } }

        public int DisabledIturs { get { return this._itemViewModel.DisabledItursWithSuchLocation; } }

        public string DoneIturs { get { return this._itemViewModel.DoneItursWithSuchLocation.ToString() + " %" ; } }
        public bool Equals(IturItemGroupLocation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.LocationCode, LocationCode);
        }

        public override int GetHashCode()
        {
            return (_itemViewModel != null ? _itemViewModel.GetHashCode() : 0);
        }
    }
}