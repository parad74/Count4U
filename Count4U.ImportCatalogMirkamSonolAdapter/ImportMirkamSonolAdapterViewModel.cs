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

namespace Count4U.ImportCatalogMirkamSonolAdapter
{
    public class ImportMirkamSonolAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		string _branchErpCode = String.Empty;

        private bool _withQuantityErp;

        public ImportMirkamSonolAdapterViewModel(IServiceLocator serviceLocator,
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
            get { return this._withQuantityErp; }
			set
			{
				this._withQuantityErp = value;
				if (value == true)
				{
					base.StepTotal = 3;
				}
				else
				{
					base.StepTotal = 2;
				}

				this.RaisePropertyChanged(() => WithQuantityErp);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();
			}
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._maskViewModel = this.BuildMaskControl("1", base.BuildMaskRegionName());

			if (this._maskViewModel != null)
			{
				if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
				{
					this._maskViewModel.MakatMask = this.MakatMask;// "0000000000000{F}";
				}
				if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
				{
					this._maskViewModel.BarcodeMask = this.BarcodeMask;// "0000000000000{F}";
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
			this._withQuantityErp = false;

          

            //init GUI
			this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
			this._fileName = "TOINFO.txt";
            base.IsInvertLetters = true;
            base.IsInvertWords = true;

	        base.Encoding = System.Text.Encoding.GetEncoding(862);
			//base.StepTotal = 2;

			this.MakatMask = "0000000000000{F}";
			this.BarcodeMask = "0000000000000{F}";

			if (this._withQuantityErp == true)
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
	        Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty,  base.GetPathToIniFile("Count4U.ImportCatalogMirkamSonolAdapter.ini"));
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

			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMikramSonolADOProvider);
            provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMikramSonolADOProvider1);
            provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
            {
                MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }
            if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
            {
                MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }
   
            StepCurrent = 1;
            provider.Import();

            StepCurrent = 2;
            provider1.Import();

			//if (WithQuantityErp == true)
			//{
			//    IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSUpdateERPQuentetyADOProvider1);
			//    provider2.ToPathDB = base.GetDbPath;
			//    provider2.FromPathFile = this.Path;
			//    provider2.Parms.Clear();
			//    provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//    provider2.ProviderEncoding = base.Encoding; //this._encoding2;
			//    provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//    provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//    provider2.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
			//    provider2.Parms[ImportProviderParmEnum.ERPNum] = this._branchErpCode;

			//    if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			//    {
			//        MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
			//        provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			//    }
			//    if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			//    {
			//        MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
			//        provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			//    }

			//    this.StepCurrent = 4;
			//    provider2.Import();

			//}
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMikramSonolADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}
