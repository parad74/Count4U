using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
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
        public static Product ToDomainObject(this Product entity)
        {
			if (entity == null) return null;
            return new Product()
            {
                ID = entity.ID,										   
                BalanceQuantityERP = entity.BalanceQuantityERP,
                BalanceQuantityPartialERP = entity.BalanceQuantityPartialERP,
				CountInParentPack = entity.CountInParentPack,
                Barcode = entity.Barcode,
                CountMax = entity.CountMax,
                CountMin = entity.CountMin,
                Description = entity.Description,
                Family = entity.Family,
				FamilyCode = entity.FamilyCode,
                Importance = entity.Importance,
                 Makat = entity.Makat,
                MakatERP = entity.MakatERP,
                Name = entity.Name,
                PriceBuy = entity.PriceBuy,
                PriceExtra = entity.PriceExtra,
                PriceSale = entity.PriceSale,
                PriceString = entity.PriceString
				//Section = entity.Section,
				// Supplier = entity.Supplier,
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
        public static Product ToSimpleDomainObject(this Product entity)
        {
            throw new NotImplementedException();
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
        public static Product ToEntity(this Product domainObject)
        {
			if (domainObject == null) return null;
            return new Product()
            {
                ID = domainObject.ID,
                BalanceQuantityERP = domainObject.BalanceQuantityERP,
                BalanceQuantityPartialERP = domainObject.BalanceQuantityPartialERP,
				CountInParentPack = domainObject.CountInParentPack,
                Barcode = domainObject.Barcode,
                  CountMax = domainObject.CountMax,
                CountMin = domainObject.CountMin,
                Description = domainObject.Description,
                Family = domainObject.Family,
				FamilyCode = domainObject.FamilyCode,
                Importance = domainObject.Importance,
                Makat = domainObject.Makat,
                MakatERP = domainObject.MakatERP,
                Name = domainObject.Name,
                PriceBuy = domainObject.PriceBuy,
                PriceExtra = domainObject.PriceExtra,
                PriceSale = domainObject.PriceSale,
                PriceString = domainObject.PriceString,
				//Section = domainObject.Section,
				//Supplier = domainObject.Supplier,
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
        public static void ApplyChanges(this Product entity, Product domainObject)
        {
			if (domainObject == null) return;
            entity.BalanceQuantityERP = domainObject.BalanceQuantityERP;
            entity.BalanceQuantityPartialERP = domainObject.BalanceQuantityPartialERP;
			entity.CountInParentPack = domainObject.CountInParentPack;
            entity.Barcode = domainObject.Barcode;
             entity.CountMax = domainObject.CountMax;
            entity.CountMin = domainObject.CountMin;
            entity.Description = domainObject.Description;
            entity.Family = domainObject.Family;
			 entity.FamilyCode = domainObject.FamilyCode;
            entity.Importance = domainObject.Importance;
            entity.Makat = domainObject.Makat;
            entity.MakatERP = domainObject.MakatERP;
            entity.Name = domainObject.Name;
            entity.PriceBuy = domainObject.PriceBuy;
            entity.PriceExtra = domainObject.PriceExtra;
            entity.PriceSale = domainObject.PriceSale;
            entity.PriceString = domainObject.PriceString;
			//entity.Section = domainObject.Section;
			//entity.Supplier = domainObject.Supplier;
        }
    }
}
