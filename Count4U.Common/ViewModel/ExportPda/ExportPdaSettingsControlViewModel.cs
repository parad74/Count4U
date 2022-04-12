using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Misc;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.ExportPda
{
    public class ExportPdaSettingsControlViewModel : NotificationObject, INavigationAware, IDataErrorInfo
    {
		protected readonly IUserSettingsManager _userSettingsManager;
		protected readonly ICustomerConfigRepository _customerConfigRepository;
        private const int ProductCodeAndNameValue = 2;
		private const int OnlyBarcodesValue = 3;
		private const int ProductCodeAndNameAndSupplierCodeValue = 4;
		private const int ProductCodeAndFamilyNameAndFamilyColorValue = 5;
		private const int ProductCodeAndNameAndUnitTypeValue = 6;
		private const int ProductCodeAndNameAndUnitNameAndSerial = 7;
		private const int OnlyMakatValue = 8;
		private const int OnlyBarcodesAndNameValue = 9;
		private const int ProductCodeAndNameAndQuantityInPackAndUnitTypeValue = 10;
		private const int MakatAndNameAndListBarcode = 11;
		
		private int _hash;
        private int _qType;
        private int _useAlphaKey;
        private int _clientId;
        private int _newItem;
		private string _newItemBool;
		private string _changeQuantityType;
		
		private string _lastSync;
		private string _htcalculateLookUp;
		private string  _addNewLocation;
		private string _allowZeroQuantity;
		private int _maxQuantity;

		private string _lookUpEXE;
        private string _maxCharacters;
        private bool _isMaxCharactersVisible;
        private bool _isMaxCharactersVisibleInternal;

		private string _password;
	
		private bool _isMISVisible;
		private bool _isHT360Visible;

		private string _confirmNewLocation;
		private string _confirmNewItem;
		private string _autoSendData;

		private string _allowQuantityFraction;
		private string _addExtraInputValue;
		private string _addExtraInputValueHeaderName;

		private string _addExtraInputValueSelectFromBatchListForm;
		private string _allowNewValueFromBatchListForm;
		private string _searchIfExistInBatchList;
		private string _allowMinusQuantity;
		private string _fractionCalculate;
		private string _partialQuantity;
		private string _host1;
		private string _host2;
		private int _timeout;
		private int _retry;
		private int _sameBarcodeInLocation;
		
		private string _defaultHost;
		
	

        private ExportPdaSettingsItemViewModel _fileTypeSelected;

        private bool _isEditable;

        private readonly ObservableCollection<int> _hashItems = new ObservableCollection<int>() { 0, 1 };
        private readonly ObservableCollection<ExportPdaSettingsItemViewModel> _fileTypeItems;
        private readonly ObservableCollection<int> _qTypeItems = new ObservableCollection<int>() { 0, 1 };
        private readonly ObservableCollection<int> _useAlphaKeyItems = new ObservableCollection<int>() { 0, 1 };
        private readonly ObservableCollection<int> _clientIdItems = new ObservableCollection<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly ObservableCollection<int> _newItemItems = new ObservableCollection<int>() { 0, 1, 2 };
		private readonly ObservableCollection<string> _newItemBoolItems = new ObservableCollection<string>() { "true", "false" };
		private readonly ObservableCollection<string> _changeQuantityTypeItems = new ObservableCollection<string>() { "true", "false" };
		
		private readonly ObservableCollection<string> _htcalculateLookUpItems = new ObservableCollection<string>() { "true", "false" };// { "NO", "YES" };
		private readonly ObservableCollection<string> _addNewLocationItems = new ObservableCollection<string>() { "true", "false" };
		private readonly ObservableCollection<string> _defaultHostItems = new ObservableCollection<string>() { "USB", "HOST1", "HOST2" }; //USB, HOST1 or HOST2
		private readonly ObservableCollection<string> _allowZeroQuantityItems = new ObservableCollection<string>() { "true", "false" };
		
		private readonly ObservableCollection<string> _allowQuantityFractionItems = new ObservableCollection<string>() { "true", "false" };
		private readonly ObservableCollection<string> _addExtraInputValueItems = new ObservableCollection<string>() { "true", "false" };

		private readonly ObservableCollection<EncodingItemViewModel> _encodingItems;
		private EncodingItemViewModel _encodingSelectedItem;

        private bool _iturTypeByName;
        private bool _iturTypeByERP;
        private string _iturNamePrefix;

        private bool _isInvertPrefix;
		private bool _isAddBinarySearch;
		private bool _isInvertWords;
		private bool _isInvertLetters;
		private bool _isInvertWordsConfig;
		private bool _isInvertLettersConfig;
		private bool _isCutAfterInvert;
		private string _searchDef;
		private Encoding _encoding;
		public int EncodingCodePage { get; set; }	

        public ExportPdaSettingsControlViewModel(
			IUserSettingsManager userSettingsManager,
			ICustomerConfigRepository customerConfigRepository)
        {
			this._userSettingsManager = userSettingsManager;
			this._customerConfigRepository = customerConfigRepository;
			
            this._fileTypeItems = new ObservableCollection<ExportPdaSettingsItemViewModel>();
            this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
            {
                Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_NoLookupFile,
                Value = 0,
            });
            this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
            {
                Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_OnlyProductCode,
                Value = 1,
            });
			this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			{
				Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_OnlyMakat,
				Value = OnlyMakatValue,
			});
            this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
            {
                Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeName,
                Value = ProductCodeAndNameValue,
            });
			 this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
            {
				Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_OnlyBarcodes,
				Value = OnlyBarcodesValue,
            });
		
			 this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			 {
				 Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeAndNameAndSupplierCode,
				 Value = ProductCodeAndNameAndSupplierCodeValue,
			 });
			 this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			 {
				 Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeAndFamilyNameAndFamilyColor,
				 Value = ProductCodeAndFamilyNameAndFamilyColorValue,
			 });

			 this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			 {
				 Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeAndNameAndUnitType,
				 Value = ProductCodeAndNameAndUnitTypeValue,
			 });
			
			this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			 {
				 Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeAndNameAndUnitNameAndSerial,
				 Value = ProductCodeAndNameAndUnitNameAndSerial,
			 });
			this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			{
				Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_OnlyBarcodesAndName,
				Value = OnlyBarcodesAndNameValue,
			});
			this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			{
				Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_ProductCodeAndNameAndQuantityInPackAndUnitType,
				Value = ProductCodeAndNameAndQuantityInPackAndUnitTypeValue,
			});

			this._fileTypeItems.Add(new ExportPdaSettingsItemViewModel()
			{
				Name = Localization.Resources.ViewModel_ExportPdaSettingsControl_MakatAndNameAndListBarcode,
				Value = MakatAndNameAndListBarcode,
			});
			
			
			this._newItemBool =   "false";
			this._changeQuantityType = "true";
			
			this._lastSync = "";
			this._htcalculateLookUp = "";
			this._addNewLocation = "";
			this._addExtraInputValueSelectFromBatchListForm = "";
			this._allowNewValueFromBatchListForm = "";
			this._searchIfExistInBatchList = "";
			this._allowMinusQuantity = "";
			this._fractionCalculate = "";
			this._partialQuantity = "";
			

			this._allowZeroQuantity = "";
			this._lookUpEXE = "";
			this._maxCharacters = "";
			this._password = "";
			this._host1 = "";
			this._host2 = "";
			this._iturNamePrefix = "";
			this._searchDef = "";
			this._defaultHost = "";
			this._sameBarcodeInLocation = 0;
			this._timeout = 0;
			this._retry = 0;
			
	
            this._maxCharacters = "16";
			this._password = "650";

			//this._host1 = "192.168.100.1";
			//this._host2 = "192.168.100.20";

			this.IsMISVisible = false;
			this.IsHT360Visible = false;

			this._iturTypeByName = true;
			this._iturTypeByERP = false;
			this._iturNamePrefix = @"אתר";
			this._isInvertPrefix = true;
			this._isAddBinarySearch = false;
			this._isInvertWords = false;
			this._isInvertLetters = false;

			this._confirmNewLocation = "";
			this._confirmNewItem = "";
			this._autoSendData = "";

			this._allowQuantityFraction = "";
			this._addExtraInputValue = "";
			this._addExtraInputValueHeaderName = "";

			this._encodingItems = new ObservableCollection<EncodingItemViewModel>();
			this.BuildEncoding();

			 //this._encodingSelectedItem = this.EncodingItems.FirstOrDefault(r => r.Encoding == base.DynViewModel.Encoding);
			 // else
			this._encodingSelectedItem =  this.EncodingItems.FirstOrDefault(r => r.Encoding ==UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager));

			if (this._encodingSelectedItem != null)
			{
				this._encoding = this._encodingSelectedItem.Encoding;
			}
			if (this._encoding == null) this._encoding = System.Text.Encoding.GetEncoding(1255);
			
			//this.EncodingItems.FirstOrDefault();
        }

		/// <summary>
		/// В  ExportCommandInfo упаковываем данные из контрола.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public ExportCommandInfo FillExportSettingsControlInfo(ExportCommandInfo info)
		{
			info.ClientId = this.ClientId;
			info.FileType = this.FileType;
			info.Hash = this.Hash;
			info.NewItem = this.NewItem;
			info.NewItemBool = this.NewItemBool;
			info.ChangeQuantityType = this.ChangeQuantityType;
			info.QType = this.QType;
			info.UseAlphaKey = this.UseAlphaKey;
			info.IturNamePrefix = this.IturNamePrefix;
			info.IturTypeByName = this.IturTypeByName;
			info.IturInvertPrefix = this.IsInvertPrefix;
			info.IsAddBinarySearch = this.IsAddBinarySearch;
			info.IsInvertWords = this.IsInvertWords;
			info.IsInvertLetters = this.IsInvertLetters;
			info.IsCutAfterInvert = this.IsCutAfterInvert;
			info.SearchDef = this.SearchDef;
			info.IsInvertWordsConfig = this.IsInvertWordsConfig;
			info.IsInvertLettersConfig = this.IsInvertLettersConfig;
			if (this._encoding == null) this._encoding = System.Text.Encoding.GetEncoding(1255);
			info.EncodingCodePage = this._encoding.CodePage;
			info.Password = this.Password;
			info.HTcalculateLookUp = this.HTcalculateLookUp;
			info.LookUpEXE = this.LookUpEXE;
			info.AddNewLocation = this.AddNewLocation;
			info.AddExtraInputValueSelectFromBatchListForm = this.AddExtraInputValueSelectFromBatchListForm;
			info.AllowNewValueFromBatchListForm = this.AllowNewValueFromBatchListForm;
			info.SearchIfExistInBatchList = this.SearchIfExistInBatchList;
			info.AllowMinusQuantity = this.AllowMinusQuantity;
			info.FractionCalculate = this.FractionCalculate;
			info.PartialQuantity = this.PartialQuantity;
			info.Host1 = this.Host1;
			info.Host2 = this.Host2;
			info.Timeout = this.Timeout;
			info.Retry = this.Retry;
			info.SameBarcodeInLocation = this.SameBarcodeInLocation;
			info.DefaultHost = this.DefaultHost;
			
			
			info.AllowZeroQuantity = this.AllowZeroQuantity;
			info.MaxQuantity = this.MaxQuantity;
			info.LastSync = this.LastSync;
			info.MaxLen = this.MaxCharactersValidated;
			info.ConfirmNewLocation = this.ConfirmNewLocation;
			info.ConfirmNewItem = this.ConfirmNewItem;
			info.AutoSendData = this.AutoSendData;
			info.AllowQuantityFraction = this.AllowQuantityFraction;
			info.AddExtraInputValue = this.AddExtraInputValue;
			info.AddExtraInputValueHeaderName = this.AddExtraInputValueHeaderName;
			return info;
		}

		/// <summary>
		/// заполнение данных по умолчанию для каждого адаптера своих
		/// потом - на следущем шаге заполняется из кастомра
		/// </summary>
		/// <param name="adapter"></param>
		public void FillGUIAdapterData(IExportPdaModuleInfo adapter, ExportCommandInfo info)
		{
			this.IturTypeByName = true;
			this.IturTypeByERP = false;
			this.IsAddBinarySearch = false;
			this.IsInvertPrefix = true;

			this.IsMISVisible = false;
			this.IsHT360Visible = false;

	
			this.EncodingSelectedItem = this.EncodingItems.FirstOrDefault(r => r.Encoding == UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager));
		//	ExportCommandInfo info = UtilsExport.GetExportPdaCommandInfoDefaultData(adapter.Name, this._userSettingsManager);

			if (adapter == null) return;
			if (adapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter)
			{
				//	UtilsExport.GetExportPdaMISDefaultData(this._userSettingsManager);
				this.Hash = info.Hash;//0; 
				this.QType = info.QType;//1;
				this.NewItemBool = info.NewItemBool;//"false";
				this.ChangeQuantityType = info.ChangeQuantityType;//"true";
				this.Password = info.Password;//"650";
				this.Host1 = info.Host1;//"192.168.100.1";
				this.Host2 = info.Host2;//"192.168.100.20";
				this.Timeout = info.Timeout;
				this.Retry = info.Retry;
				this.SameBarcodeInLocation = info.SameBarcodeInLocation;
				this.DefaultHost = info.DefaultHost;
				this.HTcalculateLookUp = info.HTcalculateLookUp;//"false";
				this.LookUpEXE = info.LookUpEXE;//this._userSettingsManager.ExportPDAPathGet().Trim('\\') + @"\IDnextData";
				this.AddNewLocation = info.AddNewLocation;//"false";
				this.AddExtraInputValueSelectFromBatchListForm = info.AddExtraInputValueSelectFromBatchListForm;   //"false";
				this.AllowNewValueFromBatchListForm = info.AllowNewValueFromBatchListForm;	 //"false";
				this.SearchIfExistInBatchList = info.SearchIfExistInBatchList;	 //"false";
				this.AllowMinusQuantity = info.AllowMinusQuantity;	 //"false";
				this.FractionCalculate = info.FractionCalculate;	//"false";
				this.PartialQuantity = info.PartialQuantity;	//"false";
				this.Host1 = info.Host1;
				this.Host2 = info.Host2;
				this.Timeout = info.Timeout;
				this.Retry = info.Retry;
				this.SameBarcodeInLocation = info.SameBarcodeInLocation;
				this.DefaultHost = info.DefaultHost;

				this.MaxQuantity = info.MaxQuantity;//100;
				this.AllowZeroQuantity = info.AllowZeroQuantity;//"false";
				this.LastSync = info.LastSync;//DateTime.Now.ToShortDateString() + " " +DateTime.Now.ToShortTimeString();

				this.IsInvertWordsConfig = info.IsInvertWordsConfig;//true;
				this.IsInvertLettersConfig = info.IsInvertLettersConfig;//true;

				this.FileType = info.FileType;//ProductCodeAndNameValue;
				this.MaxCharacters = info.MaxLen.ToString();//"25";

				this.IsInvertWords = info.IsInvertWords;//true;
				this.IsInvertLetters = info.IsInvertLetters;//true;
				this.IsCutAfterInvert = info.IsCutAfterInvert;//true;
				this.SearchDef = info.SearchDef;//"normal";
				this.IsAddBinarySearch = info.IsAddBinarySearch;

				this.ConfirmNewLocation = info.ConfirmNewLocation;	//true;
				this.ConfirmNewItem = info.ConfirmNewItem;	  //true 
				this.AutoSendData = info.AutoSendData;		  //0

				this.AllowQuantityFraction = info.AllowQuantityFraction;	  //true;
				this.AddExtraInputValue = info.AddExtraInputValue;		   //false;
				this.AddExtraInputValueHeaderName = info.AddExtraInputValueHeaderName;		  //

				this.EncodingCodePage = info.EncodingCodePage;
				System.Text.Encoding encoding1200 = System.Text.Encoding.GetEncoding(info.EncodingCodePage);
				this.EncodingSelectedItem = this.EncodingItems.FirstOrDefault(r => r.Encoding == encoding1200);
				
			}
			else if (adapter.Name == ExportPdaAdapterName.ExportHT630Adapter)
			{
				//Config
				this.Hash = info.Hash;//0; 
				this.QType = info.QType;//0;
				this.ClientId = info.ClientId; //0
				this.UseAlphaKey = info.UseAlphaKey;//1;
				this.NewItem = info.NewItem;//0;
				
				//this.Password = info.Password;//"";
				//this.HTcalculateLookUp = info.HTcalculateLookUp;//"";
				//this.LookUpEXE = info.LookUpEXE;//"";

				//this.MaxQuantity = info.MaxQuantity;//0;
				//this.AllowZeroQuantity = info.AllowZeroQuantity;//"";
				//this.AddNewLocation = info.AddNewLocation;//"false";

				//this.IsInvertWordsConfig = info.IsInvertWordsConfig;//true;
				//this.IsInvertLettersConfig = info.IsInvertLettersConfig;//true;
		
				
				//Lookup
				this.FileType = info.FileType; //ProductCodeAndNameValue;
				this.MaxCharacters = info.MaxLen.ToString();
				//this.MaxCharacters = "16";
				this.IsInvertWords = info.IsInvertWords;//true;
				this.IsInvertLetters = info.IsInvertLetters;//true;
				this.IsCutAfterInvert = info.IsCutAfterInvert;//false;
				this.IsAddBinarySearch = info.IsAddBinarySearch;
				this.EncodingCodePage = info.EncodingCodePage;
				System.Text.Encoding encoding1255 = System.Text.Encoding.GetEncoding(info.EncodingCodePage);
				this.EncodingSelectedItem =  this.EncodingItems.FirstOrDefault(r => r.Encoding == encoding1255);

			}

		}



		public void FillAdapterCustomerData(Customer customer, string adapterName)
			 //IExportPdaModuleInfo selectedAdapter)
		{
			if ( customer == null || this._customerConfigRepository == null)
				return;
			//перед этим запонить данными по умолчанию для каждого авдптера отдельно.
			//if (selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter)
			string keyCode = customer.Code + "|" + adapterName;
			Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			if (configDictionary != null)
			{	//  GetValue(this Dictionary<string, CustomerConfig> config, int parm,CustomerConfigIniEnum adapterParm)
				this.ClientId = configDictionary.GetIntValue(this.ClientId, CustomerConfigIniEnum.ClientID);
				this.FileType = configDictionary.GetIntValue(this.FileType , CustomerConfigIniEnum.FileType);
				this.Hash = configDictionary.GetIntValue(this.Hash, CustomerConfigIniEnum.Hash);
				this.NewItemBool = configDictionary.GetStringValue(this.NewItemBool, CustomerConfigIniEnum.NewItemBool);
				this.ChangeQuantityType = configDictionary.GetStringValue(this.ChangeQuantityType, CustomerConfigIniEnum.ChangeQuantityType);
				this.NewItem = configDictionary.GetIntValue(this.NewItem, CustomerConfigIniEnum.NewItem);
				this.QType = configDictionary.GetIntValue(this.QType, CustomerConfigIniEnum.QType);
				this.Password = configDictionary.GetStringValue(this.Password, CustomerConfigIniEnum.Password);
				this.Host1 = configDictionary.GetStringValue(this.Host1, CustomerConfigIniEnum.Host1);
				this.Host2 = configDictionary.GetStringValue(this.Host2, CustomerConfigIniEnum.Host2);
				this.Timeout = configDictionary.GetIntValue(this.Timeout, CustomerConfigIniEnum.Timeout);
				this.Retry = configDictionary.GetIntValue(this.Retry, CustomerConfigIniEnum.Retry);
				this.SameBarcodeInLocation = configDictionary.GetIntValue(this.SameBarcodeInLocation, CustomerConfigIniEnum.SameBarcodeInLocation);
				this.DefaultHost = configDictionary.GetStringValue(this.DefaultHost, CustomerConfigIniEnum.DefaultHost);
				this.UseAlphaKey = configDictionary.GetIntValue(this.UseAlphaKey, CustomerConfigIniEnum.UseAlphaKey);
				this.HTcalculateLookUp = configDictionary.GetStringValue(this.HTcalculateLookUp, CustomerConfigIniEnum.HTcalculateLookUp);
				this.AddNewLocation = configDictionary.GetStringValue(this.AddNewLocation, CustomerConfigIniEnum.AddNewLocation);
				this.AddExtraInputValueSelectFromBatchListForm = configDictionary.GetStringValue(this.AddExtraInputValueSelectFromBatchListForm, CustomerConfigIniEnum.AddExtraInputValueSelectFromBatchListForm);
				this.AllowNewValueFromBatchListForm = configDictionary.GetStringValue(this.AllowNewValueFromBatchListForm, CustomerConfigIniEnum.AllowNewValueFromBatchListForm);
				this.SearchIfExistInBatchList = configDictionary.GetStringValue(this.SearchIfExistInBatchList, CustomerConfigIniEnum.SearchIfExistInBatchList);
				this.AllowMinusQuantity = configDictionary.GetStringValue(this.AllowMinusQuantity, CustomerConfigIniEnum.AllowMinusQuantity);
				this.FractionCalculate = configDictionary.GetStringValue(this.FractionCalculate, CustomerConfigIniEnum.FractionCalculate);
				this.PartialQuantity = configDictionary.GetStringValue(this.PartialQuantity, CustomerConfigIniEnum.PartialQuantity);
				this.Host1 = configDictionary.GetStringValue(this.Host1, CustomerConfigIniEnum.Host1);
				this.Host2 = configDictionary.GetStringValue(this.Host2, CustomerConfigIniEnum.Host2);
				this.Timeout = configDictionary.GetIntValue(this.Timeout, CustomerConfigIniEnum.Timeout);
				this.Retry = configDictionary.GetIntValue(this.Retry, CustomerConfigIniEnum.Retry);
				this.SameBarcodeInLocation = configDictionary.GetIntValue(this.SameBarcodeInLocation, CustomerConfigIniEnum.SameBarcodeInLocation);
				this.DefaultHost = configDictionary.GetStringValue(this.DefaultHost, CustomerConfigIniEnum.DefaultHost);
				this.AllowZeroQuantity = configDictionary.GetStringValue(this.AllowZeroQuantity, CustomerConfigIniEnum.AllowZeroQuantity);
				this.LookUpEXE = configDictionary.GetStringValue(this.LookUpEXE, CustomerConfigIniEnum.LookUpEXE);
				this.IsInvertWords = configDictionary.GetBoolValue(this.IsInvertWords, CustomerConfigIniEnum.IsInvertWords);
				this.IsInvertLetters = configDictionary.GetBoolValue(this.IsInvertLetters, CustomerConfigIniEnum.IsInvertLetters);
				this.IsCutAfterInvert = configDictionary.GetBoolValue(this.IsCutAfterInvert, CustomerConfigIniEnum.IsCutAfterInvert);
				this.Encoding = System.Text.Encoding.GetEncoding(1255);
				this.EncodingCodePage = 1255;
  	
				this.ConfirmNewLocation = configDictionary.GetStringValue(this.ConfirmNewLocation, CustomerConfigIniEnum.ConfirmNewLocation); 
				this.ConfirmNewItem = configDictionary.GetStringValue(this.ConfirmNewItem, CustomerConfigIniEnum.ConfirmNewItem); 
				this.AutoSendData = configDictionary.GetStringValue(this.AutoSendData, CustomerConfigIniEnum.AutoSendData);

				this.AllowQuantityFraction = configDictionary.GetStringValue(this.AllowQuantityFraction, CustomerConfigIniEnum.AllowQuantityFraction);
				this.AddExtraInputValue = configDictionary.GetStringValue(this.AddExtraInputValue, CustomerConfigIniEnum.AddExtraInputValue);
				this.AddExtraInputValueHeaderName = configDictionary.GetStringValue(this.AddExtraInputValueHeaderName, CustomerConfigIniEnum.AddExtraInputValueHeaderName); 


				this.MaxQuantity = configDictionary.GetIntValue(this.MaxQuantity, CustomerConfigIniEnum.MaxQuantity);
				//this.LastSync = configDictionary.GetStringValue(this.LastSync, CustomerConfigIniEnum.LastSync); 
				this.SearchDef = configDictionary.GetStringValue(this.SearchDef, CustomerConfigIniEnum.SearchDef); 
				//this.AddNewLocation = configDictionary.GetStringValue(this.AddNewLocation, CustomerConfigIniEnum.AddNewLocation);
				//this.AllowZeroQuantity = configDictionary.GetStringValue(this.AllowZeroQuantity, CustomerConfigIniEnum.AllowZeroQuantity); 
				//this.LastSync = DateTime.Now.ToString("dd-MM-yy  HH:mm");
				this.IsInvertWordsConfig = configDictionary.GetBoolValue(this.IsInvertWordsConfig, CustomerConfigIniEnum.IsInvertWordsConfig);
				this.IsInvertLettersConfig = configDictionary.GetBoolValue(this.IsInvertLettersConfig, CustomerConfigIniEnum.IsInvertLettersConfig);
				this.IturNamePrefix = configDictionary.GetStringValue(this.IturNamePrefix, CustomerConfigIniEnum.IturNamePrefix);

				this.IturTypeByName = configDictionary.GetBoolValue(this.IturTypeByName, CustomerConfigIniEnum.IturNameType);
				this.IsInvertPrefix = configDictionary.GetBoolValue(this.IsInvertPrefix, CustomerConfigIniEnum.IturInvertPrefix);
				this.IsAddBinarySearch = configDictionary.GetBoolValue(this.IsAddBinarySearch, CustomerConfigIniEnum.IsAddBinarySearch);
				
 				//this.MaxCharacters = configDictionary.GetIntValue(this.MaxCharacters , CustomerConfigIniEnum.MaxLen);		  //
				

				//if (String.IsNullOrWhiteSpace(customer.ImportIturAdapterParms) == false)
				//{
					//string maxCharacters = customer.ImportIturAdapterParms;
					string maxCharacters = configDictionary.GetStringValue(this.MaxCharacters , CustomerConfigIniEnum.MaxLen);		
					int v;
					if (Int32.TryParse(maxCharacters, out v) == true)
					{
						this.MaxCharactersSet(v);				//CustomerConfigIniEnum.MaxLen
					}
					else
					{
						this.MaxCharacters = this.MaxCharacters;
					}
				//}

				if (configDictionary.ContainsKey(CustomerConfigIniEnum.EncodingCodePage.ToString()))
				{
					CustomerConfig nameTypeConfig = configDictionary[CustomerConfigIniEnum.EncodingCodePage.ToString()];
 
					this.EncodingSelectedItem = this.EncodingItems.FirstOrDefault();
					try
					{
						int codePage = 1255;
						bool yes = Int32.TryParse(nameTypeConfig.Value, out codePage);

						if (yes == true)
						{
							System.Text.Encoding newEncoding = System.Text.Encoding.GetEncoding(codePage);
							this.EncodingSelectedItem = this.EncodingItems.FirstOrDefault(r => r.Encoding == newEncoding);
						}
						//	 System.Text.Encoding.GetEncoding(codePage);
					}
					catch (Exception ex)
					{
					}
				}

			}
		}

		[NotInludeAttribute]
		public ObservableCollection<EncodingItemViewModel> EncodingItems
		{
			get { return _encodingItems; }
		}

		 [NotInludeAttribute]
		public EncodingItemViewModel EncodingSelectedItem
		{
			get { return _encodingSelectedItem; }
			set
			{
				_encodingSelectedItem = value;
				if (this._encodingSelectedItem != null)
				{
					this._encoding = this._encodingSelectedItem.Encoding;
				}
				if (this._encoding == null) this._encoding = System.Text.Encoding.GetEncoding(1255);
				RaisePropertyChanged(() => EncodingSelectedItem);
			}
		}

		public void BuildEncoding()
		{
			var list = new List<Encoding>
                           {
                               Encoding.GetEncoding(1255), Encoding.GetEncoding(862),
                               Encoding.ASCII, Encoding.Unicode, Encoding.UTF8, Encoding.UTF32, Encoding.UTF7
                           };
			foreach (var encodingInfo in Encoding.GetEncodings().OrderBy(r => r.DisplayName))
			{
				Encoding enc = encodingInfo.GetEncoding();
				if (!list.Contains(enc))
					list.Add(enc);
			}
			foreach (Encoding encoding in list)
			{
				EncodingItemViewModel item = new EncodingItemViewModel(encoding);
				this._encodingItems.Add(item);
			}
		}

        public int Hash
        {
            get { return _hash; }
            set
            {
                _hash = value;
                RaisePropertyChanged(() => Hash);
            }
        }

        public int FileType
        {
            get
            {
                if (_fileTypeSelected == null) return 0;

                return _fileTypeSelected.Value;
            }
            set
            {
                var item = _fileTypeItems.FirstOrDefault(r => r.Value == value);
                if (item != null)
                    FileTypeSelected = item;
            }
        }

		[NotInludeAttribute]
        public ExportPdaSettingsItemViewModel FileTypeSelected
        {
            get { return this._fileTypeSelected; }
            set
            {
                this._fileTypeSelected = value;
                RaisePropertyChanged(() => FileTypeSelected);

                IsMaxCharactersVisible = _fileTypeSelected != null
					&& (this._fileTypeSelected.Value == ProductCodeAndNameValue || this._fileTypeSelected.Value == ProductCodeAndNameAndSupplierCodeValue
					|| this._fileTypeSelected.Value == ProductCodeAndNameAndUnitTypeValue || this._fileTypeSelected.Value == ProductCodeAndNameAndUnitNameAndSerial
					|| this._fileTypeSelected.Value == OnlyBarcodesAndNameValue || this._fileTypeSelected.Value == ProductCodeAndNameAndQuantityInPackAndUnitTypeValue
					|| this._fileTypeSelected.Value == MakatAndNameAndListBarcode) 
					&& this._isMaxCharactersVisibleInternal;
            }
        }


        public int QType
        {
            get { return _qType; }
            set
            {
                _qType = value;
                RaisePropertyChanged(() => QType);
            }
        }


        public int UseAlphaKey
        {
            get { return _useAlphaKey; }
            set
            {
                _useAlphaKey = value;
                RaisePropertyChanged(() => UseAlphaKey);
            }
        }


		public string HTcalculateLookUp
		{
			get { return _htcalculateLookUp; }
			set
			{ 
				_htcalculateLookUp = value;
				RaisePropertyChanged(() => HTcalculateLookUp);
			}
		}

		public string AddNewLocation
		{
			get { return _addNewLocation; }
			set
			{
				_addNewLocation = value;
				RaisePropertyChanged(() => AddNewLocation);
			}
		}



		public string AddExtraInputValueSelectFromBatchListForm
		{
			get { return _addExtraInputValueSelectFromBatchListForm; }
			set
			{
				_addExtraInputValueSelectFromBatchListForm = value;
				RaisePropertyChanged(() => AddExtraInputValueSelectFromBatchListForm);
			}
		}


	public string AllowNewValueFromBatchListForm
		{
			get { return _allowNewValueFromBatchListForm; }
			set
			{
				_allowNewValueFromBatchListForm = value;
				RaisePropertyChanged(() => AllowNewValueFromBatchListForm);
			}
		}


	public string SearchIfExistInBatchList
	{
		get { return _searchIfExistInBatchList; }
		set
		{
			_searchIfExistInBatchList = value;
			RaisePropertyChanged(() => SearchIfExistInBatchList);
		}
	}


	public string AllowMinusQuantity
	{
		get { return _allowMinusQuantity; }
		set
		{
			_allowMinusQuantity = value;
			RaisePropertyChanged(() => AllowMinusQuantity);
		}
	}


	public string FractionCalculate
	{
		get { return _fractionCalculate; }
		set
		{
			_fractionCalculate = value;
			RaisePropertyChanged(() => FractionCalculate);
		}
	}


	public string PartialQuantity
	{
		get { return _partialQuantity; }
		set
		{
			_partialQuantity = value;
			RaisePropertyChanged(() => PartialQuantity);
		}
	}



		public string AllowZeroQuantity
		{
			get { return _allowZeroQuantity; }
			set
			{
				_allowZeroQuantity = value;
				RaisePropertyChanged(() => AllowZeroQuantity);
			}
		}

		public string LookUpEXE
		{
			get { return _lookUpEXE; }
			set { 
				_lookUpEXE = value;
				RaisePropertyChanged(() => LookUpEXE);
			}
		}

        public int ClientId
        {
            get { return _clientId; }
            set
            {
                _clientId = value;
                RaisePropertyChanged(() => ClientId);
            }
        }

        public int NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                RaisePropertyChanged(() => NewItem);
            }
        }

		public string NewItemBool
        {
            get { return _newItemBool; }
            set
            {
                _newItemBool = value;
                RaisePropertyChanged(() => NewItemBool);
            }
        }


		public string ChangeQuantityType
        {
			get { return _changeQuantityType; }
            set
            {
				_changeQuantityType = value;
				RaisePropertyChanged(() => ChangeQuantityType);
            }
        }

		public string LastSync
		{
			get { return _lastSync; }
			set 
			{
				_lastSync = value;
				RaisePropertyChanged(() => LastSync);
				//DD-MM-YY HH:MM
				//string dt = DateTime.Now.ToString("dd-MM-yy  hh:mm");
				//return dt; 
			}
		}

		public int MaxQuantity
        {
			get { return _maxQuantity; }
            set
            {
				_maxQuantity = value;
				RaisePropertyChanged(() => MaxQuantity);
            }
        }
		
		public string ConfirmNewLocation
        {
            get { return _confirmNewLocation; }
            set
            {
                _confirmNewLocation = value;
                RaisePropertyChanged(() => ConfirmNewLocation);
            }
        }

		public string ConfirmNewItem
        {
            get { return _confirmNewItem; }
            set
            {
                _confirmNewItem = value;
                RaisePropertyChanged(() => ConfirmNewItem);
            }
        }
	

		public string AutoSendData
        {
            get { return _autoSendData; }
            set
            {
                _autoSendData = value;
                RaisePropertyChanged(() => AutoSendData);
            }
        }

		public string AllowQuantityFraction
        {
            get { return _allowQuantityFraction; }
            set
            {
                _allowQuantityFraction = value;
                RaisePropertyChanged(() => AllowQuantityFraction);
            }
        }

		public string AddExtraInputValue
        {
            get { return _addExtraInputValue; }
            set
            {
                _addExtraInputValue = value;
                RaisePropertyChanged(() => AddExtraInputValue);
            }
        }
		
		public string AddExtraInputValueHeaderName
        {
            get { return _addExtraInputValueHeaderName; }
            set
            {
                _addExtraInputValueHeaderName = value;
                RaisePropertyChanged(() => AddExtraInputValueHeaderName);
            }
        }

	   [NotInludeAttribute]
        public ObservableCollection<int> HashItems
        {
            get { return _hashItems; }
        }

		[NotInludeAttribute]
        public ObservableCollection<ExportPdaSettingsItemViewModel> FileTypeItems
        {
            get { return _fileTypeItems; }
        }

		[NotInludeAttribute]
        public ObservableCollection<int> QTypeItems
        {
            get { return _qTypeItems; }
        }

		[NotInludeAttribute]
		public ObservableCollection<string> HtCalculateLookUpItems
		{
			get { return _htcalculateLookUpItems; }
		}

		[NotInludeAttribute]
		public ObservableCollection<string> AddNewLocationItems
		{
			get { return _addNewLocationItems; }
		}

		[NotInludeAttribute]
		public ObservableCollection<string> DefaultHostItems
		{
			get { return _defaultHostItems; }
		}

		[NotInludeAttribute]
		public ObservableCollection<string> AllowQuantityFractionItems
		{
			get { return _allowQuantityFractionItems; }
		}

		[NotInludeAttribute]
		public ObservableCollection<string> AddExtraInputValueItems
		{
			get { return _addExtraInputValueItems; }
		}

		[NotInludeAttribute]
		public ObservableCollection<string> AllowZeroQuantityItems
		{
			get { return _allowZeroQuantityItems; }
		}

		[NotInludeAttribute]
        public ObservableCollection<int> UseAlphaKeyItems
        {
            get { return _useAlphaKeyItems; }
        }

	   [NotInludeAttribute]
        public ObservableCollection<int> ClientIdItems
        {
            get { return _clientIdItems; }
        }

		[NotInludeAttribute]
        public ObservableCollection<int> NewItemItems
        {
            get { return _newItemItems; }
        }

		[NotInludeAttribute]
		public ObservableCollection<string> NewItemBoolItems
        {
			get { return _newItemBoolItems; }
        }

		
		[NotInludeAttribute]
		public ObservableCollection<string> ChangeQuantityTypeItems
        {
			get { return _changeQuantityTypeItems; }
        }

        public string MaxCharacters
        {
            get { return _maxCharacters; }
            set
            {
                _maxCharacters = value;
                RaisePropertyChanged(() => MaxCharacters);
            }
        }

		public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

		public string Host1
		{
			get { return _host1; }
			set
			{
				_host1 = value;
				RaisePropertyChanged(() => Host1);
			}
		}


		public string Host2
		{
			get { return _host2; }
			set
			{
				_host2 = value;
				RaisePropertyChanged(() => Host2);
			}
		}

		public int Timeout
		{
			get { return _timeout; }
			set
			{
				_timeout = value;
				RaisePropertyChanged(() => Timeout);
			}
		}

		public int Retry
		{
			get { return _retry; }
			set
			{
				_retry = value;
				RaisePropertyChanged(() => Retry);
			}
		}



		public int SameBarcodeInLocation
		{
			get { return _sameBarcodeInLocation; }
			set
			{
				_sameBarcodeInLocation = value;
				RaisePropertyChanged(() => SameBarcodeInLocation);
			}
		}

		public string DefaultHost
		{
			get { return _defaultHost; }
			set
			{
				_defaultHost = value;
				RaisePropertyChanged(() => DefaultHost);
			}
		}


		[NotInludeAttribute]
		public bool IsMISVisible
        {
			get { return _isMISVisible; }
            set
            {
				_isMISVisible = value;
				RaisePropertyChanged(() => IsMISVisible);
            }
        }

		[NotInludeAttribute]
		public bool IsHT360Visible
        {
			get { return _isHT360Visible; }
            set
            {
				_isHT360Visible = value;
				RaisePropertyChanged(() => IsHT360Visible);
            }
        }

		[NotInludeAttribute]
		public bool IsMaxCharactersVisible
        {
			get { return _isMaxCharactersVisible; }
            set
            {
				_isMaxCharactersVisible = value;
				RaisePropertyChanged(() => IsMaxCharactersVisible);
            }
        }


		[NotInludeAttribute]
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(() => IsEditable);
            }
        }

        public bool IturTypeByName
        {
            get { return _iturTypeByName; }
            set
            {
                _iturTypeByName = value;
                RaisePropertyChanged(() => IturTypeByName);

                _iturTypeByERP = !_iturTypeByName;
                RaisePropertyChanged(() => IturTypeByERP);
            }
        }

        public bool IturTypeByERP
        {
            get { return _iturTypeByERP; }
            set
            {
                _iturTypeByERP = value;
                RaisePropertyChanged(() => IturTypeByERP);

                _iturTypeByName = !_iturTypeByERP;
                RaisePropertyChanged(() => IturTypeByName);
            }
        }

        public string IturNamePrefix
        {
            get { return _iturNamePrefix; }
            set
            {
                _iturNamePrefix = value;
                RaisePropertyChanged(() => IturNamePrefix);
            }
        }

        public bool IsInvertPrefix
        {
            get { return _isInvertPrefix; }
            set
            {
                _isInvertPrefix = value;
                RaisePropertyChanged(() => IsInvertPrefix);
            }
        }


		public bool IsAddBinarySearch
        {
			get { return _isAddBinarySearch; }
            set
            {
				_isAddBinarySearch = value;
				RaisePropertyChanged(() => IsAddBinarySearch);
            }
        }

		public bool IsInvertWords
        {
			get { return _isInvertWords; }
            set
            {
				_isInvertWords = value;
				RaisePropertyChanged(() => IsInvertWords);
            }
        }



		public bool IsInvertLetters
        {
			get { return _isInvertLetters; }
            set
            {
				_isInvertLetters = value;
				RaisePropertyChanged(() => IsInvertLetters);
            }
        }


		public string SearchDef
        {
            get { return _searchDef; }
            set
            {
                _searchDef = value;
                RaisePropertyChanged(() => SearchDef);
            }
        }

		public bool IsCutAfterInvert
        {
			get { return _isCutAfterInvert; }
            set
            {
				_isCutAfterInvert = value;
				RaisePropertyChanged(() => IsCutAfterInvert);
            }
        }

		

			public bool IsInvertWordsConfig
        {
			get { return _isInvertWordsConfig; }
            set
            {
				_isInvertWordsConfig = value;
				RaisePropertyChanged(() => IsInvertWordsConfig);
            }
        }

			

		public bool IsInvertLettersConfig
        {
			get { return _isInvertLettersConfig; }
            set
            {
				_isInvertLettersConfig = value;
				RaisePropertyChanged(() => IsInvertLettersConfig);
            }
        }

		[NotInludeAttribute]
		public Encoding Encoding
        {
			get { return _encoding; }
            set
            {
				_encoding = value;
				if (value == null) this._encoding = System.Text.Encoding.GetEncoding(1255);
				RaisePropertyChanged(() => Encoding);

            }
        }
		

        public int MaxCharactersValidated
        {
            get
            {
                if (this._isMaxCharactersVisibleInternal
					&& this._fileTypeSelected != null
					&& (this._fileTypeSelected.Value == ProductCodeAndNameValue 
					|| this._fileTypeSelected.Value == ProductCodeAndNameAndSupplierCodeValue
					|| this._fileTypeSelected.Value == ProductCodeAndNameAndUnitTypeValue
					|| this._fileTypeSelected.Value == ProductCodeAndNameAndUnitNameAndSerial
					|| this._fileTypeSelected.Value == OnlyBarcodesAndNameValue
					|| this._fileTypeSelected.Value == ProductCodeAndNameAndQuantityInPackAndUnitTypeValue
					|| this._fileTypeSelected.Value == MakatAndNameAndListBarcode
					
					))
                {
                    int val;
                    if (Int32.TryParse(this._maxCharacters, out val))
                        return val;
                }

                return 0;
            }
        }

        public void MaxCharactersSet(int maxCharacters)
        {
            this.MaxCharacters = maxCharacters.ToString();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //  this._isMaxCharactersVisibleInternal = navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ExportPdaSettingsConrolInExportFormMode);
            _isMaxCharactersVisibleInternal = true;
            this._isMaxCharactersVisible = _isMaxCharactersVisibleInternal;
            this._isEditable = !navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IsReadOnly);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

		[NotInludeAttribute]
        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "MaxCharacters":
                        if (!IsMaxCharactersOk())
                            return Localization.Resources.ViewModel_ExportPdaSettingsControl_ValidIntegerRequired;
                        break;
                }
                return String.Empty;
            }
        }

		[NotInludeAttribute]
        public string Error
        {
            get { return String.Empty; }
        }       

        public bool IsMaxCharactersOk()
        {
            int r;
            bool okParse = Int32.TryParse(this._maxCharacters, out r);
            return okParse && r > 0;
        }

        public bool IsFormValid()
        {
            if (!_isMaxCharactersVisibleInternal)
                return true;

            if (_fileTypeSelected == null)
                return true;

            if (_fileTypeSelected.Value != ProductCodeAndNameValue)
                return true;

            return IsMaxCharactersOk();
        }
    }

	public static class ExportParamParse
	{
		public static int GetIntValue(this Dictionary<string, CustomerConfig> config, int parm,
			CustomerConfigIniEnum adapterParm)
		{
			int? parseValue = ParseValue(config, adapterParm);
			if (parseValue != null)
			{
				return parseValue.Value;
			}
			else
			{
				return parm;

			}
		}

		public static string GetStringValue(this Dictionary<string, CustomerConfig> config, string parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				return parseValue;
			}
			else
			{
				return parm;
			}
		}


		public static bool GetBoolValue(this Dictionary<string, CustomerConfig> config, bool parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				bool ret = parseValue == "1";
				return ret;
			}
			else
			{
				return parm;
			}
		}

		private static int? ParseValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value;
			int n;
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
				if (Int32.TryParse(value, out n))
					return n;
			}

			return null;
		}

		private static string ParseStringValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value = "";
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				try
				{
					value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
					return value;
				}
				catch { return value; }
			}

			return value;
		}
	}

}