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
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ImportCatalogKitAdapter.ClalitXslx
{
	public class ImportCatalogClalitXslxViewModel : TemplateAdapterOneFileViewModel
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
		private bool _importPropertyStrList;
		private bool _importPropertyStr7List;
		private bool _importBuildingConfig;
		private bool _xlsmFormat;
	//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogClalitXslxViewModel(IServiceLocator serviceLocator,
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

		public bool XlsmFormat
		{
			get { return this._xlsmFormat; }
			set { this._xlsmFormat = value; }
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
		public bool ImportPropertyStrList
		{
			get { return _importPropertyStrList; }
			set
			{
				_importPropertyStrList = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStrList);
			}
		}

		//public bool ImportPropertyStr7List
		//{
		//	get { return _importPropertyStr7List; }
		//	set
		//	{
		//		_importPropertyStr7List = value;
		//		base.StepTotal = this.GetStep();
		//		RaisePropertyChanged(() => ImportPropertyStr7List);
		//	}
		//}

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
			 if (this.ImportLocation == true) step++; //2;
			// if (this.ImportEmployee == true)  step ++;
			 if (this.ImportBuildingConfig == true) step++;
			 if (this.ImportPropertyStrList == true) step = step + 5;
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
		   this.PathFilter = "*.xlsm|*.xlsm|All files (*.*)|*.*";
		   this._fileName = "CA_Sfira_new_08AProd-Fixed_Tables.xlsm";
            base.IsInvertLetters = false;
			base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding(1255);

			this.ImportCatalog = true;
			base.StepTotal = this.GetStep();
	
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogClalitXslxAdapter.ini"));
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            if (this._fileName.Contains("XXX") == true)
            {
                this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
            }

            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			string extension = System.IO.Path.GetExtension(base.Path);

			if (extension == ".xlsm") this.XlsmFormat = true; else this.XlsmFormat = false;
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
	

		//	IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider);

			if (this.ImportLocation == true)
			{
				// большой словарь - поэтому делаем за один проход 	
				//LocationClalitXslx2SdfParser
				//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationClalitXslx2SdfProvider);
				//provider2.ToPathDB = base.GetDbPath;
				//provider2.FastImport = base.IsTryFast;
				//provider2.FromPathFile = this.Path;
				//provider2.Parms.Clear();
				//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider2.ProviderEncoding = base.Encoding;
				//provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				//provider2.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				//provider2.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				//provider2.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Slocation";

				//IturClalitXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportIturClalitXslx2SdfProvider);
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
				provider3.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Slocation";
				
				//provider2.Clear();
				provider3.Clear();

				//StepCurrent++;
				//provider2.Import();

				StepCurrent ++;
				provider3.Import();
			}

			if (this.ImportCatalog == true)
			{																					  //ProductCatalogClalitXslx2SdfParser
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogClalitXslxProvider);
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
				provider.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				provider.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog Virtual";

				StepCurrent++;

				provider.Clear();
				provider.Import();

			}

			if (this.ImportBuildingConfig == true)
			{																						 //PropertyStrBuildingConfigClalitXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigClalitXslx2SdfProvider);
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
				provider3.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "BuildingConfig";
				StepCurrent++;
				provider3.Clear();
				provider3.Import();
			}

			if (this.ImportPropertyStrList == true)
			{	
				//Field4: configuration_stat_id
				//Field5: descr
				//PropertyStr6
				//CountExcludeFirstRow = 8	 / col =  4
				IImportProvider provider1 = this.GetProviderInstance(
					ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider1);				//PropertyStrClalitXslx2SdfParser1
				provider1.ToPathDB = base.GetDbPath;
				provider1.FastImport = base.IsTryFast;
				provider1.FromPathFile = this.Path;
				provider1.Parms.Clear();
				provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider1.ProviderEncoding = base.Encoding;
				provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider1.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 5;
				provider1.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Sstatus";
				StepCurrent++;
				provider1.Clear();
				provider1.Import();

				//Field3: prod_id
				//Field4: descr
				// PropertyStr2		  
				//CountExcludeFirstRow = 8		/ / col =  3
				IImportProvider provider2 = this.GetProviderInstance(
					ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider2);	  	//PropertyStrClalitXslx2SdfParser2
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
				provider2.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 7;
				provider2.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Sprod";
				StepCurrent++;
				provider2.Clear();
				provider2.Import();
				
				//Field3: model_id
				//Field4: model_name
				// PropertyStr4		  
				//CountExcludeFirstRow = 8	 // col =  3
				IImportProvider provider3 = this.GetProviderInstance(
					ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider3);	 	//PropertyStrClalitXslx2SdfParser3
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
				provider3.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 10;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Smodel";
				StepCurrent++;
				provider3.Clear();
				provider3.Import();

				//Field6: entry_id
				//Field4: descr
				//PropertyStr8
				//CountExcludeFirstRow = 8	 / col =  4
				IImportProvider provider4 = this.GetProviderInstance(
					ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider4);	 	//PropertyStrClalitXslx2SdfParser4
				provider4.ToPathDB = base.GetDbPath;
				provider4.FastImport = base.IsTryFast;
				provider4.FromPathFile = this.Path;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding;
				provider4.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider4.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 11;
				provider4.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Szag";
				StepCurrent++;
				provider4.Clear();
				provider4.Import();

				//Field3: vend_id
				//Field4: descr
				//PropertyStr3
				//CountExcludeFirstRow = 8	 / col =  3
				IImportProvider provider5 = this.GetProviderInstance(
					ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider5);	  	//PropertyStrClalitXslx2SdfParser5
				provider5.ToPathDB = base.GetDbPath;
				provider5.FastImport = base.IsTryFast;
				provider5.FromPathFile = this.Path;
				provider5.Parms.Clear();
				provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider5.ProviderEncoding = base.Encoding;
				provider5.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider5.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 12;
				provider5.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Syatzran";
				StepCurrent++;
				provider5.Clear();
				provider5.Import();
			}


			//CountExcludeFirstRow = 7	 / col =  0
			if (this.ImportPreviousInventory == true)
			{																														 //PreviousInventoryClalitXslx2DbSetParser
				IImportProvider provider8 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryClalitDbSetProvider);
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
				provider8.Parms[ImportProviderParmEnum.FileXlsm] = this.XlsmFormat ? "1" : String.Empty;
				provider8.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
				provider8.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Ssfira";
				StepCurrent++;
				provider8.Clear();
				provider8.Import();
			}

			TemporaryInventory temporaryInventory = base.GetTemporaryInventoryWithImportModuleInfo
(Common.Constants.ImportAdapterName.ImportCatalogClalitXslxAdapter, "IMPORT ADAPTER", this._fileName, updateDateTime);
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
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogClalitXslxProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportLocation == true)
			{
				// большой словарь - поэтому делаем за один проход 
				//IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationClalitXslx2SdfProvider);
				//provider.ToPathDB = base.GetDbPath;
				//provider.Clear();

				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturClalitXslx2SdfProvider);
				provider1.ToPathDB = base.GetDbPath;
				provider1.Clear();
			}
			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigClalitXslx2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();
			}

			if (this.ImportPropertyStrList == true)
			{

				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider1);
				provider1.ToPathDB = base.GetDbPath;
				provider1.Clear();

				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider2);
				provider2.ToPathDB = base.GetDbPath;
				provider2.Clear();

				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider3);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();

				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider4);
				provider4.ToPathDB = base.GetDbPath;
				provider4.Clear();

				IImportProvider provider5 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider5);
				provider5.ToPathDB = base.GetDbPath;
				provider5.Clear();


			}


			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryClalitDbSetProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.Clear();
			}

			UpdateLogFromILog();
        }

        #endregion
    }
}