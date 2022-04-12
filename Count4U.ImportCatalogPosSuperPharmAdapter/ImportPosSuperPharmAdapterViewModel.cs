using System;
using System.Collections.Generic;
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
using System.IO;
using Count4U.Common.ViewModel;

namespace Count4U.ImportCatalogPosSuperPharmAdapter
{
    public class ImportPosSuperPharmAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
        private CatalogParserPoints _catalogParserPoints1;
        private CatalogParserPoints _catalogParserPoints2;
        string _branchErpCode = String.Empty;
		
        private bool _withQuantityERP;

        public ImportPosSuperPharmAdapterViewModel(IServiceLocator serviceLocator,
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

        public bool WithQuantityErp
        {
            get { return _withQuantityERP; }
            set
            {
                _withQuantityERP = value;
             
				if (value == true)
				{
					base.StepTotal = 3;
				}
				else
				{
					base.StepTotal = 2;
				}
				RaisePropertyChanged(() => WithQuantityErp);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();

            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));
			this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
        }

        protected override bool PreImportCheck()
        {

            return base.ContinueAfterBranchERPWarning(base.Path1, @"toinf(.+)\.txt") &&
                   base.ContinueAfterBranchERPWarning(base.Path2, @"extra(.+)\.txt");
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

            this.PathFilter1 = "*.txt|*.txt|All files (*.*)|*.*";
            this.PathFilter2 = "*.txt|*.txt|All files (*.*)|*.*";

            this._fileName1 = String.Format("toinf{0}.txt", this._branchErpCode);
            this._fileName2 = String.Format("extra{0}.txt", this._branchErpCode);

            base.Encoding = System.Text.Encoding.GetEncoding(862);
            base.IsInvertLetters = true;
            base.IsInvertWords = true;


            this.InitCatalogParserPoints1();			//this._catalogParserPoints1
            this.InitCatalogParserPoints2();			//this._catalogParserPoints2

			if (this._withQuantityERP == true)
			{
				base.StepTotal = 3;
			}
			else
			{
				base.StepTotal = 2;
			}
        }

        public override void InitFromIni()
        {
	
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportCatalogNetPOSSuperPharmADOProvider");
            Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogNetPOSSuperPharmADOProvider1");

            //init GUI
            this._fileName1 = iniData1.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
            this._fileName2 = iniData2.SetValue(ImportProviderParmEnum.FileName1, this._fileName2);
            //todo
            if (this._fileName1.Contains("XXX") == true)
            {
                this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
            }
            if (this._fileName2.Contains("XXX") == true)
            {
                this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
            }
            base.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
            base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

            //init Provider Parms
            this._catalogParserPoints1 = iniData1.SetValue(this._catalogParserPoints1);
            this._catalogParserPoints2 = iniData2.SetValue(this._catalogParserPoints2);
        }

        private void InitCatalogParserPoints1()
        {
            this._catalogParserPoints1 = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 1,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 13,
                CatalogItemNameStart = 14,
                CatalogItemNameEnd = 38,
                UnitTypeCodeStart = 39,
                UnitTypeCodeEnd = 40,
                QuantityERPStart = 41,
                QuantityERPEnd = 48,
                SectionCodeStart = 49,
                SectionCodeEnd = 50,
                CatalogPriceSaleStart = 51,
                CatalogPriceSaleEnd = 57,
                CatalogPriceBuyStart = 58,
                CatalogPriceBuyEnd = 64
            };
        }

        private void InitCatalogParserPoints2()
        {
            this._catalogParserPoints2 = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 1,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 13,
                HamarotBarcodeStart = 15,
                HamarotBarcodeEnd = 27
            };
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
				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmADOProvider);
			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmBulkProvider);
			provider1.SetColumnMappings<Product>();
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			provider1.FromPathFile = this.Path1;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmADOProvider1);
			provider2.ToPathDB = base.GetDbPath;
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path2;
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			provider2.ProviderEncoding = base.Encoding; //this._encoding2;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			provider2.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
				provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}

			if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
				provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
				provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}

			this.StepCurrent = 1;
			provider1.Import();
			this.StepCurrent = 2;
			provider2.Import();
			

			if (this._withQuantityERP == true)
			{
					IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmUpdateERPQuentetyADOProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				//provider1.Clear();
				provider3.FromPathFile = this.Path1;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

				if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
				{
					MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
					provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				}
				if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
				{
					MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
					provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				}

				this.StepCurrent = 3;
				provider3.Import();

			}            

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }
    }
}