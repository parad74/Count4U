using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class StatusInventorConfigMapper
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
		public static StatusInventorConfig ToDomainObject(this App_Data.StatusInventorConfig entity)
		{
			if (entity == null) return null;
			return new StatusInventorConfig()
			{
				ID = entity.ID,
				Code = entity.Code,
				Description = entity.Description,
				Name = entity.Name,
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
		public static StatusInventorConfig ToSimpleDomainObject(this App_Data.StatusInventorConfig entity)
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
		public static App_Data.StatusInventorConfig ToEntity(this StatusInventorConfig domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.StatusInventorConfig()
            {
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = domainObject.Description,
				Name = domainObject.Name,
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
		public static void ApplyChanges(this App_Data.StatusInventorConfig entity, StatusInventorConfig domainObject)
        {
			if (domainObject == null) return;
				entity.ID = domainObject.ID;
				entity.Code = domainObject.Code;
				entity.Description = domainObject.Description;
				entity.Name = domainObject.Name;
				entity.Bit = domainObject.Bit;
        }
    }
}
