using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Main.MappingEF
{
	public static class CustomerMaskMapper
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
		public static Mask ToCustomerDomainObject(this App_Data.CustomerMask entity)
		{
			if (entity == null) return null;
			return new Mask()
			{
				ID = entity.ID,
				Code = entity.Code,
				AdapterCode = entity.AdapterCode,
				FileCode = entity.FileCode,
				BarcodeMask = entity.BarcodeMask,
				MakatMask = entity.MakadMask
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
		public static Mask ToSimpleCustomerDomainObject(this App_Data.CustomerMask entity)
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
		public static App_Data.CustomerMask ToCustomerEntity(this Mask domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.CustomerMask()
			{
				ID = domainObject.ID,
				Code = domainObject.Code,
				AdapterCode = domainObject.AdapterCode,
				FileCode = domainObject.FileCode,
				BarcodeMask = domainObject.BarcodeMask,
				MakadMask = domainObject.MakatMask
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
		public static void ApplyCustomerChanges(this App_Data.CustomerMask entity, Mask domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.Code = domainObject.Code;
			entity.AdapterCode = domainObject.AdapterCode;
			entity.FileCode = domainObject.FileCode;
			entity.BarcodeMask = domainObject.BarcodeMask;
			entity.MakadMask = domainObject.MakatMask;
		}
	}
}
