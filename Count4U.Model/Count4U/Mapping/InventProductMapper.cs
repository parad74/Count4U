using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
{
    public static class InventProductMapper
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
        public static InventProduct ToDomainObject(this InventProduct entity)
        {
			if (entity == null) return null;
            return new InventProduct()
            {
                ID = entity.ID,
                Barcode = entity.Barcode,
                CreateDate = entity.CreateDate,
                 PartialPackage = entity.PartialPackage,
                QuantityDifference = entity.QuantityDifference,
                QuantityEdit = entity.QuantityEdit,
                QuantityOriginal = entity.QuantityOriginal,
                SerialNumber = entity.SerialNumber,
                ShelfCode = entity.ShelfCode,
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
        public static InventProduct ToSimpleDomainObject(this InventProduct entity)
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
        public static InventProduct ToEntity(this InventProduct domainObject)
        {
			if (domainObject == null) return null;
            return new InventProduct()
            {
                ID = domainObject.ID,
                Barcode = domainObject.Barcode,
                CreateDate = domainObject.CreateDate,
                PartialPackage = domainObject.PartialPackage,
                QuantityDifference = domainObject.QuantityDifference,
                QuantityEdit = domainObject.QuantityEdit,
                QuantityOriginal = domainObject.QuantityOriginal,
                SerialNumber = domainObject.SerialNumber,
                ShelfCode = domainObject.ShelfCode,
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
        public static void ApplyChanges(this InventProduct entity, InventProduct domainObject)
        {
			if (domainObject == null) return;
            entity.Barcode = domainObject.Barcode;
            entity.CreateDate = domainObject.CreateDate;
            entity.PartialPackage = domainObject.PartialPackage;
            entity.QuantityDifference = domainObject.QuantityDifference;
            entity.QuantityEdit = domainObject.QuantityEdit;
            entity.QuantityOriginal = domainObject.QuantityOriginal;
            entity.SerialNumber = domainObject.SerialNumber;
            entity.ShelfCode = domainObject.ShelfCode;
           }
    }
}
