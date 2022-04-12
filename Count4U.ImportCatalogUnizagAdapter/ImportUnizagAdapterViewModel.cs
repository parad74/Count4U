using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Interfaces;
using Count4U.Model.Main;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportCatalogUnizagAdapter
{
	public class ImportUnizagAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		private string _branchErpCode = String.Empty;
		private bool _importBarcodes;
		//private string _barcodeFileName;
		//private string _barcodeFilePath;
	

        public ImportUnizagAdapterViewModel(IServiceLocator serviceLocator,
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
				if (this.ImportBarcodes == true) base.StepTotal = base.StepTotal + 2;
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

		//public string BarcodeFileName
		//{
		//	get { return this._barcodeFileName; }
		//	set
		//	{
		//		this._barcodeFileName = value;
		//		RaisePropertyChanged(() => BarcodeFileName);
		//	}
		//}

		//private bool IsBarcodeFileOkPath()
		// {
		//	 this._barcodeFilePath = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._barcodeFileName);
		//	 return File.Exists(this._barcodeFilePath);
		//}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));

			if (this._maskViewModel1 != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask1) == false)
				{
					this._maskViewModel1.MakatMask = this._makatMask1;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask1) == false)
				{
					this._maskViewModel1.BarcodeMask = this._barcodeMask1; //init Default
				}
			}

			//this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
			//if (this._maskViewModel2 != null)
			//{
			//	if (string.IsNullOrWhiteSpace(this._makatMask2) == false)
			//	{
			//		this._maskViewModel2.MakatMask = this._makatMask2;   //init Default
			//	}
			//	if (string.IsNullOrWhiteSpace(this._barcodeMask2) == false)
			//	{
			//		this._maskViewModel2.BarcodeMask = this._barcodeMask2; //init Default
			//	}
			//}

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
			this.PathFilter1 = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName1 = "items.csv";

			this.PathFilter2 = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName2 = "AlternateCodes.csv";

            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			
		
			//this._barcodeFileName = "AlternateCodes.csv";
			//if (IsBarcodeFileOkPath() == false) {
			//	this._barcodeFileName = "AlternateCodes.csv NOT FIND";
			//	this._importBarcodes = false;
			//}

			//this._maskViewModel.MakatMask = "0000000000000{F}";
			//this._maskViewModel.BarcodeMask = "7290000000000{F}";
			this.MakatMask1 = "0000000000000{S}";
			this.BarcodeMask1 = "7290000000000{S}";


            StepTotal = 2;
			if (this.ImportBarcodes == true) base.StepTotal = base.StepTotal + 2;
        }

        public override void InitFromIni()
        {
		    Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
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
			base.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
			base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
 			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
        }

        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider);
            provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider1);
            provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path1;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

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
            if (this.MakatMask1.Trim().ToLower() != this.BarcodeMask1.Trim().ToLower())
            {
                StepCurrent = 2;
                provider1.Import();
            }

			if (ImportBarcodes == true)
			{
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider2);
				provider2.ToPathDB = base.GetDbPath;
				provider2.FastImport = base.IsTryFast;
				provider2.FromPathFile = this.Path2;
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.ProviderEncoding = base.Encoding;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			

				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider3);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path2;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
				{
					MaskRecord barcodeMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
					provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
					provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
				}
				if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
				{
					MaskRecord makatMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
					provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
					provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
				}

				StepCurrent = 3;
				provider2.Import();

				StepCurrent = 4;
				provider3.Import();
			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}