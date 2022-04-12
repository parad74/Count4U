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
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.NativExportErp
{
	public class ImportCatalogNativExportErpAdapterViewModel : TemplateAdapterOneFileViewModel
	{
		public string _fileName {get; set;}
		public string _branchErpCode = String.Empty;
			
		//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogNativExportErpAdapterViewModel(IServiceLocator serviceLocator,
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

		

		private int GetStep()
		{
			return 4;
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
			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;
			this._fileName = "ExportErpFormat.xlsx";
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			base.Encoding = System.Text.Encoding.GetEncoding(1255);

			base.StepTotal = this.GetStep();

   		}

		public override void InitFromIni()
		{
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogNativExportErpAdapterAdapter.ini"));
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

			StepCurrent = 0;
		
				//ProductCatalogNativXslx2SdfParser
				IImportProvider providerCatalog = this.GetProviderInstance(ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider3);
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
				providerCatalog.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
				providerCatalog.Parms[ImportProviderParmEnum.SheetNameXlsx] = "InventProduct";
			//	IturAnalyzes
			// IturAnalyzes Sum
			// InventProduct
			//	CurrentInventoryAdvanced
			Clear();
				StepCurrent++;
				providerCatalog.Import();
		
			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
		}

		public override void Clear()
		{
			base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider3);
			provider.ToPathDB = base.GetDbPath;
			provider.Clear();
   		
			UpdateLogFromILog();
		}

		#endregion
	}
}