using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class StatusIturMapper
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
        public static StatusItur ToDomainObject(this App_Data.StatusItur entity)
        {
			if (entity == null) return null;
            return new StatusItur()
            {
                ID = entity.ID,
                Name = entity.Name,
                Description = entity.Description,
				Bit = Convert.ToInt32(entity.Bit),
				Code = entity.Code 

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
        public static StatusItur ToSimpleDomainObject(this App_Data.StatusItur entity)
        {
            return new StatusItur()
            {
                ID = entity.ID,
				Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
				Bit = Convert.ToInt32(entity.Bit)
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
        public static App_Data.StatusItur ToEntity(this StatusItur domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.StatusItur()
            {
                ID = domainObject.ID,
				Code = domainObject.Code,
                Name = domainObject.Name,
                Description = domainObject.Description,
				Bit = domainObject.Bit
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
        public static void ApplyChanges(this App_Data.StatusItur entity, StatusItur domainObject)
        {
			if (domainObject == null) return;
            entity.Name = domainObject.Name;
			entity.Code = domainObject.Code;
            entity.Description = domainObject.Description;
			entity.Description = domainObject.Description;
        }
    }
}
