using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Common;


namespace Count4U.Model.Count4U.MappingEF
{
	public static class ProductMapper
	{
		/// <summary>
		/// Конвертация в объект предметной области.
		/// 
		/// Converting to domain object.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <returns>
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </returns>
		public static Product ToDomainObject(this App_Data.Product entity)
		{
			if (entity == null) return null;
			return new Product()
			{
				ID = entity.ID,
				BalanceQuantityERP = Convert.ToDouble(entity.BalanceQuantityERP),
				BalanceQuantityPartialERP = Convert.ToInt32(entity.BalanceQuantityPartialERP) ,
 				Barcode = entity.Barcode,
				Tag = entity.Tag,
				CountMax = entity.CountMax,
				CountMin = entity.CountMin,
				Description = entity.Description,
				Family = entity.Family,
				FamilyCode = entity.FamilyCode,
				Importance = entity.Importance,
				InputTypeCode = entity.InputTypeCode != null ? entity.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				Makat = entity.Makat,
				MakatERP = entity.MakatERP,
				Name = entity.Name,
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceExtra = Convert.ToDouble(entity.PriceExtra),
				PriceSale = Convert.ToDouble(entity.PriceSale),
				PriceString = entity.PriceString,
				SectionCode = entity.SectionCode,
				SupplierCode = entity.SupplierCode,
				TypeCode = entity.TypeCode,
				UnitTypeCode = entity.UnitTypeCode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				ModifyDate = entity.ModifyDate,
				CountInParentPack = entity.CountInParentPack != 0 ? Convert.ToInt32(entity.CountInParentPack) : 1,
				Code = entity.Code,
				ParentMakat = entity.ParentMakat,
				ParserBag = entity.ParserBag,
				MakatOriginal = entity.MakatOriginal,
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				IsUpdateERP = Convert.ToBoolean(entity.IsUpdateERP),
				IturCodeExpected = entity.IturCodeExpected ,
				SectionName = entity.SectionName != null ? entity.SectionName : "",
				SubSectionCode = entity.SubSectionCode != null ? entity.SubSectionCode : "",
				SubSectionName = entity.SubSectionName != null ? entity.SubSectionName : "",
				SupplierName = entity.SupplierName != null ? entity.SupplierName : "",
				ItemType = entity.ItemType != null ? entity.ItemType : ""
				//ParentBarcode = entity.ParentBarcode,
				//ParentCode = entity.ParentCode,
				//Section = entity.Section,
				//Supplier = entity.Supplier
		

			};
		}

		/// <summary>
		/// Конвертация в упрощенный объект предметной области.
		/// 
		/// Converting to simple domain object.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <returns>
		/// Упрощенный объект предметной области.
		/// 
		/// Simple domain object.
		/// </returns>
		public static ProductSimple ToSimpleDomainObject(this App_Data.Product entity)
		{
			if (entity == null) return null;
			return new ProductSimple()
			{
				Makat = entity.Makat,
				Code  = entity.Code,
				Name = entity.Name,
				Family = entity.Family != null ? entity.Family : "",
				FamilyCode = entity.FamilyCode != null ? entity.FamilyCode : "",
				//PriceSale = entity.PriceSale != null ? Convert.ToDouble(entity.PriceSale) : 0.0,
				PriceSale = Convert.ToDouble(entity.PriceSale),
				PriceBuy = Convert.ToDouble(entity.PriceBuy),
				PriceExtra = Convert.ToDouble(entity.PriceExtra),
				PriceString = entity.PriceString,
				SupplierCode = entity.SupplierCode,
				TypeCode = entity.TypeCode,
				ParentMakat = entity.ParentMakat,
				MakatOriginal = entity.MakatOriginal,
				CountInParentPack = entity.CountInParentPack == 0 ? 1 : Convert.ToInt32(entity.CountInParentPack),
				FromCatalogType = Convert.ToInt32(entity.FromCatalogType),
				InputTypeCode = entity.InputTypeCode != null ? entity.InputTypeCode :  InputTypeCodeEnum.B.ToString(),
				SectionCode = entity.SectionCode != null ? entity.SectionCode : "",
				UnitTypeCode = entity.UnitTypeCode != null ? entity.UnitTypeCode : "",
				BalanceQuantityERP = Convert.ToDouble(entity.BalanceQuantityERP) ,
				IsUpdateERP = Convert.ToBoolean(entity.IsUpdateERP),
				IturCodeExpected = entity.IturCodeExpected != null ? entity.IturCodeExpected : "",
				Description = entity.Description != null ? entity.Description : ""	,
	
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
		public static App_Data.Product ToEntity(this Product domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Product()
			{
				ID = domainObject.ID,
				BalanceQuantityERP =Convert.ToDouble(domainObject.BalanceQuantityERP),
				BalanceQuantityPartialERP = Convert.ToInt32(domainObject.BalanceQuantityPartialERP),
				Barcode = domainObject.Barcode.CutLength(299),
				Tag = domainObject.Tag,
				CountMax = domainObject.CountMax,
				CountMin = domainObject.CountMin,
				Description = domainObject.Description,
				Family = domainObject.Family,
				FamilyCode = domainObject.FamilyCode,
				Importance = domainObject.Importance,
				Makat = domainObject.Makat.CutLength(299),
				MakatERP = domainObject.MakatERP,
				Name = domainObject.Name.CutLength(99),
				PriceBuy = domainObject.PriceBuy,
				PriceExtra = domainObject.PriceExtra,
				PriceSale = domainObject.PriceSale,
				PriceString = domainObject.PriceString,
				SectionCode = domainObject.SectionCode,
				SupplierCode = domainObject.SupplierCode,
				TypeCode = domainObject.TypeCode,
				UnitTypeCode = domainObject.UnitTypeCode,
				InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString(),
				CreateDate = domainObject.CreateDate,
				ModifyDate = domainObject.ModifyDate,
				CountInParentPack = domainObject.CountInParentPack != null ? Convert.ToInt32(domainObject.CountInParentPack) : 1,
				Code = domainObject.Code.CutLength(299),
				ParentMakat = domainObject.ParentMakat.CutLength(299),
				ParserBag = domainObject.ParserBag,
				MakatOriginal = domainObject.MakatOriginal.CutLength(299),
				FromCatalogType = domainObject.FromCatalogType	,
				IsUpdateERP = domainObject.IsUpdateERP,
				IturCodeExpected = domainObject.IturCodeExpected  ,
				SectionName =  domainObject.SectionName != null ? domainObject.SectionName : ""	,
				SubSectionCode = domainObject.SubSectionCode != null ? domainObject.SubSectionCode : ""  ,
				SubSectionName = domainObject.SubSectionName != null ? domainObject.SubSectionName : ""  ,
				SupplierName= domainObject.SupplierName != null ? domainObject.SupplierName : ""	,
				ItemType = domainObject.ItemType != null ? domainObject.ItemType : "" 
			};
		}

		public static App_Data.Product CopyEntity(this App_Data.Product domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Product()
			{
				//ID = domainObject.ID,
				BalanceQuantityERP = Convert.ToDouble(domainObject.BalanceQuantityERP),
				BalanceQuantityPartialERP = Convert.ToInt32(domainObject.BalanceQuantityPartialERP),
				Barcode = domainObject.Barcode,
				Tag = domainObject.Tag,
				CountMax = domainObject.CountMax,
				CountMin = domainObject.CountMin,
				Description = domainObject.Description,
				Family = domainObject.Family,
				Importance = domainObject.Importance,
				Makat = domainObject.Makat,
				MakatERP = domainObject.MakatERP,
				Name = domainObject.Name,
				PriceBuy = domainObject.PriceBuy,
				PriceExtra = domainObject.PriceExtra,
				PriceSale = domainObject.PriceSale,
				PriceString = domainObject.PriceString,
				SectionCode = domainObject.SectionCode,
				SupplierCode = domainObject.SupplierCode,
				TypeCode = domainObject.TypeCode,
				UnitTypeCode = domainObject.UnitTypeCode,
				InputTypeCode = domainObject.InputTypeCode,
				CreateDate = domainObject.CreateDate,
				ModifyDate = domainObject.ModifyDate,
				CountInParentPack = domainObject.CountInParentPack != 0 ? Convert.ToInt32(domainObject.CountInParentPack) : 1,
				Code = domainObject.Code,
				ParentMakat = domainObject.ParentMakat,
				ParserBag = domainObject.ParserBag,
				MakatOriginal = domainObject.MakatOriginal,
				FromCatalogType = domainObject.FromCatalogType,
				IsUpdateERP = domainObject.IsUpdateERP,
				IturCodeExpected = domainObject.IturCodeExpected,
				SectionName = domainObject.SectionName ,
				SubSectionCode = domainObject.SubSectionCode ,
				SubSectionName = domainObject.SubSectionName,
				SupplierName = domainObject.SupplierName,
				ItemType = domainObject.ItemType 

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
		public static void ApplyChanges(this App_Data.Product entity, Product domainObject)
		{
			if (domainObject == null) return;
			entity.BalanceQuantityERP = Convert.ToDouble(domainObject.BalanceQuantityERP);
			entity.BalanceQuantityPartialERP =Convert.ToInt32(domainObject.BalanceQuantityPartialERP);
			entity.Barcode = domainObject.Barcode.CutLength(299);
			entity.Tag = domainObject.Tag;
			entity.CountMax = domainObject.CountMax;
			entity.CountMin = domainObject.CountMin;
			entity.Description = domainObject.Description;
			entity.Family = domainObject.Family;
			entity.FamilyCode = domainObject.FamilyCode;
			entity.Importance = domainObject.Importance;
			entity.Makat = domainObject.Makat.CutLength(299);
			entity.MakatERP = domainObject.MakatERP;
			entity.Name = domainObject.Name.CutLength(99);
			entity.PriceBuy = domainObject.PriceBuy;
			entity.PriceExtra = domainObject.PriceExtra;
			entity.PriceSale = domainObject.PriceSale;
			entity.PriceString = domainObject.PriceString;
			entity.SectionCode = domainObject.SectionCode;
			entity.SupplierCode = domainObject.SupplierCode;
			entity.TypeCode = domainObject.TypeCode;
			entity.UnitTypeCode = domainObject.UnitTypeCode;
			entity.InputTypeCode = domainObject.InputTypeCode != null ? domainObject.InputTypeCode : InputTypeCodeEnum.B.ToString();
			entity.CreateDate = domainObject.CreateDate;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.CountInParentPack = domainObject.CountInParentPack != null ? Convert.ToInt32(domainObject.CountInParentPack) : 1;
			entity.Code = domainObject.Code.CutLength(299);
			entity.ParentMakat = domainObject.ParentMakat.CutLength(299);
			entity.ParserBag = domainObject.ParserBag;
			entity.MakatOriginal = domainObject.MakatOriginal.CutLength(299);
			entity.FromCatalogType = domainObject.FromCatalogType;
			entity.IsUpdateERP = domainObject.IsUpdateERP;
			entity.IturCodeExpected = domainObject.IturCodeExpected;
			entity.SectionName =  domainObject.SectionName != null ? domainObject.SectionName : ""	;
			entity.SubSectionCode = domainObject.SubSectionCode != null ? domainObject.SubSectionCode : ""  ;
			entity.SubSectionName = domainObject.SubSectionName != null ? domainObject.SubSectionName : "" ;
			entity.SupplierName= domainObject.SupplierName != null ? domainObject.SupplierName : "";
			entity.ItemType = domainObject.ItemType != null ? domainObject.ItemType : "";


		}

		
	}
}
