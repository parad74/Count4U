using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class LocationMapper
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
        public static Location ToDomainObject(this App_Data.Location entity)
        {
			if (entity == null) return null;
            return new Location()
            {
                ID = entity.ID,
                BackgroundColor = entity.BackgroundColor,
                Code = entity.Code,
                Description = entity.Description,
                Name = entity.Name,
				Restore = entity.Restore,
				RestoreBit = entity.RestoreBit != null ? entity.RestoreBit : false ,

				ParentLocationCode = entity.ParentLocationCode,
				TypeCode = entity.TypeCode,
				Level1 = entity.Level1,
				Level2 = entity.Level2,
				Level3 = entity.Level3,
				Level4 = entity.Level4,
				Name1 = entity.Name1,
				Name2 = entity.Name2,
				Name3 = entity.Name3,
				Name4 = entity.Name4,
				NodeType = Convert.ToInt32(entity.NodeType),
				LevelNum = Convert.ToInt32(entity.LevelNum),
				Total = Convert.ToInt32(entity.Total),
				Tag = entity.Tag,
				//InvStatus = Convert.ToInt32(entity.InvStatus),
				InvStatus = entity.InvStatus != null ? Convert.ToInt32(entity.InvStatus) : 0,
				Disabled = entity.Disabled != null ? entity.Disabled : false,
				DateModified = Convert.ToDateTime(entity.DateModified),
			
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
        public static Location ToSimpleDomainObject(this App_Data.Location entity)
        {
            return new Location()
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
				InvStatus = entity.InvStatus != null ? Convert.ToInt32(entity.InvStatus) : 0,
				RestoreBit = entity.RestoreBit != null ? entity.RestoreBit : false,
				Disabled = entity.Disabled != null ? entity.Disabled : false,
				DateModified = Convert.ToDateTime(entity.DateModified)
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
        public static App_Data.Location ToEntity(this Location domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.Location()
            {
                ID = domainObject.ID,
                BackgroundColor = domainObject.BackgroundColor,
                Code = domainObject.Code,
                Description = domainObject.Description,
                Name = domainObject.Name,
				Restore = domainObject.Restore,
				RestoreBit = domainObject.RestoreBit != null ? domainObject.RestoreBit : false,
				ParentLocationCode = domainObject.ParentLocationCode,
				TypeCode = domainObject.TypeCode,
				Level1 = domainObject.Level1,
				Level2 = domainObject.Level2,
				Level3 = domainObject.Level3,
				Level4 = domainObject.Level4,
				Name1 = domainObject.Name1,
				Name2 = domainObject.Name2,
				Name3 = domainObject.Name3,
				Name4 = domainObject.Name4,
				NodeType = domainObject.NodeType,
				LevelNum = domainObject.LevelNum,
				Total = domainObject.Total,
				Tag = domainObject.Tag ,
				Disabled = domainObject.Disabled != null ? domainObject.Disabled : false,
				DateModified = domainObject.DateModified,
				InvStatus = domainObject.InvStatus != null ? Convert.ToInt32(domainObject.InvStatus) : 0,

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
		public static void ApplyChanges(this App_Data.Location entity, Location domainObject)
		{
			if (domainObject == null) return;
			entity.BackgroundColor = domainObject.BackgroundColor;
			entity.Description = domainObject.Description;
			entity.Name = domainObject.Name;
			entity.Code = domainObject.Code;
			entity.Restore = domainObject.Restore;
			entity.RestoreBit = domainObject.RestoreBit != null ? domainObject.RestoreBit : false;
			entity.ParentLocationCode = domainObject.ParentLocationCode;
			entity.TypeCode = domainObject.TypeCode;
			entity.Level1 = domainObject.Level1;
			entity.Level2 = domainObject.Level2;
			entity.Level3 = domainObject.Level3;
			entity.Level4 = domainObject.Level4;
			entity.Name1 = domainObject.Name1;
			entity.Name2 = domainObject.Name2;
			entity.Name3 = domainObject.Name3;
			entity.Name4 = domainObject.Name4;
			entity.NodeType = domainObject.NodeType;
			entity.LevelNum = domainObject.LevelNum;
			entity.Total = domainObject.Total;
			entity.Tag = domainObject.Tag;
			//entity.InvStatus = Convert.ToInt32(domainObject.InvStatus);
			entity.InvStatus = domainObject.InvStatus != null ? Convert.ToInt32(domainObject.InvStatus) : 0;
			entity.Disabled = domainObject.Disabled != null ? domainObject.Disabled : false;
			entity.DateModified = domainObject.DateModified;

  		}

		//public static Location ToDomainObject(this LocationMobile entity)
		//{
		//	if (entity == null) return null;
		//	return new Location()
		//	{
		//		Code = entity.Code,
		//		Description = entity.Description,
		//		Name = entity.Name,
		//		Restore = entity.Restore,
		//		RestoreBit = entity.RestoreBit != null ? entity.RestoreBit : false,
		//		ParentLocationCode = entity.ParentLocationCode,
		//		TypeCode = entity.TypeCode,
		//		Level1 = entity.Level1,
		//		Level2 = entity.Level2,
		//		Level3 = entity.Level3,
		//		Level4 = entity.Level4,
		//		Name1 = entity.Name1,
		//		Name2 = entity.Name2,
		//		Name3 = entity.Name3,
		//		Name4 = entity.Name4,
		//		NodeType = Convert.ToInt32(entity.NodeType),
		//		LevelNum = Convert.ToInt32(entity.LevelNum),
		//		Total = Convert.ToInt32(entity.Total),
		//		DateModified = Convert.ToDateTime(entity.DateModified),
		//		Tag = entity.Tag
		//	};
		//}

		//public static int Validate(this Location entity, LocationString inValue, DateTimeFormatInfo dtfi)
		//{
		//    int invalidValueBit = 0;
		//    int fkCodeIsEmpty = 0;
		//    int codeIsEmpty = 0;

		//    int ret = 0;
		//    if (inValue == null)
		//    {
		//        ret = ret + (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
		//        return ret;
		//    }

		//    string code = Convert.ToString(inValue.Code).Trim(" ".ToCharArray());
		//    if (string.IsNullOrWhiteSpace(code) == true)
		//    {
		//        ret = ret + (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
		//        return ret;
		//    }

		//    entity.Code = code;
		//    try
		//    {
		//        entity.Name = inValue.Name.Trim(" ".ToCharArray());
		//        entity.Description = inValue.Description.Trim(" ".ToCharArray());
		//        string bg = inValue.BackgroundColor.Trim(" ".ToCharArray());
		//        string[] backgroundColor = bg.Split(':');
		//        int bg1 = Int32.Parse(backgroundColor[0].Trim());
		//        int bg2 = Int32.Parse(backgroundColor[1].Trim());
		//        int bg3 = Int32.Parse(backgroundColor[2].Trim());
		//        bg = bg1 + ", " + bg2 + ", " + bg3;
		//        entity.BackgroundColor = bg;
		//    }
		//    catch
		//    {
		//        invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
		//    }

		//    ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
		//    return ret;
		//}
    }
}
