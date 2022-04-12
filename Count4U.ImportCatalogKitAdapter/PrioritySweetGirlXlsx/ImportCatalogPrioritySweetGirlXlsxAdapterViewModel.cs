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

namespace Count4U.ImportCatalogPrioritySweetGirlXlsxAdapter
{
	public class ImportCatalogPrioritySweetGirlXlsxAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        
		private string _delimiter = String.Empty;
		private string _branchErpCode = String.Empty;
		//private bool _xlsxFormat = false;
		//private bool _xlsxFormat2 = false;

		//private bool _importSection;
		//private bool _importSupplier;
		//private bool _withQuantityERP;

		public ImportCatalogPrioritySweetGirlXlsxAdapterViewModel(IServiceLocator serviceLocator,
          IContextCBIRepository contextCBIRepository,
          IEventAggregator eventAggregator,
          IRegionManager regionManager,
          ILog logImport,
          IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
          IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			base.XlsxFormat = true;
			//base.XlsxFormat2 = false;
			base.ParmsDictionary.Clear();
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

		//public bool ImportSection
		//{
		//	get { return this._importSection; }
		//	set
		//	{
		//		this._importSection = value;
		//		if (value == true)
		//		{
		//			base.StepTotal = 3;
		//		}
		//		else
		//		{
		//			base.StepTotal = 2;
		//		}
		//		if (this.WithQuantityErp == true) base.StepTotal = base.StepTotal + 1;
		//		RaisePropertyChanged(()=>ImportSection);
		//	}
		//}

		
		//public bool ImportSupplier
		//{
		//	get { return this._importSupplier; }
		//	set
		//	{
		//		this._importSupplier = value;
		//		if (this.ImportSupplier == true) base.StepTotal = base.StepTotal + 1;
		//		RaisePropertyChanged(() => ImportSupplier);
		//	}
		//}
	
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

		//public override bool CanImport()
		//{
		//	return (String.IsNullOrWhiteSpace(base.Path) == false);
		//}


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
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") XlsxFormat = true; else XlsxFormat = false;
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

			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this._fileName = "CatalogConcat.xlsx";
			base.XlsxFormat = true;

	        base.Encoding = System.Text.Encoding.GetEncoding(1255);
            base.IsInvertLetters = false;
            base.IsInvertWords = false;
			base.StepTotal = 1;
        }

		public override void InitFromIni()
		{
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogPrioritySweetGirlXlsxAdapter.ini"));
			//init GUI
			this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}

			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

			//init Provider Parms
			//this._catalogParserPoints = iniData.SetValue(this._catalogParserPoints);
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

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPrioritySweetGirlXLSXProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;

            if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
            {
                MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }
            if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
            {
                MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }

            this.StepCurrent = 1;
            provider.Import();

     	  
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPrioritySweetGirlXLSXProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
	
            UpdateLogFromILog();
        }

 
    }
}
