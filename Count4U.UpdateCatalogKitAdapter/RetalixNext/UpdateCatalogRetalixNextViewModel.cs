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

namespace Count4U.UpdateCatalogKitAdapter.RetalixNext
{
    public class UpdateCatalogRetalixNextViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        string _branchErpCode = String.Empty;
		private CatalogParserPoints _catalogParserPoints;

        public UpdateCatalogRetalixNextViewModel(IServiceLocator serviceLocator,
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
						if (this._fileName.Contains("XXX") == true)
						{
							this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
						}
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;

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

			this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
			this._fileName = "Toinfo.txt";
			base.IsInvertLetters = true;
			base.IsInvertWords = true;
			base.Encoding = System.Text.Encoding.GetEncoding(862);

			this._catalogParserPoints = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 60,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 13,
				CatalogItemNameStart = 14,
				CatalogItemNameEnd = 29,
				UnitTypeCodeStart = 30,
				UnitTypeCodeEnd = 30,
				QuantityERPStart = 31,
				QuantityERPEnd = 40,
				CatalogPriceBuyStart = 41,
				CatalogPriceBuyEnd = 49,
				CatalogPriceSaleStart = 50,
				CatalogPriceSaleEnd = 58,
				SectionCodeStart = 59,
				SectionCodeEnd = 60
			};

			this.MakatMask = "0000000000000{S}";
			this.BarcodeMask = "7290000000000{S}";

            base.StepTotal = 1;

        }

        public override void InitFromIni()
        {
	        Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.UpdateCatalogERPRetalixNextAdapter.ini"));

            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName);
            //todo
            if (this._fileName.Contains("XXX") == true)
            {
                this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
            }
            base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			this._catalogParserPoints = iniData.SetValue(this._catalogParserPoints);
        }


        protected override bool PreImportCheck()
        {
            return base.ContinueAfterBranchERPWarning(base.Path, 14, 4);
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

		    IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogRetalixNextUpdateERPQuentetyADOProvider);
            provider2.ToPathDB = base.GetDbPath;					  
            provider2.FastImport = base.IsTryFast;
            provider2.FromPathFile = this.Path;
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints);
            provider2.ProviderEncoding = base.Encoding; 
            provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
            provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
            provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}

            this.StepCurrent = 1;
            provider2.Import();

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
        }

        protected override void OnPathChanged()
        {
            base.OnPathChanged();
        }
    }
}