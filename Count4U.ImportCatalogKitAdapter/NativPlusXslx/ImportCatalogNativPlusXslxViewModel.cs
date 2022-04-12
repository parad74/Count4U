using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
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

namespace Count4U.ImportCatalogKitAdapter.NativPlusXslx
{
	public class ImportCatalogNativPlusXslxViewModel : TemplateAdapterOneFileViewModel
	{
		public string _fileName {get; set;}
		public string _branchErpCode = String.Empty;
		//private bool _withFamily;
		//private bool _importSupplier;
		private bool _withQuantityERP;

		private bool _importCatalog;
		private bool _importLocation;
		private bool _importEmployee;
		private bool _importBuildingConfig;
		private bool _importPropertyDecorator;
		private bool _importProfile;
		private bool _importPreviousInventory;
		private bool _importTemplateInventory;
		
		private bool _importPropertyStr1List;
		private bool _importPropertyStr2List;
		private bool _importPropertyStr3List;
		private bool _importPropertyStr4List;
		private bool _importPropertyStr5List;
		private bool _importPropertyStr6List;
		private bool _importPropertyStr7List;
		private bool _importPropertyStr8List;
		private bool _importPropertyStr9List;
		private bool _importPropertyStr10List;
		private bool _importPropertyStr11List;
		private bool _importPropertyStr12List;
		private bool _importPropertyStr13List;
		private bool _importPropertyStr14List;
		private bool _importPropertyStr15List;
		private bool _importPropertyStr16List;
		private bool _importPropertyStr17List;
		private bool _importPropertyStr18List;
		private bool _importPropertyStr19List;
		private bool _importPropertyStr20List;

		private bool _selectAll;
		
		//private CatalogParserPoints _catalogParserPoints;

		public ImportCatalogNativPlusXslxViewModel(IServiceLocator serviceLocator,
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




		public bool SelectAll
		{
			get { return _selectAll; }
			set
			{
				_selectAll = value;
				_importPropertyStr1List = _selectAll;
				_importPropertyStr2List = _selectAll;
				_importPropertyStr3List = _selectAll;
				_importPropertyStr4List = _selectAll;
				_importPropertyStr5List = _selectAll;
				_importPropertyStr6List = _selectAll;
				_importPropertyStr7List = _selectAll;
				_importPropertyStr8List = _selectAll;
				_importPropertyStr9List = _selectAll;
				_importPropertyStr10List = _selectAll;
				_importPropertyStr1List = _selectAll;
				_importCatalog = _selectAll;
				_importLocation = _selectAll;
				//_importEmployee = _selectAll;
				_importBuildingConfig = _selectAll;
				_importPropertyDecorator = _selectAll;
				_importPreviousInventory = _selectAll;
				_importTemplateInventory = _selectAll;

				RaisePropertyChanged(() => ImportPropertyStr1List);
				RaisePropertyChanged(() => ImportPropertyStr2List);
				RaisePropertyChanged(() => ImportPropertyStr3List);
				RaisePropertyChanged(() => ImportPropertyStr4List);
				RaisePropertyChanged(() => ImportPropertyStr5List);
				RaisePropertyChanged(() => ImportPropertyStr6List);
				RaisePropertyChanged(() => ImportPropertyStr7List);
				RaisePropertyChanged(() => ImportPropertyStr8List);
				RaisePropertyChanged(() => ImportPropertyStr9List);
				RaisePropertyChanged(() => ImportPropertyStr10List);
				RaisePropertyChanged(() => ImportCatalog);
				RaisePropertyChanged(() => ImportLocation);
			//	RaisePropertyChanged(() => ImportEmployee);
				RaisePropertyChanged(() => ImportPropertyDecorator);
				RaisePropertyChanged(() => ImportBuildingConfig);
				RaisePropertyChanged(() => ImportPreviousInventory);
				RaisePropertyChanged(() => ImportTemplateInventory);

				base.StepTotal = this.GetStep();
			}
		}

		public bool ImportPropertyStr1List
		{
			get { return _importPropertyStr1List; }
			set
			{
				_importPropertyStr1List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr1List);
			}
		}

		public bool ImportPropertyStr2List
		{
			get { return _importPropertyStr2List; }
			set
			{
				_importPropertyStr2List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr2List);
			}
		}

		public bool ImportPropertyStr3List
		{
			get { return _importPropertyStr3List; }
			set
			{
				_importPropertyStr3List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr3List);
			}
		}


		public bool ImportPropertyStr4List
		{
			get { return _importPropertyStr4List; }
			set
			{
				_importPropertyStr4List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr4List);
			}
		}

		public bool ImportPropertyStr5List
		{
			get { return _importPropertyStr5List; }
			set
			{
				_importPropertyStr5List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr5List);
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


		public bool ImportPropertyStr8List
		{
			get { return _importPropertyStr8List; }
			set
			{
				_importPropertyStr8List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr8List);
			}
		}

		public bool ImportPropertyStr9List
		{
			get { return _importPropertyStr9List; }
			set
			{
				_importPropertyStr9List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr9List);
			}
		}

		public bool ImportPropertyStr10List
		{
			get { return _importPropertyStr10List; }
			set
			{
				_importPropertyStr10List = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyStr10List);
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


		public bool ImportPropertyDecorator
		{
			get { return _importPropertyDecorator; }
			set
			{
				_importPropertyDecorator = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportPropertyDecorator);
			}
		}


   		public bool ImportProfile
		{
			get { return _importProfile; }
			set
			{
				_importProfile = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportProfile);
			}
		}

		public bool ImportCatalog
		{
			get { return _importCatalog; }
			set
			{
				_importCatalog = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportCatalog);
			}
		}

		public bool ImportLocation
		{
			get { return _importLocation; }
			set
			{
				_importLocation = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportLocation);
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


		public bool ImportTemplateInventory
		{
			get { return _importTemplateInventory; }
			set
			{
				_importTemplateInventory = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => ImportTemplateInventory);
			}
		}
	

		public bool WithQuantityERP
		{
			get { return _withQuantityERP; }
			set
			{
				_withQuantityERP = value;
				base.StepTotal = this.GetStep();
				RaisePropertyChanged(() => WithQuantityERP);
			}
		}

		private int GetStep()
		{
			int step = 0;
			if (this.ImportCatalog == true) step = step + 5;
			if (this.ImportLocation == true) step = step + 2;
			// if (this.ImportEmployee == true)  step ++;
			if (this.ImportBuildingConfig == true) step++;
			if (this.ImportPropertyDecorator == true) step++;
			if (this.ImportPropertyStr1List == true) step++;
			if (this.ImportPropertyStr2List == true) step++;
			if (this.ImportPropertyStr3List == true) step++;
			if (this.ImportPropertyStr4List == true) step++;
			if (this.ImportPropertyStr5List == true) step++;
			if (this.ImportPropertyStr6List == true) step++;
			if (this.ImportPropertyStr7List == true) step++;
			if (this.ImportPropertyStr8List == true) step++;
			if (this.ImportPropertyStr9List == true) step++;
			if (this.ImportPropertyStr10List == true) step++;
			if (this.ImportPreviousInventory == true) step++;
			if (this.ImportTemplateInventory == true) step++;
			if (this.ImportProfile == true) step++;
			
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
			this._fileName = "Nativ_Infrastructure.xlsx";
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			base.Encoding = System.Text.Encoding.GetEncoding(1255);

			base.StepTotal = this.GetStep();


			//if (string.IsNullOrWhiteSpace(this._maskViewModel.MakatMask) == true)
			//{
			//	this._maskViewModel.MakatMask = "0000000000000{S}";
			//}
			//if (string.IsNullOrWhiteSpace(this._maskViewModel.BarcodeMask) == true)
			//{
			//	this._maskViewModel.BarcodeMask = "7290000000000{S}";
			//}


		}

		public override void InitFromIni()
		{
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData(String.Empty, base.GetPathToIniFile("Count4U.ImportCatalogNativPlusXslxAdapter.ini"));
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
			{	//ProductCatalogNativXslx2SdfParser
				IImportProvider providerCatalog = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNativXslx2SdfProvider);
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
				providerCatalog.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
			

				//	FamilyNativXslx2SdfParser
				IImportProvider providerFamily = this.GetProviderInstance(ImportProviderEnum.ImportFamilyNativXslx2SdfProvider);
				providerFamily.ToPathDB = base.GetDbPath;
				providerFamily.FastImport = base.IsTryFast;
				providerFamily.FromPathFile = this.Path;
				providerFamily.Parms.Clear();
				providerFamily.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerFamily.ProviderEncoding = base.Encoding;
				providerFamily.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerFamily.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerFamily.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerFamily.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerFamily.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerFamily.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				providerFamily.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
				

				//SectionNativXslx2SdfParser
				IImportProvider providerSection = this.GetProviderInstance(ImportProviderEnum.ImportSectionNativXslx2SdfProvider);
				providerSection.ToPathDB = base.GetDbPath;
				providerSection.FastImport = base.IsTryFast;
				providerSection.FromPathFile = this.Path;
				providerSection.Parms.Clear();
				providerSection.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerSection.ProviderEncoding = base.Encoding;
				providerSection.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerSection.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerSection.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerSection.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerSection.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerSection.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				providerSection.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
			

				//SectionNativXslx2SdfParser1
				IImportProvider providerSubSection = this.GetProviderInstance(ImportProviderEnum.ImportSectionNativXslx2SdfProvider1);
				providerSubSection.ToPathDB = base.GetDbPath;
				providerSubSection.FastImport = base.IsTryFast;
				providerSubSection.FromPathFile = this.Path;
				providerSubSection.Parms.Clear();
				providerSubSection.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerSubSection.ProviderEncoding = base.Encoding;
				providerSubSection.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerSubSection.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerSubSection.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerSubSection.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerSubSection.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerSubSection.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				providerSubSection.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
			

				//SupplierNativXslx2SdfParser
				IImportProvider providerSupplier = this.GetProviderInstance(ImportProviderEnum.ImportSupplierNativXslx2SdfProvider);
				providerSupplier.ToPathDB = base.GetDbPath;
				providerSupplier.FastImport = base.IsTryFast;
				providerSupplier.FromPathFile = this.Path;
				providerSupplier.Parms.Clear();
				providerSupplier.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerSupplier.ProviderEncoding = base.Encoding;
				providerSupplier.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerSupplier.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerSupplier.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerSupplier.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerSupplier.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerSupplier.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				providerSupplier.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Catalog";
			

				//provider.Clear();
				StepCurrent++;
				providerCatalog.Import();
				StepCurrent++;
				providerFamily.Import();
				StepCurrent++;
				providerSection.Import();
				StepCurrent++;
				providerSubSection.Import();
				StepCurrent++;
				providerSupplier.Import();

			}

			if (this.ImportProfile == true)
			{																					 //PropertyStrProfileNativXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportProfileNativXslx2SdfProvider);
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
				provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 17;
				provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Profile";
				StepCurrent++;
				provider3.Clear();
				provider3.Import();
			}

			//	IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider);

			if (this.ImportLocation == true)
			{																															  //LocationNativXslx2SdfParser
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationNativXslx2SdfProvider);
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

				//IturNativXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportIturNativXslx2SdfProvider);
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

				StepCurrent++;
				provider3.Import();
			}

			if (this.ImportBuildingConfig == true)
			{																					 //PropertyStrBuildingConfigNativXslx2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigNativXslx2SdfProvider);
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

		

			if (this.ImportPreviousInventory == true)
			{		  //PreviousInventoryNativPlusXslx2DbSetParser
				IImportProvider provider8 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativPlusDbSetProvider);
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
				provider8.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 4;
				provider8.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PreviousInventory";
				StepCurrent++;
				provider8.Clear();
				provider8.Import();
			}

			IImportProvider providerPrp1 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr1NativXslx2SdfProvider);
			//providerPrp1.Clear();
			if (this.ImportPropertyStr1List == true)
			{	//Code
				//Nane
				providerPrp1.ToPathDB = base.GetDbPath;
				providerPrp1.FastImport = base.IsTryFast;
				providerPrp1.FromPathFile = this.Path;
				providerPrp1.Parms.Clear();
				providerPrp1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerPrp1.ProviderEncoding = base.Encoding;
				providerPrp1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerPrp1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerPrp1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerPrp1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerPrp1.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerPrp1.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 5;
				providerPrp1.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr1List";
				StepCurrent++;
				providerPrp1.Import();
			}

			IImportProvider providerPrp2 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr2NativXslx2SdfProvider);
			//providerPrp2.Clear();
			if (this.ImportPropertyStr2List == true)
			{
				//Code
				//ID
				providerPrp2.ToPathDB = base.GetDbPath;
				providerPrp2.FastImport = base.IsTryFast;
				providerPrp2.FromPathFile = this.Path;
				providerPrp2.Parms.Clear();
				providerPrp2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerPrp2.ProviderEncoding = base.Encoding;
				providerPrp2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerPrp2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerPrp2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerPrp2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerPrp2.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerPrp2.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 6;
				providerPrp2.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr2List";
				StepCurrent++;
				providerPrp2.Import();
			}

			IImportProvider providerPrp3 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr3NativXslx2SdfProvider);
			//providerPrp3.Clear();
			if (this.ImportPropertyStr3List == true)
			{
				//Code
				//ID
				providerPrp3.ToPathDB = base.GetDbPath;
				providerPrp3.FastImport = base.IsTryFast;
				providerPrp3.FromPathFile = this.Path;
				providerPrp3.Parms.Clear();
				providerPrp3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerPrp3.ProviderEncoding = base.Encoding;
				providerPrp3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerPrp3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerPrp3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerPrp3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerPrp3.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerPrp3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 7;
				providerPrp3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr3List";
				StepCurrent++;
				providerPrp3.Import();
			}

			IImportProvider provider4List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr4NativXslx2SdfProvider);
			if (this.ImportPropertyStr4List == true)
			{
				//Code
				//ID
				provider4List.ToPathDB = base.GetDbPath;
				provider4List.FastImport = base.IsTryFast;
				provider4List.FromPathFile = this.Path;
				provider4List.Parms.Clear();
				provider4List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4List.ProviderEncoding = base.Encoding;
				provider4List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider4List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider4List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider4List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider4List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 8;
				provider4List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr4List";
				StepCurrent++;
				provider4List.Import();
			}

			IImportProvider provider5List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr5NativXslx2SdfProvider);
			if (this.ImportPropertyStr5List == true)
			{
				//Code
				//ID
				provider5List.ToPathDB = base.GetDbPath;
				provider5List.FastImport = base.IsTryFast;
				provider5List.FromPathFile = this.Path;
				provider5List.Parms.Clear();
				provider5List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider5List.ProviderEncoding = base.Encoding;
				provider5List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider5List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider5List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider5List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider5List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider5List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 9;
				provider5List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr5List";
				StepCurrent++;
				provider5List.Import();
			}

			IImportProvider provider6List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6NativXslx2SdfProvider);
			if (this.ImportPropertyStr6List == true)
			{
				//Code
				//ID
				provider6List.ToPathDB = base.GetDbPath;
				provider6List.FastImport = base.IsTryFast;
				provider6List.FromPathFile = this.Path;
				provider6List.Parms.Clear();
				provider6List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider6List.ProviderEncoding = base.Encoding;
				provider6List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider6List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider6List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider6List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider6List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider6List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 10;
				provider6List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr6List";
				StepCurrent++;
				provider6List.Import();
			}

			IImportProvider provider7List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7NativXslx2SdfProvider);
			if (this.ImportPropertyStr7List == true)
			{
				//Code
				//ID
				provider7List.ToPathDB = base.GetDbPath;
				provider7List.FastImport = base.IsTryFast;
				provider7List.FromPathFile = this.Path;
				provider7List.Parms.Clear();
				provider7List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider7List.ProviderEncoding = base.Encoding;
				provider7List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider7List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider7List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider7List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider7List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider7List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 11;
				provider7List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr7List";
				StepCurrent++;
				provider7List.Import();
			}

			IImportProvider provider8List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr8NativXslx2SdfProvider);
			if (this.ImportPropertyStr8List == true)
			{
				//Code
				//ID
				provider8List.ToPathDB = base.GetDbPath;
				provider8List.FastImport = base.IsTryFast;
				provider8List.FromPathFile = this.Path;
				provider8List.Parms.Clear();
				provider8List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider8List.ProviderEncoding = base.Encoding;
				provider8List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider8List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider8List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider8List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider8List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider8List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 12;
				provider8List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr8List";
				StepCurrent++;
				provider8List.Import();
			}

			IImportProvider provider9List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr9NativXslx2SdfProvider);
			if (this.ImportPropertyStr9List == true)
			{
				//Code
				//ID
				provider9List.ToPathDB = base.GetDbPath;
				provider9List.FastImport = base.IsTryFast;
				provider9List.FromPathFile = this.Path;
				provider9List.Parms.Clear();
				provider9List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider9List.ProviderEncoding = base.Encoding;
				provider9List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider9List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider9List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider9List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider9List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider9List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 13;
				provider9List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr9List";
				StepCurrent++;
				provider9List.Import();
			}

			IImportProvider provider10List = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr10NativXslx2SdfProvider);
			if (this.ImportPropertyStr10List == true)
			{
				//Code
				//ID
				provider10List.ToPathDB = base.GetDbPath;
				provider10List.FastImport = base.IsTryFast;
				provider10List.FromPathFile = this.Path;
				provider10List.Parms.Clear();
				provider10List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider10List.ProviderEncoding = base.Encoding;
				provider10List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider10List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider10List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider10List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				provider10List.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				provider10List.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 14;
				provider10List.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyStr10List";
				StepCurrent++;
				provider10List.Import();
			}


			//ImportTemplateInventory																									  ////TemplateInventoryNativSdf2SqliteParser
			IImportProvider providerTemplateInventory = this.GetProviderInstance(ImportProviderEnum.ImportTemplateInventoryNativPlusDbSetProvider);
			if (this.ImportTemplateInventory == true)
			{
				//Code
				//ID
				providerTemplateInventory.ToPathDB = base.GetDbPath;
				providerTemplateInventory.FastImport = base.IsTryFast;
				providerTemplateInventory.FromPathFile = this.Path;
				providerTemplateInventory.Parms.Clear();
				providerTemplateInventory.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerTemplateInventory.ProviderEncoding = base.Encoding;
				providerTemplateInventory.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerTemplateInventory.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerTemplateInventory.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerTemplateInventory.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerTemplateInventory.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerTemplateInventory.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 15;
				providerTemplateInventory.Parms[ImportProviderParmEnum.SheetNameXlsx] = "TemplateInventory";
				StepCurrent++;
				providerTemplateInventory.Clear();
				providerTemplateInventory.Import();
			}


			//ImportPropertyDecoratorNativXslx2SdfProvider
			if (this.ImportPropertyDecorator == true)
			{																					 //PropertyStrPropertyDecoratorNativXslx2SdfParser
				IImportProvider providerDecorator = this.GetProviderInstance(ImportProviderEnum.ImportPropertyDecoratorNativXslx2SdfProvider);
				providerDecorator.ToPathDB = base.GetDbPath;
				providerDecorator.FastImport = base.IsTryFast;
				providerDecorator.FromPathFile = this.Path;
				providerDecorator.Parms.Clear();
				providerDecorator.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerDecorator.ProviderEncoding = base.Encoding;
				providerDecorator.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerDecorator.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerDecorator.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerDecorator.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				providerDecorator.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				providerDecorator.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 16;	//TODO
				providerDecorator.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyDecorator";
				StepCurrent++;
				providerDecorator.Clear();
				providerDecorator.Import();
			}

			TemporaryInventory temporaryInventory = base.GetTemporaryInventoryWithImportModuleInfo
	(Common.Constants.ImportAdapterName.ImportCatalogNativPlusXslxAdapter, "IMPORT ADAPTER NATIV +", this._fileName, updateDateTime);
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
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();

				IImportProvider providerFamily = this.GetProviderInstance(ImportProviderEnum.ImportFamilyNativXslx2SdfProvider);
				providerFamily.ToPathDB = base.GetDbPath;
				providerFamily.Clear();

				IImportProvider providerSection = this.GetProviderInstance(ImportProviderEnum.ImportSectionNativXslx2SdfProvider);
				providerSection.ToPathDB = base.GetDbPath;
				providerSection.Clear();

				IImportProvider providerSupplier = this.GetProviderInstance(ImportProviderEnum.ImportSupplierNativXslx2SdfProvider);
				providerSupplier.ToPathDB = base.GetDbPath;
				providerSupplier.Clear();


			}
			if (this.ImportLocation == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationNativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();

				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturNativXslx2SdfProvider);
				provider1.ToPathDB = base.GetDbPath;
				provider1.Clear();
			}
			if (this.ImportBuildingConfig == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigNativXslx2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.Clear();
			}


			if (this.ImportPropertyDecorator == true)
			{
				IImportProvider providerPropertyDecorator = this.GetProviderInstance(ImportProviderEnum.ImportPropertyDecoratorNativXslx2SdfProvider);
				providerPropertyDecorator.ToPathDB = base.GetDbPath;
				providerPropertyDecorator.Clear();
			}


			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativDbSetProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.Clear();
			}


			if (this.ImportPropertyStr1List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr1NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}

			if (this.ImportPropertyStr2List == true)
			{
				IImportProvider provider7 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr2NativXslx2SdfProvider);
				provider7.ToPathDB = base.GetDbPath;
				provider7.Clear();
			}

			if (this.ImportPropertyStr3List == true)
			{
				IImportProvider provider7 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr3NativXslx2SdfProvider);
				provider7.ToPathDB = base.GetDbPath;
				provider7.Clear();
			}

			if (this.ImportPropertyStr4List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr4NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr5List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr5NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr6List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr6NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr7List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr7NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr8List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr8NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr9List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr9NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}
			if (this.ImportPropertyStr10List == true)
			{
				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr10NativXslx2SdfProvider);
				provider.ToPathDB = base.GetDbPath;
				provider.Clear();
			}

			if (this.ImportTemplateInventory == true)
			{
				IImportProvider providerTemplateInventory = this.GetProviderInstance(ImportProviderEnum.ImportTemplateInventoryNativPlusDbSetProvider);
				providerTemplateInventory.ToPathDB = base.GetDbPath;
				providerTemplateInventory.Clear();
			}

			UpdateLogFromILog();
		}

		#endregion
	}
}