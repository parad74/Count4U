using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.Interfaces;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Threading.Tasks;
using Count4U.Common.Extensions;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IInventProductRepository _inventProductRepository;
        private readonly IDocumentHeaderRepository _documentHeaderRepository;
        private readonly IProductRepository _productRepository;
		private readonly IIturRepository _iturRepository;
        private readonly IMakatRepository _makatRepository;
        private readonly DynamicRepository _dynamicRepository;
        private readonly IEditorTemplateRepository _editorTemplateRepository;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _undoCommand;
        private readonly DelegateCommand _validateCommand;
        
        private Dictionary<string, ProductMakat> _makatDictionary;
		//private Dictionary<string, ProductMakat> _barcodeDictionary;

        private string _barcode;
        private string _makat;
        private string _statusInventProductCode;
        private string _statusBit;
        private string _quantityEdit;
        private string _quantityOriginal;
        private string _quantityPartialEdit;
        private string _serialNumber;
        private string _productName;
		private string _typeMakat;
		private int _fromCatalogType;
        private InventProduct _inventProduct;
        private bool _isNewMode;
		private bool _isInitComplite;

        private bool _isTimerEnabled;

        private bool _isFormValidated;
        private string _validationSummary;
        private bool _isPartialVisible;

		private string _remark;

        public InventProductAddEditViewModel(IEventAggregator eventAggregator,
            IInventProductRepository inventProductRepository,
            IContextCBIRepository contextCBIRepository,
            IDocumentHeaderRepository documentHeaderRepository,
            IMakatRepository makatRepository,
            IProductRepository productRepository,
            DynamicRepository dynamicRepository,
            IEditorTemplateRepository editorTemplateRepository,
			IIturRepository iturRepository)
            : base(contextCBIRepository)
        {
            this._editorTemplateRepository = editorTemplateRepository;
			this._dynamicRepository = dynamicRepository;
            this._productRepository = productRepository;
			this._iturRepository = iturRepository;
            this._makatRepository = makatRepository;
            this._documentHeaderRepository = documentHeaderRepository;
            this._inventProductRepository = inventProductRepository;
            this._eventAggregator = eventAggregator;

            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(SaveCommandExecuted, SaveCommandCanExecute);
            this._undoCommand = new DelegateCommand(UndoCommandExecuted);
            this._validateCommand = new DelegateCommand(ValidateCommandExecuted, ValidateCommandCanExecute);

            this._isTimerEnabled = false;
			this._isInitComplite = true;
        }

        public string Barcode
        {
            get
            {
                return this._barcode;
            }
            set
            {
                this._barcode = value;
                this.RaisePropertyChanged(() => this.Barcode);
            }
        }

        public string ProductName
        {
            get { return this._productName; }
            set
            {
                this._productName = value;
                this.RaisePropertyChanged(() => this.ProductName);			
            }
        }

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

        public string StatusInventProductCode
        {
            get { return this._statusInventProductCode; }
            set
            {
                this._statusInventProductCode = value;
                this.RaisePropertyChanged(() => this.StatusInventProductCode);
            }
        }

        public string QuantityEdit
        {
            get { return this._quantityEdit; }
            set
            {
                this._quantityEdit = value;
                this.RaisePropertyChanged(() => this.QuantityEdit);

              this._validateCommand.RaiseCanExecuteChanged();
                this._isFormValidated = false;
                if (_validateCommand.CanExecute())
                {
                    _validateCommand.Execute();
                }                
                this._okCommand.RaiseCanExecuteChanged();
                this.ValidationSummary = String.Empty;
            }
        }

        public string QuantityPartialEdit
        {
            get { return _quantityPartialEdit; }
            set
            {
                _quantityPartialEdit = value;
                RaisePropertyChanged(() => QuantityPartialEdit);

                this._validateCommand.RaiseCanExecuteChanged();
                this._isFormValidated = false;
                this._okCommand.RaiseCanExecuteChanged();
                this.ValidationSummary = String.Empty;
            }
        }

        public string QuantityOriginal
        {
            get { return this._quantityOriginal; }
            set
            {
                this._quantityOriginal = value;
                this.RaisePropertyChanged(() => this.QuantityOriginal);				
            }
        }

        public string SerialNumber
        {
            get { return this._serialNumber; }
            set
            {
                this._serialNumber = value;
                this.RaisePropertyChanged(() => this.SerialNumber);
            }
        }

		public string TypeMakat
		{
			get { return this._typeMakat; }
			set
			{
				this._typeMakat = value;
				this.RaisePropertyChanged(() => this.TypeMakat);
			}
		}

		public int FromCatalogType
		{
			get { return this._fromCatalogType; }
			set
			{
				this._fromCatalogType = value;
				this.RaisePropertyChanged(() => this.FromCatalogType);
			}
		}

        public bool IsNewMode
        {
            get { return this._isNewMode; }
            set { this._isNewMode = value; }
        }

        public bool IsEditMode
        {
            get { return !this._isNewMode; }
        }

		public string Remark
		{
			get { return this._remark; }
			set
			{
				this._remark = value;
				RaisePropertyChanged(() => Remark);
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

        public DelegateCommand UndoCommand
        {
            get { return _undoCommand; }
        }

        public string StatusBit
        {
            get { return _statusBit; }
            set
            {
                this._statusBit = value;
                RaisePropertyChanged(() => StatusBit);
            }
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

        public string ValidationSummary
        {
            get { return _validationSummary; }
            set
            {
                _validationSummary = value;
                RaisePropertyChanged(() => ValidationSummary);
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._dynamicRepository.CBIState = base.State;
			this._dynamicRepository.EditorTemplate = this._editorTemplateRepository.GetCurrent(Configuration.Constants.DynamicView.InventProductListDetailsView_InventProduct, base.GetDbPath);
			this._isPartialVisible = this._dynamicRepository.IsDynamicPropertyExist(TypedReflection<Count4U.Model.Count4U.InventProduct>.GetPropertyInfo(r => r.QuantityInPackEdit).Name);                                                                          

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.InventProductId))	 //edit
            {
                string strId = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.InventProductId).Value;
                int id;
                if (Int32.TryParse(strId, out id) == true)
                {
                    this._inventProduct = this._inventProductRepository.GetInventProductByID(id, base.GetDbPath);
                }

				this.FillFormWithObjectProperties(this._inventProduct);

                this.IsNewMode = false;
                this._isFormValidated = true;
            }
            else //add
            {
                string documentCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.DocumentCode).Value;
                DocumentHeader documentHeader = this._documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);

                if (documentHeader == null)
                {
                    this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
                    return;
                }

                this._inventProduct = new InventProduct();
                this._inventProduct.ID = 0;
                this._inventProduct.Code = Utils.CodeNewGenerate();
                this._inventProduct.Makat = String.Empty;
				this._inventProduct.Barcode = String.Empty;
				this._inventProduct.QuantityEdit = 0.0;
				this._inventProduct.QuantityOriginal = 0.0;
                this._inventProduct.QuantityInPackEdit = 0;

                this._inventProduct.DocumentHeaderCode = documentHeader.DocumentCode;
                this._inventProduct.DocumentCode = documentHeader.DocumentCode;
                this._inventProduct.IturCode = documentHeader.IturCode;

                this.IsNewMode = true;
				this._isInitComplite = false;
				Task.Factory.StartNew(() =>
				{
					this._makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
					//this._barcodeDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
                    this._isInitComplite = true;
                    _okCommand.RaiseCanExecuteChanged();
				}).LogTaskFactoryExceptions("OnNavigatedTo");		

                FillFormWithObjectProperties(this._inventProduct);
            }
        }
       
        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Makat":
                        string validation = InventProductValidate.MakatValidate(this._makat);
                        if (String.IsNullOrEmpty(validation) == false) return validation;
                        break;
                    case "QuantityEdit":
                        return InventProductValidate.QuantityEditValidate(this._quantityEdit);
                    case "QuantityOriginal":
                        return InventProductValidate.QuantityOriginalValidate(this._quantityOriginal);
                    case "QuantityPartialEdit":
                        return InventProductValidate.QuantityInPackEditValidate(this._quantityPartialEdit);
				}
                return String.Empty;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }       

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool SaveCommandCanExecute()
        {
            return this._isFormValidated == true;           
        }

        private void FillFormWithObjectProperties(InventProduct inventProduct, bool notifyUI = false)
        {
			this._makat = inventProduct.Makat;
			this._barcode = inventProduct.Barcode;
			this._typeMakat = inventProduct.TypeMakat;
			this._fromCatalogType = inventProduct.FromCatalogType;
			this._productName = inventProduct.ProductName;
			string quantityEdit = inventProduct.QuantityEdit.ToString();
			if (quantityEdit == "0") quantityEdit = "";
			this._quantityEdit = quantityEdit;
			this._quantityOriginal = inventProduct.QuantityOriginal.ToString();
            this._quantityPartialEdit = inventProduct.QuantityInPackEdit.ToString();
			this._remark = inventProduct.IPValueStr10;

			if (notifyUI == true)
            {
                this.IsTimerEnabled = false;

                RaisePropertyChanged(() => Barcode);
                RaisePropertyChanged(() => Makat);
                RaisePropertyChanged(() => QuantityEdit);
                RaisePropertyChanged(() => QuantityOriginal);
                RaisePropertyChanged(() => QuantityPartialEdit);
				RaisePropertyChanged(() => Remark);  

                this.IsTimerEnabled = true;
            }
        }

		private void FillNewObjectWithFormProperties(InventProduct inventProduct)
		{
			if (String.IsNullOrWhiteSpace(this._makat) == false)
			{
				this._makat = this._makat.CutLength(299);
				if (this._makatDictionary.ContainsKey(this._makat) == true)
				{
					ProductMakat productMakat = this._makatDictionary[this._makat];
					if (productMakat == null)
					{
						inventProduct.TypeMakat = TypeMakatEnum.W.ToString();
						inventProduct.Makat = this._makat;
						inventProduct.Barcode = this._makat; 
						inventProduct.ProductName = "NotExistInCatalog";
						inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
					}
					else if (productMakat.TypeCode == TypeMakatEnum.M.ToString())
					{
						inventProduct.Makat = productMakat.Makat;
						inventProduct.Barcode = productMakat.Makat;
						inventProduct.ProductName = productMakat.Name;
						inventProduct.TypeMakat = productMakat.TypeCode;
						inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
					}
					else if (productMakat.TypeCode == TypeMakatEnum.B.ToString())
					{
						inventProduct.Makat = productMakat.Makat;
						inventProduct.ProductName = productMakat.Name;
						if (string.IsNullOrWhiteSpace(productMakat.ParentMakat) == false)
						{
							if (this._makatDictionary.ContainsKey(productMakat.ParentMakat) == true)
							{
								ProductMakat parentProductMakat = this._makatDictionary[productMakat.ParentMakat];
								if (parentProductMakat != null)
								{
									inventProduct.Makat = parentProductMakat.Makat;
									inventProduct.ProductName = parentProductMakat.Name;
								}
							}
						}
			
						inventProduct.Barcode = productMakat.Makat;
						inventProduct.TypeMakat = productMakat.TypeCode;
						inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
					}
				}
				else
				{
					inventProduct.TypeMakat = TypeMakatEnum.W.ToString();
					inventProduct.Makat = this._makat;
					inventProduct.Barcode = this._makat;
					inventProduct.ProductName = "NotExistInCatalog";
					inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
					//inventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
				}


                //	double quantityEdit = String.IsNullOrEmpty(this._quantityEdit) ? 0.0 : Convert.ToDouble(this._quantityEdit);          //26.12.2021
                double quantityEdit = 0.0;
                bool ret = Double.TryParse(this._quantityEdit, out quantityEdit);

                inventProduct.QuantityEdit = quantityEdit;// -quantityEdit % 0.001; ;
                inventProduct.QuantityOriginal = quantityEdit;// -quantityEdit % 0.001;

                //if (ret == false)
                //{
                //    this._quantityEdit = inventProduct.QuantityEdit.ToString();
                //    RaisePropertyChanged(() => QuantityEdit);
                //}

	
				//product.SerialNumber = this._serialNumber;
				inventProduct.InputTypeCode = InputTypeCodeEnum.K.ToString();
				inventProduct.QuantityInPackEdit = String.IsNullOrWhiteSpace(this._quantityPartialEdit) ? 0 : Convert.ToInt32(this._quantityPartialEdit);
			}
		}

        private void FillObjectWithFormProperties(InventProduct inventProduct)
        {
			
			inventProduct.Makat = this._makat;
            inventProduct.Barcode = this._barcode;
			inventProduct.SerialNumber = this._serialNumber;
            inventProduct.ProductName = this._productName;

            //inventProduct.QuantityOriginal = String.IsNullOrEmpty(this._quantityOriginal) ? 0.0 : Convert.ToDouble(this._quantityOriginal);       //26.12.2021
            double quantityOriginal = 0.0;
            bool ret = Double.TryParse(this._quantityOriginal, out quantityOriginal);
            inventProduct.QuantityOriginal = quantityOriginal;
     
             inventProduct.TypeMakat = this._typeMakat;
			inventProduct.FromCatalogType = this._fromCatalogType;
            inventProduct.QuantityInPackEdit = String.IsNullOrWhiteSpace(this._quantityPartialEdit) ? 0 : Convert.ToInt32(this._quantityPartialEdit);
			//??
			//inventProduct.InputTypeCode = InputTypeCodeEnum.K.ToString();

			
			inventProduct.Makat = this._makat;
		//	double quantityEdit = String.IsNullOrEmpty(this._quantityEdit) ? 0.0 : Convert.ToDouble(this._quantityEdit);
            //26.12.2021
            double quantityEdit = 0.0;
            bool ret1 = Double.TryParse(this._quantityEdit, out quantityEdit);
            inventProduct.QuantityEdit = quantityEdit;// -quantityEdit % 0.001;
            //if (ret1 == false)
            //{
            //    this._quantityEdit = inventProduct.QuantityEdit.ToString();
            //    RaisePropertyChanged(() => QuantityEdit);
            //}
          
  	    }

        private void SaveCommandExecuted()
        {
            this._inventProduct.ModifyDate = DateTime.Now;
			this._inventProduct.StatusInventProductBit = 0;

            if (this.IsNewMode)
            {
				this.FillNewObjectWithFormProperties(this._inventProduct);
				this._inventProduct.CreateDate = DateTime.Now;
				this._inventProduct.InputTypeCode = InputTypeCodeEnum.K.ToString();
				this._inventProduct.QuantityDifference = this._inventProduct.QuantityEdit - this._inventProduct.QuantityOriginal;
				int maxIPNum = this._inventProductRepository.GetMaxNumForDocumentCode(this._inventProduct.DocumentHeaderCode, base.GetDbPath);					
				this._inventProduct.IPNum = maxIPNum + 1;
				this._inventProduct.IPValueStr10 = this._remark;

				if (this._inventProduct.ProductName.Trim().ToLower() == "NotExistInCatalog".ToLower())
				{
					this._inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
					this._inventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
				}

				this._inventProductRepository.Insert(this._inventProduct, base.GetDbPath);
                InventProduct newIp = this._inventProductRepository.GetInventProductByCode(this._inventProduct.Code, GetDbPath);

				newIp.Code = (newIp.Makat + "^" + newIp.Barcode).CutLength(299);
				this._inventProductRepository.Update(newIp, base.GetDbPath);

				this._iturRepository.RefillApproveStatusBit(this._inventProduct.IturCode, new List<string>() { this._inventProduct.DocumentHeaderCode }, base.GetDbPath);
                this._eventAggregator.GetEvent<InventProductAddedEvent>().Publish(newIp);
            }
            else
            {
				this.FillObjectWithFormProperties(this._inventProduct);

                //this._inventProduct.ShelfCode = "E";
				this._inventProduct.QuantityDifference = this._inventProduct.QuantityEdit - this._inventProduct.QuantityOriginal;
				this._inventProduct.IPValueStr10 = this._remark;

				if (this._inventProduct.ProductName.Trim().ToLower() == "NotExistInCatalog".ToLower())
				{
					this._inventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
					this._inventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
				}

				this._inventProduct.Code = (this._inventProduct.Makat + "^" + this._inventProduct.Barcode).CutLength(299) ;
                this._inventProductRepository.Update(this._inventProduct, base.GetDbPath);

                InventProduct newIp = this._inventProductRepository.GetInventProductByID(this._inventProduct.ID, GetDbPath);
				this._iturRepository.RefillApproveStatusBit(this._inventProduct.IturCode, new List<string>() { this._inventProduct.DocumentHeaderCode }, base.GetDbPath);

                this._eventAggregator.GetEvent<InventProductEditedEvent>().Publish(newIp);
            }

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }      

        private bool ValidateCommandCanExecute()
        {	
			bool isOkQuantityEdit = UtilsConvert.IsOkAsDouble(this._quantityEdit);
			bool isOkQuantityOriginal = UtilsConvert.IsOkAsDouble(this._quantityOriginal);
		    return (String.IsNullOrEmpty(this._makat) == false)
				&& (this._isInitComplite == true)
				&& (isOkQuantityEdit == true)
				&& (isOkQuantityOriginal == true);
        }

        private void ValidateCommandExecuted()
        {
            InventProduct clone = UtilsConvert.CreateClone(this._inventProduct) as InventProduct;            

            //run some validation on clone
		
            int bitValidate;
			if (this._isNewMode == true)
			{
				this.FillNewObjectWithFormProperties(clone);
				bitValidate = DummyValidateMakatAdd(_makat);
			}
			else
			{
				this.FillObjectWithFormProperties(clone);
				bitValidate = DummyValidateEdit(clone);
			}

            if (bitValidate == 0)
            {
                //or all is ok
                this._inventProduct = clone;
				string quantityEdit = this._inventProduct.QuantityEdit.ToString();
				if (quantityEdit == "0")	this._quantityEdit = "";
               // this.FillFormWithObjectProperties(this._inventProduct, true);

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

		private int DummyValidateMakatAdd(/*InventProduct inventProduct*/string  makat)
        {
			//InventProduct newInventProduct = new InventProduct();
			//int retBit = newInventProduct.ValidateError(newInventProduct, this._makatDictionary);
			//return retBit;
			if (this._makatDictionary.ContainsKey(/*inventProduct.Makat*/makat) == true)
			{
				ProductMakat productMakat = this._makatDictionary[/*inventProduct.Makat*/makat];
				this.ProductName = productMakat.Name;
				//return 0;
			}
			else
			{
				this.ProductName = "NotExistInCatalog";
				//return (int)ConvertDataErrorCodeEnum.InvalidValue; 
				//this.ProductName.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
				//this.ProductName.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
			}
			return 0;
        }

        private int DummyValidateEdit(InventProduct product)
        {
            //product.ProductName = "Edit";
            return 0;
        }             

        private void UndoCommandExecuted()
        {
            QuantityEdit = _quantityOriginal;
        }
    }
}