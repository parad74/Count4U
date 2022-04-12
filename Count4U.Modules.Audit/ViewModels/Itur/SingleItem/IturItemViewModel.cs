using System;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturItemViewModel : NotificationObject
    {
        private Itur _itur;
        private Location _location;
        private StatusItur _statusItur;

        private bool _isDisabled;

        private string _code;
        private string _status;
        private string _number;
        private string _name;
		private string _description;
        private string _locationName;
        private string _publishedFormatted;
		private string _tag;
        private string _createDate;
        private string _modifyDate;
        private string _numberPreffix;
        private string _numberSuffix;
        private string _erpCode;

        private string _statusBit;
        private string _statusBitTooltip;
        private string _statusGroupBit;
        private string _statusGroupBitTooltip;
        private string _statusDocHeaderBit;
        private string _statusDocHeaderBitTooltip;

        public IturItemViewModel(Itur itur, Location location, StatusItur status)
        {
            this._statusItur = status;
            this._location = location;
            this._itur = itur;

            Set();
        }      
       
        public Itur Itur
        {
            get { return this._itur; }
        }              

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(()=>Code);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string LocationName
        {
            get { return _locationName; }
            set
            {
                _locationName = value;
                RaisePropertyChanged(() => LocationName);
            }
        }

        public string PublishedFormatted
        {
            get { return _publishedFormatted; }
            set
            {
                _publishedFormatted = value;
                RaisePropertyChanged(() => PublishedFormatted);
            }
        }


		public string Tag
        {
            get { return _tag; }
            set
            {
				_tag = value;
				RaisePropertyChanged(() => Tag);
            }
        }

        public string CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                RaisePropertyChanged(() => CreateDate);
            }
        }

        public string ModifyDate
        {
            get { return _modifyDate; }
            set
            {
                _modifyDate = value;
                RaisePropertyChanged(() => ModifyDate);
            }
        }

        public string NumberPreffix
        {
            get { return _numberPreffix; }
            set
            {
                _numberPreffix = value;
                RaisePropertyChanged(() => NumberPreffix);
            }
        }

        public string NumberSuffix
        {
            get { return _numberSuffix; }
            set
            {
                _numberSuffix = value;
                RaisePropertyChanged(() => NumberSuffix);
            }
        }

        public bool IsDisabled
        {
            get { return this._isDisabled; }
            set
            {
                this._isDisabled = value;
                RaisePropertyChanged(() => IsDisabled);
            }
        }

        public string StatusBit
        {
            get { return _statusBit; }
            set
            {
                _statusBit = value;
                RaisePropertyChanged(() => StatusBit);
            }
        }

        public string StatusBitTooltip
        {
            get { return _statusBitTooltip; }
            set
            {
                _statusBitTooltip = value;
                RaisePropertyChanged(() => StatusBitTooltip);
            }
        }

        public string StatusGroupBit
        {
            get { return _statusGroupBit; }
            set
            {
                _statusGroupBit = value;
                RaisePropertyChanged(() => StatusGroupBit);
            }
        }

        public string StatusGroupBitTooltip
        {
            get { return _statusGroupBitTooltip; }
            set
            {
                _statusGroupBitTooltip = value;
                RaisePropertyChanged(() => StatusGroupBitTooltip);
            }
        }

        public string StatusDocHeaderBit
        {
            get { return _statusDocHeaderBit; }
            set
            {
                _statusDocHeaderBit = value;
                RaisePropertyChanged(() => StatusDocHeaderBit);
            }
        }

		public string Description
        {
			get { return _description; }
            set
            {
				_description = value;
				RaisePropertyChanged(() => Description);
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

        public string ERPCode
        {
            get { return _erpCode; }
            set { _erpCode = value; }
        }

        private void Set()
        {
            this._code = this._itur.IturCode;
            this._status = this._statusItur == null ? String.Empty : this._statusItur.Name;
            this._number = this._itur.Number.ToString();
            this._name = this._itur.Name;
			this._tag = this._itur.Tag;
			this._description = this._itur.Description;
            this._locationName = this._location == null ? this._itur.LocationCode : this._location.Name;
            this._publishedFormatted = this._itur.Publishe == null ? string.Empty : this._itur.Publishe.Value ? "Yes" : "No";
            this._createDate = this._itur.CreateDate.ToShortDateString();
            this._modifyDate = this._itur.ModifyDate == null ? string.Empty : this._itur.ModifyDate.Value.ToShortDateString();
            this._numberPreffix = this._itur.NumberPrefix;
            this._numberSuffix = this._itur.NumberSufix;
            this._isDisabled = this._itur.Disabled ?? false;
            this._erpCode = this._itur.ERPIturCode;

            this._statusBit = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._itur.StatusIturBit, DomainStatusEnum.Itur)).Trim();
            this._statusBitTooltip = BitStatus.ToString(this._itur.StatusIturBit);
            this._statusGroupBit = String.Join(Environment.NewLine, Bit2List.GetStatusGroupList(this._itur.StatusIturGroupBit)).Trim();
            this._statusGroupBitTooltip = this._itur.StatusIturGroupBit.ToString();
            this._statusDocHeaderBit = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._itur.StatusDocHeaderBit, DomainStatusEnum.Doc)).Trim();
            this._statusGroupBitTooltip = BitStatus.ToString(this._itur.StatusDocHeaderBit);

        }        

        public void UpdateViewModel(Itur itur, Location location, StatusItur status)
        {
            this._statusItur = status;
            this._location = location;
            this._itur = itur;

            Set();

            this.RaisePropertyChanged(() => this.LocationName);
            this.RaisePropertyChanged(() => this.Status);
            this.RaisePropertyChanged(() => this.Number);
            this.RaisePropertyChanged(() => this.Name);
            this.RaisePropertyChanged(() => this.Itur);
            this.RaisePropertyChanged(() => this.PublishedFormatted);
			this.RaisePropertyChanged(() => this.Tag);
            this.RaisePropertyChanged(() => this.CreateDate);
            this.RaisePropertyChanged(() => this.ModifyDate);
            this.RaisePropertyChanged(() => this.NumberPreffix);
            this.RaisePropertyChanged(() => this.NumberSuffix);
            this.RaisePropertyChanged(() => this.IsDisabled);
            this.RaisePropertyChanged(()=>this.ERPCode);

            this.RaisePropertyChanged(() => this.StatusBit);
            this.RaisePropertyChanged(() => this.StatusBitTooltip);
            this.RaisePropertyChanged(() => this.StatusGroupBit);
            this.RaisePropertyChanged(() => this.StatusGroupBitTooltip);
            this.RaisePropertyChanged(() => this.StatusDocHeaderBit);
            this.RaisePropertyChanged(() => this.StatusDocHeaderBitTooltip);
        }       
    }
}