using System;
using System.Collections.Generic;
using System.ComponentModel;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using System.Linq;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Threading.Tasks;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels.Catalog
{
    public class ProductAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        //        private string _barcode;
        private string _makat;
        private string _priceSale;
        private string _priceBuy;
        private string _priceString;
        private string _name;
        private bool _isNew;
		private bool _isInitComplite;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _validateCommand;

        private Product _product;
        private readonly IProductRepository _productRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMakatRepository _makatRepository;
        private Dictionary<string, ProductMakat> _makatDictionary;

        private bool _isTimerEnabled;

        private bool _isFormValidated;

        private string _validationSummary;

        public ProductAddEditViewModel(IContextCBIRepository contextCBIRepository,
              IProductRepository productRepository, IEventAggregator eventAggregator,
              IMakatRepository makatRepository)
            : base(contextCBIRepository)
        {
            this._makatRepository = makatRepository;
            this._eventAggregator = eventAggregator;
            this._productRepository = productRepository;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._validateCommand = new DelegateCommand(ValidateCommandExecuted, ValidateCommandCanExecute);

            this._isFormValidated = false;
			this._isInitComplite = true;
        }

        //        public string Barcode
        //        {
        //            get { return this._barcode; }
        //            set
        //            {
        //                this._barcode = value;
        //                this.RaisePropertyChanged(() => this.Barcode);
        //
        //                this._okCommand.RaiseCanExecuteChanged();
        //            }
        //        }

        public string Makat
        {
            get { return this._makat; }
            set
            {
                this._makat = value;
                this.RaisePropertyChanged(() => this.Makat);

                this._validateCommand.RaiseCanExecuteChanged();
                this._isFormValidated = false;
                this._okCommand.RaiseCanExecuteChanged();
                this.ValidationSummary = String.Empty;
            }
        }

        public string PriceSale
        {
            get { return this._priceSale; }
            set
            {
                this._priceSale = value;
                this.RaisePropertyChanged(() => this.PriceSale);
                
                this._validateCommand.RaiseCanExecuteChanged();
                this._isFormValidated = false;
                this._okCommand.RaiseCanExecuteChanged();
                this.ValidationSummary = String.Empty;
            }
        }

        public string PriceBuy
        {
            get { return this._priceBuy; }
            set
            {
                this._priceBuy = value;
                this.RaisePropertyChanged(() => this.PriceBuy);
                
                this._validateCommand.RaiseCanExecuteChanged();
                this._isFormValidated = false;
                this._okCommand.RaiseCanExecuteChanged();
                this.ValidationSummary = String.Empty;
            }
        }

        public string PriceString
        {
            get { return this._priceString; }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public bool IsNew
        {
            get { return this._isNew; }
        }

        public bool IsEdit
        {
            get { return !this._isNew; }
        }

        public bool IsTimerEnabled
        {
            get { return _isTimerEnabled; }
            set
            {
                _isTimerEnabled = value;
                RaisePropertyChanged(() => IsTimerEnabled);
            }
        }

        public DelegateCommand ValidateCommand
        {
            get { return _validateCommand; }
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        public string ValidationSummary
        {
            get { return _validationSummary; }
            set
            {
                _validationSummary = value;
                RaisePropertyChanged(() => ValidationSummary);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);


            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ProductMakat)) //edit
            {
                string productMakat = navigationContext.Parameters.FirstOrDefault(r => r.Key == Common.NavigationSettings.ProductMakat).Value;
                this._product = this._productRepository.GetProductByMakat(productMakat, base.GetDbPath);
	
				this.FillFormWithObjectProperties(this._product);

                this._isNew = false;
            }
            else //new
            {
                this._product = new Product();

                this._isNew = true;
				this._isInitComplite = false;
				Task.Factory.StartNew(() =>
				{
					this._makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
				}).LogTaskFactoryExceptions("OnNavigatedTo");
				this._isInitComplite = true;
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string propName]
        {
            get
            {
                switch (propName)
                {
                    case "Makat":

                       string validation = ProductValidate.MakatValidate(this._makat);
                        if (String.IsNullOrEmpty(validation) == false)	return validation;

                        //                        if (!IsMakatUnique())
                        //                            return Model.ValidateMessage.Product.MakatIsNotUnique;
                        break;

                    case "PriceSale":
                        return ProductValidate.PriceSaleValidate(this._priceSale);

                    case "PriceBuy":
                        return ProductValidate.PriceSaleValidate(this._priceBuy);
                }

                return String.Empty;
            }
        }

        private void FillFormWithObjectProperties(Product product, bool notifyUI = false)
        {
            // this._barcode = this._product.Barcode;
            this._makat = product.Makat;
            this._priceSale = String.Format("{0:F}", product.PriceSale);
            this._priceBuy = String.Format("{0:F}", product.PriceBuy);
            //this._priceString = product.PriceString;
            this._name = product.Name;

            if (notifyUI == true)
            {
                this.IsTimerEnabled = false;
                RaisePropertyChanged(() => Makat);
                RaisePropertyChanged(() => PriceSale);
                RaisePropertyChanged(() => PriceBuy);
               // RaisePropertyChanged(() => PriceString);
                RaisePropertyChanged(() => Name);
                this.IsTimerEnabled = true;
            }
        }

        private void FillObjectWithFormProperties(Product product)
        {
            //            this._product.Barcode = this._barcode;
			product.Makat = this._makat.CutLength1(299);
			product.MakatOriginal = this._makat.CutLength1(299);
			product.Name = this._name.CutLength1(99);
            product.PriceSale = Convert.ToDouble(this._priceSale);
			product.PriceString = this._priceSale;
            product.PriceBuy = Convert.ToDouble(this._priceBuy);
			product.TypeCode = TypeMakatEnum.M.ToString();
			product.InputTypeCode = InputTypeCodeEnum.K.ToString();
        }

		private void FillNewObjectWithFormProperties(Product product)
		{
			//            this._product.Barcode = this._barcode;
			product.Makat = this._makat.CutLength1(299);
			product.MakatOriginal = this._makat.CutLength1(299);
			product.Name = this._name.CutLength1(99); 
			product.PriceSale = Convert.ToDouble(this._priceSale);
			product.PriceString = this._priceSale;
			product.PriceBuy = Convert.ToDouble(this._priceBuy);
			product.TypeCode = TypeMakatEnum.M.ToString();
			product.InputTypeCode = InputTypeCodeEnum.K.ToString();
			product.ParentMakat = "";
			product.SectionCode = "";
			product.SupplierCode = "";
			product.UnitTypeCode = "";
			product.IturCodeExpected = "";
		}

        private bool OkCommandCanExecute()
        {
            return this._isFormValidated == true;
        }

        private void OkCommandExecuted()
        {
            if (OkCommandCanExecute() == false)	 return;

            this._product.ModifyDate = DateTime.Now;

            if (this.IsNew == true)
            {
				this.FillNewObjectWithFormProperties(this._product);
                this._product.CreateDate = DateTime.Now;
                this._productRepository.Insert(this._product, base.GetDbPath);
            }
            else
            {
				this.FillObjectWithFormProperties(this._product);
                this._productRepository.Update(this._product, base.GetDbPath);
            }

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
            this._eventAggregator.GetEvent<ProductAddedEditedEvent>().Publish(new ProductAddedEditedEventPayload() { IsNew = this.IsNew, Product = this._product });
        }

        private bool ValidateCommandCanExecute()
        {
            return (String.IsNullOrEmpty(this._makat) == false)
					&& this._isInitComplite == true  /*IsMakatUnique() &&*/
                  && UtilsConvert.IsOkAsDouble(this._priceSale) 
				  && UtilsConvert.IsOkAsDouble(this._priceBuy);
        }

        private void ValidateCommandExecuted()
        {
            Product clone = UtilsConvert.CreateClone(this._product) as Product;
            FillObjectWithFormProperties(clone);

            //run some validation on clone

            int bitValidate;
			if (this._isNew == true)
			{
				bitValidate = DummyValidateAdd(clone);
			}
			else
			{
				bitValidate = DummyValidateEdit(clone);
			}
            
            if (bitValidate == 0)
            {
                //or all is ok
                this._product = clone;
                this.FillFormWithObjectProperties(this._product, true);

                this.ValidationSummary = String.Empty;
                this._isFormValidated = true;                
            }
            else
            {
                this.ValidationSummary = String.Join(Environment.NewLine, Bit2List.GetStatusList(bitValidate, DomainStatusEnum.PDA));
                this._isFormValidated = false;
            }


            this._okCommand.RaiseCanExecuteChanged();
        }

        private int DummyValidateAdd(Product product)
        {
			Product newProduct =  new Product();
			//Dictionary<string, ProductMakat>  makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
			int retBit = newProduct.ValidateError(product, this._makatDictionary);
            //product.Name = "Add";
			return retBit;
        }

        private int DummyValidateEdit(Product product)
        {
			//TODO:
			return 0;
        }

        private string DummyValidateStringFromBit(int bit)
        {

			string msg = String.Join(Environment.NewLine, Bit2List.GetStatusList
				(bit, DomainStatusEnum.PDA));
			return msg;
			//if (bit == 1)
			//    //ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
			//return "Some error occurred";

			//return "Some other error occurred";
        }

        private bool IsMakatUnique()
        {
            if (this.IsNew)
            {
				if (this._makatDictionary == null)
				{
					this._makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
				}

                return !this._makatDictionary.Any(r => r.Key == this._makat);
            }

            return true;
        }
    }
}