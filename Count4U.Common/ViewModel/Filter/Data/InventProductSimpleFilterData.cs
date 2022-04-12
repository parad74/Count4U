using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Serialization;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Linq;
using Section = Count4U.Model.Count4U.Section;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class InventProductSimpleFilterData : CommonFilterData, IFilterData
    {
		public InventProductSimpleFilterData()
		{
			LocationItems = new List<string>();
			SectionItems = new List<string>();

			InventProductInputTypeK = true;
			InventProductInputTypeB = true;

			InventProductIsItemsFromCatalog = true;
			InventProductIsItemsNotInCatalog = true;

			InventProductQuantityDifferenceEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductQuantityEditEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductValueBuyDifferenceEquality = Common.Constants.ComboValues.Equality.Equal;
			InventProductValueBuyEditEquality = Common.Constants.ComboValues.Equality.Equal;

			ProductPriceBuyEquality = Common.Constants.ComboValues.Equality.Equal;
			ProductPriceSaleEquality = Common.Constants.ComboValues.Equality.Equal;

			//-- String.Empty 
			this.IturCode = "";
			this.IturERPCode = "";
			this.IturNumberPrefix = "";
			this.IturNumberSuffix = "";
			this.InventProductQuantityDifference = "";
			this.InventProductQuantityDifferenceEquality = "";
			this.InventProductQuantityEdit = "";
			this.InventProductQuantityEditEquality = "";
			this.InventProductValueBuyDifference = "";
			this.InventProductValueBuyDifferenceEquality = "";
			this.InventProductValueBuyEdit = "";
			this.InventProductValueBuyEditEquality = "";
			this.ProductMakat = "";
			this.ProductBarcode = "";
			this.ProductPriceBuy = "";
			this.ProductPriceBuyEquality = "";
			this.ProductPriceSale = "";
			this.ProductPriceSaleEquality = "";
			this.ProductName = "";
			this.SupplierCode = "";
			this.SupplierName = "";
			this.FamilyCode = "";
			this.FamilyName = "";
			this.ReportQuantityDifferenceOriginalERPEquality = "";
			this.ReportQuantityDifferenceOriginalERP = "";
			this.ReportValueDifferenceOriginalERPEquality = "";
			this.ReportValueDifferenceOriginalERP = "";
		}

        public bool IsLocationExpanded { get; set; }
        public bool IsFilterByLocation { get; set; }
        // public List<Location> LocationItems { get; set; }
        public List<string> LocationItems { get; set; } //Code

        public bool IsIturExpanded { get; set; }
        public bool IsFilterByItur { get; set; }
        public string IturCode { get; set; }
        public string IturERPCode { get; set; }
        public string IturNumberPrefix { get; set; }
        public string IturNumberSuffix { get; set; }

        public bool IsInventProductExpanded { get; set; }
        public bool IsFilterByInventProduct { get; set; }
        public bool InventProductInputTypeK { get; set; }
        public bool InventProductInputTypeB { get; set; }
        public bool InventProductIsItemsFromCatalog { get; set; }
        public bool InventProductIsItemsNotInCatalog { get; set; }
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
        public bool InventProductValueBuyEditIsAbsolute { get; set; }
        public string InventProductValueBuyEditEquality { get; set; }

        public bool IsProductExpanded { get; set; }
        public bool IsFilterByProduct { get; set; }
        public string ProductMakat { get; set; }
        public string ProductBarcode { get; set; }
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
        //public List<Section> SectionItems { get; set; }
        public List<string> SectionItems { get; set; } //SectionCode

        public bool IsReportExpanded { get; set; }
        public bool IsFilterByReport { get; set; }
        public string ReportQuantityDifferenceOriginalERPEquality { get; set; }
        public string ReportQuantityDifferenceOriginalERP { get; set; }
        public bool ReportQuantityDifferenceOriginalERPIsAbsolute { get; set; }

        public string ReportValueDifferenceOriginalERPEquality { get; set; }
        public string ReportValueDifferenceOriginalERP { get; set; }
        public bool ReportValueDifferenceOriginalERPIsAbsolute { get; set; }

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (IsFilterByLocation)
            {
                return true;
            }

            if (IsFilterByItur)
            {
                if (!String.IsNullOrWhiteSpace(IturCode) ||
                       !String.IsNullOrWhiteSpace(IturERPCode) ||
                       !String.IsNullOrWhiteSpace(IturNumberPrefix) ||
                       !String.IsNullOrWhiteSpace(IturNumberSuffix))
                    return true;
            }

            if (IsFilterByInventProduct)
            {
                return true;
            }

            if (IsFilterByProduct)
            {
                if (!String.IsNullOrWhiteSpace(ProductMakat) ||
                    !String.IsNullOrWhiteSpace(ProductBarcode) ||
                    !String.IsNullOrWhiteSpace(ProductPriceBuy) ||
                    !String.IsNullOrWhiteSpace(ProductPriceSale)
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

			if (IsFilterByFamily)
			{
				if (!String.IsNullOrWhiteSpace(FamilyName) || !String.IsNullOrWhiteSpace(FamilyCode))
				{
					return true;
				}
			}

            if (IsFilterBySection)
            {
                return true;
            }

            return false;
        }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchInventProductAdvancedFieldSimpleView; } }

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (IsFilterByLocation && LocationItems.Any())
            {
                List<string> values = LocationItems;

                selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
                    {
                        Values = values
                    });
            }

            if (IsFilterByItur)
            {
                if (!String.IsNullOrWhiteSpace(IturCode))
                {
                    selectParams.FilterParams.Add("IturCode", new FilterParam()
                        {
                            Operator = FilterOperator.Contains,
                            Value = IturCode
                        });
                }

                if (!String.IsNullOrWhiteSpace(IturERPCode))
                {
                    selectParams.FilterParams.Add("ERPIturCode", new FilterParam()
                    {
                        Operator = FilterOperator.Contains,
                        Value = IturERPCode
                    });
                }

                if (!String.IsNullOrWhiteSpace(IturNumberPrefix))
                {
                    selectParams.FilterParams.Add("Itur_NumberPrefix", new FilterParam()
                    {
                        Operator = FilterOperator.Contains,
                        Value = IturNumberPrefix
                    });
                }

                if (!String.IsNullOrWhiteSpace(IturNumberSuffix))
                {
                    selectParams.FilterParams.Add("Itur_NumberSufix", new FilterParam()
                    {
                        Operator = FilterOperator.Contains,
                        Value = IturNumberSuffix
                    });
                }
            }

            if (IsFilterByInventProduct)
            {

                List<string> values = new List<string>();
                if (InventProductInputTypeK)
                {
                    values.Add(InputTypeCodeEnum.K.ToString());
                }
                if (InventProductInputTypeB)
                {
                    values.Add(InputTypeCodeEnum.B.ToString());
                }

                if (values.Any())
                {
                    selectParams.FilterStringListParams.Add("InputTypeCode", new FilterStringListParam()
                    {
                        Values = values
                    });
                }

                List<string> typeMakatList = new List<string>();
                if (InventProductIsItemsFromCatalog)
                {
                    typeMakatList.Add(TypeMakatEnum.M.ToString());
                    typeMakatList.Add(TypeMakatEnum.B.ToString());
                }
                if (InventProductIsItemsNotInCatalog)
                {
                    typeMakatList.Add(TypeMakatEnum.W.ToString());
                }

                if (typeMakatList.Any())
                {
                    selectParams.FilterStringListParams["TypeMakat"] = new FilterStringListParam() { Values = typeMakatList };
                }


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

                if (!String.IsNullOrWhiteSpace(ProductBarcode))
                {
                    selectParams.FilterParams.Add("Barcode", new FilterParam()
                    {
                        Value = ProductBarcode,
                        Operator = FilterOperator.Contains
                    });
                }

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

			if (IsFilterByFamily)
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

            if (IsFilterBySection && SectionItems.Any())
            {
                List<string> values = SectionItems;

                selectParams.FilterStringListParams.Add("SectionCode", new FilterStringListParam()
                {
                    Values = values
                });
            }

            if (IsFilterByReport)
            {
                if (!String.IsNullOrWhiteSpace(ReportQuantityDifferenceOriginalERP))
                {
                    int quantityDifference;
                    if (Int32.TryParse(ReportQuantityDifferenceOriginalERP, out quantityDifference))
                    {
                        if (quantityDifference > 0)
                            selectParams.Extra[SelectParamsExtra.ReportQuantityDifferenceERP] = ReportQuantityDifferenceOriginalERP;
                    }

                }

                if (!String.IsNullOrWhiteSpace(ReportValueDifferenceOriginalERP))
                {
                    int valueDifference;
                    if (Int32.TryParse(ReportValueDifferenceOriginalERP, out valueDifference))
                    {
                        if (valueDifference > 0)
                            selectParams.Extra[SelectParamsExtra.ReportValueDifferenceERP] = ReportValueDifferenceOriginalERP;
                    }
                }
            }
        }
    }
}