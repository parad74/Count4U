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

namespace Count4U.UpdateCatalogNativPlusXslxAdapter
{																																				  
    public class UpdateCatalogNativPlusXslxAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		//private CatalogParserPoints _catalogParserPoints;
		private CatalogParserPoints _catalogParserPoints2;
		string _branchErpCode = String.Empty;
		private bool _withQuantityERP;

		private bool _importCatalog;
		private bool _importLocation;
		private bool _importEmployee;
		private bool _importBuildingConfig;
		private bool _importPropertyDecorator;
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

		public UpdateCatalogNativPlusXslxAdapterViewModel(IServiceLocator serviceLocator,
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
			if (this.ImportCatalog == true) step++;
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
			//		 if (this.WithQuantityERP == true) step++;
			return step;
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

			//init GUI
			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;
			this._fileName = "Nativ_Infrastructure.xlsx";
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			base.Encoding = System.Text.Encoding.GetEncoding(1255);

			base.StepTotal = this.GetStep();


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
			//return base.ContinueAfterBranchERPWarning(base.Path, @"I6_(.+)\.csv");
			return true;
        }

		//См ImportCatalogNativPlusXslxViewModel
        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			if (this.ImportLocation == true)
			{																															  //LocationNativXslx2SdfParser
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationUpdateNativXslx2SdfProvider);
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
				//IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportIturUpdateNativXslx2SdfProvider);
				//provider3.ToPathDB = base.GetDbPath;
				//provider3.FastImport = base.IsTryFast;
				//provider3.FromPathFile = this.Path;
				//provider3.Parms.Clear();
				//provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider3.ProviderEncoding = base.Encoding;
				//provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				//provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
				//provider3.Parms[ImportProviderParmEnum.FileXlsx] = base.XlsxFormat ? "1" : String.Empty;
				//provider3.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 2;
				//provider3.Parms[ImportProviderParmEnum.SheetNameXlsx] = "Location";

				StepCurrent++;
				provider2.Import();

				//StepCurrent++;
				//provider3.Import();
			}

			//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogSapb1XslxUpdateERPQuentetyProvider1);
			//provider2.Clear();  //!! очистить сэмулированные barcode для IturERPCode

			//provider2.ToPathDB = base.GetDbPath;
			//provider2.FastImport = base.IsTryFast;	 
			//provider2.FromPathFile = this.Path;
			//provider2.Parms.Clear();
			//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.ProviderEncoding = base.Encoding; 
			//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

			//IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogMPLUpdateERPQuentetyProvider1);
			//provider3.ToPathDB = base.GetDbPath;
			//provider3.FastImport = base.IsTryFast;
			//provider3.FromPathFile = this.Path;
			//provider3.Parms.Clear();
			//provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider3.ProviderEncoding = base.Encoding; //this._encoding2;
			//provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			//provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			//provider3.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
			//provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;



			//this.StepCurrent = 1;
			//provider2.Import();

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
