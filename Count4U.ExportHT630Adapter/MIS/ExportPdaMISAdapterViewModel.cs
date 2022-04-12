using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Text;
using Count4U.Common.Services.Ini;
using Count4U.Model.Count4U.Validate;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Model.Extensions;
using Count4U.Common.Constants;

namespace Count4U.ExportPdaMISAdapter
{	  // ExportPdaSettingsView
	public class ExportPdaMISAdapterViewModel : ExportPdaModuleBaseViewModel	//все для динамической смены\выбора  ExportPda Adapter
    {
		//private readonly IDBSettings _dbSettings;
		//private string _misCommunicatorPath = "";
		private string _misiDnextDataPath = "";

		public ExportPdaMISAdapterViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
			IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager)
        {
		//		public string IDnextDataPath
		//{
		//	get { return this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\IDnextData"; }
			//this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\MISCommunicator";
		//}
			//this._dbSettings = dbSettings;
			this._misiDnextDataPath = this._userSettingsManager.ExportPDAPathGet().Trim('\\') + @"\IDnextData";
			//this._misCommunicatorPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\MISCommunicator";
			base.LookUpEXE = this.MISiDnextDataPath;
        }

        protected override void InitDefault()		//из кастомера !
		{
			this._misiDnextDataPath = this._userSettingsManager.ExportPDAPathGet().Trim('\\') + @"\IDnextData";
			//this._misCommunicatorPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\MISCommunicator";
			//base.LookUpEXE = this.MISiDnextDataPath;
			//base.InvertPrefix = true;
			//base.InvertWords = true;
			//base.IsInvertWordsConfig = true;
			//base.IsInvertLettersConfig = true;
			//base.CutAfterInvert = true;
			//base.Encoding = System.Text.Encoding.GetEncoding(1255);
		}

		protected override void InitFromIniFile()	   //из кастомера !
		{	//[MISFolder]
			//MISiDnextDataPath = "C:\MIS\IDnextData" 
			//MISCommunicatorPath = "C:\MIS\MISCommunicator"

			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData("MISFolder",  base.GetPathToIniFile("Count4U.ExportHT630Adapter.ini"));
			//this._misCommunicatorPath = iniData.SetValue(ImportProviderParmEnum.MISCommunicatorPath, this._misCommunicatorPath);
			//this._misiDnextDataPath = iniData.SetValue(ImportProviderParmEnum.MISiDnextDataPath, this._misiDnextDataPath);
			//base.LookUpEXE = this.MISiDnextDataPath;

			//base.IsMISVisible = info.PDAType;
        }

		protected override ExportCommandInfo InitFromConfig(ExportCommandInfo info, CBIState state)
		{
			if (state == null) return info;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						info = this.FillInfoFromConfig(configXDoc, info);
						//XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
						info.EncodingCodePage = 1200;  //всегда  иначе PDA не будет работать
						if (this.State != null)
						{
							if (this.CurrentCustomer != null)
							{
								info.Customer = this.CurrentCustomer;
							}
						}
						//string exportPath = XDocumentConfigRepository.GetExportPath(this, configXDoc);
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
			return info;
		}

        public override bool CanExport()
        {
            return true;
        }

		//Заполняем из конфига ExportCommandInfo
		private ExportCommandInfo FillInfoFromConfig(XDocument configXDocument, ExportCommandInfo info)
		{
			//	Dictionary<string, OperationXElement> GetDictenryFromInitXDocumentConfig(object viewModel, XDocument configXDocument)
			try
			{
				Dictionary<string, OperationXElement> dictionaryFromInitXDocument = XDocumentConfigRepository.GetDictionaryFromInitXDocumentConfig(configXDocument);

				info.WithoutProductName = false;
				info.MakatWithoutMask = false;
				//info.CheckBaudratePDA = true;
				info.BarcodeWithoutMask = false;
				info.WithoutProductName = dictionaryFromInitXDocument.SetValueFromConfig("WithoutProductName", info.WithoutProductName);
				info.MakatWithoutMask = dictionaryFromInitXDocument.SetValueFromConfig("MakatWithoutMask", info.MakatWithoutMask);
			//	info.CheckBaudratePDA = dictionaryFromInitXDocument.SetValueFromConfig("CheckBaudratePDA", info.CheckBaudratePDA);
				info.BarcodeWithoutMask = dictionaryFromInitXDocument.SetValueFromConfig("BarcodeWithoutMask", info.BarcodeWithoutMask);

				info.IsInvertWords = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertWords);		  //true
				info.IsInvertLetters = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertLetters);			//true

				info.Hash = dictionaryFromInitXDocument.SetValueFromConfig("Hash", info.Hash);
				info.FileType = dictionaryFromInitXDocument.SetValueFromConfig("FileType", info.FileType);
				info.QType = dictionaryFromInitXDocument.SetValueFromConfig("QType", info.QType);
				info.UseAlphaKey = dictionaryFromInitXDocument.SetValueFromConfig("UseAlphaKey", info.UseAlphaKey);
				info.ClientId = dictionaryFromInitXDocument.SetValueFromConfig("ClientId", info.ClientId);
				//info.NewItem = dictionaryFromInitXDocument.SetValueFromConfig("NewItem", info.NewItem);


				info.NewItemBool = dictionaryFromInitXDocument.SetValueFromConfig("NewItemBool", info.NewItemBool);
				info.ChangeQuantityType = dictionaryFromInitXDocument.SetValueFromConfig("ChangeQuantityType", info.ChangeQuantityType);
				info.Password = dictionaryFromInitXDocument.SetValueFromConfig("Password", info.Password);
				info.HTcalculateLookUp = dictionaryFromInitXDocument.SetValueFromConfig("HTcalculateLookUp", info.HTcalculateLookUp);
				info.IturTypeByName = dictionaryFromInitXDocument.SetValueFromConfig("IturTypeByName", info.IturTypeByName);
				bool isInvertPrefix = true;
				isInvertPrefix = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertPrefix", isInvertPrefix);
				info.IturInvertPrefix = isInvertPrefix;
				info.IturNamePrefix = dictionaryFromInitXDocument.SetValueFromConfig("IturNamePrefix", info.IturNamePrefix);
				string max = "16";
				max = dictionaryFromInitXDocument.SetValueFromConfig("MaxCharacters", max);
				int intmax = 16;
				if (Int32.TryParse(max, out intmax) == true)
					info.MaxLen = intmax;
				info.IturInvertPrefix = dictionaryFromInitXDocument.SetValueFromConfig("IturInvertPrefix", info.IturInvertPrefix);
				info.IsAddBinarySearch = dictionaryFromInitXDocument.SetValueFromConfig("IsAddBinarySearch", info.IsAddBinarySearch);
				info.EncodingCodePage = 1200;
				info.IsInvertWords = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertWords);		  //true
				info.IsInvertLetters = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWords", info.IsInvertLetters);			//true
				info.IsCutAfterInvert = dictionaryFromInitXDocument.SetValueFromConfig("IsCutAfterInvert", info.IsCutAfterInvert);
				info.SearchDef = dictionaryFromInitXDocument.SetValueFromConfig("SearchDef", info.SearchDef);

				info.AddNewLocation = dictionaryFromInitXDocument.SetValueFromConfig("AddNewLocation", info.AddNewLocation);

				info.AddExtraInputValueSelectFromBatchListForm = dictionaryFromInitXDocument.SetValueFromConfig("AddExtraInputValueSelectFromBatchListForm", info.AddExtraInputValueSelectFromBatchListForm);
				info.AllowNewValueFromBatchListForm = dictionaryFromInitXDocument.SetValueFromConfig("AllowNewValueFromBatchListForm", info.AllowNewValueFromBatchListForm);
				info.SearchIfExistInBatchList = dictionaryFromInitXDocument.SetValueFromConfig("SearchIfExistInBatchList", info.SearchIfExistInBatchList);
				info.AllowMinusQuantity = dictionaryFromInitXDocument.SetValueFromConfig("AllowMinusQuantity", info.AllowMinusQuantity);
				info.FractionCalculate = dictionaryFromInitXDocument.SetValueFromConfig("FractionCalculate", info.FractionCalculate);
				info.PartialQuantity = dictionaryFromInitXDocument.SetValueFromConfig("PartialQuantity", info.PartialQuantity);
				info.Host1 = dictionaryFromInitXDocument.SetValueFromConfig("Host1", info.Host1);
				info.Host2 = dictionaryFromInitXDocument.SetValueFromConfig("Host2", info.Host2);
				info.Timeout = dictionaryFromInitXDocument.SetValueFromConfig("Timeout", info.Timeout);
				info.Retry = dictionaryFromInitXDocument.SetValueFromConfig("Retry", info.Retry);
				info.SameBarcodeInLocation = dictionaryFromInitXDocument.SetValueFromConfig("SameBarcodeInLocation", info.SameBarcodeInLocation);
				info.DefaultHost = dictionaryFromInitXDocument.SetValueFromConfig("DefaultHost", info.DefaultHost);


				info.AllowZeroQuantity = dictionaryFromInitXDocument.SetValueFromConfig("AllowZeroQuantity", info.AllowZeroQuantity);
				info.MaxQuantity = dictionaryFromInitXDocument.SetValueFromConfig("MaxQuantity", info.MaxQuantity);
				info.LastSync = dictionaryFromInitXDocument.SetValueFromConfig("LastSync", info.LastSync);
				info.IsInvertWordsConfig = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertWordsConfig", info.IsInvertWordsConfig);
				info.IsInvertLettersConfig = dictionaryFromInitXDocument.SetValueFromConfig("IsInvertLettersConfig", info.IsInvertLettersConfig);

				info.ConfirmNewLocation = dictionaryFromInitXDocument.SetValueFromConfig("ConfirmNewLocation", info.ConfirmNewLocation);
				info.ConfirmNewItem = dictionaryFromInitXDocument.SetValueFromConfig("ConfirmNewItem", info.ConfirmNewItem);
				info.AutoSendData = dictionaryFromInitXDocument.SetValueFromConfig("AutoSendData", info.AutoSendData);

				info.AllowQuantityFraction = dictionaryFromInitXDocument.SetValueFromConfig("AllowQuantityFraction", info.AllowQuantityFraction);
				info.AddExtraInputValue = dictionaryFromInitXDocument.SetValueFromConfig("AddExtraInputValue", info.AddExtraInputValue);
				info.AddExtraInputValueHeaderName = dictionaryFromInitXDocument.SetValueFromConfig("AddExtraInputValueHeaderName", info.AddExtraInputValueHeaderName);

				//	"PDAType\HT630"
				//"MaintenanceType\Regular"
				//"ProgramType\Fashion"

				info.PDAType = @"PDAType\" + dictionaryFromInitXDocument.SetValueFromConfig("PdaTypeKey", "");
				info.MaintenanceType = @"MaintenanceType\" + dictionaryFromInitXDocument.SetValueFromConfig("MaintenanceTypeKey", "");
				info.ProgramType = @"ProgramType\" + dictionaryFromInitXDocument.SetValueFromConfig("ProgramTypeKey", "");
			}
			catch { }
			return info;
		}

	
        protected override void RunExportInner(ExportCommandInfo info)
        {
            base.WithoutProductName = info.WithoutProductName ? "1" : "";
            base.MakatWithoutMask = info.MakatWithoutMask ? "1" : "";
			base.CheckBaudratePDA = info.CheckBaudratePDA ? "1" : "";
            base.BarcodeWithoutMask = info.BarcodeWithoutMask ? "1" : "";

            base.Hash = info.Hash;
            base.FileType = info.FileType;
            base.QType = info.QType;
            base.UseAlphaKey = info.UseAlphaKey;
            base.ClientId = info.ClientId;
			base.NewItemBool = info.NewItemBool;
			base.ChangeQuantityType = info.ChangeQuantityType;
			base.Password = info.Password;
			base.HTcalculateLookUp = info.HTcalculateLookUp;
			//base.LookUpEXE = info.LookUpEXE;
            base.CustomerCode = info.Customer.Code;
            base.CustomerName = info.Customer.Name;
            base.IturNameType = info.IturTypeByName;
            base.IturNamePrefix = info.IturNamePrefix;
            base.MaxLen = info.MaxLen;
            base.InvertPrefix = info.IturInvertPrefix;
			base.IsAddBinarySearch = info.IsAddBinarySearch;
			if (info.EncodingCodePage == 0) info.EncodingCodePage = 1200;
			base.Encoding = System.Text.Encoding.GetEncoding(info.EncodingCodePage);
			//if (base.Encoding.CodePage == 1201) base.Encoding = Encoding.BigEndianUnicode;
			//if (base.Encoding.CodePage == 1200) base.Encoding = Encoding.Unicode;
			base.InvertWords = info.IsInvertWords;
			base.InvertLetters = info.IsInvertLetters;
			base.CutAfterInvert = info.IsCutAfterInvert;
			base.SearchDef = info.SearchDef;
			
		    base.PDAType = info.PDAType;
            base.MaintenanceType = info.MaintenanceType;
            base.ProgramType = info.ProgramType;
	
			base.AddNewLocation = info.AddNewLocation;

			base.AddExtraInputValueSelectFromBatchListForm = info.AddExtraInputValueSelectFromBatchListForm;
			base.AllowNewValueFromBatchListForm = info.AllowNewValueFromBatchListForm;
			base.SearchIfExistInBatchList = info.SearchIfExistInBatchList;
			base.AllowMinusQuantity = info.AllowMinusQuantity;
			base.FractionCalculate = info.FractionCalculate;
			base.PartialQuantity = info.PartialQuantity;
			base.Host1 = info.Host1;
			base.Host2 = info.Host2;
			base.Timeout = info.Timeout;
			base.Retry = info.Retry;
			base.SameBarcodeInLocation = info.SameBarcodeInLocation;
			base.DefaultHost = info.DefaultHost;
		

			base.AllowZeroQuantity = info.AllowZeroQuantity;
			base.MaxQuantity = info.MaxQuantity;
			base.LastSync = info.LastSync;
			base.IsInvertWordsConfig = info.IsInvertWordsConfig;
			base.IsInvertLettersConfig = info.IsInvertLettersConfig;

			base.ConfirmNewLocation = info.ConfirmNewLocation;				
			base.ConfirmNewItem = info.ConfirmNewItem;
			base.AutoSendData = info.AutoSendData;

			base.AllowQuantityFraction = info.AllowQuantityFraction;
			base.AddExtraInputValue = info.AddExtraInputValue;
			base.AddExtraInputValueHeaderName = info.AddExtraInputValueHeaderName;
							
            this.Export();
        }

		//public override void RunClear(ExportClearCommandInfo info)
		//{
		//	this.Clear();
		//	if (info.Callback != null) Utils.RunOnUI(info.Callback);
		//}

		public override void RunClear()
		{
			this.Clear();
		}

		
        public void Export()
        {
			if (this.State != null)
			{
				if (this.CurrentCustomer != null)
				{
					base.CustomerCode = this.CurrentCustomer.Code;
					base.CustomerName = this.CurrentCustomer.Name;
				}
			}

            string processCont4uFolder = base.GetExportToPDAFolderPath(false) + @"\Process";
            string objectCodeFolder = base.GetExportToPDAFolderPath(true);
			string processToHTFolder = this.MISiDnextDataPath.Trim('\\') + @"\toHT";
			//string exeFolder = this._misiDnextDataPath.Trim('\\') + @"\PC_SW\";

			if (Directory.Exists(processToHTFolder) == false)
			{
				try { Directory.CreateDirectory(processToHTFolder); }
				catch { }
			}

			//base.ClearFolder(processCont4uFolder);
			//base.ClearFolder(objectCodeFolder);

			ClearFolders(base.State);
			//string invertLetters = "1";
			//string invertWords = "1";


            string fromLookupFileInProcessFolder = String.Empty;
            if (base.FileType != 0)	  //No LookupFile
            {
            

                IExportProvider provider =
					base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCatalogPdaMISFileProvider.ToString());
				
                provider.Parms.Clear();
                provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
                provider.Parms[ImportProviderParmEnum.FileType] = base.FileType;
                provider.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
                provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertWords ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty; 
                provider.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider.ProviderEncoding = base.Encoding;

                provider.FromPathDB = base.GetDbPath;
                fromLookupFileInProcessFolder = processCont4uFolder + @"\" + @"Lookup.csv";
                provider.ToPathFile = fromLookupFileInProcessFolder;
                provider.Export();

				string toHTPathFile = processToHTFolder + @"\" + @"Lookup.csv";
				base.CopyFile(fromLookupFileInProcessFolder, toHTPathFile);

				if (File.Exists(toHTPathFile) == false)
				{
					Thread.Sleep(1000); //wait
					if (File.Exists(toHTPathFile) == false)
					{
						Thread.Sleep(2000); //wait
						if (File.Exists(toHTPathFile) == false)
						{
							Thread.Sleep(3000); //wait
							if (File.Exists(toHTPathFile) == false)
							{
								base.CopyFile(fromLookupFileInProcessFolder, toHTPathFile);
							}
						}
					}
				}

				string toObjectCodePathFile = objectCodeFolder + @"\" + @"Lookup.csv";
				base.CopyFile(fromLookupFileInProcessFolder, toObjectCodePathFile);

				if (File.Exists(toObjectCodePathFile) == false)
				{
					Thread.Sleep(1000); //wait
					if (File.Exists(toObjectCodePathFile) == false)
					{
						Thread.Sleep(2000); //wait
						if (File.Exists(toObjectCodePathFile) == false)
						{
							Thread.Sleep(3000); //wait
							if (File.Exists(toObjectCodePathFile) == false)
							{
								base.CopyFile(fromLookupFileInProcessFolder, toObjectCodePathFile);
							}
						}
					}
				}
			

			}

            IExportProvider provider1 =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportIturPdaMISFileProvider.ToString());
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.Parms[ImportProviderParmEnum.IturNameType] = base.IturNameType ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.IturNamePrefix] = base.IturNamePrefix;
			provider1.Parms[ImportProviderParmEnum.InvertPrefix] = base.InvertPrefix ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.IsAddBinarySearch] = base.IsAddBinarySearch ? "1" : String.Empty;

			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			
			provider1.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider1.ProviderEncoding = base.Encoding;					 //add Gerrit

            //provider1.Parms.Add(ImportProviderParmEnum.WithoutProductName, base.WithoutProductName);
            //provider1.Parms.Add(ImportProviderParmEnum.BarcodeWithoutMask, base.BarcodeWithoutMask);
            //provider1.Parms.Add(ImportProviderParmEnum.MakatWithoutMask, base.MakatWithoutMask);

            provider1.FromPathDB = base.GetDbPath;
			string pathFile1 = processCont4uFolder + @"\" + @"Locations.csv";
            provider1.ToPathFile = pathFile1;
            provider1.Export();					//!!! вернуть

			string codePathFile1 = objectCodeFolder + @"\" + @"Locations.csv";
            base.CopyFile(pathFile1, codePathFile1);

			if (File.Exists(codePathFile1) == false)
			{
				Thread.Sleep(1000); //wait
				if (File.Exists(codePathFile1) == false)
				{
					Thread.Sleep(2000); //wait
					if (File.Exists(codePathFile1) == false)
					{
						Thread.Sleep(3000); //wait
						if (File.Exists(codePathFile1) == false)
						{
							base.CopyFile(pathFile1, codePathFile1);
						}
					}
				}
			}

			string toHTPathFile1 = processToHTFolder + @"\" + @"Locations.csv";
			base.CopyFile(pathFile1, toHTPathFile1);
			if (File.Exists(toHTPathFile1) == false)
			{
				Thread.Sleep(1000);
				if (File.Exists(toHTPathFile1) == false)
				{
					Thread.Sleep(2000);
					if (File.Exists(toHTPathFile1) == false)
					{
						Thread.Sleep(3000);
						if (File.Exists(toHTPathFile1) == false)
						{
							base.CopyFile(pathFile1, toHTPathFile1);
						}
					}
				}
			}

            IExportProvider provider2 =
                base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCustomerMISConfigToFileProvider.ToString());
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.Parms[ImportProviderParmEnum.Hash] = base.Hash;
			//provider2.Parms[ImportProviderParmEnum.FileType] = base.FileType;
            provider2.Parms[ImportProviderParmEnum.QType] = base.QType;
			//provider2.Parms[ImportProviderParmEnum.UseAlphaKey] = base.UseAlphaKey;
			//provider2.Parms[ImportProviderParmEnum.ClientId] = base.ClientId;
			provider2.Parms[ImportProviderParmEnum.Password] = base.Password;
			//provider2.Parms[ImportProviderParmEnum.NewItem] = base.NewItem;
			provider2.Parms[ImportProviderParmEnum.NewItemBool] = base.NewItemBool;
			provider2.Parms[ImportProviderParmEnum.ChangeQuantityType] = base.ChangeQuantityType;

			//provider2.Parms[ImportProviderParmEnum.IturNameType] = base.IturNameType ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.IturNamePrefix] = base.IturNamePrefix;
			//provider2.Parms[ImportProviderParmEnum.InvertPrefix] = base.InvertPrefix ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.CustomerCode] = base.CustomerCode;
            provider2.Parms[ImportProviderParmEnum.CustomerName] = base.CustomerName;
			//string branchCodeLocal = base.CurrentBranch == null ? "000" : base.CurrentBranch.BranchCodeLocal;
			string branchCode = base.CurrentBranch == null ? "000" : base.CurrentBranch.Code;
			string branchName = base.CurrentBranch == null ? "000" : base.CurrentBranch.Name;
			provider2.Parms[ImportProviderParmEnum.BranchCode] = branchCode;
			provider2.Parms[ImportProviderParmEnum.BranchName] = branchName;
			string inventorCode = base.CurrentInventor == null ? "" : base.CurrentInventor.Code;
			provider2.Parms[ImportProviderParmEnum.InventorCode] = inventorCode;
            provider2.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider2.Parms[ImportProviderParmEnum.LookUpEXE] = base.LookUpEXE;
			provider2.Parms[ImportProviderParmEnum.HTcalculateLookUp] = base.HTcalculateLookUp;
			provider2.Parms[ImportProviderParmEnum.AddNewLocation] = base.AddNewLocation;
			provider2.Parms[ImportProviderParmEnum.AddExtraInputValueSelectFromBatchListForm] = base.AddExtraInputValueSelectFromBatchListForm;
			provider2.Parms[ImportProviderParmEnum.AllowNewValueFromBatchListForm] = base.AllowNewValueFromBatchListForm;
			provider2.Parms[ImportProviderParmEnum.SearchIfExistInBatchList] = base.SearchIfExistInBatchList;
			provider2.Parms[ImportProviderParmEnum.AllowMinusQuantity] = base.AllowMinusQuantity;
			provider2.Parms[ImportProviderParmEnum.FractionCalculate] = base.FractionCalculate;
			provider2.Parms[ImportProviderParmEnum.PartialQuantity] = base.PartialQuantity;
			provider2.Parms[ImportProviderParmEnum.Host1] = base.Host1;
			provider2.Parms[ImportProviderParmEnum.Host2] = base.Host2;
			provider2.Parms[ImportProviderParmEnum.Timeout] = base.Timeout;
			provider2.Parms[ImportProviderParmEnum.Retry] = base.Retry;
			provider2.Parms[ImportProviderParmEnum.SameBarcodeInLocation] = base.SameBarcodeInLocation;
			provider2.Parms[ImportProviderParmEnum.DefaultHost] = base.DefaultHost;
			
			provider2.Parms[ImportProviderParmEnum.AllowZeroQuantity] = base.AllowZeroQuantity;
			provider2.Parms[ImportProviderParmEnum.MaxQuantity] = base.MaxQuantity;
			provider2.Parms[ImportProviderParmEnum.LastSync] = base.LastSync;
			provider2.Parms[ImportProviderParmEnum.InvertWordsConfig] = base.IsInvertWordsConfig? "1" : String.Empty;		
			provider2.Parms[ImportProviderParmEnum.InvertLettersConfig] = base.IsInvertLettersConfig? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.SearchDef] =base.SearchDef; //TODO
			provider2.Parms[ImportProviderParmEnum.ConfirmNewLocation] = base.ConfirmNewLocation;		 
			provider2.Parms[ImportProviderParmEnum.ConfirmNewItem] = base.ConfirmNewItem;
			provider2.Parms[ImportProviderParmEnum.AutoSendData] = base.AutoSendData;

			provider2.Parms[ImportProviderParmEnum.AllowQuantityFraction] = base.AllowQuantityFraction;
			provider2.Parms[ImportProviderParmEnum.AddExtraInputValue] = base.AddExtraInputValue;
			provider2.Parms[ImportProviderParmEnum.AddExtraInputValueHeaderName] = base.AddExtraInputValueHeaderName;

			provider2.ProviderEncoding = base.Encoding;					 //add Gerrit

			string pathFile2 = processCont4uFolder + @"\" + @"PreSettings.ini";
            provider2.ToPathFile = pathFile2;
            provider2.Export();

			string codePathFile2 = objectCodeFolder + @"\" + @"PreSettings.ini";
            base.CopyFile(pathFile2, codePathFile2);

			string toHTPathFile2 = processToHTFolder + @"\" + @"PreSettings.ini";
			base.CopyFile(pathFile2, toHTPathFile2);

            IIniFileInventor iniFileInventor =
                base.ServiceLocator.GetInstance<IIniFileInventor>();
            string paramsFolderPath = iniFileInventor.BuildParamsFolderPath(); //??

			string codePathFile5 = paramsFolderPath + @"\" + @"PreSettings.ini";
            base.CopyFile(pathFile2, codePathFile5);

			//=======================================================================
			this.BuildLookupFile();

			string fromFolderSqlFile = processToHTFolder + @"\" + @"Lookup.sql";
			if (File.Exists(fromFolderSqlFile) == true)
			{
				string toObjectCodePathSqlFile = objectCodeFolder + @"\" + @"Lookup.sql";
				base.CopyFile(fromFolderSqlFile, toObjectCodePathSqlFile);

				string toProcessCont4uSqlFile = processCont4uFolder + @"\" + @"Lookup.sql";
				base.CopyFile(fromFolderSqlFile, toProcessCont4uSqlFile);
			}

			//IExportProvider provider3 =
			//	base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportUserIniToFileProvider.ToString());
			//provider3.Parms.Clear();
			//provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);

			//IDBSettings settings = base.ServiceLocator.GetInstance<IDBSettings>();
			//string fromPathFile = settings.ExecutablePath().Trim('\\') + @"\" + @"op.txt";
			//provider3.FromPathDB = Path.GetFullPath(fromPathFile);
			//string toPathFile = processFolder + @"\" + @"op.txt";
			//provider3.ToPathFile = toPathFile;
			//provider3.Export();

			//string codePathFile3 = objectCodeFolder + @"\" + @"op.txt";
			//base.CopyFile(toPathFile, codePathFile3);

			//string toHTPathFile3 = processToHTFolder + @"\" + @"op.txt";
			//base.CopyFile(lookupFileInProcessFolder, toHTPathFile3);

			//BuildLookupFile(processFolder, objectCodeFolder, lookupFileInProcessFolder);

			//CopyProgramTypeFiles(processFolder, objectCodeFolder);
			//CopyProgramTypeFiles(processFolder, processToHTFolder); //??

			SaveFileLog(ExportPdaAdapterName.ExportPdaMISAdapter);
        }

        public void Clear()
        {
            base.LogImport.Clear();
			ClearFolders(base.State);
      
            UpdateLogFromILog();
			try
			{
				RunWebService();
			}
			catch (Exception ecx)
			{
				string message = ecx.Message;
			}

		}

		public override void ClearFolders(CBIState state) 
		{
			if (base.State == null)	 
			{
				base.State = state;
			}
			string processCont4uFolder = base.GetExportToPDAFolderPath(false) + @"\" + "Process";
			string objectCodeFolder = base.GetExportToPDAFolderPath(true);
			string processToHTFolder = this.MISiDnextDataPath.Trim('\\') + @"\toHT";

			if (Directory.Exists(processToHTFolder) == false)
			{
				try { Directory.CreateDirectory(processToHTFolder); }
				catch { }
			}

			IExportProvider provider =
				base.ServiceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportIturPdaMISFileProvider.ToString());
			provider.ToPathFile = processCont4uFolder + @"\" + @"Lookup.csv";
			provider.Clear();
			provider.ToPathFile = objectCodeFolder + @"\" + @"Lookup.csv";
			provider.Clear();
			provider.ToPathFile = processToHTFolder + @"\" + @"Lookup.csv";
			provider.Clear();

			provider.ToPathFile = processCont4uFolder + @"\" + @"Lookup.sql";
			provider.Clear();
			provider.ToPathFile = objectCodeFolder + @"\" + @"Lookup.sql";
			provider.Clear();
			provider.ToPathFile = processToHTFolder + @"\" + @"Lookup.sql";
			provider.Clear();


			provider.ToPathFile = processCont4uFolder + @"\" + @"Locations.csv";
			provider.Clear();
			provider.ToPathFile = objectCodeFolder + @"\" + @"Locations.csv";
			provider.Clear();
			provider.ToPathFile = processToHTFolder + @"\" + @"Locations.csv";
			provider.Clear();

			provider.ToPathFile = processCont4uFolder + @"\" + @"PreSettings.ini";
			provider.Clear();
			provider.ToPathFile = objectCodeFolder + @"\" + @"PreSettings.ini";
			provider.Clear();
			provider.ToPathFile = processToHTFolder + @"\" + @"PreSettings.ini";
			provider.Clear();
		}

		private void BuildLookupFile()
        {
			//C:\MIS\IDnextData
			if (String.IsNullOrEmpty(this.MISiDnextDataPath)) return;
			if (Directory.Exists(this.MISiDnextDataPath) == false) return; //C:\MIS\IDnextData\

			string exeFolder = this.MISiDnextDataPath.Trim('\\') + @"\";//C:\MIS\IDnextData\

			string convertExe = String.Format("{0}KClient.exe", exeFolder);   //C:\MIS\IDnextData\KClient.exe LookUp
			string paramLine = String.Format(@"{0}", "Lookup"); //"Lookup"

			string resultSourceFile = Path.Combine(exeFolder, "Lookup.csv");
			string resultDestFile = Path.Combine(exeFolder + "toHT", "lookup.sql");

			if (File.Exists(resultDestFile) == true)	File.Delete(resultDestFile);
			Thread.Sleep(200);

		    Process pr = System.Diagnostics.Process.Start(convertExe, paramLine);
            pr.WaitForExit();

			Thread.Sleep(1000);
        }

		private void RunWebService()
		{
			//C:\MIS\IDnextData
			//if (String.IsNullOrEmpty(this.MISiDnextDataPath)) return;
			//if (Directory.Exists(this.MISiDnextDataPath) == false) return; //C:\MIS\IDnextData\

			//string exeFolder = this.MISiDnextDataPath.Trim('\\') + @"\";//C:\MIS\IDnextData\

			//string convertExe = String.Format("{0}KClient.exe", exeFolder);   //C:\MIS\IDnextData\KClient.exe LookUp
			//string paramLine = String.Format(@"{0}", "Lookup"); //"Lookup"

			//string resultSourceFile = Path.Combine(exeFolder, "Lookup.csv");
			//string resultDestFile = Path.Combine(exeFolder + "toHT", "lookup.sql");

			//if (File.Exists(resultDestFile) == true) File.Delete(resultDestFile);
			//Thread.Sleep(200);

			//string webserviceExe = @"C:\Count4U\publish\webapi_count4u\WebAPI.Count4U.Model.exe";
			string webserviceExe = @"C:\Count4U\trunk\Count4U\fromFtp.bat";
		  Process pr = System.Diagnostics.Process.Start(webserviceExe);
	
		}

		private void CopyProgramTypeFiles(string processFolder, string objFolder)
        {
            try
            {
                string sourceDir = base.GetExportToPDAFolderPath(false);
                sourceDir = Path.Combine(sourceDir, Common.Constants.ExportPdaAdapterName.ExportHT630Adapter);
                if (!Directory.Exists(sourceDir))
                {
                    Directory.CreateDirectory(sourceDir);
                }

                string[] folders = new string[] { base.PDAType, base.MaintenanceType, base.ProgramType };

                foreach (string folder in folders)
                {
                    if (String.IsNullOrWhiteSpace(folder)) continue;

                    string finalDir = Path.Combine(sourceDir, folder.Trim('"'));

                    if (!Directory.Exists(finalDir))
                    {
                        Directory.CreateDirectory(finalDir);
                    }

                    foreach (string file in Directory.EnumerateFiles(finalDir))
                    {
                        FileInfo fi = new FileInfo(file);

                        string[] targetFolders = new string[] { processFolder, objFolder };

                        foreach (string targetFolder in targetFolders)
                        {
                            string targetFile = Path.Combine(targetFolder, fi.Name);

                            if (File.Exists(targetFile))
                            {
                                File.Delete(targetFile);
                            }

                            File.Copy(fi.FullName, targetFile);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CopyProgramTypeFiles", exc);
            }
        }

		[NotInludeAttribute]
		public string MISiDnextDataPath
		{
			get
			{
				if (Directory.Exists(this._misiDnextDataPath) == false)
				{
					try { Directory.CreateDirectory(this._misiDnextDataPath); }
					catch { }
				}
				if (Directory.Exists(this._misiDnextDataPath) == true)
				{
					return this._misiDnextDataPath;
				}
				else return "";
			}
		}

		//public string MISCommunicatorPath
		//{
		//	get
		//	{
		//		if (Directory.Exists(this._misCommunicatorPath) == false)
		//		{
		//			try { Directory.CreateDirectory(this._misCommunicatorPath); }
		//			catch { }
		//		}
		//		if (Directory.Exists(this._misCommunicatorPath) == true)
		//		{
		//			return this._misCommunicatorPath;
		//		}
		//		else return "";
		//	}
		//}
    }
}