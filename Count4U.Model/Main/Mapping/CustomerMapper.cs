using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.Mapping
{
	public static class CustomerMapper
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
		public static Customer ToDomainObject(this Customer entity)
		{
			if (entity == null) return null;
			return new Customer()
			{
				ID = entity.ID,
				Address = entity.Address,
				Code = entity.Code,
				ContactPerson = entity.ContactPerson,
				Description = entity.Description,
				Fax = entity.Fax,
				Mail = entity.Mail,
				Name = entity.Name,
				Phone = entity.Phone,
				Logo = entity.Logo
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
		public static Customer ToSimpleDomainObject(this Customer entity)
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
		public static Customer ToEntity(this Customer domainObject)
        {
			if (domainObject == null) return null;
			return new Customer()
            {
				ID = domainObject.ID,
				Address = domainObject.Address,
				Code = domainObject.Code,
				ContactPerson = domainObject.ContactPerson,
				Description = domainObject.Description,
				Fax = domainObject.Fax,
				Mail = domainObject.Mail,
				Name = domainObject.Name,
				Phone = domainObject.Phone ,
				Logo = domainObject.Logo
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
		public static void ApplyChanges(this Customer entity, Customer domainObject)
        {
			if (domainObject == null) return;
            entity.ID = domainObject.ID; 
			entity.Address = domainObject.Address;
			entity.Code = domainObject.Code;
			entity.ContactPerson = domainObject.ContactPerson;
			entity.Description = domainObject.Description;
			entity.Fax = domainObject.Fax;
			entity.Mail = domainObject.Mail;
			entity.Name = domainObject.Name;
			entity.Phone = domainObject.Phone;
			entity.Logo = domainObject.Logo;
        }
    }
}
