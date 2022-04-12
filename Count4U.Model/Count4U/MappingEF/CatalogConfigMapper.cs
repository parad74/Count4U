using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class CatalogConfigMapper
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
		public static CatalogConfig ToDomainObject(this App_Data.CatalogConfig entity)
		{
			if (entity == null) return null;
			return new CatalogConfig()
			{
				ID = entity.ID,
				InventorCode = entity.InventorCode,
				CustomerCode = entity.CustomerCode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				BranchCode = entity.BranchCode,
				ModifyDate = entity.ModifyDate,
				Description = entity.Description,
				Tag = entity.Tag
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
		public static CatalogConfig ToSimpleDomainObject(this App_Data.CatalogConfig entity)
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
		public static App_Data.CatalogConfig ToEntity(this CatalogConfig domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.CatalogConfig()
			{
				ID = domainObject.ID,
				InventorCode = domainObject.InventorCode,
				CustomerCode = domainObject.CustomerCode,
				CreateDate = domainObject.CreateDate,
				BranchCode = domainObject.BranchCode,
				ModifyDate = domainObject.ModifyDate,
				Description = domainObject.Description,
				Tag = domainObject.Tag

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
		public static void ApplyChanges(this App_Data.CatalogConfig entity, CatalogConfig domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.InventorCode = domainObject.InventorCode;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.CreateDate = domainObject.CreateDate;
			entity.BranchCode = domainObject.BranchCode;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.Description = domainObject.Description;
			entity.Tag = domainObject.Tag;
		}
	}
}
