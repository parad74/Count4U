using System;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class SectionFilterData : CommonFilterData, IFilterData
    {
        public string Code { get; set; }
        public string Name { get; set; }
		public string Tag { get; set; }

		public SectionFilterData()
		{
			this.Code = "";
			this.Tag = "";
			this.Name = "";
		}

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchSectionFieldView; } }

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

			return !String.IsNullOrWhiteSpace(Name) || !String.IsNullOrWhiteSpace(Code) || !String.IsNullOrWhiteSpace(Tag);
        }

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrWhiteSpace(this.Code))
            {
                selectParams.FilterParams.Add("SectionCode", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Code });
            }

            if (String.IsNullOrWhiteSpace(this.Name) == false)
            {
                selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Name });
            }

			if (String.IsNullOrWhiteSpace(this.Tag) == false)
			{
				selectParams.FilterParams.Add("Tag", new FilterParam() { Operator = FilterOperator.Contains, Value = this.Tag });
			}
        }
    }
}