using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.MappingEF
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
		public static AuditConfig ToDomainObject(this App_Data.AuditConfig entity)
		{
			if (entity == null) return null;
			return new AuditConfig()
			{
				ID = entity.ID,
				Code = entity.Code,
				Description = entity.Description,
				BranchCode = entity.BranchCode,
				BranchName = entity.BranchName,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				CustomerCode = entity.CustomerCode,
				CustomerName = entity.CustomerName,
				InventorCode = entity.InventorCode,
				InventorDate = entity.InventorDate,
				InventorName = entity.InventorName,
				StatusInventor = entity.StatusAuditConfig,
				StatusInventorCode = entity.StatusInventorCode,
				DBPath = entity.DBPath,
				StatusAuditConfig = entity.StatusAuditConfig

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
		public static AuditConfig ToSimpleDomainObject(this App_Data.AuditConfig entity)
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
		public static App_Data.AuditConfig ToEntity(this AuditConfig domainObject)
        {
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			return new App_Data.AuditConfig()
            {
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = description,
				BranchCode = domainObject.BranchCode,
				BranchName = domainObject.BranchName,
				CreateDate = domainObject.CreateDate,
				CustomerCode = domainObject.CustomerCode,
				CustomerName = domainObject.CustomerName,
				InventorCode = domainObject.InventorCode,
				InventorDate = domainObject.InventorDate,
				InventorName = domainObject.InventorName,
				StatusAuditConfig = domainObject.StatusAuditConfig,
				StatusInventorCode = domainObject.StatusInventorCode,
				DBPath = domainObject.DBPath,
		

            };
        }

		public static App_Data.AuditConfig ToAuditConfigEntity(this Inventor domainObject)
		{
			if (domainObject == null) return null;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);
			return new App_Data.AuditConfig()
			{
				ID = domainObject.ID,
				Code = domainObject.Code,
				Description = description,
				BranchCode = domainObject.BranchCode,
				CreateDate = domainObject.CreateDate,
				CustomerCode = domainObject.CustomerCode,
				InventorCode = domainObject.Code,
				InventorDate = domainObject.InventorDate,
				DBPath = domainObject.DBPath,
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
		public static void ApplyChanges(this App_Data.AuditConfig entity, AuditConfig domainObject)
        {
			if (domainObject == null) return;
			string description = domainObject.Description != null ? domainObject.Description : "";
			if (description.Length > 249) description = description.Substring(0, 249);

				//entity.ID = domainObject.ID;
				entity.Code = domainObject.Code;
				entity.Description = description;
				entity.BranchCode = domainObject.BranchCode;
				entity.BranchName = domainObject.BranchName;
				entity.CreateDate = domainObject.CreateDate;
				entity.CustomerCode = domainObject.CustomerCode;
				entity.CustomerName = domainObject.CustomerName;
				entity.InventorCode = domainObject.InventorCode;
				entity.InventorDate = domainObject.InventorDate;
				entity.InventorName = domainObject.InventorName;
				entity.StatusAuditConfig = domainObject.StatusAuditConfig;
				entity.StatusInventorCode = domainObject.StatusInventorCode;
				entity.DBPath = domainObject.DBPath;

        }
    }
}
