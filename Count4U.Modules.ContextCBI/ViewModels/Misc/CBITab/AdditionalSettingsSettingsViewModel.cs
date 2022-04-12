using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.Interfaces;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
	public class AdditionalSettingsSettingsViewModel : CBIContextBaseViewModel
    {
        private readonly IEditorTemplateRepository _editorTemplateRepository;
        private readonly IEventAggregator _eventAggregator;

        private bool _isEditable;

        private readonly ObservableCollection<DynamicColumnSettingsItem> _itemsInventProductList;
        private DynamicColumnSettingsItem _selectedInventProductList;
        private readonly ObservableCollection<DynamicColumnSettingsItem> _itemsInventProduct;
        private DynamicColumnSettingsItem _selectedInventProduct;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        private readonly ObservableCollection<PriceTypeItemViewModel> _itemsPriceType;
        private PriceTypeItemViewModel _selectedPriceType;

        public AdditionalSettingsSettingsViewModel(
            IContextCBIRepository contextCbiRepository,
            IEditorTemplateRepository editorTemplateRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _editorTemplateRepository = editorTemplateRepository;
            _itemsInventProductList = new ObservableCollection<DynamicColumnSettingsItem>();
            _itemsInventProduct = new ObservableCollection<DynamicColumnSettingsItem>();
			//_itemsPriceType = new ObservableCollection<PriceTypeItemViewModel>()
			//	{
			//		new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceBuy, Title = PriceCodeCaption.PriceBuy},
			//		new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceSale, Title = PriceCodeCaption.PriceSale},
			//		new PriceTypeItemViewModel() {En = PriceCodeEnum.PriceExtra, Title = PriceCodeCaption.PriceExtra},
			//	};

            _isEditable = true;
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

			//_eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterChanged);

			//FillComboDefault(_itemsInventProductList, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, PropertyInfoInventProductList);
			//FillComboDefault(_itemsInventProduct, Configuration.Constants.DynamicView.InventProductView_InventProduct, PropertyInfoInventProduct);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

			//_eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);
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
			//foreach (EditorTemplate editorTemplate in _editorTemplateRepository.Get(viewName))
			//{
			//	combo.Add(new DynamicColumnSettingsItem(editorTemplate));
			//}

			//piSelected.SetValue(this, combo.FirstOrDefault(), null);
        }

        public void SetIsEditable(bool isEditable)
        {
			this.IsEditable = isEditable;
        }

      
        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
			//this._branch = branch;

			//SetStateForBranch(mode);
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
			//this._inventor = inventor;

			//SetStateForInventor(mode);

        }

		public void SetCustomer(Customer customer)
		{
			//this._customer = customer;
	   	}

        public void SetStateForBranch(enBranchAdapterInherit mode)
        {
			//Customer customer = this.CurrentCustomer;

			//if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
			//{
			//	SetComboSelectedForObjectInventProductList(customer);
			//	SetComboSelectedForObjectInventProduct(customer);

			//	SetPriceComboByStringValue(customer.PriceCode);
			//}
			//else
			//{
			//	SetComboSelectedForObjectInventProductList(_branch);
			//	SetComboSelectedForObjectInventProduct(_branch);
			//	SetPriceComboByStringValue(_branch.PriceCode);
			//}
        }

        public void SetStateForInventor(enInventorAdapterInherit mode)
        {
			//Customer customer = this.CurrentCustomer;
			//Branch branch = this.CurrentBranch;

			//if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
			//{
			//	SetComboSelectedForObjectInventProductList(customer);
			//	SetComboSelectedForObjectInventProduct(customer);
			//	SetPriceComboByStringValue(customer.PriceCode);
			//}
			//else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
			//{
			//	SetComboSelectedForObjectInventProductList(branch);
			//	SetComboSelectedForObjectInventProduct(branch);
			//	SetPriceComboByStringValue(branch.PriceCode);
			//}
			//else
			//{
			//	SetComboSelectedForObjectInventProductList(_inventor);
			//	SetComboSelectedForObjectInventProduct(_inventor);
			//	SetPriceComboByStringValue(_inventor.PriceCode);
			//}
        }

        public void ApplyChangesNonDynColumns()
        {
			//string priceCode = String.Empty;
			//if (_selectedPriceType != null)
			//{
			//	priceCode = _selectedPriceType.En.ToString();
			//}

			//if (this._customer != null)
			//{
			//	_customer.PriceCode = priceCode;
			//}

			//if (this._branch != null)
			//{
			//	_branch.PriceCode = priceCode;
			//}

			//if (this._inventor != null)
			//{
			//	_inventor.PriceCode = priceCode;
			//}
        }

        public void ApplyChanges()
        {
			//EditorTemplate editorTemplateInventProductListView = _selectedInventProductList == null ? null : _selectedInventProductList.EditorTemplate;
			//EditorTemplate editorTemplateInventProduct = _selectedInventProduct == null ? null : _selectedInventProduct.EditorTemplate;

			//object cbiObject = null;

			//if (this._customer != null)
			//{
			//	cbiObject = _customer;
			//}

			//if (this._branch != null)
			//{
			//	cbiObject = _branch;
			//}

			//if (this._inventor != null)
			//{
			//	cbiObject = _inventor;
			//}

			//if (editorTemplateInventProductListView != null && cbiObject != null)
			//	SaveCurrentForObject(cbiObject, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, editorTemplateInventProductListView);

			//if (editorTemplateInventProduct != null && cbiObject != null)
			//	SaveCurrentForObject(cbiObject, Configuration.Constants.DynamicView.InventProductView_InventProduct, editorTemplateInventProduct);
        }

        private void SaveCurrentForObject(object cbiObject, string viewName, EditorTemplate editorTemplate)
        {
			//if (base.ContextCBIRepository.CheckDbPath(cbiObject))
			//	_editorTemplateRepository.SetCurrent(viewName, base.ContextCBIRepository.GetDBPath(cbiObject), editorTemplate);
        }

        private void SetComboSelectedForObject(object cbiObject, string viewName, ObservableCollection<DynamicColumnSettingsItem> combo, PropertyInfo piSelected)
        {
			//if (base.ContextCBIRepository.CheckDbPath(cbiObject))
			//{
			//	EditorTemplate editorTemplate = _editorTemplateRepository.GetCurrent(viewName, base.ContextCBIRepository.GetDBPath(cbiObject));
			//	if (editorTemplate != null)
			//	{
			//		string code = editorTemplate.Code;
			//		if (combo.Any(r => r.EditorTemplate.Code == code))
			//			piSelected.SetValue(this, combo.FirstOrDefault(r => r.EditorTemplate.Code == code), null);
			//	}
			//}
			//else
			//{
			//	piSelected.SetValue(this, combo.FirstOrDefault(), null);
			//}
        }

        private void SetComboSelectedForObjectInventProductList(object cbiObject)
        {
			//SetComboSelectedForObject(cbiObject, Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, _itemsInventProductList, PropertyInfoInventProductList);
        }

        private void SetComboSelectedForObjectInventProduct(object cbiObject)
        {
			//SetComboSelectedForObject(cbiObject, Configuration.Constants.DynamicView.InventProductView_InventProduct, _itemsInventProduct, PropertyInfoInventProduct);
        }

        private void SetPriceComboByStringValue(string value)
        {
			//PriceCodeEnum en;

			//if (Enum.TryParse(value, true, out en))
			//{
			//	SelectedPriceType = _itemsPriceType.FirstOrDefault(r => r.En == en);
			//}
			//else
			//{
			//	SelectedPriceType = _itemsPriceType.FirstOrDefault();
			//}
        }

        private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
        {
			//if (payload == null || payload.ImportModule == null)
			//	return;

			//if (payload.ImportModule.Name == ImportAdapterName.ImportCatalogAS400_LeumitAdapter ||
			//	payload.ImportModule.Name == ImportAdapterName.ImportCatalogYarpaAdapter ||
			//	payload.ImportModule.Name == ImportAdapterName.ImportCatalogYarpaWindowsAdapter
			//	)
			//{
			//	SelectedInventProductList = _itemsInventProductList.FirstOrDefault(r => r.EditorTemplate.Code == "InventProduct-QuantityPartial");
			//	SelectedInventProduct = _itemsInventProduct.FirstOrDefault(r => r.EditorTemplate.Code == "InventProduct-QuantityPartial");
			//}

        }
    }
}