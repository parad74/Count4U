using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using System.IO;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Model.Count4Mobile;

namespace Count4U.ImportCatalogNativPlusLadpcAdapter
{
    public class ImportCatalogNativPlusLadpcAdapterViewModel : TemplateAdapterFileFolderViewModel
    {
		public string _fileName {get; set;}
		private string _pathLocation;
		private string _pathPreviousInventory;
		private string _pathCatalog;
		private string _pathInfrastructureFile;

		private bool XlsxFormat;
		private string _branchErpCode = String.Empty;
		//private bool _withQuantityERP;

		private bool _importLocation;
		private bool _importCatalog;
		private bool _importPreviousInventory;
		private bool _updateSN;
		//from Nativ+
		private bool _importBuildingConfig;
		private bool _importPropertyDecorator;
		private bool _importProfile;

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
	
		private bool _selectAll;

		public ImportCatalogNativPlusLadpcAdapterViewModel(IServiceLocator serviceLocator,
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
			this._isDirectory = true;
			this._isSingleFile = false;
        }

		public string PathLocation
		{
			get { return this._pathLocation; }
			set
			{
				this._pathLocation = value;
			}
		}

		public string PathPreviousInventory
		{
			get { return this._pathPreviousInventory; }
			set
			{
				this._pathPreviousInventory = value;
			}
		}

		public string PathCatalog
		{
			get { return this._pathCatalog; }
			set
			{
				this._pathCatalog = value;
			}
		}

		public string PathInfrastructureFile
		{
			get { return this._pathInfrastructureFile; }
			set
			{
				this._pathInfrastructureFile = value;
			}
		}

		

		public bool ImportCatalog
		{
			get { return _importCatalog; }
			set
			{
				_importCatalog = value;
				RaisePropertyChanged(() => ImportCatalog);
			}
		}


		public bool ImportLocation
		{
			get { return _importLocation; }
			set
			{
				_importLocation = value;
				RaisePropertyChanged(() => ImportLocation);
			}
		}


		public bool ImportPreviousInventory
		{
			get { return _importPreviousInventory; }
			set
			{
				_importPreviousInventory = value;
					RaisePropertyChanged(() => ImportPreviousInventory);
			}
		}


		public bool UpdateSN
		{
			get { return _updateSN; }
			set
			{
				_updateSN = value;
				RaisePropertyChanged(() => UpdateSN);
			}
		}

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
				//_importCatalog = _selectAll;
				//_importLocation = _selectAll;
				//_importEmployee = _selectAll;
				_importBuildingConfig = _selectAll;
				_importPropertyDecorator = _selectAll;
				//_importPreviousInventory = _selectAll;
				_importPropertyDecorator = _selectAll;
				_importProfile = _selectAll;
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
				RaisePropertyChanged(() => ImportProfile);
				RaisePropertyChanged(() => UpdateSN);
				

			}
		}

		public bool ImportTemplateInventory
		{
			get { return _importTemplateInventory; }
			set
			{
				_importTemplateInventory = value;
				RaisePropertyChanged(() => ImportTemplateInventory);
			}
		}

		public bool ImportPropertyStr1List
		{
			get { return _importPropertyStr1List; }
			set
			{
				_importPropertyStr1List = value;
				RaisePropertyChanged(() => ImportPropertyStr1List);
			}
		}

		public bool ImportPropertyStr2List
		{
			get { return _importPropertyStr2List; }
			set
			{
				_importPropertyStr2List = value;
				RaisePropertyChanged(() => ImportPropertyStr2List);
			}
		}

		public bool ImportPropertyStr3List
		{
			get { return _importPropertyStr3List; }
			set
			{
				_importPropertyStr3List = value;
				RaisePropertyChanged(() => ImportPropertyStr3List);
			}
		}


		public bool ImportPropertyStr4List
		{
			get { return _importPropertyStr4List; }
			set
			{
				_importPropertyStr4List = value;
				RaisePropertyChanged(() => ImportPropertyStr4List);
			}
		}

		public bool ImportPropertyStr5List
		{
			get { return _importPropertyStr5List; }
			set
			{
				_importPropertyStr5List = value;
				RaisePropertyChanged(() => ImportPropertyStr5List);
			}
		}

		public bool ImportPropertyStr6List
		{
			get { return _importPropertyStr6List; }
			set
			{
				_importPropertyStr6List = value;
				RaisePropertyChanged(() => ImportPropertyStr6List);
			}
		}


		public bool ImportPropertyStr7List
		{
			get { return _importPropertyStr7List; }
			set
			{
				_importPropertyStr7List = value;
				RaisePropertyChanged(() => ImportPropertyStr7List);
			}
		}


		public bool ImportPropertyStr8List
		{
			get { return _importPropertyStr8List; }
			set
			{
				_importPropertyStr8List = value;
				RaisePropertyChanged(() => ImportPropertyStr8List);
			}
		}

		public bool ImportPropertyStr9List
		{
			get { return _importPropertyStr9List; }
			set
			{
				_importPropertyStr9List = value;
				RaisePropertyChanged(() => ImportPropertyStr9List);
			}
		}

		public bool ImportPropertyStr10List
		{
			get { return _importPropertyStr10List; }
			set
			{
				_importPropertyStr10List = value;
				RaisePropertyChanged(() => ImportPropertyStr10List);
			}
		}

		public bool ImportBuildingConfig
		{
			get { return _importBuildingConfig; }
			set
			{
				_importBuildingConfig = value;
				RaisePropertyChanged(() => ImportBuildingConfig);
			}
		}


		public bool ImportPropertyDecorator
		{
			get { return _importPropertyDecorator; }
			set
			{
				_importPropertyDecorator = value;
				RaisePropertyChanged(() => ImportPropertyDecorator);
			}
		}


		public bool ImportProfile
		{
			get { return _importProfile; }
			set
			{
				_importProfile = value;
				RaisePropertyChanged(() => ImportProfile);
			}
		}



		private int GetStepPropertyStr()
		{
			int step = 0;
			if (this.ImportBuildingConfig == true) step++;
			if (this.ImportPropertyDecorator == true) step++;
			if (this.ImportTemplateInventory == true) step++;
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
			if (this.ImportProfile == true) step++;

			return step;
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
						this.Path = System.IO.Path.GetFullPath(importPath);
						this.PathInfrastructureFile = System.IO.Path.GetFullPath(importPath + @"\" + this._fileName);
						this.XlsxFormat = true;
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

        }

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
			this._importLocation = true;
			this._importCatalog = true;
			this._importPreviousInventory = true;
			this._updateSN = true;
			

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
			this._fileName = "Nativ_Infrastructure.xlsx";
			this.PathFilter = "*All files (*.*)|*.*";
			this.XlsxFormat = false;

			//this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			string importPath = base.GetImportPath().Trim('\\');
			this.Path = System.IO.Path.GetFullPath(importPath);
			this.PathInfrastructureFile = System.IO.Path.GetFullPath(importPath + @"\" + this._fileName);
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

           
        }

		//public override void InitConfig()
		//{
		//	Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//init GUI
			//this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			//this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			//base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			//base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
			//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") this.XlsxFormat = true; else this.XlsxFormat = false;
		//	this.XlsxFormat = true;
		//}

		public override void InitFromIni()
        {
			//Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			////init GUI
			//this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			//this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			
			// this.XlsxFormat = true;

			//try
			//{
			//	if (base._isDirectory)
			//	{
			//		if (!Directory.Exists(this.Path))
			//			Directory.CreateDirectory(this.Path);
			//	}
			//}
			//catch (Exception exc)
			//{
			//	WriteErrorExceptionToAppLog("Create inData directory", exc);
			//}
        }

		protected override bool PreImportCheck()
		{
			return true;
		}

		public override void Import()
		{
			this.PathCatalog = this.Path + @"\Catalog";
			this.PathLocation = this.Path + @"\Location";
			this.PathPreviousInventory = this.Path + @"\PreviousInventory";

			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

			string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			string newSessionCode = Guid.NewGuid().ToString();

			// now the same format providerLocationQ ==  providerLocationSN

			//LocationNativPlusLadpcParser
			IImportProvider providerLocation = this.GetProviderInstance(ImportProviderEnum.ImportLocationNativPlusLadpcProvider);
			providerLocation.ToPathDB = base.GetDbPath;
			providerLocation.Parms.Clear();
			providerLocation.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerLocation.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerLocation.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerLocation.Parms[ImportProviderParmEnum.FileXlsx] = "0";
			providerLocation.ProviderEncoding = base.Encoding;

			//IturNativPlusLadpcParser
			IImportProvider providerItur1 = this.GetProviderInstance(ImportProviderEnum.ImportIturNativPlusLadpcProvider1);
			providerItur1.ToPathDB = base.GetDbPath;
			providerItur1.Parms.Clear();
			providerItur1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerItur1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerItur1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerItur1.Parms[ImportProviderParmEnum.FileXlsx] = "0";
			providerItur1.ProviderEncoding = base.Encoding;

			IImportProvider providerItur9999 = this.GetProviderInstance(ImportProviderEnum.ImportIturNativPlusLadpcProvider9999);
			providerItur9999.ToPathDB = base.GetDbPath;
			providerItur9999.Parms.Clear();
			providerItur9999.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerItur9999.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerItur9999.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerItur9999.Parms[ImportProviderParmEnum.FileXlsx] = "0";
			providerItur9999.ProviderEncoding = base.Encoding;

			IImportProvider providerDoc = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderAddFristDocToIturBlukProvider);
			providerDoc.ToPathDB = base.GetDbPath;
			providerDoc.FromPathFile = base.GetDbPath;
			providerDoc.Parms.Clear();
			providerDoc.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerDoc.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerDoc.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerDoc.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerDoc.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

			IImportProvider providerCatalog = this.GetProviderInstance(ImportProviderEnum.ImportCatalogNativPlusLadpcProvider);
			providerCatalog.ToPathDB = base.GetDbPath;
			providerCatalog.FromPathFile = base.GetDbPath;
			providerCatalog.Parms.Clear();
			providerCatalog.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerCatalog.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.UpdateSN] = this.UpdateSN ? "1" : String.Empty;
			providerCatalog.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerCatalog.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerCatalog.Parms[ImportProviderParmEnum.FileXlsx] = "0";
			providerCatalog.ProviderEncoding = base.Encoding;

			IImportProvider providerPreviousInventory = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativPlusLadpcDbSetProvider);
			providerPreviousInventory.ToPathDB = base.GetDbPath;
			providerPreviousInventory.FromPathFile = base.GetDbPath;
			providerPreviousInventory.Parms.Clear();
			providerPreviousInventory.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerPreviousInventory.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerPreviousInventory.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerPreviousInventory.Parms[ImportProviderParmEnum.UpdateSN] = this.UpdateSN ? "1" : String.Empty;
			providerPreviousInventory.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerPreviousInventory.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerPreviousInventory.Parms[ImportProviderParmEnum.FileXlsx] = "0";
			providerPreviousInventory.ProviderEncoding = base.Encoding;

			StepCurrent = 0;

			{
				StepCurrent = 0;
				StepTotal = GetStepPropertyStr();
				if (this.ImportCatalog == true)
				{
					var catalogFiles = Directory.GetFiles(this.PathCatalog, "*.csv");
					StepTotal = StepTotal + catalogFiles.Length;
				}
				if (this.ImportLocation == true)
				{
					var locationFiles = Directory.GetFiles(this.PathLocation, "*.csv");
					StepTotal = StepTotal + locationFiles.Length * 3 + 1;
				}
				if (this.ImportPreviousInventory == true)
				{
					var previousInventoryFiles = Directory.GetFiles(this.PathPreviousInventory, "*.csv");
					StepTotal = StepTotal + previousInventoryFiles.Length;
				}

				if (this.ImportLocation == true)
				{
					providerLocation.Clear();
					var locationFiles = Directory.GetFiles(this.PathLocation, "*.csv");
					// =============== Location ===================
					foreach (string filePath in locationFiles)
					{
						StepCurrent++;
						providerLocation.FromPathFile = filePath;
						providerLocation.Import();
					}

					// Clear doc
					IDocumentHeaderRepository docRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
					docRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);
					
					providerItur1.Clear();
					// =============== Itur ===================
					foreach (string filePath in locationFiles)
					{
						providerItur1.FromPathFile = filePath;
						StepCurrent++;
						providerItur1.Parms[ImportProviderParmEnum.IturNameSuffix] = "1";
						providerItur1.Import();

						providerItur9999.FromPathFile = filePath;
						StepCurrent++;
						providerItur9999.Parms[ImportProviderParmEnum.IturNameSuffix] = "9999";
						providerItur9999.Import();
					}
					// =============== Doc ===================
					if (locationFiles.Length > 0)
					{
						StepCurrent++;
						providerDoc.Import();
					}

					List<string> listname = new List<string>(locationFiles);
					base.BackupSourceFilesAfterImport(this.PathLocation, listname, true);
				}

				if (this.ImportCatalog == true)
				{
					providerCatalog.Clear();
					var catalogFiles = Directory.GetFiles(this.PathCatalog, "*.csv");
					// =============== Catalog ===================
					foreach (string filePath in catalogFiles)
					{
						providerCatalog.FromPathFile = filePath;
						StepCurrent++;
						providerCatalog.Import();
					}
					List<string> listname = new List<string>(catalogFiles);
					base.BackupSourceFilesAfterImport(this.PathCatalog, listname, true);
				}

				if (this.ImportPreviousInventory == true)
				{
					providerPreviousInventory.Clear();
					var previousInventoryFiles = Directory.GetFiles(this.PathPreviousInventory, "*.csv");
					// =============== PreviousInventory ===================
					foreach (string filePath in previousInventoryFiles)
					{
						providerPreviousInventory.FromPathFile = filePath;
						StepCurrent++;
						providerPreviousInventory.Import();
					}
					List<string> listname = new List<string>(previousInventoryFiles);
					base.BackupSourceFilesAfterImport(this.PathPreviousInventory, listname, true);
				}
			}

			//============================== From Nativ + all 
			{
				if (this.ImportProfile == true)
				{																					 //PropertyStrProfileNativXslx2SdfParser
					
					IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportProfileNativXslx2SdfProvider);
					provider3.ToPathDB = base.GetDbPath;
					provider3.FastImport = base.IsTryFast;
					provider3.FromPathFile = this.PathInfrastructureFile;
					provider3.Parms.Clear();
					provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider3.ProviderEncoding = base.Encoding;
					provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider3.Parms[ImportProviderParmEnum.FileXlsx] =  "1";
					provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 17;
					provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Profile";
					StepCurrent++;
					provider3.Clear();
					provider3.Import();
				}


				if (this.ImportBuildingConfig == true)
				{																					 //PropertyStrBuildingConfigNativXslx2SdfParser
					IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportBuildingConfigNativXslx2SdfProvider);
					provider3.ToPathDB = base.GetDbPath;
					provider3.FastImport = base.IsTryFast;
					provider3.FromPathFile = this.PathInfrastructureFile;
					provider3.Parms.Clear();
					provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider3.ProviderEncoding = base.Encoding;
					provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider3.Parms[ImportProviderParmEnum.FileXlsx] = "1";
					provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 3;
					provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "BuildingConfig";
					StepCurrent++;
					provider3.Clear();
					provider3.Import();
				}

				IImportProvider providerPrp1 = this.GetProviderInstance(ImportProviderEnum.ImportPropertyStr1NativXslx2SdfProvider);
				//providerPrp1.Clear();
				if (this.ImportPropertyStr1List == true)
				{	//Code
					//Nane
					providerPrp1.ToPathDB = base.GetDbPath;
					providerPrp1.FastImport = base.IsTryFast;
					providerPrp1.FromPathFile = this.PathInfrastructureFile;
					providerPrp1.Parms.Clear();
					providerPrp1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					providerPrp1.ProviderEncoding = base.Encoding;
					providerPrp1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					providerPrp1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					providerPrp1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					providerPrp1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					providerPrp1.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					providerPrp2.FromPathFile = this.PathInfrastructureFile;
					providerPrp2.Parms.Clear();
					providerPrp2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					providerPrp2.ProviderEncoding = base.Encoding;
					providerPrp2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					providerPrp2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					providerPrp2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					providerPrp2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					providerPrp2.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					providerPrp3.FromPathFile = this.PathInfrastructureFile;
					providerPrp3.Parms.Clear();
					providerPrp3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					providerPrp3.ProviderEncoding = base.Encoding;
					providerPrp3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					providerPrp3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					providerPrp3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					providerPrp3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					providerPrp3.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider4List.FromPathFile = this.PathInfrastructureFile;
					provider4List.Parms.Clear();
					provider4List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider4List.ProviderEncoding = base.Encoding;
					provider4List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider4List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider4List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider4List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider4List.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider5List.FromPathFile = this.PathInfrastructureFile;
					provider5List.Parms.Clear();
					provider5List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider5List.ProviderEncoding = base.Encoding;
					provider5List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider5List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider5List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider5List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider5List.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider6List.FromPathFile = this.PathInfrastructureFile;
					provider6List.Parms.Clear();
					provider6List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider6List.ProviderEncoding = base.Encoding;
					provider6List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider6List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider6List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider6List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider6List.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider7List.FromPathFile = this.PathInfrastructureFile;
					provider7List.Parms.Clear();
					provider7List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider7List.ProviderEncoding = base.Encoding;
					provider7List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider7List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider7List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider7List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider7List.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider8List.FromPathFile = this.PathInfrastructureFile;
					provider8List.Parms.Clear();
					provider8List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider8List.ProviderEncoding = base.Encoding;
					provider8List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider8List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider8List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider8List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider8List.Parms[ImportProviderParmEnum.FileXlsx] ="1";
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
					provider9List.FromPathFile = this.PathInfrastructureFile;
					provider9List.Parms.Clear();
					provider9List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider9List.ProviderEncoding = base.Encoding;
					provider9List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider9List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider9List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider9List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider9List.Parms[ImportProviderParmEnum.FileXlsx] = "1";
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
					provider10List.FromPathFile = this.PathInfrastructureFile;
					provider10List.Parms.Clear();
					provider10List.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider10List.ProviderEncoding = base.Encoding;
					provider10List.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider10List.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					provider10List.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					provider10List.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					provider10List.Parms[ImportProviderParmEnum.FileXlsx] ="1";
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
					providerTemplateInventory.FromPathFile = this.PathInfrastructureFile;
					providerTemplateInventory.Parms.Clear();
					providerTemplateInventory.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					providerTemplateInventory.ProviderEncoding = base.Encoding;
					providerTemplateInventory.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					providerTemplateInventory.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					providerTemplateInventory.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					providerTemplateInventory.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					providerTemplateInventory.Parms[ImportProviderParmEnum.FileXlsx] ="1";
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
					providerDecorator.FromPathFile = this.PathInfrastructureFile;
					providerDecorator.Parms.Clear();
					providerDecorator.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					providerDecorator.ProviderEncoding = base.Encoding;
					providerDecorator.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					providerDecorator.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
					providerDecorator.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
					providerDecorator.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					providerDecorator.Parms[ImportProviderParmEnum.FileXlsx] ="1";
					providerDecorator.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 16;	//TODO
					providerDecorator.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PropertyDecorator";
					StepCurrent++;
					providerDecorator.Clear();
					providerDecorator.Import();
				}

				TemporaryInventory temporaryInventory = base.GetTemporaryInventoryWithImportModuleInfo
		(Common.Constants.ImportAdapterName.ImportCatalogNativPlusXslxAdapter, "IMPORT ADAPTER NATIV + LADPC", this._fileName, updateDateTime);
				this.TemporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);   

			}
			//End ========================== From Nativ + all 
			FileLogInfo fileLogInfo = new FileLogInfo();
			////if (this._isDirectory == true)
			////{
			////	fileLogInfo.Directory = this.Path;
			////}
			////else
			////{
			fileLogInfo.File = this.PathInfrastructureFile;
			//}

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
	
			if (this.ImportPreviousInventory == true)
			{
				IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativDbSetProvider);
				provider6.ToPathDB = base.GetDbPath;
				provider6.Clear();
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