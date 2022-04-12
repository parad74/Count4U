using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class BranchFilterData : CommonFilterData, IFilterData
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CodeLocal { get; set; }
        public string CodeERP { get; set; }
        public string ContactPerson { get; set; }

        public string CustomerCode { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchBranchFieldView; } }

		public BranchFilterData()
		{
			this.Name = "";
			this.Phone = "";
			this.Address = "";
			this.CodeLocal = "";
			this.CodeERP = "";
			this.ContactPerson = "";
			this.CustomerCode = "";
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

            if (!String.IsNullOrWhiteSpace(CodeLocal))
                return true;

            if (!String.IsNullOrWhiteSpace(CodeERP))
                return true;

            if (!String.IsNullOrWhiteSpace(ContactPerson))
                return true;

            if (!String.IsNullOrWhiteSpace(CustomerCode))
                return true;

            return false;
        }        

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrEmpty(Name))
            {
                selectParams.FilterParams.Add("Name",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Name
                                        });
            }
            if (!String.IsNullOrEmpty(Address))
            {

                selectParams.FilterParams.Add("Address",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Address
                                        });
            }
            if (!String.IsNullOrEmpty(Phone))
            {

                selectParams.FilterParams.Add("Phone",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = Phone
                                        });
            }
            if (!String.IsNullOrEmpty(CodeLocal))
            {

                selectParams.FilterParams.Add("BranchCodeLocal",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = CodeLocal
                                        });
            }
            if (!String.IsNullOrEmpty(CodeERP))
            {

                selectParams.FilterParams.Add("BranchCodeERP",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = CodeERP
                                        });
            }

            if (!String.IsNullOrEmpty(ContactPerson))
            {

                selectParams.FilterParams.Add("ContactPerson",
                                        new FilterParam()
                                        {
                                            Operator = FilterOperator.Contains,
                                            Value = ContactPerson
                                        });
            }

            if (!String.IsNullOrEmpty(CustomerCode) && CustomerCode != Common.Constants.ComboValues.AllValue.All)
            {
                selectParams.FilterParams.Add("CustomerCode",
                              new FilterParam()
                              {
                                  Operator = FilterOperator.Equal,
                                  Value = CustomerCode
                              });
            }
        }
    }
}