using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.ExportPda;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;

namespace Count4U.Common.Helpers
{
    public static class UtilsExport
    {
		public static ExportCommandInfo GetExportPdaCustomerData(ICustomerConfigRepository customerConfigRepository, 
			Customer customer, ExportCommandInfo inInfo, string adapterName)
		{							 
			if (inInfo == null)	 inInfo = new ExportCommandInfo();
			string customerCode = "EmptyCode";
			if (customer != null)  customerCode = 	customer.Code;
			 
			string keyCode = customerCode + "|" + adapterName;
			Dictionary<string, CustomerConfig> configDictionary = customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			if (configDictionary != null)
			{
				inInfo.ClientId = configDictionary.GetIntValue(inInfo.ClientId, CustomerConfigIniEnum.ClientID);
				inInfo.FileType = configDictionary.GetIntValue(inInfo.FileType , CustomerConfigIniEnum.FileType);
				inInfo.Hash = configDictionary.GetIntValue(inInfo.Hash, CustomerConfigIniEnum.Hash);
				inInfo.NewItemBool = configDictionary.GetStringValue(inInfo.NewItemBool, CustomerConfigIniEnum.NewItemBool);
				inInfo.ChangeQuantityType = configDictionary.GetStringValue(inInfo.ChangeQuantityType, CustomerConfigIniEnum.ChangeQuantityType);
				inInfo.NewItem = configDictionary.GetIntValue(inInfo.NewItem, CustomerConfigIniEnum.NewItem);
				inInfo.QType = configDictionary.GetIntValue(inInfo.QType, CustomerConfigIniEnum.QType);
				inInfo.Password = configDictionary.GetStringValue(inInfo.Password, CustomerConfigIniEnum.Password);
				inInfo.UseAlphaKey = configDictionary.GetIntValue(inInfo.UseAlphaKey, CustomerConfigIniEnum.UseAlphaKey);

				inInfo.MaxLen = configDictionary.GetIntValue(inInfo.MaxLen, CustomerConfigIniEnum.MaxLen);					  //new		  MaxCharactersSet
				inInfo.IturTypeByName = configDictionary.GetBoolValue(inInfo.IturTypeByName, CustomerConfigIniEnum.IturNameType);	//правильно  
		 		inInfo.IturNamePrefix = configDictionary.GetStringValue(inInfo.IturNamePrefix, CustomerConfigIniEnum.IturNamePrefix);
				inInfo.IturInvertPrefix = configDictionary.GetBoolValue(inInfo.IturInvertPrefix, CustomerConfigIniEnum.IturInvertPrefix);
				inInfo.IsAddBinarySearch = configDictionary.GetBoolValue(inInfo.IsAddBinarySearch, CustomerConfigIniEnum.IsAddBinarySearch);	
				
				inInfo.IsInvertWords = configDictionary.GetBoolValue(inInfo.IsInvertWords, CustomerConfigIniEnum.IsInvertWords);
				inInfo.IsInvertLetters = configDictionary.GetBoolValue(inInfo.IsInvertLetters, CustomerConfigIniEnum.IsInvertLetters);
				inInfo.IsCutAfterInvert = configDictionary.GetBoolValue(inInfo.IsCutAfterInvert, CustomerConfigIniEnum.IsCutAfterInvert);
				inInfo.SearchDef = configDictionary.GetStringValue(inInfo.SearchDef, CustomerConfigIniEnum.SearchDef);

				inInfo.PDAType = configDictionary.GetStringValue(inInfo.PDAType, CustomerConfigIniEnum.PdaType); 		  //new
				inInfo.MaintenanceType = configDictionary.GetStringValue(inInfo.MaintenanceType, CustomerConfigIniEnum.MaintenanceType);
				inInfo.ProgramType = configDictionary.GetStringValue(inInfo.ProgramType, CustomerConfigIniEnum.ProgramType);

				inInfo.HTcalculateLookUp = configDictionary.GetStringValue(inInfo.HTcalculateLookUp, CustomerConfigIniEnum.HTcalculateLookUp);
				inInfo.AddNewLocation = configDictionary.GetStringValue(inInfo.AddNewLocation, CustomerConfigIniEnum.AddNewLocation);
				inInfo.AddExtraInputValueSelectFromBatchListForm = configDictionary.GetStringValue(inInfo.AddExtraInputValueSelectFromBatchListForm, CustomerConfigIniEnum.AddExtraInputValueSelectFromBatchListForm);
				inInfo.AllowNewValueFromBatchListForm = configDictionary.GetStringValue(inInfo.AllowNewValueFromBatchListForm, CustomerConfigIniEnum.AllowNewValueFromBatchListForm);
				inInfo.SearchIfExistInBatchList = configDictionary.GetStringValue(inInfo.SearchIfExistInBatchList, CustomerConfigIniEnum.SearchIfExistInBatchList);
				inInfo.AllowMinusQuantity = configDictionary.GetStringValue(inInfo.AllowMinusQuantity, CustomerConfigIniEnum.AllowMinusQuantity);
				inInfo.FractionCalculate = configDictionary.GetStringValue(inInfo.FractionCalculate, CustomerConfigIniEnum.FractionCalculate);
				inInfo.PartialQuantity = configDictionary.GetStringValue(inInfo.PartialQuantity, CustomerConfigIniEnum.PartialQuantity);
				inInfo.Host1 = configDictionary.GetStringValue(inInfo.Host1, CustomerConfigIniEnum.Host1);
				inInfo.Host2 = configDictionary.GetStringValue(inInfo.Host2, CustomerConfigIniEnum.Host2);
				inInfo.Timeout = configDictionary.GetIntValue(inInfo.Timeout, CustomerConfigIniEnum.Timeout);
				inInfo.Retry = configDictionary.GetIntValue(inInfo.Retry, CustomerConfigIniEnum.Retry);
				inInfo.SameBarcodeInLocation = configDictionary.GetIntValue(inInfo.SameBarcodeInLocation, CustomerConfigIniEnum.SameBarcodeInLocation);
				inInfo.DefaultHost = configDictionary.GetStringValue(inInfo.DefaultHost, CustomerConfigIniEnum.DefaultHost);
				

				inInfo.AllowZeroQuantity = configDictionary.GetStringValue(inInfo.AllowZeroQuantity, CustomerConfigIniEnum.AllowZeroQuantity);
				inInfo.LookUpEXE = configDictionary.GetStringValue(inInfo.LookUpEXE, CustomerConfigIniEnum.LookUpEXE);
				inInfo.MaxQuantity = configDictionary.GetIntValue(inInfo.MaxQuantity, CustomerConfigIniEnum.MaxQuantity);
				inInfo.IsInvertWords = configDictionary.GetBoolValue(inInfo.IsInvertWords, CustomerConfigIniEnum.IsInvertWords);
				inInfo.IsInvertLetters = configDictionary.GetBoolValue(inInfo.IsInvertLetters, CustomerConfigIniEnum.IsInvertLetters);
				 inInfo.LastSync = configDictionary.GetStringValue(inInfo.LastSync, CustomerConfigIniEnum.LastSync);
				inInfo.IsInvertWordsConfig = configDictionary.GetBoolValue(inInfo.IsInvertWordsConfig, CustomerConfigIniEnum.IsInvertWordsConfig);
				inInfo.IsInvertLettersConfig = configDictionary.GetBoolValue(inInfo.IsInvertLettersConfig, CustomerConfigIniEnum.IsInvertLettersConfig);
			   
				inInfo.IncludeCurrentInventor = configDictionary.GetBoolValue(inInfo.IncludeCurrentInventor, CustomerConfigIniEnum.IncludeCurrentInventor);
				inInfo.IncludePreviousInventor = configDictionary.GetBoolValue(inInfo.IncludePreviousInventor, CustomerConfigIniEnum.IncludePreviousInventor);
				inInfo.IncludeProfile = configDictionary.GetBoolValue(inInfo.IncludeProfile, CustomerConfigIniEnum.IncludeProfile);
				inInfo.CreateZipFile = configDictionary.GetBoolValue(inInfo.CreateZipFile, CustomerConfigIniEnum.CreateZipFile);

				inInfo.ConfirmNewLocation = configDictionary.GetStringValue(inInfo.ConfirmNewLocation, CustomerConfigIniEnum.ConfirmNewLocation);
				inInfo.ConfirmNewItem = configDictionary.GetStringValue(inInfo.ConfirmNewItem, CustomerConfigIniEnum.ConfirmNewItem);
				inInfo.AutoSendData = configDictionary.GetStringValue(inInfo.AutoSendData, CustomerConfigIniEnum.AutoSendData);

				inInfo.AllowQuantityFraction = configDictionary.GetStringValue(inInfo.AllowQuantityFraction, CustomerConfigIniEnum.AllowQuantityFraction);
				inInfo.AddExtraInputValue = configDictionary.GetStringValue(inInfo.AddExtraInputValue, CustomerConfigIniEnum.AddExtraInputValue);
				inInfo.AddExtraInputValueHeaderName = configDictionary.GetStringValue(inInfo.AddExtraInputValueHeaderName, CustomerConfigIniEnum.AddExtraInputValueHeaderName);

				//inInfo.IturNameType = configDictionary.GetBoolValue(inInfo.IturNameType, CustomerConfigIniEnum.IturNameType);
				//inInfo.IturInvertPrefix = configDictionary.GetBoolValue(inInfo.IturInvertPrefix, CustomerConfigIniEnum.IturInvertPrefix);
 		
				//if (String.IsNullOrWhiteSpace(customer.ImportIturAdapterParms) == false)
				//{
				//	string maxCharacters = customer.ImportIturAdapterParms;
				//	int v;
				//	if (Int32.TryParse(maxCharacters, out v))
				//	{
				//		inInfo.MaxCharactersSet(v);
				//	}
				//}

				//if (configDictionary.ContainsKey(CustomerConfigIniEnum.EncodingCodePage.ToString()))
				//{
				//	CustomerConfig nameTypeConfig = configDictionary[CustomerConfigIniEnum.EncodingCodePage.ToString()];
 
				//	inInfo.EncodingSelectedItem = inInfo.EncodingItems.FirstOrDefault();
				//	try
				//	{
				//		int codePage = 1255;
				//		bool yes = Int32.TryParse(nameTypeConfig.Value, out codePage);

				//		if (yes == true)
				//		{
				//			System.Text.Encoding newEncoding = System.Text.Encoding.GetEncoding(codePage);
				//			inInfo.EncodingSelectedItem = inInfo.EncodingItems.FirstOrDefault(r => r.Encoding == newEncoding);
				//		}
				//		//	 System.Text.Encoding.GetEncoding(codePage);
				//	}
				//	catch (Exception ex)
				//	{
				//	}
				}
			return inInfo;
		}

		public static ExportCommandInfo GetExportPdaCommandInfoDefaultData(string selectedExportPdaAdapterName, IUserSettingsManager userSettingsManager)
		{
			ExportCommandInfo info = new ExportCommandInfo();
			if (selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaMISAdapter)
			{
				  info = GetExportPdaMISDefaultData(userSettingsManager);
			}
			else if (selectedExportPdaAdapterName == ExportPdaAdapterName.ExportHT630Adapter)
			{
				info = GetExportPdaHT630DefaultData(userSettingsManager);
			}
			else if (selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
				|| selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
				|| selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
				|| selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
				|| selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
			{
				info = GetExportPdaMerkavaSQLiteDefaultData(userSettingsManager);
				if (selectedExportPdaAdapterName == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
				{
					info.IncludeCurrentInventor = false;
					info.IncludePreviousInventor = false;
					info.IncludeProfile = false;
					
				}
			}
			return info;
		}

		public static ExportCommandInfo GetExportPdaMISDefaultData(IUserSettingsManager userSettingsManager)
		{
			ExportCommandInfo info = new ExportCommandInfo();

			info.Hash = 0;
			info.QType = 1;
			info.NewItemBool = "false";
			info.ChangeQuantityType = "true";
			info.Password = "650";
			info.HTcalculateLookUp = "false";
			info.LookUpEXE = userSettingsManager.ExportPDAPathGet().Trim('\\') + @"\IDnextData";
			info.AddNewLocation = "false";
			info.AddExtraInputValueSelectFromBatchListForm = "false";
			info.AllowNewValueFromBatchListForm = "false";
			info.SearchIfExistInBatchList = "false";
			info.AllowMinusQuantity = "false";
			info.FractionCalculate = "false";
			info.PartialQuantity = "false";
			info.Host1 = "192.168.100.1";
			info.Host2 = "192.168.100.20";
			info.Timeout = 10	;
			info.Retry = 3;
			info.DefaultHost = "USB";
			info.SameBarcodeInLocation = 0;
			
			
			info.MaxQuantity = 100;
			info.AllowZeroQuantity = "false";
			info.LastSync = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

			info.IsInvertWordsConfig = true;
			info.IsInvertLettersConfig = true;

			info.FileType = 2;	 //ProductCodeAndNameValue
			info.MaxLen = 25;

			info.IsInvertWords = true;
			info.IsInvertLetters = true;
			info.IsCutAfterInvert = true;
			info.SearchDef = "normal";

			info.IturNamePrefix = @"אתר";
			info.IsAddBinarySearch = false;
			info.EncodingCodePage = 1200;

			info.ConfirmNewLocation = "true";
			info.ConfirmNewItem = "true";
			info.AutoSendData = "0";

			info.AllowQuantityFraction = "true";
			info.AddExtraInputValue = "false";
			info.AddExtraInputValueHeaderName = "";

			return info;
		}

		public static ExportCommandInfo GetExportPdaHT630DefaultData(IUserSettingsManager userSettingsManager)
		{
			ExportCommandInfo info = new ExportCommandInfo();
			info.Hash = 0;
			info.QType = 0;
			info.NewItem = 0;
			info.Password = "";
			info.UseAlphaKey = 1;
			info.HTcalculateLookUp = "";
			info.LookUpEXE = "";

			info.MaxQuantity = 0;
			info.AllowZeroQuantity = "";
			info.AddNewLocation = "false";
			info.AddExtraInputValueSelectFromBatchListForm  = "false";
			info.AllowNewValueFromBatchListForm = "false";
			info.SearchIfExistInBatchList = "false";
			info.AllowMinusQuantity = "false";
			info.FractionCalculate = "false";
			info.PartialQuantity = "false";
			info.Host1 = "";
			info.Host2 = "";
			info.Timeout = 0;
			info.Retry = 0;
			info.DefaultHost = "USB";
			info.SameBarcodeInLocation = 0;
			

			info.IsInvertWordsConfig = true;
			info.IsInvertLettersConfig = true;

			info.FileType = 2;//ProductCodeAndNameValue;
			info.MaxLen = 16;

			info.IsInvertWords = true;
			info.IsInvertLetters = true;
			
			info.SearchDef = "";

			info.IturNamePrefix = @"אתר";
			info.IsCutAfterInvert = false;

			info.ConfirmNewLocation = "";
			info.ConfirmNewItem = "";
			info.AutoSendData = "";

			info.AllowQuantityFraction = "";
			info.AddExtraInputValue = "";
			info.AddExtraInputValueHeaderName = "";

			info.EncodingCodePage = 1255;
			return info;
		}
				
		public static ExportCommandInfo GetExportPdaMerkavaSQLiteDefaultData(IUserSettingsManager userSettingsManager)
		{
			ExportCommandInfo info = new ExportCommandInfo();

			info.IncludeCurrentInventor = true;
			info.IncludePreviousInventor = true;
			info.IncludeProfile = false;
			info.CreateZipFile = false;
			return info;
		}


        public static int GetValue(Dictionary<string, CustomerConfig> config,
            CustomerConfigIniEnum en,
            ICustomerConfigRepository customerConfigRepository)
        {
            int? parseValue = ParseValue(config, en);
            if (parseValue != null)
            {
                return parseValue.Value;

            }
            else
            {
                string rawValue = customerConfigRepository.GetCustomerConfigDefaultValueDictionary()[en];
                int n;
                if (Int32.TryParse(rawValue, out n))
                    return n;
                return 0;
            }
        }

		public static string GetStringValue(Dictionary<string, CustomerConfig> config,
		 CustomerConfigIniEnum en,
		 ICustomerConfigRepository customerConfigRepository)
		{
			string parseValue = ParseStringValue(config, en);
			if (parseValue != "")
			{
				return parseValue;
			}
			else
			{
				string rawValue = customerConfigRepository.GetCustomerConfigDefaultValueDictionary()[en];
				return rawValue;
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