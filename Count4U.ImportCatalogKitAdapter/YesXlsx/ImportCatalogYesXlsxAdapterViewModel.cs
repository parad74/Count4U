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

namespace Count4U.ImportCatalogYesXlsxAdapter
{
    public class ImportCatalogYesXlsxAdapterViewModel : TemplateAdapterFileFolderViewModel
    {
		public string _fileName {get; set;}
		private bool XlsxFormat;
		private string _branchErpCode = String.Empty;
		//private bool _withQuantityERP;

		private bool _importLocation;
		private bool _importCatalog;
		private bool _importPreviousInventory;
		//private bool _importPropertyStr1List;
		//private bool _importPropertyStr2List;
		//private bool _importPropertyStr3List;
		private bool _importBuildingConfig;

		public ImportCatalogYesXlsxAdapterViewModel(IServiceLocator serviceLocator,
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


		private int GetStep()
		{
			int step = 0;
			if (this.ImportCatalog == true) step++;
			if (this.ImportLocation == true) step = step + 2;
			// if (this.ImportEmployee == true)  step ++;
			//if (this.ImportBuildingConfig == true) step++;
			//if (this.ImportPropertyStr1List == true) step++;
			//if (this.ImportPropertyStr2List == true) step++;
			//if (this.ImportPropertyStr3List == true) step++;
			if (this.ImportPreviousInventory == true) step++;
			//		 if (this.WithQuantityERP == true) step++;
			return step;
		}
		//public bool WithQuantityErp
		//{
		//	get { return _withQuantityERP; }
		//	set
		//	{
		//		this._withQuantityERP = value;
		//		//if (value == true)
		//		//{
		//		//	base.StepTotal = 5;
		//		//}
		//		//else
		//		//{
		//		//	base.StepTotal = 4;
		//		//}
		//		RaisePropertyChanged(() => WithQuantityErp);

		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();
		//	}
		//}


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
						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
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
			this._fileName = FileSystem.inData;
			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;

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
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			
			 this.XlsxFormat = true;

            try
            {
                if (base._isDirectory)
                {
                    if (!Directory.Exists(this.Path))
                        Directory.CreateDirectory(this.Path);
                }
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("Create inData directory", exc);
            }
        }

		protected override bool PreImportCheck()
		{
			return true;
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

			string newSessionCode = Guid.NewGuid().ToString();

			// now the same format providerLocationQ ==  providerLocationSN


			IImportProvider providerLocationQ = this.GetProviderInstance(ImportProviderEnum.ImportLocationYesXlsxProviderQ);
			providerLocationQ.ToPathDB = base.GetDbPath;
			providerLocationQ.Parms.Clear();
			providerLocationQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerLocationQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerLocationQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerLocationQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerLocationQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerLocationQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerLocationQ.ProviderEncoding = base.Encoding;

			IImportProvider providerLocationSN = this.GetProviderInstance(ImportProviderEnum.ImportLocationYesXlsxProviderSN);
			providerLocationSN.ToPathDB = base.GetDbPath;
			providerLocationSN.Parms.Clear();
			providerLocationSN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerLocationSN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerLocationSN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerLocationSN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerLocationSN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerLocationSN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerLocationSN.ProviderEncoding = base.Encoding;

			IImportProvider providerIturQ = this.GetProviderInstance(ImportProviderEnum.ImportIturYesXlsxProviderQ);
			providerIturQ.ToPathDB = base.GetDbPath;
			providerIturQ.Parms.Clear();
			providerIturQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerIturQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerIturQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerIturQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerIturQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerIturQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerIturQ.ProviderEncoding = base.Encoding;

			IImportProvider providerIturSN = this.GetProviderInstance(ImportProviderEnum.ImportIturYesXlsxProviderSN);
			providerIturSN.ToPathDB = base.GetDbPath;
			providerIturSN.Parms.Clear();
			providerIturSN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerIturSN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerIturSN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerIturSN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerIturSN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerIturSN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerIturSN.ProviderEncoding = base.Encoding;

			IImportProvider providerDoc = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderAddFristDocToIturBlukProvider);
			providerDoc.ToPathDB = base.GetDbPath;
			providerDoc.FromPathFile = base.GetDbPath;
			providerDoc.Parms.Clear();
			providerDoc.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerDoc.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerDoc.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerDoc.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerDoc.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

			IImportProvider providerCatalogSN = this.GetProviderInstance(ImportProviderEnum.ImportCatalogYesXlsxProviderSN);
			providerCatalogSN.ToPathDB = base.GetDbPath;
			providerCatalogSN.FromPathFile = base.GetDbPath;
			providerCatalogSN.Parms.Clear();
			providerCatalogSN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerCatalogSN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerCatalogSN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerCatalogSN.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerCatalogSN.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerCatalogSN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerCatalogSN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerCatalogSN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerCatalogSN.ProviderEncoding = base.Encoding;

			IImportProvider providerCatalogQ = this.GetProviderInstance(ImportProviderEnum.ImportCatalogYesXlsxProviderQ);
			providerCatalogQ.ToPathDB = base.GetDbPath;
			providerCatalogQ.FromPathFile = base.GetDbPath;
			providerCatalogQ.Parms.Clear();
			providerCatalogQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerCatalogQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerCatalogQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerCatalogQ.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerCatalogQ.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerCatalogQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerCatalogQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerCatalogQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerCatalogQ.ProviderEncoding = base.Encoding;

			IImportProvider providerPreviousInventoryQ = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativYesDbSetProviderQ);
			providerPreviousInventoryQ.ToPathDB = base.GetDbPath;
			providerPreviousInventoryQ.FromPathFile = base.GetDbPath;
			providerPreviousInventoryQ.Parms.Clear();
			providerPreviousInventoryQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerPreviousInventoryQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerPreviousInventoryQ.ProviderEncoding = base.Encoding;

			IImportProvider providerPreviousInventorySN = this.GetProviderInstance(ImportProviderEnum.ImportPreviousInventoryNativYesDbSetProviderSN);
			providerPreviousInventorySN.ToPathDB = base.GetDbPath;
			providerPreviousInventorySN.FromPathFile = base.GetDbPath;
			providerPreviousInventorySN.Parms.Clear();
			providerPreviousInventorySN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerPreviousInventorySN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerPreviousInventorySN.ProviderEncoding = base.Encoding;

			StepCurrent = 0;
			//if (this.IsSingleFile == true)
			//{
			//	StepTotal = 6;

			//	providerLocation.FromPathFile = this.Path;
			//	// =============== Location ===================
			//	providerLocation.Import();
			//	StepCurrent++;
			//	providerLocation.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
			//	providerLocation.Parms[ImportProviderParmEnum.SheetNameXlsx] = "סריאלי";
			//	StepCurrent++;
			//	providerLocation.Import();

			//	// =============== Itur ===================
			//	providerItur.FromPathFile = this.Path;
			//	StepCurrent++;
			//	providerItur.Import();

			//	providerItur.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
			//	providerItur.Parms[ImportProviderParmEnum.SheetNameXlsx] = "סריאלי";
			//	StepCurrent++;
			//	providerItur.Import();

			//	// =============== Doc ===================
			//	StepCurrent++;
			//	providerDoc.Import();

			//}
			//else
			{
				var files = Directory.GetFiles(this.Path);
				StepTotal = files.Length * 3 + 1;		//??
				StepCurrent = 0;

				if (this.ImportLocation == true)
				{
					// =============== Location ===================
					foreach (string filePath in files)
					{
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
						if (fileTypeName.StartsWith("q_stock_") == true)
						{
							StepCurrent++;
							providerLocationQ.FromPathFile = filePath;
							providerLocationQ.Import();
						}
					}

					foreach (string filePath in files)
					{
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());

						if (fileTypeName.StartsWith("sn_stock_") == true)
						{
							StepCurrent++;
							providerLocationSN.FromPathFile = filePath;
							providerLocationSN.Import();
						}
					}


					// Clear doc
					IDocumentHeaderRepository docRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
					docRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);

					// =============== Itur ===================

					foreach (string filePath in files)
					{
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
						if (fileTypeName.StartsWith("q_stock_") == true)
						{
							StepCurrent++;
							providerIturQ.FromPathFile = filePath;
							providerIturQ.Import();
						}

					}

					foreach (string filePath in files)
					{
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());

						if (fileTypeName.StartsWith("sn_stock_") == true)
						{
							StepCurrent++;
							providerIturSN.FromPathFile = filePath;
							providerIturSN.Import();
						}
					}

					// =============== Doc ===================
					if (files.Length > 0)
					{
						StepCurrent++;
						providerDoc.Import();
					}
				}

				if (this.ImportCatalog == true)
				{

					// =============== Catalog ===================
					foreach (string filePath in files)
					{
						//Sheet 1
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
						if (fileTypeName.StartsWith("sn_stock_") == true)
						{
							providerCatalogSN.FromPathFile = filePath;
							StepCurrent++;
							providerCatalogSN.Import();
						}
					}

					foreach (string filePath in files)
					{
						//Sheet 1
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
						if (fileTypeName.StartsWith("q_stock_") == true)
						{
							providerCatalogQ.FromPathFile = filePath;
							StepCurrent++;
							providerCatalogQ.Import();
						}
					}
				}

				if (this.ImportPreviousInventory == true)
				{
					// =============== PreviousInventory ===================
					foreach (string filePath in files)
					{
						//Sheet 1
						string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
						if (fileTypeName.StartsWith("q_stock_") == true)
						{
							providerPreviousInventoryQ.FromPathFile = filePath;
							StepCurrent++;
							providerPreviousInventoryQ.Import();
						}
					}

					//foreach (string filePath in files)
					//{
					//	//Sheet 1
					//	string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
					//	if (fileTypeName.StartsWith("sn_stock_") == true)
					//	{
					//		providerPreviousInventorySN.FromPathFile = filePath;
					//		StepCurrent++;
					//		providerPreviousInventorySN.Import();
					//	}
					//}

				}
			}

			FileLogInfo fileLogInfo = new FileLogInfo();
			if (this._isDirectory == true)
			{
				fileLogInfo.Directory = base.GetImportPath().Trim('\\');
			}
			else
			{
				fileLogInfo.File = this.Path;
			}

			base.SaveFileLog(fileLogInfo);
		}

        public override void Clear()
        {
            base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogWarehouseXslxProvider);
			provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}