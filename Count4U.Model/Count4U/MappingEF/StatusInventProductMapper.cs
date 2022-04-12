using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class StatusInventProductMapper
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
		public static StatusInventProduct ToDomainObject(this App_Data.StatusInventProduct entity)
		{
			if (entity == null) return null;
			return new StatusInventProduct()
			{
				ID = entity.ID,
				Code = entity.Code,
				Name = entity.Name,
				Description = entity.Description,
				Bit = entity.Bit != null ? Convert.ToInt32(entity.Bit) : 0,
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
		public static StatusInventProduct ToSimpleDomainObject(this App_Data.StatusInventProduct entity)
		{
			return new StatusInventProduct()
			{
				ID = entity.ID,
				Code = entity.Code,
				Name = entity.Name,
				Description = entity.Description,
				Bit = entity.Bit != null ? Convert.ToInt32(entity.Bit) : 0,
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
		public static App_Data.StatusInventProduct ToEntity(this StatusInventProduct domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.StatusInventProduct()
			{
				ID = domainObject.ID,
				Code = domainObject.Code,
				Name = domainObject.Name,
				Description = domainObject.Description,
				Bit = domainObject.Bit
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
		public static void ApplyChanges(this App_Data.StatusInventProduct entity, StatusInventProduct domainObject)
		{
			if (domainObject == null) return;
			entity.Name = domainObject.Name;
			entity.Code = domainObject.Code;
			entity.Description = domainObject.Description;
			entity.Bit = domainObject.Bit;
		}

		//public static List<BitArray> GetBitList(int[] bits)
		//{
		//    //BitArray bit = new BitArray(new int[] {4});

		//    //BitArray[] bitarray = new BitArray[] {
		//    //    new BitArray(new int[] {4}), 
		//    //    new BitArray(new int[] {4})};

		//    //List<BitArray> ba = new List<BitArray>();
		//    //ba.Add(new BitArray(new int[] { 4 }));
		//    List<BitArray> bitList = new List<BitArray>();
		//    for (int i = 0; i < bits.Length; i++)
		//    {
		//        bitList.Add(new BitArray(new int[] { bits[i] }));
		//    }
		//    return bitList;
		//}

		//public static BitArray GetBitArrayAnd(int[] bits)
		//{
		//    BitArray bitArrayAdd = new BitArray(new int[]{0});
		//    for (int i = 0; i < bits.Length; i++)
		//    {
		//        BitArray bitArrayAdd1 = new BitArray(new int[] { bits[i] });
		//        bitArrayAdd = bitArrayAdd.And(bitArrayAdd1);
		//    }
		//    return bitArrayAdd;
		//}
	}
}
