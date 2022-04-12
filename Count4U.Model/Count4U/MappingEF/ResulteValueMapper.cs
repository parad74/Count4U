using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class ResulteValueMapper
	{
		public static ResulteValue ToDomainObject(this App_Data.ResulteValue entity)
		{
			if (entity == null) return null;
			return new ResulteValue()
			{
				ID = entity.ID,
				Code = entity.Code,
				Name = entity.Name,
				ValueTypeCode = entity.ValueTypeCode,
				ColGroupCode = entity.ColGroupCode,
				RowGroupCode = entity.RowGroupCode,
				ColIndex = Convert.ToInt32(entity.ColIndex),
				RowIndex = Convert.ToInt32(entity.RowIndex),
				ColCode = entity.ColCode,
				RowCode = entity.RowCode,
				Value = entity.Value,
				ValueInt = Convert.ToInt32(entity.ValueInt),
				ValueStr = entity.ValueStr,
				ValueFloat = Convert.ToDouble(entity.ValueFloat),
				ValueBit = Convert.ToBoolean(entity.ValueBit)
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
		public static ResulteValue ToSimpleDomainObject(this App_Data.ResulteValue entity)
		{
			return new ResulteValue()
			{
				ID = entity.ID,
				Code = entity.Code,
				Value = entity.Value,
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
		public static App_Data.ResulteValue ToEntity(this ResulteValue domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.ResulteValue()
			{
				ID = domainObject.ID,
				Code = domainObject.Code,
				Name = domainObject.Name,
				ValueTypeCode = domainObject.ValueTypeCode,
				ColGroupCode = domainObject.ColGroupCode,
				RowGroupCode = domainObject.RowGroupCode,
				ColIndex = Convert.ToInt32(domainObject.ColIndex),
				RowIndex = Convert.ToInt32(domainObject.RowIndex),
				ColCode = domainObject.ColCode,
				RowCode = domainObject.RowCode,
				Value = domainObject.Value,
				ValueInt = Convert.ToInt32(domainObject.ValueInt),
				ValueStr = domainObject.ValueStr,
				ValueFloat = Convert.ToDouble(domainObject.ValueFloat),
				ValueBit = Convert.ToBoolean(domainObject.ValueBit)
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
		public static void ApplyChanges(this App_Data.ResulteValue entity, ResulteValue domainObject)
		{
			if (domainObject == null) return;
			entity.Name = domainObject.Name;
			entity.Code = domainObject.Code;
			entity.ValueTypeCode = domainObject.ValueTypeCode;
			entity.ColGroupCode = domainObject.ColGroupCode;
			entity.RowGroupCode = domainObject.RowGroupCode;
			entity.ColIndex = Convert.ToInt32(domainObject.ColIndex);
			entity.RowIndex = Convert.ToInt32(domainObject.RowIndex);
			entity.ColCode = domainObject.ColCode;
			entity.RowCode = domainObject.RowCode;
			entity.Value = domainObject.Value;
			entity.ValueInt = Convert.ToInt32(domainObject.ValueInt);
			entity.ValueStr = domainObject.ValueStr;
			entity.ValueFloat = Convert.ToDouble(domainObject.ValueFloat);
			entity.ValueBit = Convert.ToBoolean(domainObject.ValueBit);

		}
	}
}
