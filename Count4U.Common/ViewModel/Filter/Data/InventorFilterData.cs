using System;
using System.Xml.Serialization;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class InventorFilterData : CommonFilterData, IFilterData
    {
        public string Code { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Description { get; set; }

        public string CustomerCode { get; set; }
        public string BranchCode { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchInventorFieldView; } }

		public InventorFilterData()
		{
			this.Code = "";
			this.Description = "";
			this.CustomerCode = "";
			this.BranchCode = "";
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (From != null)
                return true;

            if (To != null)
                return true;

            if (!String.IsNullOrWhiteSpace(Code))
                return true;

            if (!String.IsNullOrWhiteSpace(Description))
                return true;

            if (!String.IsNullOrWhiteSpace(CustomerCode))
                return true;

            if (!String.IsNullOrWhiteSpace(BranchCode))
                return true;

            return false;
        }        

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrEmpty(Description))
            {
                selectParams.FilterParams.Add("Description",
                    new FilterParam()
                    {
                        Operator = FilterOperator.Contains,
                        Value = Description
                    }
                );
            }

            DateTime? from = From;
            DateTime? to = To;

            if (from != null && to != null)
            {
                selectParams.FilterDateTimeParams.Add("InventorDate",
                                                      new FilterParam()
                                                      {
                                                          Operator = FilterOperator.DateTimeBetween,
                                                          Value = from,
                                                          Value1 = to
                                                      }
                    );
            }

            if (from != null && to == null)
            {
                selectParams.FilterDateTimeParams.Add("InventorDate",
                                                      new FilterParam()
                                                      {
                                                          Operator = FilterOperator.DateTimeGreaterOrEqual,
                                                          Value = from
                                                      }
                    );
            }

            if (from == null && to != null)
            {
                selectParams.FilterDateTimeParams.Add("InventorDate",
                                                      new FilterParam()
                                                      {
                                                          Operator = FilterOperator.DateTimeLessOrEqual,
                                                          Value = to
                                                      }
                    );
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

            if (!String.IsNullOrEmpty(CustomerCode) && CustomerCode != Common.Constants.ComboValues.AllValue.All)
            {
                selectParams.FilterParams.Add("CustomerCode",
                        new FilterParam()
                        {
                            Operator = FilterOperator.Equal,
                            Value = CustomerCode
                        });
            }


            if (!String.IsNullOrEmpty(BranchCode) && BranchCode != Common.Constants.ComboValues.AllValue.All)
            {
                selectParams.FilterParams.Add("BranchCode",
                        new FilterParam()
                        {
                            Operator = FilterOperator.Equal,
                            Value = BranchCode
                        });
            }
        }
    }
}