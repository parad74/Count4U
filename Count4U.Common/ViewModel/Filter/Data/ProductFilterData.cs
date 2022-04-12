using System;
using System.Linq;
using System.Xml.Serialization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U;
using System.Collections.Generic;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class ProductFilterData : CommonFilterData, IFilterData
    {
        public string Makat { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public double? PriceSale { get; set; }
        public double? PriceBuy { get; set; }

		public ProductFilterData()
		{
			this.Makat = "";
			this.Barcode = "";
			this.ProductName = "";
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (!String.IsNullOrWhiteSpace(Makat))
                return true;

            if (!String.IsNullOrWhiteSpace(Barcode))
                return true;

            if (!String.IsNullOrWhiteSpace(ProductName))
                return true;

            if (PriceSale.HasValue)
                return true;

            if (PriceBuy.HasValue)
                return true;

            return false;
        }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchProductFieldView; } }

        public void ApplyToSelectParams(SelectParams selectParams, IProductRepository productRepository, string dbPath)
        {
            base.ApplySortToSelectParams(selectParams);

            selectParams.FilterParams.Add("TypeCode", new FilterParam() { Operator = FilterOperator.Equal, Value = TypeMakatEnum.M.ToString() });

            if (!String.IsNullOrWhiteSpace(Makat))
            {
                selectParams.FilterParams.Add("Makat", new FilterParam() { Operator = FilterOperator.Contains, Value = Makat });
            }

            if (!String.IsNullOrWhiteSpace(ProductName))
            {
                selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = ProductName });
            }

            if (PriceSale.HasValue)
            {
                selectParams.FilterParams.Add("PriceSale", new FilterParam() { Operator = FilterOperator.Equal, Value = PriceSale.Value });
            }

            if (PriceBuy.HasValue)
            {
                selectParams.FilterParams.Add("PriceBuy", new FilterParam() { Operator = FilterOperator.Equal, Value = PriceBuy.Value });
            }


            if (!String.IsNullOrWhiteSpace(Barcode))
            {
                SelectParams detailsSp = new SelectParams();
                detailsSp.FilterParams.Add("Makat", new FilterParam() { Operator = FilterOperator.MultipleString, Value = Barcode });
                detailsSp.FilterParams.Add("TypeCode", new FilterParam() { Operator = FilterOperator.Equal, Value = TypeMakatEnum.B.ToString() });

                Products products = productRepository.GetProducts(detailsSp, dbPath);

                if (products.Count == 0) //hack to prevent System.Linq.Dynamic failing with empty FilterStringListParams list
                    selectParams.FilterStringListParams.Add("Makat", new FilterStringListParam() { Values = new List<string>() { "NoOneMakat" } });
                else
                    selectParams.FilterStringListParams.Add("Makat", new FilterStringListParam()
                    {
                        Values = products.Select(r => r.ParentMakat).Take(2000).ToList()
                    });

            }
        }
    }
}