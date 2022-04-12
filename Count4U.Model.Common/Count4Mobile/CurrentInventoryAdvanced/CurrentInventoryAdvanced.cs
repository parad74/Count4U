using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Count4U.Model.Count4Mobile
{
	[Table("CurrentInventoryAdvanced")] 
	public class CurrentInventoryAdvanced
    {
  		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
        public string Uid { get; set; }
        public string SerialNumberLocal { get; set; }
        public string ItemCode { get; set; }
		public string DomainObject { get; set; }
		public string Table { get; set; }
		public string Adapter { get; set; }
        public string SerialNumberSupplier { get; set; }
        public string Quantity { get; set; }
		public double QuantityDouble { get; set; }
        public string PropertyStr1 { get; set; }
		public string PropertyStr1Code { get; set; }
		public string PropertyStr1Name { get; set; }
        public string PropertyStr2 { get; set; }
		public string PropertyStr2Code { get; set; }
		public string PropertyStr2Name { get; set; }
        public string PropertyStr3 { get; set; }
		public string PropertyStr3Code { get; set; }
		public string PropertyStr3Name { get; set; }
        public string PropertyStr4 { get; set; }
		public string PropertyStr4Code { get; set; }
		public string PropertyStr4Name { get; set; }
        public string PropertyStr5 { get; set; }
		public string PropertyStr5Code { get; set; }
		public string PropertyStr5Name { get; set; }
        public string PropertyStr6 { get; set; }
		public string PropertyStr6Code { get; set; }
		public string PropertyStr6Name { get; set; }
        public string PropertyStr7 { get; set; }
		public string PropertyStr7Code { get; set; }
		public string PropertyStr7Name { get; set; }
        public string PropertyStr8 { get; set; }
		public string PropertyStr8Code { get; set; }
		public string PropertyStr8Name { get; set; }
        public string PropertyStr9 { get; set; }
		public string PropertyStr9Code { get; set; }
		public string PropertyStr9Name { get; set; }
        public string PropertyStr10 { get; set; }
		public string PropertyStr10Code { get; set; }
		public string PropertyStr10Name { get; set; }
        public string PropertyStr11 { get; set; }
		public string PropertyStr11Code { get; set; }
		public string PropertyStr11Name { get; set; }
        public string PropertyStr12 { get; set; }
		public string PropertyStr12Code { get; set; }
		public string PropertyStr12Name { get; set; }
        public string PropertyStr13 { get; set; }
		public string PropertyStr13Code { get; set; }
		public string PropertyStr13Name { get; set; }
        public string PropertyStr14 { get; set; }
		public string PropertyStr14Code { get; set; }
		public string PropertyStr14Name { get; set; }
        public string PropertyStr15 { get; set; }
		public string PropertyStr15Code { get; set; }
		public string PropertyStr15Name { get; set; }
        public string PropertyStr16 { get; set; }
		public string PropertyStr16Code { get; set; }
		public string PropertyStr16Name { get; set; }
        public string PropertyStr17 { get; set; }
		public string PropertyStr17Code { get; set; }
		public string PropertyStr17Name { get; set; }
        public string PropertyStr18 { get; set; }
		public string PropertyStr18Code { get; set; }
		public string PropertyStr18Name { get; set; }
        public string PropertyStr19 { get; set; }
		public string PropertyStr19Code { get; set; }
		public string PropertyStr19Name { get; set; }
        public string PropertyStr20 { get; set; }
		public string PropertyStr20Code { get; set; }
		public string PropertyStr20Name { get; set; }

		public string PropExtenstion1 { get; set; }
		public string PropExtenstion2 { get; set; }
		public string PropExtenstion3 { get; set; }
		public string PropExtenstion4 { get; set; }
		public string PropExtenstion5 { get; set; }
		public string PropExtenstion6 { get; set; }
		public string PropExtenstion7 { get; set; }
		public string PropExtenstion8 { get; set; }
		public string PropExtenstion9 { get; set; }
		public string PropExtenstion10 { get; set; }
		public string PropExtenstion11 { get; set; }
		public string PropExtenstion12 { get; set; }
		public string PropExtenstion13 { get; set; }
		public string PropExtenstion14 { get; set; }
		public string PropExtenstion15 { get; set; }
		public string PropExtenstion16 { get; set; }
		public string PropExtenstion17 { get; set; }
		public string PropExtenstion18 { get; set; }
		public string PropExtenstion19 { get; set; }
		public string PropExtenstion20 { get; set; }
		public string PropExtenstion21 { get; set; }
		public string PropExtenstion22 { get; set; }

        public string LocationCode { get; set; }
		public string LocationDescription { get; set; }
		public string LocationLevel1Code { get; set; }
		public string LocationLevel1Name { get; set; }
		public string LocationLevel2Code { get; set; }
		public string LocationLevel2Name { get; set; }
		public string LocationLevel3Code { get; set; }
		public string LocationLevel3Name { get; set; }
		public string LocationLevel4Code { get; set; }
		public string LocationLevel4Name { get; set; }
		public string LocationInvStatus { get; set; }
		public string LocationNodeType { get; set; }
		public string LocationLevelNum { get; set; }
		public string LocationTotal { get; set; }

        public string DateModified { get; set; }
        public string DateCreated { get; set; }
        public string ItemStatus { get; set; }

		public string ItemType { get; set; }
		public string UnitTypeCode { get; set; }

        public string CatalogItemCode { get; set; }
        public string CatalogItemName { get; set; }
        public string CatalogItemType { get; set; }
		public string CatalogFamilyCode { get; set; }					
        public string CatalogFamilyName { get; set; }
        public string CatalogSectionCode { get; set; }				   
		public string CatalogSectionName { get; set; }					
        public string CatalogSubSectionCode { get; set; }
        public string CatalogSubSectionName { get; set; }
        public string CatalogPriceBuy { get; set; }
        public string CatalogPriceSell { get; set; }
        public string CatalogSupplierCode { get; set; }
        public string CatalogSupplierName { get; set; }
        public string CatalogUnitTypeCode { get; set; }
        public string CatalogDescription { get; set; }

		public string TemporaryOldUid { get; set; }
		public string TemporaryNewUid { get; set; }
		public string TemporaryOldSerialNumber { get; set; }
		public string TemporaryOldItemCode { get; set; }
		public string TemporaryOldLocationCode { get; set; }
		public string TemporaryOldKey { get; set; }

		public string TemporaryNewSerialNumber { get; set; }
		public string TemporaryNewItemCode { get; set; }
		public string TemporaryNewLocationCode { get; set; }
		public string TemporaryNewKey { get; set; }

		public string TemporaryDateModified { get; set; }
		public string TemporaryOperation { get; set; }
		public string TemporaryDevice { get; set; }
		public string TemporaryDbFileName { get; set; }
		public string IturCode { get; set; }
		//public string Tag { get; set; }

		public CurrentInventoryAdvanced()
		{
			this.Uid = Guid.NewGuid().ToString();
			this.SerialNumberLocal = "";
			this.ItemCode = "";
			this.DomainObject = "";
			this.Table = "";
			this.Adapter = "";
			this.SerialNumberSupplier = "";
			this.Quantity = "";
			this.QuantityDouble = 0;

			this.PropertyStr1 = "";
			this.PropertyStr1Name = "";
			this.PropertyStr1Code = "";
			this.PropertyStr2 = "";
			this.PropertyStr2Name = "";
			this.PropertyStr2Code = "";

			this.PropertyStr3 = "";
			this.PropertyStr3Name = "";
			this.PropertyStr3Code = "";

			this.PropertyStr4 = "";
			this.PropertyStr4Name = "";
			this.PropertyStr4Code = "";

			this.PropertyStr5 = "";
			this.PropertyStr5Name = "";
			this.PropertyStr5Code = "";

			this.PropertyStr6 = "";
			this.PropertyStr6Name = "";
			this.PropertyStr6Code = "";

			this.PropertyStr7 = "";
			this.PropertyStr7Name = "";
			this.PropertyStr7Code = "";

			this.PropertyStr8 = "";
			this.PropertyStr8Name = "";
			this.PropertyStr8Code = "";

			this.PropertyStr9 = "";
			this.PropertyStr9Name = "";
			this.PropertyStr9Code = "";

			this.PropertyStr10 = "";
			this.PropertyStr10Name = "";
			this.PropertyStr10Code = "";

			this.PropertyStr11 = "";
			this.PropertyStr11Name = "";
			this.PropertyStr11Code = "";

			this.PropertyStr12 = "";
			this.PropertyStr12Name = "";
			this.PropertyStr12Code = "";

			this.PropertyStr13 = "";
			this.PropertyStr13Name = "";
			this.PropertyStr13Code = "";

			this.PropertyStr14 = "";
			this.PropertyStr14Name = "";
			this.PropertyStr14Code = "";

			this.PropertyStr15 = "";
			this.PropertyStr15Name = "";
			this.PropertyStr15Code = "";

			this.PropertyStr16 = "";
			this.PropertyStr16Name = "";
			this.PropertyStr16Code = "";

			this.PropertyStr17 = "";
			this.PropertyStr17Name = "";
			this.PropertyStr17Code = "";

			this.PropertyStr18 = "";
			this.PropertyStr18Name = "";
			this.PropertyStr18Code = "";

			this.PropertyStr19 = "";
			this.PropertyStr19Name = "";
			this.PropertyStr19Code = "";

			this.PropertyStr20 = "";
			this.PropertyStr20Name = "";
			this.PropertyStr20Code = "";

			this.PropExtenstion1 = "";
			this.PropExtenstion2 = "";
			this.PropExtenstion3 = "";
			this.PropExtenstion4 = "";
			this.PropExtenstion5 = "";
			this.PropExtenstion6 = "";
			this.PropExtenstion7 = "";
			this.PropExtenstion8 = "";
			this.PropExtenstion9 = "";
			this.PropExtenstion10 = "";
			this.PropExtenstion11 = "";
			this.PropExtenstion12 = "";
			this.PropExtenstion13 = "";
			this.PropExtenstion14 = "";
			this.PropExtenstion15 = "";
			this.PropExtenstion16 = "";
			this.PropExtenstion17 = "";
			this.PropExtenstion18 = "";
			this.PropExtenstion19 = "";
			this.PropExtenstion20 = "";
			this.PropExtenstion21 = "";
			this.PropExtenstion22 = "";

			this.LocationCode = "";
			this.LocationDescription = "";
			this.LocationLevel1Code = "";
			this.LocationLevel1Name = "";
			this.LocationLevel2Code = "";
			this.LocationLevel2Name = "";
			this.LocationLevel3Code = "";
			this.LocationLevel3Name = "";
			this.LocationLevel4Code = "";
			this.LocationLevel4Name = "";
			this.LocationInvStatus = "";
 			this.LocationNodeType = "";
			this.LocationLevelNum = "";
			this.LocationTotal = "";

			this.DateModified = "";
			this.DateCreated = "";
			this.ItemStatus = "";
			this.ItemType = "";
			this.UnitTypeCode = "";

			this.CatalogItemCode = "";
			this.CatalogItemName = "";
			this.CatalogItemType = "";
			this.CatalogFamilyCode = "";
			this.CatalogFamilyName = "";
			this.CatalogSectionCode = "";
			this.CatalogSectionName = "";
			this.CatalogSubSectionCode = "";
			this.CatalogSubSectionName = "";
			this.CatalogPriceBuy = "";
			this.CatalogPriceSell = "";
			this.CatalogSupplierCode = "";
			this.CatalogSupplierName = "";
			this.CatalogUnitTypeCode = "";
			this.CatalogDescription = "";

			this.TemporaryOldUid = "";			  //parent
			this.TemporaryNewUid = "";			//	chalde

			this.TemporaryOldSerialNumber = "";
			this.TemporaryOldItemCode = "";
			this.TemporaryOldLocationCode = "";
			this.TemporaryOldKey = "";

			this.TemporaryNewSerialNumber = "";
			this.TemporaryNewItemCode = "";
			this.TemporaryNewLocationCode = "";
			this.TemporaryNewKey = "";

			this.TemporaryDateModified = "";
			this.TemporaryOperation = "";
			this.TemporaryDevice = "";
			this.TemporaryDbFileName = "";

			this.IturCode = "";
			//this.Tag = "";
			
	
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(CurrentInventoryAdvanced)) return false;
			return Equals((CurrentInventoryAdvanced)obj);
		}

		public bool Equals(CurrentInventoryAdvanced other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.Uid, this.Uid));
		}

		public override int GetHashCode()
		{
			return (Uid != null ? Uid.GetHashCode() : 0);
		}
    }
}
