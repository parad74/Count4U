using System;
using System.Text;
using System.Collections.Generic;
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
using System.IO;
using Count4U.Common.ViewModel;

namespace Count4U.ImportCatalogRetalixPOS_HOAdapter
{
    public class ImportCatalogRetalixPOS_HOAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        private CatalogParserPoints _catalogParserPoints;

        private bool _withQuantityERP;

        public ImportCatalogRetalixPOS_HOAdapterViewModel(IServiceLocator serviceLocator,
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
                _withQuantityERP = value;
				if (value == true)
				{
					base.StepTotal = 2;
				}
				else
				{
					base.StepTotal = 1;
				}

                RaisePropertyChanged(() => WithQuantityErp);

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
			this.MakatMask = "0000000000000{F}";
	   		//if (string.IsNullOrWhiteSpace(this._maskViewModel.BarcodeMask) == true)
			//{
			//    this._maskViewModel.BarcodeMask = "";
			//}
            //init GUI
            this.PathFilter = "*.dat|*.dat|All files (*.*)|*.*";
            this._fileName = "mlai.dat";
            base.Encoding = System.Text.Encoding.GetEncoding(862);
            base.IsInvertLetters = true;
            base.IsInvertWords = true;

            //init Provider Parms
            this._catalogParserPoints = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 67,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 13,
                CatalogItemNameStart = 15,
                CatalogItemNameEnd = 29, 
				SectionCodeStart = 38, 
				SectionCodeEnd =39, 
				CatalogPriceSaleStart =45, 
				CatalogPriceSaleEnd = 51,
				CatalogPriceBuyStart = 53,
                CatalogPriceBuyEnd = 59,
				QuantityTypeStart =61, 
				QuantityTypeEnd = 61,
				QuantityERPStart = 62,
				QuantityERPEnd = 67, 
				UnitTypeCodeStart = 72, 
				UnitTypeCodeEnd = 73

              };

			if (_withQuantityERP == true)
			{
				base.StepTotal = 2;
			}
			else
			{
				base.StepTotal = 1;
			}
        }

        public override void InitFromIni()
        {
	        Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				//this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
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

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogRetalixPosHOADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
            provider.Clear();
            provider.FromPathFile = this.Path;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.Parms.AddCatalogParserPoints(this._catalogParserPoints);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider1);
			//provider1.ToPathDB = base.GetDbPath;
			//provider1.FastImport = base.IsTryFast;
			//provider1.FromPathFile = this.Path;
			//provider1.Parms.Clear();
			//provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints);
			//provider1.ProviderEncoding = base.Encoding;
			//provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

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

			if (WithQuantityErp == true)
			{
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogRetalixPosHOUpdateERPQuentetyADOProvider);
				provider2.ToPathDB = base.GetDbPath;
				provider2.FastImport = base.IsTryFast;
				provider2.FromPathFile = this.Path;
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints);
				provider2.ProviderEncoding = base.Encoding; //this._encoding2;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
				provider2.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

				this.StepCurrent = 2;
				provider2.Import();

			}
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogRetalixPosHOADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}
