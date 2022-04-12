using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.AS400AmericanEagle
{
	public class ImportCatalogAS400AmericanEagleViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		private string _branchErpCode = String.Empty;
		private bool _importBarcodes;
		//private string Path2;
   		private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogAS400AmericanEagleViewModel(IServiceLocator serviceLocator,
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

		//BarcodeFileName
		public bool ImportBarcodes
		{
			get { return this._importBarcodes; }
			set
			{
				this._importBarcodes = value;
				if (this.ImportBarcodes == true) base.StepTotal = base.StepTotal + 1;
				//this._barcodeFileName = "AlternateCodes.csv";
				//if (IsBarcodeFileOkPath() == false)
				//{
				//	this._barcodeFileName = "AlternateCodes.csv NOT FIND";
				//	this._importBarcodes = false;
				//}
				RaisePropertyChanged(() => ImportBarcodes);
				//RaisePropertyChanged(() => BarcodeFileName);
			}
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));
			this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));

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

		public override bool CanImport()
		{
			if (ImportBarcodes == true)
			{
				return (String.IsNullOrWhiteSpace(base.Path1) == false)
					   && (String.IsNullOrWhiteSpace(base.Path2) == false)
					   && this.IsOkPath(this.Path1)
					   && this.IsOkPath(this.Path2);
			}
			else
			{
				return (String.IsNullOrWhiteSpace(base.Path1) == false)
						 && this.IsOkPath(this.Path1);
			}
		}

        #region Implementation of IImportAdapter

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

		//		RaisePropertyChanged(() => WithQuantityErp);

		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();

		//	}
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


            //init GUI
			this.PathFilter1 = "*.txt|*.txt|All files (*.*)|*.*";
			this._fileName1 = "PARIT.TXT";
		
			this.PathFilter2 = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName2 = "AE_Multi.csv";

            base.IsInvertLetters = true;
			base.IsInvertWords = true;
            base.Encoding = System.Text.Encoding.GetEncoding(862);

			//init Provider Parms
			this._catalogParserPoints = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 15,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 15,
				HamarotItemCodeStart = 17,
				HamarotItemCodeEnd = 31,
				HamarotBarcodeStart = 33,
				HamarotBarcodeEnd = 47,
				CatalogItemNameStart = 49,
				CatalogItemNameEnd = 78,
				SectionCodeStart = 100,
				SectionCodeEnd = 103
			};

			base.StepTotal = 2;
			if (this.ImportBarcodes == true) base.StepTotal = base.StepTotal + 1;


			//if (string.IsNullOrWhiteSpace(this._maskViewModel.MakatMask) == true)
			//{
			//	this._maskViewModel.MakatMask = "0000000000000{S}";
			//}
			//if (string.IsNullOrWhiteSpace(this._maskViewModel.BarcodeMask) == true)
			//{
			//	this._maskViewModel.BarcodeMask = "7290000000000{S}";
			//}

			//if (_withQuantityERP == true)
			//{
			//	base.StepTotal = 3;
			//}
			//else
			//{
			//	base.StepTotal = 2;
			//}
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogAS400AmericanEagleAdapter.ini"));
            //init GUI
			this._fileName1 = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
			if (this._fileName1.Contains("XXX") == true)
            {
				this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
            }

			this._fileName2 = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName2);
			if (this._fileName2.Contains("XXX") == true)
			{
				this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
			}

			this.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
			this.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
            base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
            base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

			//init Provider Parms
			this._catalogParserPoints = iniData.SetValue(this._catalogParserPoints);
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

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
			provider.FromPathFile = this.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms.AddCatalogParserPoints(this._catalogParserPoints);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//provider.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;


			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider1);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			provider1.FromPathFile = this.Path1;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;


			if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
            {
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }
			if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
            {
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }

            StepCurrent = 1;
            provider.Import();

			StepCurrent = 2;
			provider1.Import();

			if (ImportBarcodes == true)
			{
				base.StepTotal = 3;
				//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider2);
				//provider2.ToPathDB = base.GetDbPath;
				//provider2.FastImport = base.IsTryFast;
				//provider2.FromPathFile = this.Path2;
				//provider2.Parms.Clear();
				//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider2.ProviderEncoding = base.Encoding;
				//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider3);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path2;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

				if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
				{
					MaskRecord barcodeMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
						provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
				}
				if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
				{
					MaskRecord makatMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
					provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
				}

				StepCurrent = 3;
				//provider2.Import();

				//StepCurrent = 4;
				provider3.Import();


			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}