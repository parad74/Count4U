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

namespace Count4U.ImportCatalogXtechMeuhedetAdapter
{
    public class ImportCatalogXTechMeuhedetViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
        private CatalogParserPoints _catalogParserPoints1;
        private CatalogParserPoints _catalogParserPoints2;
        private CatalogParserPoints _catalogParserPoints3;
		private string _branchErpCode = String.Empty;

        private bool _withQuantityERP;

        public ImportCatalogXTechMeuhedetViewModel(IServiceLocator serviceLocator,
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
                this._withQuantityERP = value;
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

        public override bool CanImport()
        {
            if (_withQuantityERP)
                return (String.IsNullOrWhiteSpace(base.Path1) == false)
                       && (String.IsNullOrWhiteSpace(base.Path2) == false)
                       && this.IsOkPath(this.Path1)
                       && this.IsOkPath(this.Path2);

            return (String.IsNullOrWhiteSpace(base.Path1) == false)
                     && this.IsOkPath(this.Path1)
                     ;
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

			//this.PathFilter1 = "*.dat|*.dat|All files (*.*)|*.*";
			//this.PathFilter2 = "*.dat|*.dat|All files (*.*)|*.*";

			//this._fileName1 = "sfpart.dat";//String.Format("sfpart{0}.dat", this._branchErpCode);
			//this._fileName2 = "sfmlay.dat";//String.Format("sfmlay{0}.dat", this._branchErpCode);

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;

			this.PathFilter1 = "*.txt|*.txt|All files (*.*)|*.*";
			this.PathFilter2 = "*.txt|*.txt|All files (*.*)|*.*";

			this._fileName1 = "sfpart#db.txt";//String.Format("sfpart{0}.dat", this._branchErpCode);
			this._fileName2 = "sfmlay#db.txt";//String.Format("sfmlay{0}.dat", this._branchErpCode);
	

            this.InitCatalogParserPoints1();			//this._catalogParserPoints1
            this.InitCatalogParserPoints2();			//this._catalogParserPoints2
            this.InitCatalogParserPoints3();			//this._catalogParserPoints3

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
            Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportCatalogXtechMeuhedetADOProvider1");
            Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogXtechMeuhedetADOProvider2");
            Dictionary<ImportProviderParmEnum, string> iniData3 = base.GetIniData("ImportCatalogXtechMeuhedetUpdateERPQuentetyADOProvider");

            //init GUI
            this._fileName1 = iniData1.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
            this._fileName2 = iniData3.SetValue(ImportProviderParmEnum.FileName1, this._fileName2);
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
            //init Provider Parms
            this._catalogParserPoints1 = iniData1.SetValue(this._catalogParserPoints1);
            this._catalogParserPoints2 = iniData2.SetValue(this._catalogParserPoints2);
            this._catalogParserPoints3 = iniData3.SetValue(this._catalogParserPoints3);
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
        }

        private void InitCatalogParserPoints1()
        {
            this._catalogParserPoints1 = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 2,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 6,
                CatalogItemNameStart = 16,
                CatalogItemNameEnd = 40	,
                //UnitTypeCodeStart = 39,
                //UnitTypeCodeEnd = 40,
                //QuantityERPStart = 41,
                //QuantityERPEnd = 48,
                //SectionCodeStart = 49,
                //SectionCodeEnd = 50,
                //CatalogPriceSaleStart = 51,
                //CatalogPriceSaleEnd = 57,
                CatalogPriceBuyStart = 57,
                CatalogPriceBuyEnd = 69
            };
        }

        private void InitCatalogParserPoints2()
        {
            this._catalogParserPoints2 = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 2,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 6,
                HamarotBarcodeStart = 41,
                HamarotBarcodeEnd = 53
            };
        }

        private void InitCatalogParserPoints3()
        {
            this._catalogParserPoints3 = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 2, //1-4
                CatalogItemCodeStart = 5,
                CatalogItemCodeEnd = 10,
                //CatalogItemNameStart = 16,
                //CatalogItemNameEnd = 40
                //UnitTypeCodeStart = 39,
                //UnitTypeCodeEnd = 40,
                QuantityERPStart = 20,
                QuantityERPEnd = 30
                //SectionCodeStart = 49,
                //SectionCodeEnd = 50,
                //CatalogPriceSaleStart = 51,
                //CatalogPriceSaleEnd = 57,
                //CatalogPriceBuyStart = 58,
                //CatalogPriceBuyEnd = 64
            };
        }

        protected override bool PreImportCheck()
        {
            return base.ContinueAfterBranchERPWarning(base.Path2, 0, 4);
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
            //if (this._updateOnlyErpQuantity == false)
            //{
            IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogXtechMeuhedetADOProvider1);
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

            IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogXtechMeuhedetADOProvider2);
            provider2.ToPathDB = base.GetDbPath;					   //ProductCatalogForXtechMeuhedetUpdateERPQuentetyDBParser
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path1;
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
                provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }
            if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
            {
                MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }

            //if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
            //{
            //    MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
            //    provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            //}
            //if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
            //{
            //    MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
            //    provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            //}

            this.StepCurrent = 1;
            provider1.Import();
            this.StepCurrent = 2;
            provider2.Import();

            if (WithQuantityErp == true)
            {
                IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogXtechMeuhedetUpdateERPQuentetyADOProvider);
                provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
                //provider1.Clear();
                provider3.FromPathFile = this.Path2;
                provider3.Parms.Clear();
                provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
                provider3.Parms.AddCatalogParserPoints(this._catalogParserPoints3);		//todo
                provider3.ProviderEncoding = base.Encoding;
                provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
                provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
                provider3.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
                provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

                if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
                {
                    MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
                    provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                }
                if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
                {
                    MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
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

        protected override void OnPath2Changed()
        {
            base.OnPath2Changed();

        }
    }
}
