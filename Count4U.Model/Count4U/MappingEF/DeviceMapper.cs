using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class DeviceMapper
    {
        /// <summary>
        /// Конвертация в объект предметной области.
 		public static Device ToDomainObject(this App_Data.Device entity)
		{
			if (entity == null) return null;
			string[] workers = entity.Name.Split('|');
			string	_workerID = "";			  //Name[0]
			string	_workerName = "";			//Name[1]
			if (workers.Length > 0)
			{
				_workerID = workers[0];
			}
			if (workers.Length > 1)
			{
				_workerName = workers[1];
			}

			return new Device()
			{
				ID = entity.ID,
				DeviceCode = entity.DeviceCode,
				Description = entity.Description,
				Name = entity.Name,
				DateCreated = Convert.ToDateTime(entity.DateCreated),
				LicenseDate = Convert.ToDateTime(entity.LicenseDate),

				WorkerID = _workerID,				  //Name[0]
				WorkerName = _workerName,			//Name[1]
				QuantityEdit = 0,
				Total = 0,
				//QuantityPerHourFromStartInventorToTheLast = 0,
				QuantityPerHourFromFirstToLast = 0,
				QuantityPerHourFromFirstToLastString = "",
				QuantityPerHourFromStartInventorToEndInventor = 0,
				QuantityPerHourFromStartInventorToEndInventorString = "",
				QuantityPerHourTotal = 0,
				TotalPerHourFromFirstToLast = 0,
				TotalPerHourFromFirstToLastString = "",
				TotalPerHourFromStartInventorToEndInventor = 0,
				TotalPerHourFromStartInventorToEndInventorString = "",
				TotalPerHourTotal = 0,
				TheFirst = Convert.ToDateTime(entity.LicenseDate),			//LicenseDate
				TheLast = Convert.ToDateTime(entity.DateCreated),			  //DateCreated
				TicksTimeSpan = 0,
				PeriodFromFirstToLast = "",
			//	PeriodFromStartInventorToTheFirst = "",
				SumPeriod = "",
				PeriodAddtionTime = entity.Description
			};
		}

 
		public static Device ToSimpleDomainObject(this App_Data.Device entity)
        {
			return new Device()
			{
				ID = entity.ID,
				DeviceCode = entity.DeviceCode,
				Description = entity.Description,
				Name = entity.Name,
				DateCreated = Convert.ToDateTime(entity.DateCreated),
				LicenseDate = Convert.ToDateTime(entity.LicenseDate)
            };
        }

 
		public static App_Data.Device ToEntity(this Device domainObject)
        {
			if (domainObject == null) return null;
			return new App_Data.Device()
            {
                ID = domainObject.ID,
				DeviceCode = domainObject.DeviceCode,
				Description = domainObject.PeriodAddtionTime,
				Name = domainObject.WorkerID + "|" + domainObject.WorkerName,
				DateCreated = Convert.ToDateTime(domainObject.TheLast),
				LicenseDate = Convert.ToDateTime(domainObject.TheFirst)

            };
        }

    
		public static void ApplyChanges(this App_Data.Device entity, Device domainObject)
		{
			if (domainObject == null) return;
			entity.DeviceCode = domainObject.DeviceCode;
			entity.Name = domainObject.WorkerID + "|" + domainObject.WorkerName;
			entity.Description = domainObject.PeriodAddtionTime;
			entity.DateCreated = Convert.ToDateTime(domainObject.TheLast);
			entity.LicenseDate = Convert.ToDateTime(domainObject.TheFirst);
		}


	
    }
}
