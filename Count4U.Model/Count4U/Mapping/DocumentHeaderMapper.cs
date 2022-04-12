using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Mapping
{
	public static class DocumentHeaderMapper
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
		public static DocumentHeader ToDomainObject(this DocumentHeader entity)
		{
			if (entity == null) return null;
			return new DocumentHeader()
			{
				ID = entity.ID,
				Approve = entity.Approve,
				Code = entity.Code,
				DocumentCode = entity.DocumentCode,
				Itur = entity.Itur,
				Name = entity.Name,
				StatusDocHeader = entity.StatusDocHeader,
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
		public static DocumentHeader ToSimpleDomainObject(this DocumentHeader entity)
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
		public static DocumentHeader ToEntity(this DocumentHeader domainObject)
		{
			if (domainObject == null) return null;
			return new DocumentHeader()
			{
				ID = domainObject.ID,
				Approve = domainObject.Approve,
				Code = domainObject.Code,
				DocumentCode = domainObject.DocumentCode,
				Itur = domainObject.Itur,
				Name = domainObject.Name,
				StatusDocHeader = domainObject.StatusDocHeader,
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
		public static void ApplyChanges(this DocumentHeader entity, DocumentHeader domainObject)
		{
			if (domainObject == null) return;
			entity.Approve = domainObject.Approve;
			entity.Itur = domainObject.Itur;
			entity.Name = domainObject.Name;
			entity.StatusDocHeader = domainObject.StatusDocHeader;
		}
	}
}
