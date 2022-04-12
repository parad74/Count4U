using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.Mapping
{
	public static class StatusInventorMapper
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
		public static StatusInventor ToDomainObject(this StatusInventor entity)
		{
			if (entity == null) return null;
			return new StatusInventor()
			{
				ID = entity.ID,
				Code = entity.Code,
				Description = entity.Description,
				Name = entity.Name
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
		public static StatusInventor ToSimpleDomainObject(this StatusInventor entity)
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
		public static StatusInventor ToEntity(this StatusInventor domainObject)
        {
			if (domainObject == null) return null;
			return new StatusInventor()
            {
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = domainObject.Description,
				Name = domainObject.Name
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
		public static void ApplyChanges(this StatusInventor entity, StatusInventor domainObject)
        {
			if (domainObject == null) return;
 			entity.ID = domainObject.ID;
			entity.Code = domainObject.Code;
			entity.Description = domainObject.Description;
			entity.Name = domainObject.Name;
        }
    }
}
