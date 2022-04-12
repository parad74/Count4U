namespace Count4U.Common.ViewModel.Filter.Data
{
    public interface IFilterData
    {
        string DisplayName { get; set;  }
        bool IsAnyField();
        string FieldViewName { get; }
    }
}