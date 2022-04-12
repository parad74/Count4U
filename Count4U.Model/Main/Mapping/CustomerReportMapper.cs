using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.Mapping
{
	public static class CustomerReportMapper
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
		public static CustomerReport ToDomainObject(this CustomerReport entity)
		{
			if (entity == null) return null;
			return new CustomerReport()
			{
				ID = entity.ID,
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
		public static CustomerReport ToSimpleDomainObject(this CustomerReport entity)
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
		public static CustomerReport ToEntity(this CustomerReport domainObject)
		{
			if (domainObject == null) return null;
			return new CustomerReport()
			{
				ID = domainObject.ID,
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
		public static void ApplyChanges(this CustomerReport entity, CustomerReport domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Description = domainObject.Description;
			entity.Name = domainObject.Name;
		}
	}
}
