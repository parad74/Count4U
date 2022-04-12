using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace Count4U.Model.Count4U.MappingEF
{
	public static class ProductSimpleMapper
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
		//public static ProductSimple ToDomainObject(this App_Data.Product entity)
		//{
		//    if (entity == null) return null;
		//    return new ProductSimple()
		//    {
		//        Makat = entity.Makat,
		//        Name = entity.Name,
		//        //PriceSale = entity.PriceSale != null ? Convert.ToDouble(entity.PriceSale) : 0.0,
		//        PriceSale = Convert.ToDouble(entity.PriceSale),
		//        PriceString = entity.PriceString,
		//        SupplierCode = entity.SupplierCode,
		//        TypeCode = entity.TypeCode,
		//        ParentMakat = entity.ParentMakat,
		//    };
		//}

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
		//public static ProductSimple ToSimpleDomainObject(this App_Data.Product entity)
		//{
		//    throw new NotImplementedException();
		//}

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
		//public static App_Data.Product ToEntity(this ProductSimple domainObject)
		//{
		//    if (domainObject == null) return null;
		//    return new App_Data.Product()
		//    {
		//        Makat = domainObject.Makat,
		//        Name = domainObject.Name,
		//        //PriceSale = entity.PriceSale != null ? Convert.ToDouble(entity.PriceSale) : 0.0,
		//        PriceSale = domainObject.PriceSale,
		//        PriceString = domainObject.PriceString,
		//        SupplierCode = domainObject.SupplierCode,
		//        TypeCode = domainObject.TypeCode,
		//        ParentMakat = domainObject.ParentMakat,
		//    };
		//}

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
		//public static void ApplyChanges(this App_Data.Product entity, ProductSimple domainObject)
		//{
		//    if (domainObject == null) return;

		//    entity.Makat = domainObject.Makat;
		//    entity.Name = domainObject.Name;
		//    //entity.PriceSale = domainObject.PriceSale != null ? Convert.ToDouble(entity.PriceSale) : 0.0,
		//    entity.PriceSale = domainObject.PriceSale;
		//    entity.PriceString = domainObject.PriceString;
		//    entity.SupplierCode = domainObject.SupplierCode;
		//    entity.TypeCode = domainObject.TypeCode;
		//    entity.ParentMakat = domainObject.ParentMakat;
		//}

		
	}
}
