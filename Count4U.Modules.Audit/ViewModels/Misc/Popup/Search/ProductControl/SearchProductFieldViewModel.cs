using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.ProductControl
{
    public class SearchProductFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel, IDataErrorInfo
    {
        private readonly IRegionManager _regionManager;

        private string _makat;
        private string _barcode;
        private string _productName;
        private string _priceSale;
        private string _priceBuy;

        private SortViewModel _sortViewModel;

		public SearchProductFieldViewModel(
			IContextCBIRepository contextCbiRepository,
			IRegionManager regionManager)
			: base(contextCbiRepository)
		{
			_regionManager = regionManager;
			_makat = String.Empty;
			_barcode = String.Empty;
			_productName = String.Empty;
			_priceSale = String.Empty;
			_priceBuy = String.Empty;
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

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                RaisePropertyChanged(() => ProductName);
            }
        }

        public string PriceSale
        {
            get { return _priceSale; }
            set
            {
                _priceSale = value;
                RaisePropertyChanged(() => PriceSale);

                if (SearchCommand != null)
                {
                    SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string PriceBuy
        {
            get { return _priceBuy; }
            set
            {
                _priceBuy = value;
                RaisePropertyChanged(() => PriceBuy);

                if (SearchCommand != null)
                {
                    SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SearchCommand { get; set; }

        public bool CanSearch()
        {
            bool isOkPriceSale;
            bool isOkPriceBuy;

            if (String.IsNullOrWhiteSpace(_priceSale))
                isOkPriceSale = true;
            else
                isOkPriceSale = UtilsConvert.IsOkAsDouble(_priceSale);

            if (String.IsNullOrWhiteSpace(_priceBuy))
                isOkPriceBuy = true;
            else
                isOkPriceBuy = UtilsConvert.IsOkAsDouble(_priceBuy);

            return isOkPriceSale && isOkPriceBuy;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.ProductSearch;
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "PriceBuy":
                        if (!UtilsConvert.IsOkAsDouble(_priceBuy))
                            return Localization.Resources.Bit_InvalidFormat_General;
                        break;
                    case "PriceSale":
                        if (!UtilsConvert.IsOkAsDouble(_priceSale))
                            return Localization.Resources.Bit_InvalidFormat_General;
                        break;                        
                }
                return String.Empty;
            }
        }

        public string Error { get; private set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildSort();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.Makat));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.Barcode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.Name));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.PriceSale));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.PriceBuy));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.SectionCode));
            sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Product>.GetPropertyInfo(r => r.SupplierCode));

            _sortViewModel.Add(sortProperties);
        }

        public IFilterData BuildFilterData()
        {
            ProductFilterData result = new ProductFilterData();

            result.Makat = _makat;
            result.Barcode = _barcode;
            result.ProductName = _productName;
            result.PriceSale = (String.IsNullOrWhiteSpace(_priceSale) ? (double?)null : Convert.ToDouble(_priceSale));
            result.PriceBuy = String.IsNullOrWhiteSpace(_priceBuy) ? (double?)null : Convert.ToDouble(_priceBuy);

            _sortViewModel.ApplyToFilterData(result);

            return result;
        }

		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			ProductFilterData result = new ProductFilterData();

			result.Makat = _makat;
			result.Barcode = _barcode;
			result.ProductName = _productName;
			result.PriceSale = (String.IsNullOrWhiteSpace(_priceSale) ? (double?)null : Convert.ToDouble(_priceSale));
			result.PriceBuy = String.IsNullOrWhiteSpace(_priceBuy) ? (double?)null : Convert.ToDouble(_priceBuy);

			_sortViewModel.ApplyToFilterData(result);

			return result;
		}

        public void Reset()
        {
            Makat = String.Empty;
            Barcode = String.Empty;
            ProductName = String.Empty;
            PriceBuy = null;
            PriceSale = null;

            _sortViewModel.Reset();
        }

        public void ApplyFilterData(IFilterData data)
        {
            ProductFilterData filterData = data as ProductFilterData;

            if (filterData == null) return;

            Barcode = filterData.Barcode;
            Makat = filterData.Makat;
            ProductName = filterData.ProductName;
            PriceSale = filterData.PriceSale == null ? String.Empty : filterData.PriceSale.ToString();
            PriceBuy = filterData.PriceBuy == null ? String.Empty : filterData.PriceBuy.ToString();

            _sortViewModel.InitFromFilterData(filterData);
        }
    }
}