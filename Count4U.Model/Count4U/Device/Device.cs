using System;
using System.Globalization;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    [Serializable]
    public class Device
	{
        public long ID { get; set; }
		public string DeviceCode { get; set; }		//PDANum
		public string Name { get; set; }					 //WorkerID| WorkerName
		public string Description { get; set; }		 // PeriodFromToEdit
		public DateTime DateCreated { get; set; }	 //ToTime
		public DateTime LicenseDate { get; set; }   //FromTime

		[PropertyNotBulk]
		public string ReportTotal { get; set; }

		[PropertyNotBulk]
		public string WorkerID { get; set; }
		[PropertyNotBulk]
		public string WorkerName { get; set; }

		[PropertyNotBulk]
		public string DeviceWorkerKey { get; set; }            // //x.DeviceCode + "|" + x.WorkerGUID

		[PropertyNotBulk]
		public string IturCode { get; set; }      //IturCode from DocumentHeader

		[PropertyNotBulk]
		public string ERPIturCode { get; set; }      //IturCode from DocumentHeader

		[PropertyNotBulk]
		public double QuantityEdit { get; set; }
		[PropertyNotBulk]
		public long Total { get; set; }

		[PropertyNotBulk]
		public string QuantityEditString { get; set; }


		[PropertyNotBulk]
		public string TotalString { get; set; }

		[PropertyNotBulk]
		public DateTime TheFirst { get; set; }
		[PropertyNotBulk]
		public DateTime TheLast { get; set; }
		[PropertyNotBulk]
		public long TicksTimeSpan { get; set; }
		[PropertyNotBulk]
		public DateTime StartInventorDateTime { get; set; }
		[PropertyNotBulk]
		public DateTime EndInventorDateTime { get; set; }
	
		//[PropertyNotBulk]
		//public string PeriodFromStartInventorToTheFirst { get; set; }
		//[PropertyNotBulk]
		//public string PeriodFromStartInventorToTheLast { get; set; }
		[PropertyNotBulk]
		public string PeriodFromStartInventorToEndInventor { get; set; }
		[PropertyNotBulk]
		public string PeriodFromFirstToLast { get; set; }
		[PropertyNotBulk]
		public string PeriodAddtionTime { get; set; }
		[PropertyNotBulk]
		public string SumPeriod { get; set; }

		//[PropertyNotBulk]
		//public int QuantityPerHourFromStartInventorToTheLast { get; set; }
		[PropertyNotBulk]
		public int QuantityPerHourFromStartInventorToEndInventor { get; set; }

		[PropertyNotBulk]
		public string QuantityPerHourFromStartInventorToEndInventorString { get; set; }

		[PropertyNotBulk]
		public int QuantityPerHourFromFirstToLast { get; set; }

		[PropertyNotBulk]
		public string QuantityPerHourFromFirstToLastString { get; set; }

		[PropertyNotBulk]
		public int QuantityPerHourTotal { get; set; }

		[PropertyNotBulk]
		public int TotalPerHourFromStartInventorToEndInventor { get; set; }		   //Todo

		[PropertyNotBulk]
		public string TotalPerHourFromStartInventorToEndInventorString { get; set; }

		[PropertyNotBulk]
		public int TotalPerHourFromFirstToLast { get; set; }

		[PropertyNotBulk]
		public string TotalPerHourFromFirstToLastString { get; set; }

		[PropertyNotBulk]
		public int TotalPerHourTotal { get; set; }

		public Device()
		{
			this.DeviceCode = DomainUnknownCode.UnknownDevice;
			this.Name = DomainUnknownName.UnknownWorker;
			this.Description = "";
			this.DateCreated = DateTime.Now;
			this.LicenseDate = DateTime.Now;

			// don't save to DB
			this.ReportTotal = "XXX";
			this.DeviceWorkerKey = "";            //x.DeviceCode + "|" + x.WorkerGUID
			this.WorkerID = "";				  //Name[0]
			this.WorkerName = "";           //Name[1]
			this.IturCode = "";
			this.ERPIturCode = "";
			this.QuantityEdit = 0;
			this.Total = 0;
			this.QuantityEditString = "";
			this.TotalString = "";
			this.QuantityPerHourFromFirstToLast = 0;
			this.QuantityPerHourFromFirstToLastString = "";
			this.QuantityPerHourFromStartInventorToEndInventor = 0;
			this.QuantityPerHourFromStartInventorToEndInventorString = "";
			this.QuantityPerHourTotal = 0;
			this.TotalPerHourFromFirstToLast = 0;
			this.TotalPerHourFromFirstToLastString = "";
			this.TotalPerHourFromStartInventorToEndInventor = 0;
			this.TotalPerHourFromStartInventorToEndInventorString = "";
			this.TotalPerHourTotal = 0;
			//this.QuantityPerHourFromStartInventorToTheLast = 0;
			this.TheFirst = DateTime.Now;			//LicenseDate
			this.StartInventorDateTime = DateTime.Now;
			this.EndInventorDateTime = DateTime.Now;
			this.TheLast = DateTime.Now;			  //DateCreated
			this.TicksTimeSpan = 0;
			this.PeriodFromFirstToLast = "";
			this.PeriodAddtionTime = "";			   //Description
			//this.PeriodFromStartInventorToTheFirst = "";
			//this.PeriodFromStartInventorToTheLast = "";
			this.PeriodFromStartInventorToEndInventor = "";
			this.SumPeriod = "";	
			
			

  		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Device)) return false;
			return Equals((Device)obj);
		}

		public bool Equals(Device other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.DeviceCode, this.DeviceCode);
		}

		public override int GetHashCode()
		{
			return (DeviceCode != null ? DeviceCode.GetHashCode() : 0);
		}

		public Device Clone()
		{
			return new Device()
			{
				DeviceCode	= this.DeviceCode,
				Description = this.Description,
				Name = this.Name,
				DateCreated = DateTime.Now,
				LicenseDate = this.LicenseDate
			};

			
		}
	}
}
