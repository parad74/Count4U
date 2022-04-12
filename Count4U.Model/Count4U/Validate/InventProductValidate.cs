using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Practices.ServiceLocation;
using System.Reflection;
using Count4U.Model.Interface;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U.Validate
{
	public static class InventProductValidate
	{
		#region message
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
		public static int ValidateError(this InventProduct entity, InventProductSimpleString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			entity.Makat = "";
			entity.Barcode = "";
			entity.ProductName = "";
			entity.SerialNumber = "";
			entity.QuantityOriginal = 0.0;
			entity.QuantityInPackEdit = 0;
			//entity.ValueBuyQriginal = 0.0;
			entity.InputTypeCode = "";
			entity.ImputTypeCodeFromPDA = "";
			entity.FromCatalogType = 0;
			entity.SectionNum = 0;
			entity.TypeMakat = "";
			entity.ERPIturCode = "";

			int ret = 0;
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}

			if (string.IsNullOrWhiteSpace(inValue.DocumentCode) == true)
			{
				fkCodeIsEmpty = (int)ConvertDataErrorCodeEnum.FKCodeIsEmpty;
			}
			else
			{
				entity.DocumentCode = inValue.DocumentCode;
				entity.DocumentHeaderCode = inValue.DocumentCode;
				entity.IturCode = inValue.IturCode;
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusInventProductBit = entity.StatusInventProductBit.Or(ret);
			return ret;
		}

		public static int ValidateError(this  InventProduct entity, InventProduct inValue, Dictionary<string, ProductMakat> makatDictionary)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int sameCodeExist = 0;

			int ret = 0;
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.Makat) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			if (string.IsNullOrWhiteSpace(inValue.Barcode) == true)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				string makat = inValue.Makat.Trim(' ');
				entity.Makat = "";
				entity.Barcode = inValue.Barcode.CutLength(299); 
				entity.ProductName = inValue.ProductName.CutLength(99); 
				entity.QuantityOriginal = inValue.QuantityOriginal;
				entity.QuantityEdit = inValue.QuantityEdit;
				entity.QuantityInPackEdit = inValue.QuantityInPackEdit;

				if (IsMakatUnique(makatDictionary, makat) == false)
				{
					entity.Makat = makat.CutLength(299); 
				}
				else
				{
					codeIsEmpty = (int)ConvertDataErrorCodeEnum.FKCodeIsEmpty;
				}
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty + sameCodeExist;
			return ret;
		}

		public static int ValidateWarning(this InventProduct entity, InventProductSimpleString inValue,
			DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			int ret = 0;

			if (string.IsNullOrWhiteSpace(inValue.Barcode) == true)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}

			if (string.IsNullOrWhiteSpace(inValue.Makat) == true)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}

			try
			{
				entity.Makat = inValue.Makat.CutLength(299);
				entity.Barcode = inValue.Barcode.CutLength(299); 
				entity.SerialNumber = inValue.SerialNumber;
				
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			double quantityOriginal = 0.0;
			bool yes = Double.TryParse(inValue.QuantityOriginal.Trim(), out quantityOriginal);
			if (yes == false && string.IsNullOrWhiteSpace(inValue.QuantityOriginal.Trim()) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.QuantityOriginal = quantityOriginal;
			entity.QuantityEdit = quantityOriginal;

			int quantityInPackEdit = 0;
			string strValue = inValue.QuantityInPackEdit.Trim();
			if (string.IsNullOrWhiteSpace(strValue) == false && strValue != "0")
			{
				try
				{
					quantityInPackEdit = Convert.ToInt32(strValue);
				}
				catch
				{
					double quantityInPackEdit1 = 0;
					bool yes1 = Double.TryParse(strValue, out quantityInPackEdit1);
					try
					{
						if (yes1 == true) quantityInPackEdit = Convert.ToInt32(quantityInPackEdit1);
					}
					catch
					{
						invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
					}
				}
			}
			entity.QuantityInPackEdit = quantityInPackEdit;

			try
			{
				entity.InputTypeCode = Convert.ToString(inValue.InputTypeCode.Trim());
				entity.ImputTypeCodeFromPDA = Convert.ToString(inValue.ImputTypeCodeFromPDA.Trim());

				if (entity.InputTypeCode != InputTypeCodeEnum.B.ToString()
					&& entity.InputTypeCode != InputTypeCodeEnum.K.ToString())
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}


			if (string.IsNullOrWhiteSpace(inValue.ProductName) == false)
			{
				entity.ProductName = inValue.ProductName;
			}
			else
			{
				entity.ProductName = "ProductNameIsEmpty";
			}


			try
			{
				string dt = inValue.CreateDate.Trim(" ".ToCharArray()) + " "
					+ inValue.CreateTime.Trim(" ".ToCharArray());
				DateTime dateTime = Convert.ToDateTime(dt, dtfi);
				entity.CreateDate = dateTime;
			}
			catch { entity.CreateDate = DateTime.Now; }

			entity.WorkerID = inValue.WorkerID;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusInventProductBit = entity.StatusInventProductBit.Or(ret);
			return ret;
		}


		public static int ValidateDb3Warning(this InventProduct entity, InventProductSimpleString inValue)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			int ret = 0;

			if (string.IsNullOrWhiteSpace(inValue.Barcode) == true)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}

			try
			{
				entity.Makat = inValue.Makat.CutLength(299);
				entity.Barcode = inValue.Barcode.CutLength(299); 
				entity.SerialNumber = inValue.SerialNumber;
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			double quantityOriginal = 0.0;
			bool yes = Double.TryParse(inValue.QuantityOriginal.Trim(), out quantityOriginal);
			if (yes == false && string.IsNullOrWhiteSpace(inValue.QuantityOriginal.Trim()) == false)
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			entity.QuantityOriginal = quantityOriginal;
			entity.QuantityEdit = quantityOriginal;


			int quantityInPackEdit = 0;
			string strValue = inValue.QuantityInPackEdit.Trim();
			if (string.IsNullOrWhiteSpace(strValue) == false && strValue != "0")
			{
				try
				{
					quantityInPackEdit = Convert.ToInt32(strValue);
				}
				catch
				{
					double quantityInPackEdit1 = 0;
					bool yes1 = Double.TryParse(strValue, out quantityInPackEdit1);
					try
					{
						if (yes1 == true) quantityInPackEdit = Convert.ToInt32(quantityInPackEdit1);
					}
					catch
					{
						invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
					}
				}
			}
			entity.QuantityInPackEdit = quantityInPackEdit;

			try
			{
				entity.InputTypeCode = Convert.ToString(inValue.InputTypeCode.Trim());
				entity.ImputTypeCodeFromPDA = Convert.ToString(inValue.ImputTypeCodeFromPDA.Trim());

				if (entity.InputTypeCode != InputTypeCodeEnum.B.ToString()
					&& entity.InputTypeCode != InputTypeCodeEnum.K.ToString())
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}


			//if (string.IsNullOrWhiteSpace(inValue.ProductName) == false)
			//{
			//	entity.ProductName = inValue.ProductName;
			//}
			//else
			//{
			//	entity.ProductName = "ProductNameIsEmpty";
			//}

			//entity.WorkerID = inValue.WorkerID;

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusInventProductBit = entity.StatusInventProductBit.Or(ret);
			return ret;
		}

		public static int Validate(this InventProduct entity, InventProductSimpleString inValue, DateTimeFormatInfo dtfi)
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

		public static string MakatValidate(string makat)
		{
			if (String.IsNullOrWhiteSpace(makat))
				return ValidateMessage.InventProduct.MakatIsEmpty;

			if (!Regex.IsMatch(makat, @"^[-_\d\w\s\\="".&]+$"))
				return ValidateMessage.InventProduct.MakatFormat;

			return String.Empty;
		}

		public static string QuantityEditValidate(string quantityEdit)
		{
			if (!CommonValidate.IsOkAsDouble(quantityEdit))
				return Model.ValidateMessage.InventProduct.QuantityEditFormat;

			//if (CommonValidate.DigitsAfterDecimalPoint(quantityEdit) > 3)
			//{
			//   return Model.ValidateMessage.InventProduct.QuantityEditFormat;

			//}

			return String.Empty;
		}

		public static string QuantityInPackEditValidate(string quantityInPackEdit)
		{
			if (!CommonValidate.IsOkAsInt(quantityInPackEdit))
				return Model.ValidateMessage.InventProduct.QuantityInPackEditFormat;	  // todo
			return String.Empty;
		}

		public static string QuantityOriginalValidate(string quantityOriginal)
		{
			if (!CommonValidate.IsOkAsDouble(quantityOriginal))
				return Model.ValidateMessage.InventProduct.QuantityOriginalFormat;
			return String.Empty;
		}

		private static bool IsMakatUnique(Dictionary<string, ProductMakat> makatDictionary, string makat)
		{
			bool isMakatUnique = makatDictionary.Any(r => r.Key == makat) == false;
			return isMakatUnique;
		}


		public static void SetAdvancedValue(this InventProduct newInventProduct, string[] record, Dictionary<string, int> indexInRecordDictionary, string dbPath)
		{
			int countRecord = record.Length;
			//IFieldLinkRepository fieldLinkRepository = serviceLocator.GetInstance<IFieldLinkRepository>();
			//Dictionary<string, int> IndexInRecordDictionary
			//    = fieldLinkRepository.GetIndexInRecordDictionary("InventProduct", dbPath);
			//IndexInRecordDictionary["IPValueStr1"] = 6;
			//IndexInRecordDictionary["IPValueStr2"] = 7;
			//IndexInRecordDictionary["IPValueFloat1"] = 8;
			//IndexInRecordDictionary["IPValueFloat2"] = 9;
			//IndexInRecordDictionary["IPValueBit1"] = 10;

			foreach (KeyValuePair<string, int> keyValuePair in indexInRecordDictionary)
			{
				string propertyName = keyValuePair.Key;//"IPValueStr5";
				int index = indexInRecordDictionary[propertyName];

				if (index < countRecord && index >= 0)
				{
					if (string.IsNullOrWhiteSpace(record[index]) == true) continue;
					System.Type t = typeof(InventProduct);
					PropertyInfo pi = t.GetProperty(propertyName);
					if (pi.PropertyType == typeof(string))
					{
						string val = record[index];
						if (val.Length > 100) val = record[index].Substring(0, 99);
						pi.SetValue(newInventProduct, val, null);
					}
					else if (pi.PropertyType == typeof(double))
					{
						double valDouble = 0;
						bool ret = Double.TryParse(record[index], out valDouble);
						pi.SetValue(newInventProduct, valDouble, null);
					}
					else if (pi.PropertyType == typeof(int))
					{
						int valInt = 0;
						bool ret = Int32.TryParse(record[index], out valInt);
						pi.SetValue(newInventProduct, valInt, null);
					}
					else if (pi.PropertyType == typeof(bool))
					{
						bool valBool = false;
						string val = record[index].Trim();
						if (val == "1") valBool = true;
						pi.SetValue(newInventProduct, valBool, null);
					}

				}
			}



			//if (IndexInRecordDictionary["IPValueStr2"] < countRecord)
			//{
			//    newInventProduct.IPValueStr2 = "22";
			//}
			//newInventProduct.IPValueStr2 = "2";
			//newInventProduct.IPValueStr3 = "3";
			//newInventProduct.IPValueStr4 = "4";
			//newInventProduct.IPValueStr5 = "5";
		}


		public static string GetPropertyStringValueByPropertyName(this InventProduct inventProduct, string propertyName)
		{
			string propertyValue = "";
			if (string.IsNullOrWhiteSpace(propertyName) == true) return propertyValue;
			System.Type t = typeof(InventProduct);
			PropertyInfo pi = t.GetProperty(propertyName);
			if (pi == null) return propertyValue;
			propertyValue = pi.GetValue(inventProduct, null).ToString();
			return propertyValue;
		}

		public static bool IsNotNone(this string iturCode)
		{
			if (iturCode == "none") return false;
			if (string.IsNullOrWhiteSpace(iturCode) == true) return false;
			return true;
		}


		//для дополнительным параметрам выбранным по имени полей из БД
/// <summary>
/// 
/// </summary>
		/// <param name="newInventProduct">InventProduct</param>
		/// <param name="record">row from db3</param>
		/// <param name="tableName">table in db3 (for info)</param>
		/// <param name="dictionaryColumnNumbers">/ ColumnName, ColumnNumber in table db3</param>
		/// <param name="columnNameInRecordDictionary">columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";</param>
		/// <param name="Log"></param>
		/// <param name="dbPath"></param>
		public static void SetAdvancedValueByName(this InventProduct newInventProduct,
			string[] record,
			string tableName, 
			Dictionary<string, int> dictionaryColumnNumbers,		   // ColumnName, ColumnNumber
			Dictionary<string, string> columnNameInRecordDictionary,  //columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
			ILog Log, string dbPath)
		{
			int countRecord = record.Length;
			//IFieldLinkRepository fieldLinkRepository = serviceLocator.GetInstance<IFieldLinkRepository>();
			//Dictionary<string, int> IndexInRecordDictionary
			//    = fieldLinkRepository.GetIndexInRecordDictionary("InventProduct", dbPath);
			//columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
			//columnNameInRecordDictionary["IPValueStr2"] = "PropertyStr2";
			//columnNameInRecordDictionary["IPValueFloat1"] = "PropertyStr3";
			//columnNameInRecordDictionary["IPValueFloat2"] = "PropertyStr11";

			foreach (KeyValuePair<string, string> keyValuePair in columnNameInRecordDictionary)
			{
				string propertyCount4U = keyValuePair.Key;//Key = "IPValueStr1";	   in Count4U.InventProduct 
				string columnNameMobuile4Count = columnNameInRecordDictionary[propertyCount4U]; //columnNameMobuile4Count = "IPValueStr1";				
				//	  index - i Column в таблице, найденной по ColumnName
				int index = columnNameMobuile4Count.GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);		 //index in record, record - row from 	tableName

				if (index != -1)
				{
					if (string.IsNullOrWhiteSpace(record[index]) == true) continue;
					System.Type t = typeof(InventProduct);
					PropertyInfo pi = t.GetProperty(propertyCount4U);
					if (pi.PropertyType == typeof(string))
					{
						string val = record[index];
						if (val.Length > 100) val = record[index].Substring(0, 99);
						pi.SetValue(newInventProduct, val, null);
					}
					else if (pi.PropertyType == typeof(double))
					{
						double valDouble = 0;
						bool ret = Double.TryParse(record[index], out valDouble);
						pi.SetValue(newInventProduct, valDouble, null);
					}
					else if (pi.PropertyType == typeof(int))
					{
						int valInt = 0;
						bool ret = Int32.TryParse(record[index], out valInt);
						pi.SetValue(newInventProduct, valInt, null);
					}
					else if (pi.PropertyType == typeof(bool))
					{
						bool valBool = false;
						string val = record[index].Trim();
						if (val == "1") valBool = true;
						pi.SetValue(newInventProduct, valBool, null);
					}

				}
			}
		}


		public static int GetIndexColumnByName(this string name, Dictionary<string, int> dictionaryColumnNumbers, string fromTable, ILog Log)
		{
			int indexUid = -1;
			bool ret = dictionaryColumnNumbers.TryGetValue(name, out indexUid);
			if (ret == false)
			{
				Log.Add(MessageTypeEnum.Error, String.Format("Column {0} not find in {1} Table ", name, fromTable));
				return -1;
			}
			return indexUid;
		}


		public static int GetIndexColumnByName(this string name, Dictionary<string, int> dictionaryColumnNumbers, string fromTable, int colInTableCount, ILog Log)
		{
			int indexUid = -1;
			bool ret = dictionaryColumnNumbers.TryGetValue(name, out indexUid);
			if (ret == false)
			{
				Log.Add(MessageTypeEnum.Error, String.Format("Column {0} not find in {1} Table ", name, fromTable));
				return -1;
			}
			if (colInTableCount <= indexUid) return -1;
			return indexUid;
		}
	}
}
