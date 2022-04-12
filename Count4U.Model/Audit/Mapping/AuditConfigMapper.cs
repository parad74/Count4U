using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.Mapping
{
	public static class AuditConfigMapper
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
		public static AuditConfig ToDomainObject(this AuditConfig entity)
		{
			if (entity == null) return null;
			return new AuditConfig()
			{
				ID = entity.ID,
				Code = entity.Code,
				Description = entity.Description,
				BranchCode = entity.BranchCode,
				BranchName = entity.BranchName,
				CreateDate = entity.CreateDate,
				CustomerCode = entity.CustomerCode,
				CustomerName = entity.CustomerName,
				DirtyCode = entity.DirtyCode,
				InventorCode = entity.InventorCode,
				InventorDate = entity.InventorDate,
				InventorName = entity.InventorName,
				IsDirty = entity.IsDirty,
				StatusInventor = entity.StatusInventor,
				StatusInventorCode = entity.StatusInventorCode,
				DirtyBranchCode = entity.DirtyBranchCode,
				DBPath = entity.DBPath,
				DirtyInventorCode = entity.DirtyInventorCode

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
		public static AuditConfig ToSimpleDomainObject(this AuditConfig entity)
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
		public static AuditConfig ToEntity(this AuditConfig domainObject)
        {
			if (domainObject == null) return null;
			return new AuditConfig()
            {
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = domainObject.Description,
				BranchCode = domainObject.BranchCode,
				BranchName = domainObject.BranchName,
				CreateDate = domainObject.CreateDate,
				CustomerCode = domainObject.CustomerCode,
				CustomerName = domainObject.CustomerName,
				DirtyCode = domainObject.DirtyCode,
				InventorCode = domainObject.InventorCode,
				InventorDate = domainObject.InventorDate,
				InventorName = domainObject.InventorName,
				IsDirty = domainObject.IsDirty,
				StatusInventor = domainObject.StatusInventor,
				StatusInventorCode = domainObject.StatusInventorCode,
				DirtyBranchCode = domainObject.DirtyBranchCode,
				DBPath = domainObject.DBPath,
				DirtyInventorCode = domainObject.DirtyInventorCode

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
		public static void ApplyChanges(this AuditConfig entity, AuditConfig domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Code = domainObject.Code;
			entity.Description = domainObject.Description;
			entity.BranchCode = domainObject.BranchCode;
			entity.BranchName = domainObject.BranchName;
			entity.CreateDate = domainObject.CreateDate;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.CustomerName = domainObject.CustomerName;
			entity.DirtyCode = domainObject.DirtyCode;
			entity.InventorCode = domainObject.InventorCode;
			entity.InventorDate = domainObject.InventorDate;
			entity.InventorName = domainObject.InventorName;
			entity.IsDirty = domainObject.IsDirty;
			entity.StatusInventor = domainObject.StatusInventor;
			entity.StatusInventorCode = domainObject.StatusInventorCode;
			entity.DirtyBranchCode = domainObject.DirtyBranchCode;
			entity.DBPath = domainObject.DBPath;
			entity.DirtyInventorCode = domainObject.DirtyInventorCode;

		}
    }
}
