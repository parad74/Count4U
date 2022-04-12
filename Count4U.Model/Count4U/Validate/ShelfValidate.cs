using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Practices.ServiceLocation;
using System.Reflection;

namespace Count4U.Model.Count4U.Validate
{
	public static class ShelfValidate
	{
		#region message
		//todo
		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.DocumentCode;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.DocumentCode;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Code;
			}
			return "";
		}

		public static List<string> Bit2WarningMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(InventProductValidate.ConvertDataErrorCode2WarningMessage(b));
			}
			return ret;
		}

		public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.BarcodeOrMakatIsNotExistsInCatalog;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Barcode;
			}
			return "";
		}
		#endregion

		#region validate
		public static int ValidateError(this Shelf entity, ShelfString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			int ret = 0;
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}

			entity.ShelfCode = Guid.NewGuid().ToString(); 
			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			//entity.StatusInventProductBit = entity.StatusInventProductBit.Or(ret);
			return ret;
		}

		
		public static int ValidateWarning(this Shelf entity, ShelfString inValue,
			DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			int ret = 0;

			//ShelfCode = "";
			//ShelfPartCode = "";
			//IturCode = "";
			//SupplierCode = DomainUnknownName.UnknownSupplier;
			//SupplierName = "";
			//CreateDataTime = DateTime.Now;
			//ShelfNum = 0;
			//Width = 0;
			//Height = 0;
			//Area = 0;
	
			try
			{
				entity.ShelfPartCode = inValue.ShelfPartCode;
				entity.IturCode = inValue.IturCode;
				entity.SupplierCode = inValue.SupplierCode;
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			int shelfPartInItur = 1;
			string strValue1 = inValue.ShelfPartInItur.Trim();
			if (string.IsNullOrWhiteSpace(strValue1) == false && strValue1 != "0")
			{
				try
				{
					shelfPartInItur = Convert.ToInt32(strValue1);
				}
				catch
				{
					double shelfPartInItur1 = 1;
					bool yes1 = Double.TryParse(strValue1, out shelfPartInItur1);
					try
					{
						if (yes1 == true) shelfPartInItur = Convert.ToInt32(shelfPartInItur1);
					}
					catch
					{
						invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
					}
				}
			}
			if (shelfPartInItur == 0) shelfPartInItur = 1;

			double width = 0.0;
			bool yes = Double.TryParse(inValue.Width.Trim(), out width);
			if (yes == false && string.IsNullOrWhiteSpace(inValue.Width.Trim()) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.Width = width;

			double height = 0.0;
			bool yes2 = Double.TryParse(inValue.Height.Trim(), out height);
			if (yes2 == false && string.IsNullOrWhiteSpace(inValue.Width.Trim()) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.Height = height / (double)shelfPartInItur;

			entity.Area = (int)((entity.Height * entity.Width));// / 10000;

			int shelfNum = 0;
			string strValue = inValue.ShelfNum.Trim();
			if (string.IsNullOrWhiteSpace(strValue) == false && strValue != "0")
			{
				try
				{
					shelfNum = Convert.ToInt32(strValue);
				}
				catch
				{
					double shelfNum1 = 0;
					bool yes1 = Double.TryParse(strValue, out shelfNum1);
					try
					{
						if (yes1 == true) shelfNum = Convert.ToInt32(shelfNum1);
					}
					catch
					{
						invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
					}
				}
			}
			entity.ShelfNum = shelfNum;

			try
			{
				string dt = inValue.CreateDate.Trim(" ".ToCharArray()) + " "
					+ inValue.CreateTime.Trim(" ".ToCharArray());
				DateTime dateTime = Convert.ToDateTime(dt, dtfi);
				entity.CreateDataTime = dateTime;
			}
			catch { entity.CreateDataTime = DateTime.Now; }

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
		//entity.StatusInventProductBit = entity.StatusInventProductBit.Or(ret);
			return ret;
		}

		public static int Validate(this Shelf entity, ShelfString inValue, DateTimeFormatInfo dtfi)
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

		//public static string MakatValidate(string makat)
		//{
		//	if (String.IsNullOrWhiteSpace(makat))
		//		return ValidateMessage.InventProduct.MakatIsEmpty;

		//	if (!Regex.IsMatch(makat, @"^[-_\d\w\s\\="".&]+$"))
		//		return ValidateMessage.InventProduct.MakatFormat;

		//	return String.Empty;
		//}

		//public static string QuantityEditValidate(string quantityEdit)
		//{
		//	if (!CommonValidate.IsOkAsDouble(quantityEdit))
		//		return Model.ValidateMessage.InventProduct.QuantityEditFormat;

		//	//if (CommonValidate.DigitsAfterDecimalPoint(quantityEdit) > 3)
		//	//{
		//	//   return Model.ValidateMessage.InventProduct.QuantityEditFormat;

		//	//}

		//	return String.Empty;
		//}

		//public static string QuantityInPackEditValidate(string quantityInPackEdit)
		//{
		//	if (!CommonValidate.IsOkAsInt(quantityInPackEdit))
		//		return Model.ValidateMessage.InventProduct.QuantityInPackEditFormat;	  // todo
		//	return String.Empty;
		//}

		//public static string QuantityOriginalValidate(string quantityOriginal)
		//{
		//	if (!CommonValidate.IsOkAsDouble(quantityOriginal))
		//		return Model.ValidateMessage.InventProduct.QuantityOriginalFormat;
		//	return String.Empty;
		//}

		//private static bool IsMakatUnique(Dictionary<string, ProductMakat> makatDictionary, string makat)
		//{
		//	bool isMakatUnique = makatDictionary.Any(r => r.Key == makat) == false;
		//	return isMakatUnique;
		//}



	}
}

