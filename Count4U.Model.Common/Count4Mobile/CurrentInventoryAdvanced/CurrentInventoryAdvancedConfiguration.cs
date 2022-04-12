using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration;

namespace Count4U.Model.Count4Mobile
{
	public class CurrentInventoryAdvancedConfiguration : EntityTypeConfiguration<CurrentInventoryAdvanced>
	{
		public CurrentInventoryAdvancedConfiguration()
		{
			this.ToTable("CurrentInventoryAdvanced");
			this.Property(p => p.Uid).HasMaxLength(250).IsRequired();
			this.Property(p => p.SerialNumberLocal).HasMaxLength(250).IsRequired();
			this.Property(p => p.ItemCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.DomainObject).HasMaxLength(50).IsRequired();
			this.Property(p => p.Table).HasMaxLength(50).IsRequired();
			this.Property(p => p.Adapter).HasMaxLength(50).IsRequired();

			this.Property(p => p.SerialNumberSupplier).HasMaxLength(50).IsRequired();
			this.Property(p => p.Quantity).HasMaxLength(50).IsRequired();
			this.Property(p => p.QuantityDouble).IsRequired();
			this.Property(p => p.PropertyStr1).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr1Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr1Name).HasMaxLength(100).IsRequired();

			this.Property(p => p.PropertyStr2).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr2Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr2Name).HasMaxLength(100).IsRequired();

			this.Property(p => p.PropertyStr3).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr3Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr3Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr4).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr4Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr4Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr5).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr5Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr5Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr6).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr6Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr6Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr7).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr7Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr7Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr8).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr8Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr8Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr9).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr9Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr9Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr10).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr10Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr10Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr11).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr11Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr11Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr12).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr12Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr12Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr13).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr13Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr13Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr14).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr14Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr14Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr15).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr15Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr15Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr16).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr16Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr16Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr17).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr17Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr17Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr18).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr18Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr18Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr19).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr19Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr19Name).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr20).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr20Code).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropertyStr20Name).HasMaxLength(100).IsRequired();

			this.Property(p => p.PropExtenstion1).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion2).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion3).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion4).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion5).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion6).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion7).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion8).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion9).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion10).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion11).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion12).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion13).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion14).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion15).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion16).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion17).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion18).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion19).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion20).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion21).HasMaxLength(100).IsRequired();
			this.Property(p => p.PropExtenstion22).HasMaxLength(100).IsRequired();
			this.Property(p => p.LocationCode).HasMaxLength(250).IsRequired();
			this.Property(p => p.LocationDescription).HasMaxLength(500).IsRequired();
			this.Property(p => p.LocationLevel1Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationLevel1Name).HasMaxLength(250).IsRequired();
			this.Property(p => p.LocationLevel2Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationLevel2Name).HasMaxLength(250).IsRequired();
			this.Property(p => p.LocationLevel3Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationLevel3Name).HasMaxLength(250).IsRequired();
			this.Property(p => p.LocationLevel4Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationLevel4Name).HasMaxLength(250).IsRequired();
			this.Property(p => p.LocationInvStatus).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationNodeType).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationLevelNum).HasMaxLength(50).IsRequired();
			this.Property(p => p.LocationTotal).HasMaxLength(50).IsRequired();
			this.Property(p => p.DateModified).HasMaxLength(50).IsRequired();
			this.Property(p => p.DateCreated).HasMaxLength(50).IsRequired();
			// public string ItemStatus { get; set; }

			this.Property(p => p.CatalogItemCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogItemName).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogItemType).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogFamilyCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogFamilyName).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSectionCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSectionName).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSubSectionCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSubSectionName).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogPriceBuy).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogPriceSell).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSupplierCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogSupplierName).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogUnitTypeCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.CatalogDescription).HasMaxLength(50).IsRequired();


			this.Property(p => p.TemporaryOldUid).HasMaxLength(250).IsRequired();			  //parent
			this.Property(p => p.TemporaryNewUid).HasMaxLength(250).IsRequired();			//	chalde

			this.Property(p => p.TemporaryOldSerialNumber).HasMaxLength(250).IsRequired();
			this.Property(p => p.TemporaryOldItemCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.TemporaryOldLocationCode).HasMaxLength(250).IsRequired();
			this.Property(p => p.TemporaryOldKey).HasMaxLength(50).IsRequired();

			this.Property(p => p.TemporaryNewSerialNumber).HasMaxLength(250).IsRequired();
			this.Property(p => p.TemporaryNewItemCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.TemporaryNewLocationCode).HasMaxLength(250).IsRequired();
			this.Property(p => p.TemporaryNewKey).HasMaxLength(50).IsRequired();

			this.Property(p => p.TemporaryDateModified).HasMaxLength(50).IsRequired();
			this.Property(p => p.TemporaryOperation).HasMaxLength(50).IsRequired();
			this.Property(p => p.TemporaryDevice).HasMaxLength(50).IsRequired();
			this.Property(p => p.TemporaryDbFileName).HasMaxLength(50).IsRequired();

			this.Property(p => p.IturCode).HasMaxLength(50).IsRequired();
			//this.Property(p => p.Tag).HasMaxLength(250).IsRequired();
			
		}
	}
}

