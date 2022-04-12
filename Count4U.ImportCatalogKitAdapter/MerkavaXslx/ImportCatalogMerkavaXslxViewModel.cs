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
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.MerkavaXslx
{
	public class ImportCatalogMerkavaXslxViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        private string _branchErpCode = String.Empty;
		//private bool _withFamily;
		//private bool _importSupplier;
		private bool _withQuantityERP;

		private bool _importCatalog;
		private bool _importLocation;
		private bool _importEmployee;
		private bool _importPreviousInventory;
		private bool _importPropertyStr6List;
		private bool _importPropertyStr7List;
		private bool _importPropertyStr18List;
		private bool _importBuildingConfig;
		
	//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogMerkavaXslxViewModel(IServiceLocator serviceLocator,
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

		//public bool ImportSupplier
		//{
		//	get { return this._importSupplier; }
		//	set
		//	{
		//		this._importSupplier = value;
		//		if (this.ImportSupplier == true) base.StepTotal = base.StepTotal + 1;
		//		RaisePropertyChanged(() => ImportSupplier);
		//	}
		//}
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

		public bool ImportPropertyStr18List
		{
			get { return _importPropertyStr18List; }
			set
			{
				_importPropertyStr18List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr18List);
			}
		}

		public bool ImportBuildingConfig
		{
			get { return _importBuildingConfig; }
			set
			{
				_importBuildingConfig = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportBuildingConfig);
			}
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

		public bool ImportEmployee
		{
			get { return _importEmployee; }
			set { _importEmployee = value;
			base.StepTotal = this.GetStep();
			RaisePropertyChanged(() => ImportEmployee);			
			}

		}

		public bool ImportPreviousInventory
		{
			get { return _importPreviousInventory; }
			set
			{
				_importPreviousInventory = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPreviousInventory);
			}
		}
		public bool WithQuantityERP
		{
			get { return _withQuantityERP; }
			set { _withQuantityERP = value;
			base.StepTotal = this.GetStep();
			RaisePropertyChanged(() => WithQuantityERP);
			}
		}

	   private int GetStep()
	   {
			int step = 0;
			 if (this.ImportCatalog == true)  step ++;
			 if (this.ImportLocation == true) step = step + 2;
			// if (this.ImportEmployee == true)  step ++;
			 if (this.ImportBuildingConfig == true) step++;
			 if (this.ImportPropertyStr6List == true) step = step + 2;
			 if (this.ImportPropertyStr7List == true) step++;
			 if (this.ImportPropertyStr18List == true) step++;
			 if (this.ImportPreviousInventory == true) step++;
	//		 if (this.WithQuantityERP == true) step++;
			 return step;
 	   }

	   //private  SetMode()
	   //{
	   //	int step = 0;
	   //	if (this.ImportCatalog == true){
	   //	 base.
	   //	 }
	   //	if (this.ImportLocation == true) step++;
	   //	if (this.ImportEmployee == true) step++;
	   //	if (this.WithQuantityERP == true) step++;
	   //	return step;
	   //}

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
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogMerkavaXslxAdapter.ini"));
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

			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
			this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			StepCurrent = 0;
			if (this.ImportCatalog == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaXslxProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.FastImport = base.IsTryFast;
				provider.FromPathFile = this.Path;
				provider.Parms.Clear();
				provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider.ProviderEncoding = base.Encoding;
				provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				provider.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";

				if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
				{
					MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
					provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				}
				if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
				{
					MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
					provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
				}

				StepCurrent ++;
	
				provider.Clear();
				provider.Import();

		}

		//	IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider);

			if (this.ImportLocation == true)
			{																															  //LocationMerkavaXslx2SdfParser
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationMerkavaXslx2SdfProvider);
				provider2.ToPathDB = base.GetDbPath;
				provider2.FastImport = base.IsTryFast;
				provider2.FromPathFile = this.Path;
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.ProviderEncoding = base.Encoding;
				provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider2.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
				provider2.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Location";

				//IturMerkavaXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportIturMerkavaXslx2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider3.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Location";
				
				//provider2.Clear();
				//provider3.Clear();

				StepCurrent++;
				provider2.Import();

				StepCurrent ++;
				provider3.Import();
			}

			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigMerkavaXslx2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this.Path;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider3.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "BuildingConfig";
				StepCurrent++;
				provider3.Clear();
				provider3.Import();
			}

			if (this.ImportPropertyStr7List == true)
			{	//Code
				//Nane
				IImportProvider provider7 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7MerkavaXslx2SdfProvider);
				provider7.ToPathDB = base.GetDbPath;
				provider7.FastImport = base.IsTryFast;
				provider7.FromPathFile = this.Path;
				provider7.Parms.Clear();
				provider7.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider7.ProviderEncoding = base.Encoding;
				provider7.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider7.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider7.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider7.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider7.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider7.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 5;
				provider7.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr7List";
				StepCurrent++;
				provider7.Clear();
				provider7.Import();
			}

	
			if (this.ImportPropertyStr6List == true)
			{
				//Code
				//ID
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6MerkavaXslx2SdfProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.FastImport = base.IsTryFast;
				provider6.FromPathFile = this.Path;
				provider6.Parms.Clear();
				provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider6.ProviderEncoding = base.Encoding;
				provider6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider6.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				provider6.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr6List";
				StepCurrent++; 
				provider6.Clear();
				provider6.Import();

				//Code
				//ID		 PropertyStr6List 	=> PropertyStr7List
				IImportProvider provider61 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7MerkavaXslx2SdfProvider1);
				provider61.ToPathDB = base.GetDbPath;
				provider61.FastImport = base.IsTryFast;
				provider61.FromPathFile = this.Path;
				provider61.Parms.Clear();
				provider61.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider61.ProviderEncoding = base.Encoding;
				provider61.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider61.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider61.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider61.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider61.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider61.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				provider61.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr6List";
				StepCurrent++;
				provider61.Import();
			}



			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider8 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryMerkavaDbSetProvider);
				provider8.ToPathDB = base.GetDbPath;
				provider8.FastImport = base.IsTryFast;
				provider8.FromPathFile = this.Path;
				provider8.Parms.Clear();
				provider8.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider8.ProviderEncoding = base.Encoding;
				provider8.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider8.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider8.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider8.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider8.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider8.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 6;
				provider8.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PreviousInventory";
				StepCurrent++;
				provider8.Clear();
				provider8.Import();
			}

			if (this.ImportPropertyStr18List == true)
			{	//Code
				//Nane
				IImportProvider provider18 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr18MerkavaXslx2SdfProvider);
				provider18.ToPathDB = base.GetDbPath;
				provider18.FastImport = base.IsTryFast;
				provider18.FromPathFile = this.Path;
				provider18.Parms.Clear();
				provider18.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider18.ProviderEncoding = base.Encoding;
				provider18.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider18.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider18.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider18.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider18.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider18.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 7;
				provider18.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr18List";
				StepCurrent++;
				provider18.Clear();
				provider18.Import();
			}

			TemporaryInventory temporaryInventory = base.GetTemporaryInventoryWithImportModuleInfo
(Common.Constants.ImportAdapterName.ImportCatalogMerkavaXslxAdapter, "IMPORT ADAPTER", this._fileName, updateDateTime);
			this.TemporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);   


            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();

			if (this.ImportCatalog == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaXslxProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportLocation == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationMerkavaXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();

				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturMerkavaXslx2SdfProvider);
				provider1.ToPathDB = base.GetDbPath;
				provider1.Clear();
			}
			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigMerkavaXslx2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();
			}

			if (this.ImportPropertyStr6List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6MerkavaXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			
			if (this.ImportPropertyStr7List == true)   
			{
				IImportProvider provider7 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7MerkavaXslx2SdfProvider);
				provider7.ToPathDB = base.GetDbPath;
				provider7.Clear();
			}

			if (this.ImportPropertyStr18List == true)
			{
				IImportProvider provider18 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr18MerkavaXslx2SdfProvider);
				provider18.ToPathDB = base.GetDbPath;
				provider18.Clear();
			}


			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryMerkavaDbSetProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.Clear();
			}

			UpdateLogFromILog();
        }

        #endregion
    }
}