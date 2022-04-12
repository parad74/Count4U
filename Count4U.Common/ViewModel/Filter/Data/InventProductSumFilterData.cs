using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class InventProductSumFilterData : CommonFilterData, IFilterData
    {
		public InventProductSumFilterData()
		{
			SectionItems = new List<string>();

			InventProductQuantityDifferenceEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductQuantityEditEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductValueBuyDifferenceEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductValueBuyEditEquality = Common.Constants.ComboValues.Equality.Equal;

			ProductQuantityOriginalERPEquality = Common.Constants.ComboValues.Equality.Equal;
			ProductQuantityDifferenceOriginalERPEquality = Common.Constants.ComboValues.Equality.Equal;
			ProductValueDifferenceOriginalERPEquality = Common.Constants.ComboValues.Equality.Equal;
			ProductPriceBuyEquality = Common.Constants.ComboValues.Equality.Equal;
			ProductPriceSaleEquality = Common.Constants.ComboValues.Equality.Equal;

			ProductQuantityDifferenceOriginalERPIsAbsolute = true;
			ProductQuantityOriginalERPIsAbsolute = true;
			ProductValueDifferenceOriginalERPIsAbsolute = true;
			ProductPriceBuyIsAbsolute = true;
			ProductPriceSaleIsAbsolute = true;

			IsProductExpanded = true;

			//-- String.Empty 
			this.InventProductQuantityDifference = "";
			this.InventProductQuantityDifferenceEquality = "";
			this.InventProductQuantityEdit = "";
			this.InventProductQuantityEditEquality = "";
			this.InventProductValueBuyDifference = "";
			this.InventProductValueBuyDifferenceEquality = "";
			this.InventProductValueBuyEdit = "";
			this.InventProductValueBuyEditEquality = "";
			this.ProductMakat = "";
			this.ProductQuantityOriginalERP = "";
			this.ProductQuantityOriginalERPEquality = "";
			this.ProductQuantityDifferenceOriginalERP = "";
			this.ProductQuantityDifferenceOriginalERPEquality = "";
			this.ProductValueDifferenceOriginalERP = "";
			this.ProductValueDifferenceOriginalERPEquality = "";
			this.ProductPriceBuy = "";
			this.ProductPriceBuyEquality = "";
			this.ProductPriceSale = "";
			this.ProductPriceSaleEquality = "";
			this.ProductName = "";
			this.SupplierCode = "";
			this.SupplierName = "";
		}

        public bool IsInventProductExpanded { get; set; }
        public bool IsFilterByInventProduct { get; set; }
        public string InventProductQuantityDifference { get; set; }
        public string InventProductQuantityDifferenceEquality { get; set; }
        public bool InventProductQuantityDifferenceIsAbsolute { get; set; }
        public string InventProductQuantityEdit { get; set; }
        public string InventProductQuantityEditEquality { get; set; }
        public bool InventProductQuantityEditIsAbsolute { get; set; }
        public string InventProductValueBuyDifference { get; set; }
        public string InventProductValueBuyDifferenceEquality { get; set; }
        public bool InventProductValueBuyDifferenceIsAbsolute { get; set; }
        public string InventProductValueBuyEdit { get; set; }
        public string InventProductValueBuyEditEquality { get; set; }
        public bool InventProductValueBuyEditIsAbsolute { get; set; }

        public bool IsProductExpanded { get; set; }
        public bool IsFilterByProduct { get; set; }
        public string ProductMakat { get; set; }
		public string ProductQuantityOriginalERP { get; set; }
		public string ProductQuantityOriginalERPEquality { get; set; }
		public bool ProductQuantityOriginalERPIsAbsolute { get; set; }

        public string ProductQuantityDifferenceOriginalERP { get; set; }
        public string ProductQuantityDifferenceOriginalERPEquality { get; set; }
        public bool ProductQuantityDifferenceOriginalERPIsAbsolute { get; set; }
        public string ProductValueDifferenceOriginalERP { get; set; }
        public string ProductValueDifferenceOriginalERPEquality { get; set; }
        public bool ProductValueDifferenceOriginalERPIsAbsolute { get; set; }
        public string ProductPriceBuy { get; set; }
        public string ProductPriceBuyEquality { get; set; }
        public bool ProductPriceBuyIsAbsolute { get; set; }
        public string ProductPriceSale { get; set; }
        public string ProductPriceSaleEquality { get; set; }
        public bool ProductPriceSaleIsAbsolute { get; set; }
        public string ProductName { get; set; }

        public bool IsSupplierExpanded { get; set; }
        public bool IsFilterBySupplier { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

		public bool IsFamilyExpanded { get; set; }
		public bool IsFilterByFamily { get; set; }
		public string FamilyCode { get; set; }
		public string FamilyName { get; set; }

        public bool IsSectionExpanded { get; set; }
        public bool IsFilterBySection { get; set; }
       // public List<Count4U.Model.Count4U.Section> SectionItems { get; set; }
        public List<string> SectionItems { get; set; } //code

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (IsFilterByInventProduct)
            {
                if (!String.IsNullOrWhiteSpace(InventProductQuantityDifference) ||
                   !String.IsNullOrWhiteSpace(InventProductQuantityEdit) ||
                   !String.IsNullOrWhiteSpace(InventProductValueBuyDifference) ||
                   !String.IsNullOrWhiteSpace(InventProductValueBuyEdit)
                   )
                    return true;
            }

            if (IsFilterByProduct)
            {
                if (!String.IsNullOrWhiteSpace(ProductMakat) ||
				  !String.IsNullOrWhiteSpace(ProductQuantityOriginalERP) ||
                  !String.IsNullOrWhiteSpace(ProductQuantityDifferenceOriginalERP) ||
                  !String.IsNullOrWhiteSpace(ProductValueDifferenceOriginalERP) ||
                  !String.IsNullOrWhiteSpace(ProductPriceBuy) ||
                  !String.IsNullOrWhiteSpace(ProductPriceSale) ||
                  !String.IsNullOrWhiteSpace(ProductName)
                  )
                    return true;
            }

            if (IsFilterBySupplier)
            {
                if (!String.IsNullOrWhiteSpace(SupplierName) || !String.IsNullOrWhiteSpace(SupplierCode))
                {
                    return true;
                }
            }

            if (IsFilterBySection == true)
            {
                return true;
            }

			if (IsFilterByFamily == true)
			{
				if (!String.IsNullOrWhiteSpace(FamilyName) || !String.IsNullOrWhiteSpace(FamilyCode))
				{
					return true;
				}
			}

            return false;
        }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchInventProductAdvancedFieldSumView; } }

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (IsFilterByInventProduct)
            {
                if (!String.IsNullOrWhiteSpace(InventProductQuantityDifference) && !String.IsNullOrWhiteSpace(InventProductQuantityDifferenceEquality))
                {
                    double quantityDifference;
                    if (Double.TryParse(InventProductQuantityDifference, out quantityDifference))
                    {
                        string field = "QuantityDifference";
                        if (InventProductQuantityDifferenceIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(InventProductQuantityDifferenceEquality))
                        {
                            field = "QuantityDifference*QuantityDifference";
                            quantityDifference *= quantityDifference;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(InventProductQuantityDifferenceEquality),
                            Value = quantityDifference
                        });
                    }
                }

                if (!String.IsNullOrWhiteSpace(InventProductQuantityEdit) && !String.IsNullOrWhiteSpace(InventProductQuantityEditEquality))
                {
                    double quantityEdit;
                    if (Double.TryParse(InventProductQuantityEdit, out quantityEdit))
                    {
                        string field = "QuantityEdit";
                        if (InventProductQuantityEditIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(InventProductQuantityEditEquality))
                        {
                            field = "QuantityEdit*QuantityEdit";
                            quantityEdit *= quantityEdit;
                        }
                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(InventProductQuantityEditEquality),
                            Value = quantityEdit
                        });
                    }
                }

                if (!String.IsNullOrWhiteSpace(InventProductValueBuyDifference) && !String.IsNullOrWhiteSpace(InventProductValueBuyDifferenceEquality))
                {
                    double valueBuyDifference;
                    if (Double.TryParse(InventProductValueBuyDifference, out valueBuyDifference))
                    {
                        string field = "ValueBuyDifference";
                        if (InventProductValueBuyDifferenceIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(InventProductValueBuyDifferenceEquality))
                        {
                            field = "ValueBuyDifference*ValueBuyDifference";
                            valueBuyDifference *= valueBuyDifference;
                        }
                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(InventProductValueBuyDifferenceEquality),
                            Value = valueBuyDifference
                        });
                    }
                }

                if (!String.IsNullOrWhiteSpace(InventProductValueBuyEdit) && !String.IsNullOrWhiteSpace(InventProductValueBuyEditEquality))
                {
                    double valueBuyEdit;
                    if (Double.TryParse(InventProductValueBuyEdit, out valueBuyEdit))
                    {
                        string field = "ValueBuyEdit";
                        if (InventProductValueBuyEditIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(InventProductValueBuyEditEquality))
                        {
                            field = "ValueBuyEdit*ValueBuyEdit";
                            valueBuyEdit *= valueBuyEdit;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(InventProductValueBuyEditEquality),
                            Value = valueBuyEdit
                        });
                    }
                }
            }

            if (IsFilterByProduct)
            {
                if (!String.IsNullOrWhiteSpace(ProductMakat))
                {
                    selectParams.FilterParams.Add("Makat", new FilterParam()
                    {
                        Value = ProductMakat,
                        Operator = FilterOperator.Contains
                    });
                }

                if (!String.IsNullOrWhiteSpace(ProductQuantityDifferenceOriginalERP) && !String.IsNullOrWhiteSpace(ProductQuantityDifferenceOriginalERPEquality))
                {
                    double quantityDifferenceOriginalERP;
                    if (Double.TryParse(ProductQuantityDifferenceOriginalERP, out quantityDifferenceOriginalERP))
                    {
                        string field = "QuantityDifferenceOriginalERP";
                        if (ProductQuantityDifferenceOriginalERPIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(ProductQuantityDifferenceOriginalERPEquality))
                        {
                            field = "QuantityDifferenceOriginalERP*QuantityDifferenceOriginalERP";
                            quantityDifferenceOriginalERP *= quantityDifferenceOriginalERP;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(ProductQuantityDifferenceOriginalERPEquality),
                            Value = quantityDifferenceOriginalERP
                        });
                    }
                }

				//++
				if (!String.IsNullOrWhiteSpace(ProductQuantityOriginalERP) && !String.IsNullOrWhiteSpace(ProductQuantityOriginalERPEquality))
				{
					double quantityOriginalERP;
					if (Double.TryParse(ProductQuantityOriginalERP, out quantityOriginalERP))
					{
						string field = "QuantityOriginalERP";
						if (ProductQuantityOriginalERPIsAbsolute &&
							ComboValues.Equality.IsAvailableForAbsoluteValue(ProductQuantityOriginalERPEquality))
						{
							field = "QuantityOriginalERP*QuantityOriginalERP";
							quantityOriginalERP *= quantityOriginalERP;
						}

						selectParams.FilterParams.Add(field, new FilterParam()
						{
							Operator = Common.Constants.ComboValues.Equality.ToOperator(ProductQuantityOriginalERPEquality),
							Value = quantityOriginalERP
						});
					}
				}


                if (!String.IsNullOrWhiteSpace(ProductValueDifferenceOriginalERP) && !String.IsNullOrWhiteSpace(ProductValueDifferenceOriginalERPEquality))
                {
                    double valueDifferenceOriginalERP;
                    if (Double.TryParse(ProductValueDifferenceOriginalERP, out valueDifferenceOriginalERP))
                    {
                        string field = "ValueDifferenceOriginalERP";
                        if (ProductValueDifferenceOriginalERPIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(ProductValueDifferenceOriginalERPEquality))
                        {
                            field = "ValueDifferenceOriginalERP*ValueDifferenceOriginalERP";
                            valueDifferenceOriginalERP *= valueDifferenceOriginalERP;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(ProductValueDifferenceOriginalERPEquality),
                            Value = valueDifferenceOriginalERP
                        });
                    }
                }
                //                ----------------------------
                if (!String.IsNullOrWhiteSpace(ProductPriceBuy) && !String.IsNullOrWhiteSpace(ProductPriceBuyEquality))
                {
                    double priceBuy;
                    if (Double.TryParse(ProductPriceBuy, out priceBuy))
                    {
                        string field = "PriceBuy";
                        if (ProductPriceBuyIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(ProductPriceBuyEquality))
                        {
                            field = "PriceBuy*PriceBuy";
                            priceBuy *= priceBuy;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(ProductPriceBuyEquality),
                            Value = priceBuy
                        });
                    }
                }

                if (!String.IsNullOrWhiteSpace(ProductPriceSale) && !String.IsNullOrWhiteSpace(ProductPriceSaleEquality))
                {
                    double priceSale;
                    if (Double.TryParse(ProductPriceSale, out priceSale))
                    {
                        string field = "PriceSale";
                        if (ProductPriceSaleIsAbsolute &&
                            ComboValues.Equality.IsAvailableForAbsoluteValue(ProductPriceSaleEquality))
                        {
                            field = "PriceSale*PriceSale";
                            priceSale *= priceSale;
                        }

                        selectParams.FilterParams.Add(field, new FilterParam()
                        {
                            Operator = Common.Constants.ComboValues.Equality.ToOperator(ProductPriceSaleEquality),
                            Value = priceSale
                        });
                    }
                }

                //
                if (!String.IsNullOrWhiteSpace(ProductName))
                {
                    selectParams.FilterParams.Add("ProductName", new FilterParam()
                    {
                        Value = ProductName,
                        Operator = FilterOperator.Contains
                    });
                }
            }

            if (IsFilterBySupplier)
            {
                if (!String.IsNullOrWhiteSpace(SupplierCode))
                {
                    selectParams.FilterParams.Add("SupplierCode", new FilterParam()
                    {
                        Value = SupplierCode,
                        Operator = FilterOperator.Contains
                    });
                }

                if (!String.IsNullOrWhiteSpace(SupplierName))
                {
                    selectParams.FilterParams.Add("SupplierName", new FilterParam()
                    {
                        Value = SupplierName,
                        Operator = FilterOperator.Contains
                    });
                }
            }


			if (IsFilterByFamily == true)
            {
				if (!String.IsNullOrWhiteSpace(FamilyCode))
                {
					selectParams.FilterParams.Add("FamilyCode", new FilterParam()
                    {
						Value = FamilyCode,
                        Operator = FilterOperator.Contains
                    });
                }

				if (!String.IsNullOrWhiteSpace(FamilyName))
                {
					selectParams.FilterParams.Add("FamilyName", new FilterParam()
                    {
						Value = FamilyName,
                        Operator = FilterOperator.Contains
                    });
                }
            }

            if (IsFilterBySection)
            {
                List<string> values = SectionItems;

                selectParams.FilterStringListParams.Add("SectionCode", new FilterStringListParam()
                {
                    Values = values
                });
            }

			selectParams.FilterParams.Add("ResultCode", new FilterParam()
			{
				Value = "XXX",
				Operator = FilterOperator.Equal
			});
        }
    }
}