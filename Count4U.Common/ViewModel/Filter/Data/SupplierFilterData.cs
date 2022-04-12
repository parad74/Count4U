using System;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class SupplierFilterData : CommonFilterData, IFilterData
    {
        public string Code { get; set; }
        public string Name { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchSupplierFieldView; } }

		public SupplierFilterData()
		{
			this.Code = "";
			this.Name = "";
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            return !String.IsNullOrWhiteSpace(Name) || !String.IsNullOrWhiteSpace(Code);
        }

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrWhiteSpace(this.Code))
            {
                selectParams.FilterParams.Add("SupplierCode", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Code });
            }

            if (!String.IsNullOrWhiteSpace(this.Name))
            {
                selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Name });
            }
        }
    }
}