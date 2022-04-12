using System;
using System.Collections.Generic;
using System.Threading;
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

namespace Count4U.UpdateCatalogERPQuantityPosSuperPharm
{																																				  
    public class UpdateCatalogPosSuperPharmViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		private CatalogParserPoints _catalogParserPoints1;
		string _branchErpCode = String.Empty;

        public UpdateCatalogPosSuperPharmViewModel(IServiceLocator serviceLocator,
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

			//_updateOnlyErpQuantity = false;

			this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";

			this._fileName = String.Format("toinf{0}.txt", this._branchErpCode);

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;
 
			this.InitCatalogParserPoints1();			//this._catalogParserPoints1

			base.StepTotal = 1;
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("ImportCatalogNetPOSSuperPharmUpdateERPQuentetyADOProvider");

			//init GUI
			this._fileName = iniData1.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			//todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
			base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			//init Provider Parms
			this._catalogParserPoints1 = iniData1.SetValue(this._catalogParserPoints1);
		}

		private void InitCatalogParserPoints1()
		{
			this._catalogParserPoints1 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 1,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 13,
				CatalogItemNameStart = 14,
				CatalogItemNameEnd = 38,
				UnitTypeCodeStart = 39,
				UnitTypeCodeEnd = 40,
				QuantityERPStart = 41,
				QuantityERPEnd = 48,
				SectionCodeStart = 49,
				SectionCodeEnd = 50,
				CatalogPriceSaleStart = 51,
				CatalogPriceSaleEnd = 57,
				CatalogPriceBuyStart = 58,
				CatalogPriceBuyEnd = 64
			};
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
	
			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNetPOSSuperPharmUpdateERPQuentetyADOProvider);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
			//provider1.Clear();
			provider1.FromPathFile = this.Path;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
			{
				MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
			{
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}

			this.StepCurrent = 1;
			provider1.Import();

			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
        }
    }
}