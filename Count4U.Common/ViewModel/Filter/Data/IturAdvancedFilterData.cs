using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Count4U.Common.Constants;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class IturAdvancedFilterData : CommonFilterData, IFilterData
    {
        public string Text { get; set; }
        public string Field { get; set; }

        public bool IsLocation { get; set; }
        public bool IsStatus { get; set; }
		public bool IsTag { get; set; }
        public bool IsInventProduct { get; set; }

        //public List<Location> Locations { get; set; }
        public List<string> Locations { get; set; } //code
        //public List<StatusIturGroup> Statuses { get; set; }
        public List<int> Statuses { get; set; } //bit
		public List<string> Tags { get; set; } 

        public string InventProductMakat { get; set; }
        public string InventProductBarcode { get; set; }
        public string InventProductName { get; set; }

        public bool IsLocationExpanded { get; set; }
        public bool IsStatusExpanded { get; set; }
		public bool IsTagExpanded { get; set; }
		public bool IsFromFilter 
		{
			get;
			set; 
		}
		
        public bool IsInventProductExpanded { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchIturAdvancedFieldView; } }

		public IturAdvancedFilterData()
		{
			this.Text = "";
			this.Field = "";
			this.InventProductMakat = "";
			this.InventProductBarcode = "";
			this.InventProductName = "";
			this.IsFromFilter = false;
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (!String.IsNullOrWhiteSpace(Text))
                return true;

            if (IsInventProduct)
            {
                if (!String.IsNullOrWhiteSpace(InventProductMakat))
                    return true;

                if (!String.IsNullOrWhiteSpace(InventProductBarcode))
                    return true;

                if (!String.IsNullOrWhiteSpace(InventProductName))
                    return true;
            }

            if (IsLocation)
                return true;

			if (IsTag)
				return true;

            if (IsStatus)
                return true;

            return false;
        }        

        public void ApplyToSelectParams(SelectParams selectParams, IInventProductRepository inventProductRepository, string dbPath)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrEmpty(Text) && Field != null)
            {
                if (Field == ComboValues.FindItur.FilterIturNumber)
                {
                    int r;
                    if (Int32.TryParse(Text, out r))
                    {
                        selectParams.FilterParams.Add("Number", new FilterParam()
                        {
                            Operator = FilterOperator.Equal,
                            Value = r
                        });
                    }
                }

                if (Field == ComboValues.FindItur.FilterIturERP)
                {

                    selectParams.FilterParams.Add("ERPIturCode", new FilterParam()
                    {
                        Operator = FilterOperator.Contains,
                        Value = Text
                    });
                }
            }

            if (IsLocation && Locations.Any())
            {
                selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
                {
                    Values = Locations
                });
            }

			if (IsTag && Tags.Any())
			{
				selectParams.FilterStringListParams.Add("Tag", new FilterStringListParam()
				{
					Values = Tags
				});
			}

            if (IsStatus && Statuses.Any())
            {
                List<int> statuses = Statuses.ToList();

                //temporary hack
                if (statuses.Any(r => r == 6))
                    statuses.Add(999);

                selectParams.FilterIntListParams.Add("StatusIturGroupBit", new FilterIntListParam()
                {
                    Values = statuses
                });
            }

            if (IsInventProduct &&
                (!String.IsNullOrEmpty(InventProductBarcode) ||
                 !String.IsNullOrEmpty(InventProductMakat) ||
                 !String.IsNullOrEmpty(InventProductName)))
            {
                SelectParams ipSelectParams = new SelectParams
                {
                    IsEnablePaging = false,
                };

                if (!String.IsNullOrEmpty(InventProductBarcode))
                {
                    ipSelectParams.FilterParams.Add("Barcode", new FilterParam() { Operator = FilterOperator.MultipleString, Value = InventProductBarcode });
                }

                if (!String.IsNullOrEmpty(InventProductMakat))
                {
                    ipSelectParams.FilterParams.Add("Makat", new FilterParam() { Operator = FilterOperator.MultipleString, Value = InventProductMakat });
                }

                if (!String.IsNullOrEmpty(InventProductName))
                {
                    ipSelectParams.FilterParams.Add("ProductName", new FilterParam() { Operator = FilterOperator.Contains, Value = InventProductName });
                }

                InventProducts inventProducts = inventProductRepository.GetInventProducts(ipSelectParams, dbPath);

                if (inventProducts.Count == 0) //hack to prevent System.Linq.Dynamic failing with empty FilterStringListParams list
                {
                    selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam() { Values = new List<string>() { "NoOneIturCode" } });
                }
                else
                {
                    selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam() { Values = inventProducts.Select(r => r.IturCode).Take(2000).ToList() });
                }
            }
        }
    }
}