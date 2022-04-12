using System;
using System.Xml.Serialization;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.ViewModel.Filter.Data
{
    [Serializable]
    public class InventProductFilterData : CommonFilterData, IFilterData
    {
		private readonly IUserSettingsManager _userSettingsManager;

        public string Makat { get; set; }
        public string Barcode { get; set; }
		public string Code { get; set; }
		public string SerialNumber { get; set; }
        public string Name { get; set; }
        public bool OnlyWithError { get; set; }
        public string IturCode { get; set; }
		public string ErpIturCode { get; set; }
		public int IPNum { get; set; }
		public long ID { get; set; }
		public string PropertyStr { get; set; }
		public string PropertyStrName { get; set; }
		public string PropertyNumberName { get; set; }
		public string PropertyNumber { get; set; }
		public string PropertyNumberEquality { get; set; }
		public bool PropertyNumberIsAbsolute { get; set; }
		
		
		

        //        public string Section { get; set; }
        //        public string PriceBuy { get; set; }
        //        public string PriceSell { get; set; }
        //        public DateTime? ModifyDate { get; set; }

        [XmlIgnore]
        public string FieldViewName { get { return Common.ViewNames.SearchInventProductFieldView; } }

		public InventProductFilterData()								//IUserSettingsManager userSettingsManager
		{
			Makat = "";
			Barcode = "";
			Code = "";
			SerialNumber = "";
			Name = "";
			OnlyWithError = false;
			IturCode = "";
			ErpIturCode = "";
			PropertyStr = "";
			PropertyStrName = "";
			PropertyNumberName = "";
			PropertyNumber = "";
			IPNum = 0;
			ID = 0;
			//_userSettingsManager = userSettingsManager;
		}

        public bool IsAnyField()
        {
            if (base.IsFilterBySort())
                return true;

            if (!String.IsNullOrWhiteSpace(Makat))
                return true;

            if (!String.IsNullOrWhiteSpace(Barcode))
                return true;

			if (!String.IsNullOrWhiteSpace(Code))
				return true;

			if (!String.IsNullOrWhiteSpace(PropertyStr))
                return true;

			if (!String.IsNullOrWhiteSpace(PropertyNumber))		 //TODO
				return true;

			if (!String.IsNullOrWhiteSpace(SerialNumber))
				return true;

            if (!String.IsNullOrWhiteSpace(Name))
                return true;

            if (!String.IsNullOrWhiteSpace(IturCode))
                return true;

            if (OnlyWithError)
                return true;

			if (IPNum != 0)
				return true;

			if (ID != 0)
				return true;
            //
            //            if (!String.IsNullOrWhiteSpace(Section))
            //                return true;
            //
            //            if (!String.IsNullOrWhiteSpace(PriceBuy))
            //                return true;
            //
            //            if (!String.IsNullOrWhiteSpace(PriceSell))
            //                return true;
            //
            //            if (ModifyDate != null)
            //                return true;

            return false;
        }

        public void ApplyToSelectParams(SelectParams selectParams)
        {
            base.ApplySortToSelectParams(selectParams);

            if (!String.IsNullOrEmpty(Name))
            {
                selectParams.FilterParams.Add("ProductName",
                                        new FilterParam() { Operator = FilterOperator.Contains, Value = Name });
            }
            if (!String.IsNullOrEmpty(Barcode))
            {
                selectParams.FilterParams.Add("Barcode",
                                        new FilterParam() { Operator = FilterOperator.Contains, Value = Barcode });
            }
            if (!String.IsNullOrEmpty(Makat))
            {
                selectParams.FilterParams.Add("Makat",
                                        new FilterParam() { Operator = FilterOperator.Contains, Value = Makat });
            }
			if (!String.IsNullOrEmpty(Code))
			{
				selectParams.FilterParams.Add("Code",
										new FilterParam() { Operator = FilterOperator.Contains, Value = Code });
			}

			if (!String.IsNullOrEmpty(SerialNumber))
			{
				selectParams.FilterParams.Add("SerialNumber",
										new FilterParam() { Operator = FilterOperator.Contains, Value = SerialNumber });
			}

			//string PropertyStrName = "IPValueStr8"; 
			//string PropertyStrNameGet = _userSettingsManager.InventProductPropertySelectedItemGet();
			if (String.IsNullOrEmpty(PropertyStrName) == false)
			{
				if (!String.IsNullOrEmpty(PropertyStr))
				{
					selectParams.FilterParams.Add(PropertyStrName,
											new FilterParam() { Operator = FilterOperator.Contains, Value = PropertyStr });
				}
			}

			// PropertyNumberName
			if (!String.IsNullOrWhiteSpace(PropertyNumber) && !String.IsNullOrWhiteSpace(PropertyNumberEquality))
			{
				double quantity;
				if (Double.TryParse(PropertyNumber, out quantity))
				{
					string field = PropertyNumberName;
					if (PropertyNumberIsAbsolute &&
						Count4U.Common.Constants.ComboValues.Equality.IsAvailableForAbsoluteValue(PropertyNumberEquality))
					{
						field = PropertyNumberName + "*" + PropertyNumberName;
						quantity *= quantity;
					}
					selectParams.FilterParams.Add(field, new FilterParam()
					{
						Operator = Common.Constants.ComboValues.Equality.ToOperator(PropertyNumberEquality),
						Value = quantity
					});
				}
			}


            if (!String.IsNullOrEmpty(IturCode))
            {
                selectParams.FilterParams.Add("IturCode",
                                        new FilterParam() { Operator = FilterOperator.Contains, Value = IturCode });
            }

            if (OnlyWithError)
            {
                selectParams.FilterParams.Add("StatusInventProductBit",
                new FilterParam() { Operator = FilterOperator.Greater, Value = 0 });
            }

			if (IPNum != 0)
			{
				selectParams.FilterParams.Add("IPNum",
										new FilterParam() { Operator = FilterOperator.Equal, Value = IPNum });
			}

			if (ID != 0)
			{
				selectParams.FilterParams.Add("ID",
										new FilterParam() { Operator = FilterOperator.Equal, Value = ID });
			}


			

            //            if (!String.IsNullOrEmpty(_inventProductPriceBuy))
            //            {
            //
            //                double priceBuy = 0;
            //                bool ret = Double.TryParse(_inventProductPriceBuy, out priceBuy);
            //                if (ret)
            //                {
            //                    result.FilterParams.Add("PriceSale", new FilterParam() { Operator = FilterOperator.Equal, Value = searchVal });
            //                }
            //
            //                result.FilterParams.Add("TypeCode", new FilterParam() { Operator = FilterOperator.Equal, Value = TypeMakatEnum.M.ToString() });
            //            }
            //
            //            if (this._selectedFilter == FilterPriceBy)
            //            {
            //
            //                double searchVal = 0;
            //                bool ret = Double.TryParse(this._searchValue, out searchVal);
            //                if (ret == true)
            //                {
            //                    result.FilterParams.Add("PriceBuy", new FilterParam() { Operator = FilterOperator.Equal, Value = searchVal });
            //                }
            //
            //                result.FilterParams.Add("TypeCode", new FilterParam() { Operator = FilterOperator.Equal, Value = TypeMakatEnum.M.ToString() });
            //            }

            //            if (ModifyDate != null)
            //            {
            //                DateTime date = ModifyDate.Value;
            //                DateTime from = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            //                DateTime to = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            //                selectParams.FilterDateTimeParams.Add("ModifyDate",
            //                                                new FilterParam()
            //                                                {
            //                                                    Operator = FilterOperator.DateTimeBetween,
            //                                                    Value = from,
            //                                                    Value1 = to
            //                                                });
            //            }
        }


	}
}