using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Modules.Audit.ViewModels.Catalog
{
    public class DeviceItemViewModel : NotificationObject
    {
		private Device _device;

	
		private string _deviceCode;		//PDANum
		private string _name;		 //WorkerID| WorkerName
		private string _description;		 // PeriodFromToEdit
		private DateTime _dateCreated; //ToTime
		private DateTime _licenseDate;	//FromTime
		private string _workerID;
		private string _workerName;
		private double _quantityEdit;
		private long _total;
		private string _quantityEditString;
		private string _totalString;

		private readonly IEventAggregator _eventAggregator;

		//private int _quantityPerHourFromStartInventorToTheLast;
		private int _quantityPerHourFromFirstToLast;
		private int _quantityPerHourFromStartInventorToEndInventor;
		private int _quantityPerHourTotal;
		private int _totalPerHourFromFirstToLast;
		private int _totalPerHourFromStartInventorToEndInventor;
		private int _totalPerHourTotal;
		
		private String _theFirst;
		private String _startInventorDateTime;
		private String _endInventorDateTime;
		private String _theLast;
		private long _ticksTimeSpan;
		private string _periodFromFirstToLast;
		//private string _periodFromStartInventorToTheFirst;
		private string _periodFromStartInventorToEndInventor;
		private string _periodAddtionTime;
		private string _sumPeriod;



		public DeviceItemViewModel(IEventAggregator eventAggregator, Device device, DateTime startInventorDateTime, DateTime endInventorDateTime)
        {
			this._device = device;
			this._eventAggregator = eventAggregator;
			Load(startInventorDateTime, endInventorDateTime);
        }

		public Device Device
		{
			get { return _device; }
			set { _device = value; }
		}

		//FromTime	= inventorData
		private void Load(DateTime startInventorDateTime, DateTime endInventorDateTime)
		{
			if (this._device != null)
			{

				this.DeviceCode = this._device.DeviceCode;		//PDANum
				this.Name = this._device.Name;						 //WorkerID| WorkerName
				this.Description = this._device.Description;		 // PeriodFromToEdit
				this.DateCreated = this._device.DateCreated;   //ToTime
				this.LicenseDate = startInventorDateTime;//this._device.LicenseDate;	  //FromTime	= inventorData

				// don't save to DB
				this.WorkerID = this._device.WorkerID;
				this._workerName = this._device.WorkerName;
				this.QuantityEdit = this._device.QuantityEdit;
				this.Total = this._device.Total;
				this.QuantityEditString = this._device.QuantityEditString;
				this.TotalString = this._device.TotalString;
				this.QuantityPerHourFromFirstToLast = this._device.QuantityPerHourFromFirstToLast;
				this.QuantityPerHourFromFirstToLastString = this._device.QuantityPerHourFromFirstToLastString;
				//this.QuantityPerHourFromStartInventorToTheLast = this._device.QuantityPerHourFromStartInventorToTheLast;
				this.QuantityPerHourTotal = this._device.QuantityPerHourTotal;
				this.QuantityPerHourFromStartInventorToEndInventor = this._device.QuantityPerHourFromStartInventorToEndInventor;
				this.QuantityPerHourFromStartInventorToEndInventorString = this._device.QuantityPerHourFromStartInventorToEndInventorString;
				this.TotalPerHourTotal = this._device.TotalPerHourTotal;
				this.TotalPerHourFromStartInventorToEndInventor = this._device.TotalPerHourFromStartInventorToEndInventor;
				this.TotalPerHourFromStartInventorToEndInventorString = this._device.TotalPerHourFromStartInventorToEndInventorString;
				this.TotalPerHourFromFirstToLast = this._device.TotalPerHourFromFirstToLast;
				this.TotalPerHourFromFirstToLastString = this._device.TotalPerHourFromFirstToLastString;
				//this.StartInventorDateTime = startInventorDateTime.ToShortDateString() + " " + startInventorDateTime.ToShortTimeString();
				//this.EndInventorDateTime = endInventorDateTime.ToShortDateString() + " " + endInventorDateTime.ToShortTimeString();
				//this.TheFirst = this._device.TheFirst.ToShortDateString() + " " + this._device.TheFirst.ToShortTimeString(); //this._device.FromTime;	 //FromTime == LicenseDate (inventorData)
				//this.TheLast = this._device.TheLast.ToShortDateString() + " " + this._device.TheLast.ToShortTimeString(); 			//ToTime == DateCreated
				this.StartInventorDateTime = startInventorDateTime.ToString(@"dd/MM/yyyy HH:mm") + ":00";
				this.EndInventorDateTime = endInventorDateTime.ToString(@"dd/MM/yyyy HH:mm") + ":00"; 
				this.TheFirst = this._device.TheFirst.ToString(@"dd/MM/yyyy HH:mm:ss"); //this._device.FromTime;	 //FromTime == LicenseDate (inventorData)
				this.TheLast = this._device.TheLast.ToString(@"dd/MM/yyyy HH:mm:ss");           //ToTime == DateCreated

				this.TicksTimeSpan = this._device.TicksTimeSpan;
				this.PeriodFromFirstToLast = this._device.PeriodFromFirstToLast;
				this.PeriodFromStartInventorToEndInventor = this._device.PeriodFromStartInventorToEndInventor;
				//this.PeriodFromStartInventorToTheFirst = this._device.PeriodFromStartInventorToTheFirst;

				this.SumPeriod = this._device.SumPeriod;
				
				this.PeriodAddtionTime = this._device.PeriodAddtionTime;	 //  PeriodFromToEdit  == Description

				//string[] workers = this._device.Name.Split('|');
				//if (workers.Length > 0)
				//{
				//	this.WorkerID = workers[0];
				//}
				//if (workers.Length > 1)
				//{
				//	this.WorkerName = workers[1];
				//}
				//this.QuantityEdit = this._device.QuantityEdit;
				//this.Total = this._device.Total;
				//this.QuantityPerHour = this._device.QuantityPerHour;
				//this.FromTime = this._device.LicenseDate;	 //FromTime == LicenseDate (inventorData)
				//this.ToTime = this._device.DateCreated;			//ToTime == DateCreated
				//this.TicksTimeSpan = this._device.TicksTimeSpan;
				//this.PeriodFromToOriginal = this._device.PeriodFromToOriginal;
				//this.PeriodFromToEdit = this._device.Description;	 //  PeriodFromToEdit  == Description
			}
		}

		public string DeviceCode
		{
			get { return _deviceCode; }
			set { _deviceCode = value;
			this.RaisePropertyChanged(() => this.DeviceCode);
			}
		}

		public string Name		  //WorkerID| WorkerName
		{
			get { return _name; }
			set { _name = value;
			this.RaisePropertyChanged(() => this.Name);
			}
		}

		public string Description			 // PeriodFromToEdit
		{
			get { return _description; }
			set { _description = value;
			this.RaisePropertyChanged(() => this.Description);
			}
		}

		public DateTime DateCreated			  //ToTime
		{
			get { return _dateCreated; }
			set { _dateCreated = value;
			this.RaisePropertyChanged(() => this.DateCreated);
			}
		}

		public DateTime LicenseDate			//FromTime
		{
			get { return _licenseDate; }
			set { _licenseDate = value;
			this.RaisePropertyChanged(() => this.LicenseDate);
			}
		}

		public string WorkerID
		{
			get { return _workerID; }
			set { _workerID = value;
			this.RaisePropertyChanged(() => this.WorkerID);
			}
		}

		public string WorkerName
		{
			get { return _workerName; }
			set { 

				if (_workerName != value)
				{
				
					this._eventAggregator.GetEvent<DeviceWorkerNameEditEvent>().Publish(
									  new DeviceWorkerNameEditEventPayload() { OldWorkerName = _workerName, NewWorkerName = value, DeviceID = _deviceCode });
					_workerName = value;
				}
				this.RaisePropertyChanged(() => this.WorkerName);
			}
		}

		public double QuantityEdit
		{
			get { return _quantityEdit; }
			set { _quantityEdit = value;
			this.RaisePropertyChanged(() => this.QuantityEdit);
			}
		}

		public long Total
		{
			get { return _total; }
			set { _total = value;
			this.RaisePropertyChanged(() => this.Total);
			}
		}

		public string QuantityEditString
		{
			get { return _quantityEditString; }
			set
			{
				_quantityEditString = value;
				this.RaisePropertyChanged(() => this.QuantityEditString);
			}
		}

		public string TotalString
		{
			get { return _totalString; }
			set
			{
				_totalString = value;
				this.RaisePropertyChanged(() => this.TotalString);
			}
		}

		public int QuantityPerHourFromFirstToLast
		{
			get { return _quantityPerHourFromFirstToLast; }
			set { _quantityPerHourFromFirstToLast = value;
			this.RaisePropertyChanged(() => this.QuantityPerHourFromFirstToLast);
			}
		}

		private string _quantityPerHourFromFirstToLastString;
		public string QuantityPerHourFromFirstToLastString
		{
			get { return _quantityPerHourFromFirstToLastString; }
			set
			{
				_quantityPerHourFromFirstToLastString = value;
				this.RaisePropertyChanged(() => this.QuantityPerHourFromFirstToLastString);
			}
		}


		//public int QuantityPerHourFromStartInventorToTheLast
		//{
		//	get { return _quantityPerHourFromStartInventorToTheLast; }
		//	set
		//	{
		//		_quantityPerHourFromStartInventorToTheLast = value;
		//	this.RaisePropertyChanged(() => this.QuantityPerHourFromStartInventorToTheLast);
		//	}
		//}


		public int QuantityPerHourTotal
		{
			get { return _quantityPerHourTotal; }
			set
			{
				_quantityPerHourTotal = value;
				this.RaisePropertyChanged(() => this.QuantityPerHourTotal);
			}
		}


		public int QuantityPerHourFromStartInventorToEndInventor
		{
			get { return _quantityPerHourFromStartInventorToEndInventor; }
			set
			{
				_quantityPerHourFromStartInventorToEndInventor = value;
				this.RaisePropertyChanged(() => this.QuantityPerHourFromStartInventorToEndInventor);
			}
		}

		private string _quantityPerHourFromStartInventorToEndInventorString;
		public string QuantityPerHourFromStartInventorToEndInventorString
		{
			get { return _quantityPerHourFromStartInventorToEndInventorString; }
			set
			{
				_quantityPerHourFromStartInventorToEndInventorString = value;
				this.RaisePropertyChanged(() => this.QuantityPerHourFromStartInventorToEndInventorString);
			}
		}


		public int TotalPerHourFromFirstToLast
		{
			get { return _totalPerHourFromFirstToLast; }
			set
			{
				_totalPerHourFromFirstToLast = value;
				this.RaisePropertyChanged(() => this.TotalPerHourFromFirstToLast);
			}
		}

		private string _totalPerHourFromFirstToLastString;
		public string TotalPerHourFromFirstToLastString
		{
			get { return _totalPerHourFromFirstToLastString; }
			set
			{
				_totalPerHourFromFirstToLastString = value;
				this.RaisePropertyChanged(() => this.TotalPerHourFromFirstToLastString);
			}
		}


		public int TotalPerHourTotal
		{
			get { return _totalPerHourTotal; }
			set
			{
				_totalPerHourTotal = value;
				this.RaisePropertyChanged(() => this.TotalPerHourTotal);
			}
		}


		public int TotalPerHourFromStartInventorToEndInventor
		{
			get { return _totalPerHourFromStartInventorToEndInventor; }
			set
			{
				_totalPerHourFromStartInventorToEndInventor = value;
				this.RaisePropertyChanged(() => this.TotalPerHourFromStartInventorToEndInventor);
			}
		}

		private string _totalPerHourFromStartInventorToEndInventorString;
		public string TotalPerHourFromStartInventorToEndInventorString
		{
			get { return _totalPerHourFromStartInventorToEndInventorString; }
			set
			{
				_totalPerHourFromStartInventorToEndInventorString = value;
				this.RaisePropertyChanged(() => this.TotalPerHourFromStartInventorToEndInventorString);
			}
		}

		public String TheFirst
		{
			get { return _theFirst; }
			set { _theFirst = value;
			this.RaisePropertyChanged(() => this.TheFirst);

			}
		}


		public String StartInventorDateTime
		{
			get { return _startInventorDateTime; }
			set
			{
			_startInventorDateTime = value;
			this.RaisePropertyChanged(() => this.StartInventorDateTime);

			}
		}


		public String EndInventorDateTime
		{
			get { return _endInventorDateTime; }
			set
			{
				_endInventorDateTime = value;
				this.RaisePropertyChanged(() => this.EndInventorDateTime);

			}
		}

		public String TheLast
		{
			get { return _theLast; }
			set { _theLast = value;
			this.RaisePropertyChanged(() => this.TheLast);
			}
		}

		public long TicksTimeSpan
		{
			get { return _ticksTimeSpan; }
			set { _ticksTimeSpan = value;
			this.RaisePropertyChanged(() => this.TicksTimeSpan);
			}
		}

		public string PeriodFromFirstToLast
		{
			get { return _periodFromFirstToLast; }
			set { _periodFromFirstToLast = value;
			this.RaisePropertyChanged(() => this.PeriodFromFirstToLast);
			}
		}


		//public string PeriodFromStartInventorToTheFirst
		//{
		//	get { return _periodFromStartInventorToTheFirst; }
		//	set
		//	{
		//		_periodFromStartInventorToTheFirst = value;
		//		this.RaisePropertyChanged(() => this.PeriodFromStartInventorToTheFirst);
		//	}
		//}

		
		public string PeriodFromStartInventorToEndInventor
		{
			get { return _periodFromStartInventorToEndInventor; }
			set
			{
				_periodFromStartInventorToEndInventor = value;
				this.RaisePropertyChanged(() => this.PeriodFromStartInventorToEndInventor);
			}
		}


		public string SumPeriod
		{
			get { return _sumPeriod; }
			set
			{
				_sumPeriod = value;
				this.RaisePropertyChanged(() => this.SumPeriod);
			}
		}

		public string PeriodAddtionTime
		{
			get { return _periodAddtionTime; }
			set { _periodAddtionTime = value;
			this.RaisePropertyChanged(() => this.PeriodAddtionTime);
			}
		}

		public void DeviceSet(Device device)
		{
			this._device = device;
			this.Name = this._device.Name;
			this.WorkerID= this._device.WorkerID;
			this._workerName= this._device.WorkerName;
			this.PeriodAddtionTime = this._device.PeriodAddtionTime;		//Todo
			this.SumPeriod = this._device.SumPeriod;
			
			this.QuantityPerHourTotal = this._device.QuantityPerHourTotal;
			this.QuantityPerHourFromStartInventorToEndInventor = this._device.QuantityPerHourFromStartInventorToEndInventor;
			this.QuantityPerHourFromStartInventorToEndInventorString = this._device.QuantityPerHourFromStartInventorToEndInventorString;
			this.QuantityPerHourFromFirstToLast = this._device.QuantityPerHourFromFirstToLast;
			this.QuantityPerHourFromFirstToLastString = this._device.QuantityPerHourFromFirstToLastString;
		
			this.TotalPerHourTotal = this._device.TotalPerHourTotal;
			this.TotalPerHourFromStartInventorToEndInventor = this._device.TotalPerHourFromStartInventorToEndInventor;
			this.TotalPerHourFromStartInventorToEndInventorString = this._device.TotalPerHourFromStartInventorToEndInventorString;
			this.TotalPerHourFromFirstToLast = this._device.TotalPerHourFromFirstToLast;
			this.TotalPerHourFromFirstToLastString = this._device.TotalPerHourFromFirstToLastString;


			this.RaisePropertyChanged(() => this.DeviceCode);
			this.RaisePropertyChanged(() => this.Name);
			this.RaisePropertyChanged(() => this.Description);
			this.RaisePropertyChanged(() => this.DateCreated);
			this.RaisePropertyChanged(() => this.LicenseDate);
			this.RaisePropertyChanged(() => this.WorkerID);
			this.RaisePropertyChanged(() => this.WorkerName);
			this.RaisePropertyChanged(() => this.QuantityEdit);
			this.RaisePropertyChanged(() => this.Total);
			this.RaisePropertyChanged(() => this.QuantityPerHourFromStartInventorToEndInventor);
			this.RaisePropertyChanged(() => this.QuantityPerHourFromFirstToLast);
			this.RaisePropertyChanged(() => this.QuantityPerHourTotal);
			this.RaisePropertyChanged(() => this.TotalPerHourFromStartInventorToEndInventor);
			this.RaisePropertyChanged(() => this.TotalPerHourFromFirstToLast);
			this.RaisePropertyChanged(() => this.TotalPerHourTotal);
			this.RaisePropertyChanged(() => this.TheFirst);
			this.RaisePropertyChanged(() => this.StartInventorDateTime);
			this.RaisePropertyChanged(() => this.EndInventorDateTime);
			this.RaisePropertyChanged(() => this.TheLast);
			this.RaisePropertyChanged(() => this.TicksTimeSpan);
			this.RaisePropertyChanged(() => this.PeriodFromFirstToLast);
		//	this.RaisePropertyChanged(() => this.PeriodFromStartInventorToTheFirst);
			this.RaisePropertyChanged(() => this.PeriodFromStartInventorToEndInventor);
			this.RaisePropertyChanged(() => this.SumPeriod);
			this.RaisePropertyChanged(() => this.PeriodAddtionTime);
			//this.RaisePropertyChanged(() => this.QuantityPerHourFromStartInventorToTheLast);


			//this.RaisePropertyChanged(() => this.TypeCode);
		}


		//public string BalanceQuantityERP
		//{
		//	//get { return this._product.BalanceQuantityERP == null ? String.Empty : UtilsConvert.HebrewDouble(this._product.BalanceQuantityERP); }
		//	get { return UtilsConvert.HebrewDouble(this._product.BalanceQuantityERP); }
		//}

	
    }
}