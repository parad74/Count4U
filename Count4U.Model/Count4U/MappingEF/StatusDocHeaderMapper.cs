using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class StatusDocHeaderMapper
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
        public static StatusDocHeader ToDomainObject(this App_Data.StatusDocHeader entity)
        {
			if (entity == null) return null;
            return new StatusDocHeader()
            {
                ID = entity.ID,
				Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
				Bit = entity.Bit
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
        public static StatusDocHeader ToSimpleDomainObject(this App_Data.StatusDocHeader entity)
        {
            return new StatusDocHeader()
            {
                ID = entity.ID,
				Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
				Bit = entity.Bit
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
        public static App_Data.StatusDocHeader ToEntity(this StatusDocHeader domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.StatusDocHeader()
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
        public static void ApplyChanges(this App_Data.StatusDocHeader entity, StatusDocHeader domainObject)
        {
			if (domainObject == null) return;
			entity.Code = domainObject.Code;
            entity.Name = domainObject.Name;
            entity.Description = domainObject.Description;
			entity.Bit = domainObject.Bit;
        }
    }
}
