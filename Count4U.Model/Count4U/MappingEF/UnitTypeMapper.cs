using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class UnitTypeMapper
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
        public static UnitType ToDomainObject(this App_Data.UnitType entity)
        {
			if (entity == null) return null;
            return new UnitType()
            {
                ID = entity.ID,
                Name = entity.Name,
				Description = entity.Description,
				UnitTypeCode = entity.UnitTypeCode 
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
        public static UnitType ToSimpleDomainObject(this App_Data.UnitType entity)
        {
            return new UnitType()
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
				UnitTypeCode = entity.UnitTypeCode 
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
        public static App_Data.UnitType ToEntity(this UnitType domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.UnitType()
            {
                ID = domainObject.ID,
                Name = domainObject.Name,
                Description = domainObject.Description,
				UnitTypeCode = domainObject.UnitTypeCode 
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
        public static void ApplyChanges(this App_Data.UnitType entity, UnitType domainObject)
        {
			if (domainObject == null) return;
            entity.Name = domainObject.Name;
            entity.Description = domainObject.Description;
			entity.UnitTypeCode = domainObject.UnitTypeCode;
        }
    }
}
