using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class InventorConfigMapper
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
		public static InventorConfig ToDomainObject(this App_Data.InventorConfig entity)
		{
			if (entity == null) return null;
			return new InventorConfig()
			{
				ID = entity.ID,
				Code = entity.Code,
				InventorCode = entity.InventorCode,
				InventorDate = entity.InventorDate,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				Description = entity.Description,
				CustomerCode = entity.CustomerCode,
				CustomerName = entity.CustomerName,
				BranchCode = entity.BranchCode,
				BranchName = entity.BranchName,
				IsDirty = entity.IsDirty,
				ModifyDate = entity.ModifyDate,
				//StatusInventorConfigID = entity.StatusInventorConfigID,
				StatusInventorConfigCode = entity.StatusInventorCode,
				DBPath = entity.DBPath,
				TypeObject = entity.TypeObject 
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
		public static InventorConfig ToSimpleDomainObject(this App_Data.InventorConfig entity)
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
		public static App_Data.InventorConfig ToEntity(this InventorConfig domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.InventorConfig()
			{
				ID = domainObject.ID,
				Code = domainObject.Code,
				InventorCode = domainObject.InventorCode,
				InventorDate = domainObject.InventorDate,
				CreateDate = domainObject.CreateDate,
				Description = domainObject.Description,
				CustomerCode = domainObject.CustomerCode,
				CustomerName = domainObject.CustomerName,
				BranchCode = domainObject.BranchCode,
				BranchName = domainObject.BranchName,
				IsDirty = domainObject.IsDirty,
				ModifyDate = domainObject.ModifyDate,
				//StatusInventorConfigID = domainObject.StatusInventorConfigID,
				StatusInventorCode = domainObject.StatusInventorConfigCode,
				DBPath = domainObject.DBPath,
				TypeObject = domainObject.TypeObject

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
		public static void ApplyChanges(this App_Data.InventorConfig entity, InventorConfig domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Code = domainObject.Code;
			entity.InventorCode = domainObject.InventorCode;
			entity.InventorDate = domainObject.InventorDate;
			entity.CreateDate = domainObject.CreateDate;
			entity.Description = domainObject.Description;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.CustomerName = domainObject.CustomerName;
			entity.BranchCode = domainObject.BranchCode;
			entity.BranchName = domainObject.BranchName;
			entity.IsDirty = domainObject.IsDirty;
			entity.ModifyDate = domainObject.ModifyDate;
			//entity.StatusInventorConfigID = domainObject.StatusInventorConfigID;
			entity.StatusInventorCode = domainObject.StatusInventorConfigCode;
			entity.DBPath = domainObject.DBPath;
			entity.TypeObject = domainObject.TypeObject;
		}


		public static App_Data.Branch ToBranchEntity(this InventorConfig domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Branch()
			{
				Code = domainObject.Code,
				CustomerCode = domainObject.CustomerCode,
				DBPath = domainObject.DBPath,
				Description = domainObject.Description,
				Name = domainObject.BranchName
			};
		}

		public static App_Data.Customer ToCustomerEntity(this InventorConfig domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Customer()
			{
				Code = domainObject.Code,
				DBPath = domainObject.DBPath,
				Description = domainObject.Description,
				Name = domainObject.BranchName 
			};
		}

		public static App_Data.Inventor ToInventorEntity(this InventorConfig domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Inventor()
			{
				Code = domainObject.Code,
				DBPath = domainObject.DBPath,
				Description = domainObject.Description,
				Name = domainObject.InventorName,
				InventorDate = domainObject.InventorDate,
				CustomerCode = domainObject.CustomerCode,
				BranchCode = domainObject.BranchCode
			};
		}

	}
}
