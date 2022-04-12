using System;
using System.Xml.Serialization;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public abstract class CommonFilterData
    {        
        public string DisplayName { get; set; }

        public enSortDirection SortDirection { get; set; }
        public string SortField { get; set; }
        public bool SortIsAbsolute { get; set; }

        protected bool IsFilterBySort()
        {
            return !String.IsNullOrEmpty(SortField);
        }

        protected void ApplySortToSelectParams(SelectParams selectParams)
        {
            if (String.IsNullOrWhiteSpace(SortField))
                return;

            string calculatedProperty = SortIsAbsolute ? String.Format("{0}*{0}", SortField) : SortField;
            string sortString = String.Format("{0} {1}", calculatedProperty, SortDirection == enSortDirection.DESC ? "DESC" : "ASC");
            selectParams.SortParams = sortString;
        }
    }
}