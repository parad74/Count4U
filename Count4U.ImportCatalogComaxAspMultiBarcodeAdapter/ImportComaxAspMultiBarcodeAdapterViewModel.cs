using System;
using System.Threading.Tasks;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.Text;
using System.Collections.Generic;
using Count4U.Model.Main;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using System.IO;

namespace Count4U.ImportCatalogComaxAspMultiBarcodeAdapter
{

    public class ImportComaxAspMultiBarcodeAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		private CatalogParserPoints _gazitPoints;
		private string _branchErpCode = String.Empty;

		private bool _importSupplier;

        public ImportComaxAspMultiBarcodeAdapterViewModel(IServiceLocator serviceLocator,
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

		public bool ImportSupplier
		{
			get { return _importSupplier; }
			set
			{
				this._importSupplier = value;
				if (value == true)
				{
					base.StepTotal = 4;
				}
				else
				{
					base.StepTotal = 3;
				}
				RaisePropertyChanged(() => ImportSupplier);
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

			this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
			if (this._maskViewModel2 != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask2) == false)
				{
					this._maskViewModel2.MakatMask = this._makatMask2;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask2) == false)
				{
					this._maskViewModel2.BarcodeMask = this._barcodeMask2; //init Default
				}
			}
            
        }     

        #region IImportAdapter Members

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

     

			this.PathFilter1 = "*.asp|*.asp|All files (*.*)|*.*";
			this._fileName1 = "items.asp";

			this.PathFilter2 = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName2 = "multi_barcode.csv";

			base.Encoding = Encoding.GetEncoding(1255);
			base.IsInvertLetters = false;
			base.IsInvertWords = false;

			this.MakatMask1 = "0000000000000{F}";
			this.BarcodeMask1 = "7290000000000{F}";

            this.MakatMask2 = "0000000000000{F}";
            this.BarcodeMask2 = "7290000000000{F}";

			if (this.ImportSupplier == true)
			{
				base.StepTotal = 4;
			}
			else
			{
				base.StepTotal = 3;
			}
        }

        public override void InitFromIni()
        {
   			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
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

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider);
            provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = _branchErpCode;

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider1);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			provider1.FromPathFile = this.Path1;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = _branchErpCode;


			if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
			{
				MaskRecord barcodeMaskRecord1 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord1);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord1);

			}

			if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
			{
				MaskRecord makatMaskRecord1 = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
				provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord1);
				provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord1);

			}

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider2);
            provider2.ToPathDB = base.GetDbPath;
			provider2.FastImport = base.IsTryFast;
            provider2.FromPathFile = this.Path2;
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider2.ProviderEncoding = base.Encoding;
            provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = _branchErpCode;

			if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
			{
				MaskRecord barcodeMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
				provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
			{
				MaskRecord makatMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
				provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
			}


			StepCurrent = 1;
			provider.Import();
			StepCurrent = 2;
			provider1.Import();
			StepCurrent = 3;
			provider2.Import();

	
			if (this.ImportSupplier == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierComaxASPADOProvider);
				provider4.ToPathDB = base.GetDbPath;
				provider4.FromPathFile = this.Path1;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding;
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
				{
					MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
					provider4.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				}
				if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
				{
					MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
					provider4.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				}
				provider4.Import();
			}


			FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
                UpdateLogFromILog();
        }

        #endregion
    }
}