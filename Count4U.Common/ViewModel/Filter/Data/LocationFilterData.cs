using System;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class LocationFilterData : CommonFilterData, IFilterData
    {
        public string Code { get; set; }
        public string Name { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchLocationFieldView; } }

		public LocationFilterData()
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

            if (String.IsNullOrWhiteSpace(this.Code) == false)
            {
                selectParams.FilterParams.Add("Code", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Code });
            }
            if (String.IsNullOrWhiteSpace(this.Name) == false)
            {
                selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Name });
            }
        }
    }
}