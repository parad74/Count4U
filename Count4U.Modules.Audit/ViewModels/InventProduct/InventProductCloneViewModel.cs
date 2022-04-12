using System.ComponentModel;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using System;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Count4U.Common.Extensions;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductCloneViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;

        private readonly IInventProductRepository _inventProductRepository;
		private readonly IIturRepository _iturRepository;

        private string _makat;
        private string _quantity;
        private double _quantityBefore;
        private double _quantityAfter;
        private InventProduct _inventProductOriginal;
        private readonly IProductRepository _productRepository;
        private readonly IMakatRepository _makatRepository;
        private Dictionary<string, ProductMakat> _makatDictionary;
        private bool _isPartialVisible;
        private bool _isInitComplite;

        public InventProductCloneViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IMakatRepository makatRepository,
             IProductRepository productRepository,
            IInventProductRepository inventProductRepository,
			IIturRepository iturRepository)
            : base(contextCBIRepository)
        {
            this._productRepository = productRepository;
            this._makatRepository = makatRepository;
            this._inventProductRepository = inventProductRepository;
			this._iturRepository = iturRepository;
            this._eventAggregator = eventAggregator;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._isInitComplite = true;
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
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

        public string Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                RaisePropertyChanged(() => Quantity);
                
                QuantityAfter = CalculateQuantityAfter();
                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public double QuantityBefore
        {
            get { return _quantityBefore; }
            set
            {
                _quantityBefore = value;
                RaisePropertyChanged(() => QuantityBefore);
            }
        }

        public bool IsPartialVisible
        {
            get { return _isPartialVisible; }
            set
            {
                _isPartialVisible = value;
                RaisePropertyChanged(() => IsPartialVisible);
            }
        }

        public double QuantityAfter
        {
            get { return _quantityAfter; }
            set
            {
                _quantityAfter = value;
                RaisePropertyChanged(() => QuantityAfter);
            }
        }

        public object ResultData { get; set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            long id = Int64.Parse(navigationContext.Parameters[Common.NavigationSettings.InventProductId]);

            this._inventProductOriginal = _inventProductRepository.GetInventProductByID(id, base.GetDbPath);

            this._makat = this._inventProductOriginal.Makat;
            this._quantity = "0";
            this._quantityBefore = this._inventProductRepository.GetSumQuantityEditByMakat(this._inventProductOriginal.Makat, base.GetDbPath);

            this._isInitComplite = false;
            Task.Factory.StartNew(() =>
            {
                this._makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
                this._isInitComplite = true;
                _okCommand.RaiseCanExecuteChanged();
			}).LogTaskFactoryExceptions("OnNavigatedTo");

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Quantity":
                        if (!IsQuantityValid())
                        {
                            return Localization.Resources.Bit_InventProduct_QuantityEditFormat;
                        }
                        if (IsQuantityAfterValid() == false)
                        {
                            return Localization.Resources.ViewModel_InventProductClone_quantityValidation;
                        }
                        break;
                }
                return String.Empty;
            }
        }

        public string Error { get; private set; }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            InventProduct newInventProduct = this.FillNewObjectFromFormPropertyAndOriginlObject(this._inventProductOriginal);
            if (newInventProduct == null) return;

			newInventProduct.CreateDate = DateTime.Now;
			newInventProduct.InputTypeCode = InputTypeCodeEnum.K.ToString();
			int maxIPNum = this._inventProductRepository.GetMaxNumForDocumentCode(newInventProduct.DocumentHeaderCode, base.GetDbPath);
			newInventProduct.IPNum = maxIPNum + 1;
			newInventProduct.Code = (newInventProduct.Makat + "^" + newInventProduct.Barcode).CutLength(299); 
            this._inventProductRepository.Insert(newInventProduct, base.GetDbPath);
			this._iturRepository.RefillApproveStatusBit(newInventProduct.IturCode, new List<string>() { newInventProduct.DocumentHeaderCode }, base.GetDbPath);

            this.ResultData = newInventProduct;

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return (String.IsNullOrWhiteSpace(this._quantity) == false)
                   && (IsQuantityValid() == true)
                   && (this._isInitComplite == true
                   && IsQuantityAfterValid() == true);

        }

        private InventProduct FillNewObjectFromFormPropertyAndOriginlObject(InventProduct originalInventProduct)
        {
            InventProduct newInventProduct = new InventProduct();
            if (String.IsNullOrWhiteSpace(this._makat) == false)
            {
                if (this._makatDictionary.ContainsKey(this._makat) == true)
                {
                    ProductMakat productMakat = this._makatDictionary[this._makat];
                    newInventProduct.Makat = productMakat.Makat;
                    newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
                }
                else
                {
                    newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
                    newInventProduct.Makat = this._makat;
                    newInventProduct.Barcode = this._makat;
                    newInventProduct.ProductName = "NotExistInCatalog";
                    newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
                }
            }

            double quantityEdit = 0;
            Double.TryParse(this._quantity, out quantityEdit);
            newInventProduct.QuantityEdit = quantityEdit;
            newInventProduct.QuantityOriginal = quantityEdit;
            newInventProduct.QuantityInPackEdit = 0;
            newInventProduct.QuantityDifference = newInventProduct.QuantityEdit - newInventProduct.QuantityOriginal;
            this._quantity = newInventProduct.QuantityEdit.ToString();

            this._quantity = quantityEdit.ToString();
            RaisePropertyChanged(() => Quantity);

            newInventProduct.ID = 0;
            newInventProduct.InputTypeCode = InputTypeCodeEnum.K.ToString();

            newInventProduct.Barcode = originalInventProduct.Barcode;
			newInventProduct.Code = Utils.CodeNewGenerate();//newInventProduct.Makat + "^" + newInventProduct.Barcode;
            newInventProduct.ProductName = originalInventProduct.ProductName;
            newInventProduct.TypeMakat = originalInventProduct.TypeMakat;
            newInventProduct.DocumentHeaderCode = originalInventProduct.DocumentHeaderCode;
            newInventProduct.DocumentCode = originalInventProduct.DocumentCode;
            newInventProduct.IturCode = originalInventProduct.IturCode;
            int maxIPNum = this._inventProductRepository.GetMaxNumForDocumentCode(newInventProduct.DocumentHeaderCode, base.GetDbPath);
            newInventProduct.IPNum = maxIPNum + 1;
            newInventProduct.DocNum = originalInventProduct.DocNum;
            newInventProduct.SectionCode = originalInventProduct.SectionCode;
            newInventProduct.SectionNum = originalInventProduct.SectionNum;
            newInventProduct.SessionCode = originalInventProduct.SessionCode;
            newInventProduct.StatusInventProductBit = originalInventProduct.StatusInventProductBit;

            return newInventProduct;
        }

        private bool IsQuantityValid()
        {
            double dummy;
            return Double.TryParse(_quantity, out dummy);
        }

        private double CalculateQuantityAfter()
        {
            double result = 0;
            double parsed;
            if (Double.TryParse(_quantity, out parsed))
            {
                result = _quantityBefore + parsed;                
            }
            else
            {
                result = 0;
            }

            return result;
        }

        private bool IsQuantityAfterValid()
        {
            return CalculateQuantityAfter() >= 0;
        }
    }
}