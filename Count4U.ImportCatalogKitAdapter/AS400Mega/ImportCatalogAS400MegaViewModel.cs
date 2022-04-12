using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Common.ViewModel;

namespace Count4U.ImportCatalogKitAdapter.AS400Mega
{
	public class ImportCatalogAS400MegaViewModel : TemplateAdapterOneFileViewModel
	{
		public string _fileName { get; set; }

		private string _delimiter = String.Empty;
		private string _branchErpCode = String.Empty;

		private CatalogParserPoints _catalogParserPoints1;
	//	private CatalogParserPoints _catalogParserPoints2;

		// private bool _withQuantityERP;
		private bool _importSupplier;

		public ImportCatalogAS400MegaViewModel(IServiceLocator serviceLocator,
		  IContextCBIRepository contextCBIRepository,
		  IEventAggregator eventAggregator,
		  IRegionManager regionManager,
		  ILog logImport,
		  IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
		  IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
		{
			base.ParmsDictionary.Clear();

		}

		//    public bool WithQuantityErp
		//    {
		//        get { return _withQuantityERP; }
		//        set
		//        {
		//            this._withQuantityERP = value;
		//base.StepTotal = GetStep();
		//RaisePropertyChanged(() => WithQuantityErp);

		//            if (base.RaiseCanImport != null)
		//                base.RaiseCanImport();
		//        }
		//    }

		public bool ImportSupplier
		{
			get { return this._importSupplier; }
			set
			{
				this._importSupplier = value;
				base.StepTotal = GetStep();
				RaisePropertyChanged(() => ImportSupplier);
			}
		}

		private int GetStep()
		{
			int step = 1;
			//if (this.WithQuantityErp == true) step++;
			if (this.ImportSupplier == true) step++;
			return step;
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			this._maskViewModel = this.BuildMaskControl("1", base.BuildMaskRegionName());
			if (this._maskViewModel != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask) == false)
				{
					this._maskViewModel.MakatMask = this._makatMask;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask) == false)
				{
					this._maskViewModel.BarcodeMask = this._barcodeMask; //init Default
				}
			}

		}

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)       //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						if (this._fileName.Contains("XXX") == true)
						{
							this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
						}

						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
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
		}

		protected override void RunImport(ImportCommandInfo info)
		{
			this.Import();
		}

		protected override void RunClear()
		{
			this.Clear();
		}


		public override void InitDefault(CBIState state = null)
		{
			if (state != null) base.State = state;


			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			//   this._withQuantityERP = false;
			this._importSupplier = true;

			//this.PathFilter = "All files (*.*)|*.*";
			//this._fileName = String.Format("KB99INFO.{0}", _branchErpCode);

			this.PathFilter = "*All files (*.*)|*.*";
			//this.PathFilter2 = "*All files (*.*)|*.*";

			//this._fileName1 = String.Format("CATI6_DB.txt", _branchErpCode);
			//this._fileName2 = String.Format("PLUEXTRA.{0}", _branchErpCode);
			//	this._fileName1 = "CATI6_DB.txt";  //OLD!!
			this._fileName = "it_comax.txt";
			//	this._fileName2 = "TOI6#DB.txt";

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;

			//!!! пока не пользовать
			this.MakatMask = "0000000000000{F}";
			this.BarcodeMask = "7290000000000{F}";                 //"0000000000000{F}"; //OLD !!

			//this.MakatMask2 = "0000000000000{F}";

			base.StepTotal = GetStep();

			this.InitCatalogParserPoints1();            //this._catalogParserPoints1
														//	this.InitCatalogParserPoints2();			//this._catalogParserPoints2
		}

		public override void InitFromIni()
		{

			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogAS400MegaAdapter.ini"));

			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportCatalogAS400MegaProvider", base.GetPathToIniFile("Count4U.ImportCatalogAS400MegaAdapter.ini"));
			//		Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogAS400MegaProvider1", base.GetPathToIniFile("Count4U.ImportCatalogAS400MegaAdapter.ini"));

			//init GUI
			this._fileName = iniData1.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			//	this._fileName2 = iniData2.SetValue(ImportProviderParmEnum.FileName1, this._fileName2);

			this._delimiter = iniData.SetValue(ImportProviderParmEnum.Delimiter, this._delimiter);
			//todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
			//if (this._fileName2.Contains("XXX") == true)
			//{
			//    this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
			//}
			base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			//      base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			//		if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;

			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

			this._catalogParserPoints1 = iniData1.SetValue(this._catalogParserPoints1);
			//this._catalogParserPoints2 = iniData2.SetValue(this._catalogParserPoints2);
		}

		private void InitCatalogParserPoints1()
		{
			this._catalogParserPoints1 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 26,
				HamarotBarcodeStart = 1,
				HamarotBarcodeEnd = 13,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 13,
				CatalogItemNameStart = 14,
				CatalogItemNameEnd = 29,

				SectionCodeStart = 59,
				SectionCodeEnd = 60,
				CatalogPriceSaleStart = 41,
				CatalogPriceSaleEnd = 48,

				SupplierCodeStart = 76,
				SupplierCodeEnd = 85,
				SupplierNameStart = 86,
				SupplierNameEnd = 105,
			};
		}

		//OLD !!! 
		//private void InitCatalogParserPoints1()
		//{
		//	this._catalogParserPoints1 = new CatalogParserPoints
		//	{
		//		CatalogMinLengthIncomingRow = 26,
		//		HamarotBarcodeStart = 1,
		//		HamarotBarcodeEnd = 13,
		//		CatalogItemCodeStart = 1,
		//		CatalogItemCodeEnd = 13, 
		//		CatalogItemNameStart = 28,
		//		CatalogItemNameEnd = 47,

		//		SectionCodeStart=48, 
		//		SectionCodeEnd=49,
		//		CatalogPriceSaleStart = 59,
		//		CatalogPriceSaleEnd = 66, 

		//		SupplierCodeStart = 76, 
		//		SupplierCodeEnd = 85,	
		//		SupplierNameStart = 86, 
		//		SupplierNameEnd = 105,
		//	};
		//}

		//private void InitCatalogParserPoints2()
		//{
		//	this._catalogParserPoints2 = new CatalogParserPoints
		//	{
		//		CatalogItemCodeStart = 1,
		//		CatalogItemCodeEnd = 13,
		//		QuantityERPStart = 14,
		//		QuantityERPEnd = 25,
		//		QuantityTypeStart = 39,
		//		QuantityTypeEnd = 39, 

		//	};
		//}

		protected override bool PreImportCheck()
		{
			return true;
		}

		public override void Import()
		{
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

			string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400MegaProvider);
			provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
			provider.FromPathFile = this.Path;
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
			provider.ProviderEncoding = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400MegaProvider1);
			//provider1.ToPathDB = base.GetDbPath;
			//provider1.FastImport = base.IsTryFast;
			//provider1.FromPathFile = this.Path1;
			//provider1.Parms.Clear();
			//provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
			//provider1.ProviderEncoding = base.Encoding;
			//provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;



			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				//provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				//   provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}

			this.StepCurrent = 1;
			provider.Import();

			//this.StepCurrent = 2;
			//provider1.Import();


			if (this.ImportSupplier == true)
			{
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierAS400MegaProvider);
				provider2.ToPathDB = base.GetDbPath;
				provider2.FastImport = base.IsTryFast;
				provider2.FromPathFile = this.Path;
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
				provider2.ProviderEncoding = base.Encoding;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

				this.StepCurrent = this.StepCurrent + 1;
				provider2.Import();
			}


			//if (this.WithQuantityErp == true)
			//{
			//	IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400MegaUpdateERPQuentetyProvider);
			//	provider3.ToPathDB = base.GetDbPath;
			//	provider3.FastImport = base.IsTryFast;
			//	provider3.FromPathFile = this.Path2;
			//	provider3.Parms.Clear();
			//	provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	provider3.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			//	provider3.ProviderEncoding = base.Encoding;
			//	provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//	provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//	provider3.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			//	provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//	this.StepCurrent = this.StepCurrent + 1;

			//if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
			//{
			//	MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
			//	provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			//}
			//if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
			//{
			//	MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
			//	provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			//}

			//			provider3.Import();

		//}




		FileLogInfo fileLogInfo = new FileLogInfo();
		fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
	}


        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400MegaProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();


			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierAS400MegaProvider);
			provider3.ToPathDB = base.GetDbPath;
			provider3.Clear();
			
            UpdateLogFromILog();
        }

        //protected override void OnPath1Changed()
        //{
        //    base.OnPath1Changed();


        //}

        //protected override void OnPath2Changed()
        //{
        //    base.OnPath2Changed();

        //}
    }
}
