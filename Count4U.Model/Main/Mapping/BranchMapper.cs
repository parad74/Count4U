using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.Mapping
{
	public static class BranchMapper
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
		public static Branch ToDomainObject(this Branch entity)
		{
			if (entity == null) return null;
			return new Branch()
			{
				ID = entity.ID,
				Address = entity.Address,
				Code = entity.Code,
				ContactPerson = entity.ContactPerson,
				CustomerCode = entity.CustomerCode,
				Description = entity.Description,
				Fax = entity.Fax,
				LogoFile = entity.LogoFile,
				Mail = entity.Mail,
				Name = entity.Name,
				Phone = entity.Phone
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
		public static Branch ToSimpleDomainObject(this Branch entity)
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
		public static Branch ToEntity(this Branch domainObject)
		{
			if (domainObject == null) return null;
			return new Branch()
			{
				ID = domainObject.ID,
				Address = domainObject.Address,
				Code = domainObject.Code,
				ContactPerson = domainObject.ContactPerson,
				CustomerCode = domainObject.CustomerCode,
				Description = domainObject.Description,
				Fax = domainObject.Fax,
				LogoFile = domainObject.LogoFile,
				Mail = domainObject.Mail,
				Name = domainObject.Name,
				Phone = domainObject.Phone

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
		public static void ApplyChanges(this Branch entity, Branch domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Address = domainObject.Address;
			entity.Code = domainObject.Code;
			entity.ContactPerson = domainObject.ContactPerson;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.Description = domainObject.Description;
			entity.Fax = domainObject.Fax;
			entity.LogoFile = domainObject.LogoFile;
			entity.Mail = domainObject.Mail;
			entity.Name = domainObject.Name;
			entity.Phone = domainObject.Phone;
		}
	}
}
