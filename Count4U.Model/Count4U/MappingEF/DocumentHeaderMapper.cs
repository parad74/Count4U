using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U.MappingEF
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
		public static DocumentHeader ToDomainObject(this App_Data.DocumentHeader entity)
		{
			if (entity == null) return null;
			string name = entity.Name.CutLength(49) != null ? entity.Name : "";
			string worker = entity.WorkerGUID.CutLength(49) != null ? entity.WorkerGUID : "";
			string nameworker = name + "|" + worker;
			return new DocumentHeader()
			{
				ID = entity.ID,
				Approve = entity.Approve,
				Code = entity.Code,
				DocumentCode = entity.DocumentCode,
				Name = name,
				SessionCode = entity.SessionCode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				IturCode = entity.IturCode,
				ModifyDate = entity.ModifyDate,
				StatusDocHeaderBit = entity.StatusDocHeaderBit,
				StatusInventProductBit = entity.StatusInventProductBit,
				StatusApproveBit = entity.StatusApproveBit,
				StatusDocHeaderCode = nameworker.CutLength(49),
				WorkerGUID = worker,
				DocNum = Convert.ToInt32(entity.ID),
				QuantityEdit = entity.QuantityEdit != null ? Convert.ToDouble(entity.QuantityEdit) : 0,
				Total = entity.Total != null ? Convert.ToInt64(entity.Total) : 0,
				FromTime = Convert.ToDateTime(entity.FromTime),
				ToTime = Convert.ToDateTime(entity.ToTime),
				TicksTimeSpan = entity.TicksTimeSpan != null ? Convert.ToInt64(entity.TicksTimeSpan) : 0,
				PeriodFromTo = entity.PeriodFromTo != null ? entity.PeriodFromTo : "",
				Restore  = entity.Restore,
				RestoreBit = Convert.ToBoolean(entity.RestoreBit)
				//Convert.ToInt32(entity.Num)
				//Num = 1
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
        public static DocumentHeader ToSimpleDomainObject(this App_Data.DocumentHeader entity)
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
        public static App_Data.DocumentHeader ToEntity(this DocumentHeader domainObject)
        {
			if (domainObject == null) return null;
			string name = domainObject.Name.CutLength(49) != null ? domainObject.Name : "";
			string worker = domainObject.WorkerGUID.CutLength(49) != null ? domainObject.WorkerGUID : "";
			string nameworker = name + "|" + worker;

			return new App_Data.DocumentHeader()
			{
				ID = domainObject.ID,
				Approve = domainObject.Approve,
				Code = domainObject.Code,
				DocumentCode = domainObject.DocumentCode,
				Name = name,
				CreateDate = domainObject.CreateDate,
				IturCode = domainObject.IturCode,
				ModifyDate = domainObject.ModifyDate,
				StatusDocHeaderBit = domainObject.StatusDocHeaderBit,
				StatusInventProductBit = domainObject.StatusInventProductBit,
				StatusApproveBit = domainObject.StatusApproveBit,
				StatusDocHeaderCode = nameworker.CutLength(49),
				WorkerGUID = worker,
				SessionCode = domainObject.SessionCode,
				DocNum = 1,
				QuantityEdit = Convert.ToDouble(domainObject.QuantityEdit),
				Total = Convert.ToInt64(domainObject.Total),
				FromTime = Convert.ToDateTime(domainObject.FromTime),
				ToTime = Convert.ToDateTime(domainObject.ToTime),
				TicksTimeSpan = Convert.ToInt64(domainObject.TicksTimeSpan),
				PeriodFromTo = domainObject.PeriodFromTo,
				Restore = domainObject.Restore,
				RestoreBit = domainObject.RestoreBit
				//Num = 1

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
		public static void ApplyChanges(this App_Data.DocumentHeader entity, DocumentHeader domainObject)
		{
			if (domainObject == null) return;
			string name = domainObject.Name.CutLength(49) != null ? domainObject.Name : "";
			string worker = domainObject.WorkerGUID.CutLength(49) != null ? domainObject.WorkerGUID : "";
			string nameworker = name + "|" + worker;

			entity.Approve = domainObject.Approve;
			entity.Name = name;
			entity.SessionCode = domainObject.SessionCode;
			entity.CreateDate = domainObject.CreateDate;
			entity.IturCode = domainObject.IturCode;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.StatusDocHeaderBit = domainObject.StatusDocHeaderBit;
			entity.StatusInventProductBit = domainObject.StatusInventProductBit;
			entity.StatusApproveBit = domainObject.StatusApproveBit;
			entity.StatusDocHeaderCode = nameworker.CutLength(49); ;
			entity.WorkerGUID = worker;
			entity.Code = domainObject.Code;
			entity.DocumentCode = domainObject.DocumentCode;
			entity.QuantityEdit = Convert.ToDouble(domainObject.QuantityEdit);
			entity.Total = Convert.ToInt64(domainObject.Total);
			entity.FromTime = Convert.ToDateTime(domainObject.FromTime);
			entity.ToTime = Convert.ToDateTime(domainObject.ToTime);
			entity.TicksTimeSpan = Convert.ToInt64(domainObject.TicksTimeSpan);
			entity.PeriodFromTo = domainObject.PeriodFromTo;
				entity.Restore = domainObject.Restore;
				entity.RestoreBit = domainObject.RestoreBit;
			//entity.Num = 1 ;
		}
    }
}
