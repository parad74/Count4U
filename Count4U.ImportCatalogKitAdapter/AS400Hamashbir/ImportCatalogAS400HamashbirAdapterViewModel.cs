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

namespace Count4U.ImportCatalogKitAdapter.AS400Hamashbir
{
    public class ImportCatalogAS400HamashbirAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		//public string _fileName1_2;
		public string _fileName2 {get; set;}
		//private string Path3; 
		//private string Path4;
		//private string Path1_2;
		private string _delimiter = String.Empty;
		private string _branchErpCode = String.Empty;

		private CatalogParserPoints _catalogParserPoints1;
		//private CatalogParserPoints _catalogParserPoints1_2;
		private CatalogParserPoints _catalogParserPoints2;

        private bool _withQuantityERP;
		private bool _importSection;
		private bool _importFamily;
		

		public ImportCatalogAS400HamashbirAdapterViewModel(IServiceLocator serviceLocator,
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
			this.ImportFamily = true;
			this.ImportSection = true;
        }

		//public bool WithQuantityErp
		//{
		//	get { return _withQuantityERP; }
		//	set
		//	{
		//		this._withQuantityERP = value;
		//		if (value == true)
		//		{
		//			base.StepTotal = 3;
		//		}
		//		else
		//		{
		//			base.StepTotal = 2;
		//		}
		//		if (this.ImportSection == true) base.StepTotal = base.StepTotal + 1;

		//		RaisePropertyChanged(() => WithQuantityErp);

		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();
		//	}
		//}



		public bool ImportFamily
		{
			get { return this._importFamily; }
			set
			{
				this._importFamily = value;
				base.StepTotal = GetStep();
				RaisePropertyChanged(() => ImportFamily);
			}
		}



		public bool ImportSection
		{
			get { return this._importSection; }
			set
			{
				this._importSection = value;
				base.StepTotal = GetStep();
				RaisePropertyChanged(() => ImportSection);
			}
		}

		private int GetStep()
		{
			int step = 2;
			if (this.ImportSection == true) step++;
			if (this.ImportFamily == true) step++;
			return step;
		}
	
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));
			this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
        }

        public override bool CanImport()
        {
			//if (_withQuantityERP)
				return (String.IsNullOrWhiteSpace(base.Path1) == false)
					   && (String.IsNullOrWhiteSpace(base.Path2) == false)
					   && this.IsOkPath(this.Path1)
					   && this.IsOkPath(this.Path2);

			//return (String.IsNullOrWhiteSpace(base.Path1) == false)
			//		 && this.IsOkPath(this.Path1)
			//		 ;
        }


		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						if (this._fileName1.Contains("XXX") == true)
						{
							this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
						}

						if (this._fileName2.Contains("XXX") == true)
						{
							this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
						}
						base.Path1 = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName1);
						base.Path2 = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName2);
						if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
						if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
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

		//public string PathFilter1_2
		//{
		//	get { return this._fileName1_2; }
		//	set { this._fileName1_2 = value; }
		//}

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

            this._withQuantityERP = false;

			//this.PathFilter = "All files (*.*)|*.*";
			//this._fileName = String.Format("KB99INFO.{0}", _branchErpCode);

			this.PathFilter1 = "*All files (*.*)|*.*";
		//	this.PathFilter1_2 = "*All files (*.*)|*.*";
			this.PathFilter2 = "*All files (*.*)|*.*";

			this._fileName1 = String.Format("SPARITEWSN.{0}", _branchErpCode);
			this._fileName2 = String.Format("SPRUPCEWS.{0}", _branchErpCode);
			//this._fileName2 = String.Format("SBOUTML.{0}", _branchErpCode);

            base.Encoding = System.Text.Encoding.GetEncoding(862);
            base.IsInvertLetters = true;
			base.IsInvertWords = true;

			base.StepTotal = 2;
		
			base.StepTotal = GetStep();


			this.InitCatalogParserPoints1();			//this._catalogParserPoints1
			//this.InitCatalogParserPoints1_2();			//this._catalogParserPoints2
			this.InitCatalogParserPoints2();	
        }

        public override void InitFromIni()
        {
				Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogAS400HamashbirAdapter.ini"));

			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportCatalogAS400HamashbirProvider", base.GetPathToIniFile("Count4U.ImportCatalogAS400HamashbirAdapter.ini"));
			Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogAS400HamashbirProvider1", base.GetPathToIniFile("Count4U.ImportCatalogAS400HamashbirAdapter.ini"));

			//init GUI
			this._fileName1 = iniData1.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
			this._fileName2 = iniData2.SetValue(ImportProviderParmEnum.FileName1, this._fileName2);

    		this._delimiter = iniData.SetValue(ImportProviderParmEnum.Delimiter, this._delimiter);
            //todo
            if (this._fileName1.Contains("XXX") == true)
            {
                this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
            }
			//if (this._fileName1_2.Contains("XXX") == true)
			//{
			//	this._fileName1_2 = this._fileName1_2.Replace("XXX", this._branchErpCode);
			//}
            if (this._fileName2.Contains("XXX") == true)
            {
                this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
            }
            base.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
			//Path1_2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1_2);
            base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			//if (System.IO.Path.GetExtension(Path1_2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
			//string fileName3 = "MLAYXXX.csv";
			//fileName3 = fileName3.Replace("XXX", this._branchErpCode);
			//this.Path3 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + fileName3);

			//string fileName4 = "supplier.csv";
			//this.Path4 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + fileName4);

            base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
            base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

			this._catalogParserPoints1 = iniData1.SetValue(this._catalogParserPoints1);
			//this._catalogParserPoints1_2 = iniData2.SetValue(this._catalogParserPoints1_2);
			this._catalogParserPoints2 = iniData2.SetValue(this._catalogParserPoints2);
        }


//	Field1: CompanyCode (col 1-2)
//Field2: Item Code (col 3-10)
//Field3: Name (col 11-40)
//Field4: FamilyID (col 41-44)
//Field5: FamilyName (col 45-56)
//Field6: SectionID (col 57-60)
//Field7: SectionName (col 61-72)
//Field8: SupplierID (col 73-77)
//Field9: PriceSell (col 82-90)

		private void InitCatalogParserPoints1()
		{
			this._catalogParserPoints1 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 10,
				//Field2: Item Code (col 3-10)
				//Field3: Name (col 11-40)
				CatalogItemCodeStart = 3,
				CatalogItemCodeEnd = 10, 
				CatalogItemNameStart = 11,
				CatalogItemNameEnd = 40,
				//Field4: FamilyID (col 41-44)
				//Field5: FamilyName (col 45-56)
				FamilyCodeStart	=41,
				FamilyCodeEnd = 44,
				FamilyNameStart = 45,
				FamilyNameEnd = 56,
				//Field6: SectionID (col 57-60)
				//Field7: SectionName (col 61-72)
				SectionCodeStart = 57,
				SectionCodeEnd = 60,
				SectionNameStart = 61,
				SectionNameEnd = 72,
				//Field8: SupplierID (col 73-77)
				SupplierCodeStart = 73, 
				SupplierCodeEnd = 77,
				//Field9: PriceSell (col 82-90)
				CatalogPriceSaleStart = 82,
				CatalogPriceSaleEnd = 90 
			};
		}
//		During import catalog we insert the section (col41-44)
//You inserted 41-43 - need to insert 42-44 - in 41 there is "0" always - you can ignore i

		private void InitCatalogParserPoints2()
		{
			this._catalogParserPoints2 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 10,
				CatalogItemCodeStart = 3,
				CatalogItemCodeEnd = 10, 
				HamarotBarcodeStart = 11,
				HamarotBarcodeEnd = 23
			};
		}
		

		//private void InitCatalogParserPoints2()
		//{
		//	this._catalogParserPoints2 = new CatalogParserPoints
		//	{
		//		CatalogMinLengthIncomingRow = 25,
		//		HamarotBarcodeStart = 13,
		//		HamarotBarcodeEnd = 25,
		//		QuantityERPStart = 26,
		//		QuantityERPEnd = 31
		//	};
		//}

        protected override bool PreImportCheck()
        {
			//return true;
			//return base.ContinueAfterBranchERPWarning(base.Path2, @"MLAY_(.+)\.csv");//MLAY_{0}.csv
			return base.ContinueAfterBranchERPWarning(base.Path1, @"SPARITEWSN.(.+)") &&
				 base.ContinueAfterBranchERPWarning(base.Path2, @"SPRUPCEWS.(.+)");
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

			//ProductCatalogAS400HamashbirParser
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400HamashbirProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
			provider.FromPathFile = this.Path1;		//SPARITEWSN	   makat
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
           // provider.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//ProductCatalogAS400HamashbirParser1
			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400HamashbirProvider1);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			provider1.FromPathFile = this.Path2;		//SPRUPCEWS		  barcode
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

            if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
            {
                MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }
            if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
            {
                MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }

			this.StepCurrent = 1;
			provider.Import();

			this.StepCurrent = 2;
			provider1.Import();

			//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400HamashbirProvider2);
			//provider2.ToPathDB = base.GetDbPath;
			//provider2.FastImport = base.IsTryFast;
			//provider2.FromPathFile = this.Path2;   //SBOUTMLxx
			//provider2.Parms.Clear();
			//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			//provider2.ProviderEncoding = base.Encoding;
			//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.WithQuantityERP] = "1";
			//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//provider2.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;

			//if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
			//{
			//	MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
			//	provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			//}
			//if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
			//{
			//	MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
			//	provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			//}
			//this.StepCurrent = 3;
			//provider2.Import();

			//this.StepCurrent = 3;
			//provider3.Import();

			//if (WithQuantityErp == true)
			//{
			//	IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AprilUpdateERPQuentetyProvider);
			//	provider3.ToPathDB = base.GetDbPath;
			//	provider3.FastImport = base.IsTryFast;
			//	provider3.FromPathFile = this.Path1;
			//	provider3.Parms.Clear();
			//	provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	provider3.ProviderEncoding = base.Encoding; //this._encoding2;
			//	provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//	provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//	provider3.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			//	provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//	provider3.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;

			//	//if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
			//	//{
			//	//MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord("0000000000000{S}");
			//	//provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			//	//}
			//	//if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
			//	//{
			//	//	MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
			//	//	provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			//	//}

			//	this.StepCurrent = 4;
			//	provider3.Import();

			//}


			if (this.ImportSection == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportSectionAS400HamashbirProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path1;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
				//if (this.WithQuantityErp == true) this.StepCurrent = 4;
				//else 
				this.StepCurrent = 3;
				provider3.Import();
			}

			if (this.ImportFamily == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyAS400HamashbirProvider);
				provider4.ToPathDB = base.GetDbPath;
				provider4.FastImport = base.IsTryFast;
				provider4.FromPathFile = this.Path1;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding;
				provider4.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
				//if (this.WithQuantityErp == true) this.StepCurrent = 4;
				//else 
				this.StepCurrent = 3;
				provider4.Import();
			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AprilProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();

			if (this.ImportSection == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportSectionAS400HamashbirProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();
			}

			if (this.ImportFamily == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyAS400HamashbirProvider);
				provider4.ToPathDB = base.GetDbPath;
				provider4.Clear();
			}
			
            UpdateLogFromILog();
        }

        protected override void OnPath1Changed()
        {
            base.OnPath1Changed();


        }

        protected override void OnPath2Changed()
        {
            base.OnPath2Changed();

        }
    }
}
