using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.Interfaces;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{													  
    public class DynamicColumnSettingsViewModel : CBIContextBaseViewModel
    {
        private readonly IEditorTemplateRepository _editorTemplateRepository;
        private readonly IEventAggregator _eventAggregator;

        private bool _isEditable;
		private bool _isCustomer;

        private readonly ObservableCollection<DynamicColumnSettingsItem> _itemsInventProductList;
        private DynamicColumnSettingsItem _selectedInventProductList;
        private readonly ObservableCollection<DynamicColumnSettingsItem> _itemsInventProduct;
        private DynamicColumnSettingsItem _selectedInventProduct;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        private readonly ObservableCollection<PriceTypeItemViewModel> _itemsPriceType;
        private PriceTypeItemViewModel _selectedPriceType;

		protected IImportModuleInfo _selectedComplex;
		protected ObservableCollection<IImportModuleInfo> _itemsComplex;

	//	ExportIturAdapterCode
		private string _complexStaticPath1;
		private string _complexStaticPath2;
		private string _complexStaticPath3;

	
							
		private readonly IUnityContainer _container;
		private readonly IImportAdapterRepository _importAdapterRepository;

        public DynamicColumnSettingsViewModel(
            IContextCBIRepository contextCbiRepository,
            IEditorTemplateRepository editorTemplateRepository,
            IEventAggregator eventAggregator,
			IImportAdapterRepository importAdapterRepository,
			IUnityContainer container)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
			_container = container;
			_importAdapterRepository = importAdapterRepository; 
            _editorTemplateRepository = editorTemplateRepository;
            _itemsInventProductList = new ObservableCollection<DynamicColumnSettingsItem>();
            _itemsInventProduct = new ObservableCollection<DynamicColumnSettingsItem>();
            _itemsPriceType = new ObservableCollection<PriceTypeItemViewModel>()
                {
                    new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceBuy, Title = PriceCodeCaption.PriceBuy},
                    new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceSale, Title = PriceCodeCaption.PriceSale},
                    new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceExtra, Title = PriceCodeCaption.PriceExtra},
                };

			_isEditable = true;
			_isCustomer= false;
        }

		protected IUnityContainer Container
		{
			get { return _container; }
		}

		public ObservableCollection<IImportModuleInfo> ItemsComplex
		{
			get { return this._itemsComplex; }
		}

		public IImportModuleInfo SelectedComplex
		{
			get { return _selectedComplex; }
			set
			{
				_selectedComplex = value;
				RaisePropertyChanged(() => SelectedComplex);

				if (_selectedComplex != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ImportModule = this._selectedComplex });

			}
		}

		public string ComplexStaticPath1
		{
			get { return _complexStaticPath1; }
			set
			{
				_complexStaticPath1 = value;
				RaisePropertyChanged(() => ComplexStaticPath2);
			}
		}

		public string ComplexStaticPath2
		{
			get { return _complexStaticPath2; }
			set
			{
				_complexStaticPath2 = value;
				RaisePropertyChanged(() => ComplexStaticPath2);
			}
		}

		public string ComplexStaticPath3
		{
			get { return _complexStaticPath3; }
			set
			{
				_complexStaticPath3 = value;
				RaisePropertyChanged(() => ComplexStaticPath3);
			}
		}

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(() => IsEditable);
            }
        }


		public bool IsCustomer
        {
			get { return _isCustomer; }
            set
            {
				_isCustomer = value;
				RaisePropertyChanged(() => IsCustomer);
            }
        }

        public ObservableCollection<DynamicColumnSettingsItem> ItemsInventProductList
        {
            get { return _itemsInventProductList; }
        }

        public DynamicColumnSettingsItem SelectedInventProductList
        {
            get { return _selectedInventProductList; }
            set
            {
                _selectedInventProductList = value;
                RaisePropertyChanged(() => SelectedInventProductList);
            }
        }

        public ObservableCollection<DynamicColumnSettingsItem> ItemsInventProduct
        {
            get { return _itemsInventProduct; }
        }

        public DynamicColumnSettingsItem SelectedInventProduct
        {
            get { return _selectedInventProduct; }
            set
            {
                _selectedInventProduct = value;
                RaisePropertyChanged(() => SelectedInventProduct);
            }
        }

        private PropertyInfo PropertyInfoInventProductList
        {
            get { return TypedReflection<DynamicColumnSettingsViewModel>.GetPropertyInfo(r => r.SelectedInventProductList); }
        }

        private PropertyInfo PropertyInfoInventProduct
        {
            get { return TypedReflection<DynamicColumnSettingsViewModel>.GetPropertyInfo(r => r.SelectedInventProduct); }
        }

        public ObservableCollection<PriceTypeItemViewModel> ItemsPriceType
        {
            get { return _itemsPriceType; }
        }

        public PriceTypeItemViewModel SelectedPriceType
        {
            get { return _selectedPriceType; }
            set
            {
                _selectedPriceType = value;
                RaisePropertyChanged(() => SelectedPriceType);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
			string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
			string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            _eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterChanged);

            FillComboDefault(_itemsInventProductList, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, PropertyInfoInventProductList);
            FillComboDefault(_itemsInventProduct, Configuration.Constants.DynamicView.InventProductView_InventProduct, PropertyInfoInventProduct);

			this._itemsComplex = new ObservableCollection<IImportModuleInfo>(Utils.GetImportAdapters(this.Container, this._importAdapterRepository, ImportDomainEnum.ComplexOperation,
				currentCustomerCode, currentBranchCode, currentInventorCode));

	
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);
        }

        //        private string GetDbPathOverriden()
        //        {
        //            object cbiObject = null;
        //
        //            cbiObject = _customer ?? (object)_branch ?? _inventor ?? null;
        //
        //            return base.ContextCBIRepository.GetDBPath(cbiObject);
        //        }

        private void FillComboDefault(ObservableCollection<DynamicColumnSettingsItem> combo, string viewName, PropertyInfo piSelected)
        {
            foreach (EditorTemplate editorTemplate in _editorTemplateRepository.Get(viewName))
            {
                combo.Add(new DynamicColumnSettingsItem(editorTemplate));
            }

            piSelected.SetValue(this, combo.FirstOrDefault(), null);
        }

        public void SetIsEditable(bool isEditable)
        {
            this.IsEditable = isEditable;
        }

		public void SetIsCustomer(bool isCustomer)
		{
			this.IsCustomer = isCustomer;
		}


        public void SetCustomer(Customer customer)
        {
			if (customer == null) return;
            this._customer = customer;

            SetComboSelectedForObjectInventProductList(_customer);
            SetComboSelectedForObjectInventProduct(_customer);

            SetPriceComboByStringValue(customer.PriceCode);

			if (this._customer == null) return;
			string complexAdapterCode = this._customer.ComplexAdapterCode;
			if (!String.IsNullOrEmpty(complexAdapterCode) && ItemsComplex.Any(r => r.Name == complexAdapterCode))
				this._selectedComplex = ItemsComplex.FirstOrDefault(r => r.Name == complexAdapterCode);
			else
				this._selectedComplex = ItemsComplex.FirstOrDefault(r => r.IsDefault);


			this._complexStaticPath1 = this._customer.ImportCatalogPath != null ? this._customer.ImportCatalogPath : "";
			this._complexStaticPath2 = this._customer.ExportErpPath != null ? this._customer.ExportErpPath : "";
			this._complexStaticPath3 = this._customer.SendToOfficePath != null ? this._customer.SendToOfficePath : "";
        }


		private string GetSelectedAdapterNameByCBI()
		{
			string complexAdapterName = String.Empty;
			if (base.CurrentInventor != null)
			{
				complexAdapterName = base.CurrentInventor.ComplexAdapterCode;
			}
			if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return complexAdapterName;

			if (base.CurrentBranch != null)
			{
				complexAdapterName = base.CurrentBranch.ComplexAdapterCode;
			}
			if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return complexAdapterName;

			if (base.CurrentCustomer != null)
			{
				complexAdapterName = base.CurrentCustomer.ComplexAdapterCode;
			}
			if (string.IsNullOrWhiteSpace(complexAdapterName) == false) return complexAdapterName;
			return String.Empty;
		}

        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
            this._branch = branch;

            SetStateForBranch(mode);
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
            this._inventor = inventor;

            SetStateForInventor(mode);

        }

        public void SetStateForBranch(enBranchAdapterInherit mode)
        {
            Customer customer = this.CurrentCustomer;
			if (_branch == null) return;

            if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
            {
			   SetComboSelectedForObjectInventProductList(customer);
                SetComboSelectedForObjectInventProduct(customer);
                SetPriceComboByStringValue(customer.PriceCode);
            }
            else
            {
                SetComboSelectedForObjectInventProductList(_branch);
                SetComboSelectedForObjectInventProduct(_branch);
                SetPriceComboByStringValue(_branch.PriceCode);
            }

			if (customer == null) return;
			string complexAdapterCode = customer.ComplexAdapterCode;
			if (!String.IsNullOrEmpty(complexAdapterCode) && ItemsComplex.Any(r => r.Name == complexAdapterCode))
				_selectedComplex = ItemsComplex.FirstOrDefault(r => r.Name == complexAdapterCode);
			else
				_selectedComplex = ItemsComplex.FirstOrDefault(r => r.IsDefault);
        }

        public void SetStateForInventor(enInventorAdapterInherit mode)
        {
            Customer customer = this.CurrentCustomer;
            Branch branch = this.CurrentBranch;
	        if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
            {
                SetComboSelectedForObjectInventProductList(customer);
                SetComboSelectedForObjectInventProduct(customer);
                SetPriceComboByStringValue(customer.PriceCode);
            }
            else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
			{
               SetComboSelectedForObjectInventProductList(branch);
                SetComboSelectedForObjectInventProduct(branch);
                SetPriceComboByStringValue(branch.PriceCode);
            }
            else
            {
                SetComboSelectedForObjectInventProductList(_inventor);
                SetComboSelectedForObjectInventProduct(_inventor);
                SetPriceComboByStringValue(_inventor.PriceCode);
            }

			if (customer == null) return;
			string complexAdapterCode = customer.ComplexAdapterCode;
			if (!String.IsNullOrEmpty(complexAdapterCode) && ItemsComplex.Any(r => r.Name == complexAdapterCode))
				_selectedComplex = ItemsComplex.FirstOrDefault(r => r.Name == complexAdapterCode);
			else
				_selectedComplex = ItemsComplex.FirstOrDefault(r => r.IsDefault);
        }

        public void ApplyChangesNonDynColumns()
        {
            string priceCode = String.Empty;
            if (_selectedPriceType != null)
            {
                priceCode = _selectedPriceType.En.ToString();
            }

            if (this._customer != null)
            {
                _customer.PriceCode = priceCode;
				this._customer.ComplexAdapterCode = this.SelectedComplex == null ?
													String.Empty :
													this.SelectedComplex.Name;

				this._customer.ImportCatalogPath = "";
				if (string.IsNullOrWhiteSpace(this.ComplexStaticPath1) == false)
				{
					string complexstaticPath1 = this.ComplexStaticPath1;
					if (this.ComplexStaticPath1.Length >= 100) complexstaticPath1 = this.ComplexStaticPath1.Substring(0, 99);
					this._customer.ImportCatalogPath = complexstaticPath1;	 //Только с путем в customer работаем
				}

				this._customer.ExportErpPath = "";
				if (string.IsNullOrWhiteSpace(this.ComplexStaticPath2) == false)
				{
					//this._complexStaticPath2 = this._customer.MaskCode;
					string complexstaticPath2 = this.ComplexStaticPath2;
					if (this.ComplexStaticPath2.Length >= 100) complexstaticPath2 = this.ComplexStaticPath2.Substring(0, 99);
					this._customer.ExportErpPath = complexstaticPath2;	 //Только с путем в customer работаем
				}

				this._customer.SendToOfficePath = "";
				if (string.IsNullOrWhiteSpace(this.ComplexStaticPath3) == false)
				{
					//this._complexStaticPath3 = this._customer.ReportPath;
					string complexstaticPath3 = this.ComplexStaticPath3;
					if (this.ComplexStaticPath3.Length >= 100) complexstaticPath3 = this.ComplexStaticPath3.Substring(0, 99);
					this._customer.SendToOfficePath = complexstaticPath3;	 //Только с путем в customer работаем
				}
            }

            if (this._branch != null)
            {
                _branch.PriceCode = priceCode;
				this._branch.ComplexAdapterCode = this.SelectedComplex == null ?
															String.Empty :
															this.SelectedComplex.Name;
            }

            if (this._inventor != null)
            {
                _inventor.PriceCode = priceCode;
				this._inventor.ComplexAdapterCode = this.SelectedComplex == null ?
															String.Empty :
															this.SelectedComplex.Name;
            }
        }

        public void ApplyChanges()
        {
            EditorTemplate editorTemplateInventProductListView = _selectedInventProductList == null ? null : _selectedInventProductList.EditorTemplate;
            EditorTemplate editorTemplateInventProduct = _selectedInventProduct == null ? null : _selectedInventProduct.EditorTemplate;

            object cbiObject = null;

            if (this._customer != null)
            {
				this._customer.ComplexAdapterCode = this.SelectedComplex == null ?
													String.Empty :
													this.SelectedComplex.Name;
                cbiObject = _customer;
			
            }

            if (this._branch != null)
            {
				this._branch.ComplexAdapterCode = this.SelectedComplex == null ?
											String.Empty :
											this.SelectedComplex.Name;
                cbiObject = _branch;
				
            }

            if (this._inventor != null)
            {
				this._inventor.ComplexAdapterCode = this.SelectedComplex == null ?
									String.Empty :
									this.SelectedComplex.Name;
                cbiObject = _inventor;
			
            }

            if (editorTemplateInventProductListView != null && cbiObject != null)
                SaveCurrentForObject(cbiObject, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, editorTemplateInventProductListView);

            if (editorTemplateInventProduct != null && cbiObject != null)
                SaveCurrentForObject(cbiObject, Configuration.Constants.DynamicView.InventProductView_InventProduct, editorTemplateInventProduct);
        }

        private void SaveCurrentForObject(object cbiObject, string viewName, EditorTemplate editorTemplate)
        {
            if (base.ContextCBIRepository.CheckDbPath(cbiObject))
                _editorTemplateRepository.SetCurrent(viewName, base.ContextCBIRepository.GetDBPath(cbiObject), editorTemplate);
        }

        private void SetComboSelectedForObject(object cbiObject, string viewName, ObservableCollection<DynamicColumnSettingsItem> combo, PropertyInfo piSelected)
        {
            if (base.ContextCBIRepository.CheckDbPath(cbiObject))
            {
                EditorTemplate editorTemplate = _editorTemplateRepository.GetCurrent(viewName, base.ContextCBIRepository.GetDBPath(cbiObject));
                if (editorTemplate != null)
                {
                    string code = editorTemplate.Code;
                    if (combo.Any(r => r.EditorTemplate.Code == code))
                        piSelected.SetValue(this, combo.FirstOrDefault(r => r.EditorTemplate.Code == code), null);
                }
            }
            else
            {
                piSelected.SetValue(this, combo.FirstOrDefault(), null);
            }
        }

        private void SetComboSelectedForObjectInventProductList(object cbiObject)
        {
            SetComboSelectedForObject(cbiObject, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, _itemsInventProductList, PropertyInfoInventProductList);
        }

        private void SetComboSelectedForObjectInventProduct(object cbiObject)
        {
            SetComboSelectedForObject(cbiObject, Configuration.Constants.DynamicView.InventProductView_InventProduct, _itemsInventProduct, PropertyInfoInventProduct);
        }

        private void SetPriceComboByStringValue(string value)
        {
            PriceCodeEnum en;

            if (Enum.TryParse(value, true, out en))
            {
                SelectedPriceType = _itemsPriceType.FirstOrDefault(r => r.En == en);
            }
            else
            {
                SelectedPriceType = _itemsPriceType.FirstOrDefault();
            }
        }

        private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
        {
            if (payload == null || payload.ImportModule == null)
                return;

            if (payload.ImportModule.Name == ImportAdapterName.ImportCatalogAS400_LeumitAdapter ||
                payload.ImportModule.Name == ImportAdapterName.ImportCatalogYarpaAdapter ||
                payload.ImportModule.Name == ImportAdapterName.ImportCatalogYarpaWindowsAdapter
                )
            {
                SelectedInventProductList = _itemsInventProductList.FirstOrDefault(r => r.EditorTemplate.Code == "InventProduct-QuantityPartial");
                SelectedInventProduct = _itemsInventProduct.FirstOrDefault(r => r.EditorTemplate.Code == "InventProduct-QuantityPartial");
            }

        }
    }
}