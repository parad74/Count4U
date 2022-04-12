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
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.MerkavaSqliteXslx
{
	public class ImportCatalogMerkavaSqliteXslxViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        private string _branchErpCode = String.Empty;
		//private bool _withFamily;
		//private bool _importSupplier;
	

		private bool _importCatalog;
		private bool _importLocation;
		private bool _importBuildingConfig;
		private bool _importPropertyStr6List;
		private bool _importPropertyStr7List;
		private bool _importPreviousInventory;
		
	//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogMerkavaSqliteXslxViewModel(IServiceLocator serviceLocator,
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

		
		public bool ImportCatalog
		{
			get { return _importCatalog; }
			set {
				_importCatalog = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportCatalog);
			}
		}

		public bool ImportLocation
		{
			get { return _importLocation; }
			set { _importLocation = value;
			base.StepTotal = this.GetStep();
			RaisePropertyChanged(() => ImportLocation);
			}
		}

	

		public bool ImportBuildingConfig
		{
			get { return _importBuildingConfig; }
			set
			{_importBuildingConfig = value;
			base.StepTotal = this.GetStep();
			RaisePropertyChanged(() => ImportBuildingConfig);			
			}
 		}

		public bool ImportPropertyStr6List
		{
			get { return _importPropertyStr6List; }
			set
			{
				_importPropertyStr6List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr6List);
			}
		}

		public bool ImportPropertyStr7List
		{
			get { return _importPropertyStr7List; }
			set
			{
				_importPropertyStr7List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr7List);
			}
		}



		public bool ImportPreviousInventory
		{
			get { return _importPreviousInventory; }
			set { _importPreviousInventory = value;
			base.StepTotal = this.GetStep();
			RaisePropertyChanged(() => ImportPreviousInventory);
			}
		}

	   private int GetStep()
	   {
			int step = 0;
			 if (this.ImportCatalog == true)  step ++;
			 if (this.ImportLocation == true)  step ++;
			 if (this.ImportBuildingConfig == true) step++;
			 if (this.ImportPropertyStr6List == true) step++;
			 if (this.ImportPropertyStr7List == true) step++;
			 if (this.ImportPreviousInventory == true) step++;
			 return step;
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
		   this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
		   this.XlsxFormat = true;
		   this._fileName = "Merkava-Infrastructure.xlsx";
            base.IsInvertLetters = false;
			base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding(1255);

			base.StepTotal = this.GetStep();
	
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogMerkavaSqliteXslxAdapter.ini"));
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

		public string  GetPathBD3()
		{
			string inventorCode = String.Empty;
			string branchCode = String.Empty;
			string customerCode = String.Empty;

			if (base.CurrentInventor != null)
			{
				inventorCode = base.CurrentInventor.Code;
			}
			if (base.CurrentBranch != null)
			{
				branchCode = base.CurrentBranch.Code;
			}
			if (base.CurrentCustomer != null)
			{
				customerCode = base.CurrentCustomer.Code;
			}

			string objectCode = inventorCode;
			if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = branchCode;
			if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = customerCode;
			string pathBD3 = System.IO.Path.GetDirectoryName(this.Path) + @"\" + objectCode + ".db3";
			return 	pathBD3;
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
			string pathBD3 = GetPathBD3();

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			StepCurrent = 0;


			if (this.ImportCatalog == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.FastImport = base.IsTryFast;
				provider.FromPathFile = this.Path;
				provider.Parms.Clear();
				provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider.ProviderEncoding = base.Encoding;
				provider.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				provider.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
			

				//if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
				//{
				//	MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
				//	provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				//}
				//if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
				//{
				//	MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
				//	provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				//}

				StepCurrent ++;
				provider.Import();
			
			}

			if (this.ImportLocation == true)
			{
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationMerkavaXslx2SqliteProvider);
				provider2.ToPathDB = pathBD3;// base.GetDbPath;
				provider2.FastImport = base.IsTryFast;
				provider2.FromPathFile = this.Path;
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.ProviderEncoding = base.Encoding;
				provider2.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider2.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
				provider2.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Location";
				StepCurrent++;
				provider2.Import();
			}

			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigMerkavaSqliteXslxProvider);
				provider3.ToPathDB = pathBD3;// base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider3.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "BuildingConfig";
				StepCurrent++;
				provider3.Import();
			}

			if (this.ImportPropertyStr6List == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6ListMerkavaSqliteXslxProvider);
				provider4.ToPathDB = pathBD3;// base.GetDbPath;
				provider4.FastImport = base.IsTryFast;
				provider4.FromPathFile = this.Path;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding;
				provider4.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider4.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				provider4.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr6List";
				StepCurrent++;
				provider4.Import();
			}

			if (this.ImportPropertyStr7List == true)
			{
				IImportProvider provider5 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7ListMerkavaSqliteXslxProvider);
				provider5.ToPathDB = pathBD3;// base.GetDbPath;
				provider5.FastImport = base.IsTryFast;
				provider5.FromPathFile = this.Path;
				provider5.Parms.Clear();
				provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider5.ProviderEncoding = base.Encoding;
				provider5.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider5.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 5;
				provider5.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr7List";
				StepCurrent++;
				provider5.Import();
			}

			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryMerkavaSqliteXslxProvider);
				provider6.ToPathDB = pathBD3;// base.GetDbPath;
				provider6.FastImport = base.IsTryFast;
				provider6.FromPathFile = this.Path;
				provider6.Parms.Clear();
				provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider6.ProviderEncoding = base.Encoding;
				provider6.Parms[ImportProviderParmEnum.DBPath] = pathBD3;// base.GetDbPath;
				provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider6.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 6;
				provider6.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PreviousInventory";
				StepCurrent++;
				provider6.Import();
			}
          
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();

			string pathBD3 = GetPathBD3();

			if (this.ImportCatalog == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportLocation == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationMerkavaXslx2SqliteProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.Clear();
			}

			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigMerkavaSqliteXslxProvider);
				provider1.ToPathDB = pathBD3;// base.GetDbPath;
				provider1.Clear();
			}
			if (this.ImportPropertyStr6List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6ListMerkavaSqliteXslxProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.Clear();
			}

			if (this.ImportPropertyStr7List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7ListMerkavaSqliteXslxProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.Clear();
			}

			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryMerkavaSqliteXslxProvider);
				provider.ToPathDB = pathBD3;// base.GetDbPath;
				provider.Clear();
			}
					
            UpdateLogFromILog();
        }

        #endregion
    }
}