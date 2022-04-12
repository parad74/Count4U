using System;
using System.Collections.Generic;
using System.Threading;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using System.IO;
using Count4U.Common.ViewModel;

namespace Count4U.ImportFamilyKitAdapter.AS400Hamashbir
{
    public class ImportFamilyAS400HamashbirAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		private string _branchErpCode = String.Empty;
		private CatalogParserPoints _catalogParserPoints1;

		public ImportFamilyAS400HamashbirAdapterViewModel(IServiceLocator serviceLocator,
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

            //  this._maskViewModel = this.BuildMaskControl("1", base.BuildMaskRegionName());
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

			this.PathFilter = "All files (*.*)|*.*";

			this._fileName = String.Format("SPARITEWSN.{0}", _branchErpCode);

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;

			this.InitCatalogParserPoints1();	
            base.StepTotal = 1;
        }

		//	Field1: CompanyCode (col 1-2)
		//Field2: Item Code (col 3-10)
		//Field3: Name (col 11-40)
		//Field4: FamilyID (col 41-44)
		//Field5: FamilyName (col 45-56)
		//Field6: SectionID (col 57-60)
		//Field7: SectionName (col 61-72)
		//Field8: SupplierID (col 73-77)
		//Field9: PriceSell (col 82-90)
		private void InitCatalogParserPoints1()
		{
			this._catalogParserPoints1 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 10,
				//Field2: Item Code (col 3-10)
				//Field3: Name (col 11-40)
				CatalogItemCodeStart = 3,
				CatalogItemCodeEnd = 10,
				CatalogItemNameStart = 11,
				CatalogItemNameEnd = 40,
				//Field4: FamilyID (col 41-44)
				//Field5: FamilyName (col 45-56)
				FamilyCodeStart = 41,
				FamilyCodeEnd = 44,
				FamilyNameStart = 45,
				FamilyNameEnd = 56,
				//Field6: SectionID (col 57-60)
				//Field7: SectionName (col 61-72)
				SectionCodeStart = 57,
				SectionCodeEnd = 60,
				SectionNameStart = 61,
				SectionNameEnd = 72,
				//Field8: SupplierID (col 73-77)
				SupplierCodeStart = 73,
				SupplierCodeEnd = 77,
				//Field9: PriceSell (col 82-90)
				CatalogPriceSaleStart = 82,
				CatalogPriceSaleEnd = 90
			};
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
				this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            //todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
            base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
        }


        public override void Import()
        {
			base.LogImport.Add(MessageTypeEnum.Trace, "Start ImportFamilyAS400Hamashbir Adapter");
			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyAS400HamashbirProvider);
            provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.ProviderEncoding = base.Encoding;
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            this.StepCurrent = 1;
            provider1.Import();

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
			base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportFamilyAS400HamashbirProvider);
			provider.ToPathDB = base.GetDbPath;
			provider.Clear();
			UpdateLogFromILog();

        }

    }
}
