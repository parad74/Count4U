using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class UnitPlanValueMapper
	{

		public static UnitPlanValue ToDomainObject(this App_Data.UnitPlanValue entity)
		{
			if (entity == null) return null;
			return new UnitPlanValue()
			{
				ID = entity.ID,
				UnitPlanCode = entity.UnitPlanCode,
				TotalItur = Convert.ToInt32(entity.TotalItur),
				DoneItur = Convert.ToInt32(entity.DoneItur),
				Done = Convert.ToInt32(entity.Done),
				TotalItem = Convert.ToDouble(entity.TotalItem),
				SumQuantityEdit = Convert.ToDouble(entity.SumQuantityEdit),
				DiffQuantityEdit = Convert.ToDouble(entity.DiffQuantityEdit),
				StatusUnitPlanBit = Convert.ToInt32(entity.StatusUnitPlanBit),
				StatusGroupUnitPlanBit = Convert.ToInt32(entity.StatusGroupUnitPlanBit)

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
		public static UnitPlanValue ToSimpleDomainObject(this App_Data.UnitPlanValue entity)
		{
			return new UnitPlanValue()
			{
				ID = entity.ID,
				UnitPlanCode = entity.UnitPlanCode,
				TotalItur = Convert.ToInt32(entity.TotalItur),
				DoneItur = Convert.ToInt32(entity.DoneItur),
				Done = Convert.ToInt32(entity.Done),
				TotalItem = Convert.ToDouble(entity.TotalItem),
				SumQuantityEdit = Convert.ToDouble(entity.SumQuantityEdit),
				DiffQuantityEdit = Convert.ToDouble(entity.DiffQuantityEdit),
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
		public static App_Data.UnitPlanValue ToEntity(this UnitPlanValue domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.UnitPlanValue()
			{
				ID = domainObject.ID,
				UnitPlanCode = domainObject.UnitPlanCode,
				TotalItur = Convert.ToInt32(domainObject.TotalItur),
				DoneItur = Convert.ToInt32(domainObject.DoneItur),
				Done = Convert.ToInt32(domainObject.Done),
				TotalItem = Convert.ToDouble(domainObject.TotalItem),
				SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit),
				DiffQuantityEdit = Convert.ToDouble(domainObject.DiffQuantityEdit),
				StatusUnitPlanBit = Convert.ToInt32(domainObject.StatusUnitPlanBit),
				StatusGroupUnitPlanBit = Convert.ToInt32(domainObject.StatusGroupUnitPlanBit)

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
		public static void ApplyChanges(this App_Data.UnitPlanValue entity, UnitPlanValue domainObject)
		{
			if (domainObject == null) return;
			entity.UnitPlanCode = domainObject.UnitPlanCode;
			entity.TotalItur = Convert.ToInt32(domainObject.TotalItur);
			entity.DoneItur = Convert.ToInt32(domainObject.DoneItur);
			entity.Done = Convert.ToInt32(domainObject.Done);
			entity.TotalItem = Convert.ToDouble(domainObject.TotalItem);
			entity.SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit);
			entity.DiffQuantityEdit = Convert.ToDouble(domainObject.DiffQuantityEdit);
			entity.StatusUnitPlanBit = Convert.ToInt32(domainObject.StatusUnitPlanBit);
			entity.StatusGroupUnitPlanBit = Convert.ToInt32(domainObject.StatusGroupUnitPlanBit);

		}
	}
}
