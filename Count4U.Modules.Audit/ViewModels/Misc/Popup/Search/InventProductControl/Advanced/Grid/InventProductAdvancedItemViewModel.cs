using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid
{
    public class InventProductAdvancedItemViewModel : NotificationObject
    {
        private readonly IturAnalyzes _analyze;

        private string _locationName;
        private string _iturCode;
        private string _iturERPCode;
        private bool _iturDisabled;
        private string _iturStatusGroupBit;
        private string _iturNumberPrefix;
        private string _iturNumberSuffix;
        private string _docNum;
        private string _ipNum;
        private string _inputTypeCode;
        private string _quantityDifference;
        private string _quanitytEdit;
        private string _valueBuyDifference;
        private string _valueBuyEdit;
        private string _pdaStatusInventProductBit;
        private string _typeMakat;
        private string _makat;
        private string _barcode;
        private string _priceBuy;
        private string _priceSale;
        private string _productName;
        private string _supplierName;
        private string _sectionName;

        public InventProductAdvancedItemViewModel(IturAnalyzes analyze)
        {
            _analyze = analyze;

            _locationName = analyze.LocationName;
            _iturCode = analyze.IturCode;
            _iturERPCode = analyze.ERPIturCode;
            _iturDisabled = (bool)analyze.Itur_Disabled;
            _iturStatusGroupBit = analyze.Itur_StatusIturGroupBit.ToString(); //todo probably
            _iturNumberPrefix = analyze.Itur_NumberPrefix;
            _iturNumberSuffix = analyze.Itur_NumberSufix;
            _docNum = analyze.DocNum.ToString();
            _ipNum = analyze.IPNum.ToString();
            _inputTypeCode = analyze.InputTypeCode;
            _quantityDifference = analyze.QuantityDifference.ToString();
            _quanitytEdit = analyze.QuantityEdit.ToString();
            _valueBuyDifference = analyze.ValueBuyDifference.ToString();
            _valueBuyEdit = analyze.ValueBuyEdit.ToString();
            _pdaStatusInventProductBit = analyze.PDA_StatusInventProductBit.ToString();
            _typeMakat = analyze.TypeMakat;
            _makat = analyze.Makat;
            _barcode = analyze.Barcode;
            _priceBuy = analyze.PriceBuy.ToString();
            _priceSale = analyze.PriceSale.ToString();
            _productName = analyze.ProductName;
            _supplierName = analyze.SupplierName;
            _sectionName = analyze.SectionName;
        }

        public string LocationName
        {
            get { return _locationName; }
            set
            {
                _locationName = value;
                RaisePropertyChanged(()=>LocationName);
            }
        }

        public string IturCode
        {
            get { return _iturCode; }
            set
            {
                _iturCode = value;
                RaisePropertyChanged(()=>IturCode);
            }
        }

        public string IturERPCode
        {
            get { return _iturERPCode; }
            set
            {
                _iturERPCode = value;
                RaisePropertyChanged(() => IturERPCode);
            }
        }

        public bool IturDisabled
        {
            get { return _iturDisabled; }
            set
            {
                _iturDisabled = value;
                RaisePropertyChanged(() => IturDisabled);
            }
        }

        public string IturStatusGroupBit
        {
            get { return _iturStatusGroupBit; }
            set
            {
                _iturStatusGroupBit = value;
                RaisePropertyChanged(() => IturStatusGroupBit);
            }
        }

        public string IturNumberPrefix
        {
            get { return _iturNumberPrefix; }
            set
            {
                _iturNumberPrefix = value;
                RaisePropertyChanged(() => IturNumberPrefix);
            }
        }

        public string IturNumberSuffix
        {
            get { return _iturNumberSuffix; }
            set
            {
                _iturNumberSuffix = value;
                RaisePropertyChanged(() => IturNumberSuffix);
            }
        }

        public string DocNum
        {
            get { return _docNum; }
            set
            {
                _docNum = value;
                RaisePropertyChanged(() => DocNum);
            }
        }

        public string IpNum
        {
            get { return _ipNum; }
            set
            {
                _ipNum = value;
                RaisePropertyChanged(() => IpNum);
            }
        }

        public string InputTypeCode
        {
            get { return _inputTypeCode; }
            set
            {
                _inputTypeCode = value;
                RaisePropertyChanged(() => InputTypeCode);
            }
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

        public string PdaStatusInventProductBit
        {
            get { return _pdaStatusInventProductBit; }
            set
            {
                _pdaStatusInventProductBit = value;
                RaisePropertyChanged(() => PdaStatusInventProductBit);
            }
        }

        public string TypeMakat
        {
            get { return _typeMakat; }
            set
            {
                _typeMakat = value;
                RaisePropertyChanged(() => TypeMakat);
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

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                RaisePropertyChanged(() => Barcode);
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

        public IturAnalyzes Analyze
        {
            get { return _analyze; }
        }
    }
}