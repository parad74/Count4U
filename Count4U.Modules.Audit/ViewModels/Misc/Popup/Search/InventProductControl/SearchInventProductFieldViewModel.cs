using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.GenerationReport;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SearchInventProductFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;

        private string _makat;
        private string _barcode;
		private string _code;
		private string _serialNumber;
		private string _propertyStr;
		private string _propertyNumber;
		private string _propertyNumberEquality;
		private bool _propertyNumberIsAbsolute;
        private string _name;
        //        private string _section;
        //        private string _priceBuy;
        //        private string _priceSell;
        //        private DateTime? _modifyDate;
        private bool _onlyWithError;
        private string _iturCode;
		private string _erpIturCode;
		private readonly ObservableCollection<string> _equalityItems;


        private DelegateCommand _searchCommand;

        private SortViewModel _sortViewModel;

        private string _focusedField;

        public SearchInventProductFieldViewModel(
			IContextCBIRepository contextCbiRepository,
			IRegionManager regionManager,
            IUserSettingsManager userSettingsManager)
			: base(contextCbiRepository)
		{
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;

			this._equalityItems = new ObservableCollection<string>() { 
               // Common.Constants.ComboValues.Equality.Equal, 
                Common.Constants.ComboValues.Equality.Greater,
                Common.Constants.ComboValues.Equality.Less,
                Common.Constants.ComboValues.Equality.GreaterOrEqual,
                Common.Constants.ComboValues.Equality.LessOrEqual,
            };


			this._makat = String.Empty;
			this._code = String.Empty;
			this._serialNumber = String.Empty;
			this._propertyNumberEquality = String.Empty;
			this._propertyNumberEquality = this._equalityItems.FirstOrDefault(r => r == Common.Constants.ComboValues.Equality.GreaterOrEqual);
			this._propertyNumberIsAbsolute = true;

			this._propertyStr = String.Empty;
			this._barcode = String.Empty;
			this._name = String.Empty;
			this._iturCode = String.Empty;
			this._erpIturCode = String.Empty;
		}

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            // return ViewDomainContextEnum.ItursIturDocPDA;
            return ViewDomainContextEnum.InventProductSearch;
        }

		public ObservableCollection<string> EqualityItems
		{
			get { return this._equalityItems; }
		}

        public string Makat
        {
            get { return _makat; }
            set
            {
                _makat = value;
                RaisePropertyChanged(() => Makat);
            }
        }

		public string Code
		{
			get { return _code; }
			set
			{
				_code = value;
				RaisePropertyChanged(() => Code);
			}
		}

		public string SerialNumber
        {
			get { return _serialNumber; }
            set
            {
				_serialNumber = value;
				RaisePropertyChanged(() => SerialNumber);
            }
        }

		public string PropertyStr
		{
			get { return _propertyStr; }
			set
			{
				_propertyStr = value;
				RaisePropertyChanged(() => PropertyStr);
			}
		}

		public string PropertyNumber
		{
			get { return _propertyNumber; }
			set
			{
				_propertyNumber = value;
				RaisePropertyChanged(() => PropertyNumber);
			}
		}
		

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                RaisePropertyChanged(() => Barcode);
            }
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

        public bool OnlyWithError
        {
            get { return _onlyWithError; }
            set
            {
                _onlyWithError = value;
                RaisePropertyChanged(() => OnlyWithError);
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

		public string ErpIturCode
        {
			get { return _erpIturCode; }
            set
            {
				if (value == null) _erpIturCode = "";
				else _erpIturCode = value;
				RaisePropertyChanged(() => ErpIturCode);
            }
        }
		

        public string FocusedField
        {
            get { return _focusedField; }
        }

	

		public string PropertyStrName
		{
			get { return _userSettingsManager.InventProductPropertyFilterSelectedItemGet(); }
		}


		public bool PropertyNumberIsAbsolute
		{
			get { return _propertyNumberIsAbsolute; }
			set
			{
				this._propertyNumberIsAbsolute = value;
				RaisePropertyChanged(() => PropertyNumberIsAbsolute);
			}
		}

		public string PropertyNumberName
		{
			get { return _userSettingsManager.InventProductPropertyFilterSelectedNumberItemGet(); }
		}
        //        public string Section
        //        {
        //            get { return _section; }
        //            set
        //            {
        //                _section = value;
        //                RaisePropertyChanged(() => Section);
        //            }
        //        }
        //
        //        public string PriceBuy
        //        {
        //            get { return _priceBuy; }
        //            set
        //            {
        //                _priceBuy = value;
        //                RaisePropertyChanged(() => PriceBuy);
        //            }
        //        }
        //
        //        public string PriceSell
        //        {
        //            get { return _priceSell; }
        //            set
        //            {
        //                _priceSell = value;
        //                RaisePropertyChanged(() => PriceSell);
        //            }
        //        }
        //
        //        public DateTime? ModifyDate
        //        {
        //            get { return _modifyDate; }
        //            set
        //            {
        //                _modifyDate = value;
        //                RaisePropertyChanged(() => ModifyDate);
        //            }
        //        } 

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildSort();

            _focusedField = _userSettingsManager.InventProductFilterFocusGet();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.Makat));
			//sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.Code));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.Barcode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.ImputTypeCodeFromPDA));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.ProductName));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.SectionCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.PriceBuy));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.PriceSale));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.IturCode));
			sortProperties.Add(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.ERPIturCode));

            _sortViewModel.Add(sortProperties);
        }


		public string PropertyNumberEquality
		{
			get { return _propertyNumberEquality; }
			set
			{
				this._propertyNumberEquality = value;
				RaisePropertyChanged(() => PropertyNumberEquality);
			}
		}

        public IFilterData BuildFilterData()
        {
			InventProductFilterData result = new InventProductFilterData();

            result.Barcode = _barcode;
			result.Code = _code;
            result.Makat = _makat;
			result.SerialNumber = _serialNumber;
			result.PropertyStrName = _userSettingsManager.InventProductPropertyFilterSelectedItemGet();
			result.PropertyNumberName = _userSettingsManager.InventProductPropertyFilterSelectedNumberItemGet();
			result.PropertyNumberEquality = this._propertyNumberEquality;
			result.PropertyNumberIsAbsolute = this._propertyNumberIsAbsolute;

			result.PropertyStr = _propertyStr;
			result.PropertyNumber = _propertyNumber;
            result.Name = _name;
            result.IturCode = _iturCode;
			//result.ErpIturCode = _erpIturCode;
            //            result.ModifyDate = _modifyDate;                        
            //            result.PriceBuy = _priceBuy;
            //            result.PriceSell = _priceSell;
            //            result.Section = _section;
            result.OnlyWithError = _onlyWithError;

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode)
		{
			InventProductFilterData result = new InventProductFilterData();

			//int num = 0;
			long id = 0;
			//bool yes = Int32.TryParse(selectedCode, out num);
			bool yes = Int64.TryParse(selectedCode, out id);
			//result.IPNum = num;
			result.ID = id;
			//result.Barcode = _barcode;
			//result.Makat = selectedCode;
			//result.Name = _name;
			//result.IturCode = _iturCode;
			//            result.ModifyDate = _modifyDate;                        
			//            result.PriceBuy = _priceBuy;
			//            result.PriceSell = _priceSell;
			//            result.Section = _section;
			//result.OnlyWithError = _onlyWithError;

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void ApplyFilterData(IFilterData data)
        {
            InventProductFilterData filterData = data as InventProductFilterData;

            if (filterData == null) return;

            Barcode = filterData.Barcode;
            Makat = filterData.Makat;
			Code = filterData.Code;
			SerialNumber = filterData.SerialNumber;
			PropertyStr = filterData.PropertyStr;
			PropertyNumber = filterData.PropertyNumber;
			PropertyNumberEquality = filterData.PropertyNumberEquality;
			PropertyNumberIsAbsolute = filterData.PropertyNumberIsAbsolute;
		//	PropertyStrName = filterData.PropertyStrName;
            Name = filterData.Name;
            IturCode = filterData.IturCode;
            OnlyWithError = filterData.OnlyWithError;
            //            PriceBuy = filterData.PriceBuy;
            //            PriceSell = filterData.PriceSell;
            //            ModifyDate = filterData.ModifyDate;

            _sortViewModel.InitFromFilterData(filterData);
        }

        public void Reset()
        {
            Barcode = String.Empty;
			Makat = String.Empty;
			Code = String.Empty;
			SerialNumber = String.Empty;
			PropertyNumberEquality = this._equalityItems.FirstOrDefault();
			PropertyNumberIsAbsolute = false;
            Name = String.Empty;
			PropertyStr = String.Empty;
			PropertyNumber = String.Empty;
            OnlyWithError = false;
            IturCode = String.Empty;
            //            PriceBuy = String.Empty;
            //            PriceSell = String.Empty;
            //            ModifyDate = null;

            _sortViewModel.Reset();
        }
    }
}