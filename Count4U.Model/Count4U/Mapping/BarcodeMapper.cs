﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
{
    public static class BarcodeMapper
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
        public static Barcode ToDomainObject(this Barcode entity)
        {
			if (entity == null) return null;
            return new Barcode()
            {
                ID = entity.ID,
				ProductName = entity.ProductName,
                ProductID = entity.ProductID,
                Value = entity.Value
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
        public static Barcode ToSimpleDomainObject(this Barcode entity)
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
        public static Barcode ToEntity(this Barcode domainObject)
        {
			if (domainObject == null) return null;
            return new Barcode()
            {
                ID = domainObject.ID,
				ProductName = domainObject.ProductName,
                ProductID = domainObject.ProductID,
                Value = domainObject.Value
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
        public static void ApplyChanges(this Barcode entity, Barcode domainObject)
        {
			if (domainObject == null) return;
			entity.ProductName = domainObject.ProductName;
            entity.ProductID = domainObject.ProductID;
            entity.Value = domainObject.Value;
        }
    }
}
