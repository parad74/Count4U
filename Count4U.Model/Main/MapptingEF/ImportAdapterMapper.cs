using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.MappingEF
{
	public static class ImportAdapterMapper
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
		public static ImportAdapter ToDomainObject(this App_Data.ImportAdapter entity)
        {
			if (entity == null) return null;
			return new ImportAdapter()
            {
                ID = entity.ID,
				Description = entity.Description,
				Code = entity.Code,
				AdapterCode = entity.AdapterCode,
				DomainType = entity.DomainType
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
		public static ImportAdapter ToSimpleDomainObject(this App_Data.ImportAdapter entity)
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
		public static App_Data.ImportAdapter ToEntity(this ImportAdapter domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.ImportAdapter()
            {
				ID = domainObject.ID,
				Description = domainObject.Description,
				Code = domainObject.Code,
				AdapterCode = domainObject.AdapterCode,
				DomainType = domainObject.DomainType
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
		public static void ApplyChanges(this App_Data.ImportAdapter entity, ImportAdapter domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Description = domainObject.Description;
			entity.Code = domainObject.Code;
			entity.AdapterCode = domainObject.AdapterCode;
			entity.DomainType = domainObject.DomainType;
		}
    }
}
