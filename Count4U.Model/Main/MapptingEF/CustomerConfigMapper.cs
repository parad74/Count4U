using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model;

namespace Count4U.Model.Main.MappingEF
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
		public static CustomerConfig ToDomainObject(this App_Data.CustomerConfig entity)
        {
			if (entity == null) return null;
			return new CustomerConfig()
            {
                ID = entity.ID,
				CustomerCode = entity.CustomerCode != null ? entity.CustomerCode : "",
				Value = entity.Value = entity.Value != null ? entity.Value : "",
				Description = entity.Description != null ? entity.Description : "",
				Name = entity.Name != null ? entity.Name : "",
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
		public static CustomerConfig ToSimpleDomainObject(this App_Data.CustomerConfig entity)
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
		public static App_Data.CustomerConfig ToEntity(this CustomerConfig domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.CustomerConfig()
            {
				ID = domainObject.ID,
				CustomerCode = domainObject.CustomerCode != null ? domainObject.CustomerCode : "",
				Value = domainObject.Value != null ? domainObject.Value : "",
				Description = domainObject.Description != null ? domainObject.Description : "",
				Name = domainObject.Name != null ? domainObject.Name : "",
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
		public static void ApplyChanges(this App_Data.CustomerConfig entity, CustomerConfig domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.CustomerCode =  domainObject.CustomerCode != null ? domainObject.CustomerCode : "";
			entity.Value = domainObject.Value != null ? domainObject.Value : "";
			entity.Description = domainObject.Description != null ? domainObject.Description : "";
			entity.Name = domainObject.Name != null ? domainObject.Name : "";
		}

    }
}
