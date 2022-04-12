using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.Mapping
{
	public static class CustomerConfigMapper
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
		public static CustomerConfig ToDomainObject(this CustomerConfig entity)
        {
			if (entity == null) return null;
			return new CustomerConfig()
            {
                ID = entity.ID, 
				CustomerCode = entity.CustomerCode,
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
		public static CustomerConfig ToSimpleDomainObject(this CustomerConfig entity)
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
		public static CustomerConfig ToEntity(this CustomerConfig domainObject)
        {
			if (domainObject == null) return null;
			return new CustomerConfig()
            {
				ID = domainObject.ID,
				CustomerCode = domainObject.CustomerCode,
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
		public static void ApplyChanges(this CustomerConfig entity, CustomerConfig domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.Description = domainObject.Description;
			entity.Name = domainObject.Name;
		}
    }
}
