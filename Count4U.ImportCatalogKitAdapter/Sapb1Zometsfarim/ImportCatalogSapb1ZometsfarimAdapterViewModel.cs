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

namespace Count4U.ImportCatalogSapb1ZometsfarimAdapter
{
    public class ImportCatalogSapb1ZometsfarimAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		private string _delimiter = String.Empty;
		private string _branchErpCode = String.Empty;

        private bool _importSection;
		private bool _importSupplier;
		
        //private bool _withQuantityERP;


		public ImportCatalogSapb1ZometsfarimAdapterViewModel(IServiceLocator serviceLocator,
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

		private int GetStep()
		{
			int step = 4;
			if (this.ImportSupplier == true) step++;
			if (this.ImportSection == true) step++;
			return step;
		}

		public bool ImportSupplier
        {
			get { return _importSupplier; }
            set
            {
				this._importSupplier = value;
  
                base.StepTotal = this.GetStep();

				RaisePropertyChanged(() => ImportSupplier);

				//if (base.RaiseCanImport != null)
				//	base.RaiseCanImport();
            }
        }

        public bool ImportSection
        {
            get { return this._importSection; }
            set
            {
                this._importSection = value;

				base.StepTotal = this.GetStep();

                RaisePropertyChanged(()=>ImportSection);
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
			//if (_withQuantityERP)
			return (String.IsNullOrWhiteSpace(base.Path1) == false)
				   && (String.IsNullOrWhiteSpace(base.Path2) == false)
				   && this.IsOkPath(this.Path1)
				   && this.IsOkPath(this.Path2);

			//return (String.IsNullOrWhiteSpace(base.Path1) == false)
			//		 && this.IsOkPath(this.Path1) ;
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

			this._importSupplier = true;
			this._importSection = true;

			this.PathFilter1 = "*.txt|*.txt|All files (*.*)|*.*";
			this.PathFilter2 = "*.txt|*.txt|All files (*.*)|*.*";

			this._fileName1 = "FullCatalog.txt";//String.Format("sfpart{0}.dat", this._branchErpCode);
			this._fileName2 = String.Format("Barcodes.txt");

            base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.IsInvertLetters = false;
			base.IsInvertWords = false;

			base.StepTotal = this.GetStep();

        }

        public override void InitFromIni()
        {
	        Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();

            //init GUI
            this._fileName1 = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
            this._fileName2 = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName2);
			this._delimiter = iniData.SetValue(ImportProviderParmEnum.Delimiter, this._delimiter);
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
        }


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

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
		    provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
			provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider1);
            provider1.ToPathDB = base.GetDbPath;
            provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path1;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider1.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
			provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider2);
			provider2.ToPathDB = base.GetDbPath;
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path1;
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.ProviderEncoding = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
			provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

			if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
				provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}

			this.StepCurrent = 1;
			provider.Import();

			this.StepCurrent = 2;
			provider1.Import();

			this.StepCurrent = 3;
			provider2.Import();

			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider3);
			provider3.ToPathDB = base.GetDbPath;
			provider3.FastImport = base.IsTryFast;
			provider3.FromPathFile = this.Path2;
			provider3.Parms.Clear();
			provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider3.ProviderEncoding = base.Encoding;
			provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
			provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

			this.StepCurrent = 4;
			provider3.Import();


			if (ImportSection == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportSectionSapb1ZometsfarimProvider);
				provider4.ToPathDB = base.GetDbPath;
				provider4.FastImport = base.IsTryFast;
				provider4.FromPathFile = this.Path1;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding; //this._encoding2;
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
				provider4.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider4.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
				provider4.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

				IImportProvider provider5 = this.GetProviderInstance(ImportProviderEnum.ImportSectionSapb1ZometsfarimProvider1);
				provider5.ToPathDB = base.GetDbPath;
				provider5.FastImport = base.IsTryFast;
				provider5.FromPathFile = this.Path1;
				provider5.Parms.Clear();
				provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider5.ProviderEncoding = base.Encoding; //this._encoding2;
				provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
				provider5.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
				provider5.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider5.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
				provider5.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

	  
				this.StepCurrent = this.StepCurrent + 1;
				provider4.Import();

				this.StepCurrent = this.StepCurrent + 1;
				provider5.Import();

			}


			if (this.ImportSupplier == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierSapb1ZometsfarimProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.FastImport = base.IsTryFast;
				provider6.FromPathFile = this.Path1;
				provider6.Parms.Clear();
				provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider6.ProviderEncoding = base.Encoding;
				provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.Delimiter] = this._delimiter;
				provider6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

				this.StepCurrent = this.StepCurrent + 1;
				provider6.Import();
			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
			if (this.ImportSection == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportSectionSapb1ZometsfarimProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();
			}

			if (this.ImportSupplier == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierSapb1ZometsfarimProvider);
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
