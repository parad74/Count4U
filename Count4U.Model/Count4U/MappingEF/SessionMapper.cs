using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
    public static class SessionMapper
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
        public static Session ToDomainObject(this App_Data.Session entity)
        {
			if (entity == null) return null;
			return new Session()
			{
				ID = entity.ID,
				SessionCode = entity.SessionCode,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				PDADate = entity.PDADate,
				PDAID = entity.PDAID,
				WorkerGUID = entity.WorkerGUID,
				CountItem = Convert.ToInt32(entity.CountItem),
				CountDocument = Convert.ToInt32(entity.CountDocument),
				CountItur = Convert.ToInt32(entity.CountItur)	,
				SumQuantityEdit = Convert.ToDouble(entity.SumQuantityEdit)
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
        public static Session ToSimpleDomainObject(this App_Data.Session entity)
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
        public static App_Data.Session ToEntity(this Session domainObject)
        {
			if (domainObject == null) return null;
            return new App_Data.Session()
            {
                ID = domainObject.ID,
				SessionCode = domainObject.SessionCode,
                CreateDate = domainObject.CreateDate,
                PDADate = domainObject.PDADate,
                PDAID = domainObject.PDAID,
                WorkerGUID = domainObject.WorkerGUID,
				CountItem = Convert.ToInt32(domainObject.CountItem),
				CountDocument = Convert.ToInt32(domainObject.CountDocument),
				CountItur = Convert.ToInt32(domainObject.CountItur),
				SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit)
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
        public static void ApplyChanges(this App_Data.Session entity, Session domainObject)
        {
			if (domainObject == null) return;
            entity.CreateDate = domainObject.CreateDate;
            entity.PDADate = domainObject.PDADate;
            entity.PDAID = domainObject.PDAID;
            entity.WorkerGUID = domainObject.WorkerGUID;
			entity.CountItem = domainObject.CountItem;
			entity.CountDocument = domainObject.CountDocument;
			entity.CountItur = domainObject.CountItur;
			entity.SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit);
        }

	
    }
}
