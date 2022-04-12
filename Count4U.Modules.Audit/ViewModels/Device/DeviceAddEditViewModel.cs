using System;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class DeviceAddEditViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
      //  private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly IDeviceRepository _deviceRepository;
		
        private readonly IIturRepository _iturRepository;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private bool _isNew;

        private string _workerName;
        private string _workerID;
		private string _periodFromToEdit;
		//private string _documentCode;
		//private string _iturCode;
		//private bool _isApprove;

		private Device _device;
		//string periodFromInventorDate ;
		string periodFromStartDate;
		string quentetyEditText;


		public DeviceAddEditViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
			IDeviceRepository deviceRepository,
            IIturRepository iturRepository)
            : base(contextCBIRepository)
        {
            this._iturRepository = iturRepository;
			this._deviceRepository = deviceRepository;
            this._eventAggregator = eventAggregator;
            this._okCommand = new DelegateCommand(OkCommandExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public string WorkerName
        {
            get { return this._workerName; }
            set
            {
                this._workerName = value;
                RaisePropertyChanged(() => WorkerName);
            }
        }

        public string WorkerID
        {
            get { return this._workerID; }
            set
            {
                this._workerID = value;
                this.RaisePropertyChanged(() => this.WorkerID);
            }
        }


		public string PeriodFromToEdit
        {
			get { return this._periodFromToEdit; }
            set
            {
				this._periodFromToEdit = value;
				this.RaisePropertyChanged(() => this.PeriodFromToEdit);
            }
        }

		//public string DocumentCode
		//{
		//	get { return this._documentCode; }
		//	set
		//	{
		//		this._documentCode = value;
		//		this.RaisePropertyChanged(() => this.DocumentCode);
		//	}
		//}

		//public string IturCode
		//{
		//	get { return this._iturCode; }
		//	set
		//	{
		//		this._iturCode = value;
		//		this.RaisePropertyChanged(() => this.IturCode);
		//	}
		//}

		//public bool IsApprove
		//{
		//	get { return this._isApprove; }
		//	set
		//	{
		//		this._isApprove = value;
		//		this.RaisePropertyChanged(() => this.IsApprove);
		//	}
		//}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.DeviceCode))
            {
                this._isNew = false;
				string deviceCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.DeviceCode).Value;
				//periodFromInventorDate = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.PeriodFromInventorDate).Value;
				periodFromStartDate = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.PeriodFromStartDate).Value;
				quentetyEditText = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.QuentetyEdit).Value;
				this._device = this._deviceRepository.GetDeviceByCode(deviceCode, base.GetDbPath);
            }
			//else
			//{
			//	this._isNew = true;
			//	if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IturCode))
			//		this._iturCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.IturCode).Value;

			//	this._device = new DocumentHeader();
			//	this._device.Code = Utils.CodeNewGenerate();
			//	this._device.DocumentCode = this._device.Code;
			//	this._device.IturCode = this._iturCode;
			//}

            if (this._device != null) //load values on form
            {
				string[] workers = _device.Name.Split('|');
				string workerId = "";			  //Name[0]
				string workerName = "";			//Name[1]
				if (workers.Length > 0)
				{
					workerId = workers[0];
				}
				if (workers.Length > 1)
				{
					workerName = workers[1];
				}
				this.WorkerID = workerId;				  //Name[0]
				this.WorkerName = workerName;			//Name[1]

				string periodAddTime = "00:00:00";
				try
				{
					TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
					bool ret = TimeSpan.TryParse(_device.Description, out addTime);
					if (ret == true)
					{
						ret = TimeSpan.TryParse(_device.Description + ":00", out addTime);
						if (ret == true)
						{
							periodAddTime = addTime.ToString(@"dd\:hh\:mm");			   //\:ss
						}
					}
				}
				catch { }
				this.PeriodFromToEdit = periodAddTime;
				

                //this._workerName = this._device.Name;
				//this._workerGUID = this._device.WorkerGUID;
				//this._documentCode = this._device.DocumentCode;
				//this._iturCode = this._device.IturCode;
				//this._isApprove = this._device.Approve ?? false;
            }
        }


        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
		{
			//save values from form
			this._device.Name = this._workerID + "|" + this._workerName;
			this._device.WorkerID = this._workerID;
			this._device.WorkerName = this._workerName;
			string sumTime = "00:00:00";
			string periodAddTime = "00:00:00";
			try
			{
				TimeSpan addTime = new TimeSpan(0, 0, 0, 0);
				{
					bool ret = TimeSpan.TryParse(this.PeriodFromToEdit, out addTime);
					if (ret == true)
					{
						ret = TimeSpan.TryParse(this.PeriodFromToEdit + ":00", out addTime);
						if (ret == true)
						{
							periodAddTime = addTime.ToString(@"dd\:hh\:mm");			   //\:ss
						}
					}
				}
			}
			catch { }
				this._device.PeriodAddtionTime = periodAddTime;

				//TimeSpan fromInventorToOriginalTime = new TimeSpan(0, 0, 0, 0);
				//{
				//	bool ret1 = TimeSpan.TryParse(periodFromInventorDate, out fromInventorToOriginalTime);
				//	if (ret1 == true)
				//	{
				//		ret1 = TimeSpan.TryParse(periodFromInventorDate + ":00", out fromInventorToOriginalTime);
				//		//if (ret1 == true)
				//		//{
				//		//	periodAddTime = fromToEditTime.ToString(@"dd\:hh\:mm");			   //\:ss
				//		//}
				//	}
				//}

				//TimeSpan fromToEditOriginalTime = new TimeSpan(0, 0, 0, 0);
				//{
				//	bool ret1 = TimeSpan.TryParse(periodFromStartDate, out fromToEditOriginalTime);
				//	if (ret1 == true)
				//	{
				//		ret1 = TimeSpan.TryParse(periodFromStartDate + ":00", out fromToEditOriginalTime);
				//		//if (ret1 == true)
				//		//{
				//		//	periodAddTime = fromToEditTime.ToString(@"dd\:hh\:mm");			   //\:ss
				//		//}
				//	}
				//}

			//	double quentetyEdit = 0.0;
			//	bool ret3 = Double.TryParse(quentetyEditText, out 	quentetyEdit);

			//	TimeSpan sum = new TimeSpan(0, 0, 0, 0);
			//	{
			//		sum = addTime + fromInventorToOriginalTime + fromToEditOriginalTime;
			//		sumTime = sum.ToString(@"dd\:hh\:mm");

			//		int minuts = (sum.Days * 24) * 60 + sum.Hours * 60 + sum.Minutes;
			//		if (minuts == 0) minuts = 1;
			//		double qEdit = quentetyEdit / ((double)minuts / 60.0);
			//		int qEdit1 = (int)(qEdit * 100);
			//		this._device.QuantityPerHourTotal = (double)(qEdit1) / 100.00;
			//	}

			

			//this._device.SumPeriod = sumTime;
		

			//this._device.DocumentCode = this._documentCode;
			//this._device.ModifyDate = DateTime.Now;
			//this._device.Approve = this._isApprove;

			if (this._isNew)
			{
				//if (!String.IsNullOrEmpty(this._iturCode))
				//{
				// this._device.CreateDate = DateTime.Now;
				// Itur itur = this._iturRepository.GetIturByCode(this._iturCode, base.GetDbPath);
				//	this._deviceRepository.Insert(this._device, base.GetDbPath);

				//	this._eventAggregator.GetEvent<DeviceAddedEditedEvent>().Publish(
				//			new DeviceAddedEditedEventPayload() { Device = this._device, IsNew = true });
				//}
			}
			else
			{
				this._deviceRepository.Update(this._device, base.GetDbPath);
				this._eventAggregator.GetEvent<DeviceAddedEditedEvent>().Publish(
									  new DeviceAddedEditedEventPayload() { Device = this._device, IsNew = false });
			}



			this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
		}
    }
}