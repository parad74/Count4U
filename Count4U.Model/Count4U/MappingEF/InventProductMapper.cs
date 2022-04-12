using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class InventProductMapper
	{
	
		public static InventProduct ToDomainObject(this App_Data.InventProduct entity)
		{
			if (entity == null) return null;
			return new InventProduct()
			{
				ID = entity.ID,
				Makat = entity.Makat,
				Code = entity.Code,
				DocumentHeaderCode = entity.DocumentHeaderCode,
				DocumentCode = entity.DocumentCode,
				IturCode = entity.IturCode,
				Barcode = entity.Barcode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				InputTypeCode = entity.InputTypeCode != null ? entity.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				PartialPackage = entity.PartialPackage,
				QuantityDifference = Convert.ToDouble(entity.QuantityDifference),
				QuantityEdit = Convert.ToDouble(entity.QuantityEdit),
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				SerialNumber = entity.SerialNumber,
				ShelfCode = entity.ShelfCode,
				ModifyDate = entity.ModifyDate,
				ProductName = entity.ProductName,
				StatusInventProductBit = Convert.ToInt32(entity.StatusInventProductBit),
				StatusInventProductCode = entity.StatusInventProductCode != null ? entity.StatusInventProductCode : "",
				TypeMakat = entity.TypeMakat,
				IPNum = Convert.ToInt32(entity.IPNum),
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				SectionNum = Convert.ToInt32(entity.SectionNum),
				ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				DocNum = Convert.ToInt32(entity.DocNum),
				SessionNum = Convert.ToInt32(entity.SessionNum),
				SessionCode = entity.SessionCode,
				SectionCode = entity.SectionCode,
				SectionName = entity.SectionName,
				PriceBuy =  Convert.ToDouble(entity.PriceBuy),
				PriceSale =  Convert.ToDouble(entity.PriceSale),
				//IPValueStr1 = entity.IPValueStr1 != null ? entity.IPValueStr1 : "",
				//IPValueStr2 = entity.IPValueStr2 != null ? entity.IPValueStr2 : "",
				//IPValueStr3 = entity.IPValueStr3 != null ? entity.IPValueStr3 : "",
				//IPValueStr4 = entity.IPValueStr4 != null ? entity.IPValueStr4 : "",
				//IPValueStr5 = entity.IPValueStr5 != null ? entity.IPValueStr5 : "",
				//IPValueStr6 = entity.IPValueStr6 != null ? entity.IPValueStr6 : "",
				//IPValueStr7 = entity.IPValueStr7 != null ? entity.IPValueStr7 : "",
				//IPValueStr8 = entity.IPValueStr8 != null ? entity.IPValueStr8 : "",
				//IPValueStr9 = entity.IPValueStr9 != null ? entity.IPValueStr9 : "",
				//IPValueStr10 = entity.IPValueStr10 != null ? entity.IPValueStr10 : "",
				IPValueStr1 = entity.IPValueStr1 ,
				IPValueStr2 = entity.IPValueStr2,
				IPValueStr3 = entity.IPValueStr3 ,
				IPValueStr4 = entity.IPValueStr4,
				IPValueStr5 = entity.IPValueStr5 ,
				IPValueStr6 = entity.IPValueStr6,
				IPValueStr7 = entity.IPValueStr7,
				IPValueStr8 = entity.IPValueStr8,
				IPValueStr9 = entity.IPValueStr9,
				IPValueStr10 = entity.IPValueStr10,
				IPValueStr11 = entity.IPValueStr11,
				IPValueStr12 = entity.IPValueStr12,
				IPValueStr13 = entity.IPValueStr13,
				IPValueStr14 = entity.IPValueStr14,
				IPValueStr15 = entity.IPValueStr15,
				IPValueStr16 = entity.IPValueStr16,
				IPValueStr17 = entity.IPValueStr17,
				IPValueStr18 = entity.IPValueStr18,
				IPValueStr19 = entity.IPValueStr19,
				IPValueStr20 = entity.IPValueStr20,
				IPValueFloat1 = Convert.ToDouble(entity.IPValueFloat1),
				IPValueFloat2 = Convert.ToDouble(entity.IPValueFloat2),
				IPValueFloat3 = Convert.ToDouble(entity.IPValueFloat3),
				IPValueFloat4 = Convert.ToDouble(entity.IPValueFloat4),
				IPValueFloat5 = Convert.ToDouble(entity.IPValueFloat5),
				IPValueInt1 = Convert.ToInt32(entity.IPValueInt1) ,
				IPValueInt2 =  Convert.ToInt32(entity.IPValueInt2),
				IPValueInt3 =  Convert.ToInt32(entity.IPValueInt3),
				IPValueInt4 =  Convert.ToInt32(entity.IPValueInt4),
				IPValueInt5 =  Convert.ToInt32(entity.IPValueInt5),
				IPValueBit1 =  Convert.ToBoolean(entity.IPValueBit1),
				IPValueBit2 =  Convert.ToBoolean(entity.IPValueBit2),
				IPValueBit3 = Convert.ToBoolean(entity.IPValueBit3),
				IPValueBit4 = Convert.ToBoolean(entity.IPValueBit4),
				IPValueBit5 = Convert.ToBoolean(entity.IPValueBit5),
				WorkerID = entity.WorkerID,
				SupplierCode = entity.SupplierCode,
				SupplierName = entity.SupplierName,
				ItemStatus = entity.ItemStatus,
				ERPIturCode = entity.ERPIturCode,
				UnityCode = entity.UnityCode,
				LocationCode = entity.LocationCode,
				Tag = entity.Tag,
				Tag1 = entity.Tag1,
				Tag2 = entity.Tag2,
				Tag3 = entity.Tag3,
				QuantityWithoutPackEdit = Convert.ToDouble(entity.QuantityWithoutPackEdit),
				ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				ValueBuyEdit = Convert.ToDouble(entity.ValueBuyEdit),
				ValueBuyQriginal = Convert.ToDouble(entity.ValueBuyQriginal),
				ValueBuyWithoutPackEdit = Convert.ToDouble(entity.ValueBuyWithoutPackEdit),
				ValueBuyInPackEdit = Convert.ToDouble(entity.ValueBuyInPackEdit),
				ValueSaleDifference = Convert.ToDouble(entity.ValueSaleDifference),
				ValueSaleEdit = Convert.ToDouble(entity.ValueSaleEdit),
				ValueSaleQriginal = Convert.ToDouble(entity.ValueSaleQriginal),
				ValueSaleWithoutPackEdit = Convert.ToDouble(entity.ValueSaleWithoutPackEdit),
				ValueSaleInPackEdit = Convert.ToDouble(entity.ValueSaleInPackEdit)
			};
		}

	
		public static InventProduct ToSimpleDomainObject(this App_Data.InventProduct entity)
		{
			if (entity == null) return null;
			return new InventProduct()
			{
				Makat = entity.Makat,
				QuantityEdit = Convert.ToDouble(entity.QuantityEdit),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				TypeMakat = entity.TypeMakat,
			};
		}

		/// <summary>
		/// Конвертация в сущность базы данных.
		/// 
		/// Converting to database entity.
		/// </summary>
		/// <param name="domainObject">
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </param>
		/// <returns>Database entity.</returns>
		public static App_Data.InventProduct ToEntity(this InventProduct domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.InventProduct()
			{
				ID = domainObject.ID,
				Makat = domainObject.Makat.CutLength(299),
				Code = domainObject.Code.CutLength(299),
				DocumentHeaderCode = domainObject.DocumentHeaderCode,
				DocumentCode = domainObject.DocumentCode,
				IturCode = domainObject.IturCode,
				Barcode = domainObject.Barcode.CutLength(299),
				CreateDate = domainObject.CreateDate,
				InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				PartialPackage = domainObject.PartialPackage,
				QuantityDifference = domainObject.QuantityDifference,
				QuantityEdit = domainObject.QuantityEdit,
				QuantityInPackEdit = domainObject.QuantityInPackEdit,
				QuantityOriginal = domainObject.QuantityOriginal,
				SerialNumber = domainObject.SerialNumber,
				ShelfCode = domainObject.ShelfCode,
				ModifyDate = domainObject.ModifyDate,
				ProductName = domainObject.ProductName.CutLength(99),
				StatusInventProductBit = domainObject.StatusInventProductBit,
				StatusInventProductCode = domainObject.StatusInventProductCode != null ? domainObject.StatusInventProductCode : "",
				TypeMakat = domainObject.TypeMakat,
				IPNum = domainObject.IPNum,
				FromCatalogType = domainObject.FromCatalogType,
				SectionNum = domainObject.SectionNum,
				ImputTypeCodeFromPDA = domainObject.ImputTypeCodeFromPDA != null ? domainObject.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				DocNum = Convert.ToInt32(domainObject.DocNum),
				SessionNum = Convert.ToInt32(domainObject.SessionNum),
				SessionCode = domainObject.SessionCode,
				SectionCode = domainObject.SectionCode,
				SectionName = domainObject.SectionName,
				PriceBuy = domainObject.PriceBuy,
				PriceSale = domainObject.PriceSale ,
				//IPValueStr1 = domainObject.IPValueStr1 != null ? domainObject.IPValueStr1 : "",
				//IPValueStr2 = domainObject.IPValueStr2 != null ? domainObject.IPValueStr2 : "",
				//IPValueStr3 = domainObject.IPValueStr3 != null ? domainObject.IPValueStr3 : "",
				//IPValueStr4 = domainObject.IPValueStr4 != null ? domainObject.IPValueStr4 : "",
				//IPValueStr5 = domainObject.IPValueStr5 != null ? domainObject.IPValueStr5 : "",
				//IPValueStr6 = domainObject.IPValueStr6 != null ? domainObject.IPValueStr6 : "",
				//IPValueStr7 = domainObject.IPValueStr7 != null ? domainObject.IPValueStr7 : "",
				//IPValueStr8 = domainObject.IPValueStr8 != null ? domainObject.IPValueStr8 : "",
				//IPValueStr9 = domainObject.IPValueStr9 != null ? domainObject.IPValueStr9 : "",
				//IPValueStr10 = domainObject.IPValueStr10 != null ? domainObject.IPValueStr10 : "",
				IPValueStr1 = domainObject.IPValueStr1,
				IPValueStr2 = domainObject.IPValueStr2,
				IPValueStr3 = domainObject.IPValueStr3,
				IPValueStr4 = domainObject.IPValueStr4 ,
				IPValueStr5 = domainObject.IPValueStr5,
				IPValueStr6 = domainObject.IPValueStr6 ,
				IPValueStr7 = domainObject.IPValueStr7 ,
				IPValueStr8 = domainObject.IPValueStr8 ,
				IPValueStr9 = domainObject.IPValueStr9,
				IPValueStr10 = domainObject.IPValueStr10,
				IPValueStr11 = domainObject.IPValueStr11,
				IPValueStr12 = domainObject.IPValueStr12,
				IPValueStr13 = domainObject.IPValueStr13,
				IPValueStr14 = domainObject.IPValueStr14,
				IPValueStr15 = domainObject.IPValueStr15,
				IPValueStr16 = domainObject.IPValueStr16,
				IPValueStr17 = domainObject.IPValueStr17,
				IPValueStr18 = domainObject.IPValueStr18,
				IPValueStr19 = domainObject.IPValueStr19,
				IPValueStr20 = domainObject.IPValueStr20,
				IPValueFloat1 = domainObject.IPValueFloat1,
				IPValueFloat2 = domainObject.IPValueFloat2,
				IPValueFloat3 = domainObject.IPValueFloat3,
				IPValueFloat4 = domainObject.IPValueFloat4,
				IPValueFloat5 = domainObject.IPValueFloat5,
				IPValueInt1 = domainObject.IPValueInt1,
				IPValueInt2 = domainObject.IPValueInt2,
				IPValueInt3 = domainObject.IPValueInt3,
				IPValueInt4 = domainObject.IPValueInt4,
				IPValueInt5 = domainObject.IPValueInt5,
				IPValueBit1 = domainObject.IPValueBit1,
				IPValueBit2 = domainObject.IPValueBit2,
				IPValueBit3 = domainObject.IPValueBit3,
				IPValueBit4 = domainObject.IPValueBit4,
				IPValueBit5 = domainObject.IPValueBit5,
				WorkerID = domainObject.WorkerID,
				SupplierCode = domainObject.SupplierCode,
				SupplierName = domainObject.SupplierName	,
				ItemStatus = domainObject.ItemStatus,
				ERPIturCode = domainObject.ERPIturCode.CutLength(249),
				UnityCode = domainObject.UnityCode,
				LocationCode = domainObject.LocationCode,
				Tag = domainObject.Tag,
				Tag1 = domainObject.Tag1,
				Tag2 = domainObject.Tag2,
				Tag3 = domainObject.Tag3,
				QuantityWithoutPackEdit = Convert.ToDouble(domainObject.QuantityWithoutPackEdit),
				ValueBuyDifference = Convert.ToDouble(domainObject.ValueBuyDifference),
				ValueBuyEdit = Convert.ToDouble(domainObject.ValueBuyEdit),
				ValueBuyQriginal = Convert.ToDouble(domainObject.ValueBuyQriginal),
				ValueBuyWithoutPackEdit = Convert.ToDouble(domainObject.ValueBuyWithoutPackEdit),
				ValueBuyInPackEdit = Convert.ToDouble(domainObject.ValueBuyInPackEdit),
				ValueSaleDifference = Convert.ToDouble(domainObject.ValueSaleDifference),
				ValueSaleEdit = Convert.ToDouble(domainObject.ValueSaleEdit),
				ValueSaleQriginal = Convert.ToDouble(domainObject.ValueSaleQriginal),
				ValueSaleWithoutPackEdit = Convert.ToDouble(domainObject.ValueSaleWithoutPackEdit),
				ValueSaleInPackEdit = Convert.ToDouble(domainObject.ValueSaleInPackEdit)
			};
		}

		public static void ApplyChanges(this App_Data.InventProduct entity, InventProduct domainObject)
		{
			if (domainObject == null) return;
			entity.Barcode = domainObject.Barcode.CutLength(299);
			entity.Makat = domainObject.Makat.CutLength(299);
			entity.Code = domainObject.Code.CutLength(299);
			entity.CreateDate = domainObject.CreateDate;
			entity.DocumentHeaderCode = domainObject.DocumentHeaderCode;
			entity.DocumentCode = domainObject.DocumentCode;
			entity.IturCode = domainObject.IturCode;
			entity.InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString();
			entity.PartialPackage = domainObject.PartialPackage;
			entity.QuantityDifference = domainObject.QuantityDifference;
			entity.QuantityEdit = domainObject.QuantityEdit;
			entity.QuantityInPackEdit = domainObject.QuantityInPackEdit;
			entity.QuantityOriginal = domainObject.QuantityOriginal;
			entity.SerialNumber = domainObject.SerialNumber;
			entity.ShelfCode = domainObject.ShelfCode;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.ProductName = domainObject.ProductName.CutLength(99);
			entity.StatusInventProductBit = domainObject.StatusInventProductBit;
			entity.StatusInventProductCode = domainObject.StatusInventProductCode != null ? domainObject.StatusInventProductCode : "";
			entity.TypeMakat = domainObject.TypeMakat;
			entity.IPNum = domainObject.IPNum;
			entity.FromCatalogType = domainObject.FromCatalogType;
			entity.SectionNum = domainObject.SectionNum;
			entity.ImputTypeCodeFromPDA = domainObject.ImputTypeCodeFromPDA != null ? domainObject.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString();
			entity.DocNum = Convert.ToInt32(domainObject.DocNum);
			entity.SessionNum = Convert.ToInt32(domainObject.SessionNum);
			entity.SessionCode = domainObject.SessionCode;
			entity.SectionCode = domainObject.SectionCode;
			entity.SectionName = domainObject.SectionName;
			entity.PriceBuy = domainObject.PriceBuy;
			entity.PriceSale = domainObject.PriceSale;
			entity.SessionCode = domainObject.SessionCode;
			//entity.IPValueStr1 = domainObject.IPValueStr1 != null ? domainObject.IPValueStr1 : "";
			//entity.IPValueStr2 = domainObject.IPValueStr2 != null ? domainObject.IPValueStr2 : "";
			//entity.IPValueStr3 = domainObject.IPValueStr3 != null ? domainObject.IPValueStr3 : "";
			//entity.IPValueStr4 = domainObject.IPValueStr4 != null ? domainObject.IPValueStr4 : "";
			//entity.IPValueStr5 = domainObject.IPValueStr5 != null ? domainObject.IPValueStr5 : "";
			entity.IPValueStr1 = domainObject.IPValueStr1;
			entity.IPValueStr2 = domainObject.IPValueStr2;
			entity.IPValueStr3 = domainObject.IPValueStr3;
			entity.IPValueStr4 = domainObject.IPValueStr4;
			entity.IPValueStr5 = domainObject.IPValueStr5;
			entity.IPValueStr6 = domainObject.IPValueStr6;
			entity.IPValueStr7 = domainObject.IPValueStr7;
			entity.IPValueStr8 = domainObject.IPValueStr8;
			entity.IPValueStr9 = domainObject.IPValueStr9;
			entity.IPValueStr10 = domainObject.IPValueStr10;
			entity.IPValueStr11 = domainObject.IPValueStr11;
			entity.IPValueStr12 = domainObject.IPValueStr12;
			entity.IPValueStr13 = domainObject.IPValueStr13;
			entity.IPValueStr14 = domainObject.IPValueStr14;
			entity.IPValueStr15 = domainObject.IPValueStr15;
			entity.IPValueStr16 = domainObject.IPValueStr16;
			entity.IPValueStr17 = domainObject.IPValueStr17;
			entity.IPValueStr18 = domainObject.IPValueStr18;
			entity.IPValueStr19 = domainObject.IPValueStr19;
			entity.IPValueStr20 = domainObject.IPValueStr20;
			entity.IPValueFloat1 = domainObject.IPValueFloat1;
			entity.IPValueFloat2 = domainObject.IPValueFloat2;
			entity.IPValueFloat3 = domainObject.IPValueFloat3;
			entity.IPValueFloat4 = domainObject.IPValueFloat4;
			entity.IPValueFloat5 = domainObject.IPValueFloat5;
			entity.IPValueInt1 = domainObject.IPValueInt1;
			entity.IPValueInt2 = domainObject.IPValueInt2;
			entity.IPValueInt3 = domainObject.IPValueInt3;
			entity.IPValueInt4 = domainObject.IPValueInt4;
			entity.IPValueInt5 = domainObject.IPValueInt5;
			entity.IPValueBit1 = domainObject.IPValueBit1;
			entity.IPValueBit2 = domainObject.IPValueBit2;
			entity.IPValueBit3 = domainObject.IPValueBit3;
			entity.IPValueBit4 = domainObject.IPValueBit4;
			entity.IPValueBit5 = domainObject.IPValueBit5;
			entity.WorkerID = domainObject.WorkerID;
			entity.SupplierCode = domainObject.SupplierCode;
			entity.SupplierName = domainObject.SupplierName;
			entity.ItemStatus = domainObject.ItemStatus;
			entity.ERPIturCode = domainObject.ERPIturCode.CutLength(249);
			entity.UnityCode = domainObject.UnityCode;
			entity.LocationCode = domainObject.LocationCode;
			entity.Tag = domainObject.Tag;
			entity.Tag1 = domainObject.Tag1;
			entity.Tag2 = domainObject.Tag2;
			entity.Tag3 = domainObject.Tag3;
			entity.QuantityWithoutPackEdit = Convert.ToDouble(domainObject.QuantityWithoutPackEdit);
			entity.ValueBuyDifference = Convert.ToDouble(domainObject.ValueBuyDifference);
			entity.ValueBuyEdit = Convert.ToDouble(domainObject.ValueBuyEdit);
			entity.ValueBuyQriginal = Convert.ToDouble(domainObject.ValueBuyQriginal);
			entity.ValueBuyWithoutPackEdit = Convert.ToDouble(domainObject.ValueBuyWithoutPackEdit);
			entity.ValueBuyInPackEdit = Convert.ToDouble(domainObject.ValueBuyInPackEdit);
			entity.ValueSaleDifference = Convert.ToDouble(domainObject.ValueSaleDifference);
			entity.ValueSaleEdit = Convert.ToDouble(domainObject.ValueSaleEdit);
			entity.ValueSaleQriginal = Convert.ToDouble(domainObject.ValueSaleQriginal);
			entity.ValueSaleWithoutPackEdit = Convert.ToDouble(domainObject.ValueSaleWithoutPackEdit);
			entity.ValueSaleInPackEdit = Convert.ToDouble(domainObject.ValueSaleInPackEdit);
		}

		//public static IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings(this SqlCeBulkCopy sqlCeBulkCopy)
		//{
		//        yield return new SqlBulkCopyColumnMapping("Code", "Code");
		//        yield return new SqlBulkCopyColumnMapping("DocumentHeaderCode", "DocumentHeaderCode");
		//        yield return new SqlBulkCopyColumnMapping("DocumentCode", "DocumentCode");
		//        yield return new SqlBulkCopyColumnMapping("IturCode", "IturCode");
		//        yield return new SqlBulkCopyColumnMapping("Barcode", "Barcode");
		//        yield return new SqlBulkCopyColumnMapping("CreateDate", "CreateDate");
		//        yield return new SqlBulkCopyColumnMapping("InputTypeCode", "InputTypeCode");
		//        yield return new SqlBulkCopyColumnMapping("PartialPackage", "PartialPackage");
		//        yield return new SqlBulkCopyColumnMapping("QuantityDifference", "QuantityDifference");
		//        yield return new SqlBulkCopyColumnMapping("QuantityEdit", "QuantityEdit");
		//        yield return new SqlBulkCopyColumnMapping("QuantityInPackEdit", "QuantityInPackEdit");
		//        yield return new SqlBulkCopyColumnMapping("QuantityOriginal", "QuantityOriginal");
		//        yield return new SqlBulkCopyColumnMapping("SerialNumber", "SerialNumber");
		//        yield return new SqlBulkCopyColumnMapping("ShelfCode", "ShelfCode");
		//        yield return new SqlBulkCopyColumnMapping("ModifyDate", "ModifyDate");
		//        yield return new SqlBulkCopyColumnMapping("ProductName", "ProductName");
		//        yield return new SqlBulkCopyColumnMapping("StatusInventProductBit", "StatusInventProductBit");
		//        yield return new SqlBulkCopyColumnMapping("TypeMakat", "TypeMakat");
		//        yield return new SqlBulkCopyColumnMapping("IPNum", "PNum");
		//        yield return new SqlBulkCopyColumnMapping("FromCatalogType", "FromCatalogType");
		//        yield return new SqlBulkCopyColumnMapping("SectionNum", "SectionNum");
		//        yield return new SqlBulkCopyColumnMapping("ImputTypeCodeFromPDA", "ImputTypeCodeFromPDA");
		//        yield return new SqlBulkCopyColumnMapping("DocNum", "DocNum");
		//        yield return new SqlBulkCopyColumnMapping("SessionNum", "SessionNum");
		//        yield return new SqlBulkCopyColumnMapping("SessionCode", "SessionCode");
		//        yield return new SqlBulkCopyColumnMapping("SectionCode", "SectionCode");
		//        yield return new SqlBulkCopyColumnMapping("SectionName", "SectionName");
		//        yield return new SqlBulkCopyColumnMapping("PriceBuy", "PriceBuy");
		//        yield return new SqlBulkCopyColumnMapping("PriceSale", "PriceSale");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr1", "IPValueStr1");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr2", "IPValueStr2");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr3", "IPValueStr3");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr4", "IPValueStr4");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr5", "IPValueStr5");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr6", "IPValueStr6");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr7", "IPValueStr7");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr8", "IPValueStr8");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr9", "IPValueStr9");
		//        yield return new SqlBulkCopyColumnMapping("IPValueStr10", "IPValueStr10");
		//        yield return new SqlBulkCopyColumnMapping("IPValueFloat1", "IPValueFloat1");
		//        yield return new SqlBulkCopyColumnMapping("IPValueFloat2", "IPValueFloat2");
		//        yield return new SqlBulkCopyColumnMapping("IPValueFloat3", "IPValueFloat3");
		//        yield return new SqlBulkCopyColumnMapping("IPValueFloat4", "IPValueFloat4");
		//        yield return new SqlBulkCopyColumnMapping("IPValueFloat5", "IPValueFloat5");
		//        yield return new SqlBulkCopyColumnMapping("IPValueInt1", "IPValueInt1");
		//        yield return new SqlBulkCopyColumnMapping("IPValueInt2", "IPValueInt2");
		//        yield return new SqlBulkCopyColumnMapping("IPValueInt3", "IPValueInt3");
		//        yield return new SqlBulkCopyColumnMapping("IPValueInt4", "IPValueInt4");
		//        yield return new SqlBulkCopyColumnMapping("IPValueInt5", "IPValueInt5");
		//        yield return new SqlBulkCopyColumnMapping("IPValueBit1", "IPValueBit1");
		//        yield return new SqlBulkCopyColumnMapping("IPValueBit2", "IPValueBit2");
		//        yield return new SqlBulkCopyColumnMapping("IPValueBit3", "IPValueBit3");
		//        yield return new SqlBulkCopyColumnMapping("IPValueBit4", "IPValueBit4");
		//        yield return new SqlBulkCopyColumnMapping("IPValueBit5", "IPValueBit5");
		//}
  	}
}
