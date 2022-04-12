﻿using System;
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

namespace Count4U.ImportCatalogKitAdapter.H_M
{
    public class ImportCatalogH_MViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        private string _branchErpCode = String.Empty;
		private bool _withBarcode;
		//private bool _withQuantityERP;
		//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogH_MViewModel(IServiceLocator serviceLocator,
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

		public bool WithBarcode
		{
			get { return this._withBarcode; }
			set
			{
				this._withBarcode = value;
				if (this.WithBarcode == true) base.StepTotal = base.StepTotal + 1;
				RaisePropertyChanged(() => WithBarcode);
			}
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
			this.PathFilter = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName = "StockCount.csv";
            base.IsInvertLetters = false;
			base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding(1255);

			this._withBarcode = true;

			base.StepTotal = 5;

			if (this.WithBarcode == true) base.StepTotal = base.StepTotal + 1;

		
        }

        public override void InitFromIni()
        {
				Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogH_MAdapter.ini"));
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

		public override void Import()
		{
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

			string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogH_MProvider);
			provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
			provider.FromPathFile = this.Path;
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.ProviderEncoding = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogH_MProvider1);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			provider1.FromPathFile = this.Path;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider1.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogH_MProvider2);
			provider2.ToPathDB = base.GetDbPath;
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path;
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.ProviderEncoding = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider2.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}

			StepCurrent = 1;
			provider.Import();

			StepCurrent = 2;
			provider1.Import();

			StepCurrent = 3;
			provider2.Import();

			if (this.WithBarcode == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogH_MProvider3);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider3.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

				if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
				{
					MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
					provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				}
				if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
				{
					MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
					provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				}

				StepCurrent = StepCurrent  + 1;
				provider3.Import();
			}


			IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportSectionH_MProvider);
			provider4.ToPathDB = base.GetDbPath;
			provider4.FastImport = base.IsTryFast;
			provider4.FromPathFile = this.Path;
			provider4.Parms.Clear();
			provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider4.ProviderEncoding = base.Encoding;
			provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider4.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

			StepCurrent = StepCurrent + 1;
			provider4.Import();

			//if (this.WithBarcode == true)
			//{
			IImportProvider provider5 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyH_MProvider);
			provider5.ToPathDB = base.GetDbPath;
			provider5.FastImport = base.IsTryFast;
			provider5.FromPathFile = this.Path;
			provider5.Parms.Clear();
			provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider5.ProviderEncoding = base.Encoding;
			provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			provider5.Parms[ImportProviderParmEnum.WithBarcode] = this.WithBarcode ? "1" : String.Empty;

			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider5.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider5.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}

			StepCurrent = StepCurrent + 1;
			provider5.Import();
			//}

			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
		}

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogH_MProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyH_MProvider);
			provider1.ToPathDB = base.GetDbPath;
			provider1.Clear();

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportSectionH_MProvider);
			provider2.ToPathDB = base.GetDbPath;
			provider2.Clear();

            UpdateLogFromILog();
        }

        #endregion
    }
}