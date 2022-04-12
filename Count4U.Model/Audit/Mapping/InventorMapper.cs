using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.Mapping
{
    public static class InventorMapper
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
		public static Inventor ToDomainObject(this Inventor entity)
		{
			if (entity == null) return null;
			return new Inventor()
			{
				ID = entity.ID,
				BranchCode = entity.BranchCode,
				Code = entity.Code,
				CreateDate = entity.CreateDate,
				CompleteDate = entity.CompleteDate,
				CustomerCode = entity.CustomerCode,
				Description = entity.Description,
				ImportCatalogAdapterCode = entity.ImportCatalogAdapterCode,
				ImportIturAdapterCode = entity.ImportIturAdapterCode,
				ImportLocationAdapterCode = entity.ImportLocationAdapterCode,
				ImportCatalogParms = entity.ImportCatalogParms,
				ImportIturParms = entity.ImportIturParms,
				ImportLocationParms = entity.ImportLocationParms,
				InventorDate = entity.InventorDate,
				Name = entity.Name,
				Status = entity.Status,
				StatusCode = entity.StatusCode

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
		public static Inventor ToSimpleDomainObject(this Inventor entity)
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
		public static Inventor ToEntity(this Inventor domainObject)
        {
			if (domainObject == null) return null;
			return new Inventor()
			{
				ID = domainObject.ID,
				BranchCode = domainObject.BranchCode,
				Code = domainObject.Code,
				CreateDate = domainObject.CreateDate,
				CompleteDate = domainObject.CompleteDate,
				CustomerCode = domainObject.CustomerCode,
				Description = domainObject.Description,
				ImportCatalogAdapterCode = domainObject.ImportCatalogAdapterCode,
				ImportIturAdapterCode = domainObject.ImportIturAdapterCode,
				ImportLocationAdapterCode = domainObject.ImportLocationAdapterCode,
				ImportCatalogParms = domainObject.ImportCatalogParms,
				ImportIturParms = domainObject.ImportIturParms,
				ImportLocationParms = domainObject.ImportLocationParms,
				InventorDate = domainObject.InventorDate,
				Name = domainObject.Name,
				Status = domainObject.Status,
				StatusCode = domainObject.StatusCode
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
		public static void ApplyChanges(this Inventor entity, Inventor domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.BranchCode = domainObject.BranchCode;
			entity.Code = domainObject.Code;
			entity.CreateDate = domainObject.CreateDate;
			entity.CompleteDate = domainObject.CompleteDate;
			entity.CustomerCode = domainObject.CustomerCode;
			entity.Description = domainObject.Description;
			entity.ImportCatalogAdapterCode = domainObject.ImportCatalogAdapterCode;
			entity.ImportIturAdapterCode = domainObject.ImportIturAdapterCode;
			entity.ImportLocationAdapterCode = domainObject.ImportLocationAdapterCode;
			entity.ImportCatalogParms = domainObject.ImportCatalogParms;
			entity.ImportIturParms = domainObject.ImportIturParms;
			entity.ImportLocationParms = domainObject.ImportLocationParms;
			entity.InventorDate = domainObject.InventorDate;
			entity.Name = domainObject.Name;
			entity.Status = domainObject.Status;
			entity.StatusCode = domainObject.StatusCode;

		}
    }
}
