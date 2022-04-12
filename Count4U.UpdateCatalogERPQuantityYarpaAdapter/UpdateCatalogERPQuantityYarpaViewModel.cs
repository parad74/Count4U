using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Count4U.Common.Helpers;
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

namespace Count4U.UpdateCatalogERPQuantityYarpaAdapter
{																																				  
    public class UpdateCatalogERPQuantityYarpaViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		//private CatalogParserPoints _catalogParserPoints;
		private CatalogParserPoints _catalogParserPoints2;
		string _branchErpCode = String.Empty;

        public UpdateCatalogERPQuantityYarpaViewModel(IServiceLocator serviceLocator,
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

			this.PathFilter = "*.dat|*.dat|All files (*.*)|*.*";

			this._fileName = "mlout.dat";

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;

			this.InitCatalogParserPoints2();			

			base.StepTotal = 1;

        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogYarpaUpdateERPQuentetyADOProvider1");

			//init GUI
			this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName);
				//todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
			base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			//init Provider Parms
			//this._catalogParserPoints = iniData.SetValue(this._catalogParserPoints);
			this._catalogParserPoints2 = iniData2.SetValue(this._catalogParserPoints2);
			}

		private void InitCatalogParserPoints2()
		{
			this._catalogParserPoints2 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 2,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 6,
				QuantityERPStart = 7,
				QuantityERPEnd = 13,
				QuantityTypeStart = 14,
				QuantityTypeEnd = 14
			};
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

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyADOProvider1);
			provider2.ToPathDB = base.GetDbPath;					   //ProductCatalogForXtechMeuhedetUpdateERPQuentetyDBParser
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path;
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			provider2.ProviderEncoding = base.Encoding; //this._encoding2;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyADOProvider2);
			//provider3.ToPathDB = base.GetDbPath;
			////provider1.Clear();
			//provider3.FromPathFile = this.Path;
			//provider3.Parms.Clear();
			//provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider3.Parms.AddCatalogParserPoints(this._catalogParserPoints2);		//todo
			//provider3.ProviderEncoding = base.Encoding;
			//provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			this.StepCurrent = 1;
			provider2.Import();

			//this.StepCurrent = 2;
			//provider3.Import();

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
