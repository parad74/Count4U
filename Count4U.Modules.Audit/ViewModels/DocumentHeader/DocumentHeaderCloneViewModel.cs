using System.ComponentModel;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using System;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Count4U.Common.Extensions;
using Count4U.Model.Count4U.Validate;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Modules.Audit.ViewModels
{
    public class DocumentHeaderCloneViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _validateCommand;
       
        private bool _isTimerEnabled;

		private bool _isEditable;		  // не используется

		private bool _isIturCode;
		private bool _isERPIturCode;

		private readonly IDocumentHeaderRepository _documentHeaderRepository;
        private readonly IInventProductRepository _inventProductRepository;
		private readonly IIturRepository _iturRepository;

     //   private string _makat;
		private string _toIturCode;
		private string _toERPIturCode;
		private string _fromIturCode;
		private string _fromERPIturCode;
		
		private string _adapterCode;
        private string _quantity;
        private double _quantityBefore;
        private double _quantityAfter;
     //   private InventProduct _inventProductOriginal;
		private DocumentHeader _documentHeaderOriginal;
		private Itur _iturOriginal;
        private readonly IProductRepository _productRepository;
		private readonly ITemporaryInventoryRepository _temporaryInventoryRepository;
	    private readonly IMakatRepository _makatRepository;
        private Dictionary<string, ProductMakat> _makatDictionary;
		private Dictionary<string, DocumentHeader> _iturInFileDictionary;
		private Dictionary<string, Itur> _iturCodeDictionary;
		private Dictionary<string, Itur> _iturErpCodeDictionary; 
        private bool _isPartialVisible;
        private bool _isInitComplite;
		private bool _deleteSourceInventPtoduct;

		public DocumentHeaderCloneViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IMakatRepository makatRepository,
             IProductRepository productRepository,
            IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			IIturRepository iturRepository,
			 ITemporaryInventoryRepository temporaryInventoryRepository)
            : base(contextCBIRepository)
        {
            this._productRepository = productRepository;
            this._makatRepository = makatRepository;
            this._inventProductRepository = inventProductRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			this._iturRepository = iturRepository;
			this._temporaryInventoryRepository = temporaryInventoryRepository;
            this._eventAggregator = eventAggregator;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            //this._validateCommand = new DelegateCommand(ValidateCommandExecuted, ValidateCommandCanExecute);
            this._isInitComplite = true;
            this._isTimerEnabled = false;
			this._deleteSourceInventPtoduct = true;

			this._isEditable = true;

			this._isIturCode = true;
			this._isERPIturCode = false;
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


		public bool IsIturCode
		{
			get { return _isIturCode; }
			set
			{
				this._isIturCode = value;
				RaisePropertyChanged(() => IsIturCode);

				this._isERPIturCode = !value;
				RaisePropertyChanged(() => IsERPIturCode);
				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsERPIturCode
		{
			get { return _isERPIturCode; }
			set
			{
				this._isERPIturCode = value;
				RaisePropertyChanged(() => IsERPIturCode);

				this._isIturCode = !value;
				RaisePropertyChanged(() => IsIturCode);
				//this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public bool DeleteSourceInventPtoduct
		{
			get { return _deleteSourceInventPtoduct; }
			set
			{
				this._deleteSourceInventPtoduct = value;

				RaisePropertyChanged(() => DeleteSourceInventPtoduct);
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

        public DelegateCommand ValidateCommand
        {
            get { return _validateCommand; }
        }

		public string ToIturCode
        {
			get{ return _toIturCode; }
            set
            {
                this._toIturCode = value;
				if (IsIturCode == true)				   //редактируем 	  ToIturCode из редактора => надо поменять 	 ERPIturCode
				{
					if (_iturCodeDictionary != null)
					{
						if (_iturCodeDictionary.ContainsKey(ToIturCode) == true)
						{
							Itur toItur = this._iturCodeDictionary[ToIturCode];
							if (toItur != null)
							{
								if (ToERPIturCode != toItur.ERPIturCode)
								{
									this._toERPIturCode = toItur.ERPIturCode;
									RaisePropertyChanged(() => this.ToERPIturCode);
								}
							}
						}
					}
				}
				RaisePropertyChanged(() => this.ToIturCode);
                this._okCommand.RaiseCanExecuteChanged();
             }
        }

		public string ToERPIturCode
		{
			get { return _toERPIturCode; }
			set
			{
				this._toERPIturCode = value;
				if (IsERPIturCode == true)				   //редактируем 	  ToERPIturCode из редактора => надо поменять 	 IturCode
				{
					if (this._iturErpCodeDictionary != null)
					{
						if (this._iturErpCodeDictionary.ContainsKey(ToERPIturCode) == true)
						{
							Itur toItur = this._iturErpCodeDictionary[ToERPIturCode];
							if (toItur != null)
							{
								if (ToIturCode != toItur.IturCode)
								{
									this._toIturCode = toItur.IturCode;
									RaisePropertyChanged(() => this.ToIturCode);
								}
							}
						}
						else
						{
							this._toIturCode = "";
							RaisePropertyChanged(() => this.ToIturCode);
						}
					}
				}
				RaisePropertyChanged(() => this.ToERPIturCode);
			}
		}

		public string FromIturCode
        {
			get { return _fromIturCode; }
            set
            {
				this._fromIturCode = value;
				RaisePropertyChanged(() => this.FromIturCode);

            }
        }

		public string FromERPIturCode
		{
			get { return _fromERPIturCode; }
			set
			{
				this._fromERPIturCode = value;
				RaisePropertyChanged(() => this.FromERPIturCode);

			}
		}
		

	

	

        private void ValidateCommandExecuted()
        {
           // InventProduct clone = UtilsConvert.CreateClone(this._inventProduct) as InventProduct;

            //run some validation on clone

            //int bitValidate;
            //if (this._isNewMode == true)
            //{
            //    this.FillNewObjectWithFormProperties(clone);
            //    bitValidate = DummyValidateAdd(clone);
            //}
            //else
            //{
            //    this.FillObjectWithFormProperties(clone);
            //    bitValidate = DummyValidateEdit(clone);
            //}

            //if (bitValidate == 0)
            //{
            //    //or all is ok
            //    this._inventProduct = clone;
            //    // this.FillFormWithObjectProperties(this._inventProduct, true);

            //    this.ValidationSummary = String.Empty;
            //    this._isFormValidated = true;
            //}
            //else
            //{
            //    this.ValidationSummary = String.Join(Environment.NewLine, Bit2List.GetStatusList(bitValidate, DomainStatusEnum.PDA));
            //    this._isFormValidated = false;
            //}


            this._okCommand.RaiseCanExecuteChanged();
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
		

		//public string Quantity
		//{
		//	get { return _quantity; }
		//	set
		//	{
		//		_quantity = value;
		//		RaisePropertyChanged(() => Quantity);
                
		//		QuantityAfter = CalculateQuantityAfter();
		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public double QuantityBefore
		//{
		//	get { return _quantityBefore; }
		//	set
		//	{
		//		_quantityBefore = value;
		//		RaisePropertyChanged(() => QuantityBefore);
		//	}
		//}

		//public bool IsPartialVisible
		//{
		//	get { return _isPartialVisible; }
		//	set
		//	{
		//		_isPartialVisible = value;
		//		RaisePropertyChanged(() => IsPartialVisible);
		//	}
		//}

		//public double QuantityAfter
		//{
		//	get { return _quantityAfter; }
		//	set
		//	{
		//		_quantityAfter = value;
		//		RaisePropertyChanged(() => QuantityAfter);
		//	}
		//}

        public object ResultData { get; set; }

    
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			string documentCode = navigationContext.Parameters[Common.NavigationSettings.DocumentCode].Trim();
			            
      		this._documentHeaderOriginal = this._documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);
			if (this._documentHeaderOriginal == null) return;
			this._toIturCode = this._documentHeaderOriginal.IturCode;
			this._fromIturCode = this._documentHeaderOriginal.IturCode;
			this._iturOriginal = this._iturRepository.GetIturByCode(this._documentHeaderOriginal.IturCode, base.GetDbPath);
			this._toERPIturCode = this._iturOriginal.ERPIturCode;
			this._fromERPIturCode = this._iturOriginal.ERPIturCode;
			
			this._adapterCode = navigationContext.Parameters[Common.NavigationSettings.AdapterName].Trim();
			//this._quantity = "0";
			//this._quantityBefore = this._inventProductRepository.GetSumQuantityEditByMakat(this._inventProductOriginal.Makat, base.GetDbPath);

			this._iturInFileDictionary = this._documentHeaderRepository.GetIturDocumentCodeDictionary(base.GetDbPath);
	
            this._isInitComplite = false;
            Task.Factory.StartNew(() =>
            {
                this._makatDictionary = this._makatRepository.GetProductBarcodeDictionary(base.GetDbPath, true);
                //this._iturRepository.FillIturDictionary(base.GetDbPath); 
				this._iturCodeDictionary = this._iturRepository.GetIturDictionary(base.GetDbPath, true);
				this._iturErpCodeDictionary = this._iturRepository.GetERPIturDictionary(base.GetDbPath); 
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
                    case "IturCode":
                        string validation = IturValidate.IturCodeValidate(this.ToIturCode);
                        if (String.IsNullOrEmpty(validation) == false) return validation;
                        break;
					//case "Quantity":
					//	if (!IsQuantityValid())
					//	{
					//		return Localization.Resources.Bit_InventProduct_QuantityEditFormat;
					//	}
					//	if (IsQuantityAfterValid() == false)
					//	{
					//		return Localization.Resources.ViewModel_InventProductClone_quantityValidation;
					//	}
					//	break;
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
            using (new CursorWait())
            {
                if (this._documentHeaderOriginal == null) return;

                string sourceIturCode = this._documentHeaderOriginal.IturCode;
                string sourceDocumentCode = this._documentHeaderOriginal.DocumentCode;

				//исходный документ перемещаем (сейчас клонируем) в новый Itur 
				//есть зависимость от Адаптера (import from PDA)
				Itur newItur = this._iturRepository.GetIturByCode(this._toIturCode, base.GetDbPath);
				DocumentHeader newDocumentHeader = this.FillNewDHFrom_OriginlDH_Itur(this._documentHeaderOriginal, newItur, this._adapterCode);

                if (newDocumentHeader == null) return;
				//newDocumentHeader.CreateDate = DateTime.Now;
				//newDocumentHeader.ModifyDate = DateTime.Now;

                //int IDDocumentHeader = Convert.ToInt32(this._documentHeaderRepository.Insert(newDocumentHeader, base.GetDbPath));
                //newDocumentHeader.IturCode = IturCode;
                //	newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);
                //DocumentHeader newDocumentHeader = this.FillNewObjectFromFormPropertyAndOriginlObject(this._documentHeaderOriginal);

                this._iturRepository.RefillApproveStatusBit(newDocumentHeader.IturCode, new List<string>() { newDocumentHeader.DocumentCode }, base.GetDbPath);
                if (sourceIturCode != newDocumentHeader.IturCode)
                {
                    this._iturRepository.RefillApproveStatusBit(sourceIturCode, new List<string>() { sourceDocumentCode }, base.GetDbPath);
                }

                this.ResultData = newDocumentHeader;

                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
            }
        }

        private bool OkCommandCanExecute()
        {
           if  (String.IsNullOrWhiteSpace(ToIturCode) == true) return false;
		   if (this._iturCodeDictionary == null) return false;
		   if (this._iturErpCodeDictionary == null) return false;
		   
			if (this._iturCodeDictionary.ContainsKey(ToIturCode) == false)
			{
				return false;					
			}
			return true;		

			//if (IsIturCode == true)
			//{
			//	if (this._iturCodeDictionary.ContainsKey(ToIturCode) == false)
			//	{
			//		return false;					
			//	}
			//	else
			//	{
			//		Itur toItur = this._iturCodeDictionary[ToIturCode];
			//		if (toItur == null) return false;					 
			//		ToERPIturCode = toItur.ERPIturCode;
			//		return true;
			//	}
			//}
			//else
			//{
			//	if (this._iturErpCodeDictionary.ContainsKey(ToERPIturCode) == false)
			//	{
			//		return false;			
			//	}
			//	else
			//	{
			//		Itur toItur = this._iturErpCodeDictionary[ToERPIturCode];
			//		if (toItur == null) return false;
			//		ToIturCode = toItur.IturCode;
			//		return true;
			//	}

			//}

          // return true;

          //  return (String.IsNullOrWhiteSpace(this._quantity) == false)
				   //&& (IsQuantityValid() == true)
				//   && (this._isInitComplite == true);
				   //&& IsQuantityAfterValid() == true);

        }

		private DocumentHeader FillNewDHFrom_OriginlDH_Itur(DocumentHeader originalDocumentHeader, Itur toItur, string adapterCode)
		{
			DocumentHeader toDocumentHeader = null;

			if (String.IsNullOrWhiteSpace(toItur.IturCode) == false)		   // в диалоге IturCode
			{
				toDocumentHeader = new DocumentHeader();
			
				string toDocumentCode = Guid.NewGuid().ToString();
				string newSessionCode = Guid.NewGuid().ToString();
				//TODO add new  Session

				int IDtoDocumentHeader = 0;

				if (this._adapterCode != ImportAdapterName.ImportPdaMerkavaDB3Adapter
					&& this._adapterCode != ImportAdapterName.ImportPdaMerkavaXlsxAdapter
					&& this._adapterCode != ImportAdapterName.ImportPdaClalitSqliteAdapter
					&& this._adapterCode != ImportAdapterName.ImportPdaNativSqliteAdapter
					&& this._adapterCode != ImportAdapterName.ImportPdaYesXlsxAdapter )
					//&& this._adapterCode != ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter
					//&& this._adapterCode != ImportAdapterName.ImportPdaNativPlusSqliteAdapter)
				{
					toDocumentHeader.DocumentCode = toDocumentCode;
					toDocumentHeader.SessionCode = newSessionCode;
					toDocumentHeader.CreateDate = originalDocumentHeader.CreateDate;
					toDocumentHeader.WorkerGUID = originalDocumentHeader.WorkerGUID;
					toDocumentHeader.IturCode = toItur.IturCode;
					toDocumentHeader.Approve = null;

					IDtoDocumentHeader = Convert.ToInt32(this._documentHeaderRepository.Insert(toDocumentHeader, base.GetDbPath));
				}
				else
				{
					toDocumentHeader = GetOneDocumentHeaderCodeByIturCode(ref IDtoDocumentHeader, originalDocumentHeader, base.GetDbPath,
						toItur.IturCode, toDocumentCode, newSessionCode);
				}

				InventProducts ipOriginalList = this._inventProductRepository.GetInventProductsByDocumentCode(originalDocumentHeader.DocumentCode, base.GetDbPath);
				InventProducts ipCopyList = new InventProducts();

				int maxIPNum = this._inventProductRepository.GetMaxNumForDocumentCode(toDocumentHeader.DocumentCode, base.GetDbPath);
				//newInventProduct.IPNum = maxIPNum + 1;

				foreach (InventProduct ipOriginal in ipOriginalList)
				{
					if (ipOriginal == null) continue;
					InventProduct ipNew = CreateNewIPFromOriginlIP(ipOriginal, toItur, toDocumentHeader, IDtoDocumentHeader);
					if (ipNew == null) continue;
					maxIPNum++;
					ipNew.IPNum = maxIPNum;
				
					ipCopyList.Add(this.AdditionForAdapter(originalDocumentHeader, toItur, adapterCode, ipOriginal, ipNew));
  				}

				if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter
					|| this._adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter 
					||	this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter 
					|| this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter
					|| this._adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter)
					//|| 	this._adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
					//|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
				{
					this._inventProductRepository.InsertOrUpdate(ipCopyList, toDocumentHeader.DocumentCode, base.GetDbPath);
				}
				else
				{
					this._inventProductRepository.InsertClone(ipCopyList, base.GetDbPath);
				}

				//удаляем старый список
				if (this.DeleteSourceInventPtoduct == true)
				{
					//if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter ||
					//	  this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter||
					//	this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter)
					//{
						this._inventProductRepository.DeleteAllByDocumentHeaderCode(this._documentHeaderOriginal.DocumentCode, this.GetDbPath);
					//}
				}
			}
			return toDocumentHeader;
		}

		private InventProduct AdditionForAdapter(DocumentHeader originalDocumentHeader, Itur toItur, string adapterCode,
			InventProduct OriginalIP, InventProduct NewIP)
		{
			if (toItur == null) return NewIP;
			string toErpIturCode = toItur.ERPIturCode;

			if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter
				|| this._adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter)
			{
				//	 В Меркаве - есть 2 типа айтемов - SN и Q. Так вот только для SN данные храняться в PreviuseInventory. =>
				//Для SN четвертая составляющая ключа везде пустая строка. И в PreviuseInventory и в currentInventory
				//Для айтемов типа "Q". Их нет в PreviuseInventory. Добавляя их в currentInventory четвертая строка в ключе = propertyStrKey8
				string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, "" };	// 3 - составной ключ для SN
				if (OriginalIP.ImputTypeCodeFromPDA == "Q")
				{
					ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, OriginalIP.IPValueStr8 };	// 4 - составной ключ	  для Q
				}
				string ID = ids.JoinRecord("|");
				NewIP.Barcode = ID.CutLength(299);
				NewIP.Code = ID.CutLength(299);
			}
			else if (this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter)
			{
				string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, OriginalIP.IPValueStr13 };	// 4 - составной ключ
				string ID = ids.JoinRecord("|");
				NewIP.Barcode = ID.CutLength(299);
				NewIP.Code = ID.CutLength(299);
			}

			else if (this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter)
			{
				string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, ""};	// 4 - составной ключ
				string ID = ids.JoinRecord("|");
				NewIP.Barcode = ID.CutLength(299);
				NewIP.Code = ID.CutLength(299);
			}
			else if (this._adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter)
			{
				string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, OriginalIP.SupplierCode };	// 4 - составной ключ
				string ID = ids.JoinRecord("|");
				NewIP.Barcode = ID.CutLength(299);
				NewIP.Code = ID.CutLength(299);
			}
			else if (this._adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)		 //TODO
			{
				//string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, OriginalIP.SupplierCode };	// 4 - составной ключ
				//string ID = ids.JoinRecord("|");
				//NewIP.Barcode = ID.CutLength(299);
				//NewIP.Code = ID.CutLength(299);
				NewIP.Code = (NewIP.Makat + "^" + NewIP.Barcode).CutLength(299); 
			}
			else if (this._adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter)
			{
				string[] ids = new string[] { OriginalIP.SerialNumber, OriginalIP.Makat, toErpIturCode, OriginalIP.SupplierCode };	// 4 - составной ключ
				string ID = ids.JoinRecord("|");
				NewIP.Barcode = ID.CutLength(299);
				NewIP.Code = ID.CutLength(299);
			}
			else
			{
				NewIP.Code = (NewIP.Makat + "^" + NewIP.Barcode).CutLength(299); 
			}
			

			// если удаляем старый InventProduct
			if (this.DeleteSourceInventPtoduct == true)
			{
				if (this._adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter
					|| this._adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
					|| this._adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter
					|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
					|| this._adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter)
					//|| this._adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter
					//|| this._adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
				{
					
					//Записать в темпоральную таблицу
					TemporaryInventory temporaryInventory = new TemporaryInventory();
					temporaryInventory.NewUid = NewIP.Barcode.CutLength(249);
					temporaryInventory.OldUid = OriginalIP.Barcode.CutLength(249); 
					string dateModified = (DateTime.Now).ConvertDateTimeToAndroid();
					temporaryInventory.DateModified = dateModified;

					temporaryInventory.OldSerialNumber = OriginalIP.SerialNumber.CutLength(249); ;
					temporaryInventory.NewSerialNumber = OriginalIP.SerialNumber.CutLength(249); ;
					temporaryInventory.OldItemCode = OriginalIP.Makat.CutLength(249);
					temporaryInventory.NewItemCode = OriginalIP.Makat.CutLength(249);
					temporaryInventory.OldKey = this._iturOriginal.IturCode;
					temporaryInventory.NewKey = toItur.IturCode;
					temporaryInventory.OldLocationCode = this._iturOriginal.ERPIturCode.CutLength(249);
					temporaryInventory.NewLocationCode = toErpIturCode.CutLength(249); 
					temporaryInventory.Description = "Move DocHeader to newItur in Count4U :" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
					temporaryInventory.Operation = "MOVE";
					temporaryInventory.Domain = "Itur";
					this._temporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);
				}
			}

			return NewIP;
		}

		private InventProduct CreateNewIPFromOriginlIP(InventProduct originalInventProduct, Itur toItur,DocumentHeader documentHeader, int docNum)
        {
            InventProduct newInventProduct = new InventProduct(originalInventProduct);
			newInventProduct.IturCode = toItur.IturCode;
			newInventProduct.ERPIturCode = toItur.ERPIturCode;
			newInventProduct.DocumentCode = documentHeader.DocumentCode;
			newInventProduct.DocumentHeaderCode = documentHeader.DocumentCode;
			newInventProduct.SessionCode = documentHeader.SessionCode;
			newInventProduct.DocNum = docNum;
			newInventProduct.CreateDate = originalInventProduct.CreateDate;
			newInventProduct.ModifyDate = originalInventProduct.ModifyDate;

			//int maxIPNum = this._inventProductRepository.GetMaxNumForDocumentCode(newInventProduct.DocumentHeaderCode, base.GetDbPath);
			//newInventProduct.IPNum = maxIPNum + 1;
            return newInventProduct;
        }

		//получаем документ, для адаптеров у которых документ в Itur должет быть не более чем один
		private DocumentHeader GetOneDocumentHeaderCodeByIturCode(
				ref int IDDocumentHeader,
				DocumentHeader originalDocumentHeader,
				string dbPath,
				string toIturCode,	 //move to IturCode
				string toDocumentCode,
				string newSessionCode)
		{
			string retDocumentCode = "";
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			dtfi.ShortDatePattern = @"dd/MM/yyyy";
			dtfi.ShortTimePattern = @"hh:mm:ss";
			Itur toItur = this._iturRepository.GetIturByCode(toIturCode, dbPath);
			  //в Itur куда перемещаем, есть документ, его и возвращаем
			if (this._iturInFileDictionary.ContainsKey(toIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				DocumentHeader document = this._iturInFileDictionary[toIturCode];
				if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);
				retDocumentCode = document.DocumentCode;
				// снимаем с него 	  Approve
				document.Approve = false;
				this._documentHeaderRepository.Update(document, dbPath);
			   //меняем статус у Itur в который перемещаем, потому как он непонятный теперь
				if (toItur != null)
				{
					toItur.InvStatus = 1;
					this._iturRepository.Update(toItur, dbPath);
				}
				return document;
			}
			//========================================DocumentHeader==================
			else // create new DocumentHeader	 так как нет в Itur ни одного DH
			{
				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				//string newDocumentCode = Guid.NewGuid().ToString(); 
				newDocumentHeaderString.DocumentCode = toDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = DateTime.Now.ToString();
				newDocumentHeaderString.WorkerGUID = originalDocumentHeader.WorkerGUID;
				newDocumentHeaderString.IturCode = toIturCode;
				newDocumentHeaderString.Name = originalDocumentHeader.Name;

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					//this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
				}
				else //	  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, dtfi); //Warning
					newDocumentHeader.Approve = false;
					newDocumentHeader.CreateDate = originalDocumentHeader.CreateDate;

					IDDocumentHeader = Convert.ToInt32(this._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					newDocumentHeader.ID = IDDocumentHeader;
					//retDocumentCode = newDocumentCode;
					this._iturInFileDictionary[toIturCode] = newDocumentHeader; //словарь IturCode -> DocumentHeader. Создаем для каждого Itur только один DocumentHeader

					if (toItur != null)
					{
						toItur.InvStatus = 1;
						this._iturRepository.Update(toItur, dbPath);
					}
					//if (retBitDocumentHeader != 0)
					//{
					//	//this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
					//}
				}
				return newDocumentHeader;
			}

		}

		//private bool IsQuantityValid()
		//{
		//	double dummy;
		//	return Double.TryParse(_quantity, out dummy);
		//}

		//private double CalculateQuantityAfter()
		//{
		//	double result = 0;
		//	double parsed;
		//	if (Double.TryParse(_quantity, out parsed))
		//	{
		//		result = _quantityBefore + parsed;                
		//	}
		//	else
		//	{
		//		result = 0;
		//	}

		//	return result;
		//}

		//private bool IsQuantityAfterValid()
		//{
		//	return CalculateQuantityAfter() >= 0;
		//}
    }
}