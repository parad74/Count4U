using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class SectionMapper
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
        public static Section ToDomainObject(this App_Data.Section entity)
        {
			if (entity == null) return null;
            return new Section()
            {
                ID = entity.ID,
                Name = entity.Name,
				Description = entity.Description,
				SectionCode = entity.SectionCode  ,
				ParentSectionCode = entity.ParentSectionCode,
				Tag = entity.Tag,
				TypeCode = string.IsNullOrWhiteSpace(entity.TypeCode) == false ? entity.TypeCode : TypeSectionEnum.S.ToString(),
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
        public static Section ToSimpleDomainObject(this App_Data.Section entity)
        {
            return new Section()
            {
                ID = entity.ID,
                Name = entity.Name,
				Description = entity.Description,
				SectionCode = entity.SectionCode,
				ParentSectionCode = entity.ParentSectionCode,
				Tag = entity.Tag,
				TypeCode = string.IsNullOrWhiteSpace(entity.TypeCode) == false ? entity.TypeCode : TypeSectionEnum.S.ToString()
            };
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
        public static App_Data.Section ToEntity(this Section domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.Section()
            {
                ID = domainObject.ID,
                Name = domainObject.Name,
				Description = domainObject.Description,
				SectionCode = domainObject.SectionCode ,
				ParentSectionCode = domainObject.ParentSectionCode,
				Tag = domainObject.Tag,
				TypeCode = string.IsNullOrWhiteSpace(domainObject.TypeCode) == false ? domainObject.TypeCode : TypeSectionEnum.S.ToString(),
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
		public static void ApplyChanges(this App_Data.Section entity, Section domainObject)
		{
			if (domainObject == null) return;
			entity.Name = domainObject.Name;
			entity.Description = domainObject.Description;
			entity.SectionCode = domainObject.SectionCode;
			entity.ParentSectionCode = domainObject.ParentSectionCode;
			entity.Tag = domainObject.Tag;
			entity.TypeCode = string.IsNullOrWhiteSpace(domainObject.TypeCode) == false ? domainObject.TypeCode : TypeSectionEnum.S.ToString();
		}
    }
}
