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
    public class SearchInventProductAdvancedFieldSimpleViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly IIturRepository _iturRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IProductRepository _productRepository;
        private readonly LocationListViewModelBuilder _locationListViewModelBuilder;
        private readonly IIturAnalyzesRepository _iturAnalyzesRepository;
        private readonly SectionListViewModelBuilder _sectionListViewModelBuilder;
        private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
        private readonly IRegionManager _regionManager;

        private DelegateCommand _searchCommand;

        private readonly ObservableCollection<string> _equalityItems;

        private bool _isLocationExpanded;
        private bool _isFilterByLocation;
        private readonly ObservableCollection<LocationItemViewModel> _locationItems;

        private bool _isIturExpanded;
        private bool _isFilterByItur;
        private string _iturCode;
        private string _iturERPCode;
        private string _iturNumberPrefix;
        private string _iturNumberSuffix;

        private bool _isInventProductExpanded;
        private bool _isFilterByInventProduct;
        private bool _inventProductInputTypeK;
        private bool _inventProductInputTypeB;
        private bool _inventProductIsItemsFromCatalog;
        private bool _inventProductIsItemsNotInCatalog;
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
        private string _productBarcode;
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

        private bool _isReportExpanded;
        private bool _isFilterByReport;
        private string _reportQuantityDifferenceOriginalERPEquality;
        private string _reportQuantityDifferenceOriginalERP;
        private bool _reportQuantityDifferenceOriginalERPIsAbsolute;

        private string _reportValueDifferenceOriginalERPEquality;
        private string _reportValueDifferenceOriginalERP;
        private bool _reportValueDifferenceOriginalERPIsAbsolute;

		private bool _isCheckedLocations;
		private bool _isCheckedSections;

		private string _findByLocationCode = String.Empty;
		private string _findByLocationTag = String.Empty;


		private string _findBySectionCode = String.Empty;
		private string _findBySectionTag = String.Empty;
	
		public SearchInventProductAdvancedFieldSimpleViewModel(
			IContextCBIRepository contextCbiRepository,
			IIturRepository iturRepository,
			ILocationRepository locationRepository,
			ISectionRepository sectionRepository,
			IProductRepository productRepository,
			LocationListViewModelBuilder locationListViewModelBuilder,
			IIturAnalyzesRepository iturAnalyzesRepository,
			SectionListViewModelBuilder sectionListViewModelBuilder,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository,
			IRegionManager regionManager)
			: base(contextCbiRepository)
		{
			this._regionManager = regionManager;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._sectionListViewModelBuilder = sectionListViewModelBuilder;
			this._iturAnalyzesRepository = iturAnalyzesRepository;
			this._locationListViewModelBuilder = locationListViewModelBuilder;
			this._productRepository = productRepository;
			this._sectionRepository = sectionRepository;
			this._locationRepository = locationRepository;
			this._iturRepository = iturRepository;
			this._locationItems = new ObservableCollection<LocationItemViewModel>();
			this._equalityItems = new ObservableCollection<string>() { 
                Common.Constants.ComboValues.Equality.Equal, 
                Common.Constants.ComboValues.Equality.Greater,
                Common.Constants.ComboValues.Equality.Less,
                Common.Constants.ComboValues.Equality.GreaterOrEqual,
                Common.Constants.ComboValues.Equality.LessOrEqual,
            };

			this._sectionItems = new ObservableCollection<SectionItem2ViewModel>();

			this._iturCode = String.Empty;
			this._iturERPCode = String.Empty;
			this._iturNumberPrefix = String.Empty;
			this._iturNumberSuffix = String.Empty;

			this._inventProductQuantityDifference = String.Empty;
			this._inventProductQuantityDifferenceEquality = String.Empty;
			this._inventProductQuantityEdit = String.Empty;
			this._inventProductQuantityEditEquality = String.Empty;
			this._inventProductValueBuyDifference = String.Empty;
			this._inventProductValueBuyDifferenceEquality = String.Empty;
			this._inventProductValueBuyEdit = String.Empty;
			this._inventProductValueBuyEditEquality = String.Empty;
			this._productMakat = String.Empty;
			this._productBarcode = String.Empty;
			this._productPriceBuy = String.Empty;
			this._productPriceBuyEquality = String.Empty;
			this._productPriceSale = String.Empty;
			this._productPriceSaleEquality = String.Empty;
			this._productName = String.Empty;
			this._supplierCode = String.Empty;
			this._supplierName = String.Empty;
			this._reportQuantityDifferenceOriginalERPEquality = String.Empty;
			this._reportQuantityDifferenceOriginalERP = String.Empty;
			this._reportValueDifferenceOriginalERPEquality = String.Empty;
			this._reportValueDifferenceOriginalERP = String.Empty;

			this._inventProductInputTypeK = true;
			this._inventProductInputTypeB = true;

			this._inventProductQuantityDifferenceEquality = this._equalityItems.FirstOrDefault();
			this._inventProductQuantityEditEquality = this._equalityItems.FirstOrDefault();
			this._inventProductValueBuyDifferenceEquality = this._equalityItems.FirstOrDefault();
			this._inventProductValueBuyEditEquality = this._equalityItems.FirstOrDefault();
			this._productPriceBuyEquality = this._equalityItems.FirstOrDefault();
			this._productPriceSaleEquality = this._equalityItems.FirstOrDefault();

			this._inventProductIsItemsFromCatalog = true;
			this._inventProductIsItemsNotInCatalog = true;

			this._reportQuantityDifferenceOriginalERPEquality = this._equalityItems.FirstOrDefault(r => r == Common.Constants.ComboValues.Equality.GreaterOrEqual);
			this._reportValueDifferenceOriginalERPEquality = this._equalityItems.FirstOrDefault(r => r == Common.Constants.ComboValues.Equality.GreaterOrEqual);

			this._reportQuantityDifferenceOriginalERPIsAbsolute = true;
			this._reportValueDifferenceOriginalERPIsAbsolute = true;

			this._reportQuantityDifferenceOriginalERP = "0";
			this._reportValueDifferenceOriginalERP = "0";
		}

		

		public string FindByLocationCode
		{
			get { return _findByLocationCode; }
			set
			{
				if (_findByLocationCode != value)
				{
					_findByLocationCode = value;
					RaisePropertyChanged(() => FindByLocationCode);

					BuildLocation();
				}
			}
		}

		public string FindByLocationTag
		{
			get { return _findByLocationTag; }
			set
			{
				if (_findByLocationTag != value)
				{
					_findByLocationTag = value;
					RaisePropertyChanged(() => FindByLocationTag);

					BuildLocation();
				}
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

        public ObservableCollection<string> EqualityItems
        {
			get { return this._equalityItems; }
        }

        public bool IsLocationExpanded
        {
			get { return this._isLocationExpanded; }
            set
            {
				this._isLocationExpanded = value;

                RaisePropertyChanged(() => IsLocationExpanded);
            }
        }

		public bool IsCheckedLocations
		{
			get { return _isCheckedLocations; }
			set {
				_isCheckedLocations = value;
				this.LocationItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedLocations);

			}
		}

		public bool IsCheckedSections
		{
			get { return _isCheckedSections; }
			set {
				_isCheckedSections = value;
				this.SectionItems.ToList().ForEach(r => r.IsChecked = value);
				RaisePropertyChanged(() => IsCheckedSections);

			}
		}

		
        public bool IsFilterByLocation
        {
			get { return this._isFilterByLocation; }
            set
            {
				this._isFilterByLocation = value;
                RaisePropertyChanged(() => IsFilterByLocation);
            }
        }

        public ObservableCollection<LocationItemViewModel> LocationItems
        {
			get { return this._locationItems; }
        }

        public bool IsIturExpanded
        {
			get { return this._isIturExpanded; }
            set
            {
				this._isIturExpanded = value;
                RaisePropertyChanged(() => IsIturExpanded);
            }
        }

        public bool IsFilterByItur
        {
			get { return this._isFilterByItur; }
            set
            {
				this._isFilterByItur = value;
                RaisePropertyChanged(() => IsFilterByItur);
            }
        }

        public string IturCode
        {
			get { return this._iturCode; }
            set
            {
				this._iturCode = value;
                RaisePropertyChanged(() => IturCode);
            }
        }

        public string IturERPCode
        {
			get { return this._iturERPCode; }
            set
            {
				this._iturERPCode = value;
                RaisePropertyChanged(() => IturERPCode);
            }
        }

        public string IturNumberPrefix
        {
			get { return this._iturNumberPrefix; }
            set
            {
				this._iturNumberPrefix = value;
                RaisePropertyChanged(() => IturNumberPrefix);
            }
        }

        public string IturNumberSuffix
        {
			get { return this._iturNumberSuffix; }
            set
            {
				this._iturNumberSuffix = value;
                RaisePropertyChanged(() => IturNumberSuffix);
            }
        }

        public bool IsInventProductExpanded
        {
			get { return this._isInventProductExpanded; }
            set
            {
				this._isInventProductExpanded = value;
                RaisePropertyChanged(() => IsInventProductExpanded);
            }
        }

        public bool IsFilterByInventProduct
        {
			get { return this._isFilterByInventProduct; }
            set
            {
				this._isFilterByInventProduct = value;
                RaisePropertyChanged(() => IsFilterByInventProduct);
            }
        }

        public bool InventProductInputTypeK
        {
			get { return this._inventProductInputTypeK; }
            set
            {
				this._inventProductInputTypeK = value;
                RaisePropertyChanged(() => InventProductInputTypeK);
            }
        }

        public bool InventProductInputTypeB
        {
			get { return this._inventProductInputTypeB; }
            set
            {
				this._inventProductInputTypeB = value;
                RaisePropertyChanged(() => InventProductInputTypeB);
            }
        }


        public bool InventProductIsItemsFromCatalog
        {
			get { return this._inventProductIsItemsFromCatalog; }
            set
            {
				this._inventProductIsItemsFromCatalog = value;
                RaisePropertyChanged(() => InventProductIsItemsFromCatalog);
            }
        }

        public bool InventProductIsItemsNotInCatalog
        {
			get { return this._inventProductIsItemsNotInCatalog; }
            set
            {
				this._inventProductIsItemsNotInCatalog = value;
                RaisePropertyChanged(() => InventProductIsItemsNotInCatalog);
            }
        }

        public string InventProductQuantityDifference
        {
			get { return this._inventProductQuantityDifference; }
            set
            {
				this._inventProductQuantityDifference = value;
                RaisePropertyChanged(() => InventProductQuantityDifference);
            }
        }

        public string InventProductQuantityDifferenceEquality
        {
			get { return this._inventProductQuantityDifferenceEquality; }
            set
            {
				this._inventProductQuantityDifferenceEquality = value;
                RaisePropertyChanged(() => InventProductQuantityDifferenceEquality);
            }
        }

        public string InventProductQuantityEdit
        {
			get { return this._inventProductQuantityEdit; }
            set
            {
				this._inventProductQuantityEdit = value;
                RaisePropertyChanged(() => InventProductQuantityEdit);
            }
        }

        public string InventProductQuantityEditEquality
        {
			get { return this._inventProductQuantityEditEquality; }
            set
            {
				this._inventProductQuantityEditEquality = value;
                RaisePropertyChanged(() => InventProductQuantityEditEquality);
            }
        }

        public string InventProductValueBuyDifference
        {
			get { return this._inventProductValueBuyDifference; }
            set
            {
				this._inventProductValueBuyDifference = value;
                RaisePropertyChanged(() => InventProductValueBuyDifference);
            }
        }

        public string InventProductValueBuyDifferenceEquality
        {
			get { return this._inventProductValueBuyDifferenceEquality; }
            set
            {
				this._inventProductValueBuyDifferenceEquality = value;
                RaisePropertyChanged(() => InventProductValueBuyDifferenceEquality);
            }
        }

        public string InventProductValueBuyEdit
        {
			get { return this._inventProductValueBuyEdit; }
            set
            {
				this._inventProductValueBuyEdit = value;
                RaisePropertyChanged(() => InventProductValueBuyEdit);
            }
        }

        public string InventProductValueBuyEditEquality
        {
			get { return this._inventProductValueBuyEditEquality; }
            set
            {
				this._inventProductValueBuyEditEquality = value;
                RaisePropertyChanged(() => InventProductValueBuyEditEquality);
            }
        }

        public bool IsProductExpanded
        {
			get { return this._isProductExpanded; }
            set
            {
				this._isProductExpanded = value;
                RaisePropertyChanged(() => IsProductExpanded);
            }
        }

        public bool IsFilterByProduct
        {
			get { return this._isFilterByProduct; }
            set
            {
				this._isFilterByProduct = value;
                RaisePropertyChanged(() => IsFilterByProduct);
            }
        }

        public string ProductMakat
        {
			get { return this._productMakat; }
            set
            {
				this._productMakat = value;
                RaisePropertyChanged(() => ProductMakat);
            }
        }

        public string ProductBarcode
        {
			get { return this._productBarcode; }
            set
            {
				this._productBarcode = value;
                RaisePropertyChanged(() => ProductBarcode);
            }
        }

        public string ProductPriceBuy
        {
			get { return this._productPriceBuy; }
            set
            {
				this._productPriceBuy = value;
                RaisePropertyChanged(() => ProductPriceBuy);
            }
        }

        public string ProductPriceBuyEquality
        {
			get { return this._productPriceBuyEquality; }
            set
            {
				this._productPriceBuyEquality = value;
                RaisePropertyChanged(() => ProductPriceBuyEquality);
            }
        }

        public string ProductPriceSale
        {
			get { return this._productPriceSale; }
            set
            {
				this._productPriceSale = value;
                RaisePropertyChanged(() => ProductPriceSale);
            }
        }

        public string ProductPriceSaleEquality
        {
			get { return this._productPriceSaleEquality; }
            set
            {
				this._productPriceSaleEquality = value;
                RaisePropertyChanged(() => ProductPriceSaleEquality);
            }
        }

        public string ProductName
        {
			get { return this._productName; }
            set
            {
				this._productName = value;
                RaisePropertyChanged(() => ProductName);
            }
        }

        public bool IsSupplierExpanded
        {
			get { return this._isSupplierExpanded; }
            set
            {
				this._isSupplierExpanded = value;
                RaisePropertyChanged(() => IsSupplierExpanded);
            }
        }

        public bool IsFilterBySupplier
        {
			get { return this._isFilterBySupplier; }
            set
            {
				this._isFilterBySupplier = value;
                RaisePropertyChanged(() => IsFilterBySupplier);
            }
        }

        public string SupplierCode
        {
			get { return this._supplierCode; }
            set
            {
				this._supplierCode = value;
                RaisePropertyChanged(() => SupplierCode);
            }
        }

        public string SupplierName
        {
			get { return this._supplierName; }
            set
            {
				this._supplierName = value;
                RaisePropertyChanged(() => SupplierName);
            }
        }

				 //
	
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
			get { return this._isSectionExpanded; }
            set
            {
				this._isSectionExpanded = value;
                RaisePropertyChanged(() => IsSectionExpanded);
            }
        }

        public bool IsFilterBySection
        {
			get { return this._isFilterBySection; }
            set
            {
				this._isFilterBySection = value;
                RaisePropertyChanged(() => IsFilterBySection);
            }
        }

        public ObservableCollection<SectionItem2ViewModel> SectionItems
        {
			get { return this._sectionItems; }
        }

        public bool IsBuildingTable
        {
			get { return this._isBuildingTable; }
            set
            {
				this._isBuildingTable = value;
                RaisePropertyChanged(() => IsBuildingTable);

				if (this._searchCommand != null)
                {
					this._searchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SearchCommand
        {
			get { return this._searchCommand; }
			set { this._searchCommand = value; }
        }

        public bool InventProductQuantityDifferenceIsAbsolute
        {
			get { return this._inventProductQuantityDifferenceIsAbsolute; }
            set
            {
				this._inventProductQuantityDifferenceIsAbsolute = value;
                RaisePropertyChanged(() => InventProductQuantityDifferenceIsAbsolute);
            }
        }

        public bool InventProductQuantityEditIsAbsolute
        {
            get { return _inventProductQuantityEditIsAbsolute; }
            set
            {
				this._inventProductQuantityEditIsAbsolute = value;
                RaisePropertyChanged(() => InventProductQuantityEditIsAbsolute);
            }
        }

        public bool InventProductValueBuyDifferenceIsAbsolute
        {
            get { return _inventProductValueBuyDifferenceIsAbsolute; }
            set
            {
				this._inventProductValueBuyDifferenceIsAbsolute = value;
                RaisePropertyChanged(() => InventProductValueBuyDifferenceIsAbsolute);
            }
        }

        public bool InventProductValueBuyEditIsAbsolute
        {
            get { return _inventProductValueBuyEditIsAbsolute; }
            set
            {
				this._inventProductValueBuyEditIsAbsolute = value;
                RaisePropertyChanged(() => InventProductValueBuyEditIsAbsolute);
            }
        }

        public bool ProductPriceBuyIsAbsolute
        {
            get { return _productPriceBuyIsAbsolute; }
            set
            {
				this._productPriceBuyIsAbsolute = value;
                RaisePropertyChanged(() => ProductPriceBuyIsAbsolute);
            }
        }

        public bool ProductPriceSaleIsAbsolute
        {
            get { return _productPriceSaleIsAbsolute; }
            set
            {
				this._productPriceSaleIsAbsolute = value;
                RaisePropertyChanged(() => ProductPriceSaleIsAbsolute);
            }
        }

        public bool IsReportExpanded
        {
            get { return _isReportExpanded; }
            set
            {
				this._isReportExpanded = value;
                RaisePropertyChanged(() => IsReportExpanded);
            }
        }

        public bool IsFilterByReport
        {
            get { return _isFilterByReport; }
            set
            {
				this._isFilterByReport = value;
                RaisePropertyChanged(() => IsFilterByReport);
            }
        }

        public string ReportQuantityDifferenceOriginalERPEquality
        {
            get { return _reportQuantityDifferenceOriginalERPEquality; }
            set
            {
				this._reportQuantityDifferenceOriginalERPEquality = value;
                RaisePropertyChanged(() => ReportQuantityDifferenceOriginalERPEquality);
            }
        }

        public string ReportQuantityDifferenceOriginalERP
        {
            get { return _reportQuantityDifferenceOriginalERP; }
            set
            {
				this._reportQuantityDifferenceOriginalERP = value;
                RaisePropertyChanged(() => ReportQuantityDifferenceOriginalERP);
            }
        }

        public bool ReportQuantityDifferenceOriginalERPIsAbsolute
        {
            get { return _reportQuantityDifferenceOriginalERPIsAbsolute; }
            set
            {
				this._reportQuantityDifferenceOriginalERPIsAbsolute = value;
                RaisePropertyChanged(() => ReportQuantityDifferenceOriginalERPIsAbsolute);
            }
        }

        public string ReportValueDifferenceOriginalERPEquality
        {
            get { return _reportValueDifferenceOriginalERPEquality; }
            set
            {
				this._reportValueDifferenceOriginalERPEquality = value;
                RaisePropertyChanged(() => ReportValueDifferenceOriginalERPEquality);
            }
        }

        public string ReportValueDifferenceOriginalERP
        {
			get { return this._reportValueDifferenceOriginalERP; }
            set
            {
				this._reportValueDifferenceOriginalERP = value;
                RaisePropertyChanged(() => ReportValueDifferenceOriginalERP);
            }
        }

        public bool ReportValueDifferenceOriginalERPIsAbsolute
        {
            get { return _reportValueDifferenceOriginalERPIsAbsolute; }
            set
            {
                _reportValueDifferenceOriginalERPIsAbsolute = value;
                RaisePropertyChanged(() => ReportValueDifferenceOriginalERPIsAbsolute);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildLocation();
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
			if (this._cts != null)
            {
				this._cts.Cancel();
				if (this._task != null)
                {
					this._task.Wait();
					this._task = null;
                }
				this._cts = null;
            }
        }

        private void BuildSort()
        {
			this._sortViewModel = Utils.GetViewModelFromRegion<SortViewModel>(Common.RegionNames.Sort, this._regionManager);

            List<PropertyInfo> sortProperties = new List<PropertyInfo>();
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.LocationName));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.IturCode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ERPIturCode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.Itur_StatusIturGroupBit));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.Itur_NumberSufix));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.DocNum));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.IPNum));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityDifference));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.QuantityEdit));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueBuyDifference));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ValueBuyEdit));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.Makat));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.Barcode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.PriceBuy));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.PriceSale));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.ProductName));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SupplierCode));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SupplierName));
            sortProperties.Add(TypedReflection<IturAnalyzes>.GetPropertyInfo(r => r.SectionName));

			this._sortViewModel.Add(sortProperties);
        }

        public IFilterData BuildFilterData()
        {
            InventProductSimpleFilterData result = new InventProductSimpleFilterData();

			this._sortViewModel.ApplyToFilterData(result);

			result.IsLocationExpanded = this._isLocationExpanded;
			result.IsFilterByLocation = this._isFilterByLocation;
			result.LocationItems = this._locationItems.Where(r => r.IsChecked).Select(r => r.Location.Code).ToList();

			result.IsIturExpanded = this._isIturExpanded;
			result.IsFilterByItur = this._isFilterByItur;
			result.IturCode = this._iturCode;
			result.IturERPCode = this._iturERPCode;
			result.IturNumberPrefix = this._iturNumberPrefix;
			result.IturNumberSuffix = this._iturNumberSuffix;

			result.IsInventProductExpanded = this._isInventProductExpanded;
			result.IsFilterByInventProduct = this._isFilterByInventProduct;
			result.InventProductInputTypeK = this._inventProductInputTypeK;
			result.InventProductInputTypeB = this._inventProductInputTypeB;
			result.InventProductIsItemsFromCatalog = this._inventProductIsItemsFromCatalog;
			result.InventProductIsItemsNotInCatalog = this._inventProductIsItemsNotInCatalog;
			result.InventProductQuantityDifference = this._inventProductQuantityDifference;
			result.InventProductQuantityDifferenceEquality = this._inventProductQuantityDifferenceEquality;
			result.InventProductQuantityDifferenceIsAbsolute = this._inventProductQuantityDifferenceIsAbsolute;
			result.InventProductQuantityEdit = this._inventProductQuantityEdit;
			result.InventProductQuantityEditEquality = this._inventProductQuantityEditEquality;
			result.InventProductQuantityEditIsAbsolute = this._inventProductQuantityEditIsAbsolute;
			result.InventProductValueBuyDifference = this._inventProductValueBuyDifference;
			result.InventProductValueBuyDifferenceEquality = this._inventProductValueBuyDifferenceEquality;
			result.InventProductValueBuyDifferenceIsAbsolute = this._inventProductValueBuyDifferenceIsAbsolute;
			result.InventProductValueBuyEdit = this._inventProductValueBuyEdit;
			result.InventProductValueBuyEditEquality = this._inventProductValueBuyEditEquality;
			result.InventProductValueBuyEditIsAbsolute = this._inventProductValueBuyEditIsAbsolute;

			result.IsProductExpanded = this._isProductExpanded;
			result.IsFilterByProduct = this._isFilterByProduct;
			result.ProductMakat = this._productMakat;
			result.ProductBarcode = this._productBarcode;
			result.ProductPriceBuy = this._productPriceBuy;
			result.ProductPriceBuyEquality = this._productPriceBuyEquality;
			result.ProductPriceBuyIsAbsolute = this._productPriceBuyIsAbsolute;
			result.ProductPriceSale = this._productPriceSale;
			result.ProductPriceSaleEquality = this._productPriceSaleEquality;
			result.ProductPriceSaleIsAbsolute = this._productPriceSaleIsAbsolute;
			result.ProductName = this._productName;

			result.IsSupplierExpanded = this._isSupplierExpanded;
			result.IsFilterBySupplier = this._isFilterBySupplier;
			result.SupplierCode = this._supplierCode;
			result.SupplierName = this._supplierName;

			result.IsFamilyExpanded = this._isFamilyExpanded;
			result.IsFilterByFamily = this._isFilterByFamily;
			result.FamilyCode = this._familyCode;
			result.FamilyName = this._familyName;

			result.IsSectionExpanded = this._isSectionExpanded;
			result.IsFilterBySection = this._isFilterBySection;
			result.SectionItems = this._sectionItems.Where(r => r.IsChecked).Select(r => r.Section.SectionCode).ToList();

			result.IsReportExpanded = this._isReportExpanded;
			result.IsFilterByReport = this._isFilterByReport;

			result.ReportQuantityDifferenceOriginalERPEquality = this._reportQuantityDifferenceOriginalERPEquality;
			result.ReportQuantityDifferenceOriginalERP = this._reportQuantityDifferenceOriginalERP;
			result.ReportQuantityDifferenceOriginalERPIsAbsolute = this._reportQuantityDifferenceOriginalERPIsAbsolute;

			result.ReportValueDifferenceOriginalERPEquality = this._reportValueDifferenceOriginalERPEquality;
			result.ReportValueDifferenceOriginalERP = this._reportValueDifferenceOriginalERP;
			result.ReportValueDifferenceOriginalERPIsAbsolute = this._reportValueDifferenceOriginalERPIsAbsolute;

            return result;
        }



		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			InventProductSimpleFilterData result = new InventProductSimpleFilterData();

			this._sortViewModel.ApplyToFilterData(result);

			result.IsLocationExpanded = this._isLocationExpanded;
			result.IsFilterByLocation = this._isFilterByLocation;
			result.LocationItems = this._locationItems.Where(r => r.IsChecked).Select(r => r.Location.Code).ToList();

			result.IsIturExpanded = this._isIturExpanded;
			result.IsFilterByItur = this._isFilterByItur;
			result.IturCode = this._iturCode;
			result.IturERPCode = this._iturERPCode;
			result.IturNumberPrefix = this._iturNumberPrefix;
			result.IturNumberSuffix = this._iturNumberSuffix;

			result.IsInventProductExpanded = this._isInventProductExpanded;
			result.IsFilterByInventProduct = this._isFilterByInventProduct;
			result.InventProductInputTypeK = this._inventProductInputTypeK;
			result.InventProductInputTypeB = this._inventProductInputTypeB;
			result.InventProductIsItemsFromCatalog = this._inventProductIsItemsFromCatalog;
			result.InventProductIsItemsNotInCatalog = this._inventProductIsItemsNotInCatalog;
			result.InventProductQuantityDifference = this._inventProductQuantityDifference;
			result.InventProductQuantityDifferenceEquality = this._inventProductQuantityDifferenceEquality;
			result.InventProductQuantityDifferenceIsAbsolute = this._inventProductQuantityDifferenceIsAbsolute;
			result.InventProductQuantityEdit = this._inventProductQuantityEdit;
			result.InventProductQuantityEditEquality = this._inventProductQuantityEditEquality;
			result.InventProductQuantityEditIsAbsolute = this._inventProductQuantityEditIsAbsolute;
			result.InventProductValueBuyDifference = this._inventProductValueBuyDifference;
			result.InventProductValueBuyDifferenceEquality = this._inventProductValueBuyDifferenceEquality;
			result.InventProductValueBuyDifferenceIsAbsolute = this._inventProductValueBuyDifferenceIsAbsolute;
			result.InventProductValueBuyEdit = this._inventProductValueBuyEdit;
			result.InventProductValueBuyEditEquality = this._inventProductValueBuyEditEquality;
			result.InventProductValueBuyEditIsAbsolute = this._inventProductValueBuyEditIsAbsolute;

			result.IsProductExpanded = this._isProductExpanded;
			result.IsFilterByProduct = this._isFilterByProduct;
			result.ProductMakat = this._productMakat;
			result.ProductBarcode = this._productBarcode;
			result.ProductPriceBuy = this._productPriceBuy;
			result.ProductPriceBuyEquality = this._productPriceBuyEquality;
			result.ProductPriceBuyIsAbsolute = this._productPriceBuyIsAbsolute;
			result.ProductPriceSale = this._productPriceSale;
			result.ProductPriceSaleEquality = this._productPriceSaleEquality;
			result.ProductPriceSaleIsAbsolute = this._productPriceSaleIsAbsolute;
			result.ProductName = this._productName;

			result.IsSupplierExpanded = this._isSupplierExpanded;
			result.IsFilterBySupplier = this._isFilterBySupplier;
			result.SupplierCode = this._supplierCode;
			result.SupplierName = this._supplierName;

			result.IsFamilyExpanded = this._isFamilyExpanded;
			result.IsFilterByFamily = this._isFilterByFamily;
			result.FamilyCode = this._familyCode;
			result.FamilyName = this._familyName;

			result.IsSectionExpanded = this._isSectionExpanded;
			result.IsFilterBySection = this._isFilterBySection;
			result.SectionItems = this._sectionItems.Where(r => r.IsChecked).Select(r => r.Section.SectionCode).ToList();

			result.IsReportExpanded = this._isReportExpanded;
			result.IsFilterByReport = this._isFilterByReport;

			result.ReportQuantityDifferenceOriginalERPEquality = this._reportQuantityDifferenceOriginalERPEquality;
			result.ReportQuantityDifferenceOriginalERP = this._reportQuantityDifferenceOriginalERP;
			result.ReportQuantityDifferenceOriginalERPIsAbsolute = this._reportQuantityDifferenceOriginalERPIsAbsolute;

			result.ReportValueDifferenceOriginalERPEquality = this._reportValueDifferenceOriginalERPEquality;
			result.ReportValueDifferenceOriginalERP = this._reportValueDifferenceOriginalERP;
			result.ReportValueDifferenceOriginalERPIsAbsolute = this._reportValueDifferenceOriginalERPIsAbsolute;

			return result;
		}

        public void Reset()
        {
			this._sortViewModel.Reset();

            IsFilterByLocation = false;
			foreach (var viewModel in this._locationItems)
            {
                viewModel.IsChecked = false;
            }

            IsFilterByItur = false;
            IturCode = String.Empty;
            IturERPCode = String.Empty;
            IturNumberPrefix = String.Empty;
            IturNumberSuffix = String.Empty;

            IsFilterByInventProduct = false;
            InventProductInputTypeK = true;
            InventProductInputTypeB = true;
            InventProductIsItemsFromCatalog = true;
            InventProductIsItemsNotInCatalog = true;
            InventProductQuantityDifference = String.Empty;
			InventProductQuantityDifferenceEquality = this._equalityItems.FirstOrDefault();
            InventProductQuantityDifferenceIsAbsolute = false;
            InventProductQuantityEdit = String.Empty;
			InventProductQuantityEditEquality = this._equalityItems.FirstOrDefault();
            InventProductQuantityEditIsAbsolute = false;
            InventProductValueBuyDifference = String.Empty;
			InventProductValueBuyDifferenceEquality = this._equalityItems.FirstOrDefault();
            InventProductValueBuyDifferenceIsAbsolute = false;
            InventProductValueBuyEdit = String.Empty;
			InventProductValueBuyEditEquality = this._equalityItems.FirstOrDefault();
            InventProductValueBuyEditIsAbsolute = false;

            IsFilterByProduct = false;
            ProductMakat = String.Empty;
            ProductBarcode = String.Empty;
            ProductPriceBuy = String.Empty;
			ProductPriceBuyEquality = this._equalityItems.FirstOrDefault();
            ProductPriceBuyIsAbsolute = false;
            ProductPriceSale = String.Empty;
			ProductPriceSaleEquality = this._equalityItems.FirstOrDefault();
            ProductPriceSaleIsAbsolute = false;
            ProductName = String.Empty;

            IsFilterBySupplier = false;
            SupplierCode = String.Empty;
            SupplierName = String.Empty;

			IsFilterByFamily = false;
			FamilyCode = String.Empty;
			FamilyName = String.Empty;

            IsFilterBySection = false;
			foreach (var viewModel in this._sectionItems)
            {
                viewModel.IsChecked = true;
            }
            
            IsFilterByReport = false;
            ReportQuantityDifferenceOriginalERP = String.Empty;
            ReportValueDifferenceOriginalERP = String.Empty;
        }

        public void ApplyFilterData(IFilterData data)
        {
            InventProductSimpleFilterData filter = data as InventProductSimpleFilterData;
            if (filter == null)
                return;

			this._sortViewModel.InitFromFilterData(filter);

            IsLocationExpanded = filter.IsLocationExpanded;
            IsFilterByLocation = filter.IsFilterByLocation;

            IsIturExpanded = filter.IsIturExpanded;
            IsFilterByItur = filter.IsFilterByItur;
            IturCode = filter.IturCode;
            IturERPCode = filter.IturERPCode;
            IturNumberPrefix = filter.IturNumberPrefix;
            IturNumberSuffix = filter.IturNumberSuffix;

            IsInventProductExpanded = filter.IsInventProductExpanded;
            IsFilterByInventProduct = filter.IsFilterByInventProduct;
            InventProductInputTypeK = filter.InventProductInputTypeK;
            InventProductInputTypeB = filter.InventProductInputTypeB;
            InventProductIsItemsFromCatalog = filter.InventProductIsItemsFromCatalog;
            InventProductIsItemsNotInCatalog = filter.InventProductIsItemsNotInCatalog;
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
            ProductBarcode = filter.ProductBarcode;
            ProductPriceBuy = filter.ProductPriceBuy;
            ProductPriceBuyEquality = filter.ProductPriceBuyEquality;
            ProductPriceBuyIsAbsolute = filter.ProductPriceBuyIsAbsolute;
            ProductPriceSale = filter.ProductPriceSale;
            ProductPriceSaleEquality = filter.ProductPriceSaleEquality;
            ProductPriceSaleIsAbsolute = filter.ProductPriceSaleIsAbsolute;
            ProductName = filter.ProductName;

            IsSupplierExpanded = filter.IsSupplierExpanded;
            IsFilterBySupplier = filter.IsFilterBySupplier;
            SupplierName = filter.SupplierName;
            SupplierCode = filter.SupplierCode;

			IsFamilyExpanded = filter.IsFamilyExpanded;
			IsFilterByFamily = filter.IsFilterByFamily;
			FamilyName = filter.FamilyName;
			FamilyCode = filter.FamilyCode;

            IsSectionExpanded = filter.IsSectionExpanded;
            IsFilterBySection = filter.IsFilterBySection;

			this._locationItems.ToList().ForEach(r => r.IsChecked = false);
            foreach (string locationCode in filter.LocationItems)
            {
				LocationItemViewModel viewModel = this._locationItems.FirstOrDefault(r => r.Location.Code == locationCode);
                if (viewModel != null)
                {
                    viewModel.IsChecked = true;
                }
            }

			this._sectionItems.ToList().ForEach(r => r.IsChecked = false);
            foreach (string sectionCode in filter.SectionItems)
            {
				var viewModel = this._sectionItems.FirstOrDefault(r => r.Section.SectionCode == sectionCode);
                if (viewModel != null)
                {
                    viewModel.IsChecked = true;
                }
            }

            IsReportExpanded = filter.IsReportExpanded;
            IsFilterByReport = filter.IsFilterByReport;

            ReportQuantityDifferenceOriginalERPEquality = filter.ReportQuantityDifferenceOriginalERPEquality;
            ReportQuantityDifferenceOriginalERP = filter.ReportQuantityDifferenceOriginalERP;
            ReportQuantityDifferenceOriginalERPIsAbsolute = filter.ReportQuantityDifferenceOriginalERPIsAbsolute;

            ReportValueDifferenceOriginalERPEquality = filter.ReportValueDifferenceOriginalERPEquality;
            ReportValueDifferenceOriginalERP = filter.ReportValueDifferenceOriginalERP;
            ReportValueDifferenceOriginalERPIsAbsolute = filter.ReportValueDifferenceOriginalERPIsAbsolute;
        }

        public bool CanSearch()
        {
            return !_isBuildingTable;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            return ViewDomainContextEnum.InventProductAdvancedSearch;
        }

        private void BuildLocation()
        {
            try
            {
				this._locationItems.Clear();

				SelectParams selectParams = new SelectParams();
				if (string.IsNullOrWhiteSpace(FindByLocationCode) == false)
				{
					selectParams.FilterParams.Add("Code", new FilterParam() { Operator = FilterOperator.Contains, Value = FindByLocationCode });
				}
				if (string.IsNullOrWhiteSpace(FindByLocationTag) == false)
				{
					selectParams.FilterParams.Add("Tag", new FilterParam() { Operator = FilterOperator.Contains, Value = FindByLocationTag });
				}

				this._locationListViewModelBuilder.Build(_locationItems, base.GetDbPath, selectParams, r => { return false; }, r => { return false; });

            }
            catch (Exception exc)
            {
				_logger.ErrorException("BuildItems", exc);
            }

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

			//this._task = Task.Factory.StartNew(() =>
			this._task = Task.Factory.StartNew(() =>
                {
					this._cts = new CancellationTokenSource();

					InventProductUtils.BuildAnalyzeTableSimple(this._iturAnalyzesSourceRepository, this._iturAnalyzesRepository, this._cts, base.GetDbPath, currentInventor);

                    Utils.RunOnUIAsync(() =>
                        {
                            IsBuildingTable = false;
							this._cts = null;
						});
				});//.LogTaskFactoryExceptions("InitializeModules"); 
        }
    }
}