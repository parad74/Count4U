using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4Mobile.MappingEF
{
	public static class CurrentInventoryAdvancedMapper
	{

		public static CurrentInventoryAdvanced ToDomainObject(this App_Data.CurrentInventoryAdvanced entity)
		{
			if (entity == null) return null;
			return new CurrentInventoryAdvanced()
			{
				Id = entity.Id,
				Uid = entity.Uid != null ? entity.Uid : "",
				SerialNumberLocal = entity.SerialNumberLocal != null ? entity.SerialNumberLocal : "",
				ItemCode = entity.ItemCode != null ? entity.ItemCode : "",
				DomainObject = entity.DomainObject != null ? entity.DomainObject : "",
				Table = entity.Table != null ? entity.Table : "",
				Adapter = entity.Adapter != null ? entity.Adapter : "",
				SerialNumberSupplier = entity.SerialNumberSupplier != null ? entity.SerialNumberSupplier : "",
				Quantity = entity.Quantity != null ? entity.Quantity : "",
				QuantityDouble = entity.QuantityDouble != null ? Convert.ToDouble(entity.QuantityDouble) : 0,

				PropertyStr1 = entity.PropertyStr1 != null ? entity.PropertyStr1 : "",
				PropertyStr1Name = entity.PropertyStr1Name != null ? entity.PropertyStr1Name : "",
				PropertyStr1Code = entity.PropertyStr1Code != null ? entity.PropertyStr1Code : "",

				PropertyStr2 = entity.PropertyStr2 != null ? entity.PropertyStr2 : "",
				PropertyStr2Name = entity.PropertyStr2Name != null ? entity.PropertyStr2Name : "",
				PropertyStr2Code = entity.PropertyStr2Code != null ? entity.PropertyStr2Code : "",

				PropertyStr3 = entity.PropertyStr3 != null ? entity.PropertyStr3 : "",
				PropertyStr3Name = entity.PropertyStr3Name != null ? entity.PropertyStr3Name : "",
				PropertyStr3Code = entity.PropertyStr3Code != null ? entity.PropertyStr3Code : "",

				PropertyStr4 = entity.PropertyStr4 != null ? entity.PropertyStr4 : "",
				PropertyStr4Name = entity.PropertyStr4Name != null ? entity.PropertyStr4Name : "",
				PropertyStr4Code = entity.PropertyStr4Code != null ? entity.PropertyStr4Code : "",

				PropertyStr5 = entity.PropertyStr5 != null ? entity.PropertyStr5 : "",
				PropertyStr5Name = entity.PropertyStr5Name != null ? entity.PropertyStr5Name : "",
				PropertyStr5Code = entity.PropertyStr5Code != null ? entity.PropertyStr5Code : "",

				PropertyStr6 = entity.PropertyStr6 != null ? entity.PropertyStr6 : "",
				PropertyStr6Name = entity.PropertyStr6Name != null ? entity.PropertyStr6Name : "",
				PropertyStr6Code = entity.PropertyStr6Code != null ? entity.PropertyStr6Code : "",

				PropertyStr7 = entity.PropertyStr7 != null ? entity.PropertyStr7 : "",
				PropertyStr7Name = entity.PropertyStr7Name != null ? entity.PropertyStr7Name : "",
				PropertyStr7Code = entity.PropertyStr7Code != null ? entity.PropertyStr7Code : "",

				PropertyStr8 = entity.PropertyStr8 != null ? entity.PropertyStr8 : "",
				PropertyStr8Name = entity.PropertyStr8Name != null ? entity.PropertyStr8Name : "",
				PropertyStr8Code = entity.PropertyStr8Code != null ? entity.PropertyStr8Code : "",

				PropertyStr9 = entity.PropertyStr9 != null ? entity.PropertyStr9 : "",
				PropertyStr9Name = entity.PropertyStr9Name != null ? entity.PropertyStr9Name : "",
				PropertyStr9Code = entity.PropertyStr9Code != null ? entity.PropertyStr9Code : "",

				PropertyStr10 = entity.PropertyStr10 != null ? entity.PropertyStr10 : "",
				PropertyStr10Name = entity.PropertyStr10Name != null ? entity.PropertyStr10Name : "",
				PropertyStr10Code = entity.PropertyStr10Code != null ? entity.PropertyStr10Code : "",

				PropertyStr11 = entity.PropertyStr11 != null ? entity.PropertyStr11 : "",
				PropertyStr11Name = entity.PropertyStr11Name != null ? entity.PropertyStr11Name : "",
				PropertyStr11Code = entity.PropertyStr11Code != null ? entity.PropertyStr11Code : "",

				PropertyStr12 = entity.PropertyStr12 != null ? entity.PropertyStr12 : "",
				PropertyStr12Name = entity.PropertyStr12Name != null ? entity.PropertyStr12Name : "",
				PropertyStr12Code = entity.PropertyStr12Code != null ? entity.PropertyStr12Code : "",

				PropertyStr13 = entity.PropertyStr13 != null ? entity.PropertyStr13 : "",
				PropertyStr13Name = entity.PropertyStr13Name != null ? entity.PropertyStr13Name : "",
				PropertyStr13Code = entity.PropertyStr13Code != null ? entity.PropertyStr13Code : "",

				PropertyStr14 = entity.PropertyStr14 != null ? entity.PropertyStr14 : "",
				PropertyStr14Name = entity.PropertyStr14Name != null ? entity.PropertyStr14Name : "",
				PropertyStr14Code = entity.PropertyStr14Code != null ? entity.PropertyStr14Code : "",

				PropertyStr15 = entity.PropertyStr15 != null ? entity.PropertyStr15 : "",
				PropertyStr15Name = entity.PropertyStr15Name != null ? entity.PropertyStr15Name : "",
				PropertyStr15Code = entity.PropertyStr15Code != null ? entity.PropertyStr15Code : "",

				PropertyStr16 = entity.PropertyStr16 != null ? entity.PropertyStr16 : "",
				PropertyStr16Name = entity.PropertyStr16Name != null ? entity.PropertyStr16Name : "",
				PropertyStr16Code = entity.PropertyStr16Code != null ? entity.PropertyStr16Code : "",

				PropertyStr17 = entity.PropertyStr17 != null ? entity.PropertyStr17 : "",
				PropertyStr17Name = entity.PropertyStr17Name != null ? entity.PropertyStr17Name : "",
				PropertyStr17Code = entity.PropertyStr17Code != null ? entity.PropertyStr17Code : "",

				PropertyStr18 = entity.PropertyStr18 != null ? entity.PropertyStr18 : "",
				PropertyStr18Name = entity.PropertyStr18Name != null ? entity.PropertyStr18Name : "",
				PropertyStr18Code = entity.PropertyStr18Code != null ? entity.PropertyStr18Code : "",

				PropertyStr19 = entity.PropertyStr19 != null ? entity.PropertyStr19 : "",
				PropertyStr19Name = entity.PropertyStr19Name != null ? entity.PropertyStr19Name : "",
				PropertyStr19Code = entity.PropertyStr19Code != null ? entity.PropertyStr19Code : "",

				PropertyStr20 = entity.PropertyStr20 != null ? entity.PropertyStr20 : "",
				PropertyStr20Name = entity.PropertyStr20Name != null ? entity.PropertyStr20Name : "",
				PropertyStr20Code = entity.PropertyStr20Code != null ? entity.PropertyStr20Code : "",

				PropExtenstion1 = entity.PropExtenstion1 != null ? entity.PropExtenstion1 : "",
				PropExtenstion2 = entity.PropExtenstion2 != null ? entity.PropExtenstion2 : "",
				PropExtenstion3 = entity.PropExtenstion3 != null ? entity.PropExtenstion3 : "",
				PropExtenstion4 = entity.PropExtenstion4 != null ? entity.PropExtenstion4 : "",
				PropExtenstion5 = entity.PropExtenstion5 != null ? entity.PropExtenstion5 : "",
				PropExtenstion6 = entity.PropExtenstion6 != null ? entity.PropExtenstion6 : "",
				PropExtenstion7 = entity.PropExtenstion7 != null ? entity.PropExtenstion7 : "",
				PropExtenstion8 = entity.PropExtenstion8 != null ? entity.PropExtenstion8 : "",
				PropExtenstion9 = entity.PropExtenstion9 != null ? entity.PropExtenstion9 : "",
				PropExtenstion10 = entity.PropExtenstion10 != null ? entity.PropExtenstion10 : "",
				PropExtenstion11 = entity.PropExtenstion11 != null ? entity.PropExtenstion11 : "",
				PropExtenstion12 = entity.PropExtenstion12 != null ? entity.PropExtenstion12 : "",
				PropExtenstion13 = entity.PropExtenstion13 != null ? entity.PropExtenstion13 : "",
				PropExtenstion14 = entity.PropExtenstion14 != null ? entity.PropExtenstion14 : "",
				PropExtenstion15 = entity.PropExtenstion15 != null ? entity.PropExtenstion15 : "",
				PropExtenstion16 = entity.PropExtenstion16 != null ? entity.PropExtenstion16 : "",
				PropExtenstion17 = entity.PropExtenstion17 != null ? entity.PropExtenstion17 : "",
				PropExtenstion18 = entity.PropExtenstion18 != null ? entity.PropExtenstion18 : "",
				PropExtenstion19 = entity.PropExtenstion19 != null ? entity.PropExtenstion19 : "",
				PropExtenstion20 = entity.PropExtenstion20 != null ? entity.PropExtenstion20 : "",
				PropExtenstion21 = entity.PropExtenstion21 != null ? entity.PropExtenstion21 : "",
				PropExtenstion22 = entity.PropExtenstion22 != null ? entity.PropExtenstion22 : "",

				LocationCode = entity.LocationCode != null ? entity.LocationCode : "",
				LocationDescription = entity.LocationDescription != null ? entity.LocationDescription : "",
				LocationLevel1Code = entity.LocationLevel1Code != null ? entity.LocationLevel1Code : "",
				LocationLevel1Name = entity.LocationLevel1Name != null ? entity.LocationLevel1Name : "",
				LocationLevel2Code = entity.LocationLevel2Code != null ? entity.LocationLevel2Code : "",
				LocationLevel2Name = entity.LocationLevel2Name != null ? entity.LocationLevel2Name : "",
				LocationLevel3Code = entity.LocationLevel3Code != null ? entity.LocationLevel3Code : "",
				LocationLevel3Name = entity.LocationLevel3Name != null ? entity.LocationLevel3Name : "",
				LocationLevel4Code = entity.LocationLevel4Code != null ? entity.LocationLevel4Code : "",
				LocationLevel4Name = entity.LocationLevel4Name != null ? entity.LocationLevel4Name : "",
				LocationInvStatus = entity.LocationInvStatus != null ? entity.LocationInvStatus : "",
				LocationNodeType = entity.LocationNodeType != null ? entity.LocationNodeType : "",
				LocationLevelNum = entity.LocationLevelNum != null ? entity.LocationLevelNum : "",
				LocationTotal = entity.LocationTotal != null ? entity.LocationTotal : "",

				DateModified = entity.DateModified != null ? entity.DateModified : "",
				DateCreated = entity.DateCreated != null ? entity.DateCreated : "",
				ItemStatus = entity.ItemStatus != null ? entity.ItemStatus : "",
				ItemType = entity.ItemType != null ? entity.ItemType : "",
				UnitTypeCode = entity.UnitTypeCode != null ? entity.UnitTypeCode : "",

				CatalogItemCode = entity.CatalogItemCode != null ? entity.CatalogItemCode : "",
				CatalogItemName = entity.CatalogItemName != null ? entity.CatalogItemName : "",
				CatalogItemType = entity.CatalogItemType != null ? entity.CatalogItemType : "",
				CatalogFamilyCode = entity.CatalogFamilyCode != null ? entity.CatalogFamilyCode : "",
				CatalogFamilyName = entity.CatalogFamilyName != null ? entity.CatalogFamilyName : "",
				CatalogSectionCode = entity.CatalogSectionCode != null ? entity.CatalogSectionCode : "",
				CatalogSectionName = entity.CatalogSectionName != null ? entity.CatalogSectionName : "",
				CatalogSubSectionCode = entity.CatalogSubSectionCode != null ? entity.CatalogSubSectionCode : "",
				CatalogSubSectionName = entity.CatalogSubSectionName != null ? entity.CatalogSubSectionName : "",
				CatalogPriceBuy = entity.CatalogPriceBuy != null ? entity.CatalogPriceBuy : "",
				CatalogPriceSell = entity.CatalogPriceSell != null ? entity.CatalogPriceSell : "",
				CatalogSupplierCode = entity.CatalogSupplierCode != null ? entity.CatalogSupplierCode : "",
				CatalogSupplierName = entity.CatalogSupplierName != null ? entity.CatalogSupplierName : "",
				CatalogUnitTypeCode = entity.CatalogUnitTypeCode != null ? entity.CatalogUnitTypeCode : "",
				CatalogDescription = entity.CatalogDescription != null ? entity.CatalogDescription : "",

				TemporaryOldUid = entity.TemporaryOldUid != null ? entity.TemporaryOldUid : "",
				TemporaryNewUid = entity.TemporaryNewUid != null ? entity.TemporaryNewUid : "",

				TemporaryOldSerialNumber = entity.TemporaryOldSerialNumber != null ? entity.TemporaryOldSerialNumber : "",
				TemporaryOldItemCode = entity.TemporaryOldItemCode != null ? entity.TemporaryOldItemCode : "",
				TemporaryOldLocationCode = entity.TemporaryOldLocationCode != null ? entity.TemporaryOldLocationCode : "",
				TemporaryOldKey = entity.TemporaryOldKey != null ? entity.TemporaryOldKey : "",

				TemporaryNewSerialNumber = entity.TemporaryNewSerialNumber != null ? entity.TemporaryNewSerialNumber : "",
				TemporaryNewItemCode = entity.TemporaryNewItemCode != null ? entity.TemporaryNewItemCode : "",
				TemporaryNewLocationCode = entity.TemporaryNewLocationCode != null ? entity.TemporaryNewLocationCode : "",
				TemporaryNewKey = entity.TemporaryNewKey != null ? entity.TemporaryNewKey : "",

				TemporaryDateModified = entity.TemporaryDateModified != null ? entity.TemporaryDateModified : "",
				TemporaryOperation = entity.TemporaryOperation != null ? entity.TemporaryOperation : "",
				TemporaryDevice = entity.TemporaryDevice != null ? entity.TemporaryDevice : "",
				TemporaryDbFileName = entity.TemporaryDbFileName != null ? entity.TemporaryDbFileName : "",

				IturCode = entity.IturCode != null ? entity.IturCode : ""
			};
		}

		public static CurrentInventoryAdvanced ToSimpleDomainObject(this App_Data.CurrentInventoryAdvanced entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.CurrentInventoryAdvanced ToEntity(this CurrentInventoryAdvanced domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.CurrentInventoryAdvanced()
			{
				Id = domainObject.Id,
				Uid = domainObject.Uid != null ? domainObject.Uid : "",
				SerialNumberLocal = domainObject.SerialNumberLocal != null ? domainObject.SerialNumberLocal : "",
				ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "",
				DomainObject = domainObject.DomainObject != null ? domainObject.DomainObject : "",
				Table = domainObject.Table != null ? domainObject.Table : "",
				Adapter = domainObject.Adapter != null ? domainObject.Adapter : "",
				SerialNumberSupplier = domainObject.SerialNumberSupplier != null ? domainObject.SerialNumberSupplier : "",
				Quantity = domainObject.Quantity != null ? domainObject.Quantity : "",
				QuantityDouble = domainObject.QuantityDouble != null ? Convert.ToDouble(domainObject.QuantityDouble) : 0,

				PropertyStr1 = domainObject.PropertyStr1 != null ? domainObject.PropertyStr1 : "",
				PropertyStr1Name = domainObject.PropertyStr1Name != null ? domainObject.PropertyStr1Name : "",
				PropertyStr1Code = domainObject.PropertyStr1Code != null ? domainObject.PropertyStr1Code : "",

				PropertyStr2 = domainObject.PropertyStr2 != null ? domainObject.PropertyStr2 : "",
				PropertyStr2Name = domainObject.PropertyStr2Name != null ? domainObject.PropertyStr2Name : "",
				PropertyStr2Code = domainObject.PropertyStr2Code != null ? domainObject.PropertyStr2Code : "",

				PropertyStr3 = domainObject.PropertyStr3 != null ? domainObject.PropertyStr3 : "",
				PropertyStr3Name = domainObject.PropertyStr3Name != null ? domainObject.PropertyStr3Name : "",
				PropertyStr3Code = domainObject.PropertyStr3Code != null ? domainObject.PropertyStr3Code : "",

				PropertyStr4 = domainObject.PropertyStr4 != null ? domainObject.PropertyStr4 : "",
				PropertyStr4Name = domainObject.PropertyStr4Name != null ? domainObject.PropertyStr4Name : "",
				PropertyStr4Code = domainObject.PropertyStr4Code != null ? domainObject.PropertyStr4Code : "",

				PropertyStr5 = domainObject.PropertyStr5 != null ? domainObject.PropertyStr5 : "",
				PropertyStr5Name = domainObject.PropertyStr5Name != null ? domainObject.PropertyStr5Name : "",
				PropertyStr5Code = domainObject.PropertyStr5Code != null ? domainObject.PropertyStr5Code : "",

				PropertyStr6 = domainObject.PropertyStr6 != null ? domainObject.PropertyStr6 : "",
				PropertyStr6Name = domainObject.PropertyStr6Name != null ? domainObject.PropertyStr6Name : "",
				PropertyStr6Code = domainObject.PropertyStr6Code != null ? domainObject.PropertyStr6Code : "",

				PropertyStr7 = domainObject.PropertyStr7 != null ? domainObject.PropertyStr7 : "",
				PropertyStr7Name = domainObject.PropertyStr7Name != null ? domainObject.PropertyStr7Name : "",
				PropertyStr7Code = domainObject.PropertyStr7Code != null ? domainObject.PropertyStr7Code : "",

				PropertyStr8 = domainObject.PropertyStr8 != null ? domainObject.PropertyStr8 : "",
				PropertyStr8Name = domainObject.PropertyStr8Name != null ? domainObject.PropertyStr8Name : "",
				PropertyStr8Code = domainObject.PropertyStr8Code != null ? domainObject.PropertyStr8Code : "",

				PropertyStr9 = domainObject.PropertyStr9 != null ? domainObject.PropertyStr9 : "",
				PropertyStr9Name = domainObject.PropertyStr9Name != null ? domainObject.PropertyStr9Name : "",
				PropertyStr9Code = domainObject.PropertyStr9Code != null ? domainObject.PropertyStr9Code : "",

				PropertyStr10 = domainObject.PropertyStr10 != null ? domainObject.PropertyStr10 : "",
				PropertyStr10Name = domainObject.PropertyStr10Name != null ? domainObject.PropertyStr10Name : "",
				PropertyStr10Code = domainObject.PropertyStr10Code != null ? domainObject.PropertyStr10Code : "",

				PropertyStr11 = domainObject.PropertyStr11 != null ? domainObject.PropertyStr11 : "",
				PropertyStr11Name = domainObject.PropertyStr11Name != null ? domainObject.PropertyStr11Name : "",
				PropertyStr11Code = domainObject.PropertyStr11Code != null ? domainObject.PropertyStr11Code : "",

				PropertyStr12 = domainObject.PropertyStr12 != null ? domainObject.PropertyStr12 : "",
				PropertyStr12Name = domainObject.PropertyStr12Name != null ? domainObject.PropertyStr12Name : "",
				PropertyStr12Code = domainObject.PropertyStr12Code != null ? domainObject.PropertyStr12Code : "",

				PropertyStr13 = domainObject.PropertyStr13 != null ? domainObject.PropertyStr13 : "",
				PropertyStr13Name = domainObject.PropertyStr13Name != null ? domainObject.PropertyStr13Name : "",
				PropertyStr13Code = domainObject.PropertyStr13Code != null ? domainObject.PropertyStr13Code : "",

				PropertyStr14 = domainObject.PropertyStr14 != null ? domainObject.PropertyStr14 : "",
				PropertyStr14Name = domainObject.PropertyStr14Name != null ? domainObject.PropertyStr14Name : "",
				PropertyStr14Code = domainObject.PropertyStr14Code != null ? domainObject.PropertyStr14Code : "",

				PropertyStr15 = domainObject.PropertyStr15 != null ? domainObject.PropertyStr15 : "",
				PropertyStr15Name = domainObject.PropertyStr15Name != null ? domainObject.PropertyStr15Name : "",
				PropertyStr15Code = domainObject.PropertyStr15Code != null ? domainObject.PropertyStr15Code : "",

				PropertyStr16 = domainObject.PropertyStr16 != null ? domainObject.PropertyStr16 : "",
				PropertyStr16Name = domainObject.PropertyStr16Name != null ? domainObject.PropertyStr16Name : "",
				PropertyStr16Code = domainObject.PropertyStr16Code != null ? domainObject.PropertyStr16Code : "",

				PropertyStr17 = domainObject.PropertyStr17 != null ? domainObject.PropertyStr17 : "",
				PropertyStr17Name = domainObject.PropertyStr17Name != null ? domainObject.PropertyStr17Name : "",
				PropertyStr17Code = domainObject.PropertyStr17Code != null ? domainObject.PropertyStr17Code : "",

				PropertyStr18 = domainObject.PropertyStr18 != null ? domainObject.PropertyStr18 : "",
				PropertyStr18Name = domainObject.PropertyStr18Name != null ? domainObject.PropertyStr18Name : "",
				PropertyStr18Code = domainObject.PropertyStr18Code != null ? domainObject.PropertyStr18Code : "",

				PropertyStr19 = domainObject.PropertyStr19 != null ? domainObject.PropertyStr19 : "",
				PropertyStr19Name = domainObject.PropertyStr19Name != null ? domainObject.PropertyStr19Name : "",
				PropertyStr19Code = domainObject.PropertyStr19Code != null ? domainObject.PropertyStr19Code : "",

				PropertyStr20 = domainObject.PropertyStr20 != null ? domainObject.PropertyStr20 : "",
				PropertyStr20Name = domainObject.PropertyStr20Name != null ? domainObject.PropertyStr20Name : "",
				PropertyStr20Code = domainObject.PropertyStr20Code != null ? domainObject.PropertyStr20Code : "",

				PropExtenstion1 = domainObject.PropExtenstion1 != null ? domainObject.PropExtenstion1 : "",
				PropExtenstion2 = domainObject.PropExtenstion2 != null ? domainObject.PropExtenstion2 : "",
				PropExtenstion3 = domainObject.PropExtenstion3 != null ? domainObject.PropExtenstion3 : "",
				PropExtenstion4 = domainObject.PropExtenstion4 != null ? domainObject.PropExtenstion4 : "",
				PropExtenstion5 = domainObject.PropExtenstion5 != null ? domainObject.PropExtenstion5 : "",
				PropExtenstion6 = domainObject.PropExtenstion6 != null ? domainObject.PropExtenstion6 : "",
				PropExtenstion7 = domainObject.PropExtenstion7 != null ? domainObject.PropExtenstion7 : "",
				PropExtenstion8 = domainObject.PropExtenstion8 != null ? domainObject.PropExtenstion8 : "",
				PropExtenstion9 = domainObject.PropExtenstion9 != null ? domainObject.PropExtenstion9 : "",
				PropExtenstion10 = domainObject.PropExtenstion10 != null ? domainObject.PropExtenstion10 : "",
				PropExtenstion11 = domainObject.PropExtenstion11 != null ? domainObject.PropExtenstion11 : "",
				PropExtenstion12 = domainObject.PropExtenstion12 != null ? domainObject.PropExtenstion12 : "",
				PropExtenstion13 = domainObject.PropExtenstion13 != null ? domainObject.PropExtenstion13 : "",
				PropExtenstion14 = domainObject.PropExtenstion14 != null ? domainObject.PropExtenstion14 : "",
				PropExtenstion15 = domainObject.PropExtenstion15 != null ? domainObject.PropExtenstion15 : "",
				PropExtenstion16 = domainObject.PropExtenstion16 != null ? domainObject.PropExtenstion16 : "",
				PropExtenstion17 = domainObject.PropExtenstion17 != null ? domainObject.PropExtenstion17 : "",
				PropExtenstion18 = domainObject.PropExtenstion18 != null ? domainObject.PropExtenstion18 : "",
				PropExtenstion19 = domainObject.PropExtenstion19 != null ? domainObject.PropExtenstion19 : "",
				PropExtenstion20 = domainObject.PropExtenstion20 != null ? domainObject.PropExtenstion20 : "",
				PropExtenstion21 = domainObject.PropExtenstion21 != null ? domainObject.PropExtenstion21 : "",
				PropExtenstion22 = domainObject.PropExtenstion22 != null ? domainObject.PropExtenstion22 : "",

				LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : "",
				LocationDescription = domainObject.LocationDescription != null ? domainObject.LocationDescription : "",
				LocationLevel1Code = domainObject.LocationLevel1Code != null ? domainObject.LocationLevel1Code : "",
				LocationLevel1Name = domainObject.LocationLevel1Name != null ? domainObject.LocationLevel1Name : "",
				LocationLevel2Code = domainObject.LocationLevel2Code != null ? domainObject.LocationLevel2Code : "",
				LocationLevel2Name = domainObject.LocationLevel2Name != null ? domainObject.LocationLevel2Name : "",
				LocationLevel3Code = domainObject.LocationLevel3Code != null ? domainObject.LocationLevel3Code : "",
				LocationLevel3Name = domainObject.LocationLevel3Name != null ? domainObject.LocationLevel3Name : "",
				LocationLevel4Code = domainObject.LocationLevel4Code != null ? domainObject.LocationLevel4Code : "",
				LocationLevel4Name = domainObject.LocationLevel4Name != null ? domainObject.LocationLevel4Name : "",
				LocationInvStatus = domainObject.LocationInvStatus != null ? domainObject.LocationInvStatus : "",
				LocationNodeType = domainObject.LocationNodeType != null ? domainObject.LocationNodeType : "",
				LocationLevelNum = domainObject.LocationLevelNum != null ? domainObject.LocationLevelNum : "",
				LocationTotal = domainObject.LocationTotal != null ? domainObject.LocationTotal : "",

				DateModified = domainObject.DateModified != null ? domainObject.DateModified : "",
				DateCreated = domainObject.DateCreated != null ? domainObject.DateCreated : "",
				ItemStatus = domainObject.ItemStatus != null ? domainObject.ItemStatus : "",
				ItemType = domainObject.ItemType != null ? domainObject.ItemType : "",
				UnitTypeCode = domainObject.UnitTypeCode != null ? domainObject.UnitTypeCode : "",

				CatalogItemCode = domainObject.CatalogItemCode != null ? domainObject.CatalogItemCode : "",
				CatalogItemName = domainObject.CatalogItemName != null ? domainObject.CatalogItemName : "",
				CatalogItemType = domainObject.CatalogItemType != null ? domainObject.CatalogItemType : "",
				CatalogFamilyCode = domainObject.CatalogFamilyCode != null ? domainObject.CatalogFamilyCode : "",
				CatalogFamilyName = domainObject.CatalogFamilyName != null ? domainObject.CatalogFamilyName : "",
				CatalogSectionCode = domainObject.CatalogSectionCode != null ? domainObject.CatalogSectionCode : "",
				CatalogSectionName = domainObject.CatalogSectionName != null ? domainObject.CatalogSectionName : "",
				CatalogSubSectionCode = domainObject.CatalogSubSectionCode != null ? domainObject.CatalogSubSectionCode : "",
				CatalogSubSectionName = domainObject.CatalogSubSectionName != null ? domainObject.CatalogSubSectionName : "",
				CatalogPriceBuy = domainObject.CatalogPriceBuy != null ? domainObject.CatalogPriceBuy : "",
				CatalogPriceSell = domainObject.CatalogPriceSell != null ? domainObject.CatalogPriceSell : "",
				CatalogSupplierCode = domainObject.CatalogSupplierCode != null ? domainObject.CatalogSupplierCode : "",
				CatalogSupplierName = domainObject.CatalogSupplierName != null ? domainObject.CatalogSupplierName : "",
				CatalogUnitTypeCode = domainObject.CatalogUnitTypeCode != null ? domainObject.CatalogUnitTypeCode : "",
				CatalogDescription = domainObject.CatalogDescription != null ? domainObject.CatalogDescription : "",

				TemporaryOldUid = domainObject.TemporaryOldUid != null ? domainObject.TemporaryOldUid : "",
				TemporaryNewUid = domainObject.TemporaryNewUid != null ? domainObject.TemporaryNewUid : "",

				TemporaryOldSerialNumber = domainObject.TemporaryOldSerialNumber != null ? domainObject.TemporaryOldSerialNumber : "",
				TemporaryOldItemCode = domainObject.TemporaryOldItemCode != null ? domainObject.TemporaryOldItemCode : "",
				TemporaryOldLocationCode = domainObject.TemporaryOldLocationCode != null ? domainObject.TemporaryOldLocationCode : "",
				TemporaryOldKey = domainObject.TemporaryOldKey != null ? domainObject.TemporaryOldKey : "",

				TemporaryNewSerialNumber = domainObject.TemporaryNewSerialNumber != null ? domainObject.TemporaryNewSerialNumber : "",
				TemporaryNewItemCode = domainObject.TemporaryNewItemCode != null ? domainObject.TemporaryNewItemCode : "",
				TemporaryNewLocationCode = domainObject.TemporaryNewLocationCode != null ? domainObject.TemporaryNewLocationCode : "",
				TemporaryNewKey = domainObject.TemporaryNewKey != null ? domainObject.TemporaryNewKey : "",

				TemporaryDateModified = domainObject.TemporaryDateModified != null ? domainObject.TemporaryDateModified : "",
				TemporaryOperation = domainObject.TemporaryOperation != null ? domainObject.TemporaryOperation : "",
				TemporaryDevice = domainObject.TemporaryDevice != null ? domainObject.TemporaryDevice : "",
				TemporaryDbFileName = domainObject.TemporaryDbFileName != null ? domainObject.TemporaryDbFileName : "",

				IturCode = domainObject.IturCode != null ? domainObject.IturCode : ""
			};
		}


		public static void ApplyChanges(this App_Data.CurrentInventoryAdvanced entity, CurrentInventoryAdvanced domainObject)
		{
			if (domainObject == null) return;
			entity.Id = domainObject.Id;

			entity.Uid = domainObject.Uid != null ? domainObject.Uid : "";
				entity.SerialNumberLocal = domainObject.SerialNumberLocal != null ? domainObject.SerialNumberLocal : "";
				entity.ItemCode = domainObject.ItemCode != null ? domainObject.ItemCode : "";
				entity.DomainObject = domainObject.DomainObject != null ? domainObject.DomainObject : "";
				entity.Table = domainObject.Table != null ? domainObject.Table : "";
				entity.Adapter = domainObject.Adapter != null ? domainObject.Adapter : "";
				entity.SerialNumberSupplier = domainObject.SerialNumberSupplier != null ? domainObject.SerialNumberSupplier : "";
				entity.Quantity = domainObject.Quantity != null ? domainObject.Quantity : "";
				entity.QuantityDouble = domainObject.QuantityDouble != null ? Convert.ToDouble(domainObject.QuantityDouble) : 0;

				entity.PropertyStr1 = domainObject.PropertyStr1 != null ? domainObject.PropertyStr1 : "";
				entity.PropertyStr1Name = domainObject.PropertyStr1Name != null ? domainObject.PropertyStr1Name : "";
				entity.PropertyStr1Code = domainObject.PropertyStr1Code != null ? domainObject.PropertyStr1Code : "";

				entity.PropertyStr2 = domainObject.PropertyStr2 != null ? domainObject.PropertyStr2 : "";
				entity.PropertyStr2Name = domainObject.PropertyStr2Name != null ? domainObject.PropertyStr2Name : "";
				entity.PropertyStr2Code = domainObject.PropertyStr2Code != null ? domainObject.PropertyStr2Code : "";

				entity.PropertyStr3 = domainObject.PropertyStr3 != null ? domainObject.PropertyStr3 : "";
				entity.PropertyStr3Name = domainObject.PropertyStr3Name != null ? domainObject.PropertyStr3Name : "";
				entity.PropertyStr3Code = domainObject.PropertyStr3Code != null ? domainObject.PropertyStr3Code : "";

				entity.PropertyStr4 = domainObject.PropertyStr4 != null ? domainObject.PropertyStr4 : "";
				entity.PropertyStr4Name = domainObject.PropertyStr4Name != null ? domainObject.PropertyStr4Name : "";
				entity.PropertyStr4Code = domainObject.PropertyStr4Code != null ? domainObject.PropertyStr4Code : "";

				entity.PropertyStr5 = domainObject.PropertyStr5 != null ? domainObject.PropertyStr5 : "";
				entity.PropertyStr5Name = domainObject.PropertyStr5Name != null ? domainObject.PropertyStr5Name : "";
				entity.PropertyStr5Code = domainObject.PropertyStr5Code != null ? domainObject.PropertyStr5Code : "";

				entity.PropertyStr6 = domainObject.PropertyStr6 != null ? domainObject.PropertyStr6 : "";
				entity.PropertyStr6Name = domainObject.PropertyStr6Name != null ? domainObject.PropertyStr6Name : "";
				entity.PropertyStr6Code = domainObject.PropertyStr6Code != null ? domainObject.PropertyStr6Code : "";

				entity.PropertyStr7 = domainObject.PropertyStr7 != null ? domainObject.PropertyStr7 : "";
				entity.PropertyStr7Name = domainObject.PropertyStr7Name != null ? domainObject.PropertyStr7Name : "";
				entity.PropertyStr7Code = domainObject.PropertyStr7Code != null ? domainObject.PropertyStr7Code : "";

				entity.PropertyStr8 = domainObject.PropertyStr8 != null ? domainObject.PropertyStr8 : "";
				entity.PropertyStr8Name = domainObject.PropertyStr8Name != null ? domainObject.PropertyStr8Name : "";
				entity.PropertyStr8Code = domainObject.PropertyStr8Code != null ? domainObject.PropertyStr8Code : "";

				entity.PropertyStr9 = domainObject.PropertyStr9 != null ? domainObject.PropertyStr9 : "";
				entity.PropertyStr9Name = domainObject.PropertyStr9Name != null ? domainObject.PropertyStr9Name : "";
				entity.PropertyStr9Code = domainObject.PropertyStr9Code != null ? domainObject.PropertyStr9Code : "";

				entity.PropertyStr10 = domainObject.PropertyStr10 != null ? domainObject.PropertyStr10 : "";
				entity.PropertyStr10Name = domainObject.PropertyStr10Name != null ? domainObject.PropertyStr10Name : "";
				entity.PropertyStr10Code = domainObject.PropertyStr10Code != null ? domainObject.PropertyStr10Code : "";

				entity.PropertyStr11 = domainObject.PropertyStr11 != null ? domainObject.PropertyStr11 : "";
				entity.PropertyStr11Name = domainObject.PropertyStr11Name != null ? domainObject.PropertyStr11Name : "";
				entity.PropertyStr11Code = domainObject.PropertyStr11Code != null ? domainObject.PropertyStr11Code : "";

				entity.PropertyStr12 = domainObject.PropertyStr12 != null ? domainObject.PropertyStr12 : "";
				entity.PropertyStr12Name = domainObject.PropertyStr12Name != null ? domainObject.PropertyStr12Name : "";
				entity.PropertyStr12Code = domainObject.PropertyStr12Code != null ? domainObject.PropertyStr12Code : "";

				entity.PropertyStr13 = domainObject.PropertyStr13 != null ? domainObject.PropertyStr13 : "";
				entity.PropertyStr13Name = domainObject.PropertyStr13Name != null ? domainObject.PropertyStr13Name : "";
				entity.PropertyStr13Code = domainObject.PropertyStr13Code != null ? domainObject.PropertyStr13Code : "";

				entity.PropertyStr14 = domainObject.PropertyStr14 != null ? domainObject.PropertyStr14 : "";
				entity.PropertyStr14Name = domainObject.PropertyStr14Name != null ? domainObject.PropertyStr14Name : "";
				entity.PropertyStr14Code = domainObject.PropertyStr14Code != null ? domainObject.PropertyStr14Code : "";

				entity.PropertyStr15 = domainObject.PropertyStr15 != null ? domainObject.PropertyStr15 : "";
				entity.PropertyStr15Name = domainObject.PropertyStr15Name != null ? domainObject.PropertyStr15Name : "";
				entity.PropertyStr15Code = domainObject.PropertyStr15Code != null ? domainObject.PropertyStr15Code : "";

				entity.PropertyStr16 = domainObject.PropertyStr16 != null ? domainObject.PropertyStr16 : "";
				entity.PropertyStr16Name = domainObject.PropertyStr16Name != null ? domainObject.PropertyStr16Name : "";
				entity.PropertyStr16Code = domainObject.PropertyStr16Code != null ? domainObject.PropertyStr16Code : "";

				entity.PropertyStr17 = domainObject.PropertyStr17 != null ? domainObject.PropertyStr17 : "";
				entity.PropertyStr17Name = domainObject.PropertyStr17Name != null ? domainObject.PropertyStr17Name : "";
				entity.PropertyStr17Code = domainObject.PropertyStr17Code != null ? domainObject.PropertyStr17Code : "";

				entity.PropertyStr18 = domainObject.PropertyStr18 != null ? domainObject.PropertyStr18 : "";
				entity.PropertyStr18Name = domainObject.PropertyStr18Name != null ? domainObject.PropertyStr18Name : "";
				entity.PropertyStr18Code = domainObject.PropertyStr18Code != null ? domainObject.PropertyStr18Code : "";

				entity.PropertyStr19 = domainObject.PropertyStr19 != null ? domainObject.PropertyStr19 : "";
				entity.PropertyStr19Name = domainObject.PropertyStr19Name != null ? domainObject.PropertyStr19Name : "";
				entity.PropertyStr19Code = domainObject.PropertyStr19Code != null ? domainObject.PropertyStr19Code : "";

				entity.PropertyStr20 = domainObject.PropertyStr20 != null ? domainObject.PropertyStr20 : "";
				entity.PropertyStr20Name = domainObject.PropertyStr20Name != null ? domainObject.PropertyStr20Name : "";
				entity.PropertyStr20Code = domainObject.PropertyStr20Code != null ? domainObject.PropertyStr20Code : "";

				entity.PropExtenstion1 = domainObject.PropExtenstion1 != null ? domainObject.PropExtenstion1 : "";
				entity.PropExtenstion2 = domainObject.PropExtenstion2 != null ? domainObject.PropExtenstion2 : "";
				entity.PropExtenstion3 = domainObject.PropExtenstion3 != null ? domainObject.PropExtenstion3 : "";
				entity.PropExtenstion4 = domainObject.PropExtenstion4 != null ? domainObject.PropExtenstion4 : "";
				entity.PropExtenstion5 = domainObject.PropExtenstion5 != null ? domainObject.PropExtenstion5 : "";
				entity.PropExtenstion6 = domainObject.PropExtenstion6 != null ? domainObject.PropExtenstion6 : "";
				entity.PropExtenstion7 = domainObject.PropExtenstion7 != null ? domainObject.PropExtenstion7 : "";
				entity.PropExtenstion8 = domainObject.PropExtenstion8 != null ? domainObject.PropExtenstion8 : "";
				entity.PropExtenstion9 = domainObject.PropExtenstion9 != null ? domainObject.PropExtenstion9 : "";
				entity.PropExtenstion10 = domainObject.PropExtenstion10 != null ? domainObject.PropExtenstion10 : "";
				entity.PropExtenstion11 = domainObject.PropExtenstion11 != null ? domainObject.PropExtenstion11 : "";
				entity.PropExtenstion12 = domainObject.PropExtenstion12 != null ? domainObject.PropExtenstion12 : "";
				entity.PropExtenstion13 = domainObject.PropExtenstion13 != null ? domainObject.PropExtenstion13 : "";
				entity.PropExtenstion14 = domainObject.PropExtenstion14 != null ? domainObject.PropExtenstion14 : "";
				entity.PropExtenstion15 = domainObject.PropExtenstion15 != null ? domainObject.PropExtenstion15 : "";
				entity.PropExtenstion16 = domainObject.PropExtenstion16 != null ? domainObject.PropExtenstion16 : "";
				entity.PropExtenstion17 = domainObject.PropExtenstion17 != null ? domainObject.PropExtenstion17 : "";
				entity.PropExtenstion18 = domainObject.PropExtenstion18 != null ? domainObject.PropExtenstion18 : "";
				entity.PropExtenstion19 = domainObject.PropExtenstion19 != null ? domainObject.PropExtenstion19 : "";
				entity.PropExtenstion20 = domainObject.PropExtenstion20 != null ? domainObject.PropExtenstion20 : "";
				entity.PropExtenstion21 = domainObject.PropExtenstion21 != null ? domainObject.PropExtenstion21 : "";
				entity.PropExtenstion22 = domainObject.PropExtenstion22 != null ? domainObject.PropExtenstion22 : "";

				entity.LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : "";
				entity.LocationDescription = domainObject.LocationDescription != null ? domainObject.LocationDescription : "";
				entity.LocationLevel1Code = domainObject.LocationLevel1Code != null ? domainObject.LocationLevel1Code : "";
				entity.LocationLevel1Name = domainObject.LocationLevel1Name != null ? domainObject.LocationLevel1Name : "";
				entity.LocationLevel2Code = domainObject.LocationLevel2Code != null ? domainObject.LocationLevel2Code : "";
				entity.LocationLevel2Name = domainObject.LocationLevel2Name != null ? domainObject.LocationLevel2Name : "";
				entity.LocationLevel3Code = domainObject.LocationLevel3Code != null ? domainObject.LocationLevel3Code : "";
				entity.LocationLevel3Name = domainObject.LocationLevel3Name != null ? domainObject.LocationLevel3Name : "";
				entity.LocationLevel4Code = domainObject.LocationLevel4Code != null ? domainObject.LocationLevel4Code : "";
				entity.LocationLevel4Name = domainObject.LocationLevel4Name != null ? domainObject.LocationLevel4Name : "";
				entity.LocationInvStatus = domainObject.LocationInvStatus != null ? domainObject.LocationInvStatus : "";
				entity.LocationNodeType = domainObject.LocationNodeType != null ? domainObject.LocationNodeType : "";
				entity.LocationLevelNum = domainObject.LocationLevelNum != null ? domainObject.LocationLevelNum : "";
				entity.LocationTotal = domainObject.LocationTotal != null ? domainObject.LocationTotal : "";

				entity.DateModified = domainObject.DateModified != null ? domainObject.DateModified : "";
				entity.DateCreated = domainObject.DateCreated != null ? domainObject.DateCreated : "";
				entity.ItemStatus = domainObject.ItemStatus != null ? domainObject.ItemStatus : "";
				entity.ItemType = domainObject.ItemType != null ? domainObject.ItemType : "";
				entity.UnitTypeCode = domainObject.UnitTypeCode != null ? domainObject.UnitTypeCode : "";

				entity.CatalogItemCode = domainObject.CatalogItemCode != null ? domainObject.CatalogItemCode : "";
				entity.CatalogItemName = domainObject.CatalogItemName != null ? domainObject.CatalogItemName : "";
				entity.CatalogItemType = domainObject.CatalogItemType != null ? domainObject.CatalogItemType : "";
				entity.CatalogFamilyCode = domainObject.CatalogFamilyCode != null ? domainObject.CatalogFamilyCode : "";
				entity.CatalogFamilyName = domainObject.CatalogFamilyName != null ? domainObject.CatalogFamilyName : "";
				entity.CatalogSectionCode = domainObject.CatalogSectionCode != null ? domainObject.CatalogSectionCode : "";
				entity.CatalogSectionName = domainObject.CatalogSectionName != null ? domainObject.CatalogSectionName : "";
				entity.CatalogSubSectionCode = domainObject.CatalogSubSectionCode != null ? domainObject.CatalogSubSectionCode : "";
				entity.CatalogSubSectionName = domainObject.CatalogSubSectionName != null ? domainObject.CatalogSubSectionName : "";
				entity.CatalogPriceBuy = domainObject.CatalogPriceBuy != null ? domainObject.CatalogPriceBuy : "";
				entity.CatalogPriceSell = domainObject.CatalogPriceSell != null ? domainObject.CatalogPriceSell : "";
				entity.CatalogSupplierCode = domainObject.CatalogSupplierCode != null ? domainObject.CatalogSupplierCode : "";
				entity.CatalogSupplierName = domainObject.CatalogSupplierName != null ? domainObject.CatalogSupplierName : "";
				entity.CatalogUnitTypeCode = domainObject.CatalogUnitTypeCode != null ? domainObject.CatalogUnitTypeCode : "";
				entity.CatalogDescription = domainObject.CatalogDescription != null ? domainObject.CatalogDescription : "";

				entity.TemporaryOldUid = domainObject.TemporaryOldUid != null ? domainObject.TemporaryOldUid : "";
				entity.TemporaryNewUid = domainObject.TemporaryNewUid != null ? domainObject.TemporaryNewUid : "";

				entity.TemporaryOldSerialNumber = domainObject.TemporaryOldSerialNumber != null ? domainObject.TemporaryOldSerialNumber : "";
				entity.TemporaryOldItemCode = domainObject.TemporaryOldItemCode != null ? domainObject.TemporaryOldItemCode : "";
				entity.TemporaryOldLocationCode = domainObject.TemporaryOldLocationCode != null ? domainObject.TemporaryOldLocationCode : "";
				entity.TemporaryOldKey = domainObject.TemporaryOldKey != null ? domainObject.TemporaryOldKey : "";

				entity.TemporaryNewSerialNumber = domainObject.TemporaryNewSerialNumber != null ? domainObject.TemporaryNewSerialNumber : "";
				entity.TemporaryNewItemCode = domainObject.TemporaryNewItemCode != null ? domainObject.TemporaryNewItemCode : "";
				entity.TemporaryNewLocationCode = domainObject.TemporaryNewLocationCode != null ? domainObject.TemporaryNewLocationCode : "";
				entity.TemporaryNewKey = domainObject.TemporaryNewKey != null ? domainObject.TemporaryNewKey : "";

				entity.TemporaryDateModified = domainObject.TemporaryDateModified != null ? domainObject.TemporaryDateModified : "";
				entity.TemporaryOperation = domainObject.TemporaryOperation != null ? domainObject.TemporaryOperation : "";
				entity.TemporaryDevice = domainObject.TemporaryDevice != null ? domainObject.TemporaryDevice : "";
				entity.TemporaryDbFileName = domainObject.TemporaryDbFileName != null ? domainObject.TemporaryDbFileName : "";

				entity.IturCode = domainObject.IturCode != null ? domainObject.IturCode : "";
		}
	}
}
