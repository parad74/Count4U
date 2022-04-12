using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class IturAnalyzesMapper
	{
	
		public static IturAnalyzes ToDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			return new IturAnalyzes()
			{
				ID = entity.ID,
				Itur_Disabled = entity.Itur_Disabled,
				Itur_LocationCode = entity.Itur_LocationCode,
				Itur_Number = Convert.ToInt32(entity.Itur_Number),
				Itur_Publishe = entity.Itur_Publishe,
				Itur_StatusIturBit = Convert.ToInt32(entity.Itur_StatusIturBit),
				Itur_StatusIturGroupBit = Convert.ToInt32(entity.Itur_StatusIturGroupBit),
				Itur_NumberPrefix = entity.Itur_NumberPrefix,
				Itur_NumberSufix = entity.Itur_NumberSufix,
				Itur_StatusDocHeaderBit = Convert.ToInt32(entity.Itur_StatusDocHeaderBit),
				Doc_Name = entity.Doc_Name,
				Doc_Approve = entity.Doc_Approve,
				Doc_WorkerGUID = entity.Doc_WorkerGUID,
				Doc_StatusDocHeaderBit = Convert.ToInt32(entity.Doc_StatusDocHeaderBit),
				Doc_StatusInventProductBit = Convert.ToInt32(entity.Doc_StatusInventProductBit),
				Doc_StatusApproveBit = Convert.ToInt32(entity.Doc_StatusApproveBit),
				Makat = entity.Makat,
				InputTypeCode = entity.InputTypeCode != null ? entity.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				Barcode = entity.Barcode,
				ModifyDate = entity.ModifyDate,
				QuantityDifference = Convert.ToDouble(entity.QuantityDifference),
				QuantityEdit = Convert.ToDouble(entity.QuantityEdit),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				SerialNumber = entity.SerialNumber,
				ShelfCode = entity.ShelfCode,
				ProductName = entity.ProductName,
				PDA_StatusInventProductBit =  Convert.ToInt32(entity.PDA_StatusInventProductBit),
				Code = entity.Code,
				LocationCode = entity.LocationCode,
				DocumentHeaderCode = entity.DocumentHeaderCode,
				DocumentCode = entity.DocumentCode,
				IturCode = entity.IturCode,
				ERPIturCode = entity.ERPIturCode != null ? entity.ERPIturCode : "",
				BarcodeOriginal = entity.BarcodeOriginal,
				MakatOriginal = entity.MakatOriginal,
				PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				Price = Convert.ToDouble(entity.Price),
				PriceExtra = Convert.ToDouble(entity.PriceExtra),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				SectionNum = Convert.ToInt32(entity.SectionNum),
				SectionCode = entity.SectionCode,
				SectionName = entity.SectionName,
				TypeCode = entity.TypeCode,
				ERPType =  Convert.ToInt32(entity.ERPType),
				DocNum = Convert.ToInt32(entity.DocNum),
				IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				ValueBuyEdit = Convert.ToDouble(entity.ValueBuyEdit),
				ValueBuyQriginal = Convert.ToDouble(entity.ValueBuyQriginal),
				//PriceExtra = entity.PriceExtra
				PDA_ID = Convert.ToInt64(entity.PDA_ID),
				Count = Convert.ToInt32(entity.Count),
				ValueChar = entity.ValueChar,
				ValueInt = Convert.ToInt32(entity.ValueInt),
				ValueFloat = Convert.ToDouble(entity.ValueFloat),
				IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				IsUpdateERP = entity.IsUpdateERP != null ? entity.IsUpdateERP : false,
				ResultCode = entity.ResultCode != null ? entity.ResultCode : "",
				ResulteDescription = entity.ResulteDescription != null ? entity.ResulteDescription : "",
				ResulteValue = entity.ResulteValue != null ? entity.ResulteValue : "",
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),

				//QuantityOriginalERPAndPartial = Convert.ToDouble(entity.QuantityOriginalERPAndPartial),	 //		 + BalanceQuantityPartialERP/CountInParentPack
				//ValueOriginalERPAndPartial = Convert.ToDouble(entity.ValueOriginalERPAndPartial),
				//QuantityDifferenceOriginalERPAndPartial = Convert.ToDouble(entity.QuantityDifferenceOriginalERPAndPartial),
				//ValueDifferenceOriginalERPAndPartial = Convert.ToDouble(entity.ValueDifferenceOriginalERPAndPartial),
				//QuantityEditAndPartial = Convert.ToDouble(entity.QuantityEditAndPartial),				 //			 + QuantityInPackEdit/CountInParentPack
				//ValueEditAndPartial = Convert.ToDouble(entity.ValueEditAndPartial),
				//QuantityDifferenceEditAndPartial = Convert.ToDouble(entity.QuantityDifferenceEditAndPartial),
				//ValueDifferenceEditAndPartial = Convert.ToDouble(entity.ValueDifferenceEditAndPartial),

				SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				SupplierName = entity.SupplierName != null ? entity.SupplierName : "",
				IturName = entity.IturName != null ? entity.IturName : "",
				LocationName = entity.LocationName != null ? entity.LocationName : "",
				SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				SessionNum = Convert.ToInt32(entity.SessionNum),
				BalanceQuantityPartialERP = Convert.ToInt32(entity.BalanceQuantityPartialERP),
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				//CountInParentPack = entity.CountInParentPack != null ? Convert.ToInt32(entity.CountInParentPack) : 1,
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack) != 0 ? Convert.ToInt32(entity.CountInParentPack) : 1,
				WorkerID = entity.WorkerID,
				WorkerName = entity.WorkerName,
				Total = Convert.ToInt64(entity.Total),
				FromTime = Convert.ToDateTime(entity.FromTime),
				ToTime = Convert.ToDateTime(entity.ToTime),
				TicksTimeSpan = Convert.ToInt64(entity.TicksTimeSpan),
				PeriodFromTo = entity.PeriodFromTo,
				FamilyCode = entity.FamilyCode,
				FamilyName = entity.FamilyName,
				FamilyType = entity.FamilyType,
				FamilySize = entity.FamilySize,
				FamilyExtra1 = entity.FamilyExtra1,
				FamilyExtra2 = entity.FamilyExtra2,
				UnitTypeCode = entity.UnitTypeCode,
				InventorCode = entity.InventorCode,
				InventorName = entity.InventorName,
				BranchCode = entity.BranchCode,
				BranchName = entity.BranchName,
				BranchERPCode = entity.BranchERPCode,
				InventorDate = Convert.ToDateTime(entity.InventorDate),
				IturCodeExpected = entity.IturCodeExpected,
				IturCodeDiffer = Convert.ToBoolean(entity.IturCodeDiffer),
				SubSessionCode = entity.SubSessionCode,
				SessionName = entity.SessionName,
				SubSessionName = entity.SubSessionName,
				IPValueStr1 = entity.IPValueStr1,
				IPValueStr2 = entity.IPValueStr2,
				IPValueStr3 = entity.IPValueStr3,
				IPValueStr4 = entity.IPValueStr4,
				IPValueStr5 = entity.IPValueStr5,
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
				IPValueInt1 = Convert.ToInt32(entity.IPValueInt1),
				IPValueInt2 = Convert.ToInt32(entity.IPValueInt2),
				IPValueInt3 = Convert.ToInt32(entity.IPValueInt3),
				IPValueInt4 = Convert.ToInt32(entity.IPValueInt4),
				IPValueInt5 = Convert.ToInt32(entity.IPValueInt5),
				IPValueBit1 =  Convert.ToBoolean(entity.IPValueBit1),
				IPValueBit2 =  Convert.ToBoolean(entity.IPValueBit2),
				IPValueBit3 =  Convert.ToBoolean(entity.IPValueBit3),
				IPValueBit4 =  Convert.ToBoolean(entity.IPValueBit4),
				IPValueBit5 =  Convert.ToBoolean(entity.IPValueBit5)
				//IPValueFloat1 = Convert.ToDouble(entity.IPValueFloat1),
				//IPValueFloat2 = Convert.ToDouble(entity.IPValueFloat2),
				//IPValueFloat3 = Convert.ToDouble(entity.IPValueFloat3),
				//IPValueFloat4 = Convert.ToDouble(entity.IPValueFloat4),
				//IPValueFloat5 = Convert.ToDouble(entity.IPValueFloat5),
				//IPValueInt1 = Convert.ToInt32(entity.IPValueInt1),
				//IPValueInt2 = Convert.ToInt32(entity.IPValueInt2),
				//IPValueInt3 = Convert.ToInt32(entity.IPValueInt3),
				//IPValueInt4 = Convert.ToInt32(entity.IPValueInt4),
				//IPValueInt5 = Convert.ToInt32(entity.IPValueInt5),
				//IPValueBit1 = Convert.ToBoolean(entity.IPValueBit1),
				//IPValueBit2 = Convert.ToBoolean(entity.IPValueBit2),
				//IPValueBit3 = Convert.ToBoolean(entity.IPValueBit3),
				//IPValueBit4 = Convert.ToBoolean(entity.IPValueBit4),
				//IPValueBit5 = Convert.ToBoolean(entity.IPValueBit5)
			};
		}


		public static IturAnalyzesSimple ToSimpleMakatDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.Makat,
				//InputTypeCode = entity.InputTypeCode,
				//Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				//QuantityDifference = Convert.ToDouble(entity.QuantityDifference),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				MakatOriginal = entity.MakatOriginal != null ? entity.MakatOriginal : "",
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				SectionCode = entity.SectionCode,
				UnitTypeCode = entity.UnitTypeCode,
                FamilyCode = entity.FamilyCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID)
				Count = Convert.ToInt32(entity.Count),
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat),
				IPValueFloat5 = Convert.ToDouble(entity.IPValueFloat5), //temp QuantityEdit
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 
				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)
			};
		}



		public static IturAnalyzesSimple ToSimpleMakatOriginalDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.MakatOriginal,
				//InputTypeCode = entity.InputTypeCode,
				//Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				//IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				MakatOriginal = entity.Makat,
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				SectionCode = entity.SectionCode,
                FamilyCode = entity.FamilyCode,
				UnitTypeCode = entity.UnitTypeCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID),
				Count = Convert.ToInt32(entity.Count),
				IPValueFloat5 = Convert.ToDouble(entity.IPValueFloat5), //temp QuantityEdit
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat),
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 

				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				//SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)

				//UnitTypeCode = entity.FamilyExtra2,
				//InventorCode = entity.InventorCode,
				//InventorName = entity.InventorName,
				//BranchCode = entity.BranchCode,
				//BranchName = entity.BranchName,
				//BranchERPCode = entity.BranchERPCode,
				//InventorDate = Convert.ToDateTime(entity.InventorDate),

			};
		}

		//=========
		public static IturAnalyzes ToSimpleMakatsAndExpiredDateDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			IturAnalyzes newDomainObject = new IturAnalyzes();
			
				newDomainObject.Makat = entity.Makat;
				newDomainObject.Barcode = entity.Barcode;
				//newDomainObject.QuantityEdit =entity.QuantityEdit != null ? Convert.ToDouble(entity.QuantityEdit) : 0;
				//newDomainObject.QuantityOriginal = entity.QuantityOriginal != null ? Convert.ToDouble(entity.QuantityOriginal) : 0;
				newDomainObject.QuantityEdit = entity.QuantityEdit;
				newDomainObject.QuantityOriginal = entity.QuantityOriginal;
				newDomainObject.IturCode = entity.IturCode;

				string erpIturCode = "";
				if (string.IsNullOrWhiteSpace(entity.ERPIturCode) == true) erpIturCode = entity.IturCode;
				else erpIturCode = entity.ERPIturCode;
				newDomainObject.ERPIturCode = erpIturCode;

				newDomainObject.MakatOriginal = entity.MakatOriginal;
				//newDomainObject.BarcodeOriginal = entity.BarcodeOriginal;
				newDomainObject.TypeMakat = entity.TypeMakat;
				newDomainObject.IPValueStr1 = entity.IPValueStr1;
				newDomainObject.IPValueStr2 = entity.IPValueStr2;
				double valueFloat1 = 0	;
				bool ret = Double.TryParse(entity.IPValueStr1, out valueFloat1);
				int valueInt1 = 0;
				ret = Int32.TryParse(entity.IPValueStr1, out valueInt1);
				newDomainObject.IPValueFloat1 = valueFloat1;
				newDomainObject.IPValueInt1 = valueInt1;
				return newDomainObject;
		}

		public static IturAnalyzesSimple ToSimpleMakatNumberDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			long makatNum = - 1;
			string makat = entity.Makat.TrimStart('0');
			bool canConvertMakat = Int64.TryParse(makat, out makatNum);
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.Makat,
				MakatLong = makatNum,
				//InputTypeCode = entity.InputTypeCode,
				//Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				//QuantityDifference = Convert.ToDouble(entity.QuantityDifference),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				//ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				//MakatOriginal = entity.MakatOriginal
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				//FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				//SectionCode = entity.SectionCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID)
				Count = Convert.ToInt32(entity.Count),
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat)
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 
				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				//SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)

				//UnitTypeCode = entity.FamilyExtra2,
				//InventorCode = entity.InventorCode,
				//InventorName = entity.InventorName,
				//BranchCode = entity.BranchCode,
				//BranchName = entity.BranchName,
				//BranchERPCode = entity.BranchERPCode,
				//InventorDate = Convert.ToDateTime(entity.InventorDate),
			};
		}

		public static IturAnalyzesSimple ToSimpleMakatOriginalNumberDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			long makatNum = -1;
			string makatOriginal = entity.MakatOriginal.TrimStart('0');
			bool canConvertMakatOriginal = Int64.TryParse(makatOriginal, out makatNum);
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.MakatOriginal,
				MakatLong = makatNum,
				//InputTypeCode = entity.InputTypeCode,
				//Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				//ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				//IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				MakatOriginal = entity.Makat,
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				//FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				//SectionCode = entity.SectionCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID),
				Count = Convert.ToInt32(entity.Count),
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat),
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 

				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				//SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)


				//UnitTypeCode = entity.FamilyExtra2,
				//InventorCode = entity.InventorCode,
				//InventorName = entity.InventorName,
				//BranchCode = entity.BranchCode,
				//BranchName = entity.BranchName,
				//BranchERPCode = entity.BranchERPCode,
				//InventorDate = Convert.ToDateTime(entity.InventorDate),
			};
		}

		//=========

		public static IturAnalyzesSimple ToSimpleMakatBarcodeDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.Makat,
				//InputTypeCode = entity.InputTypeCode,
				Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				//QuantityDifference = Convert.ToDouble(entity.QuantityDifference),
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				//ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				//IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				//MakatOriginal = entity.MakatOriginal
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				//FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				//SectionCode = entity.SectionCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID)
				Count = Convert.ToInt32(entity.Count),
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat)
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 
				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				//SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)

				//UnitTypeCode = entity.FamilyExtra2,
				//InventorCode = entity.InventorCode,
				//InventorName = entity.InventorName,
				//BranchCode = entity.BranchCode,
				//BranchName = entity.BranchName,
				//BranchERPCode = entity.BranchERPCode,
				//InventorDate = Convert.ToDateTime(entity.InventorDate),
			};
		}

		public static IturAnalyzesSimple ToSimpleMakatOriginalBarcodeDomainObject(this App_Data.IturAnalyzes entity)
		{
			if (entity == null) return null;
			return new IturAnalyzesSimple()
			{
				//ID = entity.ID,
				//Itur_Disabled = entity.Itur_Disabled,
				//Itur_LocationCode = entity.Itur_LocationCode,
				//Itur_Number = entity.Itur_Number,
				//Itur_Publishe = entity.Itur_Publishe,
				//Itur_StatusIturBit = entity.Itur_StatusIturBit,
				//Itur_StatusIturGroupBit = entity.Itur_StatusIturGroupBit,
				//Itur_NumberPrefix = entity.Itur_NumberPrefix,
				//Itur_NumberSufix = entity.Itur_NumberSufix,
				//Itur_StatusDocHeaderBit = entity.Itur_StatusDocHeaderBit,
				//Doc_Name = entity.Doc_Name,
				//Doc_Approve = entity.Doc_Approve,
				//Doc_WorkerGUID = entity.Doc_WorkerGUID,
				//Doc_StatusDocHeaderBit = entity.Doc_StatusDocHeaderBit,
				//Doc_StatusInventProductBit = entity.Doc_StatusInventProductBit,
				//Doc_StatusApproveBit = entity.Doc_StatusApproveBit,
				Makat = entity.MakatOriginal,
				//InputTypeCode = entity.InputTypeCode,
				Barcode = entity.Barcode,
				//ModifyDate = entity.ModifyDate,
				QuantityOriginal = Convert.ToDouble(entity.QuantityOriginal),
				QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				QuantityEdit = entity.QuantityEdit,
				QuantityInPackEdit = Convert.ToInt32(entity.QuantityInPackEdit),
				CountInParentPack = Convert.ToInt32(entity.CountInParentPack),
				//QuantityOriginal = entity.QuantityOriginal,
				//SerialNumber = entity.SerialNumber,
				//ShelfCode = entity.ShelfCode,
				//ProductName = entity.ProductName,
				//PDA_StatusInventProductBit = entity.PDA_StatusInventProductBit,
				//Code = entity.Code,
				//LocationCode = entity.LocationCode,
				//DocumentHeaderCode = entity.DocumentHeaderCode,
				//DocumentCode = entity.DocumentCode,
				//IturCode = entity.IturCode,
				//BarcodeOriginal = entity.BarcodeOriginal,
				//MakatOriginal = entity.MakatOriginal
				//PriceString = entity.PriceString,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				Price = Convert.ToDouble(entity.Price),
				//FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				//SectionNum = Convert.ToInt32(entity.SectionNum),
				//SectionCode = entity.SectionCode,
				//SectionName = entity.SectionName,
				//TypeCode = entity.TypeCode,
				//ERPType = entity.ERPType,
				//DocNum = Convert.ToInt32(entity.DocNum),
				//IPNum = Convert.ToInt32(entity.IPNum),
				TypeMakat = entity.TypeMakat,
				//ValueBuyDifference = Convert.ToDouble(entity.ValueBuyDifference),
				//ValueBuyEdit = entity.ValueBuyEdit,
				//ValueBuyOriginal = entity.ValueBuyQriginal
				//PriceExtra = entity.PriceExtra
				//PDA_ID =  Convert.ToInt64(entity.PDA_ID),
				Count = Convert.ToInt32(entity.Count),
				//ValueChar = entity.ValueChar,
				//ValueInt = Convert.ToInt32(entity.ValueInt),
				//ValueFloat = Convert.ToDouble(entity.ValueFloat),
				//IsResulte = entity.IsResulte != null ? entity.IsResulte : false,
				//ImputTypeCodeFromPDA = entity.ImputTypeCodeFromPDA != null ? entity.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				//IsUpdateERP = entity.IsResulte != null ? entity.IsResulte : false,
				//ResultCode = entity.ResultCode,
				//ResulteDescription = entity.ResulteDescription,
				//ResulteValue = entity.ResulteValue, 

				//QuantityOriginalERP = Convert.ToDouble(entity.QuantityOriginalERP),
				//ValueOriginalERP = Convert.ToDouble(entity.ValueOriginalERP),
				//QuantityDifferenceOriginalERP = Convert.ToDouble(entity.QuantityDifferenceOriginalERP),
				//ValueDifferenceOriginalERP = Convert.ToDouble(entity.ValueDifferenceOriginalERP),
				//SupplierCode = entity.SupplierCode != null ? entity.SupplierCode : "",
				//SupplierName = entity.SupplierName != null ? entity.SupplierName : ""
				//IturName = entity.IturName != null ? entity.IturName : "",
				//LocationName = entity.LocationName != null ? entity.LocationName : "",
				//SessionCode = entity.SessionCode != null ? entity.SessionCode : "",
				//SessionNum = Convert.ToInt32(entity.SessionNum)

				//UnitTypeCode = entity.FamilyExtra2,
				//InventorCode = entity.InventorCode,
				//InventorName = entity.InventorName,
				//BranchCode = entity.BranchCode,
				//BranchName = entity.BranchName,
				//BranchERPCode = entity.BranchERPCode,
				//InventorDate = Convert.ToDateTime(entity.InventorDate),

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
		public static App_Data.IturAnalyzes ToEntity(this IturAnalyzes domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.IturAnalyzes()
			{
				ID = domainObject.ID,
				Itur_Disabled = domainObject.Itur_Disabled,
				Itur_LocationCode = domainObject.Itur_LocationCode,
				Itur_Number = domainObject.Itur_Number,
				Itur_Publishe = domainObject.Itur_Publishe,
				Itur_StatusIturBit = domainObject.Itur_StatusIturBit,
				Itur_StatusIturGroupBit = domainObject.Itur_StatusIturGroupBit,
				Itur_NumberPrefix = domainObject.Itur_NumberPrefix,
				Itur_NumberSufix = domainObject.Itur_NumberSufix,
				Itur_StatusDocHeaderBit = domainObject.Itur_StatusDocHeaderBit,
				Doc_Name = domainObject.Doc_Name,
				Doc_Approve = domainObject.Doc_Approve,					//?
				Doc_WorkerGUID = domainObject.Doc_WorkerGUID,
				Doc_StatusDocHeaderBit = domainObject.Doc_StatusDocHeaderBit,
				Doc_StatusInventProductBit = domainObject.Doc_StatusInventProductBit,
				Doc_StatusApproveBit = domainObject.Doc_StatusApproveBit,
				DocumentHeaderCode = domainObject.DocumentHeaderCode,
				Makat = domainObject.Makat,
				InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				Barcode = domainObject.Barcode,
				ModifyDate = domainObject.ModifyDate,
				QuantityDifference = domainObject.QuantityDifference,	   //?
				QuantityEdit = domainObject.QuantityEdit,						 //?
				QuantityOriginal = domainObject.QuantityOriginal,			 //?
				SerialNumber = domainObject.SerialNumber,
				ShelfCode = domainObject.ShelfCode,
				ProductName = domainObject.ProductName,
				PDA_StatusInventProductBit = domainObject.PDA_StatusInventProductBit,
				Code = domainObject.Code,
				LocationCode = domainObject.LocationCode,
				DocumentCode = domainObject.DocumentCode,
				IturCode = domainObject.IturCode,
				ERPIturCode = domainObject.ERPIturCode != null ? domainObject.ERPIturCode : "",
				BarcodeOriginal = domainObject.BarcodeOriginal,
				MakatOriginal = domainObject.MakatOriginal,
				PriceString = domainObject.PriceString,
				PriceBuy = domainObject.PriceBuy,									 //?
				PriceSale = domainObject.PriceSale,									//?
				Price = domainObject.Price,
				PriceExtra = domainObject.PriceExtra,
				FromCatalogType = domainObject.FromCatalogType,		   //?
				SectionNum = domainObject.SectionNum,						   //?
				SectionCode = domainObject.SectionCode,
				SectionName = domainObject.SectionName,

				TypeCode = domainObject.TypeCode,
				ERPType = domainObject.ERPType,									 //?
				DocNum = Convert.ToInt32(domainObject.DocNum),		  //?
				IPNum = Convert.ToInt32(domainObject.IPNum),			 //?
				TypeMakat = domainObject.TypeMakat,
				ValueBuyDifference = Convert.ToDouble(domainObject.ValueBuyDifference),		 //?
				ValueBuyEdit = domainObject.ValueBuyEdit,														  //?
				ValueBuyQriginal = domainObject.ValueBuyQriginal,											 //?

				//PriceExtra = domainObject.PriceExtra														  
				PDA_ID = Convert.ToInt64(domainObject.PDA_ID),										//?
				Count = Convert.ToInt32(domainObject.Count),												//?
				ValueChar = domainObject.ValueChar,
				ValueInt = Convert.ToInt32(domainObject.ValueInt),									   //?
				ValueFloat = Convert.ToDouble(domainObject.ValueFloat),								 //?
				IsResulte = domainObject.IsResulte != null ? domainObject.IsResulte : false,	  //?
				ImputTypeCodeFromPDA = domainObject.ImputTypeCodeFromPDA != null ? domainObject.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString(),
				IsUpdateERP = domainObject.IsUpdateERP != null ? domainObject.IsUpdateERP : false,	   //?
				ResultCode = domainObject.ResultCode != null ? domainObject.ResultCode : "",
				ResulteDescription = domainObject.ResulteDescription != null ? domainObject.ResulteDescription : "",
				ResulteValue = domainObject.ResulteValue != null ? domainObject.ResulteValue : "",
				QuantityOriginalERP = Convert.ToDouble(domainObject.QuantityOriginalERP),		  //?
				ValueOriginalERP = Convert.ToDouble(domainObject.ValueOriginalERP),					 //?
				QuantityDifferenceOriginalERP = Convert.ToDouble(domainObject.QuantityDifferenceOriginalERP),  //?
				ValueDifferenceOriginalERP = Convert.ToDouble(domainObject.ValueDifferenceOriginalERP),		   //?
				SupplierCode = domainObject.SupplierCode != null ? domainObject.SupplierCode : "",
				SupplierName = domainObject.SupplierName != null ? domainObject.SupplierName : "",
				IturName = domainObject.IturName != null ? domainObject.IturName : "",
				LocationName = domainObject.LocationName != null ? domainObject.LocationName : "",
				SessionCode = domainObject.SessionCode != null ? domainObject.SessionCode : "",
				SessionNum = Convert.ToInt32(domainObject.SessionNum),
				BalanceQuantityPartialERP = Convert.ToInt32(domainObject.BalanceQuantityPartialERP),
				QuantityInPackEdit = domainObject.QuantityInPackEdit,
				//CountInParentPack = domainObject.CountInParentPack != null ? Convert.ToInt32(domainObject.CountInParentPack) : 1,
				CountInParentPack = domainObject.CountInParentPack != 0 ? domainObject.CountInParentPack : 1,
				WorkerID = domainObject.WorkerID,
				WorkerName = domainObject.WorkerName,
				Total = Convert.ToInt64(domainObject.Total),
				FromTime = Convert.ToDateTime(domainObject.FromTime),
				ToTime = Convert.ToDateTime(domainObject.ToTime),
				TicksTimeSpan = Convert.ToInt64(domainObject.TicksTimeSpan),
				PeriodFromTo = domainObject.PeriodFromTo,
				FamilyCode = domainObject.FamilyCode,
				FamilyName = domainObject.FamilyName,
				FamilyType = domainObject.FamilyType,
				FamilySize = domainObject.FamilySize,
				FamilyExtra1 = domainObject.FamilyExtra1,
				FamilyExtra2 = domainObject.FamilyExtra2,
				UnitTypeCode = domainObject.FamilyExtra2,
				InventorCode = domainObject.InventorCode,
				InventorName = domainObject.InventorName,
				BranchCode = domainObject.BranchCode,
				BranchName = domainObject.BranchName,
				BranchERPCode = domainObject.BranchERPCode,
				InventorDate = domainObject.InventorDate,
				IturCodeExpected = domainObject.IturCodeExpected,
				IturCodeDiffer = domainObject.IturCodeDiffer,
				SubSessionCode = domainObject.SubSessionCode,
				SessionName = domainObject.SessionName,
				SubSessionName = domainObject.SubSessionName,
				IPValueStr1 = domainObject.IPValueStr1,
				IPValueStr2 = domainObject.IPValueStr2,
				IPValueStr3 = domainObject.IPValueStr3,
				IPValueStr4 = domainObject.IPValueStr4,
				IPValueStr5 = domainObject.IPValueStr5,
				IPValueStr6 = domainObject.IPValueStr6,
				IPValueStr7 = domainObject.IPValueStr7,
				IPValueStr8 = domainObject.IPValueStr8,
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
				IPValueBit5 = domainObject.IPValueBit5

			};
		}

		/// <summary>
		/// Применение изменений к сущности базы данных.
		/// 
		/// Apply changes to database entity.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <param name="domainObject">
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </param>
		public static void ApplyChanges(this App_Data.IturAnalyzes entity, IturAnalyzes domainObject)
		{
			if (domainObject == null) return;
			entity.Itur_Disabled = domainObject.Itur_Disabled;
			entity.Itur_LocationCode = domainObject.Itur_LocationCode;
			entity.Itur_Number = domainObject.Itur_Number;
			entity.Itur_Publishe = domainObject.Itur_Publishe;
			entity.Itur_StatusIturBit = domainObject.Itur_StatusIturBit;
			entity.Itur_StatusIturGroupBit = domainObject.Itur_StatusIturGroupBit;
			entity.Itur_NumberPrefix = domainObject.Itur_NumberPrefix;
			entity.Itur_NumberSufix = domainObject.Itur_NumberSufix;
			entity.Itur_StatusDocHeaderBit = domainObject.Itur_StatusDocHeaderBit;
			entity.Doc_Name = domainObject.Doc_Name;
			entity.Doc_Approve = domainObject.Doc_Approve;
			entity.Doc_WorkerGUID = domainObject.Doc_WorkerGUID;
			entity.Doc_StatusDocHeaderBit = domainObject.Doc_StatusDocHeaderBit;
			entity.Doc_StatusInventProductBit = domainObject.Doc_StatusInventProductBit;
			entity.Doc_StatusApproveBit = domainObject.Doc_StatusApproveBit;
			entity.Makat = domainObject.Makat;
			entity.InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString();
			entity.Barcode = domainObject.Barcode;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.QuantityDifference = domainObject.QuantityDifference;
			entity.QuantityEdit = domainObject.QuantityEdit;
			entity.QuantityOriginal = domainObject.QuantityOriginal;
			entity.SerialNumber = domainObject.SerialNumber;
			entity.ShelfCode = domainObject.ShelfCode;
			entity.ProductName = domainObject.ProductName;
			entity.PDA_StatusInventProductBit = domainObject.PDA_StatusInventProductBit;
			entity.Code = domainObject.Code;
			entity.LocationCode = domainObject.LocationCode;
			entity.DocumentHeaderCode = domainObject.DocumentHeaderCode;
			entity.DocumentCode = domainObject.DocumentCode;
			entity.ERPIturCode = domainObject.ERPIturCode != null ? domainObject.ERPIturCode : "";
			entity.IturCode = domainObject.IturCode;
			entity.BarcodeOriginal = domainObject.BarcodeOriginal;
			entity.MakatOriginal = domainObject.MakatOriginal;
			entity.PriceString = domainObject.PriceString;
			entity.PriceBuy = domainObject.PriceBuy;
			entity.PriceSale = domainObject.PriceSale;
			entity.Price = domainObject.Price;
			entity.PriceExtra = domainObject.PriceExtra;
			entity.FromCatalogType = domainObject.FromCatalogType;
			entity.SectionNum = domainObject.SectionNum;
			entity.TypeCode = domainObject.TypeCode;
			entity.FromCatalogType = domainObject.FromCatalogType;
			entity.SectionNum = domainObject.SectionNum;
			entity.SectionCode = domainObject.SectionCode;
			entity.SectionName = domainObject.SectionName;
			entity.TypeCode = domainObject.TypeCode;
			entity.ERPType = domainObject.ERPType;
			entity.DocNum = Convert.ToInt32(domainObject.DocNum);
			entity.IPNum = Convert.ToInt32(domainObject.IPNum);
			entity.TypeMakat = domainObject.TypeMakat;
			entity.ValueBuyDifference = Convert.ToDouble(domainObject.ValueBuyDifference);
			entity.ValueBuyEdit = domainObject.ValueBuyEdit;
			entity.ValueBuyQriginal = domainObject.ValueBuyQriginal;
			entity.PDA_ID = Convert.ToInt64(domainObject.PDA_ID);
			entity.Count = Convert.ToInt32(domainObject.Count);
			entity.ValueChar = domainObject.ValueChar;
			entity.ValueInt = Convert.ToInt32(domainObject.ValueInt);
			entity.ValueFloat = Convert.ToDouble(domainObject.ValueFloat);
			entity.IsResulte = domainObject.IsResulte != null ? domainObject.IsResulte : false;
			entity.ImputTypeCodeFromPDA = domainObject.ImputTypeCodeFromPDA != null ? domainObject.ImputTypeCodeFromPDA : InputTypeCodeEnum.B.ToString();
			entity.IsUpdateERP = domainObject.IsUpdateERP != null ? domainObject.IsUpdateERP : false;
			entity.ResultCode = domainObject.ResultCode != null ? domainObject.ResultCode : "";
			entity.ResulteDescription = domainObject.ResulteDescription != null ? domainObject.ResulteDescription : "";
			entity.ResulteValue = domainObject.ResulteValue != null ? domainObject.ResulteValue : "";
			entity.QuantityOriginalERP = Convert.ToDouble(domainObject.QuantityOriginalERP);
			entity.ValueOriginalERP = Convert.ToDouble(domainObject.ValueOriginalERP);
			entity.QuantityDifferenceOriginalERP = Convert.ToDouble(domainObject.QuantityDifferenceOriginalERP);
			entity.ValueDifferenceOriginalERP = Convert.ToDouble(domainObject.ValueDifferenceOriginalERP);
			entity.SupplierCode = domainObject.SupplierCode != null ? domainObject.SupplierCode : "";
			entity.SupplierName = domainObject.SupplierName != null ? domainObject.SupplierName : "";
			entity.IturName = domainObject.IturName != null ? domainObject.IturName : "";
			entity.LocationName = domainObject.LocationName != null ? domainObject.LocationName : "";
			entity.SessionCode = domainObject.SessionCode != null ? domainObject.SessionCode : "";
			entity.SessionNum = Convert.ToInt32(domainObject.SessionNum);
			entity.BalanceQuantityPartialERP = Convert.ToInt32(domainObject.BalanceQuantityPartialERP);
			entity.QuantityInPackEdit = domainObject.QuantityInPackEdit;
			//entity.CountInParentPack = domainObject.CountInParentPack != null ? Convert.ToInt32(domainObject.CountInParentPack) : 1;
			entity.CountInParentPack = domainObject.CountInParentPack != 0 ? domainObject.CountInParentPack : 1;
			entity.WorkerID = domainObject.WorkerID;
			entity.WorkerName = domainObject.WorkerName;
			entity.Total = Convert.ToInt64(domainObject.Total);
			entity.FromTime = Convert.ToDateTime(domainObject.FromTime);
			entity.ToTime = Convert.ToDateTime(domainObject.ToTime);
			entity.TicksTimeSpan = Convert.ToInt64(domainObject.TicksTimeSpan);
			entity.PeriodFromTo = domainObject.PeriodFromTo;
			entity.FamilyCode = domainObject.FamilyCode;
			entity.FamilyName = domainObject.FamilyName;
			entity.FamilyType = domainObject.FamilyType;
			entity.FamilySize = domainObject.FamilySize;
			entity.FamilyExtra1 = domainObject.FamilyExtra1;
			entity.FamilyExtra2 = domainObject.FamilyExtra2;
			entity.UnitTypeCode = domainObject.FamilyExtra2;
			entity.InventorCode = domainObject.InventorCode;
			entity.InventorName = domainObject.InventorName;
			entity.BranchCode = domainObject.BranchCode;
			entity.BranchName = domainObject.BranchName;
			entity.BranchERPCode = domainObject.BranchERPCode;
			entity.InventorDate = domainObject.InventorDate;
			entity.IturCodeExpected = domainObject.IturCodeExpected;
			entity.IturCodeDiffer = domainObject.IturCodeDiffer;
			entity.SubSessionCode = domainObject.SubSessionCode;
			entity.SessionName = domainObject.SessionName;
			entity.SubSessionName = domainObject.SubSessionName;
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

		}
	}
}
