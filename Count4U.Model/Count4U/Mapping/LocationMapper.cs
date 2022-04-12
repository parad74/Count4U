using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
{
    public static class LocationMapper
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
        public static Location ToDomainObject(this Location entity)
        {
			if (entity == null) return null;
            return new Location()
            {
                ID = entity.ID,
                BackgroundColor = entity.BackgroundColor,
                Code = entity.Code,
                Description = entity.Description,
                Name = entity.Name,
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
        public static Location ToSimpleDomainObject(this Location entity)
        {
            return new Location()
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
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
        public static Location ToEntity(this Location domainObject)
        {
			if (domainObject == null) return null;
            return new Location()
            {
                ID = domainObject.ID,
                BackgroundColor = domainObject.BackgroundColor,
                Code = domainObject.Code,
                Description = domainObject.Description,
                Name = domainObject.Name,
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
        public static void ApplyChanges(this Location entity, Location domainObject)
        {
			if (domainObject == null) return;
            entity.BackgroundColor = domainObject.BackgroundColor;
            entity.Description = domainObject.Description;
            entity.Name = domainObject.Name;
        }
    }
}
