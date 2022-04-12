using System;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturDashboardItemViewModel : NotificationObject
    {
        private Itur _itur;
        private readonly Location _location;
        private StatusIturGroup _statusGroup;
        private readonly int _totalItursWithSuchLocation;
        private readonly int _emptyItursWithSuchLocation;
        private readonly int _countedItursWithSuchLocation;
        private readonly int _disableItursWithSuchLocation;
        private readonly double _doneItursWithSuchLocation;
        private readonly int _totalItursWithSuchStatus;
		private readonly int _totalItursWithSuchTag;

        private bool _isDisabled;

        public IturDashboardItemViewModel(Itur itur, Location location,
			StatusIturGroup statusGroup, int totalItursWithSuchLocation, int totalItursWithSuchStatus, 
            int totalItursWithSuchTag, int emptyItursWithSuchLocation, int countedItursWithSuchLocation, 
            int disableItursWithSuchLocation, double doneItursWithSuchLocation)
        {
            this._totalItursWithSuchStatus = totalItursWithSuchStatus;
            this._totalItursWithSuchLocation = totalItursWithSuchLocation;               //TODO Marina 2022
            this._countedItursWithSuchLocation = countedItursWithSuchLocation;
            this._disableItursWithSuchLocation = disableItursWithSuchLocation;
            this._emptyItursWithSuchLocation = emptyItursWithSuchLocation;
            this._doneItursWithSuchLocation = doneItursWithSuchLocation;
            this._totalItursWithSuchTag = totalItursWithSuchTag;
            this._statusGroup = statusGroup;
            this._location = location;
            this._itur = itur;

            this._isDisabled = this._itur.Disabled ?? false;
        }

        public string Code
        {
            get { return this._itur.IturCode; }
        }

        public string LocationColor
        {
            get
            {
                return this._location == null || String.IsNullOrEmpty(this._location.BackgroundColor) ? Common.Constants.DefaultColors.EmptyLocationForIturColor() :
                this._location.BackgroundColor;
            }
        }

        public string StatusColor { get; set; }

        public string StatusCode
        {
            get { return this._statusGroup == null ? String.Empty : this._statusGroup.Code; }
        }

        public string StatusName
        {
            get { return this._statusGroup == null ? String.Empty : this._statusGroup.Name; }
        }

        public string Number
        {
            get { return this._itur.Number.ToString(); }
        }

        public string Name
        {
            get { return this._itur.Name; }
        }

        public Itur Itur
        {
            get { return this._itur; }
        }

        public string LocationName
        {
            get { return this._location == null ? this._itur.LocationCode : this._location.Name; }
        }

        public string LocationCode
        {
            // get { return this._location == null ? String.Empty : this._location.Code; }
            get { return this._location == null ? this._itur.LocationCode : this._location.Code; }
        }       

        public string NumberPreffix
        {
            get { return this._itur.NumberPrefix; }
        }

		public string Tag
		{
			get { return this._itur.Tag; }
		}

        public string NumberSuffix
        {
            get { return this._itur.NumberSufix; }
        }

        public IturItemGroupLocation IturItemGroupLocation
        {
            get { return new IturItemGroupLocation(this); }
        }

		public IturItemGroupTag IturItemGroupTag
		{
			get { return new IturItemGroupTag(this); }
		}

        public IturItemGroupStatus IturItemGroupStatus
        {
            get { return new IturItemGroupStatus(this); }
        }

        public IturItemGroupEmpty IturItemGroupEmpty
        {
            get { return new IturItemGroupEmpty(); }
        }

        public int TotalItursWithSuchLocation
        {
            get { return this._totalItursWithSuchLocation; }
        }

        public int EmptyItursWithSuchLocation
        {
            get { return this._emptyItursWithSuchLocation; }
        }

        
         public int DisabledItursWithSuchLocation
        {
            get { return this._disableItursWithSuchLocation; }
        }


        public double DoneItursWithSuchLocation
        {
            get { return this._doneItursWithSuchLocation; }
        }
        


        public int CountedItursWithSuchLocation
        {
            get { return this._countedItursWithSuchLocation; }
        }

        public int TotalItursWithSuchStatus
        {
            get { return this._totalItursWithSuchStatus; }
        }


		public int TotalItursWithSuchTag
        {
            get { return this._totalItursWithSuchTag; }
        }

        public bool IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                _isDisabled = value;

                RaisePropertyChanged(() => IsDisabled);
            }
        }

        public string ERP
        {
            get
            {
                if (ShowERP)
                {
                    int length = _itur.ERPIturCode.Length;
                    if (length > 5)
                    {
                        return _itur.ERPIturCode.Substring(length - 5, length - (length - 5));
                    }

                    return _itur.ERPIturCode;
                }

                return String.Empty;
            }
        }

        public bool ShowERP { get; set; }

        public void Update(Itur itur, StatusIturGroup statusGroup)
        {            
            this._itur = itur;
            this._statusGroup = statusGroup;

            this._isDisabled = this._itur.Disabled ?? false;

            RaisePropertyChanged(() => IsDisabled);
            RaisePropertyChanged(() => StatusColor);
            RaisePropertyChanged(() => StatusCode);
            RaisePropertyChanged(() => StatusName);
        }
    }
}