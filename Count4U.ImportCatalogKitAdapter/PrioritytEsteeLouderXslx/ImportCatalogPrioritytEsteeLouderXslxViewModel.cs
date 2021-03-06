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

namespace Count4U.ImportCatalogKitAdapter.PrioritytEsteeLouderXslx
{
    public class ImportCatalogPrioritytEsteeLouderXslxViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        private string _branchErpCode = String.Empty;
		private bool _withQuantityERP;


		public ImportCatalogPrioritytEsteeLouderXslxViewModel(IServiceLocator serviceLocator,
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

        #region Implementation of IImportAdapter



        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
			this._withQuantityERP = true;
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
		   this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
		   this.XlsxFormat = true;
		   this._fileName = "Catalog.xlsx"; 
            base.IsInvertLetters = false;
			base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding(1255);
	
			base.StepTotal = 1;
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogPrioritytEsteeLouderXslxAdapter.ini"));
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

			//ProductCatalogPrioritytEsteeLouderXslxParser
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPrioritytEsteeLouderXslxProvider);
			provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
			provider.FromPathFile = this.Path;
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.ProviderEncoding = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;


			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogWarehouseXslxProvider1);
			//provider1.ToPathDB = base.GetDbPath;
			//provider1.FastImport = base.IsTryFast;
			//provider1.FromPathFile = this.Path;
			//provider1.Parms.Clear();
			//provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider1.ProviderEncoding = base.Encoding;
			//provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//provider1.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;


			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				//provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				//provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			
			}

			StepCurrent = 1;
			provider.Import();

			//StepCurrent = 2;
			//provider1.Import();
				         
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPrioritytEsteeLouderXslxProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();

								
            UpdateLogFromILog();
        }

        #endregion
    }
}