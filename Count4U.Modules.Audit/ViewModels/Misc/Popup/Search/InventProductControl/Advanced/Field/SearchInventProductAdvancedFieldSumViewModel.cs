using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Section;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Model.Audit;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SearchInventProductAdvancedFieldSumViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly SectionListViewModelBuilder _sectionListViewModelBuilder;
        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IRegionManager _regionManager;

        private DelegateCommand _searchCommand;

        private readonly ObservableCollection<string> _equalityItems;

        private bool _isInventProductExpanded;
        private bool _isFilterByInventProduct;
        private string _inventProductQuantityDifference;
        private string _inventProductQuantityDifferenceEquality;
        private bool _inventProductQuantityDifferenceIsAbsolute;
        private string _inventProductQuantityEdit;
        private string _inventProductQuantityEditEquality;
        private bool _inventProductQuantityEditIsAbsolute;
        private string _inventProductValueBuyDifference;
        private string _inventProductValueBuyDifferenceEquality;
        private bool _inventProductValueBuyDifferenceIsAbsolute;
        private string _inventProductValueBuyEdit;
        private string _inventProductValueBuyEditEquality;
        private bool _inventProductValueBuyEditIsAbsolute;

        private bool _isProductExpanded;
        private bool _isFilterByProduct;
        private string _productMakat;

		private string _productQuantityOriginalERP;
		private string _productQuantityOriginalERPEquality;
		private bool _productQuantityOriginalERPIsAbsolute;

        private string _productQuantityDifferenceOriginalERP;
        private string _productQuantityDifferenceOriginalERPEquality;
        private bool _productQuantityDifferenceOriginalERPIsAbsolute;
        private string _productValueDifferenceOriginalERP;
        private string _productValueDifferenceOriginalERPEquality;
        private bool _productValueDifferenceOriginalERPIsAbsolute;
        private string _productPriceBuy;
        private string _productPriceBuyEquality;
        private bool _productPriceBuyIsAbsolute;
        private string _productPriceSale;
        private string _productPriceSaleEquality;
        private bool _productPriceSaleIsAbsolute;
        private string _productName;

        private bool _isSupplierExpanded;
        private bool _isFilterBySupplier;
        private string _supplierCode;
        private string _supplierName;

		private bool _isFamilyExpanded;
		private bool _isFilterByFamily;
		private string _familyCode;
		private string _familyName;

        private bool _isSectionExpanded;
        private bool _isFilterBySection;
        private readonly ObservableCollection<SectionItem2ViewModel> _sectionItems;

        private bool _isBuildingTable;
        private CancellationTokenSource _cts;

        private SortViewModel _sortViewModel;
        private Task _task;

		private bool _isCheckedSections;

		private string _findBySectionCode = String.Empty;
		private string _findBySectionTag = String.Empty;

		public SearchInventProductAdvancedFieldSumViewModel(
			IContextCBIRepository contextCbiRepository,
			SectionListViewModelBuilder sectionListViewModelBuilder,
			IIturAnalyzesRepository iturAnalyzesRepository,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository,
            ISectionRepository sectionRepository,
        IRegionManager regionManager)
			: base(contextCbiRepository)
		{
			_regionManager = regionManager;
			_iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			_iturAnalyzesRepository = iturAnalyzesRepository;
			_sectionListViewModelBuilder = sectionListViewModelBuilder;
            _sectionRepository = sectionRepository;
            _equalityItems = new ObservableCollection<string>() { 
                Common.Constants.ComboValues.Equality.Equal, 
                Common.Constants.ComboValues.Equality.Greater,
                Common.Constants.ComboValues.Equality.Less,
                Common.Constants.ComboValues.Equality.GreaterOrEqual,
                Common.Constants.ComboValues.Equality.LessOrEqual,
            };

			_sectionItems = new ObservableCollection<SectionItem2ViewModel>();

			_inventProductQuantityDifference = String.Empty;
			_inventProductQuantityDifferenceEquality = String.Empty;
			_inventProductQuantityEdit = String.Empty;
			_inventProductQuantityEditEquality = String.Empty;
			_inventProductValueBuyDifference = String.Empty;
			_inventProductValueBuyDifferenceEquality = String.Empty;
			_inventProductValueBuyEdit = String.Empty;
			_inventProductValueBuyEditEquality = String.Empty;
			_productMakat = String.Empty;
			_productQuantityOriginalERP = String.Empty;
			_productQuantityOriginalERPEquality = String.Empty;
			_productQuantityDifferenceOriginalERP = String.Empty;
			_productQuantityDifferenceOriginalERPEquality = String.Empty;
			_productValueDifferenceOriginalERP = String.Empty;
			_productValueDifferenceOriginalERPEquality = String.Empty;
			_productPriceBuy = String.Empty;
			_productPriceBuyEquality = String.Empty;
			_productPriceSale = String.Empty;
			_productPriceSaleEquality = String.Empty;
			_productName = String.Empty;
			_supplierCode = String.Empty;
			_supplierName = String.Empty;

			_inventProductQuantityDifferenceEquality = EqualityItems.FirstOrDefault();
			_inventProductQuantityEditEquality = EqualityItems.FirstOrDefault();
			_inventProductValueBuyDifferenceEquality = EqualityItems.FirstOrDefault();
			_inventProductValueBuyEditEquality = EqualityItems.FirstOrDefault();
			_productQuantityOriginalERPEquality = EqualityItems.FirstOrDefault();
			_productQuantityDifferenceOriginalERPEquality = EqualityItems.FirstOrDefault();
			_productValueDifferenceOriginalERPEquality = EqualityItems.FirstOrDefault();
			_productPriceBuyEquality = EqualityItems.FirstOrDefault();
			_productPriceSaleEquality = EqualityItems.FirstOrDefault();

			_productQuantityOriginalERPIsAbsolute = true;
			_productQuantityDifferenceOriginalERPIsAbsolute = true;
			_productValueDifferenceOriginalERPIsAbsolute = true;
			_productPriceBuyIsAbsolute = true;
			_productPriceSaleIsAbsolute = true;

			_isFilterByProduct = true;

			_isProductExpanded = true;

	
		}

        public ObservableCollection<string> EqualityItems
        {
            get { return _equalityItems; }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public bool IsInventProductExpanded
        {
            get { return _isInventProductExpanded; }
            set
            {
                _isInventProductExpanded = value;
                RaisePropertyChanged(() => IsInventProductExpanded);
            }
        }

        public bool IsFilterByInventProduct
        {
            get { return _isFilterByInventProduct; }
            set
            {
                _isFilterByInventProduct = value;
                RaisePropertyChanged(() => IsFilterByInventProduct);
            }
        }

		public string FindBySectionCode
		{
			get { return _findBySectionCode; }
			set
			{
				if (_findBySectionCode != value)
				{
					_findBySectionCode = value;
					RaisePropertyChanged(() => FindBySectionCode);

					BuildSection();
				}
			}
		}

		public string FindBySectionTag
		{
			get { return _findBySectionTag; }
			set
			{
				if (_findBySectionTag != value)
				{
					_findBySectionTag = value;
					RaisePropertyChanged(() => FindBySectionTag);

					BuildSection();
				}
			}
		}

        public string InventProductQuantityDifference
        {
            get { return _inventProductQuantityDifference; }
            set
            {
                _inventProductQuantityDifference = value;
                RaisePropertyChanged(() => InventProductQuantityDifference);
            }
        }

        public string InventProductQuantityDifferenceEquality
        {
            get { return _inventProductQuantityDifferenceEquality; }
            set
            {
                _inventProductQuantityDifferenceEquality = value;
                RaisePropertyChanged(() => InventProductQuantityDifferenceEquality);
            }
        }

        public string InventProductQuantityEdit
        {
            get { return _inventProductQuantityEdit; }
            set
            {
                _inventProductQuantityEdit = value;
                RaisePropertyChanged(() => InventProductQuantityEdit);
            }
        }

        public string InventProductQuantityEditEquality
        {
            get { return _inventProductQuantityEditEquality; }
            set
            {
                _inventProductQuantityEditEquality = value;
                RaisePropertyChanged(() => InventProductQuantityEditEquality);
            }
        }

        public string InventProductValueBuyDifference
        {
            get { return _inventProductValueBuyDifference; }
            set
            {
                _inventProductValueBuyDifference = value;
                RaisePropertyChanged(() => InventProductValueBuyDifference);
            }
        }

        public string InventProductValueBuyDifferenceEquality
        {
            get { return _inventProductValueBuyDifferenceEquality; }
            set
            {
                _inventProductValueBuyDifferenceEquality = value;
                RaisePropertyChanged(() => InventProductValueBuyDifferenceEquality);
            }
        }

        public string InventProductValueBuyEdit
        {
            get { return _inventProductValueBuyEdit; }
            set
            {
                _inventProductValueBuyEdit = value;
                RaisePropertyChanged(() => InventProductValueBuyEdit);
            }
        }

        public string InventProductValueBuyEditEquality
        {
            get { return _inventProductValueBuyEditEquality; }
            set
            {
                _inventProductValueBuyEditEquality = value;
                RaisePropertyChanged(() => InventProductValueBuyEditEquality);
            }
        }

        public bool IsProductExpanded
        {
            get { return _isProductExpanded; }
            set
            {
                _isProductExpanded = value;
                RaisePropertyChanged(() => IsProductExpanded);
            }
        }

        public bool IsFilterByProduct
        {
            get { return _isFilterByProduct; }
            set
            {
                _isFilterByProduct = value;
                RaisePropertyChanged(() => IsFilterByProduct);
            }
        }

        public string ProductMakat
        {
            get { return _productMakat; }
            set
            {
                _productMakat = value;
                RaisePropertyChanged(() => ProductMakat);
            }
        }

        public string ProductQuantityDifferenceOriginalERP
        {
            get { return _productQuantityDifferenceOriginalERP; }
            set
            {
                _productQuantityDifferenceOriginalERP = value;
                RaisePropertyChanged(() => ProductQuantityDifferenceOriginalERP);
            }
        }

		
        public string ProductQuantityDifferenceOriginalERPEquality
        {
            get { return _productQuantityDifferenceOriginalERPEquality; }
            set
            {
                _productQuantityDifferenceOriginalERPEquality = value;
                RaisePropertyChanged(() => ProductQuantityDifferenceOriginalERPEquality);
            }
        }

		public string ProductQuantityOriginalERP
		{
			get { return _productQuantityOriginalERP; }
			set
			{
				_productQuantityOriginalERP = value;
				RaisePropertyChanged(() => ProductQuantityOriginalERP);
			}
		}



		public string ProductQuantityOriginalERPEquality
        {
			get { return _productQuantityOriginalERPEquality; }
            set
            {
				_productQuantityOriginalERPEquality = value;
				RaisePropertyChanged(() => ProductQuantityOriginalERPEquality);
            }
        }

        public string ProductValueDifferenceOriginalERP
        {
            get { return _productValueDifferenceOriginalERP; }
            set
            {
                _productValueDifferenceOriginalERP = value;
                RaisePropertyChanged(() => ProductValueDifferenceOriginalERP);
            }
        }

        public string ProductValueDifferenceOriginalERPEquality
        {
            get { return _productValueDifferenceOriginalERPEquality; }
            set
            {
                _productValueDifferenceOriginalERPEquality = value;
                RaisePropertyChanged(() => ProductValueDifferenceOriginalERPEquality);
            }
        }

        public string ProductPriceBuy
        {
            get { return _productPriceBuy; }
            set
            {
                _productPriceBuy = value;
                RaisePropertyChanged(() => ProductPriceBuy);
            }
        }

        public string ProductPriceBuyEquality
        {
            get { return _productPriceBuyEquality; }
            set
            {
                _productPriceBuyEquality = value;
                RaisePropertyChanged(() => ProductPriceBuyEquality);
            }
        }

        public string ProductPriceSale
        {
            get { return _productPriceSale; }
            set
            {
                _productPriceSale = value;
                RaisePropertyChanged(() => ProductPriceSale);
            }
        }

        public string ProductPriceSaleEquality
        {
            get { return _productPriceSaleEquality; }
            set
            {
                _productPriceSaleEquality = value;
                RaisePropertyChanged(() => ProductPriceSaleEquality);
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

        public bool IsSupplierExpanded
        {
            get { return _isSupplierExpanded; }
            set
            {
                _isSupplierExpanded = value;
                RaisePropertyChanged(() => IsSupplierExpanded);
            }
        }

        public bool IsFilterBySupplier
        {
            get { return _isFilterBySupplier; }
            set
            {
                _isFilterBySupplier = value;
                RaisePropertyChanged(() => IsFilterBySupplier);
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

        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                _supplierName = value;
                RaisePropertyChanged(() => SupplierName);
            }
        }

		public bool IsFamilyExpanded
		{
			get { return this._isFamilyExpanded; }
			set
			{
				this._isFamilyExpanded = value;
				RaisePropertyChanged(() => IsFamilyExpanded);
			}
		}

		public bool IsFilterByFamily
		{
			get { return this._isFilterByFamily; }
			set
			{
				this._isFilterByFamily = value;
				RaisePropertyChanged(() => IsFilterByFamily);
			}
		}

		public string FamilyCode
		{
			get { return this._familyCode; }
			set
			{
				this._familyCode = value;
				RaisePropertyChanged(() => FamilyCode);
			}
		}

		public string FamilyName
		{
			get { return this._familyName; }
			set
			{
				this._familyName = value;
				RaisePropertyChanged(() => FamilyName);
			}
		}

        public bool IsSectionExpanded
        {
            get { return _isSectionExpanded; }
            set
            {
                _isSectionExpanded = value;
                RaisePropertyChanged(() => IsSectionExpanded);
            }
        }

		public bool IsCheckedSections
		{
			get { return _isCheckedSections; }
			set
			{
				_isCheckedSections = value;
				this.SectionItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedSections);

			}
		}

        public bool IsFilterBySection
        {
            get { return _isFilterBySection; }
            set
            {
                _isFilterBySection = value;
                RaisePropertyChanged(() => IsFilterBySection);
            }
        }

        public ObservableCollection<SectionItem2ViewModel> SectionItems
        {
            get { return _sectionItems; }
        }

        public bool IsBuildingTable
        {
            get { return _isBuildingTable; }
            set
            {
                _isBuildingTable = value;
                RaisePropertyChanged(() => IsBuildingTable);

                if (_searchCommand != null)
                {
                    _searchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool InventProductQuantityDifferenceIsAbsolute
        {
            get { return _inventProductQuantityDifferenceIsAbsolute; }
            set
            {
                _inventProductQuantityDifferenceIsAbsolute = value;
                RaisePropertyChanged(() => InventProductQuantityDifferenceIsAbsolute);
            }
        }

        public bool InventProductQuantityEditIsAbsolute
        {
            get { return _inventProductQuantityEditIsAbsolute; }
            set
            {
                _inventProductQuantityEditIsAbsolute = value;
                RaisePropertyChanged(() => InventProductQuantityEditIsAbsolute);
            }
        }

        public bool InventProductValueBuyDifferenceIsAbsolute
        {
            get { return _inventProductValueBuyDifferenceIsAbsolute; }
            set
            {
                _inventProductValueBuyDifferenceIsAbsolute = value;
                RaisePropertyChanged(() => InventProductValueBuyDifferenceIsAbsolute);
            }
        }

        public bool InventProductValueBuyEditIsAbsolute
        {
            get { return _inventProductValueBuyEditIsAbsolute; }
            set
            {
                _inventProductValueBuyEditIsAbsolute = value;
                RaisePropertyChanged(() => InventProductValueBuyEditIsAbsolute);
            }
        }



		public bool ProductQuantityOriginalERPIsAbsolute
        {
			get { return _productQuantityOriginalERPIsAbsolute; }
            set
            {
				_productQuantityOriginalERPIsAbsolute = value;
				RaisePropertyChanged(() => ProductQuantityOriginalERPIsAbsolute);
            }
        }

        public bool ProductQuantityDifferenceOriginalERPIsAbsolute
        {
            get { return _productQuantityDifferenceOriginalERPIsAbsolute; }
            set
            {
                _productQuantityDifferenceOriginalERPIsAbsolute = value;
                RaisePropertyChanged(() => ProductQuantityDifferenceOriginalERPIsAbsolute);
            }
        }

        public bool ProductValueDifferenceOriginalERPIsAbsolute
        {
            get { return _productValueDifferenceOriginalERPIsAbsolute; }
            set
            {
                _productValueDifferenceOriginalERPIsAbsolute = value;
                RaisePropertyChanged(() => ProductValueDifferenceOriginalERPIsAbsolute);
            }
        }

        public bool ProductPriceBuyIsAbsolute
        {
            get { return _productPriceBuyIsAbsolute; }
            set
            {
                _productPriceBuyIsAbsolute = value;
                RaisePropertyChanged(() => ProductPriceBuyIsAbsolute);
            }
        }

        public bool ProductPriceSaleIsAbsolute
        {
            get { return _productPriceSaleIsAbsolute; }
            set
            {
                _productPriceSaleIsAbsolute = value;
                RaisePropertyChanged(() => ProductPriceSaleIsAbsolute);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(BuildSection).LogTaskFactoryExceptions("OnNavigatedTo");

            BuildSort();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Cancel();
        }

        public void Cancel()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                if (_task != null)
                {
                    _task.Wait();
                    _task = null;
                }
                _cts = null;
            }
        }

        private void BuildSort()
        {
            _sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, _regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.Makat));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ProductName));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.PriceBuy));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.PriceSale));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityEdit));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityOriginalERP));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityDifferenceOriginalERP));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueBuyEdit));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueOriginalERP));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueDifferenceOriginalERP));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SupplierCode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SupplierName));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SectionCode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SectionName));

            //sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityDifference));

            //sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueBuyDifference));



            _sortViewModel.Add(sortProperties);
        }

        public IFilterData BuildFilterData()
        {
            InventProductSumFilterData result = new InventProductSumFilterData();

            _sortViewModel.ApplyToFilterData(result);

            result.IsInventProductExpanded = _isInventProductExpanded;
            result.IsFilterByInventProduct = _isFilterByInventProduct;

            result.InventProductQuantityDifference = _inventProductQuantityDifference;
            result.InventProductQuantityDifferenceEquality = _inventProductQuantityDifferenceEquality;
            result.InventProductQuantityDifferenceIsAbsolute = _inventProductQuantityDifferenceIsAbsolute;
            result.InventProductQuantityEdit = _inventProductQuantityEdit;
            result.InventProductQuantityEditEquality = _inventProductQuantityEditEquality;
            result.InventProductQuantityEditIsAbsolute = _inventProductQuantityEditIsAbsolute;
            result.InventProductValueBuyDifference = _inventProductValueBuyDifference;
            result.InventProductValueBuyDifferenceEquality = _inventProductValueBuyDifferenceEquality;
            result.InventProductValueBuyDifferenceIsAbsolute = _inventProductValueBuyDifferenceIsAbsolute;
            result.InventProductValueBuyEdit = _inventProductValueBuyEdit;
            result.InventProductValueBuyEditEquality = _inventProductValueBuyEditEquality;
            result.InventProductValueBuyEditIsAbsolute = _inventProductValueBuyEditIsAbsolute;

            result.IsProductExpanded = _isProductExpanded;
            result.IsFilterByProduct = _isFilterByProduct;
            result.ProductMakat = _productMakat;

			result.ProductQuantityOriginalERP = _productQuantityOriginalERP;
			result.ProductQuantityOriginalERPEquality = _productQuantityOriginalERPEquality;
			result.ProductQuantityOriginalERPIsAbsolute = _productQuantityOriginalERPIsAbsolute;

            result.ProductQuantityDifferenceOriginalERP = _productQuantityDifferenceOriginalERP;
            result.ProductQuantityDifferenceOriginalERPEquality = _productQuantityDifferenceOriginalERPEquality;
            result.ProductQuantityDifferenceOriginalERPIsAbsolute = _productQuantityDifferenceOriginalERPIsAbsolute;
            result.ProductValueDifferenceOriginalERP = _productValueDifferenceOriginalERP;
            result.ProductValueDifferenceOriginalERPEquality = _productValueDifferenceOriginalERPEquality;
            result.ProductValueDifferenceOriginalERPIsAbsolute = _productValueDifferenceOriginalERPIsAbsolute;
            result.ProductPriceBuy = _productPriceBuy;
            result.ProductPriceBuyEquality = _productPriceBuyEquality;
            result.ProductPriceBuyIsAbsolute = _productPriceBuyIsAbsolute;
            result.ProductPriceSale = _productPriceSale;
            result.ProductPriceSaleEquality = _productPriceSaleEquality;
            result.ProductPriceSaleIsAbsolute = _productPriceSaleIsAbsolute;
            result.ProductName = _productName;

            result.IsSupplierExpanded = _isSupplierExpanded;
            result.IsFilterBySupplier = _isFilterBySupplier;
            result.SupplierCode = _supplierCode;
            result.SupplierName = _supplierName;

			result.IsFamilyExpanded = this._isFamilyExpanded;
			result.IsFilterByFamily = this._isFilterByFamily;
			result.FamilyCode = this._familyCode;
			result.FamilyName = this._familyName;

            result.IsSectionExpanded = _isSectionExpanded;
            result.IsFilterBySection = _isFilterBySection;
            result.SectionItems = _sectionItems.Where(r => r.IsChecked).Select(r => r.Section.SectionCode).ToList();

            return result;
        }


		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			InventProductSumFilterData result = new InventProductSumFilterData();

			_sortViewModel.ApplyToFilterData(result);

			result.IsInventProductExpanded = _isInventProductExpanded;
			result.IsFilterByInventProduct = _isFilterByInventProduct;

			result.InventProductQuantityDifference = _inventProductQuantityDifference;
			result.InventProductQuantityDifferenceEquality = _inventProductQuantityDifferenceEquality;
			result.InventProductQuantityDifferenceIsAbsolute = _inventProductQuantityDifferenceIsAbsolute;
			result.InventProductQuantityEdit = _inventProductQuantityEdit;
			result.InventProductQuantityEditEquality = _inventProductQuantityEditEquality;
			result.InventProductQuantityEditIsAbsolute = _inventProductQuantityEditIsAbsolute;
			result.InventProductValueBuyDifference = _inventProductValueBuyDifference;
			result.InventProductValueBuyDifferenceEquality = _inventProductValueBuyDifferenceEquality;
			result.InventProductValueBuyDifferenceIsAbsolute = _inventProductValueBuyDifferenceIsAbsolute;
			result.InventProductValueBuyEdit = _inventProductValueBuyEdit;
			result.InventProductValueBuyEditEquality = _inventProductValueBuyEditEquality;
			result.InventProductValueBuyEditIsAbsolute = _inventProductValueBuyEditIsAbsolute;

			result.IsProductExpanded = _isProductExpanded;
			result.IsFilterByProduct = _isFilterByProduct;
			result.ProductMakat = _productMakat;

			result.ProductQuantityOriginalERP = _productQuantityOriginalERP;
			result.ProductQuantityOriginalERPEquality = _productQuantityOriginalERPEquality;
			result.ProductQuantityOriginalERPIsAbsolute = _productQuantityOriginalERPIsAbsolute;

			result.ProductQuantityDifferenceOriginalERP = _productQuantityDifferenceOriginalERP;
			result.ProductQuantityDifferenceOriginalERPEquality = _productQuantityDifferenceOriginalERPEquality;
			result.ProductQuantityDifferenceOriginalERPIsAbsolute = _productQuantityDifferenceOriginalERPIsAbsolute;
			result.ProductValueDifferenceOriginalERP = _productValueDifferenceOriginalERP;
			result.ProductValueDifferenceOriginalERPEquality = _productValueDifferenceOriginalERPEquality;
			result.ProductValueDifferenceOriginalERPIsAbsolute = _productValueDifferenceOriginalERPIsAbsolute;
			result.ProductPriceBuy = _productPriceBuy;
			result.ProductPriceBuyEquality = _productPriceBuyEquality;
			result.ProductPriceBuyIsAbsolute = _productPriceBuyIsAbsolute;
			result.ProductPriceSale = _productPriceSale;
			result.ProductPriceSaleEquality = _productPriceSaleEquality;
			result.ProductPriceSaleIsAbsolute = _productPriceSaleIsAbsolute;
			result.ProductName = _productName;

			result.IsSupplierExpanded = _isSupplierExpanded;
			result.IsFilterBySupplier = _isFilterBySupplier;
			result.SupplierCode = _supplierCode;
			result.SupplierName = _supplierName;

			result.IsFamilyExpanded = this._isFamilyExpanded;
			result.IsFilterByFamily = this._isFilterByFamily;
			result.FamilyCode = this._familyCode;
			result.FamilyName = this._familyName;

			result.IsSectionExpanded = _isSectionExpanded;
			result.IsFilterBySection = _isFilterBySection;
			result.SectionItems = _sectionItems.Where(r => r.IsChecked).Select(r => r.Section.SectionCode).ToList();

			return result;
		}

        public void Reset()
        {
            _sortViewModel.Reset();

            IsFilterByInventProduct = false;
            InventProductQuantityDifference = String.Empty;
            InventProductQuantityDifferenceEquality = _equalityItems.FirstOrDefault();
            InventProductQuantityDifferenceIsAbsolute = false;
            InventProductQuantityEdit = String.Empty;
            InventProductQuantityEditEquality = _equalityItems.FirstOrDefault();
            InventProductQuantityEditIsAbsolute = false;
            InventProductValueBuyDifference = String.Empty;
            InventProductValueBuyDifferenceEquality = _equalityItems.FirstOrDefault();
            InventProductValueBuyDifferenceIsAbsolute = false;
            InventProductValueBuyEdit = String.Empty;
            InventProductValueBuyEditEquality = _equalityItems.FirstOrDefault();
            InventProductValueBuyEditIsAbsolute = false;

            IsFilterByProduct = true;
            ProductMakat = String.Empty;
			ProductQuantityOriginalERP = String.Empty;
			ProductQuantityOriginalERPEquality = _equalityItems.FirstOrDefault();
			ProductQuantityOriginalERPIsAbsolute = true;
            ProductQuantityDifferenceOriginalERP = String.Empty;
            ProductQuantityDifferenceOriginalERPEquality = _equalityItems.FirstOrDefault();
            ProductQuantityDifferenceOriginalERPIsAbsolute = true;
            ProductValueDifferenceOriginalERP = String.Empty;
            ProductValueDifferenceOriginalERPEquality = _equalityItems.FirstOrDefault();
            ProductValueDifferenceOriginalERPIsAbsolute = true;
            ProductPriceBuy = String.Empty;
            ProductPriceBuyEquality = _equalityItems.FirstOrDefault();
            ProductPriceBuyIsAbsolute = true;
            ProductPriceSale = String.Empty;
            ProductPriceSaleEquality = _equalityItems.FirstOrDefault();
            ProductPriceSaleIsAbsolute = true;
            ProductName = String.Empty;

            IsFilterBySupplier = false;
            SupplierName = String.Empty;
            SupplierCode = String.Empty;

			IsFilterByFamily = false;
			FamilyCode = String.Empty;
			FamilyName = String.Empty;

            IsFilterBySection = false;
            foreach (var viewModel in _sectionItems)
            {
                viewModel.IsChecked = true;
            }
        }

        public void ApplyFilterData(IFilterData data)
        {
            InventProductSumFilterData filter = data as InventProductSumFilterData;
            if (filter == null)
                return;

            _sortViewModel.InitFromFilterData(filter);

            IsInventProductExpanded = filter.IsInventProductExpanded;
            IsFilterByInventProduct = filter.IsFilterByInventProduct;
            InventProductQuantityDifference = filter.InventProductQuantityDifference;
            InventProductQuantityDifferenceEquality = filter.InventProductQuantityDifferenceEquality;
            InventProductQuantityDifferenceIsAbsolute = filter.InventProductQuantityDifferenceIsAbsolute;
            InventProductQuantityEdit = filter.InventProductQuantityEdit;
            InventProductQuantityEditEquality = filter.InventProductQuantityEditEquality;
            InventProductQuantityEditIsAbsolute = filter.InventProductQuantityEditIsAbsolute;
            InventProductValueBuyDifference = filter.InventProductValueBuyDifference;
            InventProductValueBuyDifferenceEquality = filter.InventProductValueBuyDifferenceEquality;
            InventProductValueBuyDifferenceIsAbsolute = filter.InventProductValueBuyDifferenceIsAbsolute;
            InventProductValueBuyEdit = filter.InventProductValueBuyEdit;
            InventProductValueBuyEditEquality = filter.InventProductValueBuyEditEquality;
            InventProductValueBuyEditIsAbsolute = filter.InventProductValueBuyEditIsAbsolute;

            IsProductExpanded = filter.IsProductExpanded;
            IsFilterByProduct = filter.IsFilterByProduct;
            ProductMakat = filter.ProductMakat;

			ProductQuantityOriginalERP = filter.ProductQuantityOriginalERP;
			ProductQuantityOriginalERPEquality = filter.ProductQuantityOriginalERPEquality;
			ProductQuantityOriginalERPIsAbsolute = filter.ProductQuantityOriginalERPIsAbsolute;
            ProductQuantityDifferenceOriginalERP = filter.ProductQuantityDifferenceOriginalERP;
            ProductQuantityDifferenceOriginalERPEquality = filter.ProductQuantityDifferenceOriginalERPEquality;
            ProductQuantityDifferenceOriginalERPIsAbsolute = filter.ProductQuantityDifferenceOriginalERPIsAbsolute;
            ProductValueDifferenceOriginalERP = filter.ProductValueDifferenceOriginalERP;
            ProductValueDifferenceOriginalERPEquality = filter.ProductValueDifferenceOriginalERPEquality;
            ProductValueDifferenceOriginalERPIsAbsolute = filter.ProductValueDifferenceOriginalERPIsAbsolute;
            ProductPriceBuy = filter.ProductPriceBuy;
            ProductPriceBuyEquality = filter.ProductPriceBuyEquality;
            ProductPriceBuyIsAbsolute = filter.ProductPriceBuyIsAbsolute;
            ProductPriceSale = filter.ProductPriceSale;
            ProductPriceSaleEquality = filter.ProductPriceSaleEquality;
            ProductPriceSaleIsAbsolute = filter.ProductPriceSaleIsAbsolute;
            ProductName = filter.ProductName;

            IsSupplierExpanded = filter.IsSupplierExpanded;
            IsFilterBySupplier = filter.IsFilterBySupplier;
            SupplierCode = filter.SupplierCode;
            SupplierName = filter.SupplierName;

			IsFamilyExpanded = filter.IsFamilyExpanded;
			IsFilterByFamily = filter.IsFilterByFamily;
			FamilyName = filter.FamilyName;
			FamilyCode = filter.FamilyCode;

            IsSectionExpanded = filter.IsSectionExpanded;
            IsFilterBySection = filter.IsFilterBySection;

            _sectionItems.ToList().ForEach(r => r.IsChecked = false);
            foreach (string sectionCode in filter.SectionItems)
            {
                var viewModel = _sectionItems.FirstOrDefault(r => r.Section.SectionCode == sectionCode);
                if (viewModel != null)
                {
                    viewModel.IsChecked = true;
                }
            }
        }

        public bool CanSearch()
        {
            return !_isBuildingTable;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.InventProductSumAdvancedSearch;
        }

        private void BuildSection()
        {
            try
            {
                Utils.RunOnUI(() => _sectionItems.Clear());
                Dictionary<string, Count4U.Model.Count4U.Section> sectionsDictionary = new Dictionary<string, Model.Count4U.Section>();
              
				if (string.IsNullOrWhiteSpace(FindBySectionCode) == false)
				{
                    List<string> SectionCodeValues = FindBySectionCode.Split(',').ToList();
           
                    foreach (var sectionCode in SectionCodeValues)
                    {
                        if (string.IsNullOrWhiteSpace(sectionCode) == true) continue;
                        SelectParams selectParams = new SelectParams();
                        selectParams.FilterParams.Add("SectionCode", new FilterParam() { Operator = FilterOperator.Contains, Value = sectionCode });
                        Sections sections = _sectionRepository.GetSections(selectParams, base.GetDbPath);
                        foreach (Count4U.Model.Count4U.Section section in sections) 
                        {
                            sectionsDictionary[section.SectionCode] = section;
                        }
                    }
                   

                    //selectParams.FilterParams.Add("SectionCode", new FilterParam() { Operator = FilterOperator.Contains, Value = FindBySectionCode });
                    //selectParams.FilterStringListParams.Add("SectionCode", new FilterStringListParam()
                    //{
                    //    Values = SectionCodeValues
                    //});
                }
				if (string.IsNullOrWhiteSpace(FindBySectionTag) == false)
				{
                    SelectParams selectParams = new SelectParams();
                    selectParams.FilterParams.Add("Tag", new FilterParam() { Operator = FilterOperator.Contains, Value = FindBySectionTag });
                    //_sectionListViewModelBuilder.Build(_sectionItems, base.GetDbPath, selectParams);
                    Sections sections = _sectionRepository.GetSections(selectParams, base.GetDbPath);
                    foreach (Count4U.Model.Count4U.Section section in sections)
                    {
                        sectionsDictionary[section.SectionCode] = section;
                    }
                }

                if (string.IsNullOrWhiteSpace(FindBySectionTag) == true
                    && string.IsNullOrWhiteSpace(FindBySectionCode) == true)
                {
                    Sections sections = _sectionRepository.GetSections(base.GetDbPath);
                    foreach (Count4U.Model.Count4U.Section section in sections)
                    {
                        sectionsDictionary[section.SectionCode] = section;
                    }
                }

                    List<Count4U.Model.Count4U.Section> sectionList = sectionsDictionary.Values.ToList();
                this._sectionListViewModelBuilder.Build1(_sectionItems, sectionList);

            }
            catch (Exception exc)
            {
              _logger.ErrorException("BuildSection", exc);
                throw;
            }
        }

        public void BuildAnalyzeTable()
        {
			AuditConfig auditConfig = base._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
			Inventor currentInventor = base._contextCBIRepository.GetCurrentInventor(auditConfig);

            IsBuildingTable = true;

			_task = Task.Factory.StartNew(() => //Marina LogTaskFactoryExceptions("InitializeModules");
                {
                    _cts = new CancellationTokenSource();
					InventProductUtils.BuildAnalyzeTableSum(_iturAnalyzesSourceRepository, _iturAnalyzesRepository, _cts, base.GetDbPath, currentInventor);

                    Utils.RunOnUIAsync(() =>
                        {
                            IsBuildingTable = false;
                            _cts = null;
                        });
				});//.LogTaskFactoryExceptions("InitializeModules");
        }
    }
}