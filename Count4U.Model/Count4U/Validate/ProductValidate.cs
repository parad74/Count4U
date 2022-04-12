using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using System.IO;
using Count4U.Model.Common;


namespace Count4U.Model.Count4U.Validate
{
	public static class ProductValidate
	{
		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.ProductMakat;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.General;
			}
			return "";
		}

		public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Makat;
			}
			return "";
		}

		#region validate
		public static int ValidateError(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			entity.Makat = "";
			entity.MakatOriginal = "";
			entity.ParentMakat = "";
			entity.TypeCode = "";
			entity.Name = "";
			entity.PriceString = "";
			entity.PriceSale = 0.0;
			entity.PriceBuy = 0.0;
			entity.PriceExtra = 0.0;
			entity.SupplierCode = "";
			entity.SectionCode = DomainUnknownCode.UnknownSection;
			entity.UnitTypeCode = ""; 
			entity.BalanceQuantityERP = 0.0;
			entity.BalanceQuantityPartialERP = 0;
			entity.CountInParentPack = 1;
			entity.IsUpdateERP = false;
			entity.InputTypeCode = InputTypeCodeEnum.B.ToString();
		
			int ret = 0;
			
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.Makat) == true)	
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.Makat = inValue.Makat.CutLength(299); 
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}


		public static int ValidateError(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			entity.Makat = "";
			entity.MakatOriginal = "";
			entity.ParentMakat = "";
			entity.TypeCode = "";
			entity.Name = "";
			entity.PriceString = "";
			entity.PriceSale = 0.0;
			entity.PriceBuy = 0.0;
			entity.SupplierCode = "";
			entity.SectionCode = DomainUnknownCode.UnknownSection;
			entity.UnitTypeCode = "";
			entity.BalanceQuantityERP = 0.0;
			entity.BalanceQuantityPartialERP = 0;
			entity.CountInParentPack = 1;
			entity.IsUpdateERP = false;
			entity.InputTypeCode = InputTypeCodeEnum.B.ToString();

			int ret = 0;

			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.Makat) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (inValue.Makat.Contains(@"'") == true)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			else
			{
				entity.Makat = inValue.Makat.CutLength(299); 
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateError(this Product entity, Product inValue, Dictionary<string, ProductMakat> makatDictionary)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int sameCodeExist = 0;

			entity.Makat = "";
			entity.MakatOriginal = "";
			entity.ParentMakat = "";
			entity.TypeCode = "";
			entity.Name = "";
			entity.PriceString = "";
			entity.PriceSale = 0.0;
			entity.PriceBuy = 0.0;
			entity.SupplierCode = "";
			entity.SectionCode = DomainUnknownCode.UnknownSection;
			entity.UnitTypeCode = "";
			entity.BalanceQuantityERP = 0.0;
			entity.BalanceQuantityPartialERP = 0;
			entity.CountInParentPack = 1;
			entity.IsUpdateERP = false;
			entity.InputTypeCode = InputTypeCodeEnum.B.ToString();
	
			int ret = 0;
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.Makat) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				string makat = inValue.Makat.CutLength(299);
				entity.ParentMakat = inValue.ParentMakat.CutLength(299); 
				entity.TypeCode = inValue.TypeCode;
				if (string.IsNullOrWhiteSpace(inValue.Name) == true) inValue.Name = "";
				entity.Name = inValue.Name.CutLength(99);
				entity.PriceString = inValue.PriceString;
				entity.PriceSale = inValue.PriceSale;
				entity.PriceBuy = inValue.PriceBuy;
				entity.SupplierCode = inValue.SupplierCode;
				//entity.CountInParentPack = inValue.CountInParentPack;
				//entity.BalanceQuantityERP = inValue.BalanceQuantityERP;

				entity.Makat = "";
				entity.MakatOriginal = "";
				if (IsMakatUnique(makatDictionary, makat) == true)
				{
					entity.Makat = makat;
					entity.MakatOriginal = makat;
				}
				else
				{
					codeIsEmpty = (int)ConvertDataErrorCodeEnum.SameCodeExist;
				}
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty + sameCodeExist;
			return ret;
		}

		public static int ValidateWarning(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			try
			{
				entity.ParentMakat = inValue.ParentMakat.CutLength(299);
				entity.MakatOriginal = inValue.MakatOriginal.CutLength(299); 
				entity.TypeCode = inValue.TypeCode;
				entity.SupplierCode = inValue.SupplierCode;
				entity.SectionCode = inValue.SectionCode;
				entity.UnitTypeCode = inValue.UnitTypeCode;
				entity.Family = inValue.Family;
				entity.FamilyCode = inValue.FamilyCode;
				
				
				if (string.IsNullOrWhiteSpace(inValue.Name) == false)
				{
					string name = inValue.Name.CutLength(99);
				}
				entity.PriceString = inValue.PriceString;
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			double priceBuy = 0.0;
			string inPriceBuy = inValue.PriceBuy.TrimStart('0');
			bool resultParse = Double.TryParse(inPriceBuy, out priceBuy);
			if (resultParse == true)
			{
				entity.PriceBuy = priceBuy;
			}
			//else
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}

			double priceSale = 0.0;
			string inPriceSale = inValue.PriceSale.TrimStart('0');
			resultParse = Double.TryParse(inPriceSale, out priceSale);
			if (resultParse == true)
			{
				entity.PriceSale = priceSale;
			}

			double priceExtra = 0.0;
			string inPriceExtra = inValue.PriceExtra.TrimStart('0');
			resultParse = Double.TryParse(inPriceExtra, out priceExtra);
			if (resultParse == true)
			{
				entity.PriceExtra = priceExtra;
			}

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityERP = balanceQuantityERP;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityPartialERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;

			int countInParentPack = 1;
			string inCountInParentPack = inValue.CountInParentPack.TrimStart('0');
			resultParse = Int32.TryParse(inCountInParentPack, out countInParentPack);
			if (resultParse == false && string.IsNullOrWhiteSpace(inCountInParentPack) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.CountInParentPack = countInParentPack == 0 ? 1 : countInParentPack;
		
			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}


		public static int ValidateWarning(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			try
			{
				entity.ParentMakat = inValue.ParentMakat.CutLength(299);
				entity.MakatOriginal = inValue.MakatOriginal.CutLength(299);
				entity.TypeCode = inValue.TypeCode;
				entity.SupplierCode = inValue.SupplierCode;
				entity.SectionCode = inValue.SectionCode;
				entity.UnitTypeCode = inValue.UnitTypeCode;
				entity.Family = inValue.Family;
				entity.FamilyCode = inValue.FamilyCode;

				if (string.IsNullOrWhiteSpace(inValue.Name) == false)
				{
					string name = inValue.Name.Trim();
					if (name.Length > 99)
					{
						entity.Name = name.CutLength(99);
						entity.Description = name.CutLength(499);
					}
					else
					{
						entity.Name = name;
					}
				}
				entity.PriceString = inValue.PriceString;
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			double priceBuy = 0.0;
			string inPriceBuy = inValue.PriceBuy.TrimStart('0');
			bool resultParse = Double.TryParse(inPriceBuy, out priceBuy);
			if (resultParse == true)
			{
				entity.PriceBuy = priceBuy;
			}
			//else
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}

			double priceSale = 0.0;
			string inPriceSale = inValue.PriceSale.TrimStart('0');
			resultParse = Double.TryParse(inPriceSale, out priceSale);
			if (resultParse == true)
			{
				entity.PriceSale = priceSale;
			}

			double priceExtra = 0.0;
			string inPriceExtra = inValue.PriceExtra.TrimStart('0');
			resultParse = Double.TryParse(inPriceExtra, out priceExtra);
			if (resultParse == true)
			{
				entity.PriceExtra = priceExtra;
			}

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityERP = balanceQuantityERP;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityPartialERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;

			int countInParentPack = 1;
			string inCountInParentPack = inValue.CountInParentPack.TrimStart('0');
			resultParse = Int32.TryParse(inCountInParentPack, out countInParentPack);
			if (resultParse == false && string.IsNullOrWhiteSpace(inCountInParentPack) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.CountInParentPack = countInParentPack == 0 ? 1 : countInParentPack;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdate(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			//try
			//{
				//entity.ParentMakat = inValue.ParentMakat;
				//entity.MakatOriginal = inValue.MakatOriginal;
				//entity.TypeCode = inValue.TypeCode;
				//entity.SupplierCode = inValue.SupplierCode;
				//entity.SectionCode = inValue.SectionCode;
				//entity.UnitTypeCode = inValue.UnitTypeCode;

				//if (string.IsNullOrWhiteSpace(inValue.Name) == false)
				//{
				//    if (inValue.Name.Length > 49)
				//    {
				//        entity.Name = inValue.Name.Substring(0, 49);
				//    }
				//    else
				//    {
				//        entity.Name = inValue.Name;
				//    }
				//}
				//entity.PriceString = inValue.PriceString;
			//}
			//catch
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}
			double priceBuy = 0.0;
			string inPriceBuy = inValue.PriceBuy.TrimStart('0');
			bool resultParse = Double.TryParse(inPriceBuy, out priceBuy);
			if (resultParse == true)
			{
				entity.PriceBuy = priceBuy;
			}
			else
			{
				entity.PriceBuy = 0.0;
				//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			double priceSale = 0.0;
			string inPriceSale = inValue.PriceSale.TrimStart('0');
			resultParse = Double.TryParse(inPriceSale, out priceSale);
			if (resultParse == true)
			{
				entity.PriceSale = priceSale;
			}
			else
			{
				entity.PriceSale = 0.0;
			}

			double priceExtra = 0.0;
			string inPriceExtra = inValue.PriceExtra.TrimStart('0');
			resultParse = Double.TryParse(inPriceExtra, out priceExtra);
			if (resultParse == true)
			{
				entity.PriceExtra = priceExtra;
			}
			else
			{
				entity.PriceExtra = 0.0;
			}

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityERP = balanceQuantityERP;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityPartialERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;
		

			int countInParentPack = 1;
			string inCountInParentPack = inValue.CountInParentPack.TrimStart('0');
			resultParse = Int32.TryParse(inCountInParentPack, out countInParentPack);
			if (resultParse == false && string.IsNullOrWhiteSpace(inCountInParentPack) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.CountInParentPack = countInParentPack == 0 ? 1 : countInParentPack;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdateQuantityERP(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			bool resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			//if (resultParse == true)
			//{
				entity.BalanceQuantityERP = balanceQuantityERP;
			//}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdateQuantityPartialERP(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			bool resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			//if (resultParse == true)
			//{
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;
			//}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}


		public static int ValidateUpdate(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			//try
			//{
			//entity.ParentMakat = inValue.ParentMakat;
			//entity.MakatOriginal = inValue.MakatOriginal;
			//entity.TypeCode = inValue.TypeCode;
			//entity.SupplierCode = inValue.SupplierCode;
			//entity.SectionCode = inValue.SectionCode;
			//entity.UnitTypeCode = inValue.UnitTypeCode;

			//if (string.IsNullOrWhiteSpace(inValue.Name) == false)
			//{
			//    if (inValue.Name.Length > 49)
			//    {
			//        entity.Name = inValue.Name.Substring(0, 49);
			//    }
			//    else
			//    {
			//        entity.Name = inValue.Name;
			//    }
			//}
			//entity.PriceString = inValue.PriceString;
			//}
			//catch
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}
			double priceBuy = 0.0;
			string inPriceBuy = inValue.PriceBuy.TrimStart('0');
			bool resultParse = Double.TryParse(inPriceBuy, out priceBuy);
			if (resultParse == true)
			{
				entity.PriceBuy = priceBuy;
			}
			else
			{
				entity.PriceBuy = 0.0;
				//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			double priceSale = 0.0;
			string inPriceSale = inValue.PriceSale.TrimStart('0');
			resultParse = Double.TryParse(inPriceSale, out priceSale);
			if (resultParse == true)
			{
				entity.PriceSale = priceSale;
			}
			else
			{
				entity.PriceSale = 0.0;
			}

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityERP = balanceQuantityERP;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			if (resultParse == false && string.IsNullOrWhiteSpace(inBalanceQuantityPartialERP) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;


			int countInParentPack = 1;
			string inCountInParentPack = inValue.CountInParentPack.TrimStart('0');
			resultParse = Int32.TryParse(inCountInParentPack, out countInParentPack);
			if (resultParse == false && string.IsNullOrWhiteSpace(inCountInParentPack) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.CountInParentPack = countInParentPack == 0 ? 1 : countInParentPack;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdateQuantityERP(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			
			//string quantityERP = "6.0000";
		

			double balanceQuantityERP = 0.0;
   			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');

			bool resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse == true)
			{
			entity.BalanceQuantityERP = balanceQuantityERP;
			}
			else
			{
				bool resultParse1 = Double.TryParse(inBalanceQuantityERP, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out balanceQuantityERP);
				if (resultParse1 == true)
				{
					entity.BalanceQuantityERP = balanceQuantityERP;
				}
			}
			

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdateQuantityERPAndPrice(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			double priceBuy = 0.0;
			string inPriceBuy = inValue.PriceBuy.TrimStart('0');
			bool resultParse = Double.TryParse(inPriceBuy, out priceBuy);
			if (resultParse == true)
			{
				entity.PriceBuy = priceBuy;
			}
			else
			{
				entity.PriceBuy = 0.0;
			}

			double priceSale = 0.0;
			string inPriceSale = inValue.PriceSale.TrimStart('0');
			resultParse = Double.TryParse(inPriceSale, out priceSale);
			if (resultParse == true)
			{
				entity.PriceSale = priceSale;
			}
			else
			{
				entity.PriceSale = 0.0;
			}


			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');

			bool resultParse2 = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			if (resultParse2 == true)
			{
				entity.BalanceQuantityERP = balanceQuantityERP;
			}
			else
			{
				bool resultParse1 = Double.TryParse(inBalanceQuantityERP, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out balanceQuantityERP);
				if (resultParse1 == true)
				{
					entity.BalanceQuantityERP = balanceQuantityERP;
				}
			}


			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateUpdateQuantityPartialERP(this Product entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			bool resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			//if (resultParse == true)
			//{
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;
			//}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}


		public static int ValidateUpdateQuantityERPAndQuantityPartialERP(this Product entity, ProductSimpleString inValue)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			double balanceQuantityERP = 0.0;
			string inBalanceQuantityERP = inValue.BalanceQuantityERP.TrimStart('0');
			bool resultParse = Double.TryParse(inBalanceQuantityERP, out balanceQuantityERP);
			entity.BalanceQuantityERP = balanceQuantityERP;
		
			int balanceQuantityPartialERP = 0;
			string inBalanceQuantityPartialERP = inValue.BalanceQuantityPartialERP.TrimStart('0');
			resultParse = Int32.TryParse(inBalanceQuantityPartialERP, out balanceQuantityPartialERP);
			entity.BalanceQuantityPartialERP = balanceQuantityPartialERP;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static string FormatInputString(this string inputString, MaskRecord maskRecord, MaskTemplate maskTemplate)
		{
			string outputString = inputString;
			if (maskRecord != null && maskTemplate != null)
			{
				try
				{
					outputString = maskTemplate.FormatString(inputString, maskRecord.Value);
				}
				catch { }
			}
			return outputString;
		}

		public static int Validate(this ProductSimple entity, ProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int ret = 0;
			int validateError = 0;
			int validateWarning = 0;

			validateError = ValidateError(entity, inValue, dtfi);
			if (validateError == 0)
			{
				validateWarning = ValidateWarning(entity, inValue, dtfi);
			}

			ret = ret + validateError + validateWarning;
			return ret;
		}
		#endregion

		#region dictionary
		public static bool IsDictionaryContainsKey(this Dictionary<string, ProductMakat> dictionary, string key, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.MakatExistInDB, message));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, ProductMakat> dictionary, string key, ProductMakat newLocation, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.MakatExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newLocation);
			}
		}
		#endregion

		//public static string ReverseWinHebrew(this string name, bool reverse = true, bool Rt2Lf = false)
		//{
		//    if (reverse == false) return name;
		//    if (name.Length == 0) return name;
		//    string temp = name;
		//    List<string> list = new List<string>();
		//    char[] charSet = temp.ToCharArray();
		//    CodingEnum currentCoding = charSet[0].IsDosHebrew();
		//    int startIndex = 0;
		//    int endIndex = 0;
		//    for (int i = 0; i <= charSet.Length - 1; i++)
		//    {
		//        CodingEnum tempCoding = charSet[i].IsDosHebrew();
		//        if (tempCoding != currentCoding)
		//        {
		//            endIndex = i;
		//            temp = name.Substring(startIndex, endIndex - startIndex);
		//            string tempReverse = ReverseText(temp, currentCoding);
		//            list.Add(tempReverse);
		//            startIndex = endIndex;
		//            currentCoding = tempCoding;
		//        }
		//        if (i == charSet.Length - 1)
		//        {
		//            endIndex = i + 1;
		//            temp = name.Substring(startIndex, endIndex - startIndex);
		//            string tempReverse = ReverseText(temp, currentCoding);
		//            list.Add(tempReverse);
		//        }
		//    }
		//    temp = "";
		//    foreach (string s in list)
		//    {
		//        temp = temp + s;
		//    }

		//    return temp;
		//}

		public static string FormatComa2(this string price)
		{
			string retFormatComa2 = "0";
			if (String.IsNullOrEmpty(price) == false)
			{
				int priceSaleLength = price.Length;
				string priceSale2 = "0";
				string priceSale1 = "0";
				if (price.Length > 2)
				{
					int start = price.Length - 2;
					priceSale1 = price.Substring(0, start);
					priceSale2 = price.Substring(start);
				}
				else if (price.Length == 2)
				{
					priceSale2 = price;
				}
				else if (price.Length == 1)
				{
					priceSale2 =  "0" + price;
				}

				retFormatComa2 = String.Format("{0}.{1}", priceSale1, priceSale2);
			}
			else
			{
				retFormatComa2 = "0";
			}
			return retFormatComa2;
		}

		public static string FormatComa3(this string price)
		{
			string retFormatComa3 = "0";
			if (String.IsNullOrEmpty(price) == false)
			{
				int priceSaleLength = price.Length;
				string priceSale2 = "0";
				string priceSale1 = "0";
				if (price.Length > 3)
				{
					int start = price.Length - 3;
					priceSale1 = price.Substring(0, start);
					priceSale2 = price.Substring(start);
				}
				else if (price.Length == 3)
				{
					priceSale2 = price;
				}
				else if (price.Length == 2)
				{
					priceSale2 = "0" +price;
				}
				else if (price.Length == 1)
				{
					priceSale2 = "00" + price;
				}

				retFormatComa3 = String.Format("{0}.{1}", priceSale1, priceSale2);
			}
			else
			{
				retFormatComa3 = "0";
			}
			return retFormatComa3;
		}

		public static string ReverseDosHebrew(this string name, bool reverseLetterInWord = true, bool Rt2Lf = false)  //TODO  Rt2Lf = false
		{
			name = name.Trim();
			if (reverseLetterInWord == false && Rt2Lf == false) return name;
			if (name.Length == 0) return name;
			string temp = name;
			if (reverseLetterInWord == true)
			{
				List<string> list = new List<string>();
				char[] charSet = temp.ToCharArray();
				CodingEnum currentCoding = charSet[0].IsDosHebrew();
				int startIndex = 0;
				int endIndex = 0;
				for (int i = 0; i <= charSet.Length - 1; i++)
				{
					CodingEnum tempCoding = charSet[i].IsDosHebrew();
					if (tempCoding != currentCoding)
					{
						endIndex = i;
						temp = name.Substring(startIndex, endIndex - startIndex);
						string tempReverse = ReverseText(temp, currentCoding);
						list.Add(tempReverse);
						startIndex = endIndex;
						currentCoding = tempCoding;
					}
					if (i == charSet.Length - 1)
					{
						endIndex = i + 1;
						temp = name.Substring(startIndex, endIndex - startIndex);
						string tempReverse = ReverseText(temp, currentCoding);
						list.Add(tempReverse);
					}
				}
				temp = "";
				foreach (string s in list)
				{
					temp = temp + s;
				}
			}
			if (Rt2Lf == true)
			{
				if (temp.HaveDosHebrewCharInWord() == true)	 //если хоь одна буква на иврите
				{
 					string[] listRecord = temp.Split(',');
					string retRecord = "";
					for (int i = 0; i < listRecord.Length; i++)	   //записи в строке, разделенные запятой
					{
						string tempRecord = listRecord[i];		//каждую запись переставляем слова поотдельности
						listRecord[i] = ReverseWord(tempRecord);	 //меняем запись - порядок слов в записи на
						retRecord = retRecord + listRecord[i] + ",";		//собираем запись
					}
					retRecord = retRecord.TrimEnd(',');
					return retRecord;
				}
			}
			return temp;
		}


		public static string ReverseAnyEncoding(this string name, bool reverseLetterInWord = true, bool Rt2Lf = false)  //TODO  Rt2Lf = false
		{
			name = name.Trim();
			if (reverseLetterInWord == false && Rt2Lf == false) return name;
			if (name.Length == 0) return name;
			string temp = name;
			if (reverseLetterInWord == true)
			{
				string[] listRecord = temp.Split(' ');
				CodingEnum currentCoding = CodingEnum.EnglishAndHebrew;
				string recordReverse = "";
				foreach (var record in listRecord)
				{
					recordReverse = recordReverse + " " + ReverseText(record, currentCoding);
				}
				temp = recordReverse.Trim();
			}
			if (Rt2Lf == true)
			{
					string[] listRecord = temp.Split(',');
					string retRecord = "";
					for (int i = 0; i < listRecord.Length; i++)	   //записи в строке, разделенные запятой
					{
						string tempRecord = listRecord[i];		//каждую запись переставляем слова поотдельности
						listRecord[i] = ReverseWord(tempRecord);	 //меняем запись - порядок слов в записи на
						retRecord = retRecord + listRecord[i] + ",";		//собираем запись
					}
					retRecord = retRecord.TrimEnd(',');
					return retRecord;
			}
			return temp;
		}

		public static bool HaveDosHebrewCharInWord(this string tempRecord)
		{
			char[] charSet = tempRecord.ToCharArray();
			for (int i = 0; i <= charSet.Length - 1; i++)
			{
				CodingEnum tempCoding = charSet[i].IsDosHebrew();
				if (tempCoding == CodingEnum.Hebrew) return true;

			}
			return false;
		}

		public static bool HaveOnlyDosHebrewCharInWord(this string tempRecord)
		{
			char[] charSet = tempRecord.ToCharArray();
			for (int i = 0; i <= charSet.Length - 1; i++)
			{
				if (char.IsWhiteSpace(charSet[i]) == false)
				{
					CodingEnum tempCoding = charSet[i].IsDosHebrew();
					if (tempCoding == CodingEnum.Any) return false;
				}
			}
			return true;
		}

		private static string ReverseWord(string tempRecord)
		{
			string[] listRt2Lf = tempRecord.Split(' ');		 //список слов разделенных пробелом
			string tempRt2Lf = "";
			for (int j = listRt2Lf.Length - 1; j >= 0; j--)	//Меняем порядок слов на обратный
			{
				tempRt2Lf = tempRt2Lf + listRt2Lf[j] + " ";
			}
			tempRt2Lf = tempRt2Lf.TrimEnd(' ');
			return tempRt2Lf;
		}

		public static CodingEnum IsDosHebrew(this char ch)
		{
			CodingEnum ret = CodingEnum.Any;
			decimal codeCh = (decimal)ch;
			if (codeCh > 1487 && codeCh < 1515)
			{
				ret = CodingEnum.Hebrew;
				//char tmp = (char)codeCh;
			}
			if (codeCh == 39 || codeCh== 34)
			{
				ret = CodingEnum.Hebrew;
				//char tmp = (char)codeCh;
			}
			return ret;
		}

		//public static CodingEnum IsWinHebrew(this char ch)
		//{
		//    CodingEnum ret = CodingEnum.Any;
		//    decimal codeCh = (decimal)ch;
		//    if (codeCh > 211 && codeCh < 217)
		//    {
		//        ret = CodingEnum.Hebrew;
		//        //char tmp = (char)codeCh;
		//    }
		//    if (codeCh > 224 && codeCh < 250)
		//    {
		//        ret = CodingEnum.Hebrew;
		//        //char tmp = (char)codeCh;
		//    }
		//    return ret;
		//}

		private static string ReverseText(this string text, CodingEnum codingType)
		{
			if (codingType == CodingEnum.Any) return text;
			char[] charResulte = new char[text.Length];
			for (int i = 0, j = text.Length - 1; j >=0 ; i++, j--)
			{
				charResulte[i] = text[j];
			}
			return new string(charResulte);
		}

		public static string TrimSpaceInText(this string text)
		{
			for (int j = 30; j > 1; j--)
			{
				string inString = " ";
				inString = inString.PadLeft(j, ' ');
				text = text.Replace(inString, " ");
			}

			return text;
		}

		public static string ResaveText(this string text, Encoding encoding)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(ms, encoding);
				writer.Write(text);
				writer.Flush();
				return encoding.GetString(ms.ToArray());
			}
		}

		public enum CodingEnum
		{
			Hebrew,
			Any,
			EnglishAndHebrew
		}

        public static string MakatValidate(string makat)
        {
            if (String.IsNullOrWhiteSpace(makat))
                return ValidateMessage.Product.MakatIsEmpty;

            if (!Regex.IsMatch(makat, @"^[-_\d\w\s\\="".&]+$"))
                return ValidateMessage.Product.MakatFormat;

            return String.Empty;
        }

        public static string PriceSaleValidate(string priceSale)
        {
            if (!CommonValidate.IsOkAsDouble(priceSale))
                return Model.ValidateMessage.Product.PriceSaleFormat;

            return String.Empty;
        }

        public static string PriceBuyValidate(string priceBuy)
        {
            if (!CommonValidate.IsOkAsDouble(priceBuy))
                return Model.ValidateMessage.Product.PriceBuyFormat;

            return String.Empty;
        }

		private static bool IsMakatUnique(Dictionary<string, ProductMakat> makatDictionary, string makat)
		{
			bool isMakatUnique = makatDictionary.Any(r => r.Key == makat) == false;
			return isMakatUnique;
		}

		#region staticStringFunction

		public static string Fraction(this double inDouble)
		{
			string ret = "0";
			if (inDouble == 0.0) return ret;
			string inString = inDouble.ToString();
			
			if (string.IsNullOrWhiteSpace(inString) != true)
			{
				int index = -1;
				index = inString.IndexOf('.');
				if (index > 0)
				{
					ret = inString.Substring(index);
					ret = ret.TrimStart('.');
				}
			}
			return ret;
		}

		public static string LeadingZero2(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "00";
			}
			else
			{
				code = LeadingZeroN(inString, 2, code);
			}
			return code;
		}

		public static string LeadingZero3(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "000";
			}
			else
			{
				code = LeadingZeroN(inString, 3, code);
			}
			return code;
		}

		public static string LeadingZero4(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "0000";
			}
			else
			{
				code = LeadingZeroN(inString, 4, code);
			}
			return code;
		}

		public static string LeadingZero5(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "00000";
			}
			else
			{
				code = LeadingZeroZ(inString, 5, code);
			}
			return code;
		}

		public static string LeadingZero6(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 6, code);
			}
			return code;
		}

		public static string LeadingZero7(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "0000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 7, code);
			}
			return code;
		}

		public static string LeadingZero8(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "00000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 8, code);
			}
			return code;
		}

		public static string LeadingZero9(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "000000000";
			}
			else
			{
				code = LeadingZeroN(inString, 9, code);
			}
			return code;
		}

		public static string LeadingZero10(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "0000000000";
			}
			else
			{
				code = LeadingZeroN(inString, 10, code);
			}
			return code;
		}

		public static string LeadingZero11(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "00000000000";
			}
			else
			{
				code = LeadingZeroN(inString, 11, code);
			}
			return code;
		}

		public static string LeadingZero12(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "000000000000";
			}
			else
			{
				code = LeadingZeroN(inString, 12, code);
			}
			return code;
		}



		public static string LeadingZero13(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "0000000000000";
			}
			else
			{
				code = LeadingZeroN(inString, 13, code);
			}
			return code;
		}

		public static string LeadingZero14(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "00000000000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 14, code);
			}
			return code;
		}

		public static string LeadingZero15(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "000000000000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 15, code);
			}
			return code;
		}

		public static string LeadingZero16(this string inString)
		{
			string code = inString;
			if (string.IsNullOrWhiteSpace(inString) == true)
			{
				code = "0000000000000000";
			}
			else
			{
				code = LeadingZeroZ(inString, 16, code);
			}
			return code;
		}

		private static string LeadingZeroN(string inString, int n, string code)
		{
			int lenCode = inString.Length;

			if (lenCode > n) code = inString.Substring(0, n);

			if (lenCode < n)
			{
				int rest = n - lenCode;
				string temp = "";
				for (int i = 0; i < rest; i++)
				{
					temp = temp + "0";
				}
				code = temp + inString;
			}
			return code;
		}

		private static string LeadingZeroZ(string inString, int n, string code)
		{
			int lenCode = inString.Length;

			if (lenCode > n) code = inString.Substring(lenCode - n, n);

			if (lenCode < n)
			{
				int rest = n - lenCode;
				string temp = "";
				for (int i = 0; i < rest; i++)
				{
					temp = temp + "0";
				}
				code = temp + inString;
			}
			return code;
		}
		#endregion staticStringFunction
		
	}
}
