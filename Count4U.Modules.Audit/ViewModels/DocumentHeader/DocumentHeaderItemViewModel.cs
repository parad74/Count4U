using System;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model;
using Codeplex.Reactive;
using System.Collections.Generic;

namespace Count4U.Modules.Audit.ViewModels
{
    public class DocumentHeaderItemViewModel : NotificationObject
    {
        private DocumentHeader _documentHeader;
        private bool _isApprove;
        private string _documentCode;
        private DateTime _createDate;
        private int _pdaiId;
		private string _deviceCode;
        private string _workerId;

        private string _name;
		private double _quantityEdit;
		private long _total;
		private String _fromTime;
		private String _toTime;
		private long _ticksTimeSpan;
		private string _periodFromTo;

		//private string _description;
        private string _statusDocHeaderBit;
        private string _statusInventProductBit;
        private string _statusApproveBit;

        private string _statusDocHeaderBitTooltip;
        private string _statusInventProductBitTooltip;
        private string _statusApproveBitTooltip;

        private string _num;

        private string _iturName;
        private string _iturCode;
        private Itur _itur;

        private ReactiveProperty<string> _totalItems;        

        public DocumentHeaderItemViewModel(DocumentHeader documentHeader, Itur itur)
        {            
            this._documentHeader = documentHeader;
            this._itur = itur;
            Load();
        }

		public DocumentHeaderItemViewModel(DocumentHeader documentHeader, string deviceCode, Itur itur)
		{
			this._documentHeader = documentHeader;
			this._itur = itur;
			LoadWithCountTime(deviceCode);
		}

        public bool IsApprove
        {
            get { return this._isApprove; }
            set
            {
                this._isApprove = value;
                this.DocumentHeader.Approve = this._isApprove;
                this.RaisePropertyChanged(() => this.IsApprove);
            }
        }

		public string DocumentCode
		{
			get
			{
				return this._documentCode;
			}
			set
			{
				this._documentCode = value;
				this.RaisePropertyChanged(() => this.DocumentCode);
			}
		}

        public string CreateDate
        {
            get
            {
                return UtilsConvert.DateToStringLong(this._createDate);
            }
        }

        public int PdaiId
        {
            get { return this._pdaiId; }
            set
            {
                this._pdaiId = value;
                this.RaisePropertyChanged(() => this.PdaiId);
            }
        }


		public string DeviceCode
        {
			get { return this._deviceCode; }
            set
            {
				this._deviceCode = value;
				this.RaisePropertyChanged(() => this.DeviceCode);
            }
        }

        public string WorkerId
        {
            get { return this._workerId; }
            set
            {
                this._workerId = value;
                this.RaisePropertyChanged(() => this.WorkerId);
            }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.RaisePropertyChanged(() => this.Name);
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


		public String FromTime
		{
			get { return _fromTime; }
			set { _fromTime = value;
			this.RaisePropertyChanged(() => this.FromTime);
			}
		}


		public String ToTime
		{
			get { return _toTime; }
			set { _toTime = value;
			this.RaisePropertyChanged(() => this.ToTime);
			}
		}


		public long TicksTimeSpan
		{
			get { return _ticksTimeSpan; }
			set { _ticksTimeSpan = value;
			this.RaisePropertyChanged(() => this.TicksTimeSpan);
			}
		}

		public string PeriodFromTo
		{
			get { return _periodFromTo; }
			set { _periodFromTo = value;
			this.RaisePropertyChanged(() => this.PeriodFromTo);
			}
		}

        public DocumentHeader DocumentHeader
        {
            get { return this._documentHeader; }
        }

        public string StatusDocHeaderBit
        {
            get { return this._statusDocHeaderBit; }
            set
            {
                this._statusDocHeaderBit = value;
                RaisePropertyChanged(() => this.StatusDocHeaderBit);
            }
        }

        public string StatusInventProductBit
        {
            get { return this._statusInventProductBit; }
            set
            {
                this._statusInventProductBit = value;
                RaisePropertyChanged(() => this.StatusInventProductBit);
            }
        }

        public string StatusApproveBit
        {
            get { return this._statusApproveBit; }
            set
            {
                this._statusApproveBit = value;
                RaisePropertyChanged(() => this.StatusApproveBit);
            }
        }

        public string Num
        {
            get { return _num; }
            set
            {
                _num = value;
                RaisePropertyChanged(() => Num);
            }
        }

        public string StatusDocHeaderBitTooltip
        {
            get { return _statusDocHeaderBitTooltip; }
            set
            {
                _statusDocHeaderBitTooltip = value;
                RaisePropertyChanged(() => StatusDocHeaderBitTooltip);
            }
        }

		//public string Description
		//{
		//	get { return _description; }
		//	set
		//	{
		//		_description = value;
		//		RaisePropertyChanged(() => Description);
		//	}
		//}
		

        public string StatusInventProductBitTooltip
        {
            get { return _statusInventProductBitTooltip; }
            set
            {
                _statusInventProductBitTooltip = value;
                RaisePropertyChanged(() => StatusInventProductBitTooltip);
            }
        }

        public string StatusApproveBitTooltip
        {
            get { return _statusApproveBitTooltip; }
            set
            {
                _statusApproveBitTooltip = value;
                RaisePropertyChanged(() => StatusApproveBitTooltip);
            }
        }

        public string IturName
        {
            get { return _iturName; }
            set
            {
                _iturName = value;
                RaisePropertyChanged(() => IturName);
            }
        }

        public string IturCode
        {
            get { return _iturCode; }
            set
            {
                _iturCode = value;
                RaisePropertyChanged(() => IturCode);
            }
        }

        public ReactiveProperty<string> TotalItems
        {
            get { return _totalItems; }
            set
            {
                _totalItems = value;
                RaisePropertyChanged(() => TotalItems);
            }
        }

        public void DocumentHeaderSet(DocumentHeader documentHeader, Itur itur)
        {
            this._itur = itur;
            this._documentHeader = documentHeader;
            Load();
        }

		public void DocumentHeaderSet(DocumentHeader documentHeader, string deviceCode, Itur itur)
        {
            this._itur = itur;
            this._documentHeader = documentHeader;
			LoadWithCountTime(deviceCode);
        }

		private void LoadWithCountTime(string deviceCode)
		{
			Load();
			this.DeviceCode = deviceCode;
			if (this._documentHeader != null)
			{
				this.QuantityEdit = this._documentHeader.QuantityEdit;
				this.Total = this._documentHeader.Total;
				this.FromTime = this._documentHeader.FromTime.ToString(@"dd/MM/yyyy HH:mm:ss");
                this.ToTime = this._documentHeader.ToTime.ToString(@"dd/MM/yyyy HH:mm:ss");
                this.TicksTimeSpan = this._documentHeader.TicksTimeSpan;
				this.PeriodFromTo = this._documentHeader.PeriodFromTo;
			}
		}
		

        private void Load()
        {
			if (this._documentHeader != null)
			{
				this.DocumentCode = this._documentHeader.DocumentCode;
				this.IsApprove = this._documentHeader.Approve ?? false;
             
                this.PdaiId = 0;
				this.WorkerId = this._documentHeader.WorkerGUID;
				this.Name = this._documentHeader.Name;
				this.Num = this._documentHeader.DocNum.ToString();
				this._createDate = this._documentHeader.CreateDate;

				this.RaisePropertyChanged(() => this.CreateDate);

				this.StatusDocHeaderBit = BitStatus.ToString(this._documentHeader.StatusDocHeaderBit);
				this.StatusInventProductBit = BitStatus.ToString(this._documentHeader.StatusInventProductBit);
				this.StatusApproveBit = BitStatus.ToString(this._documentHeader.StatusApproveBit);

				this.StatusDocHeaderBitTooltip = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._documentHeader.StatusDocHeaderBit, DomainStatusEnum.Doc));
				this.StatusInventProductBitTooltip = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._documentHeader.StatusInventProductBit, DomainStatusEnum.PDA));
				this.StatusApproveBitTooltip = String.Join(Environment.NewLine, Bit2List.GetApproveList(this._documentHeader.StatusApproveBit));
			}

			if (this._itur != null)
			{
				string iturName = this._itur.Name;
				if (String.IsNullOrEmpty(iturName))
					iturName = String.Format(Localization.Resources.ViewModel_DocumentHeaderItem_Itur, this._itur.NumberPrefix, this._itur.NumberSufix);
				this.IturName = iturName;
             }
            this.IturCode = this._documentHeader.IturCode;

        }
    }
}
