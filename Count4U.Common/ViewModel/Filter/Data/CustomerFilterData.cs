using System;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class CustomerFilterData : CommonFilterData, IFilterData
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string ContactPerson { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchCustomerFieldView; } }

		public CustomerFilterData()
		{
			this.Name = "";
			this.Phone = "";
			this.Address = "";
			this.Code = "";
			this.ContactPerson = "";
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (!String.IsNullOrWhiteSpace(Name))
                return true;

            if (!String.IsNullOrWhiteSpace(Phone))
                return true;

            if (!String.IsNullOrWhiteSpace(Address))
                return true;

            if (!String.IsNullOrWhiteSpace(Code))
                return true;

            if (!String.IsNullOrWhiteSpace(ContactPerson))
                return true;

            return false;
        }        

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrWhiteSpace(Name))
            {
                selectParams.FilterParams.Add("Name",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Name
                                        });
            }

            if (!String.IsNullOrWhiteSpace(Address))
            {
                selectParams.FilterParams.Add("Address",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Address
                                        });
            }

            if (!String.IsNullOrWhiteSpace(Phone))
            {
                selectParams.FilterParams.Add("Phone",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Phone
                                        });
            }

            if (!String.IsNullOrWhiteSpace(Code))
            {
                selectParams.FilterParams.Add("Code",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Code
                                        });
            }

            if (!String.IsNullOrWhiteSpace(ContactPerson))
            {
                selectParams.FilterParams.Add("ContactPerson",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = ContactPerson
                                        });
            }
        }
    }
}