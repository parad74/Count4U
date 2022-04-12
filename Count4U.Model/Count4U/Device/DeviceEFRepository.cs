using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using System.Globalization;

namespace Count4U.Model.Count4U
{
    public class DeviceEFRepository : BaseEFRepository, IDeviceRepository
    {
		private Dictionary<string, Device> _deviceDictionary;
		private readonly IServiceLocator _serviceLocator;
		private CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
		public DeviceEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
            : base(connection)
        {
			this._deviceDictionary = new Dictionary<string, Device>();
			this._serviceLocator = serviceLocator;
        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion
		public void FillDevicesFromDocumrntHeaders(string pathDB)
		{
			//DeleteAll(pathDB);
			Devices ret = new Devices();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var codes = db.DocumentHeaders.Select(e => e.Name).Distinct().ToList();
					foreach (string deviceCode in codes)
					{
						var entity = this.GetEntityByCode(db, deviceCode);
						if (entity == null)
						{
							Device device = new Device();
							device.DeviceCode = deviceCode;
							var entity1 = device.ToEntity();
							db.Devices.AddObject(entity1);
						}
					}
					db.SaveChanges();
				}
				catch (Exception exp)
				{
					_logger.ErrorException("FillDevicesFromDocumrntHeaders", exp);
				}
			}
		}

		#region IDeviceRepository Members

		public Devices GetDevices(string pathDB, bool fefill = false)
        {
			if (fefill == true)
			{
				FillDevicesFromDocumrntHeaders(pathDB);
			}
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var domainObjects = db.Devices.ToList().Select(e => e.ToDomainObject());
				return Devices.FromEnumerable(domainObjects);
            }
        }

		public Devices GetDevices(int topCount, string pathDB, bool fefill = false)
		{
			if (fefill == true)
			{
				FillDevicesFromDocumrntHeaders(pathDB);
			}
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Devices.Take(topCount).ToList().Select(e => e.ToDomainObject());
				return Devices.FromEnumerable(domainObjects);
			}
		}

		public Devices GetDevices(SelectParams selectParams, string pathDB, bool fefill = false)
		{
			if (fefill == true)
			{
				FillDevicesFromDocumrntHeaders(pathDB);
			}
			if (selectParams == null)
				return GetDevices(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Devices), db.Devices.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Devices.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(Device device, string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, device.DeviceCode);
				if (entity == null) return;
				db.Devices.DeleteObject(entity);
                db.SaveChanges();
            }
        }

		public void DeleteAll(string pathDB)
        {
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				db.Devices.ToList().ForEach(e => db.Devices.DeleteObject(e));
                db.SaveChanges();
            }
        }

		public void Insert(Device device, string pathDB)
        {
			if (device == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = device.ToEntity();
				db.Devices.AddObject(entity);
                db.SaveChanges();
            }
        }

		public void Update(Device device, string pathDB)
        {
			if (device == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
				var entity = this.GetEntityByCode(db, device.DeviceCode);
				if (entity == null) return;
				entity.ApplyChanges(device);
                db.SaveChanges();
            }
        }

	
		public Device GetDeviceByName(string name, string pathDB, bool fefill = false)
		{
			if (fefill == true)
			{
				FillDevicesFromDocumrntHeaders(pathDB);
			}
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
            {
                var entity = this.GetEntityByName(db, name);
				if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

		public Device GetDeviceByCode(string code, string pathDB, bool fefill = false)
		{
			if (fefill == true)
			{
				FillDevicesFromDocumrntHeaders(pathDB);
			}
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		//===================
		public DateTime GetTheLastForDevice(/*SelectParams selectParams,*/ string pathDB)
		{
			IDeviceRepository deviceRepository = this._serviceLocator.GetInstance<IDeviceRepository>();
			Devices dev = deviceRepository.GetDevices(pathDB, true);
			// selectParams - TO DO
			Devices deviceSumList = new Devices();
			DateTime theLastForAllDevice = DateTime.Now;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> deviceCodeList = db.DocumentHeaders.Select(s => s.Name).Distinct().ToList();

				foreach (var deviceCode in deviceCodeList)
				{
					if (string.IsNullOrEmpty(deviceCode) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.Name == deviceCode).ToList().Select(e => e);

						var deviceSumByDeviceCode = from e in documentHeaders
													orderby e.Name
													group e by e.Name into g
													select new Device
													{
														DeviceCode = g.Key,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};

						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumByDeviceCode.Where(x => x.DeviceCode == deviceCode).FirstOrDefault();
						}
						catch { }
						deviceSumList.Add(deviceSum);
					}
				}
			}

		
			try
			{
				if (deviceSumList != null)
				{
					if (deviceSumList.Count > 0)
					{
						theLastForAllDevice = deviceSumList.Max(x => x.TheLast);
					}
				}
			}
			catch { }

			return theLastForAllDevice;
		}


		//======================================
		public Devices RefillDeviceStatisticByDeviceCode(DateTime startInventorDate, DateTime endInventorDate, SelectParams selectParams, string pathDB)
		{
			IDeviceRepository deviceRepository = this._serviceLocator.GetInstance<IDeviceRepository>();
			Devices dev = deviceRepository.GetDevices(selectParams, pathDB, true);
			// selectParams - TO DO
			Devices deviceSumList = new Devices();
			Devices returnDeviceSumList = new Devices();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> deviceCodeList = db.DocumentHeaders.Select(s => s.Name).Distinct().ToList();      //Name == Device

				foreach (var deviceCode in deviceCodeList)
				{
					if (string.IsNullOrEmpty(deviceCode) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.Name == deviceCode).ToList().Select(e => e);
		
						var deviceSumByDeviceCode = from e in documentHeaders
													orderby e.Name
													group e by e.Name into g
													select new Device
													{
														DeviceCode = g.Key,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};

						App_Data.Device entity = null;
						Device deviceSum = null;

						try
						{
							entity = db.Devices.Where(x => x.DeviceCode == deviceCode).FirstOrDefault();
							deviceSum = deviceSumByDeviceCode.Where(x => x.DeviceCode == deviceCode).FirstOrDefault();
						}
						catch { }


						if (deviceSumByDeviceCode != null)
						{
							deviceSumList.Add(deviceSum);
							Device device = new Device();
							if (entity != null)
							{
								device = entity.ToDomainObject();
							}
							else
							{
								entity = new App_Data.Device();
								db.Devices.AddObject(entity);
							}

							device.DeviceCode = deviceSum.DeviceCode;
							device.LicenseDate = deviceSum.TheFirst;
							device.DateCreated = deviceSum.TheLast;

							entity.ApplyChanges(device);
							db.SaveChanges();
						}
					}
				}


				foreach (var deviceCode in deviceCodeList)
				{
					if (string.IsNullOrEmpty(deviceCode) != true)
					{
						App_Data.Device entity = null;
						Device deviceSum = null;

						try
						{
							entity = db.Devices.Where(x => x.DeviceCode == deviceCode).FirstOrDefault();
							deviceSum = deviceSumList.Where(x => x.DeviceCode == deviceCode).FirstOrDefault();

						}
						catch { }

						if (entity != null)
						{
							var deviceEntity = entity.ToDomainObject();
							deviceSum.Name = deviceEntity.Name;
							deviceSum.WorkerName = deviceEntity.WorkerName;
							deviceSum.WorkerID = deviceEntity.WorkerID;

							DateTime theFirst = DateTime.Now;
							DateTime theLast = theFirst;
							long ticksTimeSpan = 0;
							string periodFromFirstToLast = "00:00:00";
							int periodFromFirstToLastDays = 0;
							int periodFromFirstToLastH = 0;
							int periodFromFirstToLastMin = 0;
							int periodFromFirstToLastSec = 0;
							long total = 0;
							double quantityEdit = 0;

							//if (deviceSum != null)
							//{
							try
							{
								theFirst = deviceSum.TheFirst;
								theLast = deviceSum.TheLast;
							}
							catch { }
							TimeSpan fromFirstToLast = (TimeSpan)(theLast - theFirst);

							try
							{
								ticksTimeSpan = fromFirstToLast.Ticks;
								//periodFromFirstToLast = fromFirstToLast.ToString(@"dd\:hh\:mm");               //\:ss
								periodFromFirstToLastDays = fromFirstToLast.Days;
								periodFromFirstToLastH = fromFirstToLast.Hours;
								periodFromFirstToLastMin = fromFirstToLast.Minutes;
								periodFromFirstToLastSec = fromFirstToLast.Seconds;
								periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
																		periodFromFirstToLastMin.ToString().PadLeft(2, '0');// + ":" +
																		//periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss

								total = deviceSum.Total;
								quantityEdit = deviceSum.QuantityEdit;
							}
							catch { }
							deviceSum.QuantityEdit = quantityEdit;
							deviceSum.Total = total;

							//long total10000 = total * 10000;
							//double quantityEdit10000 = total * 10000;
							deviceSum.TotalString = total.ToString("N0", culture);
							deviceSum.QuantityEditString = quantityEdit.ToString("N0", culture);

							//	}

							deviceSum.TheFirst = theFirst;
							deviceSum.TheLast = theLast;
							deviceSum.TicksTimeSpan = ticksTimeSpan;
							deviceSum.PeriodFromFirstToLast = periodFromFirstToLast;
							{
								//int minuts = (fromFirstToLast.Days * 24) * 60 + fromFirstToLast.Hours * 60 + fromFirstToLast.Minutes;
								//if (minuts == 0)
								//{
								//	minuts = 1;
								//	deviceSum.PeriodFromFirstToLast = "00:00:01";
								//}

								//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
								//double totalPerMinute = total / ((double)minuts / 60.0);

								//int edit1 = (int)(editPerMinute);
								//int total1 = (int)(totalPerMinute);

								int secs = (fromFirstToLast.Days * 24) * 3600 + fromFirstToLast.Hours * 3600 + fromFirstToLast.Minutes * 60 + +fromFirstToLast.Seconds;
								if (secs == 0)
								{
									secs = 1;
								}

								double editPerSec = quantityEdit / ((double)secs / 3600.0);
								double totalPerSec = total / ((double)secs / 3600.0);

								int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
								int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);


								deviceSum.QuantityPerHourFromFirstToLast = edit1;
								deviceSum.QuantityPerHourFromFirstToLastString = edit1.ToString("N0", culture);
								deviceSum.TotalPerHourFromFirstToLast = total1;
								deviceSum.TotalPerHourFromFirstToLastString = total1.ToString("N0", culture); 
							}
							deviceSum.StartInventorDateTime = startInventorDate;
							deviceSum.EndInventorDateTime = endInventorDate;

							//===============  FromStartInventorToTheFirst ===================
							//	TimeSpan fromInventorTo = (TimeSpan)(toTime - inventorDate);
							//TimeSpan fromStartInventorToTheFirst = (TimeSpan)(theFirst - startInventorDate);

							//long ticksFromStartInventorToTheFirstTimeSpan = 0;
							//string periodFromStartInventorToTheFirst = "00:00:00";
							//try
							//{
							//	ticksFromStartInventorToTheFirstTimeSpan = fromStartInventorToTheFirst.Ticks;
							//	periodFromStartInventorToTheFirst = fromStartInventorToTheFirst.ToString(@"dd\:hh\:mm");		
							//}
							//catch { }
							//deviceSum.PeriodFromStartInventorToTheFirst = periodFromStartInventorToTheFirst;

							//===============  FromStartInventorToTheLast ===================
							//TimeSpan fromInventorToTheLast = (TimeSpan)(theLast - startInventorDate);
							//long ticksInventorToTheLastTimeSpan = 0;
							//string periodFromInventorToTheLast = "00:00:00";
							//try
							//{
							//	ticksInventorToTheLastTimeSpan = fromInventorToTheLast.Ticks;
							//	periodFromInventorToTheLast = fromInventorToTheLast.ToString(@"dd\:hh\:mm");
							//}
							//catch { }
							//deviceSum.PeriodFromStartInventorToTheLast = periodFromInventorToTheLast;

							//{
							//	int minuts = (fromInventorToTheLast.Days * 24) * 60 + fromInventorToTheLast.Hours * 60 + fromInventorToTheLast.Minutes;
							//	if (minuts == 0) minuts = 1;
							//	double qEdit = quantityEdit / ((double)minuts / 60.0);
							//	//int qEdit1 = (int)(qEdit * 100);
							//	//deviceSum.QuantityPerHourFromStartInventorToTheLast = (double)(qEdit1) / 100.00;
							//	int edit1 = (int)(qEdit);
							//	deviceSum.QuantityPerHourFromStartInventorToTheLast = edit1;
							//}
							//===============  FromStartInventorToEndInventor ===================

							TimeSpan fromInventorToEndInventor = (TimeSpan)(endInventorDate - startInventorDate);
							long ticksInventorToEndInventorTimeSpan = 0;
							string periodFromInventorToEndInventor = "00:00";
							int periodFromInventorToEndDays = 0;
							int periodFromInventorToEndH = 0;
							int periodFromInventorToEndMin = 0;
							int periodFromInventorToEndSec = 0;
							try
							{
								ticksInventorToEndInventorTimeSpan = fromInventorToEndInventor.Ticks;
								//periodFromInventorToEndInventor = fromInventorToEndInventor.ToString(@"dd\:hh\:mm");
								periodFromInventorToEndDays = fromInventorToEndInventor.Days;
								periodFromInventorToEndH = fromInventorToEndInventor.Hours;
								periodFromInventorToEndMin = fromInventorToEndInventor.Minutes;
								periodFromInventorToEndSec = fromInventorToEndInventor.Seconds;
								periodFromInventorToEndInventor = (periodFromInventorToEndDays * 24 + periodFromInventorToEndH).ToString().PadLeft(2, '0') + ":" +
																		periodFromInventorToEndMin.ToString().PadLeft(2, '0');// + ":" +
																		//periodFromInventorToEndSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss

							}
							catch { }
							deviceSum.PeriodFromStartInventorToEndInventor = periodFromInventorToEndInventor;

							{
								//int minuts = (fromInventorToEndInventor.Days * 24) * 60 + fromInventorToEndInventor.Hours * 60 + fromInventorToEndInventor.Minutes;
								//if (minuts == 0)
								//{
								//	minuts = 1;
								//	deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
								//}
								//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
								//double totalPerMinute = total / ((double)minuts / 60.0);

								////int qEdit1 = (int)(qEdit * 100);
								////deviceSum.QuantityPerHourFromStartInventorToEndInventor = (double)(qEdit1) / 100.00;
								//int edit1 = (int)(editPerMinute);
								//int total1 = (int)(totalPerMinute);

								int secs = (fromInventorToEndInventor.Days * 24) * 3600 + fromInventorToEndInventor.Hours * 3600 + fromInventorToEndInventor.Minutes * 60 + fromInventorToEndInventor.Seconds; ;
								if (secs == 0)
								{
									secs = 1;
								}
								double editPerSec = quantityEdit / ((double)secs / 3600.0);
								double totalPerSec = total / ((double)secs / 3600.0);

								//int edit1 = (int)(editPerSec);
								//int total1 = (int)(totalPerSec);
								int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
								int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

								deviceSum.QuantityPerHourFromStartInventorToEndInventor = edit1;
								deviceSum.TotalPerHourFromStartInventorToEndInventor = total1;

								deviceSum.QuantityPerHourFromStartInventorToEndInventorString = edit1.ToString("N0", culture);
								deviceSum.TotalPerHourFromStartInventorToEndInventorString = total1.ToString("N0", culture);
							}


							// ============ from deviceEntity

							//string periodAddTime = "00:00:00";
							//TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
							//try
							//{

							//	bool ret = TimeSpan.TryParse(deviceEntity.Description, out addTime);
							//	if (ret == true)
							//	{
							//		ret = TimeSpan.TryParse(deviceEntity.Description + ":00", out addTime);
							//		if (ret == true)
							//		{
							//			periodAddTime = addTime.ToString(@"dd\:hh\:mm");               //\:ss
							//		}
							//	}
							//}
							//catch { }
							//deviceSum.PeriodAddtionTime = periodAddTime;

							//=========
						//	string sumTime = "00:00:00";

							//TimeSpan fromStartInventorToTheFirstTime = new TimeSpan(0, 0, 0, 0);
							//{
							//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst, out fromStartInventorToTheFirstTime);
							//	if (ret1 == true)
							//	{
							//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst + ":00", out fromStartInventorToTheFirstTime);
							//	}
							//}

							//TimeSpan fromStartInventorToEndInventorTime = new TimeSpan(0, 0, 0, 0);
							//{
							//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor, out fromStartInventorToEndInventorTime);
							//	if (ret1 == true)
							//	{
							//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor /*+ ":00"*/, out fromStartInventorToEndInventorTime);
							//	}
							//}

							//TimeSpan fromFirstToLastTime = new TimeSpan(0, 0, 0, 0);
							//{
							//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast, out fromFirstToLastTime);
							//	if (ret1 == true)
							//	{
							//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast /*+ ":00"*/, out fromFirstToLastTime);
							//	}
							//}

							//TimeSpan sum = new TimeSpan(0, 0, 0, 0);
							//{
							//	//sum = addTime + fromStartInventorToTheFirstTime + fromFirstToLastTime;
							//	//sum = addTime + fromStartInventorToEndInventorTime;
							//	sum = fromStartInventorToEndInventorTime;
							//	sumTime = deviceSum.PeriodFromStartInventorToEndInventor;

							//}

							//{
								//int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
								//if (minuts == 0)
								//{
								//	minuts = 1;
								//	deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
								//}

								//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
								//double totalPerMinute = total / ((double)minuts / 60.0);

								//int edit1 = (int)(editPerMinute);
								//int total1 = (int)(totalPerMinute);

							//	int secs = (sum.Days * 24) * 3600 + sum.Hours * 3600 + sum.Minutes * 60 + sum.Seconds; ;
							//	if (secs == 0)
							//	{
							//		secs = 1;
							//	}
							//	double editPerSec = quantityEdit / ((double)secs / 3600.0);
							//	double totalPerSec = total / ((double)secs / 3600.0);

							//	int edit1 = (int)(editPerSec);
							//	int total1 = (int)(totalPerSec);
							//	deviceSum.QuantityPerHourTotal = edit1;
							//	deviceSum.TotalPerHourTotal = edit1;
							//}

							deviceSum.SumPeriod = periodFromInventorToEndInventor;
							//SumPeriod

							//entity.ApplyChanges(device);
							returnDeviceSumList.Add(deviceSum);
						}
					}
				}
				//}//foreach
				//	db.SaveChanges();
			}       //db
			return returnDeviceSumList;
		}


		public Devices RefillDeviceStatisticByWorkerOld(DateTime startInventorDate, DateTime endInventorDate, /*SelectParams selectParams, */string pathDB)
		{
			Devices deviceSumList = new Devices();
			Devices returnDeviceSumList = new Devices();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> workerList = db.DocumentHeaders.Select(s => s.WorkerGUID).Distinct().ToList();      


				foreach (var worker in workerList)     //x.WorkerGUID
				{
					if (string.IsNullOrEmpty(worker) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.WorkerGUID == worker).ToList().Select(e => e);

						var deviceSumByWorkerGUID = from e in documentHeaders
													orderby e.WorkerGUID
													group e by new
													{
														e.WorkerGUID
													} into g
													select new Device
													{
														DeviceWorkerKey = g.Key.WorkerGUID,
														WorkerName = g.Key.WorkerGUID,
														WorkerID = g.Key.WorkerGUID,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};

						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumByWorkerGUID.Where(x => x.DeviceWorkerKey == worker).FirstOrDefault(); //deviceWorker = x.WorkerGUID
							if (deviceSum != null)
							{
								deviceSumList.Add(deviceSum);
							}
						}
						catch { }

					}
				}            //end foreach	   deviceWorkerList     //x.WorkerGUID


				foreach (var worker in workerList)        //x.WorkerGUID
				{
					if (string.IsNullOrEmpty(worker) != true)
					{
						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumList.Where(x => x.DeviceWorkerKey == worker).FirstOrDefault();
						}
						catch { }

				
						DateTime theFirst = DateTime.Now;
						DateTime theLast = theFirst;
						long ticksTimeSpan = 0;
						string periodFromFirstToLast = "00:00:00";
						int periodFromFirstToLastDays = 0;
						int periodFromFirstToLastH = 0;
						int periodFromFirstToLastMin = 0;
						int periodFromFirstToLastSec = 0;
						long total = 0;
						double quantityEdit = 0;

	
						try
						{
							theFirst = deviceSum.TheFirst;
							theLast = deviceSum.TheLast;
						}
						catch { }
						TimeSpan fromFirstToLast = (TimeSpan)(theLast - theFirst);

						try
						{
							ticksTimeSpan = fromFirstToLast.Ticks;
							periodFromFirstToLastDays = fromFirstToLast.Days;
							periodFromFirstToLastH = fromFirstToLast.Hours;
							periodFromFirstToLastMin = fromFirstToLast.Minutes;
							periodFromFirstToLastSec = fromFirstToLast.Seconds;
							periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastMin.ToString().PadLeft(2, '0');// + ":" + 
																	//periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss
							total = deviceSum.Total;
							quantityEdit = deviceSum.QuantityEdit;
						}
						catch { }
						deviceSum.QuantityEdit = quantityEdit;
						deviceSum.Total = total;

						//long total10000 = total * 10000;
						//double quantityEdit10000 = total * 10000;
						deviceSum.TotalString = total.ToString("N0", culture);
						deviceSum.QuantityEditString = quantityEdit.ToString("N0", culture);

						deviceSum.TheFirst = theFirst;
						deviceSum.TheLast = theLast;
						deviceSum.TicksTimeSpan = ticksTimeSpan;
						deviceSum.PeriodFromFirstToLast = periodFromFirstToLast;
						{
							//int minuts = (fromFirstToLast.Days * 24) * 60 + fromFirstToLast.Hours * 60 + fromFirstToLast.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromFirstToLast.Days * 24) * 3600 + fromFirstToLast.Hours * 3600 + fromFirstToLast.Minutes * 60 + +fromFirstToLast.Seconds;
							if (secs == 0)
							{
								secs = 1;
							}

							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromFirstToLast = edit1;
							deviceSum.QuantityPerHourFromFirstToLastString = edit1.ToString("N0", culture); ;
							deviceSum.TotalPerHourFromFirstToLast = total1;
							deviceSum.TotalPerHourFromFirstToLastString = total1.ToString("N0", culture); ;
						}
						deviceSum.StartInventorDateTime = startInventorDate;
						deviceSum.EndInventorDateTime = endInventorDate;

				
						TimeSpan fromInventorToEndInventor = (TimeSpan)(endInventorDate - startInventorDate);
						long ticksInventorToEndInventorTimeSpan = 0;
						string periodFromInventorToEndInventor = "00:00";
						int periodFromInventorToEndDays = 0;
						int periodFromInventorToEndH = 0;
						int periodFromInventorToEndMin = 0;
						int periodFromInventorToEndSec = 0;
						try
						{
							ticksInventorToEndInventorTimeSpan = fromInventorToEndInventor.Ticks;
							periodFromInventorToEndDays = fromInventorToEndInventor.Days;
							periodFromInventorToEndH = fromInventorToEndInventor.Hours;
							periodFromInventorToEndMin = fromInventorToEndInventor.Minutes;
							periodFromInventorToEndSec = fromInventorToEndInventor.Seconds;
							periodFromInventorToEndInventor = (periodFromInventorToEndDays * 24 + periodFromInventorToEndH).ToString().PadLeft(2, '0') + ":" +
																	periodFromInventorToEndMin.ToString().PadLeft(2, '0');// + ":" +
																	//periodFromInventorToEndSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss


							//periodFromInventorToEndInventor = fromInventorToEndInventor.ToString(@"dd\:hh\:mm");
						}
						catch { }
						deviceSum.PeriodFromStartInventorToEndInventor = periodFromInventorToEndInventor;

						{

							//int minuts = (fromInventorToEndInventor.Days * 24) * 60 + fromInventorToEndInventor.Hours * 60 + fromInventorToEndInventor.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//	//deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromInventorToEndInventor.Days * 24) * 3600 + fromInventorToEndInventor.Hours * 3600 + fromInventorToEndInventor.Minutes * 60 + fromInventorToEndInventor.Seconds; ;
							if (secs == 0)
							{
								secs = 1;
							}
							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromStartInventorToEndInventor = edit1;
							deviceSum.TotalPerHourFromStartInventorToEndInventor = total1;

							deviceSum.QuantityPerHourFromStartInventorToEndInventorString = edit1.ToString("N0", culture);
							deviceSum.TotalPerHourFromStartInventorToEndInventorString = total1.ToString("N0", culture);
						}


						// ============ from deviceEntity

						//string periodAddTime = "00:00:00";
						//убрала в новой версии
						//TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
						//try
						//{

						//	bool ret = TimeSpan.TryParse(deviceEntity.Description, out addTime);
						//	if (ret == true)
						//	{
						//		ret = TimeSpan.TryParse(deviceEntity.Description + ":00", out addTime);
						//		if (ret == true)
						//		{
						//			periodAddTime = addTime.ToString(@"dd\:hh\:mm");               //\:ss
						//		}
						//	}
						//}
						//catch { }
						//deviceSum.PeriodAddtionTime = periodAddTime;

						////=========
						//string sumTime = "00:00:00";

						//TimeSpan fromStartInventorToTheFirstTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst, out fromStartInventorToTheFirstTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst + ":00", out fromStartInventorToTheFirstTime);
						//	}
						//}

						//TimeSpan fromStartInventorToEndInventorTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor, out fromStartInventorToEndInventorTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor /*+ ":00"*/, out fromStartInventorToEndInventorTime);
						//	}
						//}

						//TimeSpan fromFirstToLastTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast, out fromFirstToLastTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast/* + ":00"*/, out fromFirstToLastTime);
						//	}
						//}

						//TimeSpan sum = new TimeSpan(0, 0, 0, 0);
						//{
							//sum = addTime + fromStartInventorToTheFirstTime + fromFirstToLastTime;
							//sum = addTime + fromStartInventorToEndInventorTime;
						//	sum = fromStartInventorToEndInventorTime;
						//	sumTime = deviceSum.PeriodFromStartInventorToEndInventor; //sum.ToString(@"dd\:hh\:mm");
						//}

						//{
							//int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}

							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

						//	int secs = (sum.Days * 24) * 3600 + sum.Hours * 3600 + sum.Minutes * 60 + sum.Seconds; ;
						//	if (secs == 0)
						//	{
						//		secs = 1;
						//	}
						//	double editPerSec = quantityEdit / ((double)secs / 3600.0);
						//	double totalPerSec = total / ((double)secs / 3600.0);

						//	int edit1 = (int)(editPerSec);
						//	int total1 = (int)(totalPerSec);
						//	deviceSum.QuantityPerHourTotal = edit1;
						//	deviceSum.TotalPerHourTotal = edit1;
						//}

						deviceSum.SumPeriod = periodFromInventorToEndInventor;
						//SumPeriod

						//entity.ApplyChanges(device);
						returnDeviceSumList.Add(deviceSum);
						//}	 //end						if (entity != null)
					}
				}
				//}//foreach
				//	db.SaveChanges();
			}       //db
			return returnDeviceSumList;
		}

		//[Rep-DW1-12sp]
		//[Rep-DW1-13]
		//[Rep-DW1-13p]
		//+ Rep-DW1-12	первая таблица
		public Devices RefillDeviceStatisticByWorker(DateTime startInventorDate, DateTime endInventorDate, /*SelectParams selectParams, */string pathDB)
		{

			Devices deviceWorkerSumList = this.RefillDeviceStatisticByDeviceAndWorker(startInventorDate, endInventorDate, pathDB);
		
			Devices deviceSumList = new Devices();
	
			Devices returnDeviceSumList = new Devices();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
								  					//=========================		workerList
				List<string> workerList = db.DocumentHeaders.Select(s => s.WorkerGUID).Distinct().ToList();

				foreach (var worker in workerList)     //x.WorkerGUID
				{
					if (string.IsNullOrEmpty(worker) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => x.WorkerGUID == worker).ToList().Select(e => e);

						var deviceSumByWorkerGUID = from e in documentHeaders
													orderby e.WorkerGUID
													group e by new
													{
														e.WorkerGUID
													} into g
													select new Device
													{
														DeviceWorkerKey = g.Key.WorkerGUID,
														WorkerName = g.Key.WorkerGUID,
														WorkerID = g.Key.WorkerGUID,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};

						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumByWorkerGUID.Where(x => x.DeviceWorkerKey == worker).FirstOrDefault(); //deviceWorker = x.WorkerGUID
							if (deviceSum != null)
							{
								deviceSumList.Add(deviceSum);
							}
						}
						catch { }

					}
				}            //end foreach	   deviceWorkerList     //x.WorkerGUID


				foreach (var worker in workerList)        //x.WorkerGUID
				{
					if (string.IsNullOrEmpty(worker) != true)
					{
						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumList.Where(x => x.DeviceWorkerKey == worker).FirstOrDefault();
						}
						catch { }


						DateTime theFirst = DateTime.Now;
						DateTime theLast = theFirst;
						long ticksTimeSpan = 0;
						string periodFromFirstToLast = "00:00:00";
						int periodFromFirstToLastDays = 0;
						int periodFromFirstToLastH = 0;
						int periodFromFirstToLastMin = 0;
						int periodFromFirstToLastSec = 0;
						long total = 0;
						double quantityEdit = 0;


						try
						{
							List<Device> deviceList = deviceWorkerSumList.Where(s=>s.WorkerID == worker).Select(s => s).ToList();
							if (deviceList != null) {
								foreach (var device in deviceList)
								{
									theFirst = device.TheFirst;
									theLast = device.TheLast;
									TimeSpan fromFirstToLastDevice = (TimeSpan)(theLast - theFirst);
									ticksTimeSpan += fromFirstToLastDevice.Ticks;
								}
							}
						}
						catch { }


						//TimeSpan fromFirstToLast = (TimeSpan)(theLast - theFirst);
						TimeSpan fromFirstToLast = new TimeSpan(ticksTimeSpan);
						try
						{
							//ticksTimeSpan = fromFirstToLast.Ticks;
							periodFromFirstToLastDays = fromFirstToLast.Days;
							periodFromFirstToLastH = fromFirstToLast.Hours;
							periodFromFirstToLastMin = fromFirstToLast.Minutes;
							periodFromFirstToLastSec = fromFirstToLast.Seconds;
							periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastMin.ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss
							total = deviceSum.Total;
							quantityEdit = deviceSum.QuantityEdit;
						}
						catch { }
						deviceSum.QuantityEdit = quantityEdit;
						deviceSum.Total = total;

						//long total10000 = total * 10000;
						//double quantityEdit10000 = total * 10000;
						deviceSum.TotalString = total.ToString("N0", culture);
						deviceSum.QuantityEditString = quantityEdit.ToString("N0", culture);

						//deviceSum.TheFirst = theFirst;
						//deviceSum.TheLast = theLast;
						deviceSum.TicksTimeSpan = ticksTimeSpan;
						deviceSum.PeriodFromFirstToLast = periodFromFirstToLast;
						{
							//int minuts = (fromFirstToLast.Days * 24) * 60 + fromFirstToLast.Hours * 60 + fromFirstToLast.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromFirstToLast.Days * 24) * 3600 + fromFirstToLast.Hours * 3600 + fromFirstToLast.Minutes * 60 + +fromFirstToLast.Seconds;
							if (secs == 0)
							{
								secs = 1;
							}

							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromFirstToLast = edit1;
							deviceSum.QuantityPerHourFromFirstToLastString = edit1.ToString("N0", culture); ;
							deviceSum.TotalPerHourFromFirstToLast = total1;
							deviceSum.TotalPerHourFromFirstToLastString = total1.ToString("N0", culture); ;
						}
						deviceSum.StartInventorDateTime = startInventorDate;
						deviceSum.EndInventorDateTime = endInventorDate;


						TimeSpan fromInventorToEndInventor = (TimeSpan)(endInventorDate - startInventorDate);
						long ticksInventorToEndInventorTimeSpan = 0;
						string periodFromInventorToEndInventor = "00:00";
						int periodFromInventorToEndDays = 0;
						int periodFromInventorToEndH = 0;
						int periodFromInventorToEndMin = 0;
						int periodFromInventorToEndSec = 0;
						try
						{
							ticksInventorToEndInventorTimeSpan = fromInventorToEndInventor.Ticks;
							periodFromInventorToEndDays = fromInventorToEndInventor.Days;
							periodFromInventorToEndH = fromInventorToEndInventor.Hours;
							periodFromInventorToEndMin = fromInventorToEndInventor.Minutes;
							periodFromInventorToEndSec = fromInventorToEndInventor.Seconds;
							periodFromInventorToEndInventor = (periodFromInventorToEndDays * 24 + periodFromInventorToEndH).ToString().PadLeft(2, '0') + ":" +
																	periodFromInventorToEndMin.ToString().PadLeft(2, '0');// + ":" +
																	//periodFromInventorToEndSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss


							//periodFromInventorToEndInventor = fromInventorToEndInventor.ToString(@"dd\:hh\:mm");
						}
						catch { }
						deviceSum.PeriodFromStartInventorToEndInventor = periodFromInventorToEndInventor;

						{

							//int minuts = (fromInventorToEndInventor.Days * 24) * 60 + fromInventorToEndInventor.Hours * 60 + fromInventorToEndInventor.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//	//deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromInventorToEndInventor.Days * 24) * 3600 + fromInventorToEndInventor.Hours * 3600 + fromInventorToEndInventor.Minutes * 60 + fromInventorToEndInventor.Seconds; ;
							if (secs == 0)
							{
								secs = 1;
							}
							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromStartInventorToEndInventor = edit1;
							deviceSum.TotalPerHourFromStartInventorToEndInventor = total1;

							deviceSum.QuantityPerHourFromStartInventorToEndInventorString = edit1.ToString("N0", culture);
							deviceSum.TotalPerHourFromStartInventorToEndInventorString = total1.ToString("N0", culture);
						}


						// ============ from deviceEntity

						//string periodAddTime = "00:00:00";
						//убрала в новой версии
						//TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
						//try
						//{

						//	bool ret = TimeSpan.TryParse(deviceEntity.Description, out addTime);
						//	if (ret == true)
						//	{
						//		ret = TimeSpan.TryParse(deviceEntity.Description + ":00", out addTime);
						//		if (ret == true)
						//		{
						//			periodAddTime = addTime.ToString(@"dd\:hh\:mm");               //\:ss
						//		}
						//	}
						//}
						//catch { }
						//deviceSum.PeriodAddtionTime = periodAddTime;

						//=========
						//string sumTime = "00:00:00";

						//TimeSpan fromStartInventorToTheFirstTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst, out fromStartInventorToTheFirstTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst + ":00", out fromStartInventorToTheFirstTime);
						//	}
						//}

						//TimeSpan fromStartInventorToEndInventorTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor, out fromStartInventorToEndInventorTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor /*+ ":00"*/, out fromStartInventorToEndInventorTime);
						//	}
						//}

						//TimeSpan fromFirstToLastTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast, out fromFirstToLastTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast/* + ":00"*/, out fromFirstToLastTime);
						//	}
						//}

						//TimeSpan sum = new TimeSpan(0, 0, 0, 0);
						//{
						//	//sum = addTime + fromStartInventorToTheFirstTime + fromFirstToLastTime;
						//	//sum = addTime + fromStartInventorToEndInventorTime;
						//	sum = fromStartInventorToEndInventorTime;
						//	sumTime = deviceSum.PeriodFromStartInventorToEndInventor; //sum.ToString(@"dd\:hh\:mm");
						//}

						//{
							//int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}

							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

						//	int secs = (sum.Days * 24) * 3600 + sum.Hours * 3600 + sum.Minutes * 60 + sum.Seconds; ;
						//	if (secs == 0)
						//	{
						//		secs = 1;
						//	}
						//	double editPerSec = quantityEdit / ((double)secs / 3600.0);
						//	double totalPerSec = total / ((double)secs / 3600.0);

						//	int edit1 = (int)(editPerSec);
						//	int total1 = (int)(totalPerSec);
						//	deviceSum.QuantityPerHourTotal = edit1;
						//	deviceSum.TotalPerHourTotal = edit1;
						//}

						deviceSum.SumPeriod = periodFromInventorToEndInventor;
						//SumPeriod

						//entity.ApplyChanges(device);
						returnDeviceSumList.Add(deviceSum);
						//}	 //end						if (entity != null)
					}
				}
				//}//foreach
				//	db.SaveChanges();
			}       //db
			return returnDeviceSumList;
		}

			// Rep-DW1-12
			// Rep-DW1-12dp
		public Devices RefillDeviceStatisticByDeviceAndWorker(DateTime startInventorDate, DateTime endInventorDate, /*SelectParams selectParams, */string pathDB)
		{
			Devices deviceSumList = new Devices();
			Devices returnDeviceSumList = new Devices();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> deviceWorkerList = db.DocumentHeaders.Select(s => s.Name + "|" + s.WorkerGUID).Distinct().ToList();      //Name == Device


				foreach (var deviceWorker in deviceWorkerList)     //x.Name + "|" + x.WorkerGUID
				{
					if (string.IsNullOrEmpty(deviceWorker) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => (x.Name + "|" + x.WorkerGUID) == deviceWorker).ToList().Select(e => e);

						var deviceSumByDeviceCode = from e in documentHeaders
													orderby e.Name, e.WorkerGUID
													group e by new
													{
														e.Name,
														e.WorkerGUID
													} into g
													select new Device
													{
														DeviceWorkerKey = g.Key.Name + "|" + g.Key.WorkerGUID,
														DeviceCode = g.Key.Name,
														Name = g.Key.Name,
														WorkerName = g.Key.WorkerGUID,
														WorkerID = g.Key.WorkerGUID,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};



						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumByDeviceCode.Where(x => x.DeviceWorkerKey == deviceWorker).FirstOrDefault(); //deviceWorker = x.Name + "|" + x.WorkerGUID
							if (deviceSum != null)
							{
								deviceSumList.Add(deviceSum);
							}
						}
						catch { }

					}
				}            //end foreach	   deviceWorkerList     //x.Name + "|" + x.WorkerGUID


				foreach (var deviceWorker in deviceWorkerList)        //x.Name + "|" + x.WorkerGUID
				{
					if (string.IsNullOrEmpty(deviceWorker) != true)
					{
						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumList.Where(x => x.DeviceWorkerKey == deviceWorker).FirstOrDefault();
						}
						catch { }


						DateTime theFirst = DateTime.Now;
						DateTime theLast = theFirst;
						long ticksTimeSpan = 0;
						string periodFromFirstToLast = "00:00:00";
						int periodFromFirstToLastDays = 0;
						int periodFromFirstToLastH = 0;
						int periodFromFirstToLastMin = 0;
						int periodFromFirstToLastSec = 0;
						long total = 0;
						double quantityEdit = 0;


						try
						{
							theFirst = deviceSum.TheFirst;
							theLast = deviceSum.TheLast;
						}
						catch { }
						TimeSpan fromFirstToLast = (TimeSpan)(theLast - theFirst);

						try
						{
							ticksTimeSpan = fromFirstToLast.Ticks;
							periodFromFirstToLastDays = fromFirstToLast.Days;
							periodFromFirstToLastH = fromFirstToLast.Hours;
							periodFromFirstToLastMin = fromFirstToLast.Minutes;
							periodFromFirstToLastSec = fromFirstToLast.Seconds;
							periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastMin.ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss
							total = deviceSum.Total;
							quantityEdit = deviceSum.QuantityEdit;
						}
						catch { }
						deviceSum.QuantityEdit = quantityEdit;
						deviceSum.Total = total;

						//long total10000 = total * 10000;
						//double quantityEdit10000 = total * 10000;
						deviceSum.TotalString = total.ToString("N0", culture);
						deviceSum.QuantityEditString = quantityEdit.ToString("N0", culture);

						//deviceSum.TheFirst = theFirst;
						//deviceSum.TheLast = theLast;
						deviceSum.TicksTimeSpan = ticksTimeSpan;
						deviceSum.PeriodFromFirstToLast = periodFromFirstToLast;
						{
							//int minuts = (fromFirstToLast.Days * 24) * 60 + fromFirstToLast.Hours * 60 + fromFirstToLast.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromFirstToLast.Days * 24) * 3600 + fromFirstToLast.Hours * 3600 + fromFirstToLast.Minutes * 60 + +fromFirstToLast.Seconds;
							if (secs == 0)
							{
								secs = 1;
							}

							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromFirstToLast = edit1;
							deviceSum.QuantityPerHourFromFirstToLastString = edit1.ToString("N0", culture); ;
							deviceSum.TotalPerHourFromFirstToLast = total1;
							deviceSum.TotalPerHourFromFirstToLastString = total1.ToString("N0", culture); ;
						}
						deviceSum.StartInventorDateTime = startInventorDate;
						deviceSum.EndInventorDateTime = endInventorDate;


						TimeSpan fromInventorToEndInventor = (TimeSpan)(endInventorDate - startInventorDate);
						long ticksInventorToEndInventorTimeSpan = 0;
						string periodFromInventorToEndInventor = "00:00";
						int periodFromInventorToEndDays = 0;
						int periodFromInventorToEndH = 0;
						int periodFromInventorToEndMin = 0;
						int periodFromInventorToEndSec = 0;
						try
						{
							ticksInventorToEndInventorTimeSpan = fromInventorToEndInventor.Ticks;
							periodFromInventorToEndDays = fromInventorToEndInventor.Days;
							periodFromInventorToEndH = fromInventorToEndInventor.Hours;
							periodFromInventorToEndMin = fromInventorToEndInventor.Minutes;
							periodFromInventorToEndSec = fromInventorToEndInventor.Seconds;
							periodFromInventorToEndInventor = (periodFromInventorToEndDays * 24 + periodFromInventorToEndH).ToString().PadLeft(2, '0') + ":" +
																	periodFromInventorToEndMin.ToString().PadLeft(2, '0');// + ":" +
															//	periodFromInventorToEndSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss


							//periodFromInventorToEndInventor = fromInventorToEndInventor.ToString(@"dd\:hh\:mm");
						}
						catch { }
						deviceSum.PeriodFromStartInventorToEndInventor = periodFromInventorToEndInventor;

						{

							//int minuts = (fromInventorToEndInventor.Days * 24) * 60 + fromInventorToEndInventor.Hours * 60 + fromInventorToEndInventor.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//	//deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
							//}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							int secs = (fromInventorToEndInventor.Days * 24) * 3600 + fromInventorToEndInventor.Hours * 3600 + fromInventorToEndInventor.Minutes * 60 + fromInventorToEndInventor.Seconds; ;
							if (secs == 0)
							{
								secs = 1;
							}
							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromStartInventorToEndInventor = edit1;
							deviceSum.TotalPerHourFromStartInventorToEndInventor = total1;

							deviceSum.QuantityPerHourFromStartInventorToEndInventorString = edit1.ToString("N0", culture);
							deviceSum.TotalPerHourFromStartInventorToEndInventorString = total1.ToString("N0", culture);

							deviceSum.QuantityPerHourTotal = edit1;				   //? 
							deviceSum.TotalPerHourTotal = total1;
						}


						// ============ from deviceEntity

						//string periodAddTime = "00:00:00";
						//убрала в новой версии
						//TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
						//try
						//{

						//	bool ret = TimeSpan.TryParse(deviceEntity.Description, out addTime);
						//	if (ret == true)
						//	{
						//		ret = TimeSpan.TryParse(deviceEntity.Description + ":00", out addTime);
						//		if (ret == true)
						//		{
						//			periodAddTime = addTime.ToString(@"dd\:hh\:mm");               //\:ss
						//		}
						//	}
						//}
						//catch { }
						//deviceSum.PeriodAddtionTime = periodAddTime;

						//=========
						//string sumTime = "00:00:00";

						//TimeSpan fromStartInventorToTheFirstTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst, out fromStartInventorToTheFirstTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst + ":00", out fromStartInventorToTheFirstTime);
						//	}
						//}

						//TimeSpan fromStartInventorToEndInventorTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor, out fromStartInventorToEndInventorTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor /*+ ":00"*/, out fromStartInventorToEndInventorTime);
						//	}
						//}

						//TimeSpan fromFirstToLastTime = new TimeSpan(0, 0, 0, 0);
						//{
						//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast, out fromFirstToLastTime);
						//	if (ret1 == true)
						//	{
						//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast/* + ":00"*/, out fromFirstToLastTime);
						//	}
						//}

						//TimeSpan sum = new TimeSpan(0, 0, 0, 0);
						//{
							//sum = addTime + fromStartInventorToTheFirstTime + fromFirstToLastTime;
							//sum = addTime + fromStartInventorToEndInventorTime;
						//	sum = fromStartInventorToEndInventorTime;
						//	sumTime = deviceSum.PeriodFromStartInventorToEndInventor; //sum.ToString(@"dd\:hh\:mm");
						//}

						//{
							//int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
							//if (minuts == 0)
							//{
							//	minuts = 1;
							//}

							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

						//	int secs = (sum.Days * 24) * 3600 + sum.Hours * 3600 + sum.Minutes * 60 + sum.Seconds; ;
						//	if (secs == 0)
						//	{
						//		secs = 1;
						//	}
						//	double editPerSec = quantityEdit / ((double)secs / 3600.0);
						//	double totalPerSec = total / ((double)secs / 3600.0);

						//	int edit1 = (int)(editPerSec);
						//	int total1 = (int)(totalPerSec);
						//	deviceSum.QuantityPerHourTotal = edit1;
						//	deviceSum.TotalPerHourTotal = edit1;
						//}

						deviceSum.SumPeriod = periodFromInventorToEndInventor;			  //??
						//SumPeriod

						//entity.ApplyChanges(device);
						returnDeviceSumList.Add(deviceSum);
						//}	 //end						if (entity != null)
					}
				}
				//}//foreach
				//	db.SaveChanges();
			}       //db
			return returnDeviceSumList;
		}

		//DW1-11, DW1-11p
		public Devices RefillDeviceStatisticByDeviceAndWorkerAndItur(DateTime startInventorDate, DateTime endInventorDate, SelectParams selectParams, string pathDB)
		{
			Devices deviceSumList = new Devices();
			Devices returnDeviceSumList = new Devices();
	

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				List<string> deviceWorkerIturList = db.DocumentHeaders.Select(s => s.Name + "|" + s.WorkerGUID + "|" + s.IturCode).Distinct().ToList();      //Name == Device

				foreach (var deviceWorkerItur in deviceWorkerIturList)     //x.Name + "|" + x.WorkerGUID
				{
					if (string.IsNullOrEmpty(deviceWorkerItur) != true)
					{
						var documentHeaders = AsQueryable(db.DocumentHeaders).Where(x => (x.Name + "|" + x.WorkerGUID + "|" + x.IturCode) == deviceWorkerItur).ToList().Select(e => e);

						var deviceSumByDeviceCode = from e in documentHeaders
													orderby e.Name , e.WorkerGUID, e.IturCode
													//group e by e.Name into g
													group e by new
													{
														e.Name,
														e.WorkerGUID ,
														 e.IturCode
													} into g
													select new Device
													{
														DeviceWorkerKey = g.Key.Name + "|" + g.Key.WorkerGUID + "|" + g.Key.IturCode,
														DeviceCode = g.Key.Name,
														Name = g.Key.Name,
														WorkerName = g.Key.WorkerGUID,
														WorkerID = g.Key.WorkerGUID,
														IturCode = g.Key.IturCode,
														QuantityEdit = (double)g.Sum(x => x.QuantityEdit),
														TheFirst = g.Min(x => (DateTime)x.FromTime),
														TheLast = g.Max(x => (DateTime)x.ToTime),
														Total = (long)g.Sum(x => x.Total)
													};

						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumByDeviceCode.Where(x => x.DeviceWorkerKey == deviceWorkerItur).FirstOrDefault(); //deviceWorker = x.Name + "|" + x.WorkerGUID
							if (deviceSum != null)
							{
								deviceSumList.Add(deviceSum);
							}
						}
						catch { }
				
					}
				}            //end foreach	   deviceWorkerList     //x.Name + "|" + x.WorkerGUID	+ "|" + x.IturCode


				foreach (var deviceWorkerItur in deviceWorkerIturList)        //x.Name + "|" + x.WorkerGUID	  + "|" + x.IturCode
				{
					if (string.IsNullOrEmpty(deviceWorkerItur) != true)
					{
						Device deviceSum = null;

						try
						{
							deviceSum = deviceSumList.Where(x => x.DeviceWorkerKey == deviceWorkerItur).FirstOrDefault();
						}
						catch { }

							DateTime theFirst = DateTime.Now;
							DateTime theLast = theFirst;
							long ticksTimeSpan = 0;
							string periodFromFirstToLast = "00:00:00";
							int periodFromFirstToLastDays = 0;
							int periodFromFirstToLastH = 0;
							int periodFromFirstToLastMin = 0;
							int periodFromFirstToLastSec = 0;
							long total = 0;
							double quantityEdit = 0;

							try
							{
								theFirst = deviceSum.TheFirst;
								theLast = deviceSum.TheLast;
							}
							catch { }
							TimeSpan fromFirstToLast = (TimeSpan)(theLast - theFirst);

							try
							{
								ticksTimeSpan = fromFirstToLast.Ticks;
								periodFromFirstToLastDays = fromFirstToLast.Days;
								periodFromFirstToLastH = fromFirstToLast.Hours;
								periodFromFirstToLastMin = fromFirstToLast.Minutes;
								periodFromFirstToLastSec = fromFirstToLast.Seconds;
							periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
																	periodFromFirstToLastMin.ToString().PadLeft(2, '0') + ":" +
																		periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss

								total = deviceSum.Total;
								quantityEdit = deviceSum.QuantityEdit;
							}
							catch { }
							deviceSum.QuantityEdit = quantityEdit;
							deviceSum.Total = total;

						//long total10000 = total * 10000;
						//double quantityEdit10000 = total * 10000;
						deviceSum.TotalString = total.ToString("N0", culture);
						deviceSum.QuantityEditString = quantityEdit.ToString("N0", culture);


						//deviceSum.TheFirst = theFirst;
						//	deviceSum.TheLast = theLast;
							deviceSum.TicksTimeSpan = ticksTimeSpan;
							deviceSum.PeriodFromFirstToLast = periodFromFirstToLast;
							{
							//	int minuts = (fromFirstToLast.Days * 24) * 60 + fromFirstToLast.Hours * 60 + fromFirstToLast.Minutes;
							//	if (minuts == 0)
							//	{
							//		minuts = 1;
							//		deviceSum.PeriodFromFirstToLast = "00:00:01";
							//	}

							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//	double totalPerMinute = total / ((double)minuts / 60.0);

							//	int edit1 = (int)(editPerMinute);
							//	int total1 = (int)(totalPerMinute);

							int secs = (fromFirstToLast.Days * 24) * 3600 + fromFirstToLast.Hours * 3600 + fromFirstToLast.Minutes * 60 + +fromFirstToLast.Seconds;
							if (secs == 0)
							{
								secs = 1;
							}

							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);

							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromFirstToLast = edit1;
							deviceSum.QuantityPerHourFromFirstToLastString = edit1.ToString("N0", culture); 
							deviceSum.TotalPerHourFromFirstToLast = total1;
							deviceSum.TotalPerHourFromFirstToLastString = total1.ToString("N0", culture);
						}
							deviceSum.StartInventorDateTime = startInventorDate;
							deviceSum.EndInventorDateTime = endInventorDate;

							
							//===============  FromStartInventorToEndInventor ===================

							TimeSpan fromInventorToEndInventor = (TimeSpan)(endInventorDate - startInventorDate);
							long ticksInventorToEndInventorTimeSpan = 0;
							string periodFromInventorToEndInventor = "00:00";
							int periodFromInventorToEndDays = 0;
							int periodFromInventorToEndH = 0;
							int periodFromInventorToEndMin = 0;
							int periodFromInventorToEndSec = 0;
						try
							{
								ticksInventorToEndInventorTimeSpan = fromInventorToEndInventor.Ticks;
							periodFromInventorToEndDays = fromInventorToEndInventor.Days;
							periodFromInventorToEndH = fromInventorToEndInventor.Hours;
							periodFromInventorToEndMin = fromInventorToEndInventor.Minutes;
							periodFromInventorToEndSec = fromInventorToEndInventor.Seconds;
							periodFromInventorToEndInventor = (periodFromInventorToEndDays * 24 + periodFromInventorToEndH).ToString().PadLeft(2, '0') + ":" +
																	periodFromInventorToEndMin.ToString().PadLeft(2, '0');// + ":" +
																	//periodFromInventorToEndSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");               //\:ss

							//periodFromInventorToEndInventor = fromInventorToEndInventor.ToString(@"dd\:hh\:mm");
						}
						catch { }
							deviceSum.PeriodFromStartInventorToEndInventor = periodFromInventorToEndInventor;

							{
							//	int minuts = (fromInventorToEndInventor.Days * 24) * 60 + fromInventorToEndInventor.Hours * 60 + fromInventorToEndInventor.Minutes;

							//	if (minuts == 0)
							//	{
							//		minuts = 1;
							//		deviceSum.PeriodFromStartInventorToEndInventor = "00:00:01";
							//	}
							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//	double totalPerMinute = total / ((double)minuts / 60.0);

							//	//int qEdit1 = (int)(qEdit * 100);
							//	//deviceSum.QuantityPerHourFromStartInventorToEndInventor = (double)(qEdit1) / 100.00;
							//	int edit1 = (int)(editPerMinute);
							//	int total1 = (int)(totalPerMinute);
							int secs = (fromInventorToEndInventor.Days * 24) * 3600 + fromInventorToEndInventor.Hours * 3600 + fromInventorToEndInventor.Minutes * 60 + fromInventorToEndInventor.Seconds; ;
							if (secs == 0)
							{
								secs = 1;
							}
							double editPerSec = quantityEdit / ((double)secs / 3600.0);
							double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							int edit1 = (int)Math.Round(editPerSec);//(int)(editPerSec); 
							int total1 = (int)Math.Round(totalPerSec);//(int)(totalPerSec);

							deviceSum.QuantityPerHourFromStartInventorToEndInventor = edit1;
							deviceSum.TotalPerHourFromStartInventorToEndInventor = total1;

							deviceSum.QuantityPerHourFromStartInventorToEndInventorString = edit1.ToString("N0", culture);
							deviceSum.TotalPerHourFromStartInventorToEndInventorString = total1.ToString("N0", culture);
						}


							// ============ from deviceEntity

							//string periodAddTime = "00:00:00";
							//убрала в новой версии
							//TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
							//try
							//{

							//	bool ret = TimeSpan.TryParse(deviceEntity.Description, out addTime);
							//	if (ret == true)
							//	{
							//		ret = TimeSpan.TryParse(deviceEntity.Description + ":00", out addTime);
							//		if (ret == true)
							//		{
							//			periodAddTime = addTime.ToString(@"dd\:hh\:mm");               //\:ss
							//		}
							//	}
							//}
							//catch { }
							//deviceSum.PeriodAddtionTime = periodAddTime;

							//=========
							//string sumTime = "00:00:00";

							//TimeSpan fromStartInventorToTheFirstTime = new TimeSpan(0, 0, 0, 0);
							//{
							//	bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst, out fromStartInventorToTheFirstTime);
							//	if (ret1 == true)
							//	{
							//		ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToTheFirst + ":00", out fromStartInventorToTheFirstTime);
							//	}
							//}

						//	TimeSpan fromStartInventorToEndInventorTime = new TimeSpan(0, 0, 0, 0);
						//	{
						//		bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor, out fromStartInventorToEndInventorTime);
						//		if (ret1 == true)
						//		{
						//			ret1 = TimeSpan.TryParse(deviceSum.PeriodFromStartInventorToEndInventor + ":00", out fromStartInventorToEndInventorTime);
						//		}
						//	}

						//	TimeSpan fromFirstToLastTime = new TimeSpan(0, 0, 0, 0);
						//	{
						//		bool ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast, out fromFirstToLastTime);
						//		if (ret1 == true)
						//		{
						//			ret1 = TimeSpan.TryParse(deviceSum.PeriodFromFirstToLast + ":00", out fromFirstToLastTime);
						//		}
						//	}

						//	TimeSpan sum = new TimeSpan(0, 0, 0, 0);
						//	{
						//		//sum = addTime + fromStartInventorToTheFirstTime + fromFirstToLastTime;
						//		//sum = addTime + fromStartInventorToEndInventorTime;
						//		sum = fromStartInventorToEndInventorTime;
						//	sumTime = deviceSum.PeriodFromStartInventorToEndInventor; //sum.ToString(@"dd\:hh\:mm");
						//}

							//{
							//int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
							//if (minuts == 0) minuts = 1;

							//double editPerMinute = quantityEdit / ((double)minuts / 60.0);
							//double totalPerMinute = total / ((double)minuts / 60.0);

							//int edit1 = (int)(editPerMinute);
							//int total1 = (int)(totalPerMinute);

							//int secs = (sum.Days * 24) * 3600 + sum.Hours * 3600 + sum.Minutes * 60 + sum.Seconds; ;
							//if (secs == 0)
							//{
							//	secs = 1;
							//}
							//double editPerSec = quantityEdit / ((double)secs / 3600.0);
							//double totalPerSec = total / ((double)secs / 3600.0);

							//int edit1 = (int)(editPerSec);
							//int total1 = (int)(totalPerSec);
							//deviceSum.QuantityPerHourTotal = edit1;
							//	deviceSum.TotalPerHourTotal = edit1;
							//}

							deviceSum.SumPeriod = periodFromInventorToEndInventor;
							//SumPeriod

							//entity.ApplyChanges(device);
							returnDeviceSumList.Add(deviceSum);
						//}	 //end						if (entity != null)
					}
				}
				//}//foreach
				//	db.SaveChanges();
			}       //db
			return returnDeviceSumList;
		}
		//работает
		//public void RefillDocumentStatistic(List<string> docCodes, string pathDB)
		//{
		//	//string inputTypeCodeB = InputTypeCodeEnum.B.ToString();
		//	//Dictionary<string, DocumentHeader> documentHeaderDictionary = this.GetDocumentHeaderDictionary(pathDB, true);

		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		Dictionary<string, App_Data.DocumentHeader> documentHeaderEntityDictionary = db.DocumentHeaders.Select(e => e).Distinct().ToDictionary(k => k.DocumentCode);
		//		List<string> iturCodes = new List<string>();
		//		foreach (var docCode in docCodes)
		//		{
		//			//Parallel.ForEach(docCodes, docCode =>
		//			//{
		//			if (string.IsNullOrEmpty(docCode) != true)
		//			{
		//				//DocumentHeader documentHeader = new DocumentHeader();
		//				App_Data.DocumentHeader entity = new App_Data.DocumentHeader();
		//				bool retCan = documentHeaderEntityDictionary.TryGetValue(docCode, out entity);
		//				//bool retCan = documentHeaderDictionary.TryGetValue(docCode, out documentHeader);
		//				if (retCan == true)
		//				{
		//					var documentHeader = entity.ToDomainObject();
		//					iturCodes.Add(documentHeader.IturCode);
		//					double quantityEdit = 0;
		//					long total = 0;
		//					DateTime fromTime = DateTime.Now;
		//					DateTime toTime = fromTime;
		//					long ticksTimeSpan = 0;
		//					string periodFromTo = "00:00:00";
		//					//&& e.InputTypeCode == inputTypeCodeB)

		//					var inventProductEntities = db.InventProducts.Where(e => e.DocumentCode == docCode).ToList().Select(e => e.ToDomainObject());
		//					if (inventProductEntities != null && inventProductEntities.Count() > 0)
		//					{

		//						try { quantityEdit = inventProductEntities.Sum(x => x.QuantityEdit); }
		//						catch { }

		//						total = inventProductEntities.LongCount();

		//						try
		//						{
		//							fromTime = inventProductEntities.Min(x => x.CreateDate);
		//							toTime = inventProductEntities.Max(x => x.CreateDate);
		//						}
		//						catch { }
		//						TimeSpan fromTo = (TimeSpan)(toTime - fromTime);

		//						try
		//						{
		//							ticksTimeSpan = fromTo.Ticks;
		//							periodFromTo = fromTo.ToString(@"hh\:mm\:ss");
		//						}
		//						catch { }
		//					}

		//					documentHeader.QuantityEdit = quantityEdit;
		//					documentHeader.Total = total;
		//					documentHeader.FromTime = fromTime;
		//					documentHeader.ToTime = toTime;
		//					documentHeader.TicksTimeSpan = ticksTimeSpan;
		//					documentHeader.PeriodFromTo = periodFromTo;
		//					entity.ApplyChanges(documentHeader);
		//				}
		//			}
		//			//db.SaveChanges();
		//			//}); //Parallel.ForEach docCodes
		//		}//foreach

		//		iturCodes = iturCodes.Select(x => x).Distinct().ToList();
		//		//fo Iturs
		//		Dictionary<string, App_Data.Itur> iturEntityDictionary = db.Iturs.Select(e => e).Distinct().ToDictionary(k => k.IturCode);
		//		foreach (var code in iturCodes)
		//		{
		//			if (string.IsNullOrEmpty(code) != true)
		//			{
		//				App_Data.Itur iturEntity = new App_Data.Itur();
		//				bool retCan = iturEntityDictionary.TryGetValue(code, out iturEntity);
		//				//bool retCan = documentHeaderDictionary.TryGetValue(docCode, out documentHeader);
		//				if (retCan == true)
		//				{
		//					var iturDomain = iturEntity.ToDomainObject();
		//					double quantityEdit = 0;
		//					long total = 0;
		//					//DateTime fromTime = DateTime.Now;
		//					//DateTime toTime = fromTime;
		//					//long ticksTimeSpan = 0;
		//					//string periodFromTo = "00:00:00";

		//					var inventProductEntities = db.InventProducts.Where(e => e.IturCode == code).ToList().Select(e => e.ToDomainObject());  //&& e.InputTypeCode == inputTypeCodeB
		//					if (inventProductEntities != null && inventProductEntities.Count() > 0)
		//					{

		//						try { quantityEdit = inventProductEntities.Sum(x => x.QuantityEdit); }
		//						catch { }

		//						//total = inventProductEntities.LongCount();
		//						List<string> ipCode = inventProductEntities.Select(x => x.Makat).Distinct().ToList();
		//						total = ipCode.LongCount();

		//						//try
		//						//{
		//						//	fromTime = inventProductEntities.Min(x => x.CreateDate);
		//						//	toTime = inventProductEntities.Max(x => x.CreateDate);
		//						//}
		//						//catch { }
		//						//TimeSpan fromTo = (TimeSpan)(toTime - fromTime);

		//						//try
		//						//{
		//						//	ticksTimeSpan = fromTo.Ticks;
		//						//	periodFromTo = fromTo.ToString(@"hh\:mm\:ss");
		//						//}
		//						//catch { }
		//					}

		//					iturDomain.SumQuantityEdit = quantityEdit;
		//					iturDomain.TotalItem = total; // Distinct Makat
		//					//need add DistinctMakat
		//					//iturDomain.FromTime = fromTime;
		//					//iturDomain.ToTime = toTime;
		//					//iturDomain.TicksTimeSpan = ticksTimeSpan;
		//					//iturDomain.PeriodFromTo = periodFromTo;
		//					iturEntity.ApplyChanges(iturDomain);

		//				}
		//			}
		//		}//foreach iturcode

		//		db.SaveChanges();
		//	}
		//	//documentHeaderEntityDictionary.Clear();
		//	//documentHeaderEntityDictionary = null;

		//}		//db


		//public string GetFistDeviceCodeWithoutIturs(string pathDB)
		//{
		//	using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//	{
		//		List<App_Data.Location> entities = db.Locations.OrderBy(e => e.Code).Select(e => e).ToList();
		//		foreach (var entety in entities)
		//		{
		//			int countItursInLocation = db.Iturs.Where(x => x.LocationCode.CompareTo(entety.Code) == 0).Count();
		//			if (countItursInLocation == 0) return entety.Code;
		//		}
		//		var entety1 = entities.FirstOrDefault();
		//		if (entety1 == null) return "";
		//		return entety1.Code;

		//	}
		//}




		#endregion

		#region Dictionary

		public Dictionary<string, Device> GetDeviceDictionary(string pathDB,
			bool refill = false)
		{
			if (refill == true)
			{
				this.ClearDeviceDictionary();
				this.FillDeviceDictionary(pathDB);
			}
			return this._deviceDictionary;
		}

		public void ClearDeviceDictionary()
		{
			this._deviceDictionary.Clear();
			GC.Collect();
		}

		public void AddDeviceInDictionary(string code, Device device)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			if (this._deviceDictionary.ContainsKey(code) == false)
			{
				this._deviceDictionary.Add(code, device);
			}
		}

		public void RemoveDeviceFromDictionary(string code)
		{
			try
			{
				this._deviceDictionary.Remove(code);
			}
			catch { }
		}

		public bool IsExistDeviceInDictionary(string code)
		{
			if (this._deviceDictionary.ContainsKey(code) == true) return true;
			else return false;
		}

		public Device GetDeviceByCodeFromDictionary(string code)
		{
			if (this._deviceDictionary.ContainsKey(code) == true)
			{
				return this._deviceDictionary[code];
			}
			return null;
		}

		public void FillDeviceDictionary(string pathDB)
		{
			this.ClearDeviceDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Devices devices = this.GetDevices(pathDB);
					//this._locationDictionary = db.Locations.Select(e => new Location
					//{
					//    Code = e.Code,
					//    Name = e.Name,
					//    BackgroundColor = e.BackgroundColor,
					//    Description = e.Description
					//}).Distinct().ToDictionary(k => k.Code);

					this._deviceDictionary = devices.Select(e => e).Distinct().ToDictionary(k => k.DeviceCode); 
					//foreach (var i in this._locationDictionary)
					//{
					//    var f = i.Key;
					//    var f1 = i.Value;
					//}
				}
				catch { }
			}
		}

		#endregion

        #region private

		private App_Data.Device GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = db.Devices.FirstOrDefault(e => e.DeviceCode.CompareTo(code) == 0);
			return entity;
		}


		private App_Data.Device GetEntityByName(App_Data.Count4UDB db, string name)
        {
			var entity = db.Devices.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
            return entity;
        }

		public List<string> GetDeviceCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Devices.Select(e => e.DeviceCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetDeviceCodeList", exp);
				}
			}
			return ret;
		}

		//public void RepairCodeFromDB(string pathDB)
		//{
		//	IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
		//	List<string> locationCodeListFromItur = iturRepository.GetLocationCodeList(pathDB);			//из
		//	List<string> locationCodeListFromLocation = this.GetDeviceCodeList(pathDB); //в
		//	Dictionary<string, string> difference = new Dictionary<string, string>();

		//	foreach (var locationCodeFromItur in locationCodeListFromItur)			   //из
		//	{
		//		if (locationCodeListFromLocation.Contains(locationCodeFromItur) == false)		 //в
		//		{
		//			difference[locationCodeFromItur] = locationCodeFromItur;
		//		}
		//	}

		//	foreach (KeyValuePair<string, string> keyValuePair in difference)
		//	{
		//		if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
		//		{
		//			Location locationNew = new Location();
		//			locationNew.Code = keyValuePair.Value;
		//			locationNew.Name = keyValuePair.Value;
		//			//if (locationNew.Code == DomainUnknownCode.UnknownLocation)
		//			//{
		//			//    locationNew.Name = DomainUnknownName.UnknownLocation;
		//			//}
		//			locationNew.RestoreBit = true;
		//			locationNew.Description = "Repair from Itur";
		//			locationNew.Restore = DateTime.Now.ToString();
		//			this.Insert(locationNew, pathDB);
		//		}

		//	}

		//	Location unknownLocation = this.GetLocationByCode(DomainUnknownCode.UnknownLocation, pathDB);
		//	if (unknownLocation == null)
		//	{
		//		Location locationNew = new Location();
		//		locationNew.Code = DomainUnknownCode.UnknownLocation;
		//		locationNew.Name = DomainUnknownName.UnknownLocation;
		//		locationNew.Description = "Repair";
		//		this.Insert(locationNew, pathDB);
		//	}
		//	else
		//	{
		//		unknownLocation.Name = DomainUnknownName.UnknownLocation;
		//		this.Update(unknownLocation, pathDB);
		//	}

		//}
        #endregion
    }
}
