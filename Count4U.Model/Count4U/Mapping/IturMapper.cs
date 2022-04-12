using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
{
    public static class IturMapper
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
        public static Itur ToDomainObject(this Itur entity)
        {
			if (entity == null) return null;
            return new Itur()
            {
                ID = entity.ID,
                Approve = entity.Approve,
				Disabled = entity.Disabled,
                IturCode = entity.IturCode,
				//ERPIturCode = entity.ERPIturCode,
                Description = entity.Description,
                InitialQuantityMakatExpected = entity.InitialQuantityMakatExpected,
                LocationCode = entity.LocationCode,
                 Name = entity.Name,
                Number = entity.Number
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
        public static Itur ToSimpleDomainObject(this Itur entity)
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
        public static Itur ToEntity(this Itur domainObject)
        {
			if (domainObject == null) return null;
            return new Itur()
            {
                ID = domainObject.ID,
                Approve = domainObject.Approve,
				Disabled = domainObject.Disabled,
                IturCode = domainObject.IturCode,
                Description = domainObject.Description,
                InitialQuantityMakatExpected = domainObject.InitialQuantityMakatExpected,
                LocationCode = domainObject.LocationCode,
                 Name = domainObject.Name,
                Number = domainObject.Number
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
        public static void ApplyChanges(this Itur entity, Itur domainObject)
        {
			if (domainObject == null) return;
            entity.Approve = domainObject.Approve;
			entity.Disabled = domainObject.Disabled;
            entity.Description = domainObject.Description;
            entity.InitialQuantityMakatExpected = domainObject.InitialQuantityMakatExpected;
           // entity.Location = domainObject.Location;
            entity.LocationCode = domainObject.LocationCode;
              entity.Name = domainObject.Name;
            entity.Number = domainObject.Number;
           }
    }
}
