using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.Count4U;
using System.Threading;

namespace Count4U.Model
{
	public static class  ParserParms
	{
		public static MaskPackage GetMaskPackageFromParms(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			MaskPackage maskPackage = new MaskPackage();
			if (entities.ContainsKey(ImportProviderParmEnum.MakatMaskRecord) == true)
			{
				maskPackage.MakatMaskRecord = entities[ImportProviderParmEnum.MakatMaskRecord] as MaskRecord;
				if (maskPackage.MakatMaskRecord != null)
				{
					maskPackage.MakatMaskTemplate = MaskTemplateRepository.GetMaskTemplateDictionary()
						[maskPackage.MakatMaskRecord.MaskTemplateType];
				}
				//outputString = maskPackage.MakatMaskTemplate.FormatString(inputString, maskPackage.MakatMaskRecord.Value);
			}
			if (entities.ContainsKey(ImportProviderParmEnum.BarcodeMaskRecord) == true)
			{
				maskPackage.BarcodeMaskRecord = entities[ImportProviderParmEnum.BarcodeMaskRecord] as MaskRecord;
				if (maskPackage.BarcodeMaskRecord != null)
				{
					maskPackage.BarcodeMaskTemplate = MaskTemplateRepository.GetMaskTemplateDictionary()
						[maskPackage.BarcodeMaskRecord.MaskTemplateType];
				}
			}
			return maskPackage;
		}

		public static bool IsEqualsMaskMakatAndBarcodeFromParms(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			MaskPackage maskPackage = GetMaskPackageFromParms(entities);
			if ((maskPackage.BarcodeMaskTemplate.MaskTemplateType
				== maskPackage.MakatMaskTemplate.MaskTemplateType) 
				&& (maskPackage.BarcodeMaskRecord == maskPackage.MakatMaskRecord) )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string GetStringValueFromParm(this Dictionary<ImportProviderParmEnum, object> entities,
			ImportProviderParmEnum importProviderParmEnum)
		{
			if (entities.ContainsKey(importProviderParmEnum) == true)
			{
				string parm = "";
				try{
					parm = entities[importProviderParmEnum] as string;
				}catch{}
				
				return parm;
			}
			else return "";
		}

	
		public static bool GetBoolValueFromParm(this Dictionary<ImportProviderParmEnum, object> entities,
		ImportProviderParmEnum importProviderParmEnum)
		{
			string parm = "";
			if (entities.ContainsKey(importProviderParmEnum) == true)
			{
				try{
				 parm = entities[importProviderParmEnum] as string;
				}catch{}
			}
			if (parm == "1") return true;
			else return false;
		}

		public static int GetIntValueFromParm(this Dictionary<ImportProviderParmEnum, object> entities,
		ImportProviderParmEnum importProviderParmEnum)
		{
			int? parm = null;
			if (entities.ContainsKey(importProviderParmEnum) == true)
			{
				try{
				parm = entities[importProviderParmEnum] as int?;
				}catch{}
			}
			if (parm == null) return 0;
			else return Convert.ToInt32(parm);
		}

		

		//public static int GetInt1000ValueFromParm(this Dictionary<ImportProviderParmEnum, object> entities,
		//ImportProviderParmEnum importProviderParmEnum)
		//{
		//    int? parm = null;
		//    if (entities.ContainsKey(importProviderParmEnum) == true)
		//    {
		//        try
		//        {
		//            parm = entities[importProviderParmEnum] as int?;
		//        }
		//        catch { }
		//    }
		//    if (parm == null) return 1000;
		//    else return Convert.ToInt32(parm);
		//}

		public static double GetDoubleValueFromParm(this Dictionary<ImportProviderParmEnum, object> entities,
		ImportProviderParmEnum importProviderParmEnum)
		{
			double? parm = null;
			if (entities.ContainsKey(importProviderParmEnum) == true)
			{
				try
				{
					parm = entities[importProviderParmEnum] as double?;
				}
				catch { }
			}
			if (parm == null) return 0;
			else return Convert.ToDouble(parm);
		}

		public static Action<long> GetActionUpdateProgressFromParm(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			Action<long> parm = null;
		
			if (entities.ContainsKey(ImportProviderParmEnum.ActionUpdateProgress) == true)
			{
				parm = entities[ImportProviderParmEnum.ActionUpdateProgress] as Action<long>;
			}
			return parm;
		}

		public static CancellationToken GetCancellationTokenFromParm(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			CancellationToken parm = CancellationToken.None;
			if (entities.ContainsKey(ImportProviderParmEnum.CancellationToken) == true)
			{
				parm = (CancellationToken)entities[ImportProviderParmEnum.CancellationToken];
			}
			return parm;
		}

		public static void AddCancellationUpdate(this Dictionary<ImportProviderParmEnum, object> parms,
		Action<long> updateProgress, CancellationToken cancellationToken)
		{
			parms[ImportProviderParmEnum.ActionUpdateProgress] = updateProgress;
			parms[ImportProviderParmEnum.CancellationToken] = cancellationToken;
		}

		public static Dictionary<ImportProviderParmEnum, object> ConvertToImportProviderParmEnum(
			this Dictionary<object, object> entities)
		{
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			foreach (KeyValuePair<object, object> keyValuePair in entities)
			{
				ImportProviderParmEnum parm = (ImportProviderParmEnum)keyValuePair.Key;
				parms[parm] = keyValuePair.Value; 
			}
			return parms;
		}

		public static Dictionary<object, object> ConvertToObjectFromImportProviderParmEnum(
			this Dictionary<ImportProviderParmEnum, object> entities)
		{
			Dictionary<object, object> parms = new Dictionary<object, object>();
			foreach (KeyValuePair<ImportProviderParmEnum, object> keyValuePair in entities)
			{
				parms[keyValuePair.Key] = keyValuePair.Value;
			}
			return parms;
		}

		public static ExportFileType GetFileTypeFromParm(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			ExportFileType parm = ExportFileType.ProductCodeAndName;
			if (entities.ContainsKey(ImportProviderParmEnum.FileType) == true)
			{
				parm = (ExportFileType)entities[ImportProviderParmEnum.FileType];
			}
			return parm;
		}

	

		public static PriceCodeEnum GetPriceCodeEnumFromParm(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			PriceCodeEnum parm = PriceCodeEnum.PriceBuy;
			if (entities.ContainsKey(ImportProviderParmEnum.PriceCode) == true)
			{
				string priceCodeString = (string)entities[ImportProviderParmEnum.PriceCode];

				if (string.IsNullOrWhiteSpace(priceCodeString) == false)
				{
					PriceCodeEnum tempPriceCode = PriceCodeEnum.PriceBuy;
					bool parce = Enum.TryParse(priceCodeString, out tempPriceCode);
					if (parce == true)
					{
						parm = tempPriceCode;
					}
				}
}
			return parm;
		}

		#region   CatalogParserPoints
		//* I из ini файла с проверкой
		public static CatalogParserPoints SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
			CatalogParserPoints gazitPoints)
		{
			CatalogParserPoints ret = gazitPoints;
			ret.CatalogMinLengthIncomingRow = iniParms.SetValue(
				ImportProviderParmEnum.CatalogMinLengthIncomingRow, ret.CatalogMinLengthIncomingRow);

			ret.CatalogItemCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.CatalogItemCodeStart, ret.CatalogItemCodeStart);
			ret.CatalogItemCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.CatalogItemCodeEnd, ret.CatalogItemCodeEnd);
			ret.CatalogItemNameStart = iniParms.SetValue(
				ImportProviderParmEnum.CatalogItemNameStart, ret.CatalogItemNameStart);
			ret.CatalogItemNameEnd = iniParms.SetValue(
				ImportProviderParmEnum.CatalogItemNameEnd, ret.CatalogItemNameEnd);

			ret.CatalogPriceBuyStart = iniParms.SetValue(
				ImportProviderParmEnum.CatalogPriceBuyStart, ret.CatalogPriceBuyStart);
			ret.CatalogPriceBuyEnd = iniParms.SetValue(
				ImportProviderParmEnum.CatalogPriceBuyEnd, ret.CatalogPriceBuyEnd);
			ret.CatalogPriceSaleStart = iniParms.SetValue(
					ImportProviderParmEnum.CatalogPriceSaleStart, ret.CatalogPriceSaleStart);
			ret.CatalogPriceSaleEnd = iniParms.SetValue(
				ImportProviderParmEnum.CatalogPriceSaleEnd, ret.CatalogPriceSaleEnd);

			ret.HamarotBarcodeStart = iniParms.SetValue(
				ImportProviderParmEnum.HamarotBarcodeStart, ret.HamarotBarcodeStart);
			ret.HamarotBarcodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.HamarotBarcodeEnd, ret.HamarotBarcodeEnd);
			ret.HamarotItemCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.HamarotItemCodeStart, ret.HamarotItemCodeStart);
			ret.HamarotItemCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.HamarotItemCodeEnd, ret.HamarotItemCodeEnd);

			ret.SectionCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.SectionCodeStart, ret.SectionCodeStart);
			ret.SectionCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.SectionCodeEnd, ret.SectionCodeEnd);
			ret.SectionNameStart = iniParms.SetValue(
				ImportProviderParmEnum.SectionNameStart, ret.SectionNameStart);
			ret.SectionNameEnd = iniParms.SetValue(
				ImportProviderParmEnum.SectionNameEnd, ret.SectionNameEnd);

			ret.SupplierCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.SupplierCodeStart, ret.SupplierCodeStart);
			ret.SupplierCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.SupplierCodeEnd, ret.SupplierCodeEnd);
			ret.SupplierNameStart = iniParms.SetValue(
		ImportProviderParmEnum.SupplierNameStart, ret.SupplierNameStart);
			ret.SupplierNameEnd = iniParms.SetValue(
				ImportProviderParmEnum.SupplierNameEnd, ret.SupplierNameEnd);

			ret.UnitTypeCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.UnitTypeCodeStart, ret.UnitTypeCodeStart);
			ret.UnitTypeCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.UnitTypeCodeEnd, ret.UnitTypeCodeEnd);

			ret.FamilyCodeStart = iniParms.SetValue(
				ImportProviderParmEnum.FamilyCodeStart, ret.FamilyCodeStart);
			ret.FamilyCodeEnd = iniParms.SetValue(
				ImportProviderParmEnum.FamilyCodeEnd, ret.FamilyCodeEnd);
			ret.FamilyNameStart = iniParms.SetValue(
				ImportProviderParmEnum.FamilyNameStart, ret.FamilyNameStart);
			ret.FamilyNameEnd = iniParms.SetValue(
				ImportProviderParmEnum.FamilyNameEnd, ret.FamilyNameEnd);

   			ret.QuantityERPStart = iniParms.SetValue(
				ImportProviderParmEnum.QuantityERPStart, ret.QuantityERPStart);
			ret.QuantityERPEnd = iniParms.SetValue(
				ImportProviderParmEnum.QuantityERPEnd, ret.QuantityERPEnd);

			ret.QuantityInPackStart = iniParms.SetValue(
				ImportProviderParmEnum.QuantityInPackStart, ret.QuantityInPackStart);
			ret.QuantityInPackEnd = iniParms.SetValue(
				ImportProviderParmEnum.QuantityInPackEnd, ret.QuantityInPackEnd);

			ret.QuantityTypeStart = iniParms.SetValue(
		ImportProviderParmEnum.QuantityTypeStart, ret.QuantityTypeStart);
			ret.QuantityTypeEnd = iniParms.SetValue(
				ImportProviderParmEnum.QuantityTypeEnd, ret.QuantityTypeEnd);
			return ret;
		}

		// III+ Получение параметров у Parser - а
		public static CatalogParserPoints GetCatalogParserPointsFromParm(this Dictionary<ImportProviderParmEnum, object> entities)
		{
			CatalogParserPoints parm = new CatalogParserPoints();
			try
			{
				parm.CatalogMinLengthIncomingRow = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogMinLengthIncomingRow]);

				parm.CatalogItemCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogItemCodeStart]);
				if (parm.CatalogItemCodeStart > 0) parm.CatalogItemCodeStart--;
				parm.CatalogItemCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogItemCodeEnd]);
				parm.CatalogItemCodeEnd--;

				parm.CatalogItemNameStart = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogItemNameStart]);
				parm.CatalogItemNameStart--;
				parm.CatalogItemNameEnd = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogItemNameEnd]);
				parm.CatalogItemNameEnd--;

				parm.CatalogPriceBuyStart = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogPriceBuyStart]);
				parm.CatalogPriceBuyStart--;
				parm.CatalogPriceBuyEnd = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogPriceBuyEnd]);
				parm.CatalogPriceBuyEnd--;

				parm.CatalogPriceSaleStart = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogPriceSaleStart]);
				parm.CatalogPriceSaleStart--;
				parm.CatalogPriceSaleEnd = Convert.ToInt32(entities[ImportProviderParmEnum.CatalogPriceSaleEnd]);
				parm.CatalogPriceSaleEnd--;

				parm.HamarotBarcodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.HamarotBarcodeStart]);
				if (parm.HamarotBarcodeStart > 0) parm.HamarotBarcodeStart--;
				parm.HamarotBarcodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.HamarotBarcodeEnd]);
				parm.HamarotBarcodeEnd--;

				parm.HamarotItemCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.HamarotItemCodeStart]);
				parm.HamarotItemCodeStart--;
				parm.HamarotItemCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.HamarotItemCodeEnd]);
				parm.HamarotItemCodeEnd--;

				parm.SectionCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.SectionCodeStart]);
				parm.SectionCodeStart--;
				parm.SectionCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.SectionCodeEnd]);
				parm.SectionCodeEnd--;

				parm.SectionNameStart = Convert.ToInt32(entities[ImportProviderParmEnum.SectionNameStart]);
				parm.SectionNameStart--;
				parm.SectionNameEnd = Convert.ToInt32(entities[ImportProviderParmEnum.SectionNameEnd]);
				parm.SectionNameEnd--;

				parm.SupplierCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.SupplierCodeStart]);
				parm.SupplierCodeStart--;
				parm.SupplierCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.SupplierCodeEnd]);
				parm.SupplierCodeEnd--;

				parm.SupplierNameStart = Convert.ToInt32(entities[ImportProviderParmEnum.SupplierNameStart]);
				parm.SupplierNameStart--;
				parm.SupplierNameEnd = Convert.ToInt32(entities[ImportProviderParmEnum.SupplierNameEnd]);
				parm.SupplierNameEnd--;


				parm.FamilyCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.FamilyCodeStart]);
				parm.FamilyCodeStart--;
				parm.FamilyCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.FamilyCodeEnd]);
				parm.FamilyCodeEnd--;

				parm.FamilyNameStart = Convert.ToInt32(entities[ImportProviderParmEnum.FamilyNameStart]);
				parm.FamilyNameStart--;
				parm.FamilyNameEnd = Convert.ToInt32(entities[ImportProviderParmEnum.FamilyNameEnd]);
				parm.FamilyNameEnd--;
				

				parm.UnitTypeCodeStart = Convert.ToInt32(entities[ImportProviderParmEnum.UnitTypeCodeStart]);
				parm.UnitTypeCodeStart--;
				parm.UnitTypeCodeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.UnitTypeCodeEnd]);
				parm.UnitTypeCodeEnd--;

				parm.QuantityERPStart = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityERPStart]);
				parm.QuantityERPStart--;
				parm.QuantityERPEnd = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityERPEnd]);
				parm.QuantityERPEnd--;

				parm.QuantityInPackStart = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityInPackStart]);
				parm.QuantityInPackStart--;
				parm.QuantityInPackEnd = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityInPackEnd]);
				parm.QuantityInPackEnd--;

				parm.QuantityTypeStart = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityTypeStart]);
				parm.QuantityTypeStart--;
				parm.QuantityTypeEnd = Convert.ToInt32(entities[ImportProviderParmEnum.QuantityTypeEnd]);
				parm.QuantityTypeEnd--;

			}
			catch
			{
				return parm;
			}
			return parm;
		}

	

		//+II заполнение параметров у Provider - a
		public static void AddCatalogParserPoints(this Dictionary<ImportProviderParmEnum, object> parms, CatalogParserPoints gazitPoints){
			parms[ImportProviderParmEnum.CatalogMinLengthIncomingRow] = gazitPoints.CatalogMinLengthIncomingRow;
			parms[ImportProviderParmEnum.CatalogItemCodeStart]  = gazitPoints.CatalogItemCodeStart;
			parms[ImportProviderParmEnum.CatalogItemCodeEnd]  =  gazitPoints.CatalogItemCodeEnd;
			parms[ImportProviderParmEnum.CatalogItemNameStart]  =  gazitPoints.CatalogItemNameStart;
			parms[ImportProviderParmEnum.CatalogItemNameEnd]  =  gazitPoints.CatalogItemNameEnd;

			parms[ImportProviderParmEnum.CatalogPriceBuyStart]  =  gazitPoints.CatalogPriceBuyStart;
			parms[ImportProviderParmEnum.CatalogPriceBuyEnd]  =  gazitPoints.CatalogPriceBuyEnd;
			parms[ImportProviderParmEnum.CatalogPriceSaleStart] = gazitPoints.CatalogPriceSaleStart;
			parms[ImportProviderParmEnum.CatalogPriceSaleEnd] = gazitPoints.CatalogPriceSaleEnd;


			parms[ImportProviderParmEnum.FamilyCodeStart] = gazitPoints.FamilyCodeStart;
			parms[ImportProviderParmEnum.FamilyCodeEnd] = gazitPoints.FamilyCodeEnd;
			parms[ImportProviderParmEnum.FamilyNameStart] = gazitPoints.FamilyNameStart;
			parms[ImportProviderParmEnum.FamilyNameEnd] = gazitPoints.FamilyNameEnd;


			parms[ImportProviderParmEnum.HamarotBarcodeStart]  =  gazitPoints.HamarotBarcodeStart;
			parms[ImportProviderParmEnum.HamarotBarcodeEnd]  =  gazitPoints.HamarotBarcodeEnd;
			parms[ImportProviderParmEnum.HamarotItemCodeStart]  =  gazitPoints.HamarotItemCodeStart;
			parms[ImportProviderParmEnum.HamarotItemCodeEnd]  =  gazitPoints.HamarotItemCodeEnd;

			parms[ImportProviderParmEnum.SectionCodeStart] = gazitPoints.SectionCodeStart;
			parms[ImportProviderParmEnum.SectionCodeEnd] = gazitPoints.SectionCodeEnd;
			parms[ImportProviderParmEnum.SectionNameStart] = gazitPoints.SectionNameStart;
			parms[ImportProviderParmEnum.SectionNameEnd] = gazitPoints.SectionNameEnd;

			parms[ImportProviderParmEnum.SupplierCodeStart] = gazitPoints.SupplierCodeStart;
			parms[ImportProviderParmEnum.SupplierCodeEnd] = gazitPoints.SupplierCodeEnd;
			parms[ImportProviderParmEnum.SupplierNameStart] = gazitPoints.SupplierNameStart;
			parms[ImportProviderParmEnum.SupplierNameEnd] = gazitPoints.SupplierNameEnd;
			

			parms[ImportProviderParmEnum.UnitTypeCodeStart] = gazitPoints.UnitTypeCodeStart;
			parms[ImportProviderParmEnum.UnitTypeCodeEnd] = gazitPoints.UnitTypeCodeEnd;

			parms[ImportProviderParmEnum.QuantityERPStart] = gazitPoints.QuantityERPStart;
			parms[ImportProviderParmEnum.QuantityERPEnd] = gazitPoints.QuantityERPEnd;

			parms[ImportProviderParmEnum.QuantityInPackStart] = gazitPoints.QuantityInPackStart;
			parms[ImportProviderParmEnum.QuantityInPackEnd] = gazitPoints.QuantityInPackEnd;

			parms[ImportProviderParmEnum.QuantityTypeStart] = gazitPoints.QuantityTypeStart;
			parms[ImportProviderParmEnum.QuantityTypeEnd] = gazitPoints.QuantityTypeEnd;
		}

		#endregion   CatalogParserPoints

		#region   InventProductAdvancedField
		//+II заполнение параметров у Provider - a из объекта - для передачи парсеру
		public static void AddInventProductAdvancedField(this Dictionary<ImportProviderParmEnum, object> parms,
			InventProductAdvancedFieldIndex advancedField)
		{
			//parms[ImportProviderParmEnum.QuantityInPackEdit] = advancedField.QuantityInPackEdit;
			parms[ImportProviderParmEnum.IPValueStr1] = advancedField.IPValueStr1;
			parms[ImportProviderParmEnum.IPValueStr2] = advancedField.IPValueStr2;
			parms[ImportProviderParmEnum.IPValueStr3] = advancedField.IPValueStr3;
			parms[ImportProviderParmEnum.IPValueStr4] = advancedField.IPValueStr4;
			parms[ImportProviderParmEnum.IPValueStr5] = advancedField.IPValueStr5;
			parms[ImportProviderParmEnum.IPValueStr6] = advancedField.IPValueStr6;
			parms[ImportProviderParmEnum.IPValueStr7] = advancedField.IPValueStr7;
			parms[ImportProviderParmEnum.IPValueStr8] = advancedField.IPValueStr8;
			parms[ImportProviderParmEnum.IPValueStr9] = advancedField.IPValueStr9;
			parms[ImportProviderParmEnum.IPValueStr10] = advancedField.IPValueStr10;
			parms[ImportProviderParmEnum.IPValueStr11] = advancedField.IPValueStr11;
			parms[ImportProviderParmEnum.IPValueStr12] = advancedField.IPValueStr12;
			parms[ImportProviderParmEnum.IPValueStr13] = advancedField.IPValueStr13;
			parms[ImportProviderParmEnum.IPValueStr14] = advancedField.IPValueStr14;
			parms[ImportProviderParmEnum.IPValueStr15] = advancedField.IPValueStr15;
			parms[ImportProviderParmEnum.IPValueStr16] = advancedField.IPValueStr16;
			parms[ImportProviderParmEnum.IPValueStr17] = advancedField.IPValueStr17;
			parms[ImportProviderParmEnum.IPValueStr18] = advancedField.IPValueStr18;
			parms[ImportProviderParmEnum.IPValueStr19] = advancedField.IPValueStr19;
			parms[ImportProviderParmEnum.IPValueStr20] = advancedField.IPValueStr20;
			parms[ImportProviderParmEnum.IPValueFloat1] = advancedField.IPValueFloat1;
			parms[ImportProviderParmEnum.IPValueFloat2] = advancedField.IPValueFloat2;
			parms[ImportProviderParmEnum.IPValueFloat3] = advancedField.IPValueFloat3;
			parms[ImportProviderParmEnum.IPValueFloat4] = advancedField.IPValueFloat4;
			parms[ImportProviderParmEnum.IPValueFloat5] = advancedField.IPValueFloat5;
			parms[ImportProviderParmEnum.IPValueInt1] = advancedField.IPValueInt1;
			parms[ImportProviderParmEnum.IPValueInt2] = advancedField.IPValueInt2;
			parms[ImportProviderParmEnum.IPValueInt3] = advancedField.IPValueInt3;
			parms[ImportProviderParmEnum.IPValueInt4] = advancedField.IPValueInt4;
			parms[ImportProviderParmEnum.IPValueInt5] = advancedField.IPValueInt5;
			parms[ImportProviderParmEnum.IPValueBit1] = advancedField.IPValueBit1;
			parms[ImportProviderParmEnum.IPValueBit2] = advancedField.IPValueBit2;
			parms[ImportProviderParmEnum.IPValueBit3] = advancedField.IPValueBit3;
			parms[ImportProviderParmEnum.IPValueBit4] = advancedField.IPValueBit4;
			parms[ImportProviderParmEnum.IPValueBit5] = advancedField.IPValueBit5;
		}

		//+II заполнение параметров у Provider - a из объекта - для передачи парсеру
		//Тоже самое для string имен полей в таблице db3
		public static void AddInventProductAdvancedField(this Dictionary<ImportProviderParmEnum, object> parms,
			InventProductAdvancedFieldName advancedField)
		{
			//parms[ImportProviderParmEnum.QuantityInPackEdit] = advancedField.QuantityInPackEdit;
			parms[ImportProviderParmEnum.IPValueStr1] = advancedField.IPValueStr1;
			parms[ImportProviderParmEnum.IPValueStr2] = advancedField.IPValueStr2;
			parms[ImportProviderParmEnum.IPValueStr3] = advancedField.IPValueStr3;
			parms[ImportProviderParmEnum.IPValueStr4] = advancedField.IPValueStr4;
			parms[ImportProviderParmEnum.IPValueStr5] = advancedField.IPValueStr5;
			parms[ImportProviderParmEnum.IPValueStr6] = advancedField.IPValueStr6;
			parms[ImportProviderParmEnum.IPValueStr7] = advancedField.IPValueStr7;
			parms[ImportProviderParmEnum.IPValueStr8] = advancedField.IPValueStr8;
			parms[ImportProviderParmEnum.IPValueStr9] = advancedField.IPValueStr9;
			parms[ImportProviderParmEnum.IPValueStr10] = advancedField.IPValueStr10;
			parms[ImportProviderParmEnum.IPValueStr11] = advancedField.IPValueStr11;
			parms[ImportProviderParmEnum.IPValueStr12] = advancedField.IPValueStr12;
			parms[ImportProviderParmEnum.IPValueStr13] = advancedField.IPValueStr13;
			parms[ImportProviderParmEnum.IPValueStr14] = advancedField.IPValueStr14;
			parms[ImportProviderParmEnum.IPValueStr15] = advancedField.IPValueStr15;
			parms[ImportProviderParmEnum.IPValueStr16] = advancedField.IPValueStr16;
			parms[ImportProviderParmEnum.IPValueStr17] = advancedField.IPValueStr17;
			parms[ImportProviderParmEnum.IPValueStr18] = advancedField.IPValueStr18;
			parms[ImportProviderParmEnum.IPValueStr19] = advancedField.IPValueStr19;
			parms[ImportProviderParmEnum.IPValueStr20] = advancedField.IPValueStr20;
			parms[ImportProviderParmEnum.IPValueFloat1] = advancedField.IPValueFloat1;
			parms[ImportProviderParmEnum.IPValueFloat2] = advancedField.IPValueFloat2;
			parms[ImportProviderParmEnum.IPValueFloat3] = advancedField.IPValueFloat3;
			parms[ImportProviderParmEnum.IPValueFloat4] = advancedField.IPValueFloat4;
			parms[ImportProviderParmEnum.IPValueFloat5] = advancedField.IPValueFloat5;
			parms[ImportProviderParmEnum.IPValueInt1] = advancedField.IPValueInt1;
			parms[ImportProviderParmEnum.IPValueInt2] = advancedField.IPValueInt2;
			parms[ImportProviderParmEnum.IPValueInt3] = advancedField.IPValueInt3;
			parms[ImportProviderParmEnum.IPValueInt4] = advancedField.IPValueInt4;
			parms[ImportProviderParmEnum.IPValueInt5] = advancedField.IPValueInt5;
			parms[ImportProviderParmEnum.IPValueBit1] = advancedField.IPValueBit1;
			parms[ImportProviderParmEnum.IPValueBit2] = advancedField.IPValueBit2;
			parms[ImportProviderParmEnum.IPValueBit3] = advancedField.IPValueBit3;
			parms[ImportProviderParmEnum.IPValueBit4] = advancedField.IPValueBit4;
			parms[ImportProviderParmEnum.IPValueBit5] = advancedField.IPValueBit5;
		}

		//* I из ini файла с проверкой 
		public static InventProductAdvancedFieldIndex SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
			InventProductAdvancedFieldIndex advancedField)
		{
			InventProductAdvancedFieldIndex ret = advancedField;
			//ret.QuantityInPackEdit = iniParms.SetValue(ImportProviderParmEnum.QuantityInPackEdit, ret.QuantityInPackEdit);
			ret.IPValueStr1 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr1, ret.IPValueStr1);
			ret.IPValueStr2 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr2, ret.IPValueStr2);
			ret.IPValueStr3 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr3, ret.IPValueStr3);
			ret.IPValueStr4 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr4, ret.IPValueStr4);
			ret.IPValueStr5 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr5, ret.IPValueStr5);
			ret.IPValueStr6 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr6, ret.IPValueStr6);
			ret.IPValueStr7 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr7, ret.IPValueStr7);
			ret.IPValueStr8 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr8, ret.IPValueStr8);
			ret.IPValueStr9 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr9, ret.IPValueStr9);
			ret.IPValueStr10 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr10, ret.IPValueStr10);
			ret.IPValueStr11 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr11, ret.IPValueStr11);
			ret.IPValueStr12 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr12, ret.IPValueStr12);
			ret.IPValueStr13 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr13, ret.IPValueStr13);
			ret.IPValueStr14 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr14, ret.IPValueStr14);
			ret.IPValueStr15 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr15, ret.IPValueStr15);
			ret.IPValueStr16 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr16, ret.IPValueStr16);
			ret.IPValueStr17 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr17, ret.IPValueStr17);
			ret.IPValueStr18 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr18, ret.IPValueStr18);
			ret.IPValueStr19 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr19, ret.IPValueStr19);
			ret.IPValueStr20 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr20, ret.IPValueStr20);
			ret.IPValueFloat1 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat1, ret.IPValueFloat1);
			ret.IPValueFloat2 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat2, ret.IPValueFloat2);
			ret.IPValueFloat3 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat3, ret.IPValueFloat3);
			ret.IPValueFloat4 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat4, ret.IPValueFloat4);
			ret.IPValueFloat5 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat5, ret.IPValueFloat5);
			ret.IPValueInt1 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt1, ret.IPValueInt1);
			ret.IPValueInt2 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt2, ret.IPValueInt2);
			ret.IPValueInt3 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt3, ret.IPValueInt3);
			ret.IPValueInt4 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt4, ret.IPValueInt4);
			ret.IPValueInt5 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt5, ret.IPValueInt5);
			ret.IPValueBit1 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit1, ret.IPValueBit1);
			ret.IPValueBit2 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit2, ret.IPValueBit2);
			ret.IPValueBit3 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit3, ret.IPValueBit3);
			ret.IPValueBit4 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit4, ret.IPValueBit4);
			ret.IPValueBit5 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit5, ret.IPValueBit5);
			return ret;
		}

		public static void AddIPAdvancedFieldIndex(
			this Dictionary<string, int> fieldIndexDictionary, Dictionary<ImportProviderParmEnum, object> entities, 
			ImportProviderParmEnum IPParmIndex)
		{
			int index= entities.GetIntValueFromParm(IPParmIndex);
			if (index != 0) { fieldIndexDictionary[IPParmIndex.ToString()] = entities.GetIntValueFromParm(IPParmIndex); }
		}

		//* I из ini файла с проверкой 
		// то же для строк-ключей	 NAME
		public static InventProductAdvancedFieldName SetValueName(this Dictionary<ImportProviderParmEnum, string> iniParms,
		InventProductAdvancedFieldName advancedField)
		{
			InventProductAdvancedFieldName ret = advancedField;
			//ret.QuantityInPackEdit = iniParms.SetValue(ImportProviderParmEnum.QuantityInPackEdit, ret.QuantityInPackEdit);
			ret.IPValueStr1 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr1, ret.IPValueStr1);
			ret.IPValueStr2 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr2, ret.IPValueStr2);
			ret.IPValueStr3 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr3, ret.IPValueStr3);
			ret.IPValueStr4 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr4, ret.IPValueStr4);
			ret.IPValueStr5 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr5, ret.IPValueStr5);
			ret.IPValueStr6 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr6, ret.IPValueStr6);
			ret.IPValueStr7 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr7, ret.IPValueStr7);
			ret.IPValueStr8 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr8, ret.IPValueStr8);
			ret.IPValueStr9 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr9, ret.IPValueStr9);
			ret.IPValueStr10 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr10, ret.IPValueStr10);
			ret.IPValueStr11 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr11, ret.IPValueStr11);
			ret.IPValueStr12 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr12, ret.IPValueStr12);
			ret.IPValueStr13 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr13, ret.IPValueStr13);
			ret.IPValueStr14 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr14, ret.IPValueStr14);
			ret.IPValueStr15 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr15, ret.IPValueStr15);
			ret.IPValueStr16 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr16, ret.IPValueStr16);
			ret.IPValueStr17 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr17, ret.IPValueStr17);
			ret.IPValueStr18 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr18, ret.IPValueStr18);
			ret.IPValueStr19 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr19, ret.IPValueStr19);
			ret.IPValueStr20 = iniParms.SetValue(ImportProviderParmEnum.IPValueStr20, ret.IPValueStr20);
			ret.IPValueFloat1 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat1, ret.IPValueFloat1);
			ret.IPValueFloat2 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat2, ret.IPValueFloat2);
			ret.IPValueFloat3 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat3, ret.IPValueFloat3);
			ret.IPValueFloat4 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat4, ret.IPValueFloat4);
			ret.IPValueFloat5 = iniParms.SetValue(ImportProviderParmEnum.IPValueFloat5, ret.IPValueFloat5);
			ret.IPValueInt1 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt1, ret.IPValueInt1);
			ret.IPValueInt2 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt2, ret.IPValueInt2);
			ret.IPValueInt3 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt3, ret.IPValueInt3);
			ret.IPValueInt4 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt4, ret.IPValueInt4);
			ret.IPValueInt5 = iniParms.SetValue(ImportProviderParmEnum.IPValueInt5, ret.IPValueInt5);
			ret.IPValueBit1 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit1, ret.IPValueBit1);
			ret.IPValueBit2 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit2, ret.IPValueBit2);
			ret.IPValueBit3 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit3, ret.IPValueBit3);
			ret.IPValueBit4 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit4, ret.IPValueBit4);
			ret.IPValueBit5 = iniParms.SetValue(ImportProviderParmEnum.IPValueBit5, ret.IPValueBit5);
			return ret;
		}

		public static void AddIPAdvancedFieldName(
			this Dictionary<string, string> fieldNameDictionary, Dictionary<ImportProviderParmEnum, object> entities,
			ImportProviderParmEnum IPParmName)
		{
			string key = entities.GetStringValueFromParm(IPParmName);
			if (string.IsNullOrWhiteSpace(key) == false) { fieldNameDictionary[IPParmName.ToString()] = entities.GetStringValueFromParm(IPParmName); }
		}


		// III+ Получение параметров у Parser - а
		public static Dictionary<string, int> GetIPAdvancedFieldIndexDictionaryFromParm(
			this Dictionary<ImportProviderParmEnum, object> entities)
		{
			Dictionary<string, int> inventProductAdvancedFieldIndexDictionary = new Dictionary<string, int>();
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.QuantityInPackEdit);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr1);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr2);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr3);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr4);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr5);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr6);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr7);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr8);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr9);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr10);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr11);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr12);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr13);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr14);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr15);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr16);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr17);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr18);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr19);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueStr20);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueFloat1);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueFloat2);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueFloat3);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueFloat4);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueFloat5);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueInt1);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueInt2);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueInt3);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueInt4);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueInt5);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueBit1);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueBit2);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueBit3);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueBit4);
			inventProductAdvancedFieldIndexDictionary.AddIPAdvancedFieldIndex(entities, ImportProviderParmEnum.IPValueBit5);
	
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.QuantityInPackEdit.ToString()] =
			// entities.GetIntValueFromParm(ImportProviderParmEnum.QuantityInPackEdit);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr1.ToString()] =
			// entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr1);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr2.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr2);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr3.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr3);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr4.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr4);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr5.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr5);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr6.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr6);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr7.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr7);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr8.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr8);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr9.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr9);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueStr10.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr10);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueFloat1.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat1);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueFloat2.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat2);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueFloat3.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat3);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueFloat4.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat4);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueFloat5.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat5);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueInt1.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt1);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueInt2.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt2);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueInt3.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt3);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueInt4.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt4);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueInt5.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt5);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueBit1.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit1);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueBit2.ToString()] =
			//  entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit2);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueBit3.ToString()] =
			//   entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit3);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueBit4.ToString()] =
			//   entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit4);
			//InventProductAdvancedFieldIndexDictionary[ImportProviderParmEnum.IPValueBit5.ToString()] =
			//   entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit5);

			//InventProductAdvancedFieldIndex parm = new InventProductAdvancedFieldIndex();
			//try
			//{
			//    parm.QuantityInPackEdit = entities.GetIntValueFromParm(ImportProviderParmEnum.QuantityInPackEdit);
			//    parm.IPValueStr1 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr1);
			//    parm.IPValueStr2 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr2);
			//    parm.IPValueStr3 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr3);
			//    parm.IPValueStr4 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr4);
			//    parm.IPValueStr5 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr5);
			//    parm.IPValueStr6 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr6);
			//    parm.IPValueStr7 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr7);
			//    parm.IPValueStr8 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr8);
			//    parm.IPValueStr9 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr9);
			//    parm.IPValueStr10 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueStr10);
			//    parm.IPValueFloat1 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat1);
			//    parm.IPValueFloat2 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat2);
			//    parm.IPValueFloat3 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat3);
			//    parm.IPValueFloat4 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat4);
			//    parm.IPValueFloat5 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueFloat5);
			//    parm.IPValueInt1 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt1);
			//    parm.IPValueInt2 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt2);
			//    parm.IPValueInt3 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt3);
			//    parm.IPValueInt4 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt4);
			//    parm.IPValueInt5 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueInt5);
			//    parm.IPValueBit1 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit1);
			//    parm.IPValueBit2 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit2);
			//    parm.IPValueBit3 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit3);
			//    parm.IPValueBit4 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit4);
			//    parm.IPValueBit5 = entities.GetIntValueFromParm(ImportProviderParmEnum.IPValueBit5);
			//}
			//catch
			//{
			//    return parm;
			//}
			return inventProductAdvancedFieldIndexDictionary;
		}

		//	//columnNameInRecordDictionary["IPValueStr1"] = "PropertyStr1";
		// III+ Получение параметров у Parser - а		 то же самое про   string ключи по именам полей в таблице db3
		public static Dictionary<string, string> GetIPAdvancedFieldNameDictionaryFromParm(
			this Dictionary<ImportProviderParmEnum, object> entities)
		{
			Dictionary<string, string> inventProductAdvancedFieldNameDictionary = new Dictionary<string, string>();
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.QuantityInPackEdit);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr1);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr2);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr3);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr4);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr5);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr6);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr7);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr8);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr9);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr10);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr11);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr12);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr13);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr14);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr15);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr16);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr17);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr18);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr19);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueStr20);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueFloat1);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueFloat2);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueFloat3);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueFloat4);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueFloat5);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueInt1);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueInt2);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueInt3);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueInt4);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueInt5);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueBit1);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueBit2);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueBit3);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueBit4);
			inventProductAdvancedFieldNameDictionary.AddIPAdvancedFieldName(entities, ImportProviderParmEnum.IPValueBit5);

			return inventProductAdvancedFieldNameDictionary;
		}
		#endregion   InventProductAdvancedField

		 #region   SetValue
		public static bool SetInvertValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
			ImportProviderParmEnum importProviderParm, bool parm)
		{
			bool ret = parm;
			if (iniParms.ContainsKey(importProviderParm) == true)
			{
				if (iniParms[importProviderParm] == "1") return true;
				if (iniParms[importProviderParm] == "0") return false;
			}
			return ret;
		}

		public static Encoding SetEncodingValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
		ImportProviderParmEnum importProviderParm, Encoding parmEncoding)
		{
			Encoding retEncoding = parmEncoding;
			if (iniParms.ContainsKey(ImportProviderParmEnum.Encoding) == true)
			{
				string value = iniParms[ImportProviderParmEnum.Encoding];
				if (String.IsNullOrWhiteSpace(value) == false)
				{
					retEncoding = StringToEncoding(value);
				}

				if (retEncoding == null)
				{
					retEncoding = parmEncoding;
				}
				if (retEncoding == null)
				{
					retEncoding = Encoding.GetEncoding(1255);
				}
			}
			return retEncoding;
		}

	   	public static string SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
		ImportProviderParmEnum importProviderParm, string parm)
		{
			string ret = parm;
			if (iniParms.ContainsKey(importProviderParm) == true)
			{
				ret = iniParms[importProviderParm] == string.Empty ? ret : iniParms[importProviderParm];
			}
			return ret;
		}

		public static int SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
			ImportProviderParmEnum importProviderParm, int parm)
		{
			int ret = parm;
			int outRet = 0;
			if (iniParms.ContainsKey(importProviderParm) == true )
			{
				if (Int32.TryParse(iniParms[importProviderParm], out outRet) == true)
				{
					if (outRet == 0) return ret;
					else return outRet;
				}
			}
			return ret;
		}

		public static double SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
			ImportProviderParmEnum importProviderParm, double parm)
		{
			double ret = parm;
			double outRet = 0;
			if (iniParms.ContainsKey(importProviderParm) == true)
			{
				if (Double.TryParse(iniParms[importProviderParm], out outRet) == true)
				{
					if (outRet == 0) return ret;
					else return outRet;
				}
			}
			return ret;
		}

		public static bool SetValue(this Dictionary<ImportProviderParmEnum, string> iniParms,
		ImportProviderParmEnum importProviderParm, bool parm)
		{
			bool ret = parm;
			if (iniParms.ContainsKey(importProviderParm) == true)
			{
				if (iniParms[importProviderParm] == "1") return true;
				if (iniParms[importProviderParm] == "0") return false;
			}
			return ret;
		}

	
	
	
		#endregion   SetValue

		public static Encoding StringToEncoding(string str)
		{
			if (String.IsNullOrEmpty(str))
				return null;

			Encoding result = null;

			int n;
			if (Int32.TryParse(str, out n))
			{
				try
				{
					result = Encoding.GetEncoding(n);
				}
				catch (Exception)
				{
					result = null;
				}
			}

			if (result == null)
			{
				try
				{
					result = Encoding.GetEncoding(str);
				}
				catch (Exception)
				{
					result = null;
				}
			}

			return result;
		}
	}

	   

	public class CatalogParserPoints
	{
		public int CatalogMinLengthIncomingRow { get; set; }

		public int CatalogItemCodeStart { get; set; }
		public int CatalogItemCodeEnd { get; set; }

		public int CatalogItemNameStart { get; set; }
		public int CatalogItemNameEnd { get; set; }

		public int CatalogPriceBuyStart { get; set; }
		public int CatalogPriceBuyEnd { get; set; }

		public int CatalogPriceSaleStart { get; set; }
		public int CatalogPriceSaleEnd { get; set; }

		public int HamarotBarcodeStart { get; set; }
		public int HamarotBarcodeEnd { get; set; }

		public int HamarotItemCodeStart { get; set; }
		public int HamarotItemCodeEnd { get; set; }

		public int SectionCodeStart { get; set; }
		public int SectionCodeEnd { get; set; }

		public int SectionNameStart { get; set; }
		public int SectionNameEnd { get; set; }

		public int UnitTypeCodeStart { get; set; }
		public int UnitTypeCodeEnd { get; set; }

		public int QuantityERPStart { get; set; }
		public int QuantityERPEnd { get; set; }

		public int QuantityInPackStart { get; set; }
		public int QuantityInPackEnd { get; set; }

		public int QuantityTypeStart { get; set; }
		public int QuantityTypeEnd { get; set; }

		public int SupplierCodeStart { get; set; }
		public int SupplierCodeEnd { get; set; }

		public int SupplierNameStart { get; set; }
		public int SupplierNameEnd { get; set; }

		public int FamilyCodeStart { get; set; }
		public int FamilyCodeEnd { get; set; }

		public int FamilyNameStart { get; set; }
		public int FamilyNameEnd { get; set; }

	}

	public class InventProductAdvancedFieldIndex
	{
		//public int QuantityInPackEdit { get; set; }
		public int IPValueStr1 { get; set; }
		public int IPValueStr2 { get; set; }
		public int IPValueStr3 { get; set; }
		public int IPValueStr4 { get; set; }
		public int IPValueStr5 { get; set; }
		public int IPValueStr6 { get; set; }
		public int IPValueStr7 { get; set; }
		public int IPValueStr8 { get; set; }
		public int IPValueStr9 { get; set; }
		public int IPValueStr10 { get; set; }
		public int IPValueStr11 { get; set; }
		public int IPValueStr12 { get; set; }
		public int IPValueStr13 { get; set; }
		public int IPValueStr14 { get; set; }
		public int IPValueStr15 { get; set; }
		public int IPValueStr16 { get; set; }
		public int IPValueStr17 { get; set; }
		public int IPValueStr18 { get; set; }
		public int IPValueStr19 { get; set; }
		public int IPValueStr20 { get; set; }
		public int IPValueFloat1 { get; set; }
		public int IPValueFloat2 { get; set; }
		public int IPValueFloat3 { get; set; }
		public int IPValueFloat4 { get; set; }
		public int IPValueFloat5 { get; set; }
		public int IPValueInt1 { get; set; }
		public int IPValueInt2 { get; set; }
		public int IPValueInt3 { get; set; }
		public int IPValueInt4 { get; set; }
		public int IPValueInt5 { get; set; }
		public int IPValueBit1 { get; set; }
		public int IPValueBit2 { get; set; }
		public int IPValueBit3 { get; set; }
		public int IPValueBit4 { get; set; }
		public int IPValueBit5 { get; set; }	
	}

	public class InventProductAdvancedFieldName
	{
		//public int QuantityInPackEdit { get; set; }
		public string IPValueStr1 { get; set; }
		public string IPValueStr2 { get; set; }
		public string IPValueStr3 { get; set; }
		public string IPValueStr4 { get; set; }
		public string IPValueStr5 { get; set; }
		public string IPValueStr6 { get; set; }
		public string IPValueStr7 { get; set; }
		public string IPValueStr8 { get; set; }
		public string IPValueStr9 { get; set; }
		public string IPValueStr10 { get; set; }
		public string IPValueStr11 { get; set; }
		public string IPValueStr12 { get; set; }
		public string IPValueStr13 { get; set; }
		public string IPValueStr14 { get; set; }
		public string IPValueStr15 { get; set; }
		public string IPValueStr16 { get; set; }
		public string IPValueStr17 { get; set; }
		public string IPValueStr18 { get; set; }
		public string IPValueStr19 { get; set; }
		public string IPValueStr20 { get; set; }
		public string IPValueFloat1 { get; set; }
		public string IPValueFloat2 { get; set; }
		public string IPValueFloat3 { get; set; }
		public string IPValueFloat4 { get; set; }
		public string IPValueFloat5 { get; set; }
		public string IPValueInt1 { get; set; }
		public string IPValueInt2 { get; set; }
		public string IPValueInt3 { get; set; }
		public string IPValueInt4 { get; set; }
		public string IPValueInt5 { get; set; }
		public string IPValueBit1 { get; set; }
		public string IPValueBit2 { get; set; }
		public string IPValueBit3 { get; set; }
		public string IPValueBit4 { get; set; }
		public string IPValueBit5 { get; set; }
	}

	public enum ExportFileType
	{
		NoLookupFile = 0,
		OnlyProductCode = 1,
		ProductCodeAndName = 2,
		OnlyBarcodes = 3,
		ProductCodeAndNameAndSupplierCode = 4,
		ProductCodeAndFamilyNameAndFamilyColor = 5,
		ProductCodeAndNameAndUnitType = 6,
		ProductCodeAndNameAndUnitNameAndSerial = 7,
		OnlyMakats = 8,
		OnlyBarcodesAndName = 9,
		ProductCodeAndNameAndQuantityInPackAndUnitType = 10,
		MakatAndNameAndListBarcode = 11
	}

	public static class ConfigIniFileType
	{
		public static int ConvertExportConfigIniFileType2FileTypeInt(ExportFileType fileTypeEnum)
		{
			int FileTypeInt = 1;

			if (fileTypeEnum == ExportFileType.NoLookupFile)
			{
				FileTypeInt = 0;
			}
			else if (fileTypeEnum == ExportFileType.OnlyBarcodes)
			{
				FileTypeInt = 1;
			}
			else if (fileTypeEnum == ExportFileType.OnlyBarcodesAndName)
			{
				FileTypeInt = 2;
			}
			else if (fileTypeEnum == ExportFileType.OnlyMakats)
			{
				FileTypeInt = 1;
			}
			else if (fileTypeEnum == ExportFileType.MakatAndNameAndListBarcode)
			{
				FileTypeInt = 1;
			}
			else if (fileTypeEnum == ExportFileType.OnlyProductCode)
			{
				FileTypeInt = 1;
			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndName)
			{
				// string[] newRows = new string[]{ makat, productName };
				FileTypeInt = 2;
			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndNameAndSupplierCode)
			{
				//string[] newRows = new string[] { makat, productName, product.SupplierCode };
				FileTypeInt = 2;

			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndFamilyNameAndFamilyColor)
			{
				//string[] newRows = new string[] { makat, familyType + " " + familyColor };
				FileTypeInt = 2;
			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndNameAndUnitType)
			{
				//string[] newRows = new string[] { makat, productName,  product.UnitTypeCode  };
				FileTypeInt = 2;
			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndNameAndUnitNameAndSerial)
			{
				//string[] newRows = new string[] { makat, productName,  product.UnitTypeCode  };
				FileTypeInt = 2;
			}
			else if (fileTypeEnum == ExportFileType.ProductCodeAndNameAndQuantityInPackAndUnitType)
			{
				//string[] newRows = new string[] { makat, productName,  product.UnitTypeCode  };
				FileTypeInt = 2;
			}
				
			else
			{
				//string[] newRows = new string[] { makat }; 
				FileTypeInt = 1;
			}
			return FileTypeInt;
		}
	}
}
