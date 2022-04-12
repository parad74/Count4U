using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class FieldLinkMapper
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
		public static FieldLink ToDomainObject(this App_Data.FieldLink entity)
		{
			if (entity == null) return null;
			return new FieldLink()
			{
				ID = entity.ID,
				ViewName = entity.ViewName,
				EditorTemplate = entity.EditorTemplate,
				DomainType = entity.DomainType,
				TableName = entity.TableName,
				PropertyNameInDomainType = entity.PropertyNameInDomainType,
				FieldNameInTable = entity.FieldNameInTable,
				NumStringInRecord = Convert.ToInt32(entity.NumStringInRecord),
				Editor = entity.Editor,
				Validator = entity.Validator,
				CodeLocalizationEditorLable = entity.CodeLocalizationEditorLable,
				DefaultEditorLable = entity.DefaultEditorLable,
				NN = Convert.ToInt32(entity.NN),
				InGrid = Convert.ToBoolean(entity.InGrid),
				InEdit = Convert.ToBoolean(entity.InEdit),
				InAdd = Convert.ToBoolean(entity.InAdd)
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
		public static FieldLink ToSimpleDomainObject(this App_Data.FieldLink entity)
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
		public static App_Data.FieldLink ToEntity(this FieldLink domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.FieldLink()
			{
				ID = domainObject.ID,
				ViewName = domainObject.ViewName,
				EditorTemplate = domainObject.EditorTemplate,
				DomainType = domainObject.DomainType,
				TableName = domainObject.TableName,
				PropertyNameInDomainType = domainObject.PropertyNameInDomainType,
				FieldNameInTable = domainObject.FieldNameInTable,
				NumStringInRecord = Convert.ToInt32(domainObject.NumStringInRecord),
				Editor = domainObject.Editor,
				Validator = domainObject.Validator,
				CodeLocalizationEditorLable = domainObject.CodeLocalizationEditorLable,
				DefaultEditorLable = domainObject.DefaultEditorLable,
				NN = Convert.ToInt32(domainObject.NN),
				InGrid = Convert.ToBoolean(domainObject.InGrid),
				InEdit = Convert.ToBoolean(domainObject.InEdit),
				InAdd = Convert.ToBoolean(domainObject.InAdd)
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
		public static void ApplyChanges(this App_Data.FieldLink entity, FieldLink domainObject)
		{
			if (domainObject == null) return;
			entity.ViewName = domainObject.ViewName;
			entity.EditorTemplate = domainObject.EditorTemplate;
			entity.DomainType = domainObject.DomainType;
			entity.TableName = domainObject.TableName;
			entity.PropertyNameInDomainType = domainObject.PropertyNameInDomainType;
			entity.FieldNameInTable = domainObject.FieldNameInTable;
			entity.NumStringInRecord = Convert.ToInt32(domainObject.NumStringInRecord);
			entity.Editor = domainObject.Editor;
			entity.Validator = domainObject.Validator;
			entity.CodeLocalizationEditorLable = domainObject.CodeLocalizationEditorLable;
			entity.DefaultEditorLable = domainObject.DefaultEditorLable;
			entity.NN = Convert.ToInt32(domainObject.NN);
			entity.InGrid = Convert.ToBoolean(domainObject.InGrid);
			entity.InEdit = Convert.ToBoolean(domainObject.InEdit);
			entity.InAdd = Convert.ToBoolean(domainObject.InAdd);
		}

	}
}
