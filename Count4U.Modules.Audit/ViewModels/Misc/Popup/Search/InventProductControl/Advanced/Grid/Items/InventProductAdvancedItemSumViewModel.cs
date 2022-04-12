using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid
{
    public class InventProductAdvancedItemSumViewModel : NotificationObject
    {
        private readonly IturAnalyzes _analyze;

        private string _quantityDifference;
        private string _quanitytEdit;
        private string _valueBuyDifference;
        private string _valueBuyEdit;
        private string _makat;
        private string _quantityOriginalERP;
        private string _valueOriginalERP;
        private string _quantityDifferenceOriginalERP;
        private string _valueDifferenceOriginalERP;
        private string _priceBuy;
        private string _priceSale;
        private string _productName;
        private string _supplierName;
        private string _supplierCode;
        private string _sectionName;
        private string _sectionCode;
		private string _familyCode;

        public InventProductAdvancedItemSumViewModel(IturAnalyzes analyze)
        {
            _analyze = analyze;

            _quantityDifference = analyze.QuantityDifference.ToString();
            _quanitytEdit = UtilsConvert.HebrewDouble(analyze.QuantityEdit);
            _valueBuyDifference = UtilsConvert.HebrewDouble(analyze.ValueBuyDifference);
            _valueBuyEdit = UtilsConvert.HebrewDouble(analyze.ValueBuyEdit);
            _makat = analyze.Makat;
            _quantityOriginalERP = UtilsConvert.HebrewDouble(analyze.QuantityOriginalERP);
            _valueOriginalERP = UtilsConvert.HebrewDouble(analyze.ValueOriginalERP);
            _quantityDifferenceOriginalERP = UtilsConvert.HebrewDouble(analyze.QuantityDifferenceOriginalERP);
            _valueDifferenceOriginalERP = UtilsConvert.HebrewDouble(analyze.ValueDifferenceOriginalERP);
            _priceBuy = UtilsConvert.HebrewDouble(analyze.PriceBuy);
            _priceSale = UtilsConvert.HebrewDouble(analyze.PriceSale);
            _productName = analyze.ProductName;
            _supplierName = analyze.SupplierName;
            _sectionName = analyze.SectionName;
			_familyCode = analyze.FamilyCode;
            _supplierCode = analyze.SupplierCode;
            _sectionCode = analyze.SectionCode;
        }

        public IturAnalyzes Analyze
        {
            get { return _analyze; }
        }

        public string QuantityDifference
        {
            get { return _quantityDifference; }
            set
            {
                _quantityDifference = value;
                RaisePropertyChanged(() => QuantityDifference);
            }
        }

        public string QuanitytEdit
        {
            get { return _quanitytEdit; }
            set
            {
                _quanitytEdit = value;
                RaisePropertyChanged(() => QuanitytEdit);
            }
        }

        public string ValueBuyDifference
        {
            get { return _valueBuyDifference; }
            set
            {
                _valueBuyDifference = value;
                RaisePropertyChanged(() => ValueBuyDifference);
            }
        }

        public string ValueBuyEdit
        {
            get { return _valueBuyEdit; }
            set
            {
                _valueBuyEdit = value;
                RaisePropertyChanged(() => ValueBuyEdit);
            }
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

        public string QuantityOriginalERP
        {
            get { return _quantityOriginalERP; }
            set
            {
                _quantityOriginalERP = value;
                RaisePropertyChanged(() => QuantityOriginalERP);
            }
        }

        public string ValueOriginalERP
        {
            get { return _valueOriginalERP; }
            set
            {
                _valueOriginalERP = value;
                RaisePropertyChanged(() => ValueOriginalERP);
            }
        }

        public string QuantityDifferenceOriginalERP
        {
            get { return _quantityDifferenceOriginalERP; }
            set
            {
                _quantityDifferenceOriginalERP = value;
                RaisePropertyChanged(() => QuantityDifferenceOriginalERP);
            }
        }

        public string ValueDifferenceOriginalERP
        {
            get { return _valueDifferenceOriginalERP; }
            set
            {
                _valueDifferenceOriginalERP = value;
                RaisePropertyChanged(() => ValueDifferenceOriginalERP);
            }
        }

        public string PriceBuy
        {
            get { return _priceBuy; }
            set
            {
                _priceBuy = value;
                RaisePropertyChanged(() => PriceBuy);
            }
        }

        public string PriceSale
        {
            get { return _priceSale; }
            set
            {
                _priceSale = value;
                RaisePropertyChanged(() => PriceSale);
            }
        }

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                RaisePropertyChanged(() => ProductName);
            }
        }

        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                _supplierName = value;
                RaisePropertyChanged(() => SupplierName);
            }
        }

        public string SectionName
        {
            get { return _sectionName; }
            set
            {
                _sectionName = value;
                RaisePropertyChanged(() => SectionName);
            }
        }

		public string FamilyCode
        {
			get { return _familyCode; }
            set
            {
				_familyCode = value;
				RaisePropertyChanged(() => FamilyCode);
            }
        }


            public string SupplierCode
        {
            get { return _supplierCode; }
            set
            {
                _supplierCode = value;
                RaisePropertyChanged(() => SupplierCode);
            }
        }


            public string SectionCode
        {
            get { return _sectionCode; }
            set
            {
                _sectionCode = value;
                RaisePropertyChanged(() => SectionCode);
            }
        }


    }
}