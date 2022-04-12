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

namespace Count4U.UpdateCatalogERPQuantityPriorityKedsAdapter
{																																				  
    public class UpdateCatalogERPQuantityPriorityKedsViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		//private CatalogParserPoints _catalogParserPoints;
		private CatalogParserPoints _catalogParserPoints2;
		string _branchErpCode = String.Empty;

		private bool _updatePriceBuy;
		private bool _updatePriceSell; 

        public UpdateCatalogERPQuantityPriorityKedsViewModel(IServiceLocator serviceLocator,
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
			_updatePriceBuy = false;
			_updatePriceSell= false; 
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

        }

		public bool UpdatePriceBuy
		{
			get { return this._updatePriceBuy; }
			set
			{
				this._updatePriceBuy = value;
				RaisePropertyChanged(() => UpdatePriceBuy);
			}
		}

		public bool UpdatePriceSell
		{
			get { return this._updatePriceSell; }
			set
			{
				this._updatePriceSell = value;
				RaisePropertyChanged(() => UpdatePriceSell);
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

			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;

			this._fileName = String.Format("MLAY_{0}.xlsx", this._branchErpCode);

			base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.IsInvertLetters = false;
			base.IsInvertWords = false;

			//this.InitCatalogParserPoints2();			

			base.StepTotal = 1;

        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//Dictionary<ImportProviderParmEnum, string> iniData2 = base.GetIniData("ImportCatalogYarpaUpdateERPQuentetyADOProvider1");

			//init GUI
			this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName);
				//todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
			base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
		}

	
        protected override bool PreImportCheck()
        {
			// base.ContinueAfterBranchERPWarning(base.Path, 14, 4);
			return base.ContinueAfterBranchERPWarning(base.Path, @"MLAY_(.+)\.xlsx");
        }

        public override void Import()
        {
			base.StepTotal = 1;
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPriorityKedsUpdateERPQuentetyADOProvider1);
			//provider2.ToPathDB = base.GetDbPath;
			//provider2.FastImport = base.IsTryFast;	 
			//provider2.FromPathFile = this.Path;
			//provider2.Parms.Clear();
			//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.ProviderEncoding = base.Encoding; 
			//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;


			IImportProvider providerCatalog = this.GetProviderInstance(ImportProviderEnum.ImportCatalogGeneralXLSXUpdateERPQuentetyADOProvider2);
			providerCatalog.ToPathDB = base.GetDbPath;
			providerCatalog.FastImport = base.IsTryFast;
			providerCatalog.FromPathFile = this.Path;
			providerCatalog.Parms.Clear();
			providerCatalog.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerCatalog.ProviderEncoding = base.Encoding;
			providerCatalog.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerCatalog.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			providerCatalog.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerCatalog.Parms[ImportProviderParmEnum.UpdatePriceBuy] = this.UpdatePriceBuy ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.UpdatePriceSell] = this.UpdatePriceSell ? "1" : String.Empty;
			StepCurrent = 1;
			providerCatalog.Import();

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
